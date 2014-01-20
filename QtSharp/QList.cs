using System.Globalization;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace QtSharp
{
    // this type map has actually nothing to do with compiling calls to the copy ctor of QItemSelection
    // it's only about marshalling members using QList
    // about those calls, they only look for members in QItemSelection itself, not in its base classes
    // so, to solve it, we need some kind of logic to replace these calls with manual code
    // which would actually be quite simple, copying from one QListData to the other
    // it would also involve accessing the otherwise private QListData::Data *d field of QList, from QItemSelection
    // so, to break it up:
    // 1. Expose QListData::Data *d to any derived types;
    // 2. Add a copy ctor making use of that field;
    // 3. Replace any calls to the missing ctor with calls to the ctor from 2.
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
                return "System.Collections.Generic.IList<global::System.IntPtr>";
            }
            return string.Format("System.Collections.Generic.IList<{0}>", ctx.GetTemplateParameterList());
        }

        public override void CSharpMarshalToNative(MarshalContext ctx)
        {
            TextGenerator supportBefore = ctx.SupportBefore;
            string suffix = ctx.ParameterIndex > 0 ? ctx.ParameterIndex.ToString(CultureInfo.InvariantCulture) : string.Empty;
            string qList = string.Format("__qList{0}", suffix);
            supportBefore.WriteLine(string.Format("var {0} = new QtCore.QList();", qList));
            string qListDataData = string.Format("__qlistDataData{0}", suffix);
            supportBefore.WriteLine("var {0} = (QListData.Data.Internal*) {1}.d.d;", qListDataData, qList);
            // TODO: tests with Qt shows that while alloc is not smaller than end, it's not equal, it reserves more space actually
            supportBefore.WriteLine("{0}->alloc = {1}.Count;", qListDataData, ctx.Parameter.Name);
            supportBefore.WriteLine("{0}->begin = 0;", qListDataData, ctx.Parameter.Name);
            supportBefore.WriteLine("{0}->end = {1}.Count;", qListDataData, ctx.Parameter.Name);
            supportBefore.WriteLine("fixed (void** __v = new void*[{0}.Count])", ctx.Parameter.Name);
            supportBefore.WriteStartBraceIndent();
            supportBefore.WriteLine("{0}->array = __v;", qListDataData);
            supportBefore.WriteCloseBraceIndent();
            supportBefore.WriteLine("", qListDataData, ctx.Parameter.Name);
            var parameterType = ctx.Parameter.Type.Desugar();
            TemplateSpecializationType type = parameterType as TemplateSpecializationType;
            if (type == null)
            {
                TypedefType typedef;
                if (parameterType.IsPointerTo(out typedef))
                {
                    type = (TemplateSpecializationType) typedef.Desugar();
                }
                else
                {
                    parameterType.IsPointerTo(out type);
                }
            }
            Type elementType = type.Arguments[0].Type.Type.Desugar();
            string instance = string.Empty;
            if (!elementType.IsPointerToPrimitiveType())
            {
                instance = string.Format(".{0}", Helpers.InstanceIdentifier);
            }
            supportBefore.WriteLine("for (int i = 0; i < {0}.Count; i++)", ctx.Parameter.Name);
            supportBefore.WriteStartBraceIndent();
            Type desugared = ctx.Parameter.Type.Desugar();
            TemplateSpecializationType templateSpecializationType = desugared as TemplateSpecializationType;
            if (templateSpecializationType == null)
            {
                Type paramType;
                desugared.IsPointerTo(out paramType);
                templateSpecializationType = (TemplateSpecializationType) paramType.Desugar();
            }
            if (templateSpecializationType.Arguments[0].Type.ToString() == "string")
            {
                supportBefore.WriteLine("{0}->array[i] = Marshal.StringToHGlobalUni({1}[i]).ToPointer();", qListDataData, ctx.Parameter.Name, instance);                
            }
            else
            {
                supportBefore.WriteLine("{0}->array[i] = (void*) {1}[i]{2};", qListDataData, ctx.Parameter.Name, instance);                
            }
            supportBefore.WriteCloseBraceIndent();
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

        public override void CSharpMarshalCopyCtorToManaged(MarshalContext ctx)
        {
            ctx.SupportBefore.WriteLine("var __instance = new {0}.Internal();", ctx.ReturnType);
            ctx.SupportBefore.WriteLine("QListData.Data.Internal* qListData = (QListData.Data.Internal*) __instance.d;");
            ctx.SupportBefore.WriteLine("QListData.Data.Internal* result = (QListData.Data.Internal*) __ret.d;");
            ctx.SupportBefore.WriteLine("qListData->begin = result->begin;");
            ctx.SupportBefore.WriteLine("qListData->end = result->end;");
            ctx.SupportBefore.WriteLine("qListData->alloc = result->alloc;");
            ctx.SupportBefore.WriteLine("fixed (void** v = new void*[qListData->end])");
            ctx.SupportBefore.WriteStartBraceIndent();
            ctx.SupportBefore.WriteLine("for (int i = 0; i < qListData->end; i++)");
            ctx.SupportBefore.WriteStartBraceIndent();
            ctx.SupportBefore.WriteLine("v[i] = *(result->array + i);");
            ctx.SupportBefore.WriteCloseBraceIndent();
            ctx.SupportBefore.WriteLine("qListData->array = v;");
            ctx.SupportBefore.WriteCloseBraceIndent();
            ctx.SupportBefore.WriteLine("__instance.d = (IntPtr) qListData;");
        }
    }
}
