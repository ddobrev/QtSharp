using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using CppSharp;
using CppSharp.AST;
using CppSharp.Parser;
using ClangParser = CppSharp.Parser.ClangParser;

namespace QtSharp.CLI
{
    public class Program
    {
        static int ParseArgs(string[] args, out string qmake, out string make, out bool debug)
        {
            qmake = null;
            make = null;
            debug = false;

            if (args.Length < 2)
            {
               Console.WriteLine("Please enter the paths to qmake and make.");
               return 1;
            }

            qmake = args [0];
            if (!File.Exists(qmake))
            {
                Console.WriteLine("The specified qmake does not exist.");
                return 1;
            }

            make = args [1];
            if (!File.Exists(make))
            {
               Console.WriteLine("The specified make does not exist.");
               return 1;
            }

            debug = args.Length > 2 && (args[2] == "d" || args[2] == "debug");

            return 0;
        }

        class QtVersion
        {
            public int MajorVersion;
            public int MinorVersion;
            public string Path;
            public string Target;
            public string Docs;
            public string QMake;
            public string Make;
            public string Libs;
            public string Headers;
            public IEnumerable<string> LibFiles;
            public IEnumerable<string> SystemIncludeDirs;
        }

        static List<QtVersion> FindQt()
        {
            var home = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var qts = new List<QtVersion>();

            foreach (var path in Directory.EnumerateDirectories(Path.Combine(home, "Qt")))
            {
                var dir = Path.GetFileName(path);
                bool isNumber = dir.All(c => char.IsDigit(c) || c == '.');
                if (!isNumber)
                    continue;
                var qt = new QtVersion { Path = path };
                var match = Regex.Match(dir, @"([0-9]+)\.([0-9]+)");
                if (!match.Success)
                    continue;
                qt.MajorVersion = int.Parse(match.Groups[1].Value);
                qt.MinorVersion = int.Parse(match.Groups[2].Value);
                qts.Add(qt);
            }

            return qts;
        }

        static bool QueryQt(QtVersion qt, bool debug)
        {
            qt.QMake = Path.Combine(qt.Path, "clang_64/bin/qmake");
            qt.Make = "/usr/bin/make";

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
            path = Path.GetDirectoryName(qt.Make) + Path.PathSeparator + path;
            Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.Process);

