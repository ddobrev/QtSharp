using System.Globalization;
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
            TextGenerator supportBefore = ctx.SupportBefore;
            var suffix = ctx.ParameterIndex > 0 ? ctx.ParameterIndex.ToString(CultureInfo.InvariantCulture) : string.Empty;
            string qList = string.Format("__qList{0}", suffix);
            supportBefore.WriteLine(string.Format("var {0} = new QtCore.QList();", qList));
            string qListDataData = string.Format("__qlistDataData{0}", suffix);
            supportBefore.WriteLine("var {0} = (QListData.Data.Internal*) {1}.d.d;", qListDataData, qList);
            // TODO: tests with Qt shows that while alloc is larger than end, it's not equal, it reserves more space actually
            supportBefore.WriteLine("{0}->alloc = {1}.Count;", qListDataData, ctx.Parameter.Name);
            supportBefore.WriteLine("{0}->begin = 0;", qListDataData, ctx.Parameter.Name);
            supportBefore.WriteLine("{0}->end = {1}.Count;", qListDataData, ctx.Parameter.Name);
            // TODO: wrap the array of void* in QListData.Data, iterate through the items in the param and append them to that array
            ctx.Return.Write(qList);
        }

        public override void CSharpMarshalToManaged(MarshalContext ctx)
        {
            TemplateSpecializationType templateType = (TemplateSpecializationType) this.Type;
            QualifiedType type = templateType.Arguments[0].Type;

            TextGenerator supportBefore = ctx.SupportBefore;
            supportBefore.WriteLine("var __qlistData = new QListData({0}.d);", ctx.ReturnVarName);
            supportBefore.WriteLine("var __size = __qlistData.Size;");
            supportBefore.WriteLine("var __list = new System.Collections.Generic.List<{0}>(__size);", type);
            supportBefore.WriteLine("for (int i = 0; i < __size; i++)");
            supportBefore.WriteStartBraceIndent();
            // TODO: handle pointers to primitives, they cannot be used as a placeholder type and use IntPtr instead
            if (type.Type.IsPrimitiveType() || type.Type.IsEnumType())
            {
                supportBefore.WriteLine("__list.Add(*({0}*) __qlistData.At(i));", type);                        
            }
            else
            {
                Class @class;
                Type pointee;
                if ((type.Type.IsTagDecl(out @class) ||
                     (type.Type.IsPointerTo(out pointee) && pointee.IsTagDecl(out @class))) && @class.IsAbstract)
                {
                    supportBefore.WriteLine("__list.Add(new {0}Internal(new global::System.IntPtr(__qlistData.At(i))));", type, ctx.ReturnVarName);
                }
                else
                {
                    supportBefore.WriteLine("__list.Add(new {0}(new global::System.IntPtr(__qlistData.At(i))));", type, ctx.ReturnVarName);
                }
            }
            supportBefore.WriteCloseBraceIndent();
            ctx.Return.Write("__list");
        }
    }
}
