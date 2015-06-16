namespace QtCore
{
    public partial class QChar
    {
        public static implicit operator QChar(char ch)
        {
            return new QChar(ch);
        }
    }
}
