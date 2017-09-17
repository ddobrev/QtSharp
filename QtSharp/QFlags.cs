using System.Linq;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace QtSharp
{
    [TypeMap("QFlags")]
    public class QFlags : TypeMap
    {
        public override string CSharpConstruct() => string.Empty;

        public override Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return GetEnumType(ctx.Type);
        }

        public override string CSharpSignature(TypePrinterContext ctx)
        {
            var enumType = GetEnumType(ctx.Type);
            if (enumType == null)
            {
                var specializationType = (TemplateSpecializationType) ctx.Type;
                return $@"{specializationType.Template.Name}<{
                           string.Join(", ", specializationType.Arguments.Select(a => a.Type.Type))}>";
            }
            return this.CSharpSignatureType(ctx).ToString();
        }

        public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
        {
            if (ctx.Parameter.Type.Desugar().IsAddress())
                ctx.Return.Write("new global::System.IntPtr(&{0})", ctx.Parameter.Name);
            else
                ctx.Return.Write(ctx.Parameter.Name);
        }

        public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
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

        public override bool IsIgnored => Type != null ? Type.IsDependent : Declaration.IsDependent;

        private static Type GetEnumType(Type mappedType)
        {
            var type = mappedType.Desugar();
            ClassTemplateSpecialization classTemplateSpecialization;
            var templateSpecializationType = type as TemplateSpecializationType;
            if (templateSpecializationType != null)
                classTemplateSpecialization = templateSpecializationType.GetClassTemplateSpecialization();
            else
                classTemplateSpecialization = (ClassTemplateSpecialization) ((TagType) type).Declaration;
            if (classTemplateSpecialization == null)
            {
                return null;
            }
            return classTemplateSpecialization.Arguments[0].Type.Type;
        }
    }
}
