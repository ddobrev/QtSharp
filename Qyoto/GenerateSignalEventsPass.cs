using System;
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
            this.GenerateSignalEvents(generatorOutput);
            OverrideQtMetacall(generatorOutput);
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
        int signalId = MetaObject.IndexOfSignal(QMetaObject.NormalizedSignature(""{2}""));
        Slots.Add(value);
        QMetaObject.Connect(this, signalId, (QObject) value.Target, Slots.Count - 1 + MetaObject.MethodCount, 0, null);
	}}
	remove
	{{
        int i = Slots.IndexOf(value);
        if (i >= 0)
        {{
            int signalId = MetaObject.IndexOfSignal(QMetaObject.NormalizedSignature(""{2}""));
        	QMetaObject.Disconnect(this, signalId, (QObject) value.Target, i + MetaObject.MethodCount);
            Slots.RemoveAt(i);
        }}
	}}
}}", fullNameBuilder, char.ToUpperInvariant(@event.Name[0]) + @event.Name.Substring(1), signature));
                }
            }
        }

        private static void OverrideQtMetacall(GeneratorOutput generatorOutput)
        {
            Block block = (from template in generatorOutput.Templates
                           from b in template.FindBlocks(CSharpBlockKind.Method)
                           where b.Declaration != null
                           let m = (Method) b.Declaration
                           where m.Name == "Qt_metacall" && m.Namespace.Name == "QObject"
                           select b).FirstOrDefault();
            if (block == null)
            {
                return;
            }
            string body = block.Text.StringBuilder.ToString();
            block.Text.StringBuilder.Clear();
            block.WriteLine(@"protected readonly System.Collections.Generic.List<Delegate> Slots = new System.Collections.Generic.List<Delegate>();");
            block.NewLine();
            block.Text.StringBuilder.Append(body.Substring(0, body.IndexOf("return", StringComparison.Ordinal)));
            block.Text.StringBuilder.Append(string.Format(@"
    if (__ret < 0 || {0} != QMetaObject.Call.InvokeMetaMethod)
    {{
        return __ret;
    }}
    IntPtr ptr = new IntPtr(_3);
    Delegate @delegate = Slots[__ret];
    System.Reflection.ParameterInfo[] @params = @delegate.Method.GetParameters();
    IntPtr[] args = new IntPtr[@params.Length];
    Marshal.Copy(ptr, args, 0, args.Length);
    object[] parameters = new object[args.Length];
    for (int i = 0; i < @params.Length; i++)
    {{
        System.Reflection.ParameterInfo parameter = @params[i];
        object value;
        if (parameter.ParameterType.IsValueType)
        {{
            value = Marshal.PtrToStructure(args[i], parameter.ParameterType);
        }}
        else
        {{
            value = Activator.CreateInstance(parameter.ParameterType, args[i]);
        }}
        parameters[i] = value;
    }}
    @delegate.DynamicInvoke(parameters);
    return -1;
}}", ((Method) block.Declaration).Parameters[0].Name));
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
