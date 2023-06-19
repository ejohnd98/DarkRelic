namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// Represents a direction as a vector, or collection of directions such as Cardinal and Diagonal
    /// </summary>
    public struct Direction
    {
        /// <summary>
        /// North
        /// </summary>
        public static readonly Vector2i N = new Vector2i(0, -1);

        /// <summary>
        /// North-East
        /// </summary>
        public static readonly Vector2i NE = new Vector2i(1, -1);

        /// <summary>
        /// East
        /// </summary>
        public static readonly Vector2i E = new Vector2i(1, 0);

        /// <summary>
        /// South-East
        /// </summary>
        public static readonly Vector2i SE = new Vector2i(1, 1);

        /// <summary>
        /// South
        /// </summary>
        public static readonly Vector2i S = new Vector2i(0, 1);

        /// <summary>
        /// South-West
        /// </summary>
        public static readonly Vector2i SW = new Vector2i(-1, 1);

        /// <summary>
        /// West
        /// </summary>
        public static readonly Vector2i W = new Vector2i(-1, 0);

        /// <summary>
        /// North-West
        /// </summary>
        public static readonly Vector2i NW = new Vector2i(-1, -1);

        /// <summary>
        /// All  8 directions
        /// </summary>
        public static readonly Vector2i[] All = new Vector2i[] { N, NE, E, SE, S, SW, W, NW };

        /// <summary>
        /// Cardinal directions
        /// </summary>
        public static readonly Vector2i[] Cardinal = new Vector2i[] { N, E, S, W };

        /// <summary>
        /// Diagonal directions
        /// </summary>
        public static readonly Vector2i[] Diagonal = new Vector2i[] { NE, SE, SW, NW };

        /// <summary>
        /// Turn direction to left by 45 degrees
        /// </summary>
        /// <param name="v">Direction vector</param>
        /// <returns>Result vector</returns>
        public static Vector2i Left45(Vector2i v)
        {
            return new Vector2i(Mathf.Clamp(v.x + v.y, -1, 1), Mathf.Clamp(v.y - v.x, -1, 1));
        }

        /// <summary>
        /// Turn direction to right by 45 degrees
        /// </summary>
        /// <param name="v">Direction vector</param>
        /// <returns>Result vector</returns>
        public static Vector2i Right45(Vector2i v)
        {
            return new Vector2i(Mathf.Clamp(v.x - v.y, -1, 1), Mathf.Clamp(v.y + v.x, -1, 1));
        }

        /// <summary>
        /// Turn direction to left by 90 degrees
        /// </summary>
        /// <param name="v">Direction vector</param>
        /// <returns>Result vector</returns>
        public static Vector2i Left90(Vector2i v)
        {
            return new Vector2i(v.y, -v.x);
        }

        /// <summary>
        /// Turn direction to right by 90 degrees
        /// </summary>
        /// <param name="v">Direction vector</param>
        /// <returns>Result vector</returns>
        public static Vector2i Right90(Vector2i v)
        {
            return new Vector2i(-v.y, v.x);
        }
    }
}
