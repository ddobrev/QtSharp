using System;
using System.IO;
using System.Linq;
using System.Text;
using CppSharp.AST;
using CppSharp.Passes;
using CppSharp;
using CppSharp.Parser;

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

        public override bool VisitLibrary(ASTContext context)
        {
            string error;
            const string qtVersionVariable = "QT_VERSION";
            var qtVersion = ProcessHelper.Run(this.qmake, string.Format("-query {0}", qtVersionVariable), out error);
            var qtVersionFile = Path.Combine(this.Context.Options.OutputDir, qtVersionVariable);
            var qtVersionFileInfo = new FileInfo(qtVersionFile);
            var text = string.Empty;
            if (!qtVersionFileInfo.Exists || (text = File.ReadAllText(qtVersionFile)) != qtVersion)
            {
                File.WriteAllText(qtVersionFile, qtVersion);
                qtVersionFileInfo = new FileInfo(qtVersionFile);
            }
            var dir = Platform.IsMacOS ? this.Context.Options.OutputDir : Path.Combine(this.Context.Options.OutputDir, "release");
            var inlines = Path.GetFileName(string.Format("{0}Qt-inlines.{1}", Platform.IsWindows ? string.Empty : "lib",
                Platform.IsWindows ? "dll" : Platform.IsMacOS ? "dylib" : "so"));
            var libFile = Path.Combine(dir, inlines);
            var inlinesFileInfo = new FileInfo(libFile);
            if (!inlinesFileInfo.Exists || qtVersionFileInfo.LastWriteTimeUtc > inlinesFileInfo.LastWriteTimeUtc)
            {
                if (!this.CompileInlines())
                {
                    return false;
                }
            }
            var parserOptions = new ParserOptions();
            parserOptions.addLibraryDirs(dir);
            parserOptions.LibraryFile = inlines;
            using (var parserResult = CppSharp.Parser.ClangParser.ParseLibrary(parserOptions))
            {
                if (parserResult.Kind == ParserResultKind.Success)
                {
                    var nativeLibrary = CppSharp.ClangParser.ConvertLibrary(parserResult.Library);
                    this.Context.Symbols.Libraries.Add(nativeLibrary);
                    this.Context.Symbols.IndexSymbols();
                    parserResult.Library.Dispose();
                }
            }
            return true;
        }

        private bool CompileInlines()
        {
            var pro = string.Format("Qt-inlines.pro");
            var path = Path.Combine(this.Context.Options.OutputDir, pro);
            var proBuilder = new StringBuilder();
            var qtModules = string.Join(" ", from module in this.Context.Options.Modules
                                             from header in module.Headers
                                             where !header.EndsWith(".h", StringComparison.Ordinal)
                                             select header.Substring("Qt".Length).ToLowerInvariant());
            // QtTest is only library which has a "lib" suffix to its module alias for qmake
            qtModules = qtModules.Replace(" test ", " testlib ");

            proBuilder.AppendFormat("QT += {0}\n", qtModules);
            proBuilder.Append("CONFIG += c++11\n");
            proBuilder.Append("QMAKE_CXXFLAGS += -fkeep-inline-functions\n");
            proBuilder.AppendFormat("TARGET = Qt-inlines\n");
            proBuilder.Append("TEMPLATE = lib\n");
            proBuilder.AppendFormat("SOURCES += {0}\n", string.Join(" ", this.Context.Options.Modules.Select(m => m.InlinesLibraryName + ".cpp")));
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                proBuilder.Append("LIBS += -loleaut32 -lole32");
            }
            File.WriteAllText(path, proBuilder.ToString());
            string error;
            // HACK: Clang does not support -fkeep-inline-functions so force compilation with (the real) GCC on OS X
            ProcessHelper.Run(this.qmake, string.Format("{0}\"{1}\"", Platform.IsMacOS ? "-spec macx-g++ " : string.Empty, path), out error);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            var makefile = File.Exists(Path.Combine(this.Context.Options.OutputDir, "Makefile.Release")) ? "Makefile.Release" : "Makefile";
            if (Platform.IsMacOS)
            {
                // HACK: Clang does not support -fkeep-inline-functions so force compilation with (the real) GCC on OS X
                var makefilePath = Path.Combine(this.Context.Options.OutputDir, makefile);
                var script = new StringBuilder(File.ReadAllText(makefilePath));
                var xcodePath = XcodeToolchain.GetXcodePath();
                script.Replace(Path.Combine(xcodePath, "Contents", "Developer", "usr", "bin", "gcc"), "/usr/local/bin/gcc");
                script.Replace(Path.Combine(xcodePath, "Contents", "Developer", "usr", "bin", "g++"), "/usr/local/bin/g++");
                File.WriteAllText(makefilePath, script.ToString());
            }
            ProcessHelper.Run(this.make, string.Format("-j{0} -f {1}", Environment.ProcessorCount + 1, makefile), out error, true);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
                return false;
            }
            return true;
        }
    }
}
