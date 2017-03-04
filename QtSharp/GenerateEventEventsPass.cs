using System;
using System.Collections.Generic;
using System.Linq;
using CppSharp;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace QtSharp
{
    public class GenerateEventEventsPass : TranslationUnitPass
    {
        public GenerateEventEventsPass(Generator generator)
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
            var blocks = (from output in generatorOutput.Outputs
                          from block in output.FindBlocks(BlockKind.Method)
                          where this.events.Contains(block.Object)
                          select block).ToList();
            foreach (var block in blocks)
            {
                var method = (Function) block.Object;
                string @event;
                if (((Class) method.Namespace).Methods.Any(m => m != method && m.OriginalName == method.OriginalName))
                {
                    @event = method.OriginalName;
                }
                else
                {
                    @event = char.ToUpperInvariant(method.OriginalName[0]) + method.OriginalName.Substring(1);
                }
                var blockIndex = block.Parent.Blocks.IndexOf(block);
                var eventBlock = new Block(BlockKind.Event);
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
                if (type.TryGetClass(out @class))
                {
                    while (@class.BaseClass != null)
                        @class = @class.BaseClass;
                    if (@class.OriginalName == "QEvent")
                    {
                        var name = char.ToUpperInvariant(method.Name[0]) + method.Name.Substring(1);
                        method.Name = "on" + name;
                        Method baseMethod;
                        if (!method.IsOverride ||
                            (baseMethod = ((Class) method.Namespace).GetBaseMethod(method, true, true)) == null ||
                            baseMethod.IsPure)
                        {
                            this.events.Add(method);
                            this.Context.Options.ExplicitlyPatchedVirtualFunctions.Add(method.QualifiedOriginalName);
                        }
                    }
                }
            }
            return true;
        }

        private bool eventAdded;
        private readonly HashSet<Method> events = new HashSet<Method>();
        private Generator generator;
    }
}
