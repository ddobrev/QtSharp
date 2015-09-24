using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Util;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace QtSharp
{
    public class GenerateEventEventsPass : TranslationUnitPass
    {
        private bool eventAdded;
        private readonly HashSet<Event> events = new HashSet<Event>();
        private bool addedEventHandlers;

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
                    if (!this.addedEventHandlers && (@class.Name == "QObject" || @class.Name == "QGraphicsItem"))
                    {
                        block.WriteLine("protected readonly System.Collections.Generic.List<{0}> " +
                                        "eventFilters = new System.Collections.Generic.List<{0}>();",
                                        @class.Name == "QObject" ? "QtCore.QEventHandler" : "QtWidgets.QSceneEventHandler");
                        block.NewLine();
                        this.addedEventHandlers = true;
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
                    var @base = @class.GetNonIgnoredRootBase();
                    block.WriteLine(@"public virtual event EventHandler<QtCore.QEventArgs<{0}>> {1}
{{
	add
	{{
		var qEventArgs = new QtCore.QEventArgs<{0}>(new System.Collections.Generic.List<QtCore.QEvent.Type> {{ {2} }});
		var qEventHandler = new {4}<{0}>(this, qEventArgs, value);
        foreach (var eventFilter in eventFilters)
        {{
            this.Remove{3}EventFilter(eventFilter);
        }}
		eventFilters.Add(qEventHandler);
        for (int i = eventFilters.Count - 1; i >= 0; i--)
        {{
		    this.Install{3}EventFilter(eventFilters[i]);
        }}
	}}
	remove
	{{
		for (int i = eventFilters.Count - 1; i >= 0; i--)
		{{
			var eventFilter = eventFilters[i];
			if (eventFilter.Handler == value)
			{{
				this.Remove{3}EventFilter(eventFilter);
				eventFilters.RemoveAt(i);
                break;
			}}
		}}
	}}
}}",
                        @event.Parameters[0].Type, @event.Name, this.GetEventTypes(@event),
                        @base.Name == "QObject" ? string.Empty : "Scene",
                        @base.Name == "QObject" ? "QtCore.QEventHandler" : "QtWidgets.QSceneEventHandler");
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
                method.Parameters.Count == 1 && method.Parameters[0].Type.ToString().EndsWith("Event") &&
                method.OriginalName != "widgetEvent")
            {
                var @event = new Event();
                var name = char.ToUpperInvariant(method.Name[0]) + method.Name.Substring(1);
                @event.Name = name;
                @event.OriginalDeclaration = method;
                @event.Namespace = method.Namespace;
                @event.Parameters.AddRange(method.Parameters);
                method.Namespace.Events.Add(@event);
                this.events.Add(@event);
                method.Name = "on" + name;
            }
            return true;
        }

        private readonly Dictionary<string, List<string>> eventTypes = 
            new Dictionary<string, List<string>>
            {
                { "", new List<string> { "" } },
                { "Action", new List<string> { "ActionAdded", "ActionRemoved", "ActionChanged" } },
                { "Change", new List<string> { "ToolBarChange", "ActivationChange", "EnabledChange",
                                               "FontChange", "StyleChange", "PaletteChange", "WindowTitleChange",
                                               "IconTextChange", "ModifiedChange", "MouseTrackingChange",
                                               "ParentChange", "WindowStateChange", "LanguageChange",
                                               "LocaleChange", "LayoutDirectionChange" } },
                { "Child", new List<string> { "ChildAdded", "ChildPolished", "ChildRemoved" } },
                { "Custom", new List<string> { "User" } },
                { "Focus", new List<string> { "FocusIn", "FocusOut", "FocusAboutToChange" } },
                { "Gesture", new List<string> { "Gesture", "GestureOverride" } },
                { "Help", new List<string> { "ToolTip" } },
                { "Hover", new List<string> { "HoverEnter", "HoverLeave", "HoverMove" } },
                { "MouseDoubleClick", new List<string> { "MouseButtonDblClick" } },
                { "MousePress", new List<string> { "MouseButtonPress" } },
                { "MouseRelease", new List<string> { "MouseButtonRelease" } },
                { "SwallowContextMenu", new List<string> { "ContextMenu" } },
                { "Tablet", new List<string> { "TabletMove", "TabletPress", "TabletRelease",
                                               "TabletEnterProximity", "TabletLeaveProximity" } },
                { "Touch", new List<string> { "TouchBegin", "TouchCancel", "TouchEnd", "TouchUpdate" } },
                { "Viewport", new List<string> { "" } },
                { "Widget", new List<string> { "" } }
        };

        private string GetEventTypes(Event @event)
        {
            string eventName = @event.Name.Substring(0, @event.Name.IndexOf("Event", StringComparison.Ordinal));
            if (this.eventTypes.ContainsKey(eventName))
            {
                return string.Join(", ", from e in this.eventTypes[eventName]
                                         select string.IsNullOrEmpty(e) ? e : "QtCore.QEvent.Type." + e);
            }
            Class @class;
            if ((@event.Parameters[0].Type.GetFinalPointee() ?? @event.Parameters[0].Type).TryGetClass(out @class) &&
                @class.Name == "QEvent")
            {
                return string.Empty;
            }
            return string.Format("QtCore.QEvent.Type.{0}", eventName);
        }
    }
}
