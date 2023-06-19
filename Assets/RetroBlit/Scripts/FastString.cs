using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// A class for handling strings without the costs of GC
/// </summary>
/// <remarks>
/// A class for handling strings without the costs of garbage collection. Using C# *string* type can have a significant impact
/// on the garbage collector because most string manipulation operations cause reallocation of the underlying
/// string buffer. <see cref="FastString"/> works exclusively with a single pre-allocated buffer and will not
/// cause any unexpected garbage collection.
///
/// All RetroBlit methods that take in a *string* parameter also have a <see cref="FastString"/> overload that can be used instead.
/// <seedoc>Features:String GC</seedoc>
/// <seedoc>Features:Avoid GC with Fast String</seedoc>
/// </remarks>
public class FastString : IEquatable<FastString>
{
    /// <summary>
    /// Undefined float precision specifier
    /// </summary>
    /// <remarks>
    /// Default float precision specifier.
    /// <see cref="FastString.PRECISION_UNDEFINED"/> is the default precision specifier for floating values. This precision
    /// specifier will trim leading 0s from the decimal places.
    ///
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// const double PI = 3.1415927;
    ///
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // This will print "3.1415927"
    ///     fstr.Set(PI, FastString.PRECISION_UNDEFINED);
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // This will print "3.14"
    ///     fstr.Set(PI, 2);
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    /// }
    /// </code>
    /// <seealso cref="FastString.Append"/>
    /// <seealso cref="FastString.Clear"/>
    public const int PRECISION_UNDEFINED = 999;

    /// <summary>
    /// Format hex numbers with lower-case alpha characters
    /// </summary>
    /// <remarks>
    /// Format hex numbers with lower-case alpha characters.
    ///
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // This will print "0xdead"
    ///     fstr.Set("0x").Append(0xDEAD, 0, FastString.FORMAT_HEX_SMALL);
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // This will print "0xDEAD"
    ///     fstr.Set("0x").Append(0xDEAD, 0, FastString.FORMAT_HEX_CAPS);
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    /// }
    /// </code>
    /// <seealso cref="FastString.Append"/>
    /// <seealso cref="FastString.Clear"/>
    /// <seealso cref="FastString.FORMAT_HEX_CAPS"/>
    public const int FORMAT_HEX_SMALL = 1 << 0;

    /// <summary>
    /// Format hex numbers with upper-case alpha characters
    /// </summary>
    /// <remarks>
    /// Format hex numbers with upper-case alpha characters.
    ///
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // This will print "0xdead"
    ///     fstr.Set("0x").Append(0xDEAD, 0, FastString.FORMAT_HEX_SMALL);
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // This will print "0xDEAD"
    ///     fstr.Set("0x").Append(0xDEAD, 0, FastString.FORMAT_HEX_CAPS);
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    /// }
    /// </code>
    /// <seealso cref="FastString.Append"/>
    /// <seealso cref="FastString.Clear"/>
    /// <seealso cref="FastString.FORMAT_HEX_SMALL"/>
    public const int FORMAT_HEX_CAPS = 1 << 1;

    /// <summary>
    /// Fill leading number spaces with zeros
    /// </summary>
    /// <remarks>
    /// This flag can be used to fill leading number spaces with zeros when appending a number with minimum digits specified.
    ///
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // This will print "123"
    ///     fstr.Set(123);
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // This will print "00123"
    ///     fstr.Set(123, 5, FastString.FILL_ZEROS);
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    ///
    ///     // This will print "  123"
    ///     fstr.Set(123, 5, FastString.FILL_SPACES);
    ///     RB.Print(new Vector2i(0, 40), Color.white, fstr);
    /// }
    /// </code>
    /// <seealso cref="FastString.Append"/>
    /// <seealso cref="FastString.Clear"/>
    /// <seealso cref="FastString.FILL_SPACES"/>
    public const int FILL_ZEROS = 1 << 2;

    /// <summary>
    /// Fill leading number spaces with spaces
    /// </summary>
    /// <remarks>
    /// This flag can be used to fill leading number spaces with spaces when appending a number with minimum digits specified.
    ///
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // This will print "123"
    ///     fstr.Set(123);
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // This will print "00123"
    ///     fstr.Set(123, 5, FastString.FILL_ZEROS);
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    ///
    ///     // This will print "  123"
    ///     fstr.Set(123, 5, FastString.FILL_SPACES);
    ///     RB.Print(new Vector2i(0, 40), Color.white, fstr);
    /// }
    /// </code>
    /// <seealso cref="FastString.Append"/>
    /// <seealso cref="FastString.Clear"/>
    /// <seealso cref="FastString.FILL_ZEROS"/>
    public const int FILL_SPACES = 1 << 3;

    private const int FORCE_NEGATIVE = 1 << 16;

    private static readonly Mutex mMutex = new Mutex();

    private static readonly char[] mSmallHex = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
    private static readonly char[] mCapsHex = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

    private readonly char[] mChars;
    private readonly uint mCapacity = 0;

    private readonly char[] mBuf = new char[128];

    private int mLength = 0;

    private char[] mFloatBuf = new char[32];
    private readonly static double[] mPowers10 = new double[] {
        1,
        10,
        100,
        1000,
        10000,
        100000, //    , Wheeeeee!
        1000000, //  O
        10000000, //  \/
        100000000, //  \_
        1000000000, //
        10000000000, //
        100000000000,
        1000000000000,
        10000000000000,
        100000000000000,
        1000000000000000, // Max double before precision loss
    };

    private const double PRINTF_MAX_FLOAT = 1e9;
    private const int MAX_FLOAT_PRECISION = 9;
    private const int DEFAULT_PRECISION = 9;

    [StructLayout(LayoutKind.Explicit)]
    struct DoubleBreakdown
    {
        [FieldOffset(0)]
        public ulong U;
        [FieldOffset(0)]
        public double F;
    };

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// Create a new FastString with the *maxCapacity* specified. The string will never be able to exceed the
    /// specified capacity, but its contents can be smaller.
    ///
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <param name="maxCapacity">Maximum capacity of the string</param>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     fstr.Set("Hello there ").Append(playerName).Append("!");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    /// }
    /// </code>
    /// <seealso cref="FastString.Append"/>
    /// <seealso cref="FastString.Clear"/>
    public FastString(uint maxCapacity)
    {
        mChars = new char[maxCapacity];
        mCapacity = maxCapacity;
        mLength = 0;
    }

    /// <summary>
    /// Capacity of string
    /// </summary>
    /// <remarks>
    /// The maximum capacity of the string, as specified in the constructor. This is equivalent of <see cref="FastString.Buf"/><b>.Length</b>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <seealso cref="FastString.Length"/>
    /// <seealso cref="FastString.Buf"/>
    public int Capacity
    {
        get
        {
            return (int)mCapacity;
        }
    }

    /// <summary>
    /// Length of string
    /// </summary>
    /// <remarks>
    /// The current length of the string content, which could be equal to or smaller than <see cref="FastString.Capacity"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <seealso cref="FastString.Capacity"/>
    public int Length
    {
        get
        {
            return mLength;
        }
    }

    /// <summary>
    /// The char buffer of the string
    /// </summary>
    /// <remarks>
    /// The char buffer of the string. Characters in the buffer are undefined past <see cref="FastString.Length"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    public char[] Buf
    {
        get
        {
            return mChars;
        }
    }

