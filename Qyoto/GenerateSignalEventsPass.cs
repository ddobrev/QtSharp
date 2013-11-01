using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace Qyoto
{
    public class GenerateSignalEventsPass : TranslationUnitPass
    {
        private bool eventAdded;
        private readonly HashSet<Event> events = new HashSet<Event>();

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
            foreach (Block block in from template in generatorOutput.Templates
                from block in template.FindBlocks(CSharpBlockKind.Event)
                select block)
            {
                Event @event = (Event) block.Declaration;
                if (this.events.Contains(@event))
                {
                    block.Text.StringBuilder.Clear();

                    Class @class = (Class) @event.Namespace;
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
                    string signature = string.Format("{0}({1})", @event.Name,
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
                            (@class.Methods.Any(m => m.OriginalName == @event.Name) ||
                             @class.Properties.Any(p => p.OriginalName == @event.Name)))
                        {
                            @event.Name += GetSignalEventSuffix(@event);
                        }
                    }
                    block.WriteLine(string.Format(@"
public event {0} {1}
{{
	add
	{{
        QObject.Connect(this, ""2{2}"", (QObject) value.Target, ""1"" + value.Method.Name + ""{3}"", QtCore.Qt.ConnectionType.AutoConnection);
	}}
	remove
	{{
        QObject.Disconnect(this, ""2{2}"", (QObject) value.Target, ""1"" + value.Method.Name + ""{3}"");
	}}
}}", fullNameBuilder, char.ToUpperInvariant(@event.Name[0]) + @event.Name.Substring(1), signature, signature.Substring(signature.IndexOf('('))));
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
            FunctionType functionType = new FunctionType
                                        {
                                            Parameters = method.Parameters,
                                            ReturnType = method.ReturnType
                                        };

            Event @event = new Event
                            {
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
                lastParamBuilder[0] = char.ToUpper(lastParamBuilder[0]);
                suffix = lastParamBuilder.ToString();
            }
            return suffix;
        }
    }
}
