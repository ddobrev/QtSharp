using CppSharp.AST;
using CppSharp.Passes;

namespace QtSharp
{
    public class GenerateEventEventsPass : TranslationUnitPass
    {
        public override bool VisitClassDecl(Class @class)
        {
            if (AlreadyVisited(@class))
            {
                return true;
            }
            // HACK: work around the bug about methods in v-tables and methods in classes not being shared objects
            foreach (VTableComponent entry in VTables.GatherVTableMethodEntries(@class))
            {
                if (entry.Method != null && (entry.Method.Name.EndsWith("Event") || entry.Method.Name == "event") &&
                    entry.Method.Parameters.Count == 1 && entry.Method.Parameters[0].Type.ToString().EndsWith("Event") &&
                    !entry.Method.Name.StartsWith("on"))
                {
                    entry.Method.Name = "on" + char.ToUpperInvariant(entry.Method.Name[0]) + entry.Method.Name.Substring(1);
                }
            }
            return base.VisitClassDecl(@class);
        }

        public override bool VisitMethodDecl(Method method)
        {
            if ((method.Name.EndsWith("Event") || method.Name == "event") &&
                method.Parameters.Count == 1 && method.Parameters[0].Type.ToString().EndsWith("Event"))
            {
                method.Name = "on" + char.ToUpperInvariant(method.Name[0]) + method.Name.Substring(1);
            }
            return base.VisitMethodDecl(method);
        }
    }
}
