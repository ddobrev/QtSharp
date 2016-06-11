using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace QtSharp
{
    [TypeMap("QFlags")]
    public class QFlags : TypeMap
    {
        public override string CSharpConstruct()
        {
            return string.Empty;
        }

        public override Type CSharpSignatureType(CSharpTypePrinterContext ctx)
        {
            return GetEnumType(ctx.Type);
        }

        public override string CSharpSignature(CSharpTypePrinterContext ctx)
        {
            return this.CSharpSignatureType(ctx).ToString();
        }

        public override void CSharpMarshalToNative(MarshalContext ctx)
        {
            if (ctx.Parameter.Type.Desugar().IsAddress())
                ctx.Return.Write("new global::System.IntPtr(&{0})", ctx.Parameter.Name);
            else
                ctx.Return.Write(ctx.Parameter.Name);
        }

        public override void CSharpMarshalToManaged(MarshalContext ctx)
        {
            if (ctx.ReturnType.Type.Desugar().IsAddress())
            {
                var finalType = ctx.ReturnType.Type.GetFinalPointee() ?? ctx.ReturnType.Type;
                var enumType = GetEnumType(finalType);
                ctx.Return.Write("*({0}*) {1}", enumType, ctx.ReturnVarName);
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName);
            }
        }

        private static Type GetEnumType(Type mappedType)
        {
            var type = mappedType.Desugar();
            ClassTemplateSpecialization classTemplateSpecialization;
            var templateSpecializationType = type as TemplateSpecializationType;
            if (templateSpecializationType != null)
                classTemplateSpecialization = templateSpecializationType.GetClassTemplateSpecialization();
            else
                classTemplateSpecialization = (ClassTemplateSpecialization) ((TagType) type).Declaration;
            return classTemplateSpecialization.Arguments[0].Type.Type;
        }
    }
}
