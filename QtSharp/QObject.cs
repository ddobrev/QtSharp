using System;
using System.Runtime.InteropServices;

namespace QtCore
{
    public unsafe partial class QObject
    {
        private struct Handler
        {
            public Handler(int signalId, Delegate @delegate)
                : this()
            {
                this.SignalId = signalId;
                this.Delegate = @delegate;
            }

            public int SignalId { get; private set; }
            public Delegate Delegate { get; private set; }
        }

        private readonly System.Collections.Generic.List<Handler> slots = new System.Collections.Generic.List<Handler>();

        protected unsafe bool ConnectDynamicSlot(QObject sender, string signal, Delegate slot)
        {
            int signalId = sender.MetaObject.IndexOfSignal(QMetaObject.NormalizedSignature(signal));
            this.slots.Add(new Handler(signalId, slot));
            QMetaObject.Connection connection = QMetaObject.Connect(sender, signalId, this, this.slots.Count - 1 + MetaObject.MethodCount);
            return connection != null;
        }

        protected bool DisconnectDynamicSlot(QObject sender, string signal, Delegate value)
        {
            int i = this.slots.FindIndex(h => h.Delegate == value);
            if (i >= 0)
            {
                int signalId = this.slots[i].SignalId;
                bool disconnect = QMetaObject.Disconnect(sender, signalId, this, i + MetaObject.MethodCount);
                this.slots.RemoveAt(i);
                return disconnect;
            }
            return false;
        }

        protected int HandleQtMetacall(int index, QMetaObject.Call call, void** arguments)
        {
            if (index < 0 || call != QMetaObject.Call.InvokeMetaMethod)
            {
                return index;
            }
            Handler handler = this.slots[index];
            System.Reflection.ParameterInfo[] @params = handler.Delegate.Method.GetParameters();
            object[] parameters = new object[@params.Length];
            for (int i = 0; i < @params.Length; i++)
            {
                System.Reflection.ParameterInfo parameter = @params[i];
                var arg = new IntPtr(arguments[1 + i]);
                object value;
                if (parameter.ParameterType.IsValueType)
                {
                    value = Marshal.PtrToStructure(arg, parameter.ParameterType);
                }
                else
                {
                    if (parameter.ParameterType.IsAssignableFrom(typeof(string)))
                    {
                        var metaMethod = this.Sender.MetaObject.Method(handler.SignalId);
                        if (metaMethod.ParameterType(i) == (int) QMetaType.Type.QString)
                        {
                            value = Marshal.PtrToStringUni(new IntPtr(new QString(arg).Utf16));
                        }
                        else
                        {
                            value = Marshal.PtrToStringUni(arg);
                        }
                    }
                    else
                    {
                        value = Activator.CreateInstance(parameter.ParameterType, arg);
                    }
                }
                parameters[i] = value;
            }
            handler.Delegate.DynamicInvoke(parameters);
            return -1;
        }
    }
}