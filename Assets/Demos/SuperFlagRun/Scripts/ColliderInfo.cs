namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Collision parameters
    /// </summary>
    public class ColliderInfo
    {
        /// <summary>
        /// Collider type
        /// </summary>
        public ColliderType Type = ColliderType.NONE;

        /// <summary>
        /// Rectangular area of this collider
        /// </summary>
        public Rect2i Rect;

        /// <summary>
        /// Collision type, these values must match the meta data set for tiles in tilesets
        /// </summary>
        public enum ColliderType
        {
            /// <summary>
            /// None colliding
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Collide from all directions
            /// </summary>
            BLOCK = 1,

            /// <summary>
            /// Collide only from the top
            /// </summary>
            PLATFORM = 2,

            /// <summary>
            /// Colliding entity
            /// </summary>
            ENTITY = 3
        }
    }
}
