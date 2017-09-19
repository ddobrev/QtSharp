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
                parameters[i] = GetParameterValue(handler, i, parameter, arg);
            }
            handler.Delegate.DynamicInvoke(parameters);
            return -1;
        }

        private object GetParameterValue(Handler handler, int i, System.Reflection.ParameterInfo parameter, IntPtr arg)
        {
            if (arg == IntPtr.Zero)
            {
                return null;
            }
            var type = parameter.ParameterType.IsEnum ? parameter.ParameterType.GetEnumUnderlyingType() : parameter.ParameterType;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                    return null;
                case TypeCode.Object:
                    var constructor = type.GetMethod("__CreateInstance",
                                                     global::System.Reflection.BindingFlags.NonPublic |
                                                     global::System.Reflection.BindingFlags.Static |
                                                     global::System.Reflection.BindingFlags.FlattenHierarchy,
                                                     null, new[] { typeof(IntPtr), typeof(bool) }, null);
                    return constructor.Invoke(null, new object[] { arg, false });
                case TypeCode.DBNull:
                    return DBNull.Value;
                case TypeCode.Boolean:
                    return *(bool*) arg;
                case TypeCode.Char:
                    return *(char*) arg;
                case TypeCode.SByte:
                    return *(sbyte*) arg;
                case TypeCode.Byte:
                    return *(byte*) arg;
                case TypeCode.Int16:
                    return *(short*) arg;
                case TypeCode.UInt16:
                    return *(ushort*) arg;
                case TypeCode.Int32:
                    return *(int*) arg;
                case TypeCode.UInt32:
                    return *(uint*) arg;
                case TypeCode.Int64:
                    return *(long*) arg;
                case TypeCode.UInt64:
                    return *(ulong*) arg;
                case TypeCode.Single:
                    return *(float*) arg;
                case TypeCode.Double:
                    return *(double*) arg;
                case TypeCode.Decimal:
                    return *(decimal*) arg;
                case TypeCode.DateTime:
                    return *(DateTime*) arg;
                case TypeCode.String:
                    var metaMethod = this.Sender.MetaObject.Method(handler.SignalId);
                    if (metaMethod.ParameterType(i) == (int) QMetaType.Type.QString)
                    {
                        return Marshal.PtrToStringUni(new IntPtr(QtCore.QString.__CreateInstance(arg).Utf16));
                    }
                    return Marshal.PtrToStringUni(arg);
                default:
                    throw new ArgumentOutOfRangeException(parameter.Name, "Parameter type with invalid type code.");
            }
        }
    }
}