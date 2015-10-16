namespace QtGui
{
    /// <summary>
    /// <para>The QAccessibleActionInterface class implements support for invocable actions in the interface.</para>
    /// </summary>
    /// <remarks>
    /// <para>Accessible objects should implement the action interface if they support user interaction. Usually this interface is implemented by classes that also implement QAccessibleInterface.</para>
    /// <para>The supported actions should use the predefined actions offered in this class unless they do not fit a predefined action. In that case a custom action can be added.</para>
    /// <para>When subclassing QAccessibleActionInterface you need to provide a list of actionNames which is the primary means to discover the available actions. Action names are never localized. In order to present actions to the user there are two functions that need to return localized versions of the name and give a description of the action. For the predefined action names use QAccessibleActionInterface::localizedActionName() and QAccessibleActionInterface::localizedActionDescription() to return their localized counterparts.</para>
    /// <para>In general you should use one of the predefined action names, unless describing an action that does not fit these:</para>
    /// <para></para>
    /// <para>Action nameDescription</para>
    /// <para>toggleAction()	toggles the item (checkbox, radio button, switch, ...)</para>
    /// <para>decreaseAction()	decrease the value of the accessible (e.g. spinbox)</para>
    /// <para>increaseAction()	increase the value of the accessible (e.g. spinbox)</para>
    /// <para>pressAction()	press or click or activate the accessible (should correspont to clicking the object with the mouse)</para>
    /// <para>setFocusAction()	set the focus to this accessible</para>
    /// <para>showMenuAction()	show a context menu, corresponds to right-clicks</para>
    /// <para></para>
    /// <para>In order to invoke the action, doAction() is called with an action name.</para>
    /// <para>Most widgets will simply implement pressAction(). This is what happens when the widget is activated by being clicked, space pressed or similar.</para>
    /// <para>IAccessible2 Specification</para>
    /// </remarks>
    public unsafe partial interface IQAccessibleActionInterface
    {
        /// <summary>
        /// <para>Returns a localized action name of actionName.</para>
        /// <para>For custom actions this function has to be re-implemented. When using one of the default names, you can call this function in QAccessibleActionInterface to get the localized string.</para>
        /// <para>See also actionNames() and localizedActionDescription().</para>
        /// </summary>
        string LocalizedActionName(string actionName);

        /// <summary>
        /// <para>Returns a localized action description of the action actionName.</para>
        /// <para>When using one of the default names, you can call this function in QAccessibleActionInterface to get the localized string.</para>
        /// <para>See also actionNames() and localizedActionName().</para>
        /// </summary>
        string LocalizedActionDescription(string actionName);

        /// <summary>
        /// <para>Invokes the action specified by actionName. Note that actionName is the non-localized name as returned by actionNames() This function is usually implemented by calling the same functions that other user interaction, such as clicking the object, would trigger.</para>
        /// <para>See also actionNames().</para>
        /// </summary>
        void DoAction(string actionName);

        /// <summary>
        /// <para>Returns a list of the keyboard shortcuts available for invoking the action named actionName.</para>
        /// <para>This is important to let users learn alternative ways of using the application by emphasizing the keyboard.</para>
        /// <para>See also actionNames().</para>
        /// </summary>
        QtCore.QStringList KeyBindingsForAction(string actionName);

        global::System.IntPtr __Instance { get; }

        /// <summary>
        /// <para>Returns the list of actions supported by this accessible object. The actions returned should be in preferred order, i.e. the action that the user most likely wants to trigger should be returned first, while the least likely action should be returned last.</para>
        /// <para>The list does only contain actions that can be invoked. It won't return disabled actions, or actions associated with disabled UI controls.</para>
        /// <para>The list can be empty.</para>
        /// <para>Note that this list is not localized. For a localized representation re-implement localizedActionName() and localizedActionDescription()</para>
        /// <para>See also doAction(), localizedActionName(), and localizedActionDescription().</para>
        /// </summary>
        QtCore.QStringList ActionNames { get; }
    }
}