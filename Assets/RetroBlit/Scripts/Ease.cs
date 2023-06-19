using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// A set of tweening functions
/// </summary>
/// <remarks>
/// A set of tweening functions that can help make fluid animations and transitions. RetroBlit implements many
/// tweening functions listed in <mref refid="Ease.Func">Ease.Func</mref>. Each of these functions will interpolate over a time period
/// from 0 to 1. The <mref refid="Ease.Value">Ease.Value</mref> method can be used to get a single float value for the given time *t*,
/// and the <mref refid="Ease.Interpolate">Ease.Interpolate</mref> method can be used to interpolate other value types such as vectors, or colors.
/// <image src="interpolate.gif">Visualization of all the different interpolation functions.</image>
/// <seedoc>Features:Interpolation</seedoc>
/// </remarks>
public static class Ease
{
    private const float PI = 3.141592653589793f;
    private const float HALF_PI = PI / 2;

    /// <summary>
    /// Easing functions
    /// </summary>
    /// <remarks>
    /// These are all the easing functions supported by RetroBlit. Each of them will interpolate a value from t=0.0 to t=1.0, with a different
    /// value curve.
    /// <image src="interpolate.gif">Visualization of all the different interpolation functions.</image>
    /// <seedoc>Features:Interpolation</seedoc>
    /// </remarks>
    public enum Func
    {
        /// <summary>
        /// Linear function
        /// </summary>
        /// <remarks>
        /// Linear interpolation function.
        /// <image src="tween_linear.png"></image>
        /// </remarks>
        Linear = 0,

        /// <summary>
        /// Sine-in function
        /// </summary>
        /// <remarks>
        /// Sine-in interpolation function.
        /// <image src="tween_sinein.png"></image>
        /// </remarks>
        SineIn,

        /// <summary>
        /// Sine-out function
        /// </summary>
        /// <remarks>
        /// Sine-out interpolation function.
        /// <image src="tween_sineout.png"></image>
        /// </remarks>
        SineOut,

        /// <summary>
        /// Sine-in/out function
        /// </summary>
        /// <remarks>
        /// Sine-in/out interpolation function.
        /// <image src="tween_sineinout.png"></image>
        /// </remarks>
        SineInOut,

        /// <summary>
        /// Quadratic-in function
        /// </summary>
        /// <remarks>
        /// Quadratic-in interpolation function.
        /// <image src="tween_quadin.png"></image>
        /// </remarks>
        QuadIn,

        /// <summary>
        /// Quadratic-out function
        /// </summary>
        /// <remarks>
        /// Quadratic-out interpolation function.
        /// <image src="tween_quadout.png"></image>
        /// </remarks>
        QuadOut,

        /// <summary>
        /// Quadratic in/out function
        /// </summary>
        /// <remarks>
        /// Quadratic-in/out interpolation function.
        /// <image src="tween_quadinout.png"></image>
        /// </remarks>
        QuadInOut,

        /// <summary>
        /// Cubic-in function
        /// </summary>
        /// <remarks>
        /// Cubic-in interpolation function.
        /// <image src="tween_cubicin.png"></image>
        /// </remarks>
        CubicIn,

        /// <summary>
        /// Cubic-out function
        /// </summary>
        /// <remarks>
        /// Cubic-out interpolation function.
        /// <image src="tween_cubicout.png"></image>
        /// </remarks>
        CubicOut,

        /// <summary>
        /// Cubic-in/out function
        /// </summary>
        /// <remarks>
        /// Cubic-in/out interpolation function.
        /// <image src="tween_cubicinout.png"></image>
        /// </remarks>
        CubicInOut,

        /// <summary>
        /// Quartic-in function
        /// </summary>
        /// <remarks>
        /// Quartic-in interpolation function.
        /// <image src="tween_quarticin.png"></image>
        /// </remarks>
        QuarticIn,

        /// <summary>
        /// Quartic-out function
        /// </summary>
        /// <remarks>
        /// Quartic-out interpolation function.
        /// <image src="tween_quarticout.png"></image>
        /// </remarks>
        QuarticOut,

        /// <summary>
        /// Quartic-in/out function
        /// </summary>
        /// <remarks>
        /// Quartic-in/out interpolation function.
        /// <image src="tween_quarticinout.png"></image>
        /// </remarks>
        QuarticInOut,

