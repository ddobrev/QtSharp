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
            if (ctx.CSharpKind == CSharpTypePrinterContextKind.Native)
            {
                if (ctx.Type.IsAddress())
                {
                    return "QtCore.QString.Internal*";
                }
                return "QtCore.QString.Internal";
            }
            return "string";
        }

        public override void CSharpMarshalToNative(MarshalContext ctx)
        {
            ctx.SupportBefore.WriteLine("var __stringPtr{0} = (ushort*) Marshal.StringToHGlobalUni({1}).ToPointer();",
                                        ctx.ParameterIndex, ctx.Parameter.Name);
            ctx.SupportBefore.WriteLine("var __qstring{0} = QtCore.QString.FromUtf16(ref *__stringPtr{0}, {1}.Length);",
                                        ctx.ParameterIndex, ctx.Parameter.Name);
            Type type = ctx.Parameter.Type.Desugar();
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
                Type.TryGetClass(out @class);
            }
            typePrinter = typePrinter ?? (typePrinter = new CSharpTypePrinter(ctx.Driver));
            var qualifiedIdentifier = (@class.OriginalClass ?? @class).Visit(typePrinter);
            ctx.Return.Write("ReferenceEquals(__qstring{0}, null) ? new {1}.Internal() : *({1}.Internal*) (__qstring{0}.{2})",
                             ctx.ParameterIndex, qualifiedIdentifier, Helpers.InstanceIdentifier);
        }

        public override void CSharpMarshalToManaged(MarshalContext ctx)
        {
            ctx.Return.Write("Marshal.PtrToStringUni(new IntPtr(QtCore.QString.{0}({1}).Utf16))",
                Helpers.CreateInstanceIdentifier, ctx.ReturnVarName);
        }

        CSharpTypePrinter typePrinter;
    }
}
