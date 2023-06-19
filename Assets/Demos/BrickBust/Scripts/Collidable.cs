namespace RetroBlitDemoBrickBust
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Base collidable class
    /// </summary>
    public class Collidable
    {
        /// <summary>
        /// Rect of this collidable
        /// </summary>
        public Rect2i Rect;

        /// <summary>
        /// Most collidables have a hit flash animation made of multiple frames, this keeps track of current frame
        /// </summary>
        public int FlashFrame = 0;

        /// <summary>
        /// Whether this collidable must be destroyed to win a level
        /// </summary>
        public bool RequiredForWin = false;

        /// <summary>
        /// Get collision rect
        /// </summary>
        public virtual Rect2i CollideRect
        {
            get { return Rect; }
        }

        /// <summary>
        /// Handle a hit
        /// </summary>
        /// <param name="collider">Who hit us</param>
        /// <param name="pos">Contact position</param>
        /// <param name="velocity">Velocity of the collision</param>
        public virtual void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
        {
            FlashFrame = 3;
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            if (RB.Ticks % 4 == 0)
            {
                FlashFrame = Math.Max(0, FlashFrame - 1);
            }
        }
    }
}