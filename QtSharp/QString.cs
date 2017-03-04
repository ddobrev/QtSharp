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
        public override string CSharpSignature(CSharpTypePrinterContext ctx)
        {
            if (ctx.CSharpKind == TypePrinterContextKind.Native)
            {
                return string.Format("QtCore.QString.{0}{1}", Helpers.InternalStruct, ctx.Type.IsAddress() ? "*" : string.Empty);
            }
            return "string";
        }

        public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
        {
            ctx.SupportBefore.WriteLine("var __stringPtr{0} = ReferenceEquals({1}, null) ? null : (ushort*) Marshal.StringToHGlobalUni({1}).ToPointer();",
                                        ctx.ParameterIndex, ctx.Parameter.Name);
            ctx.SupportBefore.WriteLine("var __qstring{0} = __stringPtr{0} == null ? null : QtCore.QString.FromUtf16(ref *__stringPtr{0}, {1}.Length);",
                                        ctx.ParameterIndex, ctx.Parameter.Name);
            var type = ctx.Parameter.Type.Desugar();
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
            ctx.Return.Write("Marshal.PtrToStringUni(new IntPtr(QtCore.QString.{0}({1}).Utf16))",
                Helpers.CreateInstanceIdentifier, ctx.ReturnVarName);
        }

        private CSharpTypePrinter typePrinter;
    }
}
