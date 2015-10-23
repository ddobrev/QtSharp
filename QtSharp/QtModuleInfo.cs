using System.Collections.Generic;

namespace QtSharp
{
    public class QtModuleInfo
    {
        public QtModuleInfo(string qmake, string make, string includePath, string libraryPath, string library, string target, IEnumerable<string> systemIncludeDirs, string docs)
        {
            this.Qmake = qmake;
            this.Make = make;
            this.IncludePath = includePath;
            this.LibraryPath = libraryPath;
            this.Library = library;
            this.Target = target;
            this.SystemIncludeDirs = systemIncludeDirs;
            this.Docs = docs;
        }

        public string Qmake { get; private set; }

        public string Make { get; private set; }

        public string IncludePath { get; private set; }

        public string LibraryPath { get; private set; }

        public string Library { get; private set; }

        public string Target { get; private set; }

        public IEnumerable<string> SystemIncludeDirs { get; private set; }

        public string Docs { get; private set; }
    }
}