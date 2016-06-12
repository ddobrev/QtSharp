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
            var blocks = (from template in generatorOutput.Templates
                          from block in template.FindBlocks(CSharpBlockKind.Method)
                          where this.events.Contains(block.Declaration)
                          select block).ToList();
            foreach (var block in blocks)
            {
                var method = (Function) block.Declaration;
                var @event = char.ToUpperInvariant(method.OriginalName[0]) + method.OriginalName.Substring(1);
                var blockIndex = block.Parent.Blocks.IndexOf(block);
                var eventBlock = new Block(CSharpBlockKind.Event);
                eventBlock.WriteLine("public event global::System.Action<object, {0}> {1};",
                                     method.Parameters[0].Type, @event);
                eventBlock.NewLine();
                const string eventHandler = @"__eventHandler";
                var raiseEvent = string.Format(
                    @"var {0} = {1};
    if ({0} != null)
        {0}(this, {2});
    if ({2}.Handled)
        return{3};
",
                    eventHandler, @event, method.Parameters[0].Name,
                    method.OriginalReturnType.Type.IsPrimitiveType(PrimitiveType.Void) ? string.Empty : " true");
                if (block.Blocks.Count > 0 && block.Blocks[0].Kind == BlockKind.BlockComment)
                {
                    eventBlock.Blocks.Add(block.Blocks[0]);
                }
                block.Parent.Blocks.Insert(blockIndex, eventBlock);
                block.Text.StringBuilder.Replace("var __slot", raiseEvent + "    var __slot");
            }
        }

        public override bool VisitMethodDecl(Method method)
        {
            if (!base.VisitMethodDecl(method))
            {
                return false;
            }

            if (!method.IsConstructor && (method.Name.EndsWith("Event", StringComparison.Ordinal) || method.Name == "event") &&
                method.Parameters.Count == 1)
            {
                var type = method.Parameters[0].Type;
                type = type.GetFinalPointee() ?? type;
                Class @class;
                if (type.TryGetClass(out @class) && @class.Name.EndsWith("Event", StringComparison.Ordinal))
                {
                    var name = char.ToUpperInvariant(method.Name[0]) + method.Name.Substring(1);
                    method.Name = "on" + name;
                    Method baseMethod;
                    if (!method.IsOverride ||
                        (baseMethod = ((Class) method.Namespace).GetBaseMethod(method, true, true)) == null ||
                        baseMethod.IsPure)
                    {
                        this.events.Add(method);
                        this.Driver.Options.ExplicitlyPatchedVirtualFunctions.Add(method.QualifiedOriginalName);
                    }
                }
            }
            return true;
        }

        private bool eventAdded;
        private readonly HashSet<Method> events = new HashSet<Method>();
    }
}
