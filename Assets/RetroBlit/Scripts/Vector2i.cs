using System;
using System.Runtime.InteropServices;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// Defines a 2D vector with integers
/// </summary>
/// <remarks>
/// Defines a 2D vector with integers. This structure is similar to *Unity.Vector2* but it uses integers rather than floats for its coordinates.
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
public struct Vector2i : IEquatable<Vector2i>
{
    /// <summary>
    /// Zero size
    /// </summary>
    /// <remarks>
    /// Zero size vector.
    /// </remarks>
    public static readonly Vector2i zero = new Vector2i(0, 0);

    /// <summary>
    /// X coordinate, same as <see cref="Vector2i.width"/>
    /// </summary>
    /// <remarks>
    /// X coordinate, same as <see cref="Vector2i.width"/>.
    /// </remarks>
    [FieldOffset(0)]
    public int x;

    /// <summary>
    /// Y coordinate, same as <see cref="Vector2i.height"/>
    /// </summary>
    /// <remarks>
    /// Y coordinate, same as <see cref="Vector2i.height"/>.
    /// </remarks>
    [FieldOffset(4)]
    public int y;

    /// <summary>
    /// Width, same as <see cref="Vector2i.x"/> coordinate
    /// </summary>
    /// <remarks>
    /// Width, same as <see cref="Vector2i.x"/> coordinate.
    /// </remarks>
    [FieldOffset(0)]
    public int width;

    /// <summary>
    /// Height, same as <see cref="Vector2i.y"/> coordinate
    /// </summary>
    /// <remarks>
    /// Height, same as <see cref="Vector2i.y"/> coordinate.
    /// </remarks>
    [FieldOffset(4)]
    public int height;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public Vector2i(int x, int y) : this()
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public Vector2i(float x, float y) : this()
    {
        this.x = (int)x;
        this.y = (int)y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v">Size as <see cref="Vector2i"/></param>
    public Vector2i(Vector2i v) : this()
    {
        this.x = v.x;
        this.y = v.y;
    }

    /// <summary>
    /// Constructor using Rect2i x and y coordinates
    /// </summary>
    /// <param name="rect">Rect</param>
    public Vector2i(Rect2i rect) : this()
    {
        this.x = rect.x;
        this.y = rect.y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v2">Vector</param>
    public Vector2i(Vector2 v2) : this()
    {
        this.x = (int)v2.x;
        this.y = (int)v2.y;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="v3">Vector</param>
    public Vector2i(Vector3 v3) : this()
    {
        this.x = (int)v3.x;
        this.y = (int)v3.y;
    }

    /// <summary>
    /// Indexed getter/setter
    /// </summary>
    /// <remarks>
    /// Indexed getter/setter
    /// </remarks>
    /// <param name="index">Index</param>
    /// <returns>Component value</returns>
    public int this[int index]
    {
        get
        {
#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (index)
#pragma warning restore IDE0066 // Convert switch statement to expression
            {
                case 0:
                    return this.x;
                case 1:
                    return this.y;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector2i index!");
            }
        }

        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;
                case 1:
                    this.y = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector2i index!");
            }
        }
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <remarks>
    /// Implicit operator.
    /// </remarks>
    /// <param name="vector">Vector</param>
    public static implicit operator Vector2i(Vector2 vector)
    {
        return new Vector2i(vector);
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <param name="vector">Vector</param>
    public static implicit operator Vector2i(Vector3 vector)
    {
        return new Vector2i(vector);
    }

    /// <summary>
    /// Add two vectors together
    /// </summary>
    /// <remarks>
    /// Add two vector together, returning the result in a new vector.
    /// </remarks>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator +(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x + b.x, a.y + b.y);
    }

    /// <summary>
    /// Subtract two vectors from each other
    /// </summary>
    /// <remarks>
    /// Subtract two vectors from each other, returning the result in a new vector.
    /// </remarks>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator -(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x - b.x, a.y - b.y);
    }

    /// <summary>
    /// Negate vector
    /// </summary>
    /// <remarks>
    /// Negate vector, returning result in a new vector.
    /// </remarks>
    /// <param name="a">Vector</param>
    /// <returns>Result</returns>
    public static Vector2i operator -(Vector2i a)
    {
        return new Vector2i(-a.x, -a.y);
    }

    /// <summary>
    /// Multiply two by integer scalar
    /// </summary>
    /// <remarks>
    /// Multiply vector by an integer scalar.
    /// </remarks>
    /// <param name="a">Left</param>
    /// <param name="d">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(Vector2i a, int d)
    {
        return new Vector2i(a.x * d, a.y * d);
    }

    /// <summary>
    /// Multiply vector by float scalar
    /// </summary>
    /// <remarks>
    /// Multiply vector by a float scalar.
    /// </remarks>
    /// <param name="scalar">Scalar</param>
    /// <param name="vector">Vector</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(int scalar, Vector2i vector)
    {
        return new Vector2i(vector.x * scalar, vector.y * scalar);
    }

    /// <summary>
    /// Multiply two vectors together
    /// </summary>
    /// <remarks>
    /// Multiply two vectors together, returning the result in a new vector.
    /// </remarks>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x * b.x, a.y * b.y);
    }