        /// <summary>
        /// Quantic-in function
        /// </summary>
        /// <remarks>
        /// Quantic-in interpolation function.
        /// <image src="tween_quanticin.png"></image>
        /// </remarks>
        QuanticIn,

        /// <summary>
        /// Quantic-out function
        /// </summary>
        /// <remarks>
        /// Quantic-out interpolation function.
        /// <image src="tween_quanticout.png"></image>
        /// </remarks>
        QuanticOut,

        /// <summary>
        /// Quantic-in/out function
        /// </summary>
        /// <remarks>
        /// Quantic-in/out interpolation function.
        /// <image src="tween_quanticinout.png"></image>
        /// </remarks>
        QuanticInOut,

        /// <summary>
        /// Exponential-in function
        /// </summary>
        /// <remarks>
        /// Exponential-in interpolation function.
        /// <image src="tween_expoin.png"></image>
        /// </remarks>
        ExpoIn,

        /// <summary>
        /// Exponential-out function
        /// </summary>
        /// <remarks>
        /// Exponential-out interpolation function.
        /// <image src="tween_expoout.png"></image>
        /// </remarks>
        ExpoOut,

        /// <summary>
        /// Exponential-in/out function
        /// </summary>
        /// <remarks>
        /// Exponential-in/out interpolation function.
        /// <image src="tween_expoinout.png"></image>
        /// </remarks>
        ExpoInOut,

        /// <summary>
        /// Circular-in function
        /// </summary>
        /// <remarks>
        /// Circular-in interpolation function.
        /// <image src="tween_circin.png"></image>
        /// </remarks>
        CircIn,

        /// <summary>
        /// Circular-out function
        /// </summary>
        /// <remarks>
        /// Circular-out interpolation function.
        /// <image src="tween_circout.png"></image>
        /// </remarks>
        CircOut,

        /// <summary>
        /// Circular-in/out function
        /// </summary>
        /// <remarks>
        /// Circular-in/out interpolation function.
        /// <image src="tween_circinout.png"></image>
        /// </remarks>
        CircInOut,

        /// <summary>
        /// Back-in function
        /// </summary>
        /// <remarks>
        /// Back-in interpolation function.
        /// <image src="tween_backin.png"></image>
        /// </remarks>
        BackIn,

        /// <summary>
        /// Back-out function
        /// </summary>
        /// <remarks>
        /// Back-out interpolation function.
        /// <image src="tween_backout.png"></image>
        /// </remarks>
        BackOut,

        /// <summary>
        /// Back-in/out function
        /// </summary>
        /// <remarks>
        /// Back-in/out interpolation function.
        /// <image src="tween_backinout.png"></image>
        /// </remarks>
        BackInOut,

        /// <summary>
        /// Elastic-in function
        /// </summary>
        /// <remarks>
        /// Elastic-in interpolation function.
        /// <image src="tween_elasticin.png"></image>
        /// </remarks>
        ElasticIn,

        /// <summary>
        /// Elastic-out function
        /// </summary>
        /// <remarks>
        /// Elastic-out interpolation function.
        /// <image src="tween_elasticout.png"></image>
        /// </remarks>
        ElasticOut,

        /// <summary>
        /// Elastic-in/out function
        /// </summary>
        /// <remarks>
        /// Elastic-in/out interpolation function.
        /// <image src="tween_elasticinout.png"></image>
        /// </remarks>
        ElasticInOut,

        /// <summary>
        /// Bounce-in function
        /// </summary>
        /// <remarks>
        /// Bounce-in interpolation function.
        /// <image src="tween_bouncein.png"></image>
        /// </remarks>
        BounceIn,

        /// <summary>
        /// Bounce-out function
        /// </summary>
        /// <remarks>
        /// Bounce-out interpolation function.
        /// <image src="tween_bounceout.png"></image>
        /// </remarks>
        BounceOut,

        /// <summary>
        /// Bounce-in/out function
        /// </summary>
        /// <remarks>
        /// Bounce-in/out interpolation function.
        /// <image src="tween_bounceinout.png"></image>
        /// </remarks>
        BounceInOut
    }

