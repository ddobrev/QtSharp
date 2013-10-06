using System;
using System.Collections.Generic;
using System.Linq;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace QtSharp
{
    public class GenerateEventEventsPass : TranslationUnitPass
    {
        private readonly HashSet<Event> events = new HashSet<Event>();

        public override bool VisitTranslationUnit(TranslationUnit unit)
        {
            if (Driver.Generator.OnUnitGenerated != OnUnitGenerated)
                Driver.Generator.OnUnitGenerated = OnUnitGenerated;
            return base.VisitTranslationUnit(unit);
        }

        private void OnUnitGenerated(GeneratorOutput generatorOutput)
        {
            foreach (Block block in from template in generatorOutput.Templates
                                    from block in template.FindBlocks(CSharpBlockKind.Event)
                                    select block)
            {
                Event @event = (Event) block.Declaration;
                if (events.Contains(@event))
                {
                    block.Text.StringBuilder.Clear();
                    const string eventFilters = "eventFilters";
                    Class @class = (Class) @event.Namespace;
                    if (@class.Fields.Find(m => m.Name == eventFilters) == null)
                    {
                        Field field = new Field { Name = eventFilters, Namespace = @event.Namespace };
                        field.Access = AccessSpecifier.Private;
                        @class.Fields.Add(field);
                        block.WriteLine(@"private readonly List<QEventHandler> {0} = new List<QEventHandler>();",
                                        eventFilters);
                        block.NewLine();
                    }
                    bool isQAbstractScrollArea = @class.Name != "QAbstractScrollArea";
                    block.WriteLine(@"public {0} event EventHandler<QEventArgs<{1}>> {2}
{{
	add
	{{
		QEventArgs<{1}> qEventArgs = new QEventArgs<{1}>(new List<QEvent.Type> {{ {3} }});
		QEventHandler<{1}> qEventHandler = new QEventHandler<{1}>(this{4}, qEventArgs, value);
        foreach (QEventHandler eventFilter in eventFilters)
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
			QEventHandler eventFilter = eventFilters[i];
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
                        GetEventTypes(@event), isQAbstractScrollArea ? string.Empty : ".Viewport");
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
                @event.Namespace = method.Namespace;
                @event.Parameters.AddRange(method.Parameters);
                method.Namespace.Events.Add(@event);
                events.Add(@event);
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
            if (eventTypes.ContainsKey(eventName))
            {
                return string.Join(", ", from e in eventTypes[eventName]
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
