using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CppSharp;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Passes;
using CppSharp.Utils;
using CppAbi = CppSharp.Parser.AST.CppAbi;

namespace QtSharp
{
    public class QtSharp : ILibrary
    {
        public QtSharp(QtInfo qtInfo)
        {
            this.qtInfo = qtInfo;
        }

        public ICollection<KeyValuePair<string, string>> GetVerifiedWrappedModules()
        {
            for (int i = this.wrappedModules.Count - 1; i >= 0; i--)
            {
                var wrappedModule = this.wrappedModules[i];
                if (!File.Exists(wrappedModule.Key) || !File.Exists(wrappedModule.Value))
                {
                    this.wrappedModules.RemoveAt(i);
                }
            }
            return this.wrappedModules;
        }

        public void Preprocess(Driver driver, ASTContext lib)
        {
            foreach (var unit in lib.TranslationUnits.Where(u => u.IsValid))
            {
                IgnorePrivateDeclarations(unit);
            }

            // QString is type-mapped to string so we only need two methods for the conversion
            var qString = lib.FindCompleteClass("QString");
            foreach (var @class in qString.Declarations)
            {
                @class.ExplicitlyIgnore();
            }
            foreach (var method in qString.Methods.Where(m => m.OriginalName != "utf16" && m.OriginalName != "fromUtf16"))
            {
                method.ExplicitlyIgnore();
            }

            // HACK: work around https://github.com/mono/CppSharp/issues/594
            lib.FindCompleteClass("QGraphicsItem").FindEnum("Extension").Access = AccessSpecifier.Public;
            lib.FindCompleteClass("QAbstractSlider").FindEnum("SliderChange").Access = AccessSpecifier.Public;
            lib.FindCompleteClass("QAbstractItemView").FindEnum("CursorAction").Access = AccessSpecifier.Public;
            lib.FindCompleteClass("QAbstractItemView").FindEnum("State").Access = AccessSpecifier.Public;
            lib.FindCompleteClass("QAbstractItemView").FindEnum("DropIndicatorPosition").Access = AccessSpecifier.Public;
            var classesWithTypeEnums = new[]
                                       {
                                           "QGraphicsEllipseItem", "QGraphicsItemGroup", "QGraphicsLineItem",
                                           "QGraphicsPathItem", "QGraphicsPixmapItem", "QGraphicsPolygonItem",
                                           "QGraphicsProxyWidget", "QGraphicsRectItem", "QGraphicsSimpleTextItem",
                                           "QGraphicsTextItem", "QGraphicsWidget", "QGraphicsSvgItem"
                                       };
            foreach (var enumeration in from @class in classesWithTypeEnums
                                        from @enum in lib.FindCompleteClass(@class).Enums
                                        where string.IsNullOrEmpty(@enum.Name)
                                        select @enum)
            {
                enumeration.Name = "TypeEnum";
            }

            // HACK: work around https://github.com/mono/CppSharp/issues/692
            foreach (var name in new[] { "QImage", "QPixmap" })
            {
                var @class = lib.FindCompleteClass(name);
                var ctorWithArray = @class.Constructors.FirstOrDefault(
                    c => c.Parameters.Count == 1 && c.Parameters[0].Type.Desugar() is ArrayType);
                if (ctorWithArray != null)
                {
                    ctorWithArray.ExplicitlyIgnore();
                }
            }
            foreach (var name in new[] { "QGraphicsScene", "QGraphicsView" })
            {
                var @class = lib.FindCompleteClass(name);
                var drawItems = @class.Methods.FirstOrDefault(m => m.OriginalName == "drawItems");
                if (drawItems != null)
                {
                    drawItems.ExplicitlyIgnore();
                }
            }
            lib.FindCompleteClass("QAbstractPlanarVideoBuffer").ExplicitlyIgnore();
            var qAbstractVideoBuffer = lib.FindCompleteClass("QAbstractVideoBuffer");
            var mapPlanes = qAbstractVideoBuffer.Methods.FirstOrDefault(m => m.OriginalName == "mapPlanes");
            if (mapPlanes != null)
            {
                mapPlanes.ExplicitlyIgnore();
            }
        }

