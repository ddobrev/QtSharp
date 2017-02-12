using System;
using System.IO;
using System.Linq;
using System.Text;
using CppSharp.AST;
using CppSharp.Passes;
using CppSharp;
using CppSharp.Parser;
using System.Threading;

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

        public override bool VisitASTContext(ASTContext context)
        {
            var dir = Platform.IsMacOS ? this.Context.Options.OutputDir : Path.Combine(this.Context.Options.OutputDir, "release");
            var findSymbolsPass = this.Context.TranslationUnitPasses.FindPass<FindSymbolsPass>();
            findSymbolsPass.Wait = true;
            remainingCompilationTasks = this.Context.Options.Modules.Count;
            foreach (var module in this.Context.Options.Modules)
            {
                var inlines = Path.GetFileName(string.Format("{0}{1}.{2}", Platform.IsWindows ? string.Empty : "lib",
                    module.InlinesLibraryName, Platform.IsMacOS ? "dylib" : "dll"));
                this.CompileInlines(module, dir, inlines);
            }
            return true;
        }

        private void CompileInlines(Module module, string dir, string inlines)
        {
            var pro = string.Format("{0}.pro", module.InlinesLibraryName);
            var path = Path.Combine(this.Context.Options.OutputDir, pro);
            var proBuilder = new StringBuilder();
            var qtModules = string.Join(" ", from header in module.Headers
                                             where !header.EndsWith(".h", StringComparison.Ordinal)
                                             select header.Substring("Qt".Length).ToLowerInvariant());
            // QtTest is only library which has a "lib" suffix to its module alias for qmake
            if (qtModules == "test")
            {
                qtModules += "lib";
            }

            proBuilder.AppendFormat("QT += {0}\n", qtModules);
            proBuilder.Append("CONFIG += c++11\n");
            proBuilder.AppendFormat("TARGET = {0}\n", module.InlinesLibraryName);
            proBuilder.Append("TEMPLATE = lib\n");
            proBuilder.AppendFormat("SOURCES += {0}\n", Path.ChangeExtension(pro, "cpp"));
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                proBuilder.Append("LIBS += -loleaut32 -lole32");
            }
            File.WriteAllText(path, proBuilder.ToString());
            // HACK: work around https://bugreports.qt.io/browse/QTBUG-55952
            if (module.LibraryName == "Qt3DRender.Sharp")
            {
                var cpp = Path.ChangeExtension(pro, "cpp");
                var unlinkable = new[]
                                 {
                                     "&Qt3DRender::QSortCriterion::tr;",
                                     "&Qt3DRender::QSortCriterion::trUtf8;",
                                     "&Qt3DRender::qt_getEnumMetaObject;"
                                 };
                var linkable = (from line in File.ReadLines(cpp)
                                where unlinkable.All(ul => !line.EndsWith(ul, StringComparison.Ordinal))
                                select line).ToList();
                File.WriteAllLines(cpp, linkable);
            }
            string error;
            ProcessHelper.Run(this.qmake, $"\"{path}\"", out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return;
            }
            var makefile = File.Exists(Path.Combine(this.Context.Options.OutputDir, "Makefile.Release")) ? "Makefile.Release" : "Makefile";
            InvokeCompiler(dir, inlines, makefile);
        }

        private void InvokeCompiler(string dir, string inlines, string makefile)
        {
            new Thread(() =>
            {
                try
                {
                    string error;
                    ProcessHelper.Run(this.make, $"-f {makefile}", out error);
                    if (string.IsNullOrEmpty(error))
                    {
                        var parserOptions = new ParserOptions();
                        parserOptions.AddLibraryDirs(dir);
                        parserOptions.LibraryFile = inlines;
                        using (var parserResult = CppSharp.Parser.ClangParser.ParseLibrary(parserOptions))
                        {
                            if (parserResult.Kind == ParserResultKind.Success)
                            {
                                var nativeLibrary = CppSharp.ClangParser.ConvertLibrary(parserResult.Library);
                                lock (@lock)
                                {
                                    this.Context.Symbols.Libraries.Add(nativeLibrary);
                                    this.Context.Symbols.IndexSymbols();
                                }
                                parserResult.Library.Dispose();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(error);
                    }
                }
                finally
                {
                    RemainingCompilationTasks--;
                }
            }).Start();
        }

        private int RemainingCompilationTasks
        {
            get
            {
                return remainingCompilationTasks;
            }
            set
            {
                if (remainingCompilationTasks != value)
                {
                    remainingCompilationTasks = value;
                    if (remainingCompilationTasks == 0)
                    {
                        var findSymbolsPass = this.Context.TranslationUnitPasses.FindPass<FindSymbolsPass>();
                        findSymbolsPass.Wait = false;
                    }
                }
            }
        }

        private int remainingCompilationTasks;
        private static readonly object @lock = new object();
    }
}