    /// <summary>
    /// Get a float value transformed by an easing function specified by <paramref name="func"/>
    /// </summary>
    /// <remarks>
    /// Get a float value transformed by an easing function specified by <paramref name="func"/>.
    /// <seedoc>Features:Interpolation</seedoc>
    /// </remarks>
    /// <code>
    /// float t = 0;
    ///
    /// public void Update()
    /// {
    ///     t += 0.01f;
    ///     if (t > 1.0f) {
    ///         t = 1.0f;
    ///     }
    /// }
    ///
    /// public void Render()
    /// {
    ///     // Draw game title at a position based on BounceOut tween, this will make it bounce into view.
    ///     float yOffset = -50 + (50 * Ease.Value(Ease.Func.BounceOut, t));
    ///     RB.DrawSprite("title", new Vector2i(20, yOffset));
    /// }
    /// </code>
    /// <param name="func">Easing function to use</param>
    /// <param name="t">Input value, functions are defined for values 0.0 to 1.0</param>
    /// <returns>Transformed value</returns>
    /// <seealso cref="Ease.Interpolate"/>
    public static float Value(Func func, float t)
    {
        switch (func)
        {
            case Func.Linear: return Linear(t);
            case Func.SineIn: return SineIn(t);
            case Func.SineOut: return SineOut(t);
            case Func.SineInOut: return SineInOut(t);
            case Func.QuadIn: return QuadIn(t);
            case Func.QuadOut: return QuadOut(t);
            case Func.QuadInOut: return QuadInOut(t);
            case Func.CubicIn: return CubicIn(t);
            case Func.CubicOut: return CubicOut(t);
            case Func.CubicInOut: return CubicInOut(t);
            case Func.QuarticIn: return QuarticIn(t);
            case Func.QuarticOut: return QuarticOut(t);
            case Func.QuarticInOut: return QuarticInOut(t);
            case Func.QuanticIn: return QuanticIn(t);
            case Func.QuanticOut: return QuanticOut(t);
            case Func.QuanticInOut: return QuanticInOut(t);
            case Func.ExpoIn: return ExpoIn(t);
            case Func.ExpoOut: return ExpoOut(t);
            case Func.ExpoInOut: return ExpoInOut(t);
            case Func.CircIn: return CircIn(t);
            case Func.CircOut: return CircOut(t);
            case Func.CircInOut: return CircInOut(t);
            case Func.BackIn: return BackIn(t);
            case Func.BackOut: return BackOut(t);
            case Func.BackInOut: return BackInOut(t);
            case Func.ElasticIn: return ElasticIn(t);
            case Func.ElasticOut: return ElasticOut(t);
            case Func.ElasticInOut: return ElasticInOut(t);
            case Func.BounceIn: return BounceIn(t);
            case Func.BounceOut: return BounceOut(t);
            case Func.BounceInOut: return BounceInOut(t);
        }

        return t;
    }

    /// <summary>
    /// Interpolate a value from the <paramref name="start"/> to <paramref name="end"/> by time <paramref name="t"/>
    /// </summary>
    /// <remarks>
    /// Interpolate a value from the <paramref name="start"/> to <paramref name="end"/> by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// <seedoc>Features:Interpolation</seedoc>
    /// </remarks>
    /// <code>
    /// float t = 0;
    ///
    /// public void Update()
    /// {
    ///     t += 0.01f;
    ///     if (t > 1.0f) {
    ///         t = 1.0f;
    ///     }
    /// }
    ///
    /// public void Render()
    /// {
    ///     // Use CubicOut interpolation to animate a "poisoned" tint color on a character
    ///     var color = Ease.Interpolate(Ease.Func.CubicOut, Color.white, Color.green, t);
    ///     RB.TintColorSet(color);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    /// <seealso cref="Ease.Value"/>
    public static byte Interpolate(Func func, byte start, byte end, float t)
    {
        t = Value(func, t);
        return (byte)Clamp(Mathf.RoundToInt(start + ((end - start) * t)), byte.MinValue, byte.MaxValue);
    }

    /// <summary>
    /// Interpolate a signed byte value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static sbyte Interpolate(Func func, sbyte start, sbyte end, float t)
    {
        t = Value(func, t);
        return (sbyte)Clamp(Mathf.RoundToInt(start + ((end - start) * t)), sbyte.MinValue, sbyte.MaxValue);
    }

