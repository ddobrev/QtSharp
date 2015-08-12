using System;

namespace QtCore
{
	public class QEventHandler : QObject
	{
		protected readonly QObject sender;
		private readonly object handler;

		public QEventHandler(QObject sender, object handler)
		{
			this.sender = sender;
			this.handler = handler;
		}

		public object Handler
		{
			get { return handler; }
		}
	}

	public class QEventHandler<T> : QEventHandler where T : QEvent
	{
		private readonly EventHandler<QEventArgs<T>> handler;
		private readonly QEventArgs<T> args;

		public QEventHandler(QObject sender, QEventArgs<T> args, EventHandler<QEventArgs<T>> handler) : base(sender, handler)
		{
			this.args = args;
			this.handler = handler;
		}

		public new EventHandler<QEventArgs<T>> Handler
		{
			get { return handler; }
		}

		public override bool EventFilter(QObject arg1, QEvent arg2)
		{
			if (arg1 == sender && 
				(args.EventTypes.Count == 0 || args.EventTypes.Contains(arg2.type) ||
				(args.EventTypes.Contains(QEvent.Type.User) && arg2.type >= QEvent.Type.User)))
			{
				args.Event = (T) arg2;
				handler(sender, args);
				return args.Handled;
			}
			return base.EventFilter(arg1, arg2);
		}
	}
}
