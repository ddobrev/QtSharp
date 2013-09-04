using System;
using System.Collections.Generic;
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
            string makePath = args[1];
            if (!File.Exists(makePath))
            {
                Console.WriteLine("The specified make does not exist.");
                return -1;
            }
            string error;
            string libs = ProcessHelper.Run(qmake, "-query QT_INSTALL_LIBS", out error);
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
            IEnumerable<string> libFiles = GetLibFiles(libsInfo);
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
            foreach (string libFile in libFiles)
            {
                if (libFile == "libQt5Core.a")
                {
                    string module = Regex.Match(libFile, @"Qt\d?(?<module>\w+)\.\w+$").Groups["module"].Value;
                    ConsoleDriver.Run(new QtSharp(qmake, makePath, headers, module, libs, libFile));
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
    }
}
