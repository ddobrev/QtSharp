using System.Linq;
using CppSharp.AST;
using CppSharp.Passes;

namespace QtSharp
{
    public class GetCommentsFromQtDocsPass : TranslationUnitPass
    {
        public GetCommentsFromQtDocsPass(string docsPath, string module)
        {
            this.documentation = new Documentation(docsPath, module);
            this.Options.VisitFunctionReturnType = false;
            this.Options.VisitFunctionParameters = false;
            this.Options.VisitClassBases = false;
            this.Options.VisitTemplateArguments = false;
            this.Options.VisitClassFields = false;
        }

        public override bool VisitLibrary(ASTContext context)
        {
            return this.documentation.Exists && base.VisitLibrary(context);
        }

        public override bool VisitClassDecl(Class @class)
        {
            if (!@class.IsIncomplete && base.VisitClassDecl(@class))
            {
                if (@class.IsInterface)
                {
                    @class.Comment = @class.OriginalClass.Comment;
                    foreach (var method in @class.OriginalClass.Methods)
                    {
                        var interfaceMethod = @class.Methods.FirstOrDefault(m => m.OriginalPtr == method.OriginalPtr);
                        if (interfaceMethod != null)
                        {
                            interfaceMethod.Comment = method.Comment;
                        }
                    }
                    foreach (var property in @class.OriginalClass.Properties)
                    {
                        var interfaceProperty = @class.Properties.FirstOrDefault(p => p.Name == property.Name);
                        if (interfaceProperty != null)
                        {
                            interfaceProperty.Comment = property.Comment;
                        }
                    }
                }
                else
                {
                    this.documentation.DocumentType(@class);
                }
                return true;
            }
            return false;
        }

        public override bool VisitDeclarationContext(DeclarationContext context)
        {
            return context.IsGenerated && base.VisitDeclarationContext(context);
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            if (!base.VisitEnumDecl(@enum))
            {
                return false;
            }
            if (@enum.IsGenerated)
            {
                this.documentation.DocumentEnum(@enum);
                return true;
            }
            return false;
        }

        public override bool VisitFunctionDecl(Function function)
        {
            if (!base.VisitFunctionDecl(function))
            {
                return false;
            }
            if (function.IsGenerated)
            {
                this.DocumentFunction(function);
                return true;
            }
            return false;
        }

        public override bool VisitProperty(Property property)
        {
            if (!base.VisitProperty(property))
            {
                return false;
            }
            if (!property.IsSynthetized && property.IsGenerated)
            {
                this.documentation.DocumentProperty(property);
                return true;
            }
            return false;
        }

        public override bool VisitEvent(Event @event)
        {
            if (!base.VisitEvent(@event))
            {
                return false;
            }
            var function = @event.OriginalDeclaration as Function;
            if (function != null && @event.IsGenerated)
            {
                this.DocumentFunction(function);
                return true;
            }
            return false;
        }

        public override bool VisitVariableDecl(Variable variable)
        {
            // HACK: it doesn't work to call the base as everywhere else because the type of the variable is visited too
            if (this.AlreadyVisited(variable))
            {
                return false;
            }
            base.VisitVariableDecl(variable);
            if (variable.IsGenerated)
            {
                this.documentation.DocumentVariable(variable);
                return true;
            }
            return false;
        }

        private void DocumentFunction(Function function)
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

        private readonly Documentation documentation;
    }
}
