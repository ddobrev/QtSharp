using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace QtSharp
{
    [TypeMap("QString")]
    public class QString : TypeMap
    {
        public override string CSharpSignature(TypePrinterContext ctx)
        {
            if (ctx.Kind == TypePrinterContextKind.Native)
            {
                return $"QtCore.QString.{Helpers.InternalStruct}{(ctx.Type.IsAddress() ? "*" : string.Empty)}";
            }
            return "string";
        }

        public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
        {
            var type = ctx.Parameter.Type.Desugar(false);
            var finalType = (type.GetFinalPointee() ?? type).Desugar(false);
            var substitution = finalType as TemplateParameterSubstitutionType;
            string param;
            if (substitution != null)
            {
                param = Generator.GeneratedIdentifier(ctx.Parameter.Name.Trim('@'));
                ctx.Before.WriteLine($"var {param} = (string) (object) {ctx.Parameter.Name};");
            }
            else
            {
                param = ctx.Parameter.Name;
            }
            ctx.Before.WriteLine("var __stringPtr{0} = ReferenceEquals({1}, null) ? null : (ushort*) Marshal.StringToHGlobalUni({1}).ToPointer();",
                                 ctx.ParameterIndex, param);
            ctx.Before.WriteLine("var __qstring{0} = __stringPtr{0} == null ? null : QtCore.QString.FromUtf16(ref *__stringPtr{0}, {1}.Length);",
                                 ctx.ParameterIndex, param);
            if (type.IsAddress())
            {
                ctx.Return.Write("ReferenceEquals(__qstring{0}, null) ? global::System.IntPtr.Zero : __qstring{0}.{1}",
                                 ctx.ParameterIndex, Helpers.InstanceIdentifier);
                return;
            }
            Class @class;
            type.TryGetClass(out @class);
            if (@class == null)
            {
                this.Type.TryGetClass(out @class);
            }
            this.typePrinter = this.typePrinter ?? (this.typePrinter = new CSharpTypePrinter(ctx.Context));
            var qualifiedIdentifier = (@class.OriginalClass ?? @class).Visit(this.typePrinter);
            ctx.Return.Write("ReferenceEquals(__qstring{0}, null) ? new {1}.{2}() : *({1}.{2}*) (__qstring{0}.{3})",
                             ctx.ParameterIndex, qualifiedIdentifier, Helpers.InternalStruct, Helpers.InstanceIdentifier);
        }

        public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
        {
            var type = ctx.ReturnType.Type.Desugar(false);
            var finalType = (type.GetFinalPointee() ?? type).Desugar(false);
            var templateParameter = finalType as TemplateParameterType;
            string cast = string.Empty;
            if (templateParameter != null)
            {
                cast = $"({templateParameter.Parameter.Name}) (object) ";
            }
            ctx.Return.Write($@"{cast}Marshal.PtrToStringUni(new IntPtr(QtCore.QString.{
                                Helpers.CreateInstanceIdentifier}({ctx.ReturnVarName}).Utf16))");
        }

        private CSharpTypePrinter typePrinter;
    }
}
