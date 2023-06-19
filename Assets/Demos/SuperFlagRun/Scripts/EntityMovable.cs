namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// A moveable entity, with physics interactions
    /// </summary>
    public class EntityMovable : Entity
    {
        /// <summary>
        /// Physics state
        /// </summary>
        protected PlatformPhysics mPhysics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        public EntityMovable(Vector2 pos) : base(pos)
        {
            mPhysics = new PlatformPhysics(this);
        }

        /// <summary>
        /// Get physics
        /// </summary>
        public PlatformPhysics Physics
        {
            get { return mPhysics; }
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            UpdateVelocity();
            mPhysics.ApplyVelocity();
        }

        /// <summary>
        /// Update velocity, must be implemented by subclass
        /// </summary>
        protected virtual void UpdateVelocity()
        {
        }

        /// <summary>
        /// Update drag, must be implemented by subclass
        /// </summary>
        protected virtual void UpdateDrag()
        {
        }
    }
}
