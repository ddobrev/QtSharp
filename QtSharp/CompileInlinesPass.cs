using System;
using System.IO;
using System.Text;
using CppSharp.AST;
using CppSharp.Parser;
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
            proBuilder.Append("QT += widgets\n");
            // HACK: work around https://bugreports.qt.io/browse/QTBUG-47569
            if (this.Driver.Options.InlinesLibraryName.StartsWith("QtWidgets"))
            {
                proBuilder.Append("DEFINES += QT_NO_ACCESSIBILITY\n");
            }
            proBuilder.Append("QMAKE_CXXFLAGS += -fkeep-inline-functions -std=c++0x\n");
            proBuilder.AppendFormat("TARGET = {0}\n", this.Driver.Options.InlinesLibraryName);
            proBuilder.Append("TEMPLATE = lib\n");
            proBuilder.AppendFormat("SOURCES += {0}\n", Path.ChangeExtension(pro, "cpp"));
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                proBuilder.Append("LIBS += -loleaut32 -lole32");
            }
            File.WriteAllText(path, proBuilder.ToString());
            string error;
            ProcessHelper.Run(this.qmake, string.Format("\"{0}\"", path), out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            ProcessHelper.Run(this.make, "-f Makefile.Release", out error, true);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            var parserOptions = new ParserOptions();
            parserOptions.addLibraryDirs(Path.Combine(this.Driver.Options.OutputDir, "release"));
            parserOptions.FileName = Path.GetFileName(string.Format("lib{0}.a", Path.GetFileNameWithoutExtension(pro)));
            var parserResult = ClangParser.ParseLibrary(parserOptions);
            if (parserResult.Kind == ParserResultKind.Success)
            {
                var nativeLibrary = CppSharp.ClangParser.ConvertLibrary(parserResult.Library);
                this.Driver.Symbols.Libraries.Add(nativeLibrary);
                this.Driver.Symbols.IndexSymbols();
            }
            return result;
        }
    }
}
