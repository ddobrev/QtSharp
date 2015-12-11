namespace QtQml
{    /// <summary>
     /// <para>The QQmlParserStatus class provides updates on the QML parser state.</para>
     /// </summary>
     /// <remarks>
     /// <para>QQmlParserStatus provides a mechanism for classes instantiated by a QQmlEngine to receive notification at key points in their creation.</para>
     /// <para>This class is often used for optimization purposes, as it allows you to defer an expensive operation until after all the properties have been set on an object. For example, QML's Text element uses the parser status to defer text layout until all of its properties have been set (we don't want to layout when the text is assigned, and then relayout when the font is assigned, and relayout again when the width is assigned, and so on).</para>
     /// <para>Be aware that QQmlParserStatus methods are only called when a class is instantiated by a QQmlEngine. If you create the same class directly from C++, these methods will not be called automatically. To avoid this problem, it is recommended that you start deferring operations from classBegin instead of from the initial creation of your class. This will still prevent multiple revaluations during initial binding assignment in QML, but will not defer operations invoked from C++.</para>
     /// <para>To use QQmlParserStatus, you must inherit both a QObject-derived class and QQmlParserStatus, and use the Q_INTERFACES() macro.</para>
     /// <para>class MyObject : public QObject, public QQmlParserStatus</para>
     /// <para>{</para>
     /// <para>    Q_OBJECT</para>
     /// <para>    Q_INTERFACES(QQmlParserStatus)</para>
     /// <para></para>
     /// <para>public:</para>
     /// <para>    MyObject(QObject *parent = 0);</para>
     /// <para>    ...</para>
     /// <para>    void classBegin();</para>
     /// <para>    void componentComplete();</para>
     /// <para>}</para>
     /// <para>The Qt Quick 1 version of this class is named QDeclarativeParserStatus.</para>
     /// </remarks>
    public unsafe partial interface IQQmlParserStatus
    {
        /// <summary>
        /// <para>Invoked after class creation, but before any properties have been set.</para>
        /// </summary>
        void ClassBegin();

        /// <summary>
        /// <para>Invoked after the root component that caused this instantiation has completed construction. At this point all static values and binding values have been assigned to the class.</para>
        /// </summary>
        void ComponentComplete();
    }
}