            string error;
            var queryLibs = Platform.IsWindows ? "QT_INSTALL_BINS" : "QT_INSTALL_LIBS";
            qt.Libs = ProcessHelper.Run(qt.QMake, string.Format("-query {0}", queryLibs), out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            DirectoryInfo libsInfo = new DirectoryInfo(qt.Libs);
            if (!libsInfo.Exists)
            {
                Console.WriteLine(
                    "The directory \"{0}\" that qmake returned as the lib directory of the Qt installation, does not exist.",
                    libsInfo.Name);
                return false;
            }
            qt.LibFiles = GetLibFiles(libsInfo, debug);
            qt.Headers = ProcessHelper.Run(qt.QMake, "-query QT_INSTALL_HEADERS", out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            DirectoryInfo headersInfo = new DirectoryInfo(qt.Headers);
            if (!headersInfo.Exists)
            {
                Console.WriteLine(
                    "The directory \"{0}\" that qmake returned as the header direcory of the Qt installation, does not exist.",
                    headersInfo.Name);
                return false;
            }
            qt.Docs = ProcessHelper.Run(qt.QMake, "-query QT_INSTALL_DOCS", out error);

            string emptyFile = Platform.IsWindows ? "NUL" : "/dev/null";
            string output;
            ProcessHelper.Run("gcc", string.Format("-v -E -x c++ {0}", emptyFile), out output);
            qt.Target = Regex.Match(output, @"Target:\s*(?<target>[^\r\n]+)").Groups["target"].Value;

            const string includeDirsRegex = @"#include <\.\.\.> search starts here:(?<includes>.+)End of search list";
            string allIncludes = Regex.Match(output, includeDirsRegex, RegexOptions.Singleline).Groups["includes"].Value;
            qt.SystemIncludeDirs = allIncludes.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(Path.GetFullPath);            

            return true;
        }

        public static int Main(string[] args)
        {
            var qts = FindQt();
            bool found = qts.Count != 0;
            bool debug = false;
            QtVersion qt;

            if (!found)
            {
                qt = new QtVersion();

                var result = ParseArgs(args, out qt.Make, out qt.QMake, out debug);
                if (result != 0)
                    return result;
            }
            else
            {
                // TODO: Only for OSX for now, generalize for all platforms.
                qt = qts.Last();
            }

            ConsoleLogger logredirect = new ConsoleLogger();
            logredirect.CreateLogDirectory();

            if (!QueryQt(qt, debug))
                return 1;

            Dictionary<string, IEnumerable<string>> dependencies = new Dictionary<string, IEnumerable<string>>();
            var parserOptions = new ParserOptions();
            parserOptions.addLibraryDirs(qt.Libs);
            foreach (var libFile in qt.LibFiles)
            {
                parserOptions.FileName = libFile;
                using (var parserResult = ClangParser.ParseLibrary(parserOptions))
                {
                    if (parserResult.Kind == ParserResultKind.Success)
                    {
                        dependencies[libFile] = CppSharp.ClangParser.ConvertLibrary(parserResult.Library).Dependencies;
                        parserResult.Library.Dispose();
                    }
                    else
                    {
                        dependencies[libFile] = Enumerable.Empty<string>();
                    }
                }
            }
            var modules = new List<string>
                          {
                              "Qt5Core",
                              "Qt5Gui",
                              "Qt5Widgets",
                              "Qt5Xml",
                              "Qt5Designer",
                              "Qt5Network",
                              "Qt5Qml",
                              "Qt5Nfc",
                              "Qt5OpenGL",
                              "Qt5ScriptTools",
                              "Qt5Sensors",
                              "Qt5SerialPort",
                              "Qt5Svg",
                              "Qt5Multimedia",
                              "Qt5MultimediaWidgets",
                              "Qt5Quick",
                              "Qt5QuickWidgets"
                          };
            if (debug)
            {
                for (var i = 0; i < modules.Count; i++)
                {
                    modules[i] += "d";
                }
            }
            qt.LibFiles = qt.LibFiles.ToList().TopologicalSort(l => dependencies.ContainsKey(l) ? dependencies[l] : Enumerable.Empty<string>());
            var wrappedModules = new List<KeyValuePair<string, string>>(modules.Count);
            foreach (var libFile in qt.LibFiles.Where(l => modules.Any(m => m == Path.GetFileNameWithoutExtension(l))))
            {
                logredirect.SetLogFile(libFile.Replace(".dll", "") + "Log.txt");
                logredirect.Start();

                var qtSharp = new QtSharp(new QtModuleInfo(qt.QMake, qt.Make, qt.Headers, qt.Libs, libFile,
                    qt.Target, qt.SystemIncludeDirs, qt.Docs));
                ConsoleDriver.Run(qtSharp);
                if (File.Exists(qtSharp.LibraryName) && File.Exists(Path.Combine("release", qtSharp.InlinesLibraryName)))
                {
                    wrappedModules.Add(new KeyValuePair<string, string>(qtSharp.LibraryName, qtSharp.InlinesLibraryName));
                }
                logredirect.Stop();
            }

#if DEBUG
            if (File.Exists("../../../QtSharp.Tests/bin/Debug/QtCore-inlines.dll"))
            {
                File.Delete("../../../QtSharp.Tests/bin/Debug/QtCore-inlines.dll");
            }
			File.Copy("release/QtCore-inlines.dll", "../../../QtSharp.Tests/bin/Debug/QtCore-inlines.dll");
#else
            if (File.Exists("../../../QtSharp.Tests/bin/Release/QtCore-inlines.dll"))
            {
                File.Delete("../../../QtSharp.Tests/bin/Release/QtCore-inlines.dll");
            }
			System.IO.File.Copy("release/QtCore-inlines.dll", "../../../QtSharp.Tests/bin/Release/QtCore-inlines.dll");
#endif
            if (wrappedModules.Count == 0)
            {
                Console.WriteLine("Generation failed.");
                return 1;
            }

            var qtSharpZip = "QtSharp.zip";
            if (File.Exists(qtSharpZip))
            {
                File.Delete(qtSharpZip);
            }
            using (var zip = File.Create(qtSharpZip))
            {
                using (var zipArchive = new ZipArchive(zip, ZipArchiveMode.Create))
                {
                    foreach (var wrappedModule in wrappedModules)
                    {
                        zipArchive.CreateEntryFromFile(wrappedModule.Key, wrappedModule.Key);
                        var documentation = Path.ChangeExtension(wrappedModule.Key, "xml");
                        zipArchive.CreateEntryFromFile(documentation, documentation);
                        zipArchive.CreateEntryFromFile(Path.Combine("release", wrappedModule.Value), wrappedModule.Value);
                    }
                    zipArchive.CreateEntryFromFile("CppSharp.Runtime.dll", "CppSharp.Runtime.dll");
                }
            }

            return 0;
        }

        private static IList<string> GetLibFiles(DirectoryInfo libsInfo, bool debug)
        {
            List<string> modules = (from file in libsInfo.EnumerateFiles()
                                    where Regex.IsMatch(file.Name, @"^Qt\d?\w+\.\w+$")
                                    select file.Name).ToList();
            for (var i = modules.Count - 1; i >= 0; i--)
            {
                var module = Path.GetFileNameWithoutExtension(modules[i]);
                if (debug && module != null && !module.EndsWith("d"))
                {
                    modules.Remove(module + Path.GetExtension(modules[i]));                    
                }
                else
                {
                    modules.Remove(module + "d" + Path.GetExtension(modules[i]));                    
                }
            }
            return modules;
        }
    }
}
