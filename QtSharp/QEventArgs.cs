using System;
using System.Collections.Generic;

namespace QtCore
{
	public class QEventArgs<T> : EventArgs where T : QEvent
	{
		public QEventArgs(ICollection<QEvent.Type> eventTypes)
		{
			this.EventTypes = eventTypes;
		}

		public ICollection<QEvent.Type> EventTypes { get; private set; }

		public bool Handled { get; set; }

		public T Event { get; set; }
	}
}
