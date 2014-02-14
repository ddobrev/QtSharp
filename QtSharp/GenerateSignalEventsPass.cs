using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Util;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace QtSharp
{
    public class GenerateSignalEventsPass : TranslationUnitPass
    {
        private bool eventAdded;
        private readonly HashSet<Event> events = new HashSet<Event>();
        private bool addedDynamicQObject;

        public override bool VisitTranslationUnit(TranslationUnit unit)
        {
            if (!this.eventAdded)
            {
                this.Driver.Generator.OnUnitGenerated += this.OnUnitGenerated;
                this.eventAdded = true;
            }
            return base.VisitTranslationUnit(unit);
        }

        private void OnUnitGenerated(GeneratorOutput generatorOutput)
        {
            this.GenerateSignalEvents(generatorOutput);
        }

        private void GenerateSignalEvents(GeneratorOutput generatorOutput)
        {
            foreach (Block block in from template in generatorOutput.Templates
                                    from block in template.FindBlocks(CSharpBlockKind.Event)
                                    select block)
            {
                Event @event = (Event) block.Declaration;
                if (this.events.Contains(@event))
                {
                    block.Text.StringBuilder.Clear();
                    Class @class = (Class) @event.Namespace;
                    if (!this.addedDynamicQObject && @class.Name == "QObject")
                    {
                        block.WriteLine(@"protected DynamicQObject dynamicQObject;");
                        block.NewLine();
                        block.WriteLine("protected DynamicQObject __DynamicQObject");
                        block.WriteStartBraceIndent();
                        block.WriteLine("get");
                        block.WriteStartBraceIndent();
                        block.WriteLine("if (dynamicQObject == null)");
                        block.WriteStartBraceIndent();
                        block.WriteLine("dynamicQObject = new DynamicQObject(null);");
                        block.WriteCloseBraceIndent();
                        block.WriteLine("return dynamicQObject;");
                        block.WriteCloseBraceIndent();
                        block.WriteCloseBraceIndent();
                        block.NewLine();
                        this.addedDynamicQObject = true;
                    }

                    int argNum = 1;
                    StringBuilder fullNameBuilder = new StringBuilder("Action");
                    foreach (Parameter parameter in @event.Parameters)
                    {
                        argNum++;
                        if (argNum == 2)
                        {
                            fullNameBuilder.Append('<');
                        }
                        fullNameBuilder.Append(parameter.Type);
                        fullNameBuilder.Append(',');
                    }
                    if (fullNameBuilder[fullNameBuilder.Length - 1] == ',')
                    {
                        fullNameBuilder[fullNameBuilder.Length - 1] = '>';
                    }
                    string signature = string.Format("{0}({1})", @event.OriginalName,
                        string.Join(", ",
                            from e in @event.Parameters
                            select GetOriginalParameterType(e)));
                    Event existing = @class.Events.FirstOrDefault(e => e.Name == @event.Name);
                    if (existing != null && existing != @event)
                    {
                        if (@event.Parameters.Count > 0)
                        {
                            @event.Name += GetSignalEventSuffix(@event);
                        }
                        else
                        {
                            existing.Name += GetSignalEventSuffix(@event);
                        }
                    }
                    else
                    {
                        if (@event.Parameters.Count > 0 &&
                            (@class.Methods.Any(m => m.IsGenerated && m.OriginalName == @event.Name) ||
                             @class.Properties.Any(p => p.IsGenerated && p.OriginalName == @event.Name)))
                        {
                            @event.Name += GetSignalEventSuffix(@event);
                        }
                    }
                    if (@event.OriginalDeclaration.Comment != null)
                    {
                        block.WriteLine("/// <summary>");
                        foreach (string line in HtmlEncoder.HtmlEncode(@event.OriginalDeclaration.Comment.BriefText).Split(
                                                    Environment.NewLine.ToCharArray()))
                        {
                            block.WriteLine("/// <para>{0}</para>", line);
                        }
                        block.WriteLine("/// </summary>");
                    }
                    block.WriteLine(string.Format(@"public event {0} {1}
{{
	add
	{{
        __DynamicQObject.ConnectDynamicSlot(this, ""{2}"", value);
	}}
	remove
	{{
        __DynamicQObject.DisconnectDynamicSlot(this, ""{2}"", value);
	}}
}}", fullNameBuilder, char.ToUpperInvariant(@event.Name[0]) + @event.Name.Substring(1), signature));
                }
            }
        }

        private static string GetOriginalParameterType(ITypedDecl parameter)
        {
            Declaration decl;
            return parameter.Type.IsTagDecl(out decl) ? decl.QualifiedOriginalName : parameter.Type.ToString();
        }

        public override bool VisitClassDecl(Class @class)
        {
            if (this.AlreadyVisited(@class))
            {
                return false;
            }
            foreach (Method method in from method in @class.Methods
                                      let access = method.AccessDecl
                                      where access != null
                                      select method)
            {
                this.HandleQSignal(@class, method);
            }
            return true;
        }

        private void HandleQSignal(DeclarationContext @class, Method method)
        {
            AccessSpecifierDecl access = method.AccessDecl;

            IEnumerable<MacroExpansion> expansions = access.PreprocessedEntities.OfType<MacroExpansion>();
            if (expansions.All(e => e.Text != "Q_SIGNALS"))
            {
                return;
            }
            if (method.Parameters.Any())
            {
                Declaration decl;
                if (method.Parameters.Last().Type.IsTagDecl(out decl) && decl.Name == "QPrivateSignal")
                {
                    method.Parameters.RemoveAt(method.Parameters.Count - 1);
                }
            }
            FunctionType functionType = method.GetFunctionType();

            Event @event = new Event
                            {
                                OriginalDeclaration = method,
                                Name = method.Name,
                                OriginalName = method.OriginalName,
                                Namespace = method.Namespace,
                                QualifiedType = new QualifiedType(functionType),
                                Parameters = method.Parameters
                            };
            method.IsGenerated = false;
            @class.Events.Add(@event);
            this.events.Add(@event);
        }

        private static string GetSignalEventSuffix(Event signalToUse)
        {
            string suffix = signalToUse.Parameters.Last().Name;
            int indexOfSpace = suffix.IndexOf(' ');
            if (indexOfSpace > 0)
            {
                suffix = suffix.Substring(0, indexOfSpace);
            }
            if (suffix.StartsWith("_"))
            {
                string lastType = signalToUse.Parameters.Last().Type.ToString();
                suffix = lastType.Substring(lastType.LastIndexOf('.') + 1);
            }
            else
            {
                StringBuilder lastParamBuilder = new StringBuilder(suffix);
                while (!char.IsLetter(lastParamBuilder[0]))
                {
                    lastParamBuilder.Remove(0, 1);
                }
                lastParamBuilder[0] = char.ToUpper(lastParamBuilder[0]);
                suffix = lastParamBuilder.ToString();
            }
            return suffix;
        }
    }
}
