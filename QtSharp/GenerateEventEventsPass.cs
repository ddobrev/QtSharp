using System;
using System.Collections.Generic;
using System.Linq;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace QtSharp
{
    public class GenerateEventEventsPass : TranslationUnitPass
    {
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
            foreach (var block in from template in generatorOutput.Templates
                                  from block in template.FindBlocks(CSharpBlockKind.Method)
                                  select block)
            {
                var method = (Method) block.Declaration;
                if (this.events.Contains(method))
                {
                    var @event = char.ToUpperInvariant(method.OriginalName[0]) + method.OriginalName.Substring(1);
                    var stringBuilder = block.Parent.Blocks[block.Parent.Blocks.IndexOf(block) - 1].Text.StringBuilder;
                    var comment = string.Empty;
                    if (block.Blocks.Count > 0 && block.Blocks[0].Kind == BlockKind.BlockComment)
                    {
                        comment = block.Blocks[0].Text.StringBuilder.ToString();
                    }
                    stringBuilder.AppendLine();
                    stringBuilder.Append(string.Format("{0}public event global::System.Action<object, {1}> {2};{3}",
                         comment, method.Parameters[0].Type, @event, Environment.NewLine));
                    const string eventHandler = @"__eventHandler";
                    var raiseEvent = string.Format(
@"    var {0} = {1};
    if ({0} != null)
        {0}(this, {2});
", eventHandler, @event, method.Parameters[0].Name);
                    stringBuilder = block.Text.StringBuilder;
                    if (method.OriginalReturnType.Type.IsPrimitiveType(PrimitiveType.Void))
                    {
                        stringBuilder.Insert(stringBuilder.Length - 1 - Environment.NewLine.Length, raiseEvent);
                    }
                    else
                    {
                        const string @return = "    return ";
                        stringBuilder.Replace(@return, raiseEvent + @return);
                    }
                }
            }
        }

        public override bool VisitMethodDecl(Method method)
        {
            if (!base.VisitMethodDecl(method))
            {
                return false;
            }

            if (!method.IsConstructor && (method.Name.EndsWith("Event") || method.Name == "event") &&
                method.Parameters.Count == 1 && method.Parameters[0].Type.ToString().EndsWith("Event"))
            {
                var name = char.ToUpperInvariant(method.Name[0]) + method.Name.Substring(1);
                method.Name = "on" + name;
                Method rootBaseMethod;
                if (!method.IsOverride ||
                    (rootBaseMethod = ((Class) method.Namespace).GetBaseMethod(method, true, true)) == null ||
                    rootBaseMethod.IsPure)
                {
                    this.events.Add(method);
                }
            }
            return true;
        }

        private bool eventAdded;
        private readonly HashSet<Method> events = new HashSet<Method>();
    }
}