    /// <summary>
    /// Get character at given index.
    /// </summary>
    /// <remarks>
    /// Get character at given index. Characters are undefined past <see cref="FastString.Length"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <param name="index">Index of char</param>
    /// <returns>Character</returns>
    /// <seealso cref="FastString.Length"/>
    /// <seealso cref="FastString.Buf"/>
    public char this[int index]
    {
        get
        {
            return mChars[index];
        }

        set
        {
            mChars[index] = value;
        }
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <remarks>
    /// Equality operator.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(FastString a, object b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <remarks>
    /// Inequality operator.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(FastString a, object b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Append a value to the string
    /// </summary>
    /// <remarks>
    /// Appends a value to the string. Many different kinds of values can be appended, including strings, numbers, vectors, rects, and colors.
    ///
    /// When appending a *string* or another <see cref="FastString"/> a substring can be specified using the *startIndex* and *len* parameters.
    ///
    /// When appending numbers, *minDigits* specify the minimum amount of digits that should be appended, with padding added as necessary. Padding type
    /// is specified with *flags*, which can be either <see cref="FastString.FILL_SPACES"/>, or <see cref="FastString.FILL_ZEROS"/>.
    ///
    /// When appending floating point numbers the decimal point precision can be specified with *precision*, the default value is <see cref="FastString.PRECISION_UNDEFINED"/> for maximum
    /// precision.
    ///
    /// The <see cref="FastString.Append"/> method returns *this*, which allows multiple append calls to be chained together.
    ///
    /// The string can be cleared with <see cref="FastString.Clear"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     fstr.Set("Hello there ").Append(playerName).Append("!");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    /// }
    /// </code>
    /// <param name="str">Source string</param>
    /// <param name="startIndex">Start index, default 0</param>
    /// <param name="len">Length, defaults to end of string</param>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Clear"/>
    /// <seealso cref="FastString.Truncate"/>
    public FastString Append(string str, int startIndex = 0, int len = int.MaxValue)
    {
        if (str == null || str.Length == 0 || startIndex < 0 || len <= 0)
        {
            return this;
        }

        int endIndex = startIndex + len;
        if (endIndex > str.Length - 1)
        {
            endIndex = str.Length - 1;
        }

        for (int i = startIndex; i <= endIndex && mLength < mCapacity; i++, mLength++)
        {
            mChars[mLength] = str[i];
        }

        return this;
    }

    /// <summary>
    /// Append a string with optional start index and length
    /// </summary>
    /// <param name="str">Source string</param>
    /// <param name="startIndex">Start index, default 0</param>
    /// <param name="len">Length, defaults to end of string</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(FastString str, int startIndex = 0, int len = int.MaxValue)
    {
        if (str == null || str.Length == 0 || startIndex < 0 || len <= 0)
        {
            return this;
        }

        int endIndex = startIndex + len;
        if (endIndex > str.Length - 1)
        {
            endIndex = str.Length - 1;
        }

        for (int i = startIndex; i <= endIndex && mLength < mCapacity; i++, mLength++)
        {
            mChars[mLength] = str[i];
        }

        return this;
    }

    /// <summary>
    /// Append a char
    /// </summary>
    /// <param name="c">Char</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(char c)
    {
        if (mLength < mCapacity - 1)
        {
            mChars[mLength++] = c;
        }

        return this;
    }

    /// <summary>
    /// Append a long value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(long val, uint minDigits = 0, uint flags = 0)
    {
        mMutex.WaitOne();

        int charsWritten;
        LongToBuffer(val, flags, out charsWritten);

        char fillChar = '0';
        if ((flags & FILL_SPACES) != 0)
        {
            fillChar = ' ';
        }

        for (uint i = 0; i < (minDigits - charsWritten) && mLength < mCapacity; i++)
        {
            mChars[mLength++] = fillChar;
        }

        for (int i = mBuf.Length - charsWritten; i < mBuf.Length && mLength < mCapacity; i++, mLength++)
        {
            mChars[mLength] = mBuf[i];
        }

        mMutex.ReleaseMutex();

        return this;
    }

    /// <summary>
    /// Append an unsigned long value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(ulong val, uint minDigits = 0, uint flags = 0)
    {
        mMutex.WaitOne();

        int charsWritten;
        ULongToBuffer(val, flags, out charsWritten);

        char fillChar = '0';
        if ((flags & FILL_SPACES) != 0)
        {
            fillChar = ' ';
        }

        for (uint i = 0; i < (minDigits - charsWritten) && mLength < mCapacity; i++)
        {
            mChars[mLength++] = fillChar;
        }

        for (int i = mBuf.Length - charsWritten; i < mBuf.Length && mLength < mCapacity; i++, mLength++)
        {
            mChars[mLength] = mBuf[i];
        }

        mMutex.ReleaseMutex();

        return this;
    }

    /// <summary>
    /// Append an integer value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(int val, uint minDigits = 0, uint flags = 0)
    {
        mMutex.WaitOne();

        int charsWritten;
        IntToBuffer(val, flags, out charsWritten);

        char fillChar = '0';
        if ((flags & FILL_SPACES) != 0)
        {
            fillChar = ' ';
        }

        for (uint i = 0; i < (minDigits - charsWritten) && mLength < mCapacity; i++)
        {
            mChars[mLength++] = fillChar;
        }

        for (int i = mBuf.Length - charsWritten; i < mBuf.Length && mLength < mCapacity; i++, mLength++)
        {
            mChars[mLength] = mBuf[i];
        }

        mMutex.ReleaseMutex();

        return this;
    }

    /// <summary>
    /// Append an unsigned integer value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(uint val, uint minDigits = 0, uint flags = 0)
    {
        mMutex.WaitOne();

        int charsWritten;
        UIntToBuffer(val, flags, out charsWritten);

        char fillChar = '0';
        if ((flags & FILL_SPACES) != 0)
        {
            fillChar = ' ';
        }

        for (uint i = 0; i < (minDigits - charsWritten) && mLength < mCapacity; i++)
        {
            mChars[mLength++] = fillChar;
        }

        for (int i = mBuf.Length - charsWritten; i < mBuf.Length && mLength < mCapacity; i++, mLength++)
        {
            mChars[mLength] = mBuf[i];
        }

        mMutex.ReleaseMutex();

        return this;
    }

    /// <summary>
    /// Append a short value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(short val, uint minDigits = 0, uint flags = 0)
    {
        return Append((int)val, minDigits, flags);
    }

    /// <summary>
    /// Append an unsigned short value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(ushort val, uint minDigits = 0, uint flags = 0)
    {
        return Append((uint)val, minDigits, flags);
    }

    /// <summary>
    /// Append a signed byte value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(sbyte val, uint minDigits = 0, uint flags = 0)
    {
        return Append((int)val, minDigits, flags);
    }

    /// <summary>
    /// Append a byte value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(byte val, uint minDigits = 0, uint flags = 0)
    {
        return Append((uint)val, minDigits, flags);
    }

    /// <summary>
    /// Append a float value, maximum supported integral portion is long.MaxValue
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="precision">Decimal precision</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(float val, uint precision = PRECISION_UNDEFINED)
    {
        if (precision > PRECISION_UNDEFINED) precision = PRECISION_UNDEFINED;

        mMutex.WaitOne();
        HandleDouble(val, precision);
        mMutex.ReleaseMutex();

        return this;
    }

    /// <summary>
    /// Append a double value, maximum supported integral portion is long.MaxValue
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="precision">Decimal precision</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(double val, uint precision = PRECISION_UNDEFINED)
    {
        if (precision > PRECISION_UNDEFINED) precision = PRECISION_UNDEFINED;

        mMutex.WaitOne();
        HandleDouble(val, precision);
        mMutex.ReleaseMutex();

        return this;
    }

    /// <summary>
    /// Append a <see cref="Vector2i"/> in the format (x, y)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Vector2i val)
    {
        Append('(');
        Append(val.x);
        Append(',');
        Append(' ');
        Append(val.y);
        Append(')');

        return this;
    }

    /// <summary>
    /// Append <see cref="UnityEngine.Vector2"/> in the format (x, y)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Vector2 val)
    {
        Append('(');
        Append(val.x);
        Append(',');
        Append(' ');
        Append(val.y);
        Append(')');

        return this;
    }

    /// <summary>
    /// Append <see cref="UnityEngine.Vector3"/> in the format (x, y, z)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Vector3 val)
    {
        Append('(');
        Append(val.x);
        Append(',');
        Append(' ');
        Append(val.y);
        Append(',');
        Append(' ');
        Append(val.z);
        Append(')');

        return this;
    }

    /// <summary>
    /// Append <see cref="UnityEngine.Vector4"/> in the format (x, y, z, w)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Vector4 val)
    {
        Append('(');
        Append(val.x);
        Append(',');
        Append(' ');
        Append(val.y);
        Append(',');
        Append(' ');
        Append(val.z);
        Append(',');
        Append(' ');
        Append(val.w);
        Append(')');

        return this;
    }

    /// <summary>
    /// Append <see cref="Rect2i"/> in the format (x, y, width, height)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Rect2i val)
    {
        Append('(');
        Append(val.x);
        Append(',');
        Append(' ');
        Append(val.y);
        Append(',');
        Append(' ');
        Append(val.width);
        Append(',');
        Append(' ');
        Append(val.height);
        Append(')');

        return this;
    }

    /// <summary>
    /// Append <see cref="UnityEngine.Rect"/> in the format (x, y, width, height)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Rect val)
    {
        Append('(');
        Append(val.x);
        Append(',');
        Append(' ');
        Append(val.y);
        Append(',');
        Append(' ');
        Append(val.width);
        Append(',');
        Append(' ');
        Append(val.height);
        Append(')');

        return this;
    }

    /// <summary>
    /// Append <see cref="Color32"/>. The format depends on the <paramref name="flags"/> value.
    /// For <see cref="FastString.FORMAT_HEX_SMALL"/> format is in small hex alpha numerics, eg 3a60abff.
    /// For <see cref="FastString.FORMAT_HEX_CAPS"/> format is in small hex alpha numerics, eg 3A60ABFF.
    /// If <paramref name="flags"/> is 0 format is (r, g, b, a)
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="flags">0 or one of <see cref="FastString.FORMAT_HEX_SMALL"/> or <see cref="FastString.FORMAT_HEX_CAPS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Color32 val, uint flags = 0)
    {
        if ((flags & FORMAT_HEX_CAPS) != 0)
        {
            Append(val.r, 2, FORMAT_HEX_CAPS);
            Append(val.g, 2, FORMAT_HEX_CAPS);
            Append(val.b, 2, FORMAT_HEX_CAPS);
            Append(val.a, 2, FORMAT_HEX_CAPS);
        }
        else if ((flags & FORMAT_HEX_SMALL) != 0)
        {
            Append(val.r, 2, FORMAT_HEX_SMALL);
            Append(val.g, 2, FORMAT_HEX_SMALL);
            Append(val.b, 2, FORMAT_HEX_SMALL);
            Append(val.a, 2, FORMAT_HEX_SMALL);
        }
        else
        {
            Append('(');
            Append(val.r);
            Append(',');
            Append(' ');
            Append(val.g);
            Append(',');
            Append(' ');
            Append(val.b);
            Append(',');
            Append(' ');
            Append(val.a);
            Append(')');
        }

        return this;
    }

    /// <summary>
    /// Append <see cref="Color"/>. The format depends on the <paramref name="flags"/> value.
    /// For <see cref="FastString.FORMAT_HEX_SMALL"/> format is in small hex alpha numerics, eg 3a60abff.
    /// For <see cref="FastString.FORMAT_HEX_CAPS"/> format is in small hex alpha numerics, eg 3A60ABFF.
    /// If <paramref name="flags"/> is 0 format is (r, g, b, a)
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="flags">0 or one of <see cref="FastString.FORMAT_HEX_SMALL"/> or <see cref="FastString.FORMAT_HEX_CAPS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Append(Color val, uint flags = 0)
    {
        if ((flags & FORMAT_HEX_CAPS) != 0)
        {
            Append((Color32)val, flags);
        }
        else if ((flags & FORMAT_HEX_SMALL) != 0)
        {
            Append((Color32)val, flags);
        }
        else
        {
            Append('(');
            Append(val.r);
            Append(',');
            Append(' ');
            Append(val.g);
            Append(',');
            Append(' ');
            Append(val.b);
            Append(',');
            Append(' ');
            Append(val.a);
            Append(')');
        }

        return this;
    }

    /// <summary>
    /// Clear the string and set it to a new value.
    /// </summary>
    /// <remarks>
    /// Sets a value of the string. Many different kinds of values can be set, including strings, numbers, vectors, rects, and colors.
    ///
    /// When setting to a *string* or another <see cref="FastString"/> a substring can be specified using the *startIndex* and *len* parameters.
    ///
    /// When setting to numbers, *minDigits* specify the minimum amount of digits that should be added, with padding added as necessary. Padding type
    /// is specified with *flags*, which can be either <see cref="FastString.FILL_SPACES"/>, or <see cref="FastString.FILL_ZEROS"/>.
    ///
    /// When setting to floating point numbers the decimal point precision can be specified with *precision*, the default value is <see cref="FastString.PRECISION_UNDEFINED"/>.
    ///
    /// The <see cref="FastString.Set"/> method returns *this*, which allows it to be chained with other <see cref="FastString"/> methods such as <see cref="FastString.Append"/>.
    ///
    /// This is equivalent of calling <see cref="FastString.Clear"/> followed by <see cref="FastString.Append"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     fstr.Set("Hello there ").Append(playerName).Append("!");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    /// }
    /// </code>
    /// <param name="str">Source string</param>
    /// <param name="startIndex">Start index, default 0</param>
    /// <param name="len">Length, defaults to end of string</param>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Clear"/>
    /// <seealso cref="FastString.Truncate"/>
    public FastString Set(string str, int startIndex = 0, int len = int.MaxValue)
    {
        return Clear().Append(str, startIndex, len);
    }

    /// <summary>
    /// Set to a string value.
    /// Append a string with optional start index and length
    /// </summary>
    /// <param name="str">Source string</param>
    /// <param name="startIndex">Start index, default 0</param>
    /// <param name="len">Length, defaults to end of string</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(FastString str, int startIndex = 0, int len = int.MaxValue)
    {
        return Clear().Append(str, startIndex, len);
    }

    /// <summary>
    /// Set to a char value.
    /// </summary>
    /// <param name="c">Char</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(char c)
    {
        return Clear().Append(c);
    }

    /// <summary>
    /// Set a long value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(long val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set an unsigned long value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(ulong val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set an integer value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(int val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set an unsigned integer value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(uint val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set a short value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(short val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set an unsigned short value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(ushort val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set a signed byte value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(sbyte val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set a byte value with optional minimum digits. If the value has less digits than <paramref name="minDigits"/> then it will be filled with spaces or zeros depends on value of <paramref name="flags"/>.
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="minDigits">Minimum digits</param>
    /// <param name="flags">One of <see cref="FastString.FILL_SPACES"/> or <see cref="FastString.FILL_ZEROS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(byte val, uint minDigits = 0, uint flags = 0)
    {
        return Clear().Append(val, minDigits, flags);
    }

    /// <summary>
    /// Set a float value, maximum supported integral portion is long.MaxValue
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="precision">Decimal precision</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(float val, uint precision = PRECISION_UNDEFINED)
    {
        return Clear().Append(val, precision);
    }

    /// <summary>
    /// Set a double value, maximum supported integral portion is long.MaxValue
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="precision">Decimal precision</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(double val, uint precision = PRECISION_UNDEFINED)
    {
        return Clear().Append(val, precision);
    }

    /// <summary>
    /// Set a <see cref="Vector2i"/> in the format (x, y)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Vector2i val)
    {
        return Clear().Append(val);
    }

    /// <summary>
    /// Set <see cref="UnityEngine.Vector2"/> in the format (x, y)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Vector2 val)
    {
        return Clear().Append(val);
    }

    /// <summary>
    /// Set <see cref="UnityEngine.Vector3"/> in the format (x, y, z)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Vector3 val)
    {
        return Clear().Append(val);
    }

    /// <summary>
    /// Set <see cref="UnityEngine.Vector4"/> in the format (x, y, z, w)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Vector4 val)
    {
        return Clear().Append(val);
    }

    /// <summary>
    /// Set <see cref="Rect2i"/> in the format (x, y, width, height)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Rect2i val)
    {
        return Clear().Append(val);
    }

    /// <summary>
    /// Set <see cref="UnityEngine.Rect"/> in the format (x, y, width, height)
    /// </summary>
    /// <param name="val">Value</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Rect val)
    {
        return Clear().Append(val);
    }

    /// <summary>
    /// Set <see cref="Color32"/>. The format depends on the <paramref name="flags"/> value.
    /// For <see cref="FastString.FORMAT_HEX_SMALL"/> format is in small hex alpha numerics, eg 3a60abff.
    /// For <see cref="FastString.FORMAT_HEX_CAPS"/> format is in small hex alpha numerics, eg 3A60ABFF.
    /// If <paramref name="flags"/> is 0 format is (r, g, b, a)
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="flags">0 or one of <see cref="FastString.FORMAT_HEX_SMALL"/> or <see cref="FastString.FORMAT_HEX_CAPS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Color32 val, uint flags = 0)
    {
        return Clear().Append(val, flags);
    }

    /// <summary>
    /// Set <see cref="Color"/>. The format depends on the <paramref name="flags"/> value.
    /// For <see cref="FastString.FORMAT_HEX_SMALL"/> format is in small hex alpha numerics, eg 3a60abff.
    /// For <see cref="FastString.FORMAT_HEX_CAPS"/> format is in small hex alpha numerics, eg 3A60ABFF.
    /// If <paramref name="flags"/> is 0 format is (r, g, b, a)
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="flags">0 or one of <see cref="FastString.FORMAT_HEX_SMALL"/> or <see cref="FastString.FORMAT_HEX_CAPS"/></param>
    /// <returns>The same instance of FastString</returns>
    public FastString Set(Color val, uint flags = 0)
    {
        return Clear().Append(val, flags);
    }

    /// <summary>
    /// Clear the string
    /// </summary>
    /// <remarks>
    /// Clears the contents of the string. This effectively sets the string <see cref="FastString.Length"/> back to zero.
    ///
    /// The <see cref="FastString.Clear"/> method returns *this*, which allows it to be chained with other methods such as <see cref="FastString.Append"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     fstr.Set("Hello there ").Append(playerName).Append("!");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Append"/>
    /// <seealso cref="FastString.Truncate"/>
    public FastString Clear()
    {
        mLength = 0;

        return this;
    }

    /// <summary>
    /// Truncate the string to the given length
    /// </summary>
    /// <remarks>
    /// Truncates the string to the given length.
    ///
    /// The <see cref="FastString.Truncate"/> method returns *this*, which allows it to be chained with other methods such as <see cref="FastString.Append"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // Prints "RetroBlit"
    ///     fstr.Set("RetroBlit");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // Prints "Retro"
    ///     fstr.Truncate(5);
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    /// }
    /// </code>
    /// <param name="truncatedLength">New truncated length</param>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Clear"/>
    /// <seealso cref="FastString.Append"/>
    public FastString Truncate(int truncatedLength)
    {
        if (truncatedLength <= 0)
        {
            mLength = 0;
        }
        else if (mLength > truncatedLength)
        {
            mLength = truncatedLength;
        }

        return this;
    }

    /// <summary>
    /// Convert to lower case characters.
    /// </summary>
    /// <remarks>
    /// Convert to lower case characters. The results could differ depending on regional settings. For consistent results use <see cref="FastString.ToLowerInvariant"/>.
    ///
    /// The <see cref="FastString.ToLower"/> method returns *this*, which allows it to be chained with other methods such as <see cref="FastString.Append"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // Prints "RetroBlit"
    ///     fstr.Set("RetroBlit");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // Prints "retroblit"
    ///     fstr.ToLower();
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    ///
    ///     // Prints "RETROBLIT"
    ///     fstr.ToUpper();
    ///     RB.Print(new Vector2i(0, 40), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.ToUpper"/>
    /// <seealso cref="FastString.ToLowerInvariant"/>
    /// <seealso cref="FastString.ToUpperInvariant"/>
    public FastString ToLower()
    {
        for (int i = 0; i < mLength; i++)
        {
            mChars[i] = char.ToLower(mChars[i]);
        }

        return this;
    }

    /// <summary>
    /// Convert to upper case characters.
    /// </summary>
    /// <remarks>
    /// Convert to upper case characters. The results could differ depending on regional settings. For consistent results use <see cref="FastString.ToUpperInvariant"/>.
    ///
    /// The <see cref="FastString.ToUpper"/> method returns *this*, which allows it to be chained with other methods such as <see cref="FastString.Append"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // Prints "RetroBlit"
    ///     fstr.Set("RetroBlit");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // Prints "retroblit"
    ///     fstr.ToLower();
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    ///
    ///     // Prints "RETROBLIT"
    ///     fstr.ToUpper();
    ///     RB.Print(new Vector2i(0, 40), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.ToLower"/>
    /// <seealso cref="FastString.ToLowerInvariant"/>
    /// <seealso cref="FastString.ToUpperInvariant"/>
    public FastString ToUpper()
    {
        for (int i = 0; i < mLength; i++)
        {
            mChars[i] = char.ToUpper(mChars[i]);
        }

        return this;
    }

    /// <summary>
    /// Convert to lower case characters. The results will be consistent regardless of regional settings.
    /// </summary>
    /// <remarks>
    /// Convert to lower case characters. The results will be consistent regardless of regional settings.
    ///
    /// The <see cref="FastString.ToLowerInvariant"/> method returns *this*, which allows it to be chained with other methods such as <see cref="FastString.Append"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // Prints "RetroBlit"
    ///     fstr.Set("RetroBlit");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // Prints "retroblit"
    ///     fstr.ToLowerInvariant();
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    ///
    ///     // Prints "RETROBLIT"
    ///     fstr.ToUpperInvariant();
    ///     RB.Print(new Vector2i(0, 40), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.ToLower"/>
    /// <seealso cref="FastString.ToUpper"/>
    /// <seealso cref="FastString.ToUpperInvariant"/>
    public FastString ToLowerInvariant()
    {
        for (int i = 0; i < mLength; i++)
        {
            mChars[i] = char.ToLowerInvariant(mChars[i]);
        }

        return this;
    }

    /// <summary>
    /// Convert to upper case characters. The results will be consistent regardless of regional settings.
    /// </summary>
    /// <remarks>
    /// Convert to upper case characters. The results will be consistent regardless of regional settings.
    ///
    /// The <see cref="FastString.ToUpperInvariant"/> method returns *this*, which allows it to be chained with other methods such as <see cref="FastString.Append"/>.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Render() {
    ///     // Prints "RetroBlit"
    ///     fstr.Set("RetroBlit");
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    ///
    ///     // Prints "retroblit"
    ///     fstr.ToLowerInvariant();
    ///     RB.Print(new Vector2i(0, 20), Color.white, fstr);
    ///
    ///     // Prints "RETROBLIT"
    ///     fstr.ToUpperInvariant();
    ///     RB.Print(new Vector2i(0, 40), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.ToLower"/>
    /// <seealso cref="FastString.ToUpper"/>
    /// <seealso cref="FastString.ToLowerInvariant"/>
    public FastString ToUpperInvariant()
    {
        for (int i = 0; i < mLength; i++)
        {
            mChars[i] = char.ToUpperInvariant(mChars[i]);
        }

        return this;
    }

    /// <summary>
    /// Trim all leading and trailing spaces from the string
    /// </summary>
    /// <remarks>
    /// Trim all leading and trailing spaces from the string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     // Prints "RetroBlit"
    ///     fstr.Set("  RetroBlit  ");
    ///     fstr.Trim();
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.TrimStart"/>
    /// <seealso cref="FastString.TrimEnd"/>
    public FastString Trim()
    {
        // Count left and right spaces
        int leftSpaces = 0;
        for (int i = 0; i < mLength; i++)
        {
            if (mChars[i] == ' ')
            {
                leftSpaces++;
            }
            else
            {
                break;
            }
        }

        int rightSpaces = 0;
        for (int i = mLength - 1; i >= 0; i--)
        {
            if (mChars[i] == ' ')
            {
                rightSpaces++;
            }
            else
            {
                break;
            }
        }

        // Trim left first
        if (leftSpaces > 0)
        {
            Buffer.BlockCopy(mChars, leftSpaces * sizeof(char), mChars, 0, (mLength - leftSpaces) * sizeof(char));
        }

        // Trim right spaces + left spaces that were shifted
        Truncate(mLength - rightSpaces - leftSpaces);

        return this;
    }

    /// <summary>
    /// Trim all leading spaces from the string
    /// </summary>
    /// <remarks>
    /// Trim all leading spaces from the string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     // Prints "RetroBlit  "
    ///     fstr.Set("  RetroBlit  ");
    ///     fstr.TrimStart();
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Trim"/>
    /// <seealso cref="FastString.TrimEnd"/>
    public FastString TrimStart()
    {
        // Count left spaces
        int leftSpaces = 0;
        for (int i = 0; i < mLength; i++)
        {
            if (mChars[i] == ' ')
            {
                leftSpaces++;
            }
            else
            {
                break;
            }
        }

        if (leftSpaces > 0)
        {
            Buffer.BlockCopy(mChars, leftSpaces * sizeof(char), mChars, 0, (mLength - leftSpaces) * sizeof(char));
        }

        // Trim left spaces that were shifted
        Truncate(mLength - leftSpaces);

        return this;
    }

    /// <summary>
    /// Trim all trailing spaces from the string
    /// </summary>
    /// <remarks>
    /// Trim all trailing spaces from the string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     // Prints "  RetroBlit"
    ///     fstr.Set("  RetroBlit  ");
    ///     fstr.TrimEnd();
    ///     RB.Print(new Vector2i(0, 0), Color.white, fstr);
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Trim"/>
    /// <seealso cref="FastString.TrimStart"/>
    public FastString TrimEnd()
    {
        // Count right spaces
        int rightSpaces = 0;
        for (int i = mLength - 1; i >= 0; i--)
        {
            if (mChars[i] == ' ')
            {
                rightSpaces++;
            }
            else
            {
                break;
            }
        }

        // Trim right spaces + left spaces that were shifted
        Truncate(mLength - rightSpaces);

        return this;
    }

    /// <summary>
    /// Returns true if string starts with the given string
    /// </summary>
    /// <param name="str">String to look for</param>
    /// <remarks>
    /// Returns true if string starts with the given string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("RetroBlit");
    ///     Debug.Log(fstr.StartsWith("Retro")); // true
    ///     Debug.Log(fstr.StartsWith("Blit")); // false
    ///
    ///     fstr.Set("  RetroBlit");
    ///     Debug.Log(fstr.StartsWith("Retro")); // false
    /// }
    /// </code>
    /// <returns>True if starts with given string</returns>
    /// <seealso cref="FastString.EndsWith"/>
    public bool StartsWith(string str)
    {
        return StartsWith(new RetroBlitInternal.RBFont.TextWrapString(str));
    }

    /// <summary>
    /// Returns true if string starts with the given string
    /// </summary>
    /// <param name="str">String to look for</param>
    /// <returns>True if starts with given string</returns>
    public bool StartsWith(FastString str)
    {
        return StartsWith(new RetroBlitInternal.RBFont.TextWrapFastString(str));
    }

    /// <summary>
    /// Returns true if string ends with the given string
    /// </summary>
    /// <param name="str">String to look for</param>
    /// <remarks>
    /// Returns true if string ends with the given string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("RetroBlit");
    ///     Debug.Log(fstr.EndsWith("Blit")); // true
    ///     Debug.Log(fstr.EndsWith("Retro")); // false
    ///
    ///     fstr.Set("RetroBlit  ");
    ///     Debug.Log(fstr.EndsWith("Blit")); // false
    /// }
    /// </code>
    /// <returns>True if ends with given string</returns>
    /// <seealso cref="FastString.StartsWith"/>
    public bool EndsWith(string str)
    {
        return EndsWith(new RetroBlitInternal.RBFont.TextWrapString(str));
    }

    /// <summary>
    /// Returns true if string ends with the given string
    /// </summary>
    /// <param name="str">String to look for</param>
    /// <returns>True if ends with given string</returns>
    public bool EndsWith(FastString str)
    {
        return EndsWith(new RetroBlitInternal.RBFont.TextWrapFastString(str));
    }

    /// <summary>
    /// Return first index of given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <param name="startIndex">Index to start searching at</param>
    /// <remarks>
    /// Return first index of given string. <paramref name="startIndex"/> can be specified to start search from the given index.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("retroblit is retro");
    ///     Debug.Log(fstr.IndexOf("retro")); // index 0
    ///     Debug.Log(fstr.IndexOf("retro"), 1); // index 13
    ///     Debug.Log(fstr.IndexOf("missing"), 1); // index -1
    /// }
    /// </code>
    /// <returns>Index of the string, or -1 if not found</returns>
    /// <seealso cref="FastString.LastIndexOf"/>
    public int IndexOf(string str, int startIndex = 0)
    {
        return IndexOf(new RetroBlitInternal.RBFont.TextWrapString(str), startIndex);
    }

    /// <summary>
    /// Return first index of given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <param name="startIndex">Index to start searching at</param>
    /// <returns>Index of the string, or -1 if not found</returns>
    public int IndexOf(FastString str, int startIndex = 0)
    {
        return IndexOf(new RetroBlitInternal.RBFont.TextWrapFastString(str), startIndex);
    }

    /// <summary>
    /// Return first index of given character
    /// </summary>
    /// <param name="c">Character to find</param>
    /// <param name="startIndex">Index to start searching at</param>
    /// <returns>Index of the string, or -1 if not found</returns>
    public int IndexOf(char c, int startIndex = 0)
    {
        if (mLength == 0)
        {
            return -1;
        }

        for (int i = startIndex; i < mLength; i++)
        {
            if (mChars[i] == c)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Return last index of given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <remarks>
    /// Return last index of given string. <paramref name="startIndex"/> can be specified to start search from the given index.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("retroblit is retro");
    ///     Debug.Log(fstr.LastIndexOf("retro")); // index 13
    ///     Debug.Log(fstr.LastIndexOf("retro"), 8); // index 0
    ///     Debug.Log(fstr.LastIndexOf("missing"), 1); // index -1
    /// }
    /// </code>
    /// <returns>Index of the string, or -1 if not found</returns>
    /// <seealso cref="FastString.IndexOf"/>
    public int LastIndexOf(string str)
    {
        return LastIndexOf(new RetroBlitInternal.RBFont.TextWrapString(str), Length - 1);
    }

    /// <summary>
    /// Return last index of given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <returns>Index of the string, or -1 if not found</returns>
    public int LastIndexOf(FastString str)
    {
        return LastIndexOf(new RetroBlitInternal.RBFont.TextWrapFastString(str), Length - 1);
    }

    /// <summary>
    /// Return last index of given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <param name="startIndex">Index to start searching at</param>
    /// <returns>Index of the string, or -1 if not found</returns>
    public int LastIndexOf(string str, int startIndex)
    {
        return LastIndexOf(new RetroBlitInternal.RBFont.TextWrapString(str), startIndex);
    }

    /// <summary>
    /// Return last index of given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <param name="startIndex">Index to start searching at</param>
    /// <returns>Index of the string, or -1 if not found</returns>
    public int LastIndexOf(FastString str, int startIndex)
    {
        return LastIndexOf(new RetroBlitInternal.RBFont.TextWrapFastString(str), startIndex);
    }

    /// <summary>
    /// Return last index of given character
    /// </summary>
    /// <param name="c">Character to find</param>
    /// <returns>Index of the string, or -1 if not found</returns>
    public int LastIndexOf(char c)
    {
        int startIndex = Length - 1;

        if (mLength == 0)
        {
            return -1;
        }

        for (int i = startIndex; i >= 0; i--)
        {
            if (mChars[i] == c)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Return last index of given character
    /// </summary>
    /// <param name="c">Character to find</param>
    /// <param name="startIndex">Index to start searching at</param>
    /// <returns>Index of the string, or -1 if not found</returns>
    public int LastIndexOf(char c, int startIndex)
    {
        if (mLength == 0)
        {
            return -1;
        }

        for (int i = startIndex; i >= 0; i--)
        {
            if (mChars[i] == c)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns true if contains given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <remarks>
    /// Returns true if contains given string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("retroblit is retro");
    ///     Debug.Log(fstr.Contains("retro")); // true
    ///     Debug.Log(fstr.Contains('i')); // true
    ///     Debug.Log(fstr.Contains("missing")); // false
    /// }
    /// </code>
    /// <returns>True if found</returns>
    public bool Contains(string str)
    {
        return Contains(new RetroBlitInternal.RBFont.TextWrapString(str));
    }

    /// <summary>
    /// Returns true if contains given string
    /// </summary>
    /// <param name="str">String to find</param>
    /// <returns>True if found</returns>
    public bool Contains(FastString str)
    {
        return Contains(new RetroBlitInternal.RBFont.TextWrapFastString(str));
    }

    /// <summary>
    /// Returns true if contains given character
    /// </summary>
    /// <param name="c">Character to find</param>
    /// <returns>True if found</returns>
    public bool Contains(char c)
    {
        return IndexOf(c) != -1;
    }

    /// <summary>
    /// Remove a section of the string
    /// </summary>
    /// <param name="startIndex">Index to start removing from</param>
    /// <remarks>
    /// Remove a section of the string. If count is not specified then deletes all characters from startIndex to the end of the string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("You can't delete me!");
    ///     Debug.Log(fstr.Remove(3)); // fstr == "You"
    ///
    ///     fstr.Set("You can't delete me!");
    ///     Debug.Log(fstr.Remove(3, 6)); // fstr == "You delete me!"
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Replace"/>
    public FastString Remove(int startIndex)
    {
        return RemoveInternal(startIndex, int.MaxValue);
    }

    /// <summary>
    /// Remove a section of the string
    /// </summary>
    /// <param name="startIndex">Index to start removing from</param>
    /// <param name="count">Amount of characters to delete</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Remove(int startIndex, int count)
    {
        return RemoveInternal(startIndex, count);
    }

    /// <summary>
    /// Replace all instances of the given string with a new string.
    /// </summary>
    /// <param name="oldStr">Old string</param>
    /// <param name="newStr">New string</param>
    /// <remarks>
    /// Replace all instances of the given string with a new string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("{name} gains experience! {name} levels up!");
    ///     Debug.Log(fstr.Replace("{name}", "Pete")); // fstr == "Pete gains experience! Pete levels up!"
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Remove"/>
    /// <seealso cref="FastString.Substring"/>
    public FastString Replace(string oldStr, string newStr)
    {
        return Replace(new RetroBlitInternal.RBFont.TextWrapString(oldStr), new RetroBlitInternal.RBFont.TextWrapString(newStr));
    }

    /// <summary>
    /// Replace all instances of the given string with a new string.
    /// </summary>
    /// <param name="oldStr">Old string</param>
    /// <param name="newStr">New string</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Replace(FastString oldStr, FastString newStr)
    {
        return Replace(new RetroBlitInternal.RBFont.TextWrapFastString(oldStr), new RetroBlitInternal.RBFont.TextWrapFastString(newStr));
    }

    /// <summary>
    /// Replace all instances of the given string with a new string.
    /// </summary>
    /// <param name="oldStr">Old string</param>
    /// <param name="newStr">New string</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Replace(string oldStr, FastString newStr)
    {
        return Replace(new RetroBlitInternal.RBFont.TextWrapString(oldStr), new RetroBlitInternal.RBFont.TextWrapFastString(newStr));
    }

    /// <summary>
    /// Replace all instances of the given string with a new string.
    /// </summary>
    /// <param name="oldStr">Old string</param>
    /// <param name="newStr">New string</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Replace(FastString oldStr, string newStr)
    {
        return Replace(new RetroBlitInternal.RBFont.TextWrapFastString(oldStr), new RetroBlitInternal.RBFont.TextWrapString(newStr));
    }

    /// <summary>
    /// Replace all instances of the given character with a new character.
    /// </summary>
    /// <param name="oldChar">Old character</param>
    /// <param name="newChar">New character</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Replace(char oldChar, char newChar)
    {
        if (oldChar == newChar)
        {
            return this;
        }

        for (int i = 0; i < mLength; i++)
        {
            if (mChars[i] == oldChar)
            {
                mChars[i] = newChar;
            }
        }

        return this;
    }

    /// <summary>
    /// Set to a substring of the original string
    /// </summary>
    /// <param name="startIndex">Start of the substring</param>
    /// <remarks>
    /// Set to a substring of the original string. If length is not specified then the substring contains all characters from the index
    /// to the end of the original string.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("RetroBlit is pretty neat!");
    ///     Debug.Log(fstr.Substring(13)); // fstr == "pretty neat!"
    ///
    ///     fstr.Set("RetroBlit is pretty neat!");
    ///     Debug.Log(fstr.Substring(13, 6)); // fstr == "pretty"
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    /// <seealso cref="FastString.Remove"/>
    /// <seealso cref="FastString.Replace"/>
    public FastString Substring(int startIndex)
    {
        if (startIndex <= 0)
        {
            return this;
        }

        if (startIndex >= mLength)
        {
            return Clear();
        }

        return ShiftLeft(startIndex, startIndex);
    }

    /// <summary>
    /// Set to a substring of the original string
    /// </summary>
    /// <param name="startIndex">Start of the substring</param>
    /// <param name="length">Length of the substring</param>
    /// <returns>The same instance of FastString</returns>
    public FastString Substring(int startIndex, int length)
    {
        if (startIndex < 0)
        {
            return this;
        }

        if (startIndex == 0)
        {
            return Truncate(length);
        }

        if (startIndex >= mLength)
        {
            return Clear();
        }

        ShiftLeft(startIndex, startIndex);
        return Truncate(length);
    }

    /// <summary>
    /// Pad string with <paramref name="paddingChar"/> on the left side
    /// </summary>
    /// <param name="count">Amount of characters to pad with</param>
    /// <param name="paddingChar">Character to pad with</param>
    /// <remarks>
    /// Pad string with <paramref name="paddingChar"/> on the left side.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("500");
    ///     Debug.Log(fstr.PadLeft(4)); // fstr == "    500"
    ///
    ///     fstr.Set("500");
    ///     Debug.Log(fstr.PadLeft(4, '.')); // fstr == "....500"
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    public FastString PadLeft(int count, char paddingChar = ' ')
    {
        if (count <= 0)
        {
            return this;
        }

        if (mLength == 0)
        {
            mLength = count > (int)mCapacity ? (int)mCapacity : count;
        }
        else
        {
            ShiftRight(count);
        }

        if (count > mLength)
        {
            count = mLength;
        }

        for (int i = 0; i < count; i++)
        {
            mChars[i] = paddingChar;
        }

        return this;
    }

    /// <summary>
    /// Pad string with <paramref name="paddingChar"/> on the right side
    /// </summary>
    /// <param name="count">Amount of characters to pad with</param>
    /// <param name="paddingChar">Character to pad with</param>
    /// <remarks>
    /// Pad string with <paramref name="paddingChar"/> on the right side.
    /// </remarks>
    /// <code>
    /// // Allocate a FastString that can hold up to 256 characters
    /// FastString fstr = new FastString(256);
    ///
    /// void Update() {
    ///     fstr.Set("500");
    ///     Debug.Log(fstr.PadRight(4)); // fstr == "500    "
    ///
    ///     fstr.Set("500");
    ///     Debug.Log(fstr.PadRight(4, '.')); // fstr == "500...."
    /// }
    /// </code>
    /// <returns>The same instance of FastString</returns>
    public FastString PadRight(int count, char paddingChar = ' ')
    {
        if (count <= 0)
        {
            return this;
        }

        int endIndex = mLength + count - 1;

        if (endIndex >= mCapacity)
        {
            endIndex = (int)mCapacity - 1;
        }

        for (int i = mLength; i <= endIndex; i++)
        {
            mChars[i] = paddingChar;
        }

        mLength = endIndex + 1;

        return this;
    }

    /// <summary>
    /// FastString equality
    /// </summary>
    /// <remarks>
    /// FastString equality check.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public bool Equals(FastString other)
    {
        if (other == null)
        {
            return false;
        }

        if (mLength != other.mLength)
        {
            return false;
        }

        for (int i = 0; i < mLength; i++)
        {
            if (mChars[i] != other.mChars[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// String equality
    /// </summary>
    /// <remarks>
    /// String equality check.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }

        if (other is FastString)
        {
#pragma warning disable IDE0020 // Use pattern matching
            FastString str = (FastString)other;
#pragma warning restore IDE0020 // Use pattern matching
            if (mLength != str.mLength)
            {
                return false;
            }

            for (int i = 0; i < mLength; i++)
            {
                if (mChars[i] != str.mChars[i])
                {
                    return false;
                }
            }

            return true;
        }
#pragma warning disable IDE0038 // Use pattern matching
        else if (other is string)
#pragma warning restore IDE0038 // Use pattern matching
        {
            if (mLength != ((string)other).Length)
            {
                return false;
            }

            for (int i = 0; i < mLength; i++)
            {
                if (mChars[i] != ((string)other)[i])
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get string hashcode
    /// </summary>
    /// <remarks>
    /// Gets the string hash code. The hash code is guaranteed to be stable across various platforms, and it is safe
    /// to persist the hash for later comparison if needed.
    ///
    /// The hash code will never be 0, nor -1.
    /// <seedoc>Features:Avoid GC with Fast String</seedoc>
    /// </remarks>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return RetroBlitInternal.RBUtil.StableStringHash(this);
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
        return new string(Buf, 0, Length);
    }

    private bool StartsWith(RetroBlitInternal.RBFont.TextWrap str)
    {
        int len = str.Length;

        if (mLength < len)
        {
            return false;
        }

        for (int i = 0; i < len; i++)
        {
            if (mChars[i] != str[i])
            {
                return false;
            }
        }

        return true;
    }

    private bool EndsWith(RetroBlitInternal.RBFont.TextWrap str)
    {
        int len = str.Length;

        if (Length < len)
        {
            return false;
        }

        int j = len - 1;

        for (int i = mLength - 1; i >= mLength - len; i--)
        {
            if (mChars[i] != str[j])
            {
                return false;
            }

            j--;
        }

        return true;
    }

    private int IndexOf(RetroBlitInternal.RBFont.TextWrap str, int startIndex = 0)
    {
        int len = str.Length;

        if (mLength - startIndex < len)
        {
            return -1;
        }

        if (len == 0)
        {
            return 0;
        }

        int j = 0;
        for (int i = startIndex; i < mLength; i++)
        {
            if (mChars[i] == str[j])
            {
                j++;

                if (j == len)
                {
                    return startIndex;
                }
            }
            else
            {
                j = 0;
                startIndex = i + 1;
            }
        }

        return -1;
    }

    private int LastIndexOf(RetroBlitInternal.RBFont.TextWrap str, int startIndex = int.MaxValue)
    {
        int len = str.Length;
        if (startIndex == int.MaxValue)
        {
            startIndex = mLength - 1;
        }

        if (startIndex + 1 < len)
        {
            return -1;
        }

        if (len == 0)
        {
            if (mLength == 0)
            {
                return 0;
            }

            return mLength - 1;
        }

        int j = len - 1;
        for (int i = startIndex; i >= 0; i--)
        {
            if (mChars[i] == str[j])
            {
                j--;

                if (j == -1)
                {
                    return startIndex - len + 1;
                }
            }
            else
            {
                j = len - 1;
                startIndex = i - 1;
            }
        }

        return -1;
    }

    private bool Contains(RetroBlitInternal.RBFont.TextWrap str)
    {
        if (str.Length == 0)
        {
            return true;
        }

        return IndexOf(str) != -1;
    }

    private FastString RemoveInternal(int startIndex, int count = int.MaxValue)
    {
        if (startIndex < 0 || startIndex >= mLength)
        {
            return this;
        }

        if (count > mLength)
        {
            count = mLength;
        }

        if (startIndex + count >= mLength)
        {
            return Truncate(startIndex);
        }

        ShiftLeft(count, startIndex + count);

        return this;
    }

    private FastString Replace(RetroBlitInternal.RBFont.TextWrap oldStr, RetroBlitInternal.RBFont.TextWrap newStr)
    {
        if (oldStr == newStr)
        {
            return this;
        }

        if (oldStr.Length == 0)
        {
            return this;
        }

        int index = IndexOf(oldStr);
        int newLen = newStr.Length;
        int oldLen = oldStr.Length;
        int sizeDiff = newLen - oldLen;

        // New string is empty so we're just deleting, which is equivalent of Remove() call
        if (newStr.Length == 0)
        {
            while (index != -1)
            {
                Remove(index, oldLen);
                index = IndexOf(oldStr);
            }

            return this;
        }

        if (sizeDiff == 0)
        {
            while (index != -1)
            {
                int j = index;
                for (int i = 0; i < newLen; i++)
                {
                    mChars[j] = newStr[i];
                    j++;
                }

                index = IndexOf(oldStr, index + newLen);
            }
        }
        else if (sizeDiff > 0)
        {
            while (index != -1)
            {
                ShiftRight(sizeDiff, index);

                int j = index;
                for (int i = 0; i < newLen; i++)
                {
                    if (j >= mCapacity)
                    {
                        break;
                    }

                    mChars[j] = newStr[i];
                    j++;
                }

                // If we increased string size by replacing then grow the string size, and break out, there should be no more to replace
                if (j > mLength)
                {
                    mLength = j;
                    break;
                }

                index = IndexOf(oldStr, index + newLen);
            }
        }
        else
        {
            while (index != -1)
            {
                ShiftLeft(-sizeDiff, index + oldLen);

                int j = index;
                for (int i = 0; i < newLen; i++)
                {
                    mChars[j] = newStr[i];
                    j++;
                }

                index = IndexOf(oldStr, index + newLen);
            }
        }

        return this;
    }

    private FastString ShiftLeft(int count, int startIndex = 0)
    {
        // Don't allow to shift left from beyond end of string
        if (startIndex >= mLength)
        {
            return this;
        }

        // TODO: Fall back on ShiftRight if < 0?
        if (count <= 0)
        {
            return this;
        }

        if (count >= mLength)
        {
            return Clear();
        }

        if (startIndex < 0)
        {
            startIndex = 0;
        }

        int shiftStart = startIndex;
        int shiftEnd = startIndex - count;
        int shiftLen = mLength - startIndex;

        if (shiftEnd < 0)
        {
            shiftStart -= shiftEnd;
            shiftEnd = 0;
        }

        if (shiftLen > mLength - shiftStart)
        {
            shiftLen = mLength - shiftStart;
        }

        Buffer.BlockCopy(mChars, shiftStart * sizeof(char), mChars, shiftEnd * sizeof(char), shiftLen * sizeof(char));

        Truncate(mLength - count);

        return this;
    }

    private FastString ShiftRight(int count, int startIndex = 0)
    {
        // Don't allow to shift right from beyond end of string
        if (startIndex >= mLength)
        {
            return this;
        }

        // TODO: Fall back on ShiftLeft if < 0?
        if (count <= 0)
        {
            return this;
        }

        mLength += count;
        if (mLength > mCapacity)
        {
            mLength = (int)mCapacity;
        }

        int shiftStart = startIndex;
        int shiftEnd = shiftStart + count;
        int shiftLen = mLength - count;

        // If the right side is entirely shifted out then just truncate instead
        if (shiftEnd >= mLength)
        {
            for (int i = startIndex; i < mLength; i++)
            {
                mChars[i] = ' ';
            }

            return this;
        }

        if (mLength - shiftEnd < shiftLen)
        {
            shiftLen = mLength - shiftEnd;
        }

        Buffer.BlockCopy(mChars, startIndex * sizeof(char), mChars, shiftEnd * sizeof(char), shiftLen * sizeof(char));
        for (int i = startIndex; i < startIndex + count && i < mLength; i++)
        {
            mChars[i] = ' ';
        }

        return this;
    }

    // Must already hold the mutex!
    private void LongToBuffer(long val, uint flags, out int charsWritten)
    {
        int i = mBuf.Length - 1;

        if ((flags & (FORMAT_HEX_SMALL | FORMAT_HEX_CAPS)) != 0)
        {
            int leadingZeros = 0;

            char[] hexLookup = (flags & FORMAT_HEX_SMALL) != 0 ? mSmallHex : mCapsHex;

            for (int j = 0; j < 64 / 4; j++)
            {
                int digit = (int)((val >> (j * 4)) & 0xF);
                mBuf[i] = hexLookup[digit];
                i--;

                if (digit == 0)
                {
                    leadingZeros++;
                }
                else
                {
                    leadingZeros = 0;
                }
            }

            if (leadingZeros > 0)
            {
                i += leadingZeros;
                if (i > mBuf.Length - 2)
                {
                    i = mBuf.Length - 2;
                }
            }
        }
        else
        {
            bool negative = ((flags & FORCE_NEGATIVE) != 0) || (val < 0);
            val = System.Math.Abs(val);

            while (val != 0 || i == mBuf.Length - 1)
            {
                mBuf[i] = (char)('0' + (val % 10));
                val /= 10;
                i--;
            }

            if (negative)
            {
                mBuf[i] = '-';
                i--;
            }
        }

        charsWritten = (mBuf.Length - 1) - i;
    }

    // Must already hold the mutex!
    private void ULongToBuffer(ulong val, uint flags, out int charsWritten)
    {
        int i = mBuf.Length - 1;

        if ((flags & (FORMAT_HEX_SMALL | FORMAT_HEX_CAPS)) != 0)
        {
            int leadingZeros = 0;

            char[] hexLookup = (flags & FORMAT_HEX_SMALL) != 0 ? mSmallHex : mCapsHex;

            for (int j = 0; j < 64 / 4; j++)
            {
                int digit = (int)((val >> (j * 4)) & 0xF);
                mBuf[i] = hexLookup[digit];
                i--;

                if (digit == 0)
                {
                    leadingZeros++;
                }
                else
                {
                    leadingZeros = 0;
                }
            }

            if (leadingZeros > 0)
            {
                i += leadingZeros;
                if (i > mBuf.Length - 2)
                {
                    i = mBuf.Length - 2;
                }
            }
        }
        else
        {
            while (val != 0 || i == mBuf.Length - 1)
            {
                mBuf[i] = (char)('0' + (val % 10));
                val /= 10;
                i--;
            }
        }

        charsWritten = (mBuf.Length - 1) - i;
    }

    // Must already hold the mutex!
    private void IntToBuffer(int val, uint flags,  out int charsWritten)
    {
        int i = mBuf.Length - 1;

        if ((flags & (FORMAT_HEX_SMALL | FORMAT_HEX_CAPS)) != 0)
        {
            int leadingZeros = 0;

            char[] hexLookup = (flags & FORMAT_HEX_SMALL) != 0 ? mSmallHex : mCapsHex;

            for (int j = 0; j < 32 / 4; j++)
            {
                int digit = (int)((val >> (j * 4)) & 0xF);
                mBuf[i] = hexLookup[digit];
                i--;

                if (digit == 0)
                {
                    leadingZeros++;
                }
                else
                {
                    leadingZeros = 0;
                }
            }

            if (leadingZeros > 0)
            {
                i += leadingZeros;
                if (i > mBuf.Length - 2)
                {
                    i = mBuf.Length - 2;
                }
            }
        }
        else
        {
            bool negative = val < 0;

            val = System.Math.Abs(val);

            while (val != 0 || i == mBuf.Length - 1)
            {
                mBuf[i] = (char)('0' + (val % 10));
                val /= 10;
                i--;
            }

            if (negative)
            {
                mBuf[i] = '-';
                i--;
            }
        }

        charsWritten = (mBuf.Length - 1) - i;
    }

    // Must already hold the mutex!
    private void UIntToBuffer(uint val, uint flags, out int charsWritten)
    {
        int i = mBuf.Length - 1;

        if ((flags & (FORMAT_HEX_SMALL | FORMAT_HEX_CAPS)) != 0)
        {
            int leadingZeros = 0;

            char[] hexLookup = (flags & FORMAT_HEX_SMALL) != 0 ? mSmallHex : mCapsHex;

            for (int j = 0; j < 32 / 4; j++)
            {
                int digit = (int)((val >> (j * 4)) & 0xF);
                mBuf[i] = hexLookup[digit];
                i--;

                if (digit == 0)
                {
                    leadingZeros++;
                }
                else
                {
                    leadingZeros = 0;
                }
            }

            if (leadingZeros > 0)
            {
                i += leadingZeros;
                if (i > mBuf.Length - 2)
                {
                    i = mBuf.Length - 2;
                }
            }
        }
        else
        {
            while (val != 0 || i == mBuf.Length - 1)
            {
                mBuf[i] = (char)('0' + (val % 10));
                val /= 10;
                i--;
            }
        }

        charsWritten = (mBuf.Length - 1) - i;
    }

    // Adapted and ported from https://github.com/mpaland/printf
    private void HandleDoubleExp(double value, uint precision)
    {
        // Remember that precision was undefined, and set precision to 9, leading 0s will be chopped off
        uint orginalPrecision = precision;
        if (orginalPrecision == PRECISION_UNDEFINED)
        {
            precision = DEFAULT_PRECISION;
        }

        // determine the sign
        bool negative = value < 0;
        if (negative)
        {
            value = -value;
        }

        // Determine the decimal exponent
        // Based on the algorithm by David Gay (https://www.ampl.com/netlib/fp/dtoa.c)
        DoubleBreakdown conv = new DoubleBreakdown();

        conv.F = value;
        int exp2 = (int)((conv.U >> 52) & 0x07FFU) - 1023;     // Effectively log2
        conv.U = (conv.U & ((1UL << 52) -1U)) | (1023UL << 52);  // Drop the exponent so conv.F is now in [1,2)
                                                                 // Now approximate log10 from the log2 integer part and an expansion of ln around 1.5

        int expval = (int)(0.1760912590558 + exp2 * 0.301029995663981 + (conv.F - 1.5) * 0.289529654602168);

        // Now we want to compute 10^expval but we want to be sure it won't overflow
        exp2 = (int)(expval * 3.321928094887362 + 0.5);
        double z = expval * 2.302585092994046 - exp2 * 0.6931471805599453;
        double z2 = z * z;
        conv.U = (ulong)(exp2 + 1023) << 52;

        // Compute exp(z) using continued fractions, see https://en.wikipedia.org/wiki/Exponential_function#Continued_fractions_for_ex
        conv.F *= 1 + 2 * z / (2 - z + (z2 / (6 + (z2 / (10 + z2 / 14)))));

        // Correct for rounding errors
        if (value < conv.F)
        {
            expval--;
            conv.F /= 10;
        }

        // Rescale the float value
        if (expval != 0)
        {
            value /= conv.F;
        }

        // Output the floating part
        HandleDouble(negative ? -value : value, orginalPrecision);

        // Output the exponent part
        Append("e");
        Append((expval < 0) ? -expval : expval);

        return;
    }

    // Adapted and ported from https://github.com/mpaland/printf
    private void HandleDouble(double value, uint precision)
    {
        // Remember that precision was undefined, and set precision to 9, leading 0s will be chopped off
        uint orginalPrecision = precision;
        if (orginalPrecision == PRECISION_UNDEFINED)
        {
            precision = DEFAULT_PRECISION;
        }

        uint len = 0;
        double diff = 0.0;

        // Test for special values
        if (double.IsNaN(value)) {
            Append("nan");
            return;
        }

        if (value < double.MinValue) {
            Append("-inf");
            return;
        }

        if (value > double.MaxValue) {
            Append("inf");
            return;
        }

        // Test for very large values
        // Standard printf behavior is to print EVERY whole number digit -- which could be 100s of characters overflowing your buffers == bad
        if ((value > PRINTF_MAX_FLOAT) || (value < -PRINTF_MAX_FLOAT))
        {
            HandleDoubleExp(value, orginalPrecision);
            return;
        }

        // Test for negative
        bool negative = false;
        if (value < 0)
        {
            negative = true;
            value = 0 - value;
        }

        // Limit precision to MAX_FLOAT_PRECISION, cause a precision > MAX_FLOAT_PRECISION can lead to overflow errors
        while ((len < mFloatBuf.Length) && (precision > MAX_FLOAT_PRECISION))
        {
            mFloatBuf[len++] = '0';
            precision--;
        }

        long whole = (long)value;
        double tmp = (value - whole) * mPowers10[precision];
        ulong frac = (ulong)tmp;
        diff = tmp - frac;

        if (diff > 0.5)
        {
            ++frac;
            // Handle rollover, e.g. case 0.99 with prec 1 is 1.0
            if (frac >= mPowers10[precision])
            {
                frac = 0;
                ++whole;
            }
        }
        else if (diff < 0.5)
        {
            // Do nothing
        }
        else if ((frac == 0U) || (frac & 1U) != 0)
        {
            // If halfway, round up if odd OR if last digit is 0
            ++frac;
        }

        if (precision == 0U)
        {
            diff = value - (double)whole;
            if ((!(diff < 0.5) || (diff > 0.5)) && (whole & 1) != 0)
            {
                // Exactly 0.5 and ODD, then round up
                // 1.5 -> 2, but 2.5 -> 2
                ++whole;
            }
        }
        else
        {
            uint count = (uint)precision;
            // Now do fractional part, as an unsigned number
            while (len < mFloatBuf.Length)
            {
                --count;
                mFloatBuf[len++] = (char)(48U + (frac % 10U));
                if ((frac /= 10U) == 0)
                {
                    break;
                }
            }
            // Add extra 0s
            while ((len < mFloatBuf.Length) && (count-- > 0U))
            {
                mFloatBuf[len++] = '0';
            }
            if (len < mFloatBuf.Length)
            {
                // Add decimal
                mFloatBuf[len++] = '.';
            }
        }

        // Do whole part, number is reversed
        while (len < mFloatBuf.Length)
        {
            mFloatBuf[len++] = (char)(48 + (whole % 10));
            if ((whole /= 10) == 0)
            {
                break;
            }
        }

        if (len < mFloatBuf.Length)
        {
            if (negative)
            {
                mFloatBuf[len++] = '-';
            }
        }

        int trunc = 0;

        // If precision was undefined then chop off leading 0s in decimal places.
        if (orginalPrecision == PRECISION_UNDEFINED)
        {
            for (int i = 0; i < len; i++)
            {
                if (mFloatBuf[i] != '0')
                {
                    break;
                }

                trunc++;
            }

            if (mFloatBuf[trunc] == '.')
            {
                trunc++;
            }
        }

        for (int i = (int)(len - 1); i >= trunc; i--)
        {
            Append(mFloatBuf[i]);
        }
    }
}
