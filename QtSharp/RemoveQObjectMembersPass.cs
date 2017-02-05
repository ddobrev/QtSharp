using System.Collections.Generic;
using System.Linq;
using CppSharp.AST;
using CppSharp.Passes;

namespace QtSharp
{
    public class RemoveQObjectMembersPass : TranslationUnitPass
    {
        public override bool VisitClassDecl(Class @class)
        {
            if (AlreadyVisited(@class) || @class.Name == "QObject")
            {
                return false;
            }

            IEnumerable<MacroExpansion> expansions = @class.PreprocessedEntities.OfType<MacroExpansion>();

            bool isQObject = expansions.Any(e => e.Text == "Q_OBJECT");
            if (isQObject)
            {
                RemoveQObjectMembers(@class);
            }
            return true;
        }

        private static void RemoveQObjectMembers(Class @class)
        {
            // Every Qt object "inherits" a lot of members via the Q_OBJECT macro.
            // See the define of Q_OBJECT in qobjectdefs.h for a list of the members.
            // We cannot use the Qt defines for disabling the expansion of these
            // because it would mess up with the object layout size.

            RemoveMethodOverloads(@class, "tr");
            RemoveMethodOverloads(@class, "trUtf8");
            RemoveMethodOverloads(@class, "qt_static_metacall");
            RemoveVariables(@class, "staticMetaObject");
        }

        private static void RemoveMethodOverloads(Class @class, string originalName)
        {
            var overloads = @class.Methods.Where(m => m.OriginalName == originalName).ToList();
            foreach (var method in overloads)
                @class.Methods.Remove(method);
        }

        private static void RemoveVariables(Class @class, string originalName)
        {
            var variables = @class.Variables.Where(v => v.OriginalName == originalName).ToList();
            foreach (var variable in variables)
                variable.ExplicitlyIgnore();
        }
    }
}
