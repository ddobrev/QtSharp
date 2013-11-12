using CppSharp.AST;

namespace CppSharp.Passes
{
    public class ClearCommentsPass : TranslationUnitPass
    {
        public override bool VisitDeclaration(Declaration decl)
        {
            decl.Comment = null;
            return base.VisitDeclaration(decl);
        }
    }
}
