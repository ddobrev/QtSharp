using System;
using System.Collections.Generic;
using CppSharp.AST;
using CppSharp.Passes;
using Type = CppSharp.AST.Type;

namespace QtSharp
{
    public class GetCommentsFromQtDocsPass : TranslationUnitPass
    {
        private readonly Documentation documentation;

        public GetCommentsFromQtDocsPass(string docsPath, string module, Dictionary<Type, List<TypedefDecl>> typeDefsPerType)
        {
            this.documentation = new Documentation(docsPath, module, typeDefsPerType);
        }

        public override bool VisitClassDecl(Class @class)
        {
            if (@class.Comment == null)
            {
                this.documentation.DocumentType(@class);
            }
            return base.VisitClassDecl(@class);
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            if (@enum.Comment == null)
            {
                this.documentation.DocumentEnum(@enum);
                foreach (Enumeration.Item item in @enum.Items)
                {
                    this.documentation.DocumentEnumItem(@enum, item);
                }
            }
            return base.VisitEnumDecl(@enum);
        }

        public override bool VisitFunctionDecl(Function function)
        {
            if (function.Comment == null && !function.ExplicityIgnored)
            {
                this.documentation.DocumentFunction(function);
            }
            return base.VisitFunctionDecl(function);
        }

        public override bool VisitProperty(Property property)
        {
            if (property.Field != null)
            {
                
            }
            else
            {
                Method getter = property.GetMethod;
                if (getter.Comment == null)
                {
                    this.VisitFunctionDecl(getter);
                }
                if (getter.Comment != null)
                {
                    var comment = new RawComment();
                    comment.Kind = getter.Comment.Kind;
                    comment.BriefText = getter.Comment.BriefText;
                    comment.Text = getter.Comment.Text;
                    Method setter = property.SetMethod;
                    if (setter != null)
                    {
                        if (setter.Comment == null)
                        {
                            this.VisitFunctionDecl(setter);
                        }
                        if (setter.Comment != null)
                        {
                            comment.BriefText += Environment.NewLine + setter.Comment.BriefText;
                            comment.Text += Environment.NewLine + setter.Comment.Text;
                        }
                    }
                    property.Comment = comment;
                }
            }
            return base.VisitProperty(property);
        }
    }
}
