using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CppSharp;
using CppSharp.Parser;
using ClangParser = CppSharp.Parser.ClangParser;

namespace QtSharp.CLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please enter the paths to qmake and make.");
                return 0;
            }
            string qmake = args[0];
            if (!File.Exists(qmake))
            {
                Console.WriteLine("The specified qmake does not exist.");
                return -1;
            }
            string make = args[1];
            if (!File.Exists(make))
            {
                Console.WriteLine("The specified make does not exist.");
                return -1;
            }

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
            path = Path.GetDirectoryName(make) + Path.PathSeparator + path;
            Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.Process);

            string error;
            string libs = ProcessHelper.Run(qmake, "-query QT_INSTALL_BINS", out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return -1;
            }
            DirectoryInfo libsInfo = new DirectoryInfo(libs);
            if (!libsInfo.Exists)
            {
                Console.WriteLine(
                    "The directory \"{0}\" that qmake returned as the lib direcory of the Qt installation, does not exist.",
                    libsInfo.Name);
                return -1;
            }
            ICollection<string> libFiles = GetLibFiles(libsInfo);
            string headers = ProcessHelper.Run(qmake, "-query QT_INSTALL_HEADERS", out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return -1;
            }
            DirectoryInfo headersInfo = new DirectoryInfo(headers);
            if (!headersInfo.Exists)
            {
                Console.WriteLine(
                    "The directory \"{0}\" that qmake returned as the header direcory of the Qt installation, does not exist.",
                    headersInfo.Name);
                return -1;
            }
            string docs = ProcessHelper.Run(qmake, "-query QT_INSTALL_DOCS", out error);
            string emptyFile = Environment.OSVersion.Platform == PlatformID.Win32NT ? "NUL" : "/dev/null";
            string output;
            ProcessHelper.Run("gcc", string.Format("-v -E -x c++ {0}", emptyFile), out output);
            string target = Regex.Match(output, @"Target:\s*(?<target>[^\r\n]+)").Groups["target"].Value;
            const string includeDirsRegex = @"#include <\.\.\.> search starts here:(?<includes>.+)End of search list";
            string allIncludes = Regex.Match(output, includeDirsRegex, RegexOptions.Singleline).Groups["includes"].Value;
            var systemIncludeDirs = allIncludes.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(Path.GetFullPath);
            Dictionary<string, IEnumerable<string>> dependencies = new Dictionary<string, IEnumerable<string>>();
            var parserOptions = new ParserOptions();
            parserOptions.addLibraryDirs(libs);
            foreach (var libFile in libFiles)
            {
                parserOptions.FileName = libFile;
                var parserResult = ClangParser.ParseLibrary(parserOptions);
                if (parserResult.Kind == ParserResultKind.Success)
                {
                    dependencies[libFile] = CppSharp.ClangParser.ConvertLibrary(parserResult.Library).Dependencies;
                }
                else
                {
                    dependencies[libFile] = Enumerable.Empty<string>();
                }
            }
            var wrappedModules = new List<string> { "Qt5Core.dll", "Qt5Gui.dll", "Qt5Widgets.dll" };
            libFiles = libFiles.TopologicalSort(l => dependencies.ContainsKey(l) ? dependencies[l] : Enumerable.Empty<string>());
            foreach (var libFile in libFiles.Where(libFile => wrappedModules.Contains(libFile)))
            {
                ConsoleDriver.Run(new QtSharp(qmake, make, headers, libs, libFile, target, systemIncludeDirs, docs));
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
            return 0;
        }

        private static IList<string> GetLibFiles(DirectoryInfo libsInfo)
        {
            List<string> modules = (from file in libsInfo.EnumerateFiles()
                                    where Regex.IsMatch(file.Name, @"^Qt\d?\w+\.\w+$")
                                    select file.Name).ToList();
            for (int i = modules.Count - 1; i >= 0; i--)
            {
                modules.Remove(Path.GetFileNameWithoutExtension(modules[i]) + "d" + Path.GetExtension(modules[i]));
            }
            return modules;
        }
    }
}
