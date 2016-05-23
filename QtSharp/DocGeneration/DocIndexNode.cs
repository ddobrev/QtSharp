namespace QtSharp.DocGeneration
{
    public class DocIndexNode
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int LineNumber { get; set; }
        public string HRef { get; set; }
        public bool IsObsolete { get; set; }
    }
}
