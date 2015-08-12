using System;
using QtCore;

namespace QtWidgets
{
    public class QSceneEventHandler : QGraphicsItem
	{
        protected readonly QGraphicsItem sender;
		private readonly object handler;

        public QSceneEventHandler(QGraphicsItem sender, object handler)
		{
			this.sender = sender;
			this.handler = handler;
		}

		public object Handler
		{
			get { return handler; }
		}

        public override QtCore.QRectF BoundingRect
        {
            get { return new QtCore.QRectF(); }
        }

        public override void Paint(QtGui.QPainter painter, QStyleOptionGraphicsItem option, QWidget widget = null)
        {
        }
	}

    public class QSceneEventHandler<T> : QSceneEventHandler where T : QEvent
	{
		private readonly EventHandler<QEventArgs<T>> handler;
		private readonly QEventArgs<T> args;

        public QSceneEventHandler(QGraphicsItem sender, QEventArgs<T> args, EventHandler<QEventArgs<T>> handler)
            : base(sender, handler)
		{
			this.args = args;
			this.handler = handler;
		}

		public new EventHandler<QEventArgs<T>> Handler
		{
			get { return handler; }
		}

        protected override bool SceneEventFilter(IQGraphicsItem arg1, QEvent arg2)
		{
			if (arg1 == sender && 
				(args.EventTypes.Count == 0 || args.EventTypes.Contains(arg2.type) ||
				(args.EventTypes.Contains(QEvent.Type.User) && arg2.type >= QEvent.Type.User)))
			{
				args.Event = (T) arg2;
				handler(sender, args);
				return args.Handled;
			}
            return base.SceneEventFilter(arg1, arg2);
		}
	}
}
