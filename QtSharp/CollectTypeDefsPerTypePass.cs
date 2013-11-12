using System.Collections.Generic;
using CppSharp.AST;

namespace CppSharp.Passes
{
    /// <summary>
    /// Collects all types with their type defs in a dictionary.
    /// </summary>
    public class CollectTypeDefsPerTypePass : TranslationUnitPass
    {
        public CollectTypeDefsPerTypePass()
        {
            this.TypeDefsPerType = new Dictionary<Type, List<TypedefDecl>>();
        }

        public Dictionary<Type, List<TypedefDecl>> TypeDefsPerType { get; private set; }

        public override bool VisitTypedefDecl(TypedefDecl typedef)
        {
            Type type = typedef.Type.Desugar();
            List<TypedefDecl> typedefDecls;
            if (TypeDefsPerType.ContainsKey(type))
                typedefDecls = TypeDefsPerType[type];
            else
                TypeDefsPerType.Add(type, typedefDecls = new List<TypedefDecl>());
            if (!typedefDecls.Contains(typedef))
                typedefDecls.Add(typedef);
            return base.VisitTypedefDecl(typedef);
        }
    }
}
