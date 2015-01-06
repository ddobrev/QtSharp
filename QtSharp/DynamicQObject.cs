using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace QtCore
{
    public class DynamicQObject : QObject
    {
        private struct Handler
        {
            public Handler(int signalId, Delegate @delegate) : this()
            {
                this.SignalId = signalId;
                this.Delegate = @delegate;
            }

            public int SignalId { get; private set; }
            public Delegate Delegate { get; private set; }
        }

        private readonly List<Handler> slots = new List<Handler>();

        public DynamicQObject(QObject parent) : base(parent)
        {
        }

        public unsafe bool ConnectDynamicSlot(QObject sender, string signal, Delegate slot)
        {
            int signalId = sender.MetaObject.IndexOfSignal(QMetaObject.NormalizedSignature(signal));
            this.slots.Add(new Handler(signalId, slot));
            QMetaObject.Connection connection = QMetaObject.Connect(sender, signalId, this, this.slots.Count - 1 + MetaObject.MethodCount);
            return connection != null;
        }

        public bool DisconnectDynamicSlot(QObject sender, string signal, Delegate value)
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

        public override unsafe int Qt_metacall(QMetaObject.Call _1, int _2, void** _3)
        {
            var arg0 = _1;
            var arg2 = _3;
            var __ret = base.Qt_metacall(arg0, _2, arg2);

            if (__ret < 0 || _1 != QMetaObject.Call.InvokeMetaMethod)
            {
                return __ret;
            }
            Handler handler = this.slots[__ret];
            ParameterInfo[] @params = handler.Delegate.Method.GetParameters();
            object[] parameters = new object[@params.Length];
            for (int i = 0; i < @params.Length; i++)
            {
                ParameterInfo parameter = @params[i];
                var arg = new IntPtr(_3[1 + i]);
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
