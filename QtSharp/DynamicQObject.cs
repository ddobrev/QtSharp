using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QtCore
{
    public class DynamicQObject : QObject
    {
        private readonly List<Delegate> slots = new List<Delegate>();

        public DynamicQObject(QObject parent) : base(parent)
        {
        }

        public unsafe bool ConnectDynamicSlot(QObject sender, string signal, Delegate slot)
        {
            this.slots.Add(slot);
            int signalId = sender.MetaObject.IndexOfSignal(QMetaObject.NormalizedSignature(signal));
            QMetaObject.Connection connection = QMetaObject.Connect(sender, signalId, this, this.slots.Count - 1 + MetaObject.MethodCount, 0, null);
            return connection != null;
        }

        public bool DisconnectDynamicSlot(QObject sender, string signal, Delegate value)
        {
            int i = this.slots.IndexOf(value);
            if (i >= 0)
            {
                int signalId = sender.MetaObject.IndexOfSignal(QMetaObject.NormalizedSignature(signal));
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
            Delegate @delegate = this.slots[__ret];
            System.Reflection.ParameterInfo[] @params = @delegate.Method.GetParameters();
            object[] parameters = new object[@params.Length];
            for (int i = 0; i < @params.Length; i++)
            {
                System.Reflection.ParameterInfo parameter = @params[i];
                var arg = new IntPtr((int*) _3 + 1 + i);
                object value;
                if (parameter.ParameterType.IsValueType)
                {
                    value = Marshal.PtrToStructure(arg, parameter.ParameterType);
                }
                else
                {
                    if (parameter.ParameterType.IsAssignableFrom(typeof(string)))
                    {
                        // TODO: must properly handle QString here
                        value = Marshal.PtrToStringUni(arg);
                    }
                    else
                    {
                        value = Activator.CreateInstance(parameter.ParameterType, arg);                        
                    }
                }
                parameters[i] = value;
            }
            @delegate.DynamicInvoke(parameters);
            return -1;
        }
    }
}
