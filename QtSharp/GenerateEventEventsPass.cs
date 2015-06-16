using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Util;
using CppSharp.AST;
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
                    if (!this.addedEventHandlers && @class.Name == "QObject")
                    {
                        block.WriteLine("protected readonly System.Collections.Generic.List<QtCore.QEventHandler> " +
                                        "eventFilters = new System.Collections.Generic.List<QtCore.QEventHandler>();");
                        block.NewLine();
                        this.addedEventHandlers = true;
                    }
                    bool isQAbstractScrollArea = @class.Name != "QAbstractScrollArea";
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
                    block.WriteLine(@"public {0} event EventHandler<QtCore.QEventArgs<{1}>> {2}
{{
	add
	{{
		QtCore.QEventArgs<{1}> qEventArgs = new QtCore.QEventArgs<{1}>(new System.Collections.Generic.List<QEvent.Type> {{ {3} }});
		QtCore.QEventHandler<{1}> qEventHandler = new QtCore.QEventHandler<{1}>(this{4}, qEventArgs, value);
        foreach (QtCore.QEventHandler eventFilter in eventFilters)
        {{
            this{4}.RemoveEventFilter(eventFilter);
        }}
		eventFilters.Add(qEventHandler);
        for (int i = eventFilters.Count - 1; i >= 0; i--)
        {{
		    this{4}.InstallEventFilter(eventFilters[i]);                    
        }}
	}}
	remove
	{{
		for (int i = eventFilters.Count - 1; i >= 0; i--)
		{{
			QtCore.QEventHandler eventFilter = eventFilters[i];
			if (eventFilter.Handler == value)
			{{
				this{4}.RemoveEventFilter(eventFilter);
				eventFilters.RemoveAt(i);
                break;
			}}
		}}
	}}
}}",
                        isQAbstractScrollArea ? "virtual" : "override", @event.Parameters[0].Type, @event.Name,
                        this.GetEventTypes(@event), isQAbstractScrollArea ? string.Empty : ".Viewport");
                }
            }
        }

        public override bool VisitClassDecl(Class @class)
        {
            // HACK: work around the bug about methods in v-tables and methods in classes not being shared objects
            foreach (VTableComponent entry in VTables.GatherVTableMethodEntries(@class))
            {
                if (entry.Method != null && (entry.Method.Name.EndsWith("Event") || entry.Method.Name == "event") &&
                    entry.Method.Parameters.Count == 1 && entry.Method.Parameters[0].Type.ToString().EndsWith("Event") &&
                    !entry.Method.Name.StartsWith("on"))
                {
                    entry.Method.Name = "on" + char.ToUpperInvariant(entry.Method.Name[0]) + entry.Method.Name.Substring(1);
                }
            }
            return base.VisitClassDecl(@class);
        }

        public override bool VisitMethodDecl(Method method)
        {
            if (!method.IsConstructor && (method.Name.EndsWith("Event") || method.Name == "event") &&
                method.Parameters.Count == 1 && method.Parameters[0].Type.ToString().EndsWith("Event"))
            {
                Event @event = new Event();
                if (method.Name.StartsWith("on"))
                {
                    @event.Name = method.Name.Substring(2);                   
                }
                else
                {
                    @event.Name = method.Name;
                }
                @event.OriginalDeclaration = method;
                @event.Namespace = method.Namespace;
                @event.Parameters.AddRange(method.Parameters);
                method.Namespace.Events.Add(@event);
                this.events.Add(@event);
                if (!method.Name.StartsWith("on"))
                    method.Name = "on" + char.ToUpperInvariant(method.Name[0]) + method.Name.Substring(1);
            }
            return base.VisitMethodDecl(method);
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
                { "Hover", new List<string> { "HoverEnter", "HoverLeave", "HoverMove" } },
                { "MouseDoubleClick", new List<string> { "MouseButtonDblClick" } },
                { "MousePress", new List<string> { "MouseButtonPress" } },
                { "MouseRelease", new List<string> { "MouseButtonRelease" } },
                { "SwallowContextMenu", new List<string> { "ContextMenu" } },
                { "Tablet", new List<string> { "TabletMove", "TabletPress", "TabletRelease",
                                               "TabletEnterProximity", "TabletLeaveProximity" } },
                { "Viewport", new List<string> { "" } },
                { "Widget", new List<string> { "" } },
        };

        private string GetEventTypes(Event @event)
        {
            string eventName = @event.Name.Substring(0, @event.Name.IndexOf("Event", StringComparison.Ordinal));
            if (this.eventTypes.ContainsKey(eventName))
            {
                return string.Join(", ", from e in this.eventTypes[eventName]
                                         select string.IsNullOrEmpty(e) ? e : "QEvent.Type." + e);
            }
            if (@event.Parameters[0].Type.ToString() == "QEvent")
            {
                return string.Empty;
            }
            return string.Format("QEvent.Type.{0}", eventName);
        }
    }
}
