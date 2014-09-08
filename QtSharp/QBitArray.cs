using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace QtSharp
{
    // TODO: this type map replaces a missing because of symbols copy ctor; remove the whole map when these symbols have been properly fixed
    [TypeMap("QBitArray")]
    public class QBitArray : TypeMap
    {
        public override bool DoesMarshalling
        {
            get { return false; }
        }

        public override string CSharpSignature(CSharpTypePrinterContext ctx)
        {
            return string.Empty;
        }

        public override void CSharpMarshalToNative(MarshalContext ctx)
        {
        }

        public override void CSharpMarshalToManaged(MarshalContext ctx)
        {
        }

        public override void CSharpMarshalCopyCtorToManaged(MarshalContext ctx)
        {
            Class @class;
            ctx.ReturnType.Type.TryGetClass(out @class);
            ctx.SupportBefore.WriteLine("global::System.IntPtr {0} = Marshal.AllocHGlobal({1});", ctx.ReturnVarName, @class.Layout.Size);
            ctx.SupportBefore.WriteLine("*(QBitArray.Internal*) {0} = native;", ctx.ReturnVarName);
            ctx.SupportBefore.WriteLine("return {0};", ctx.ReturnVarName);
        }
    }
}
