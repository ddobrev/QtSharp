using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CppSharp;

namespace QtSharp.CLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter the path to qmake.");
                return 0;
            }
            string qmake = args[0];
            if (!File.Exists(qmake))
            {
                Console.WriteLine("The specified qmake does not exist.");
                return -1;
            }
            int exitCode;
            string libs = RunQMake(qmake, out exitCode, "QT_INSTALL_LIBS");
            if (exitCode != 0)
            {
                return exitCode;
            }
            DirectoryInfo libsInfo = new DirectoryInfo(libs);
            if (!libsInfo.Exists)
            {
                Console.WriteLine(
                    "The directory \"{0}\" that qmake returned as the lib direcory of the Qt installation, does not exist.",
                    libsInfo.Name);
                return -1;
            }
            IEnumerable<string> libFiles = GetLibFiles(libsInfo);
            string headers = RunQMake(qmake, out exitCode, "QT_INSTALL_HEADERS");
            if (exitCode != 0)
            {
                return exitCode;
            }
            DirectoryInfo headersInfo = new DirectoryInfo(headers);
            if (!headersInfo.Exists)
            {
                Console.WriteLine(
                    "The directory \"{0}\" that qmake returned as the header direcory of the Qt installation, does not exist.",
                    headersInfo.Name);
                return -1;
            }
            foreach (string libFile in libFiles)
            {
                if (libFile == "libQt5Core.a")
                {
                    ConsoleDriver.Run(new QtSharp(headers, Regex.Match(libFile, @"Qt\d?(?<module>\w+)\.\w+$").Groups["module"].Value, libs, libFile));                    
                }
            }
            return 0;
        }

        private static IEnumerable<string> GetLibFiles(DirectoryInfo libsInfo)
        {
            List<string> modules = (from file in libsInfo.EnumerateFiles()
                                    where Regex.IsMatch(file.Name, @"^libQt\d?\w+\.\w+$")
                                    select file.Name).ToList();
            for (int i = modules.Count - 1; i >= 0; i--)
            {
                modules.Remove(Path.GetFileNameWithoutExtension(modules[i]) + "d" + Path.GetExtension(modules[i]));
            }
            return modules;
        }

        private static string RunQMake(string qmake, out int exitCode, string qmakeVariable)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = qmake;
                    process.StartInfo.Arguments = string.Format("-query {0}", qmakeVariable);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    process.WaitForExit(1000);
                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine("qmake failed with exit code {0}.", process.ExitCode);
                        exitCode = process.ExitCode;
                        return string.Empty;
                    }
                    exitCode = 0;
                    return process.StandardOutput.ReadToEnd().Trim().Replace(@"\\", @"\");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Calling qmake caused an exception: {0}.", exception.Message);
                exitCode = 2;
                return string.Empty;
            }
        }
    }
}
