using System.Runtime.InteropServices;

public unsafe partial class _iobuf
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Internal
    {
        public char* _ptr;
        public int _cnt;
        public char* _base;
        public int _flag;
        public int _file;
        public int _charbuf;
        public int _bufsiz;
        public char* _tmpfname;
    }

    public global::System.IntPtr __Instance { get; protected set; }
}
