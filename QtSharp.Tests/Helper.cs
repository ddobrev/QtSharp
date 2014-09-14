using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using QtCore;

namespace QtSharp.Tests
{
    public static class Helper
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string RandomString(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var builder = new StringBuilder();
            char ch;

            for (var i = 0; i < size; i++)
            {
                ch = (char)random.Next('0', 'z');
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string[] GenerateQtArgv(string[] args)
        {
            var argv = new string[args.Length + 1];

            var a = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var attrs = a.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attrs.Length > 0)
            {
                argv[0] = ((AssemblyTitleAttribute)attrs[0]).Title;
            }
            else
            {
                var info = new QFileInfo(a.Location);
                argv[0] = info.BaseName;
            }
            args.CopyTo(argv, 1);

            return argv;
        }

        public unsafe static QCoreApplication CreateQCoreApplicationInstance(string[] args)
        {
            var argv = GenerateQtArgv(args);
            var argc = argv.Count();

            var arguments = argv.Aggregate("", (current, arg) => current + (arg + "\0"));

            var stringPointer = (char*)Marshal.StringToHGlobalAuto(arguments).ToPointer();

            Marshal.FreeHGlobal(new IntPtr(stringPointer));

            var app = new QCoreApplication(&argc, null);
            //var app = new QCoreApplication(&argc, &stringPointer);

            Marshal.FreeHGlobal(new IntPtr(stringPointer));

            return app;
        }
    }
}