using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace QtSharp
{
    [TypeMap("QFlags")]
    public class QFlags : TypeMap
    {
        public override string CSharpSignature(CSharpTypePrinterContext ctx)
        {
            TemplateArgument templateArgument = ((TemplateSpecializationType) ctx.Type.Desugar()).Arguments[0];
            return templateArgument.Type.Type.ToString();
        }

        public override void CSharpMarshalToNative(MarshalContext ctx)
        {
            ctx.Return.Write(ctx.Parameter.Name);
        }

        public override void CSharpMarshalToManaged(MarshalContext ctx)
        {
            ctx.Return.Write(ctx.ReturnVarName);
        }
    }
}