    /// <summary>
    /// Multiply two vectors together
    /// </summary>
    /// <remarks>
    /// Multiply two vectors together, returning the result in a new vector.
    /// </remarks>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(Vector2i a, Vector2 b)
    {
        return new Vector2i(a.x * b.x, a.y * b.y);
    }

    /// <summary>
    /// Divide vector by scalar
    /// </summary>
    /// <remarks>
    /// Divide vector by scalar, returning the result in a new vector.
    /// </remarks>
    /// <param name="vector">Vector</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Vector2i operator /(Vector2i vector, int scalar)
    {
        return new Vector2i(vector.x / scalar, vector.y / scalar);
    }

    /// <summary>
    /// Multiply vector by scalar
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(Vector2i vector, float scalar)
    {
        return new Vector2i(vector.x * scalar, vector.y * scalar);
    }

    /// <summary>
    /// Multiply vector by scalar
    /// </summary>
    /// <param name="scalar">Scalar</param>
    /// <param name="vector">Vector</param>
    /// <returns>Result</returns>
    public static Vector2i operator *(float scalar, Vector2i vector)
    {
        return new Vector2i(vector.x * scalar, vector.y * scalar);
    }

    /// <summary>
    /// Divide by scalar
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <param name="scalar">Scalar</param>
    /// <returns>Result</returns>
    public static Vector2i operator /(Vector2i vector, float scalar)
    {
        return new Vector2i(vector.x / scalar, vector.y / scalar);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <remarks>
    /// Equality operator.
    /// </remarks>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(Vector2i a, Vector2i b)
    {
        return a.x == b.x && a.y == b.y;
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <remarks>
    /// Inequality operator.
    /// </remarks>
    /// <param name="a">Left side</param>
    /// <param name="b">Right side</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(Vector2i a, Vector2i b)
    {
        return a.x != b.x || a.y != b.y;
    }

    /// <summary>
    /// Get length of vector.
    /// </summary>
    /// <remarks>
    /// Get length of a vector. This is a relatively expensive operation as it involves a square root calculation.
    /// <see cref="Vector2i.SqrMagnitude"/> should be used if a relative magnitude/distance is sufficient.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     float dist = (playerPos - enemyPos).Magnitude();
    ///     if (dist < 10.0f) {
    ///         playerHealth--;
    ///     }
    /// }
    /// </code>
    /// <returns>Magnitude</returns>
    /// <seealso cref="Vector2i.SqrMagnitude"/>
    public float Magnitude()
    {
        return Mathf.Sqrt((float)((x * x) + (y * y)));
    }

    /// <summary>
    /// Get squared length of vector.
    /// </summary>
    /// <remarks>
    /// Get the squared length of a vector. This is much faster than <see cref="Vector2i.Magnitude"/>.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     int shortestDist = 99999;
    ///     Vector2i closestEnemyPos;
    ///
    ///     // Find the closest enemy by comparing relative squared distances.
    ///     foreach (Vector2i enemyPos in enemyPos) {
    ///         int squareDist = (playerPos - enemyPos).SqrMagnitude();
    ///         if (squareDist < shortestDist) {
    ///             closestEnemyPos = enemyPos;
    ///             shortestDistance = squareDist;
    ///         }
    ///     }
    ///
    ///     if (shorestDist < 99999) {
    ///         AreaAttack(closestEnemyPos);
    ///     }
    /// }
    /// </code>
    /// <returns>Square magnitude</returns>
    /// <seealso cref="Vector2i.Magnitude"/>
    public int SqrMagnitude()
    {
        return (x * x) + (y * y);
    }

    /// <summary>
    /// Convert to Vector2
    /// </summary>
    /// <remarks>
    /// Convert to Vector2.
    /// </remarks>
    /// <returns>Vector2</returns>
    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <remarks>
    /// Convert to string.
    /// </remarks>
    /// <returns>String</returns>
    public override string ToString()
    {
        return string.Format("({0}, {1})", new object[] { this.x, this.y });
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <param name="format">Format</param>
    /// <returns>String</returns>
    public string ToString(string format)
    {
        return string.Format("({0}, {1})", new object[] { this.x.ToString(format), this.y.ToString(format) });
    }

    /// <summary>
    /// Vector2i equality
    /// </summary>
    /// <remarks>
    /// Vector2i equality
    /// </remarks>
    /// <param name="other">Other Vector2i</param>
    /// <returns>True if equal</returns>
    public bool Equals(Vector2i other)
    {
        return x == other.x && y == other.y;
    }

    /// <summary>
    /// Object equality
    /// </summary>
    /// <remarks>
    /// Object equality.
    /// </remarks>
    /// <param name="other">Other</param>
    /// <returns>True if equal</returns>
    public override bool Equals(object other)
    {
        bool result;

#pragma warning disable IDE0038 // Use pattern matching
        if (!(other is Vector2i))
#pragma warning restore IDE0038 // Use pattern matching
        {
            result = false;
        }
        else
        {
            Vector2i v = (Vector2i)other;
            result = this.x.Equals(v.x) && this.y.Equals(v.y);
        }

        return result;
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <remarks>
    /// Get the hash code of the vector.
    /// </remarks>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
    }
}
