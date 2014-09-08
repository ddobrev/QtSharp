using System;
using System.Runtime.InteropServices;
using System.Security;

namespace QtCore
{
    /// <summary>
    /// <para>The QString class provides a Unicode character string.</para>
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// <para>QString stores a string of 16-bit QChars, where each QChar
    /// corresponds one Unicode 4.0 character. (Unicode characters with code values
    /// above 65535 are stored using surrogate pairs, i.e., two consecutive
    /// QChars.)</para>
    /// <para>Unicode is an international standard that supports most of the
    /// writing systems in use today. It is a superset of US-ASCII (ANSI X3.4-1986)
    /// and Latin-1 (ISO 8859-1), and all the US-ASCII/Latin-1 characters are
    /// available at the same code positions.</para>
    /// <para>Behind the scenes, QString uses implicit sharing (copy-on-write)
    /// to reduce memory usage and to avoid the needless copying of data. This also
    /// helps reduce the inherent overhead of storing 16-bit characters instead of
    /// 8-bit characters.</para>
    /// <para>In addition to QString, Qt also provides the QByteArray class to
    /// store raw bytes and traditional 8-bit '\\0'-terminated strings. For most
    /// purposes, QString is the class you want to use. It is used throughout the
    /// Qt API, and the Unicode support ensures that your applications will be easy
    /// to translate if you want to expand your application's market at some point.
    /// The two main cases where QByteArray is appropriate are when you need to
    /// store raw binary data, and when memory conservation is critical (like in
    /// embedded systems).</para>
    /// <para></para>
    /// <para>Initializing a String</para>
    /// <para>One way to initialize a QString is simply to pass a const char *
    /// to its constructor. For example, the following code creates a QString of
    /// size 5 containing the data &quot;Hello&quot;:</para>
    /// <para>QString str = &quot;Hello&quot;;</para>
    /// <para>QString converts the const char * data into Unicode using the
    /// fromUtf8() function.</para>
    /// <para>In all of the QString functions that take const char *
    /// parameters, the const char * is interpreted as a classic C-style
    /// '\\0'-terminated string encoded in UTF-8. It is legal for the const char *
    /// parameter to be 0.</para>
    /// <para>You can also provide string data as an array of QChars:</para>
    /// <para>static const QChar data[4] = { 0x0055, 0x006e, 0x10e3, 0x03a3
    /// };</para>
    /// <para>QString str(data, 4);</para>
    /// <para>QString makes a deep copy of the QChar data, so you can modify it
    /// later without experiencing side effects. (If for performance reasons you
    /// don't want to take a deep copy of the character data, use
    /// QString::fromRawData() instead.)</para>
    /// <para>Another approach is to set the size of the string using resize()
    /// and to initialize the data character per character. QString uses 0-based
    /// indexes, just like C++ arrays. To access the character at a particular
    /// index position, you can use operator[](). On non-const strings,
    /// operator[]() returns a reference to a character that can be used on the
    /// left side of an assignment. For example:</para>
    /// <para>QString str;</para>
    /// <para>str.resize(4);</para>
    /// <para></para>
    /// <para>str[0] = QChar('U');</para>
    /// <para>str[1] = QChar('n');</para>
    /// <para>str[2] = QChar(0x10e3);</para>
    /// <para>str[3] = QChar(0x03a3);</para>
    /// <para>For read-only access, an alternative syntax is to use the at()
    /// function:</para>
    /// <para>QString str;</para>
    /// <para></para>
    /// <para>for (int i = 0; i &lt; str.size(); ++i) {</para>
    /// <para>    if (str.at(i) &gt;= QChar('a') &amp;&amp; str.at(i) &lt;=
    /// QChar('f'))</para>
    /// <para>        qDebug() &lt;&lt; &quot;Found character in range
    /// [a-f]&quot;;</para>
    /// <para>}</para>
    /// <para>The at() function can be faster than operator[](), because it
    /// never causes a deep copy to occur. Alternatively, use the left(), right(),
    /// or mid() functions to extract several characters at a time.</para>
    /// <para>A QString can embed '\\0' characters (QChar::Null). The size()
    /// function always returns the size of the whole string, including embedded
    /// '\\0' characters.</para>
    /// <para>After a call to the resize() function, newly allocated characters
    /// have undefined values. To set all the characters in the string to a
    /// particular value, use the fill() function.</para>
    /// <para>QString provides dozens of overloads designed to simplify string
    /// usage. For example, if you want to compare a QString with a string literal,
    /// you can write code like this and it will work as expected:</para>
    /// <para>QString str;</para>
    /// <para></para>
    /// <para>if (str == &quot;auto&quot; || str == &quot;extern&quot;</para>
    /// <para>        || str == &quot;static&quot; || str ==
    /// &quot;register&quot;) {</para>
    /// <para>    // ...</para>
    /// <para>}</para>
    /// <para>You can also pass string literals to functions that take QStrings
    /// as arguments, invoking the QString(const char *) constructor. Similarly,
    /// you can pass a QString to a function that takes a const char * argument
    /// using the qPrintable() macro which returns the given QString as a const
    /// char *. This is equivalent to calling
    /// &lt;QString&gt;.toLocal8Bit().constData().</para>
    /// <para></para>
    /// <para>Manipulating String Data</para>
    /// <para>QString provides the following basic functions for modifying the
    /// character data: append(), prepend(), insert(), replace(), and remove(). For
    /// example:</para>
    /// <para>QString str = &quot;and&quot;;</para>
    /// <para>str.prepend(&quot;rock &quot;);     // str == &quot;rock
    /// and&quot;</para>
    /// <para>str.append(&quot; roll&quot;);        // str == &quot;rock and
    /// roll&quot;</para>
    /// <para>str.replace(5, 3, &quot;&amp;&quot;);   // str == &quot;rock
    /// &amp; roll&quot;</para>
    /// <para>If you are building a QString gradually and know in advance
    /// approximately how many characters the QString will contain, you can call
    /// reserve(), asking QString to preallocate a certain amount of memory. You
    /// can also call capacity() to find out how much memory QString actually
    /// allocated.</para>
    /// <para>The replace() and remove() functions' first two arguments are the
    /// position from which to start erasing and the number of characters that
    /// should be erased. If you want to replace all occurrences of a particular
    /// substring with another, use one of the two-parameter replace()
    /// overloads.</para>
    /// <para>A frequent requirement is to remove whitespace characters from a
    /// string ('\\n', '\\t', ' ', etc.). If you want to remove whitespace from
    /// both ends of a QString, use the trimmed() function. If you want to remove
    /// whitespace from both ends and replace multiple consecutive whitespaces with
    /// a single space character within the string, use simplified().</para>
    /// <para>If you want to find all occurrences of a particular character or
    /// substring in a QString, use the indexOf() or lastIndexOf() functions. The
    /// former searches forward starting from a given index position, the latter
    /// searches backward. Both return the index position of the character or
    /// substring if they find it; otherwise, they return -1. For example, here's a
    /// typical loop that finds all occurrences of a particular substring:</para>
    /// <para>QString str = &quot;We must be &lt;b&gt;bold&lt;/b&gt;, very
    /// &lt;b&gt;bold&lt;/b&gt;&quot;;</para>
    /// <para>int j = 0;</para>
    /// <para></para>
    /// <para>while ((j = str.indexOf(&quot;&lt;b&gt;&quot;, j)) != -1)
    /// {</para>
    /// <para>    qDebug() &lt;&lt; &quot;Found &lt;b&gt; tag at index
    /// position&quot; &lt;&lt; j;</para>
    /// <para>    ++j;</para>
    /// <para>}</para>
    /// <para>QString provides many functions for converting numbers into
    /// strings and strings into numbers. See the arg() functions, the setNum()
    /// functions, the number() static functions, and the toInt(), toDouble(), and
    /// similar functions.</para>
    /// <para>To get an upper- or lowercase version of a string use toUpper()
    /// or toLower().</para>
    /// <para>Lists of strings are handled by the QStringList class. You can
    /// split a string into a list of strings using the split() function, and join
    /// a list of strings into a single string with an optional separator using
    /// QStringList::join(). You can obtain a list of strings from a string list
    /// that contain a particular substring or that match a particular QRegExp
    /// using the QStringList::filter() function.</para>
    /// <para></para>
    /// <para>Querying String Data</para>
    /// <para>If you want to see if a QString starts or ends with a particular
    /// substring use startsWith() or endsWith(). If you simply want to check
    /// whether a QString contains a particular character or substring, use the
    /// contains() function. If you want to find out how many times a particular
    /// character or substring occurs in the string, use count().</para>
    /// <para>QStrings can be compared using overloaded operators such as
    /// operator&lt;(), operator&lt;=(), operator==(), operator&gt;=(), and so on.
    /// Note that the comparison is based exclusively on the numeric Unicode values
    /// of the characters. It is very fast, but is not what a human would expect;
    /// the QString::localeAwareCompare() function is a better choice for sorting
    /// user-interface strings.</para>
    /// <para>To obtain a pointer to the actual character data, call data() or
    /// constData(). These functions return a pointer to the beginning of the QChar
    /// data. The pointer is guaranteed to remain valid until a non-const function
    /// is called on the QString.</para>
    /// <para></para>
    /// <para>Converting Between 8-Bit Strings and Unicode Strings</para>
    /// <para>QString provides the following three functions that return a
    /// const char * version of the string as QByteArray: toUtf8(), toLatin1(), and
    /// toLocal8Bit().</para>
    /// <para></para>
    /// <para>toLatin1() returns a Latin-1 (ISO 8859-1) encoded 8-bit
    /// string.</para>
    /// <para>toUtf8() returns a UTF-8 encoded 8-bit string. UTF-8 is a
    /// superset of US-ASCII (ANSI X3.4-1986) that supports the entire Unicode
    /// character set through multibyte sequences.</para>
    /// <para>toLocal8Bit() returns an 8-bit string using the system's local
    /// encoding.</para>
    /// <para></para>
    /// <para>To convert from one of these encodings, QString provides
    /// fromLatin1(), fromUtf8(), and fromLocal8Bit(). Other encodings are
    /// supported through the QTextCodec class.</para>
    /// <para>As mentioned above, QString provides a lot of functions and
    /// operators that make it easy to interoperate with const char * strings. But
    /// this functionality is a double-edged sword: It makes QString more
    /// convenient to use if all strings are US-ASCII or Latin-1, but there is
    /// always the risk that an implicit conversion from or to const char * is done
    /// using the wrong 8-bit encoding. To minimize these risks, you can turn off
    /// these implicit conversions by defining the following two preprocessor
    /// symbols:</para>
    /// <para></para>
    /// <para>QT_NO_CAST_FROM_ASCII disables automatic conversions from C
    /// string literals and pointers to Unicode.</para>
    /// <para>QT_NO_CAST_TO_ASCII disables automatic conversion from QString to
    /// C strings.</para>
    /// <para></para>
    /// <para>One way to define these preprocessor symbols globally for your
    /// application is to add the following entry to your qmake project
    /// file:</para>
    /// <para>DEFINES += QT_NO_CAST_FROM_ASCII \\</para>
    /// <para>           QT_NO_CAST_TO_ASCII</para>
    /// <para>You then need to explicitly call fromUtf8(), fromLatin1(), or
    /// fromLocal8Bit() to construct a QString from an 8-bit string, or use the
    /// lightweight QLatin1String class, for example:</para>
    /// <para>QString url =
    /// QLatin1String(&quot;http://www.unicode.org/&quot;);</para>
    /// <para>Similarly, you must call toLatin1(), toUtf8(), or toLocal8Bit()
    /// explicitly to convert the QString to an 8-bit string. (Other encodings are
    /// supported through the QTextCodec class.)</para>
    /// <para></para>
    /// <para> Note for C Programmers</para>
    /// <para>Due to C++'s type system and the fact that QString is implicitly
    /// shared, QStrings may be treated like ints or other basic types. For
    /// example:QString Widget::boolToString(bool b)</para>
    /// <para>{</para>
    /// <para>    QString result;</para>
    /// <para>    if (b)</para>
    /// <para>        result = &quot;True&quot;;</para>
    /// <para>    else</para>
    /// <para>        result = &quot;False&quot;;</para>
    /// <para>    return result;</para>
    /// <para>}</para>
    /// <para>The result variable, is a normal variable allocated on the stack.
    /// When return is called, and because we're returning by value, the copy
    /// constructor is called and a copy of the string is returned. No actual
    /// copying takes place thanks to the implicit sharing.</para>
    /// </remarks>
    internal unsafe partial class QString
    {
        public partial struct Internal
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("Qt5Cored", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.ThisCall,
                EntryPoint = "_ZN7QStringC2ERKS_")]
            internal static extern void ctor_5(global::System.IntPtr instance, global::System.IntPtr other);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("Qt5Cored", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.Cdecl,
                EntryPoint = "_ZN7QString9fromUtf16EPKti")]
            internal static extern void FromUtf16_0(global::System.IntPtr __return, ushort* unicode, int size);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("Qt5Cored", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.ThisCall,
                EntryPoint = "_ZNK7QString5utf16Ev")]
            internal static extern ushort* Utf16_0(global::System.IntPtr instance);
        }

        public global::System.IntPtr __Instance { get; protected set; }

        internal QString(QString.Internal* native)
            : this(new global::System.IntPtr(native))
        {
        }

        private static global::System.IntPtr CopyQStringValue(QString.Internal native)
        {
            global::System.IntPtr ret = Marshal.AllocHGlobal(4);
            *(QString.Internal*) ret = native;
            return ret;
        }

        internal QString(QString.Internal native)
            : this(CopyQStringValue(native))
        {
        }

        public QString(global::System.IntPtr native)
        {
            __Instance = native;
        }

        /// <summary>
        /// <para>Returns a QString initialized with the first size characters of
        /// the Unicode string unicode (ISO-10646-UTF-16 encoded).</para>
        /// <para>If size is -1 (default), unicode must be terminated with a
        /// 0.</para>
        /// <para>This function checks for a Byte Order Mark (BOM). If it is
        /// missing, host byte order is assumed.</para>
        /// <para>This function is slow compared to the other Unicode conversions.
        /// Use QString(const QChar *, int) or QString(const QChar *) if
        /// possible.</para>
        /// <para>QString makes a deep copy of the Unicode data.</para>
        /// <para>See also utf16() and setUtf16().</para>
        /// </summary>
        public static QString FromUtf16(ushort* unicode, int size)
        {
            var arg0 = unicode;
            var __ret = new QtCore.QString.Internal();
            Internal.FromUtf16_0(new IntPtr(&__ret), arg0, size);
            var __instance = Marshal.AllocHGlobal(4);
            QString.Internal.ctor_5(__instance, new global::System.IntPtr(&__ret));
            return new QString(__instance);
        }

        /// <summary>
        /// <para>Returns the QString as a '\\0'-terminated array of unsigned
        /// shorts. The result remains valid until the string is modified.</para>
        /// <para>The returned string is in host byte order.</para>
        /// <para>See also setUtf16() and unicode().</para>
        /// </summary>
        public ushort* Utf16
        {
            get
            {
                var __ret = Internal.Utf16_0(__Instance);
                return __ret;
            }
        }
    }
}
