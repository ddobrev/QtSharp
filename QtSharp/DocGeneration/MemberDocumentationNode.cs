using System.Collections.Generic;

namespace QtSharp.DocGeneration
{
    public class MemberDocumentationNode : DocumentationNode
    {
        public MemberDocumentationNode()
        {
            this.Codes = new List<MemberDocumentationNode>();
            this.Td = new List<MemberDocumentationNode>();
        }

        public string Name { get; set; }
        public string InnerText { get; set; }
        public bool ContainsEnumMembers { get; set; }
        public List<MemberDocumentationNode> Codes { get; private set; }
        public List<MemberDocumentationNode> Td { get; private set; }
        public MemberDocumentationNode ParentOfParent { get; set; }
    }
}
