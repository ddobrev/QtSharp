namespace QtSharp.DocGeneration
{
    public class DocumentationNode
    {
        public DocumentationNode()
        {
        }

        public DocumentationNode(string outerHtml, string innerHtml)
        {
            this.OuterHtml = outerHtml;
            this.InnerHtml = innerHtml;
        }

        public string OuterHtml { get; set; }
        public string InnerHtml { get; set; }
    }
}
