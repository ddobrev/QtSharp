using System;
using System.Text;

namespace QtSharp.Tests
{
    public static class Helper
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string RandomString(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var builder = new StringBuilder();
            char ch;

            for (var i = 0; i < size; i++)
            {
                ch = (char)random.Next('0', 'z');
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}