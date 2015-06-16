using System;
using System.Diagnostics;
using System.Linq;
using CppSharp.AST;
using CppSharp.Passes;

namespace QtSharp
{
    public class GetCommentsFromQtDocsPass : TranslationUnitPass
    {
        private readonly Documentation documentation;

        public override bool VisitLibrary(ASTContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var visitLibrary = base.VisitLibrary(context);
            stopwatch.Stop();
            Console.WriteLine("Elapsed time: {0}", stopwatch.ElapsedMilliseconds);
            return visitLibrary;
        }

        public GetCommentsFromQtDocsPass(string docsPath, string module)
        {
            this.documentation = new Documentation(docsPath, module);
            this.Options.VisitFunctionReturnType = false;
            this.Options.VisitFunctionParameters = false;
            this.Options.VisitClassBases = false;
            this.Options.VisitTemplateArguments = false;
            this.Options.VisitClassFields = false;
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
            if (!context.IsGenerated)
                return false;
            return base.VisitDeclarationContext(context);
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            if (!this.AlreadyVisited(@enum) && @enum.IsGenerated)
            {
                this.documentation.DocumentEnum(@enum);
            }
            return base.VisitEnumDecl(@enum);
        }

        public override bool VisitFunctionDecl(Function function)
        {
            if (function.IsGenerated)
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
            if (!this.AlreadyVisited(function) && function.Comment == null)
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