        public void Postprocess(Driver driver, ASTContext lib)
        {
            new ClearCommentsPass().VisitASTContext(driver.Context.ASTContext);
            var modules = this.qtInfo.LibFiles.Select(l => GetModuleNameFromLibFile(l));
            var s = System.Diagnostics.Stopwatch.StartNew();
            new GetCommentsFromQtDocsPass(this.qtInfo.Docs, modules).VisitASTContext(driver.Context.ASTContext);
            System.Console.WriteLine("Documentation done in: {0}", s.Elapsed);

            var qChar = lib.FindCompleteClass("QChar");
            var op = qChar.FindOperator(CXXOperatorKind.ExplicitConversion)
                .FirstOrDefault(o => o.Parameters[0].Type.IsPrimitiveType(PrimitiveType.Char));
            if (op != null)
                op.ExplicitlyIgnore();
            op = qChar.FindOperator(CXXOperatorKind.Conversion)
                .FirstOrDefault(o => o.Parameters[0].Type.IsPrimitiveType(PrimitiveType.Int));
            if (op != null)
                op.ExplicitlyIgnore();
            // QString is type-mapped to string so we only need two methods for the conversion
            // go through the methods a second time to ignore free operators moved to the class
            var qString = lib.FindCompleteClass("QString");
            foreach (var method in qString.Methods.Where(
                m => !m.Ignore && m.OriginalName != "utf16" && m.OriginalName != "fromUtf16"))
            {
                method.ExplicitlyIgnore();
            }

            foreach (var module in driver.Options.Modules)
            {
                var prefix = Platform.IsWindows ? string.Empty : "lib";
                var extension = Platform.IsWindows ? ".dll" : Platform.IsMacOS ? ".dylib" : ".so";
                var inlinesLibraryFile = string.Format("{0}{1}{2}", prefix, module.InlinesLibraryName, extension);
                var inlinesLibraryPath = Path.Combine(driver.Options.OutputDir, Platform.IsWindows ? "release" : string.Empty, inlinesLibraryFile);
                this.wrappedModules.Add(new KeyValuePair<string, string>(module.LibraryName + ".dll", inlinesLibraryPath));
            }
        }

        public void Setup(Driver driver)
        {
            driver.ParserOptions.MicrosoftMode = false;
            driver.ParserOptions.NoBuiltinIncludes = true;
            driver.ParserOptions.TargetTriple = this.qtInfo.Target;
            driver.ParserOptions.Abi = CppAbi.Itanium;
            driver.ParserOptions.AddDefines("__float128=void");
            driver.Options.GeneratorKind = GeneratorKind.CSharp;
            driver.Options.UnityBuild = true;
            driver.Options.CheckSymbols = true;
            driver.Options.CompileCode = true;
            driver.Options.GenerateDefaultValuesForArguments = true;
            driver.Options.MarshalCharAsManagedChar = true;
            driver.Options.CommentKind = CommentKind.BCPLSlash;

            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            const string qt = "Qt";
            foreach (var libFile in this.qtInfo.LibFiles)
            {
                string qtModule = GetModuleNameFromLibFile(libFile);
                var module = driver.Options.AddModule($"{qtModule}.Sharp");
                module.Headers.Add(qtModule);
                var moduleName = qtModule.Substring(qt.Length);
                // some Qt modules have their own name-spaces
                if (moduleName == "Charts" || moduleName == "DataVisualization" ||
                    moduleName.StartsWith("3D", StringComparison.Ordinal))
                {
                    module.OutputNamespace = string.Empty;
                    module.InlinesLibraryName = $"{qtModule}-inlines";
                    module.TemplatesLibraryName = $"{qtModule}-templates";
                }
                else
                {
                    module.OutputNamespace = qtModule;
                }
                if (Platform.IsMacOS)
                {
                    var framework = $"{qtModule}.framework";
                    module.IncludeDirs.Add(Path.Combine(this.qtInfo.Libs, framework));
                    module.IncludeDirs.Add(Path.Combine(this.qtInfo.Libs, framework, "Headers"));
                    if (moduleName == "UiPlugin")
                    {
                        var qtUiPlugin = $"Qt{moduleName}.framework";
                        module.IncludeDirs.Add(Path.Combine(this.qtInfo.Libs, qtUiPlugin));
                        module.IncludeDirs.Add(Path.Combine(this.qtInfo.Libs, qtUiPlugin, "Headers"));
                    }
                }
                else
                {
                    var moduleInclude = Path.Combine(qtInfo.Headers, qtModule);
                    if (Directory.Exists(moduleInclude))
                        module.IncludeDirs.Add(moduleInclude);
                    if (moduleName == "Designer")
                    {
                        module.IncludeDirs.Add(Path.Combine(qtInfo.Headers, "QtUiPlugin"));
                    }
                }
                if (moduleName == "Designer")
                {
                    foreach (var header in Directory.EnumerateFiles(module.IncludeDirs.Last(), "*.h"))
                    {
                        module.Headers.Add(Path.GetFileName(header));
                    }
                }
                module.Libraries.Add(libFile);
                if (moduleName == "Core")
                {
                    module.CodeFiles.Add(Path.Combine(dir, "QObject.cs"));
                    module.CodeFiles.Add(Path.Combine(dir, "QChar.cs"));
                    module.CodeFiles.Add(Path.Combine(dir, "QEvent.cs"));
                }
            }

            foreach (var systemIncludeDir in this.qtInfo.SystemIncludeDirs)
                driver.ParserOptions.AddSystemIncludeDirs(systemIncludeDir);
            
            if (Platform.IsMacOS)
            {
                foreach (var frameworkDir in this.qtInfo.FrameworkDirs)
                    driver.ParserOptions.AddArguments($"-F{frameworkDir}");
                driver.ParserOptions.AddArguments($"-F{qtInfo.Libs}");
            }

            driver.ParserOptions.AddIncludeDirs(qtInfo.Headers);

            driver.ParserOptions.AddLibraryDirs(Platform.IsWindows ? qtInfo.Bins : qtInfo.Libs);

            // Qt defines its own GUID with the same header guard as the system GUID, so ensure the system GUID is read first
            driver.Project.AddFile("guiddef.h");
        }

