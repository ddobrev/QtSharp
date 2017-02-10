using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Util;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace QtSharp
{
    public class GenerateSignalEventsPass : TranslationUnitPass
    {
        private bool eventAdded;
        private readonly HashSet<Event> events = new HashSet<Event>();
        private Generator generator;

        public GenerateSignalEventsPass(Generator generator)
        {
            this.generator = generator;
        }

        public override bool VisitTranslationUnit(TranslationUnit unit)
        {
            if (!this.eventAdded)
            {
                this.generator.OnUnitGenerated += this.OnUnitGenerated;
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
            foreach (var block in from output in generatorOutput.Outputs
                                  from block in output.FindBlocks(CSharpBlockKind.Event)
                                  select block)
            {
                Event @event = (Event) block.Object;
                if (this.events.Contains(@event))
                {
                    block.Text.StringBuilder.Clear();
                    Class @class = (Class) @event.Namespace;

                    int argNum = 1;
                    StringBuilder fullNameBuilder = new StringBuilder("global::System.Action");
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
                    var finalName = char.ToUpperInvariant(@event.Name[0]) + @event.Name.Substring(1);
                    if (@event.Namespace.Declarations.Exists(d => d != @event && d.Name == finalName))
                    {
                        finalName += "Signal";
                    }
                    block.WriteLine(string.Format(@"public event {0} {1}
{{
	add
	{{
        ConnectDynamicSlot(this, ""{2}"", value);
	}}
	remove
	{{
        DisconnectDynamicSlot(this, ""{2}"", value);
	}}
}}", fullNameBuilder, finalName, signature));
                }
            }
            var qtMetacall = (from output in generatorOutput.Outputs
                              from block in output.FindBlocks(CSharpBlockKind.Method)
                              let declaration = block.Object as Declaration
                              where declaration != null && declaration.Name == "QtMetacall" &&
                                    declaration.Namespace.Name == "QObject"
                              select block).FirstOrDefault();
            if (qtMetacall != null)
            {
                qtMetacall.Text.StringBuilder.Replace("return __ret;", "return HandleQtMetacall(__ret, _0, _2);");
            }
        }

        private static string GetOriginalParameterType(ITypedDecl parameter)
        {
            Class decl;
            return parameter.Type.Desugar().SkipPointerRefs().Desugar().TryGetClass(out decl)
                ? decl.QualifiedOriginalName
                : parameter.Type.ToString();
        }

        public override bool VisitClassDecl(Class @class)
        {
            if (this.AlreadyVisited(@class))
            {
                return false;
            }
            Declaration decl;
            foreach (var method in @class.Methods.Where(m => m.IsGenerated ||
                (m.Parameters.Any() && m.Parameters.Last().Type.Desugar().TryGetDeclaration(out decl) &&
                 decl.OriginalName == "QPrivateSignal")))
            {
                this.HandleQSignal(@class, method);
            }
            var qtMetaCall = @class.FindMethod("qt_metacall");
            if (qtMetaCall != null)
            {
                this.Context.Options.ExplicitlyPatchedVirtualFunctions.Add(qtMetaCall.QualifiedOriginalName);
            }
            return true;
        }

        private void HandleQSignal(Class @class, Method method)
        {
            for (int i = 0; i < @class.Specifiers.Count; i++)
            {
                var accessSpecifierDecl = @class.Specifiers[i];
                if (accessSpecifierDecl.DebugText == "Q_SIGNALS:" &&
                    accessSpecifierDecl.LineNumberStart < method.LineNumberStart &&
                    (i == @class.Specifiers.Count - 1 || method.LineNumberEnd <= @class.Specifiers[i + 1].LineNumberStart))
                {
                    if (method.Parameters.Any())
                    {
                        Class decl;
                        if (method.Parameters.Last().Type.Desugar().TryGetClass(out decl) && decl.Name == "QPrivateSignal")
                        {
                            method.Parameters.RemoveAt(method.Parameters.Count - 1);
                        }
                    }

                    var @event = new Event
                    {
                        OriginalDeclaration = method,
                        Name = method.Name,
                        OriginalName = method.OriginalName,
                        Namespace = method.Namespace,
                        QualifiedType = new QualifiedType(method.FunctionType.Type),
                        Parameters = method.Parameters
                    };
                    if (method.IsGenerated)
                    {
                        method.ExplicitlyIgnore();
                    }
                    @class.Events.Add(@event);
                    this.events.Add(@event);
                    return;
                }
            }
        }

        private static string GetSignalEventSuffix(Event signalToUse)
        {
            var suffix = signalToUse.Parameters.Last().Name;
            var indexOfSpace = suffix.IndexOf(' ');
            if (indexOfSpace > 0)
            {
                suffix = suffix.Substring(0, indexOfSpace);
            }
            if (suffix.StartsWith("_", StringComparison.Ordinal))
            {
                var lastType = signalToUse.Parameters.Last().Type.ToString();
                suffix = lastType.Substring(lastType.LastIndexOf('.') + 1);
                suffix = char.ToUpperInvariant(suffix[0]) + suffix.Substring(1);
            }
            else
            {
                var lastParamBuilder = new StringBuilder(suffix);
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
