using System.Collections.Generic;

namespace QtSharp
{
    public class QtInfo
    {
        public int MajorVersion;
        public int MinorVersion;
        public string Path;
        public string Target;
        public string Docs;
        public string QMake;
        public string Make;
        public string Bins;
        public string Libs;
        public string Headers;
        public IList<string> LibFiles;
        public IEnumerable<string> SystemIncludeDirs;
        public IEnumerable<string> FrameworkDirs;
    }
}
