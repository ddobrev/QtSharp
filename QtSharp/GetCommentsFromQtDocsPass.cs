using CppSharp.AST;
using CppSharp.Passes;

namespace QtSharp
{
    public class GetCommentsFromQtDocsPass : TranslationUnitPass
    {
        private readonly Documentation documentation;

        public GetCommentsFromQtDocsPass(string docsPath, string module)
        {
            this.documentation = new Documentation(docsPath, module);
        }

        public override bool VisitClassDecl(Class @class)
        {
            if (@class.Comment == null)
            {
                this.documentation.CommentType(@class);
            }
            return base.VisitClassDecl(@class);
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            return base.VisitEnumDecl(@enum);
        }

        public override bool VisitEnumItem(Enumeration.Item item)
        {
            return base.VisitEnumItem(item);
        }

        public override bool VisitFunctionDecl(Function function)
        {
            return base.VisitFunctionDecl(function);
        }

        public override bool VisitProperty(Property property)
        {
            return base.VisitProperty(property);
        }
    }
}
