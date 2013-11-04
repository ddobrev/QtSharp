using System;
using System.IO;
using System.Linq;
using System.Text;
using CppSharp.AST;
using CppSharp.Passes;

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

        public override bool VisitLibrary(ASTContext library)
        {
            bool result = base.VisitLibrary(library);
            string pro = string.Format("{0}.pro", this.Driver.Options.InlinesLibraryName);
            string path = Path.Combine(this.Driver.Options.OutputDir, pro);
            StringBuilder proBuilder = new StringBuilder();
            proBuilder.Append("QMAKE_CXXFLAGS += -fkeep-inline-functions -std=c++0x\n");
            proBuilder.AppendFormat("TARGET = {0}\n", Path.GetFileNameWithoutExtension(pro));
            proBuilder.Append("TEMPLATE = lib\n");
            proBuilder.AppendFormat("SOURCES += {0}\n", Path.ChangeExtension(pro, "cpp"));
            File.WriteAllText(path, proBuilder.ToString());
            string error;
            ProcessHelper.Run(this.qmake, string.Format("\"{0}\"", path), out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            ProcessHelper.Run(this.make, "-f Makefile.Release", out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            this.Driver.Options.LibraryDirs.Add(Path.Combine(this.Driver.Options.OutputDir, "release"));
            this.Driver.Options.Libraries.Add(string.Format("lib{0}.a", Path.GetFileNameWithoutExtension(pro)));
            this.Driver.ParseLibraries();
            NativeLibrary inlines = this.Driver.Symbols.Libraries.Last();
            foreach (string symbol in this.Driver.Symbols.Libraries.Take(
                this.Driver.Symbols.Libraries.Count - 1).SelectMany(
                    nativeLibrary => nativeLibrary.Symbols))
            {
                inlines.Symbols.Remove(symbol);
            }
            this.Driver.Symbols.IndexSymbols();
            return result;
        }
    }
}