        public static string GetModuleNameFromLibFile(string libFile)
        {
            var qtModule = Path.GetFileNameWithoutExtension(libFile);
            if (Platform.IsWindows)
            {
                return "Qt" + qtModule.Substring("Qt".Length + 1);
            }
            return libFile.Substring("lib".Length);
        }

        public void SetupPasses(Driver driver)
        {
            driver.Context.TranslationUnitPasses.AddPass(new GenerateSignalEventsPass(driver.Generator));
            driver.Context.TranslationUnitPasses.AddPass(new GenerateEventEventsPass(driver.Generator));
            driver.Context.TranslationUnitPasses.AddPass(new RemoveQObjectMembersPass());

            var generateInlinesPass = driver.Context.TranslationUnitPasses.FindPass<GenerateInlinesPass>();
            generateInlinesPass.InlinesCodeGenerated += (sender, e) =>
                                                        {
                                                            e.OutputDir = driver.Context.Options.OutputDir;
                                                            this.CompileMakefile(e);
                                                        };
        }

        private static void IgnorePrivateDeclarations(DeclarationContext unit)
        {
            foreach (var declaration in unit.Declarations)
            {
                IgnorePrivateDeclaration(declaration);
            }
        }

        private static void IgnorePrivateDeclaration(Declaration declaration)
        {
            if (declaration.Name != null &&
                (declaration.Name.StartsWith("Private", StringComparison.Ordinal) ||
                 declaration.Name.EndsWith("Private", StringComparison.Ordinal)))
            {
                declaration.ExplicitlyIgnore();
            }
            else
            {
                DeclarationContext declarationContext = declaration as DeclarationContext;
                if (declarationContext != null)
                {
                    IgnorePrivateDeclarations(declarationContext);
                }
            }
        }

        private void CompileMakefile(GenerateInlinesPass.InlinesCodeEventArgs e)
        {
            var pro = $"{e.Module.InlinesLibraryName}.pro";
            var path = Path.Combine(e.OutputDir, pro);
            var proBuilder = new StringBuilder();
            var qtModules = string.Join(" ", from header in e.Module.Headers
                                             where !header.EndsWith(".h", StringComparison.Ordinal)
                                             select header.Substring("Qt".Length).ToLowerInvariant());
            // QtTest is only library which has a "lib" suffix to its module alias for qmake
            if (qtModules == "test")
            {
                qtModules += "lib";
            }

            proBuilder.Append($"QT += {qtModules}\n");
            proBuilder.Append("CONFIG += c++11\n");
            proBuilder.Append($"TARGET = {e.Module.InlinesLibraryName}\n");
            proBuilder.Append("TEMPLATE = lib\n");
            proBuilder.Append($"SOURCES += {Path.ChangeExtension(pro, "cpp")}\n");
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                proBuilder.Append("LIBS += -loleaut32 -lole32");
            }
            File.WriteAllText(path, proBuilder.ToString());
            // HACK: work around https://bugreports.qt.io/browse/QTBUG-55952
            if (e.Module.LibraryName == "Qt3DRender.Sharp")
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
            int error;
            string errorMessage;
            ProcessHelper.Run(this.qtInfo.QMake, $"\"{path}\"", out error, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine(errorMessage);
                return;
            }
            var makefile = File.Exists(Path.Combine(e.OutputDir, "Makefile.Release")) ? "Makefile.Release" : "Makefile";
            e.CustomCompiler = this.qtInfo.Make;
            e.CompilerArguments = $"-f {makefile}";
            e.OutputDir = Platform.IsMacOS ? e.OutputDir : Path.Combine(e.OutputDir, "release");
        }

        private readonly QtInfo qtInfo;
        private List<KeyValuePair<string, string>> wrappedModules = new List<KeyValuePair<string, string>>();
    }
}