    /// <summary>
    /// Interpolate a short value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static short Interpolate(Func func, short start, short end, float t)
    {
        t = Value(func, t);
        return (short)Clamp(Mathf.RoundToInt(start + ((end - start) * t)), short.MinValue, short.MaxValue);
    }

    /// <summary>
    /// Interpolate an unsigned short value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static ushort Interpolate(Func func, ushort start, ushort end, float t)
    {
        t = Value(func, t);
        return (ushort)Clamp(Mathf.RoundToInt(start + ((end - start) * t)), ushort.MinValue, ushort.MaxValue);
    }

    /// <summary>
    /// Interpolate a char value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static char Interpolate(Func func, char start, char end, float t)
    {
        t = Value(func, t);
        return (char)Clamp(Mathf.RoundToInt(start + ((end - start) * t)), char.MinValue, char.MaxValue);
    }

    /// <summary>
    /// Interpolate an int value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static int Interpolate(Func func, int start, int end, float t)
    {
        t = Value(func, t);
        return Clamp(Mathf.RoundToInt(start + ((end - start) * t)), int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Interpolate an unsigned int value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static uint Interpolate(Func func, uint start, uint end, float t)
    {
        t = Value(func, t);
        return Clamp((uint)Mathf.Round(start + ((end - start) * t)), uint.MinValue, uint.MaxValue);
    }

    /// <summary>
    /// Interpolate a long value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static long Interpolate(Func func, long start, long end, float t)
    {
        t = Value(func, t);
        return Clamp((long)Mathf.Round(start + ((end - start) * t)), long.MinValue, long.MaxValue);
    }

    /// <summary>
    /// Interpolate an unsigned long value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static ulong Interpolate(Func func, ulong start, ulong end, float t)
    {
        t = Value(func, t);
        return Clamp((ulong)Mathf.Round(start + ((end - start) * t)), ulong.MinValue, ulong.MaxValue);
    }

    /// <summary>
    /// Interpolate a float value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static float Interpolate(Func func, float start, float end, float t)
    {
        t = Value(func, t);
        return start + ((end - start) * t);
    }

    /// <summary>
    /// Interpolate a double value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static double Interpolate(Func func, double start, double end, float t)
    {
        t = Value(func, t);
        return start + ((end - start) * t);
    }

    /// <summary>
    /// Interpolate a <see cref="Vector2i"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Vector2i Interpolate(Func func, Vector2i start, Vector2i end, float t)
    {
        t = Value(func, t);
        return new Vector2i(
            Clamp(Mathf.RoundToInt(start.x + ((end.x - start.x) * t)), int.MinValue, int.MaxValue),
            Clamp(Mathf.RoundToInt(start.y + ((end.y - start.y) * t)), int.MinValue, int.MaxValue));
    }

    /// <summary>
    /// Interpolate a <see cref="Vector2"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Vector2 Interpolate(Func func, Vector2 start, Vector2 end, float t)
    {
        t = Value(func, t);
        return new Vector2(start.x + ((end.x - start.x) * t), start.y + ((end.y - start.y) * t));
    }

    /// <summary>
    /// Interpolate a <see cref="Vector3"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Vector3 Interpolate(Func func, Vector3 start, Vector3 end, float t)
    {
        t = Value(func, t);
        return new Vector3(start.x + ((end.x - start.x) * t), start.y + ((end.y - start.y) * t), start.z + ((end.z - start.z) * t));
    }

    /// <summary>
    /// Interpolate a <see cref="Vector4"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Vector4 Interpolate(Func func, Vector4 start, Vector4 end, float t)
    {
        t = Value(func, t);
        return new Vector4(start.x + ((end.x - start.x) * t), start.y + ((end.y - start.y) * t), start.z + ((end.z - start.z) * t), start.w + ((end.w - start.w) * t));
    }

    /// <summary>
    /// Interpolate a <see cref="Rect2i"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Rect2i Interpolate(Func func, Rect2i start, Rect2i end, float t)
    {
        t = Value(func, t);
        return new Rect2i(
            Clamp(Mathf.RoundToInt(start.x + ((end.x - start.x) * t)), int.MinValue, int.MaxValue),
            Clamp(Mathf.RoundToInt(start.y + ((end.y - start.y) * t)), int.MinValue, int.MaxValue),
            Clamp(Mathf.RoundToInt(start.width + ((end.width - start.width) * t)), int.MinValue, int.MaxValue),
            Clamp(Mathf.RoundToInt(start.height + ((end.height - start.height) * t)), int.MinValue, int.MaxValue));
    }

    /// <summary>
    /// Interpolate a <see cref="Rect"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Rect Interpolate(Func func, Rect start, Rect end, float t)
    {
        t = Value(func, t);
        return new Rect(start.x + ((end.x - start.x) * t), start.y + ((end.y - start.y) * t), start.width + ((end.width - start.width) * t), start.height + ((end.height - start.height) * t));
    }

    /// <summary>
    /// Interpolate a <see cref="Color"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Color Interpolate(Func func, Color start, Color end, float t)
    {
        t = Value(func, t);
        return new Color(start.r + ((end.r - start.r) * t), start.g + ((end.g - start.g) * t), start.b + ((end.b - start.b) * t), start.a + ((end.a - start.a) * t));
    }

    /// <summary>
    /// Interpolate a <see cref="Color32"/> value from the <paramref name="start"/> value to the <paramref name="end"/> value by time <paramref name="t"/>
    /// where at t = 0.0 the value is <paramref name="start"/> and at t = 1.0 the value is <paramref name="end"/>
    /// </summary>
    /// <param name="func">Easing function</param>
    /// <param name="start">Starting value</param>
    /// <param name="end">End value</param>
    /// <param name="t">Time from 0.0 to 1.0</param>
    /// <returns>Interpolated value</returns>
    public static Color32 Interpolate(Func func, Color32 start, Color32 end, float t)
    {
        t = Value(func, t);
        return new Color32(
            (byte)Clamp(Mathf.RoundToInt(start.r + ((end.r - start.r) * t)), byte.MinValue, byte.MaxValue),
            (byte)Clamp(Mathf.RoundToInt(start.g + ((end.g - start.g) * t)), byte.MinValue, byte.MaxValue),
            (byte)Clamp(Mathf.RoundToInt(start.b + ((end.b - start.b) * t)), byte.MinValue, byte.MaxValue),
            (byte)Clamp(Mathf.RoundToInt(start.a + ((end.a - start.a) * t)), byte.MinValue, byte.MaxValue));
    }

    private static int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private static uint Clamp(uint value, uint min, uint max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private static long Clamp(long value, long min, long max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private static ulong Clamp(ulong value, ulong min, ulong max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private static float Linear(float t)
    {
        return t;
    }

    private static float SineIn(float t)
    {
        return Mathf.Sin((t - 1) * HALF_PI) + 1;
    }

    private static float SineOut(float t)
    {
        return Mathf.Sin(t * HALF_PI);
    }

    private static float SineInOut(float t)
    {
        return 0.5f * (1 - Mathf.Cos(t * PI));
    }

    private static float QuadIn(float t)
    {
        return t * t;
    }

    private static float QuadOut(float t)
    {
        return -(t * (t - 2));
    }

    private static float QuadInOut(float t)
    {
        if (t < 0.5f)
        {
            return 2 * t * t;
        }
        else
        {
            return (-2 * t * t) + (4 * t) - 1;
        }
    }

    private static float CubicIn(float t)
    {
        return t * t * t;
    }

    private static float CubicOut(float t)
    {
        float f = t - 1;
        return (f * f * f) + 1;
    }

    private static float CubicInOut(float t)
    {
        if (t < 0.5f)
        {
            return 4 * t * t * t;
        }
        else
        {
            float f = (2 * t) - 2;
            return (0.5f * f * f * f) + 1;
        }
    }

    private static float QuarticIn(float t)
    {
        return t * t * t * t;
    }

    private static float QuarticOut(float t)
    {
        float f = t - 1;
        return (f * f * f * (1 - t)) + 1;
    }

    private static float QuarticInOut(float t)
    {
        if (t < 0.5f)
        {
            return 8 * t * t * t * t;
        }
        else
        {
            float f = t - 1;
            return (-8 * f * f * f * f) + 1;
        }
    }

    private static float QuanticIn(float t)
    {
        return t * t * t * t * t;
    }

    private static float QuanticOut(float t)
    {
        float f = t - 1;
        return (f * f * f * f * f) + 1;
    }

    private static float QuanticInOut(float t)
    {
        if (t < 0.5f)
        {
            return 16 * t * t * t * t * t;
        }
        else
        {
            float f = (2 * t) - 2;
            return (0.5f * f * f * f * f * f) + 1;
        }
    }

    private static float ExpoIn(float t)
    {
        return (t == 0.0f) ? t : Mathf.Pow(2, 10 * (t - 1));
    }

    private static float ExpoOut(float t)
    {
        return (t == 1.0f) ? t : 1 - Mathf.Pow(2, -10 * t);
    }

    private static float ExpoInOut(float t)
    {
        if (t == 0.0f || t == 1.0f)
        {
            return t;
        }

        if (t < 0.5f)
        {
            return 0.5f * Mathf.Pow(2, (20 * t) - 10);
        }
        else
        {
            return (-0.5f * Mathf.Pow(2, (-20 * t) + 10)) + 1;
        }
    }

    private static float CircIn(float t)
    {
        return 1 - Mathf.Sqrt(1 - (t * t));
    }

    private static float CircOut(float t)
    {
        return Mathf.Sqrt((2 - t) * t);
    }

    private static float CircInOut(float t)
    {
        if (t < 0.5f)
        {
            return 0.5f * (1 - Mathf.Sqrt(1 - (4 * (t * t))));
        }
        else
        {
            return 0.5f * (Mathf.Sqrt(-((2 * t) - 3) * ((2 * t) - 1)) + 1);
        }
    }

    private static float BackIn(float t)
    {
        return (t * t * t) - (t * Mathf.Sin(t * PI));
    }

    private static float BackOut(float t)
    {
        float f = 1 - t;
        return 1 - ((f * f * f) - (f * Mathf.Sin(f * PI)));
    }

    private static float BackInOut(float t)
    {
        if (t < 0.5f)
        {
            float f = 2 * t;
            return 0.5f * ((f * f * f) - (f * Mathf.Sin(f * PI)));
        }
        else
        {
            float f = 1 - ((2 * t) - 1);
            return (0.5f * (1 - ((f * f * f) - (f * Mathf.Sin(f * PI))))) + 0.5f;
        }
    }

    private static float ElasticIn(float t)
    {
        return Mathf.Sin(13 * HALF_PI * t) * Mathf.Pow(2, 10 * (t - 1));
    }

    private static float ElasticOut(float t)
    {
        return (Mathf.Sin(-13 * HALF_PI * (t + 1)) * Mathf.Pow(2, -10 * t)) + 1;
    }

    private static float ElasticInOut(float t)
    {
        if (t < 0.5f)
        {
            return 0.5f * Mathf.Sin(13 * HALF_PI * (2 * t)) * Mathf.Pow(2, 10 * ((2 * t) - 1));
        }
        else
        {
            return 0.5f * ((Mathf.Sin(-13 * HALF_PI * (((2 * t) - 1) + 1)) * Mathf.Pow(2, -10 * ((2 * t) - 1))) + 2);
        }
    }

    private static float BounceIn(float t)
    {
        return 1 - BounceOut(1 - t);
    }

    private static float BounceOut(float t)
    {
        if (t < 4 / 11.0f)
        {
            return (121 * t * t) / 16.0f;
        }
        else if (t < 8 / 11.0f)
        {
            return (363 / 40.0f * t * t) - (99 / 10.0f * t) + (17 / 5.0f);
        }
        else if (t < 9 / 10.0f)
        {
            return (4356 / 361.0f * t * t) - (35442 / 1805.0f * t) + (16061 / 1805.0f);
        }
        else
        {
            return (54 / 5.0f * t * t) - (513 / 25.0f * t) + (268 / 25.0f);
        }
    }

    private static float BounceInOut(float t)
    {
        if (t < 0.5f)
        {
            return 0.5f * BounceIn(t * 2);
        }
        else
        {
            return (0.5f * BounceOut((t * 2) - 1)) + 0.5f;
        }
    }
}
