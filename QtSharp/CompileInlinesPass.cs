using System;
using System.IO;
using System.Text;
using CppSharp.AST;
using CppSharp.Passes;
using System.Linq;

namespace QtSharp
{
    public class CompileInlinesPass : TranslationUnitPass
    {
        private readonly string qmake;
        private readonly string make;

        public CompileInlinesPass(string qmake, string make)
        {
            this.qmake = qmake;
            this.make = make;
        }

        public override bool VisitLibrary(Library library)
        {
            bool result = base.VisitLibrary(library);
            string pro = string.Format("{0}.pro", Driver.Options.InlinesLibraryName);
            string path = Path.Combine(Driver.Options.OutputDir, pro);
            StringBuilder proBuilder = new StringBuilder();
            proBuilder.Append("QMAKE_CXXFLAGS += -fkeep-inline-functions -std=c++0x\n");
            proBuilder.AppendFormat("TARGET = {0}\n", Path.GetFileNameWithoutExtension(pro));
            proBuilder.Append("TEMPLATE = lib\n");
            proBuilder.AppendFormat("SOURCES += {0}\n", Path.ChangeExtension(pro, "cpp"));
            File.WriteAllText(path, proBuilder.ToString());
            string error;
            ProcessHelper.Run(qmake, string.Format("\"{0}\"", path), out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            ProcessHelper.Run(make, "-f Makefile.Release", out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            this.Driver.Options.LibraryDirs.Add(Path.Combine(this.Driver.Options.OutputDir, "release"));
            this.Driver.Options.Libraries.Add(string.Format("lib{0}.a", Path.GetFileNameWithoutExtension(pro)));
            this.Driver.ParseLibraries();
            NativeLibrary inlines = this.Driver.LibrarySymbols.Libraries.Last();
            foreach (string symbol in this.Driver.LibrarySymbols.Libraries.Take(
                this.Driver.LibrarySymbols.Libraries.Count - 1).SelectMany(
                    nativeLibrary => nativeLibrary.Symbols))
            {
                inlines.Symbols.Remove(symbol);
            }
            this.Driver.LibrarySymbols.IndexSymbols();
            return result;
        }
    }
}
