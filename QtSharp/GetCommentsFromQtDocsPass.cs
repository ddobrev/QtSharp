using CppSharp.AST;
using CppSharp.Passes;

namespace QtSharp
{
    public class GetCommentsFromQtDocsPass : TranslationUnitPass
    {
        private readonly Documentation documentation;

        public GetCommentsFromQtDocsPass(string docsPath, string module)
        {
            this.documentation = new Documentation(docsPath, module);
            this.Options.VisitFunctionParameters = false;
        }

        public override bool VisitClassDecl(Class @class)
        {
            if (!@class.IsIncomplete && base.VisitClassDecl(@class))
            {
                this.documentation.DocumentType(@class);
                return true;
            }
            return false;
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            if (!this.AlreadyVisited(@enum))
            {
                this.documentation.DocumentEnum(@enum);
            }
            return base.VisitEnumDecl(@enum);
        }

        public override bool VisitFunctionDecl(Function function)
        {
            if (!function.ExplicityIgnored && function.IsGenerated)
            {
                this.DocumentFunction(function);
            }
            return base.VisitFunctionDecl(function);
        }

        public override bool VisitProperty(Property property)
        {
            if (!property.IsSynthetized && !this.AlreadyVisited(property))
            {
                this.documentation.DocumentProperty(property);
            }
            return base.VisitProperty(property);
        }

        public override bool VisitEvent(Event @event)
        {
            Function function = @event.OriginalDeclaration as Function;
            if (function != null && !this.AlreadyVisited(@event))
            {
                this.DocumentFunction(function);
            }
            return base.VisitEvent(@event);
        }

        private void DocumentFunction(Function function)
        {
            if (!this.AlreadyVisited(function))
            {
                if (function.Comment == null)
                {
                    if (function.IsSynthetized)
                    {
                        if (function.SynthKind == FunctionSynthKind.DefaultValueOverload)
                        {
                            function.Comment = function.OriginalFunction.Comment;
                        }
                    }
                    else
                    {
                        this.documentation.DocumentFunction(function);
                    }
                }
            }
        }
    }
}
