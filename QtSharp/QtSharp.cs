using System.IO;
using System.Reflection;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using Template = CppSharp.AST.Template;

namespace QtSharp
{
	public class QtSharp : ILibrary
	{
	    private readonly string qmake;
	    private readonly string make;
	    private readonly string includePath;
	    private readonly string module;
	    private readonly string libraryPath;
	    private readonly string library;
	    private readonly string docs;

	    public QtSharp(string qmake, string make, string includePath, string module, string libraryPath, string library, string docs)
	    {
	        this.qmake = qmake;
	        this.includePath = includePath;
	        this.module = module;
	        this.libraryPath = libraryPath;
	        this.library = library;
            this.make = make;
            this.docs = docs;
	    }

	    public void Preprocess(Driver driver, ASTContext lib)
	    {
            string qtModule = "Qt" + this.module;
	        string moduleIncludes = Path.Combine(this.includePath, qtModule);
	        foreach (TranslationUnit unit in lib.TranslationUnits)
	        {
	            if (Path.GetDirectoryName(unit.FilePath) != moduleIncludes)
	            {
	                unit.ExplicityIgnored = true;
	            }
	            else
	            {
	                IgnorePrivateDeclarations(unit);
	            }
	        }
		}

	    private static void IgnorePrivateDeclarations(DeclarationContext unit)
	    {
	        foreach (Namespace ns in unit.Namespaces)
	        {
	            IgnorePrivateDeclaration(ns);
	        }
	        foreach (Enumeration enumeration in unit.Enums)
	        {
	            IgnorePrivateDeclaration(enumeration);
	        }
	        foreach (Function function in unit.Functions)
	        {
	            IgnorePrivateDeclaration(function);
	        }
	        foreach (Class @class in unit.Classes)
	        {
	            IgnorePrivateDeclaration(@class);
	        }
	        foreach (Template template in unit.Templates)
	        {
	            IgnorePrivateDeclaration(template);
	        }
	        foreach (TypedefDecl typedefDecl in unit.Typedefs)
	        {
	            IgnorePrivateDeclaration(typedefDecl);
	        }
	        foreach (Variable variable in unit.Variables)
	        {
	            IgnorePrivateDeclaration(variable);
	        }
	        foreach (Event @event in unit.Events)
	        {
	            IgnorePrivateDeclaration(@event);
	        }
	    }

	    private static void IgnorePrivateDeclaration(Declaration declaration)
	    {
	        // this will be ignored anyway
	        if (declaration.Access == AccessSpecifier.Private)
	        {
	            return;
	        }
	        if (declaration.Name != null &&
	            (declaration.Name.StartsWith("Private") || declaration.Name.EndsWith("Private")))
	        {
	            declaration.ExplicityIgnored = true;
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

        public void Postprocess(Driver driver, ASTContext lib)
        {
            new CaseRenamePass(
                RenameTargets.Function | RenameTargets.Method | RenameTargets.Property | RenameTargets.Delegate,
                RenameCasePattern.UpperCamelCase).VisitLibrary(driver.ASTContext);
        }

		public void Setup(Driver driver)
		{
			driver.Options.GeneratorKind = GeneratorKind.CSharp;
		    string qtModule = "Qt" + this.module;
            driver.Options.Abi = CppAbi.Itanium;
		    driver.Options.LibraryName = string.Format("{0}Sharp", qtModule);
		    driver.Options.OutputNamespace = qtModule;
			driver.Options.Verbose = true;
		    driver.Options.GenerateAbstractImpls = true;
            driver.Options.GenerateVirtualTables = true;
		    driver.Options.GenerateInterfacesForMultipleInheritance = true;
		    driver.Options.GenerateProperties = true;
			driver.Options.IgnoreParseWarnings = true;
		    driver.Options.CheckSymbols = true;
            driver.Options.Headers.Add(qtModule);
			driver.Options.IncludeDirs.Add(this.includePath);
            driver.Options.IncludeDirs.Add(Path.Combine(this.includePath, qtModule));
            driver.Options.LibraryDirs.Add(this.libraryPath);
            driver.Options.Libraries.Add(this.library);
			driver.Options.Defines.Add("_MSC_FULL_VER=170050215");
		    string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            driver.Options.CodeFiles.Add(Path.Combine(dir, "QEventArgs.cs"));
            driver.Options.CodeFiles.Add(Path.Combine(dir, "QEventHandler.cs"));
		}

		public void SetupPasses(Driver driver)
		{
            driver.TranslationUnitPasses.AddPass(new CompileInlinesPass(this.qmake, this.make));
            driver.TranslationUnitPasses.AddPass(new GenerateEventEventsPass());
            driver.TranslationUnitPasses.AddPass(new GenerateSignalEventsPass());
            driver.TranslationUnitPasses.AddPass(new RemoveStaticsFromDerivedTypesPass());
            driver.TranslationUnitPasses.AddPass(new GetCommentsFromQtDocsPass(this.docs, this.module));
		}
	}
}
