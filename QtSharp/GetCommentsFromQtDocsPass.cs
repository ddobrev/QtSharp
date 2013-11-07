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
                this.documentation.DocumentType(@class);
            }
            return base.VisitClassDecl(@class);
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            if (@enum.Comment == null)
            {
                this.documentation.DocumentEnum(@enum);
                foreach (Enumeration.Item item in @enum.Items)
                {
                    this.documentation.DocumentEnumItem(@enum, item);
                }
            }
            return base.VisitEnumDecl(@enum);
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
