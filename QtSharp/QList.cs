using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace QtSharp
{
    [TypeMap("QList")]
    public class QList : TypeMap
    {
        public override bool IsIgnored
        {
            get
            {
                TemplateSpecializationType type = (TemplateSpecializationType) this.Type;
                QualifiedType pointeeType = type.Arguments[0].Type;
                TypeIgnoreChecker checker = new TypeIgnoreChecker(TypeMapDatabase);
                pointeeType.Visit(checker);
                return checker.IsIgnored;
            }
        }

        public override string CSharpSignature(CSharpTypePrinterContext ctx)
        {
            if (ctx.CSharpKind == CSharpTypePrinterContextKind.Native)
                return "QList";

            TemplateSpecializationType templateSpecialization = (TemplateSpecializationType) ctx.Type.Desugar();
            TemplateArgument templateArgument = templateSpecialization.Arguments[0];
            if (templateArgument.Type.Type.IsPointerToPrimitiveType())
            {
                return "System.Collections.Generic.List<global::System.IntPtr>";
            }
            return string.Format("System.Collections.Generic.List<{0}>", ctx.GetTemplateParameterList());
        }

        public override void CSharpMarshalToNative(MarshalContext ctx)
        {
            // TODO: create a QList, iterate through the items in the param, append them to the QList and pass the QList
            ctx.Return.Write("new QList()");
        }

        public override void CSharpMarshalToManaged(MarshalContext ctx)
        {
            TemplateSpecializationType templateType = (TemplateSpecializationType) this.Type;
            QualifiedType type = templateType.Arguments[0].Type;

            TextGenerator supportBefore = ctx.SupportBefore;
            supportBefore.WriteLine("var __size = {0}.d.Size;", ctx.ReturnVarName);
            supportBefore.WriteLine("var __list = new System.Collections.Generic.List<{0}>(__size);", type);
            supportBefore.WriteLine("for (int i = 0; i < __size; i++)");
            supportBefore.WriteStartBraceIndent();
            // TODO: handle pointers to primitives, they cannot be used as a placeholder type and use IntPtr instead
            if (type.Type.IsPrimitiveType() || type.Type.IsEnumType())
            {
                supportBefore.WriteLine("__list.Add(*({0}*) {1}.d.At(i));", type, ctx.ReturnVarName);                        
            }
            else
            {
                Class @class;
                Type pointee;
                if ((type.Type.IsTagDecl(out @class) ||
                     (type.Type.IsPointerTo(out pointee) && pointee.IsTagDecl(out @class))) && @class.IsAbstract)
                {
                    supportBefore.WriteLine("__list.Add(new {0}Internal(new global::System.IntPtr({1}.d.At(i))));", type, ctx.ReturnVarName);
                }
                else
                {
                    supportBefore.WriteLine("__list.Add(new {0}(new global::System.IntPtr({1}.d.At(i))));", type, ctx.ReturnVarName);
                }
            }
            supportBefore.WriteCloseBraceIndent();
            ctx.Return.Write("__list");
        }
    }
}
