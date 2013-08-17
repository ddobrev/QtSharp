using System.IO;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;

namespace QtSharp
{
	public class QtSharp : ILibrary
	{
	    private readonly string includePath;
	    private readonly string module;
	    private readonly string libraryPath;
	    private readonly string library;

	    public QtSharp(string includePath, string module, string libraryPath, string library)
	    {
	        this.includePath = includePath;
	        this.module = module;
	        this.libraryPath = libraryPath;
	        this.library = library;
	    }

	    public void Preprocess(Driver driver, Library lib)
	    {
            string qtModule = "Qt" + this.module;
	        string moduleIncludes = Path.Combine(includePath, qtModule);
	        foreach (TranslationUnit unit in lib.TranslationUnits)
	        {
	            string unitFileDir = Path.GetDirectoryName(unit.FilePath);
	            if (unitFileDir != moduleIncludes)
	            {
	                unit.ExplicityIgnored = true;
	            }
	            else
	            {
	                this.GetHashCode();
	            }
            }
		}

		public void Postprocess(Library lib)
		{
		}

		public void Setup(Driver driver)
		{
			driver.Options.GeneratorKind = LanguageGeneratorKind.CSharp;
		    string qtModule = "Qt" + this.module;
            driver.Options.Abi = CppAbi.Itanium;
		    driver.Options.LibraryName = string.Format("{0}Sharp", qtModule);
		    driver.Options.OutputNamespace = qtModule;
			driver.Options.Verbose = true;
			driver.Options.IgnoreParseWarnings = true;
            driver.Options.Headers.Add(qtModule);
			driver.Options.IncludeDirs.Add(includePath);
            driver.Options.IncludeDirs.Add(Path.Combine(includePath, qtModule));
            driver.Options.LibraryDirs.Add(this.libraryPath);
            driver.Options.Libraries.Add(this.library);
			driver.Options.Defines.Add("_MSC_FULL_VER=170050215");
		}

		public void SetupPasses(Driver driver)
		{
		}
	}
}
