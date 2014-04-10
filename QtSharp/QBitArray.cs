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
            ctx.SupportBefore.WriteLine("var __instance = new QBitArray.Internal();");
            ctx.SupportBefore.WriteLine("__instance.d = __ret.d;");
        }
    }
}
