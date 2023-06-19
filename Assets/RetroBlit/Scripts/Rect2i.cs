using System;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// Defines a rectangular area using integers
/// </summary>
/// <remarks>
/// Defines a rectangular area. This structure is similar to *Unity.Rect* but it uses integers rather than floats for its coordinates.
/// </remarks>
public struct Rect2i : IEquatable<Rect2i>
{
    /// <summary>
    /// Zero sized rectangle
    /// </summary>
    /// <remarks>
    /// Zero sized rectangle.
    /// </remarks>
    public static readonly Rect2i zero = new Rect2i(0, 0, 0, 0);

    /// <summary>
    /// X coordinate of top left corner
    /// </summary>
    /// <remarks>
    /// X coordinate of top left corner.
    /// </remarks>
    public int x;

    /// <summary>
    /// Y coordinate of top left corner
    /// </summary>
    /// <remarks>
    /// Y coordinate of top left corner.
    /// </remarks>
    public int y;

    /// <summary>
    /// Width
    /// </summary>
    /// <remarks>
    /// Width of the rectangular region.
    /// </remarks>
    public int width;

    /// <summary>
    /// Height
    /// </summary>
    /// <remarks>
    /// Height of the rectangular region.
    /// </remarks>
    public int height;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="x">X coordinate of top left corner</param>
    /// <param name="y">Y coordinate of top left corner</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public Rect2i(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X coordinate of top left corner</param>
    /// <param name="y">Y coordinate of top left corner</param>
    /// <param name="size">Size</param>
    public Rect2i(int x, int y, Vector2i size)
    {
        this.x = x;
        this.y = y;
        this.width = size.width;
        this.height = size.height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public Rect2i(Vector2i pos, int width, int height)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="size">Size</param>
    public Rect2i(Vector2i pos, Vector2i size)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.width = size.width;
        this.height = size.height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X coordinate of top left corner</param>
    /// <param name="y">Y coordinate of top left corner</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public Rect2i(float x, float y, float width, float height)
    {
        this.x = (int)x;
        this.y = (int)y;
        this.width = (int)width;
        this.height = (int)height;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rect">Rect</param>
    public Rect2i(Rect rect)
    {
        this.x = (int)rect.x;
        this.y = (int)rect.y;
        this.width = (int)rect.width;
        this.height = (int)rect.height;
    }

    /// <summary>
    /// Gets the position of the minimum corner of the rectangle
    /// </summary>
    /// <remarks>
    /// Gets the position of the minimum corner of the rectangle.
    /// </remarks>
    public Vector2i min
    {
        get
        {
            return new Vector2i(x, y);
        }
    }

    /// <summary>
    /// Gets the position of the maximum corner of the rectangle
    /// </summary>
    /// <remarks>
    /// Gets the position of the maximum corner of the rectangle.
    /// </remarks>
    public Vector2i max
    {
        get
        {
            return new Vector2i(x + width, y + height);
        }
    }

    /// <summary>
    /// Gets the center point of the rectangle
    /// </summary>
    /// <remarks>
    /// Gets the center point of the rectangle.
    /// </remarks>
    public Vector2i center
    {
        get
        {
            return new Vector2i(x + (width / 2), y + (height / 2));
        }
    }

    /// <summary>
    /// Indexed getter/setter
    /// </summary>
    /// <remarks>
    /// Indexed getter/setter.
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
                case 2:
                    return this.width;
                case 3:
                    return this.height;
                default:
                    throw new IndexOutOfRangeException("Invalid Rect2i index!");
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
                case 2:
                    this.width = value;
                    break;
                case 3:
                    this.height = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Rect2i index!");
            }
        }
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
    public static bool operator ==(Rect2i a, Rect2i b)
    {
        return a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height;
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
    public static bool operator !=(Rect2i a, Rect2i b)
    {
        return a.x != b.x || a.y != b.y || a.width != b.width || a.height != b.height;
    }

    /// <summary>
    /// Implicit operator
    /// </summary>
    /// <remarks>
    /// Implicit operator.
    /// </remarks>
    /// <param name="rect">Rect</param>
    public static implicit operator Rect2i(Rect rect)
    {
        return new Rect2i(rect);
    }

    /// <summary>
    /// Convert to Rect
    /// </summary>
    /// <remarks>
    /// Convert to *Unity.Rect*.
    /// </remarks>
    /// <returns>Rect</returns>
    public Rect ToRect()
    {
        return new Rect(x, y, width, height);
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <remarks>
    /// Convert to a string.
    /// </remarks>
    /// <returns>String</returns>
    public override string ToString()
    {
        return string.Format("({0}, {1}, {2}, {3})", new object[] { this.x, this.y, this.width, this.height });
    }

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <param name="format">Format</param>
    /// <returns>String</returns>
    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2}, {3})", new object[] { this.x.ToString(format), this.y.ToString(format), this.width.ToString(format), this.height.ToString(format) });
    }

    /// <summary>
    /// Rect2i equality
    /// </summary>
    /// <remarks>
    /// Rect2i equality
    /// </remarks>
    /// <param name="other">Other Rect2i</param>
    /// <returns>True if equal</returns>
    public bool Equals(Rect2i other)
    {
        return x == other.x && y == other.y && width == other.width && height == other.height;
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

        if (!(other is Rect2i i))
        {
            result = false;
        }
        else
        {
            Rect2i r = i;
            result = this.x.Equals(r.x) && this.y.Equals(r.y) && this.width.Equals(r.width) && this.height.Equals(r.height);
        }

        return result;
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <remarks>
    /// Get hash code.
    /// </remarks>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.width.GetHashCode() >> 2 ^ this.height.GetHashCode() << 4;
    }

    /// <summary>
    /// Expand the rectangular area vertically and horizontally by the given amount.
    /// </summary>
    /// <remarks>
    /// Expand the rectangular area vertically and horizontally by the given amount. Positive values will grow the rectangle, and
    /// negative values will shrink it.
    /// </remarks>
    /// <code>
    /// void Render() {
    ///     // Draw progressively smaller rectangles
    ///     var rect = new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height);
    ///     for (int i = 0; i < 8; i++) {
    ///         RB.DrawRect(rect, Color.white);
    ///         rect.Expand(-4);
    ///     }
    /// </code>
    /// <param name="amount">Amount to expand by</param>
    /// <returns>The expanded Rect</returns>
    public Rect2i Expand(int amount)
    {
        return Expand(amount, amount);
    }

    /// <summary>
    /// Expand the rectangular area by the given width and height. Negative values effectively shrink the rectangular area.
    /// </summary>
    /// <param name="widthAmount">Width to expand by</param>
    /// <param name="heightAmount">Height to expand by</param>
    /// <returns>The expanded Rect</returns>
    public Rect2i Expand(int widthAmount, int heightAmount)
    {
        x -= widthAmount;
        width += widthAmount * 2;
        y -= heightAmount;
        height += heightAmount * 2;

        return this;
    }

    /// <summary>
    /// Return true if the given point is contained within the rectangle, or on its boundary.
    /// </summary>
    /// <remarks>
    /// Return true if the given point is contained within the rectangle, or on its boundary.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     if (lavaRect.Contains(playerPos)) {
    ///         Burn();
    ///     }
    /// }
    /// </code>
    /// <param name="point">Point</param>
    /// <returns>True if point is contained within or on rectangle boundary</returns>
    public bool Contains(Vector2i point)
    {
        if (point.x >= x && point.y >= y && point.x <= x + width && point.y <= y + height)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if there is an intersections with another rectangle
    /// </summary>
    /// <remarks>
    /// Checks if there is an intersections with another rectangle.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     // Check for intersection, and resolve.
    ///     if (playerRect.Intersects(blockRect)) {
    ///         var depth = playerRect.IntersectionDepth(blockRect);
    ///         playerRect.x += (int)depth.x;
    ///         playerRect.y += (int)depth.y;
    ///     }
    /// }
    /// </code>
    /// <param name="rect">Other rectangle</param>
    /// <returns>True if there is an intersection, false otherwise.</returns>
    /// <seealso cref="Rect2i.IntersectionDepth"/>
    /// <seealso cref="Rect2i.Intersect"/>
    /// <seealso cref="Rect2i.Contains"/>
    public bool Intersects(Rect2i rect)
    {
        if (rect.x + rect.width <= x || rect.y + rect.height <= y || rect.x >= x + width || rect.y >= y + height)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Calculates how far another rectangle intersects into this rectangle.
    /// </summary>
    /// <remarks>
    /// Calculates how far another rectangle intersects into this rectangle. The returned depth represents how much the
    /// rectangle would have to be offset so that it no longer collides. This method is useful in collision resolution.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     // Check for intersection, and resolve.
    ///     if (playerRect.Intersects(blockRect)) {
    ///         var depth = playerRect.IntersectionDepth(blockRect);
    ///         playerRect.x += (int)depth.x;
    ///         playerRect.y += (int)depth.y;
    ///     }
    /// }
    /// </code>
    /// <param name="rect">Other rect</param>
    /// <returns>Intersection depth</returns>
    /// <seealso cref="Rect2i.Intersect"/>
    /// <seealso cref="Rect2i.Intersects"/>
    /// <seealso cref="Rect2i.Contains"/>
    public Vector2 IntersectionDepth(Rect2i rect)
    {
        Vector2 depth;

        // Calculate half sizes.
        float halfWidthA = width / 2f;
        float halfHeightA = height / 2f;
        float halfWidthB = rect.width / 2f;
        float halfHeightB = rect.height / 2f;

        // Calculate centers.
        Vector2 centerA, centerB;
        centerA = new Vector2((int)min.x + halfWidthA, (int)min.y + halfHeightA);
        centerB = new Vector2((int)rect.min.x + halfWidthB, (int)rect.min.y + halfHeightB);

        // Calculate current and minimum-non-intersecting distances between centers.
        float distanceX = centerA.x - centerB.x;
        float distanceY = centerA.y - centerB.y;
        float minDistanceX = halfWidthA + halfWidthB;
        float minDistanceY = halfHeightA + halfHeightB;

        // If we are not intersecting at all, return (0, 0).
        // NOTE: Potential loss of precision here, might want to create a specialized template for doubles
        if (System.Math.Abs((float)distanceX) >= minDistanceX || System.Math.Abs((float)distanceY) >= minDistanceY)
        {
            depth.x = 0;
            depth.y = 0;
            return depth;
        }

        // Calculate and return intersection depths.
        float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        depth.x = depthX;
        depth.y = depthY;

        return depth;
    }

    /// <summary>
    /// Get a rect representing the intersection area with another rectangle
    /// </summary>
    /// <remarks>
    /// Get a rect representing the intersection area with another rectangle. In other words, the returned rectangle
    /// represents the overlapping area between two rectangles.
    /// </remarks>
    /// <code>
    /// void Render() {
    ///     // Draw the intersection area of the player and block. Useful for collision debugging.
    ///     if (playerRect.Intersects(blockRect)) {
    ///         var area = playerRect.Intersect(blockRect);
    ///         RB.DrawRect(area, Color.red);
    ///     }
    /// }
    /// </code>
    /// <param name="rect">Rectangle to intersect with</param>
    /// <returns>Intersection area</returns>
    /// <seealso cref="Rect2i.Intersects"/>
    /// <seealso cref="Rect2i.IntersectionDepth"/>
    /// <seealso cref="Rect2i.Contains"/>
    public Rect2i Intersect(Rect2i rect)
    {
        int x0 = Math.Max(x, rect.x);
        int x1 = Math.Min(x + width - 1, rect.x + rect.width - 1);
        int y0 = Math.Max(y, rect.y);
        int y1 = Math.Min(y + height - 1, rect.y + rect.height -1);

        if ((x1 >= x0) && (y1 >= y0))
        {
            return new Rect2i(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
        }

        return new Rect2i();
    }

    /// <summary>
    /// Get a copy of the rect offset by given position.
    /// </summary>
    /// <remarks>
    /// Returns a copy of the rectangle offset by given position. Does not change the original rectangle.
    /// </remarks>
    /// <code>
    /// void Update() {
    ///     var newPlayerRect = playerRect.Offset(playerPos - oldPlayerPos);
    /// }
    /// </code>
    /// <param name="pos">Position to offset by</param>
    /// <returns>An offset rect</returns>
    public Rect2i Offset(Vector2i pos)
    {
        Rect2i offsetRect = this;
        offsetRect.x += pos.x;
        offsetRect.y += pos.y;

        return offsetRect;
    }
}
