namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// An entity in the game world.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Floating point position of the entity, for subpixel accuracy of physics
        /// </summary>
        protected Vector2 mFPos;

        /// <summary>
        /// Floating point position delta since last update
        /// </summary>
        protected Vector2 mFPosDelta;

        /// <summary>
        /// Collider attached to this entity
        /// </summary>
        protected ColliderInfo mColliderInfo = new ColliderInfo();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position to create this entity at</param>
        public Entity(Vector2 pos)
        {
            // Snap position to integer values
            mFPos = new Vector2((int)pos.x, (int)pos.y);
        }

        /// <summary>
        /// Get the collider
        /// </summary>
        public ColliderInfo ColliderInfo
        {
            get { return mColliderInfo; }
        }

        /// <summary>
        /// Get the position
        /// </summary>
        public Vector2 Pos
        {
            get
            {
                return mFPos;
            }

            set
            {
                mFPosDelta = value - mFPos;
                mFPos = value;
            }
        }

        /// <summary>
        /// Get the position delta
        /// </summary>
        public Vector2 PosDelta
        {
            get { return mFPosDelta; }
        }

        /// <summary>
        /// Update, must be implemented by subclass
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// Render, must be implemented by subclass
        /// </summary>
        public virtual void Render()
        {
        }
    }
}
