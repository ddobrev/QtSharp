using System.Collections.Generic;
using CppSharp.AST;
using CppSharp.Passes;

namespace QtSharp
{
    public class GetCommentsFromQtDocsPass : TranslationUnitPass
    {
        private IDictionary<string, string> docs;

        public GetCommentsFromQtDocsPass(string docsPath, string module)
        {
            this.docs = Documentation.Get(docsPath, module);
        }

        public override bool VisitClassDecl(Class @class)
        {
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
    }
}
