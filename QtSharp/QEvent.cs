namespace QtCore
{
    public partial class QEvent
    {
        /// <summary>
        /// Set to true in order to stop propagating the event to other widgets.
        /// </summary>
        /// <remarks>
        /// It's different to <see cref="Accepted"/> because it's guaranteed to be false by default and
        /// because setting it on ensures the base event handler is not called.
        /// See https://doc.qt.io/qt-5.6/eventsandfilters.html#event-handlers and https://doc.qt.io/archives/qq/qq11-events.html#acceptorignore.
        /// </remarks>
        public bool Handled { get; set; }
    }
}
