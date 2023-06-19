namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Custom platformer physics code, tailored for player entity, would need to be generalized to be used for other
    /// types of entities
    /// </summary>
    public class PlatformPhysics
    {
        /// <summary>
        /// Force of gravity
        /// </summary>
        public const float GRAVITY = 0.12f;

        /// <summary>
        /// Movement acceleration
        /// </summary>
        public float MoveAccel = 50;

        /// <summary>
        /// Max movement speed
        /// </summary>
        public float MoveSpeed = 2.25f;

        /// <summary>
        /// Max jump force
        /// </summary>
        public float MaxJumpForce = 4.25f;

        /// <summary>
        /// Min jump force
        /// </summary>
        public float MinJumpForce = 3.485f;

        /// <summary>
        /// Force of drag for movement
        /// </summary>
        public float InverseDrag = 0.5f;

        /// <summary>
        /// Amount of air control while not on ground
        /// </summary>
        public float AirControl = 0.5f;

        private Entity mEntity;

        private Vector2 mFVel;
        private bool mIsOnGround;

        private bool mJumped = false;

        private Rect2i mPreviousRect;

        private bool mMovementApplied = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entity">Entity that this physics instance will control</param>
        public PlatformPhysics(Entity entity)
        {
            mEntity = entity;

            mFVel = new Vector2(0, 0);

            mIsOnGround = false;

            mPreviousRect = mEntity.ColliderInfo.Rect.Offset(new Vector2i((int)mEntity.Pos.x, (int)mEntity.Pos.y));
        }

        /// <summary>
        /// Get/Set velocity
        /// </summary>
        public Vector2 Velocity
        {
            get { return mFVel; }
            set { mFVel = value; }
        }

        /// <summary>
        /// Get on ground status
        /// </summary>
        public bool IsOnGround
        {
            get { return mIsOnGround; }
        }

        /// <summary>
        /// Get jumped status
        /// </summary>
        public bool Jumped
        {
            get { return mJumped; }
        }

        /// <summary>
        /// Perform a jump
        /// </summary>
        public void Jump()
        {
            var game = (SuperFlagRun)RB.Game;

            mJumped = false;
            if (mIsOnGround)
            {
                // Adjust the jump force by horizontal velocity, the faster the higher the jump
                var jumpForce = ((Mathf.Abs(mFVel.x) / MoveSpeed) * (MaxJumpForce - MinJumpForce)) + MinJumpForce;
                mFVel.y = -jumpForce;
                mJumped = true;

                RB.SoundPlay(game.assets.soundJump, 0.5f, Random.Range(0.9f, 1.1f));
            }
        }

        /// <summary>
        /// Add a force, unconstrained
        /// </summary>
        /// <param name="force">Force</param>
        public void AddForce(Vector2 force)
        {
            mFVel += force;
        }

        /// <summary>
        /// Add force constrained by maximum movement speed
        /// This has no effect if the existing force already is >= movespeed
        /// </summary>
        /// <param name="force">Force</param>
        public void AddMovementForce(Vector2 force)
        {
            if (force.x == 0 && force.y == 0)
            {
                return;
            }

            // Note this technically would allow total velocity to be above MoveSpeed
            // if both axis are moving at or need MoveSpeed... meh, dont care about that right now

            // Less control while falling
            if (!mIsOnGround)
            {
                force *= AirControl;
            }

            // X Axis
            if (force.x > 0)
            {
                if (mFVel.x < MoveSpeed)
                {
                    mFVel.x += force.x;
                    if (mFVel.x > MoveSpeed)
                    {
                        mFVel.x = MoveSpeed;
                    }
                }
            }
            else if (force.x < 0)
            {
                if (mFVel.x > -MoveSpeed)
                {
                    mFVel.x += force.x;
                    if (mFVel.x < -MoveSpeed)
                    {
                        mFVel.x = -MoveSpeed;
                    }
                }
            }

            // Y Axis
            if (force.y > 0)
            {
                if (mFVel.y < MoveSpeed)
                {
                    mFVel.y += force.y;
                    if (mFVel.y > MoveSpeed)
                    {
                        mFVel.y = MoveSpeed;
                    }
                }
            }
            else if (force.y < 0)
            {
                if (mFVel.y > -MoveSpeed)
                {
                    mFVel.y += force.y;
                    if (mFVel.y < -MoveSpeed)
                    {
                        mFVel.y = -MoveSpeed;
                    }
                }
            }

            mMovementApplied = true;
        }

        /// <summary>
        /// Apply velocity
        /// </summary>
        public void ApplyVelocity()
        {
            Vector2 velocity = mFVel;

            float lenLeft = velocity.magnitude;

            // float fudge to ensure that when splitting velocity into steps we still add up to at least the total length of the
            // velocity vector
            lenLeft += 0.0001f;

            Vector2 normalizedVelocity = velocity.normalized;
            float incr = RB.SpriteSheetGet().grid.cellSize.width / 4.0f;
            Vector3 newPos;

            while (lenLeft > 0)
            {
                lenLeft -= incr;
                float len = incr;
                if (lenLeft < 0)
                {
                    len += lenLeft;
                    lenLeft = 0;
                }

                Vector2 incrDelta = normalizedVelocity * len;

                // Note that we should ALWAYS resolve along Y and X axis even if player movement is 0,
                // we might be colliding with other moving entities/platforms

                // Always resolve the Y axis first
                newPos = mEntity.Pos;
                newPos.y += incrDelta.y;
                ResolveCollisions(newPos, 1);

                newPos = mEntity.Pos;
                newPos.x += incrDelta.x;
                ResolveCollisions(newPos, 0);
            }

            // Apply Drag

            // Decay horizontal velocity
            if (mIsOnGround && !mMovementApplied)
            {
                mFVel.x *= InverseDrag;
            }

            if (Mathf.Abs(mFVel.x) < 0.01f)
            {
                mFVel.x = 0;
            }

            if (Mathf.Abs(mFVel.y) < 0.01f)
            {
                mFVel.y = 0;
            }

            mMovementApplied = false;
        }

        /// <summary>
        /// The heart of platformer physics, resolves any collisions
        /// </summary>
        /// <param name="newPos">New position to resolve collisions for</param>
        /// <param name="axis">Axis to resolve on 0 = X, 1 = Y</param>
        private void ResolveCollisions(Vector2 newPos, int axis)
        {
            Rect2i rect = mEntity.ColliderInfo.Rect.Offset(new Vector2i((int)newPos.x, (int)newPos.y));

            int leftTile = (int)Mathf.Floor((int)rect.min.x / RB.SpriteSheetGet().grid.cellSize.width) - 1;
            int rightTile = (int)Mathf.Ceil((int)rect.max.x / RB.SpriteSheetGet().grid.cellSize.width);
            int topTile = (int)Mathf.Ceil((int)rect.min.y / RB.SpriteSheetGet().grid.cellSize.height) - 1;
            int bottomTile = (int)Mathf.Floor((int)rect.max.y / RB.SpriteSheetGet().grid.cellSize.height);

            /* If we're calculating Y axis then reset the on ground flag. It will be set
               to true if we hit something from the top */
            if (axis == 1)
            {
                mIsOnGround = false;
            }

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    var tile = RB.MapSpriteGet(0, new Vector2i(x, y));
                    if (tile != RB.SPRITE_EMPTY)
                    {
                        var collisionType = RB.MapDataGet<ColliderInfo.ColliderType>(SuperFlagRun.MAP_LAYER_TERRAIN, new Vector2i(x, y));

                        if (collisionType == ColliderInfo.ColliderType.NONE)
                        {
                            continue;
                        }

                        Rect2i tileRect = new Rect2i(x * RB.SpriteSheetGet().grid.cellSize.width, y * RB.SpriteSheetGet().grid.cellSize.height, RB.SpriteSheetGet().grid.cellSize.width, RB.SpriteSheetGet().grid.cellSize.height);

                        bool collided;
                        Vector2 depth;

                        newPos = ResolveCollisionForRects(rect, tileRect, newPos, collisionType, null, axis, out collided, out depth);

                        // Perform further collisions with the new bounds.
                        rect = mEntity.ColliderInfo.Rect.Offset(new Vector2i((int)newPos.x, (int)newPos.y));

                        if (!mIsOnGround)
                        {
                            Rect2i groundCheckRect = rect;
                            groundCheckRect.y += 1;

                            depth = groundCheckRect.IntersectionDepth(tileRect);
                            if (depth.y < 0)
                            {
                                mIsOnGround = true;
                            }
                        }
                    }
                }
            }

            mPreviousRect = mEntity.ColliderInfo.Rect.Offset(new Vector2i((int)newPos.x, (int)newPos.y));

            mEntity.Pos = newPos;

            mJumped = false;
        }

        private Vector2 ResolveCollisionForRects(Rect2i entityRect, Rect2i colliderRect, Vector2 newPos, ColliderInfo.ColliderType collisionType, Entity collidingEntity, int axis, out bool collided, out Vector2 depth)
        {
            depth = entityRect.IntersectionDepth(colliderRect);
            collided = false;

            /* NOTE, response is mulitpled by 1.0001f to correct for float imprecision. Without
               this the response might not actually clear the collision, and that would be bad */
            if (axis == 0 && depth.x != 0 && collisionType != ColliderInfo.ColliderType.PLATFORM)
            {
                newPos.x += depth.x * 1.0001f;
                mFVel.x = 0;
                collided = true;
            }
            else if (axis == 1 && depth.y != 0)
            {
                float colliderMovementOffset = 0;

                // If the colliding entity is moving upwards then adjust out collider check by the delta. This is
                // a hack in attempt to fix mixed collisions on upwards moving platforms. I'd like to remove this eventually ***
                if (collidingEntity != null && collidingEntity.PosDelta.y < 0)
                {
                    colliderMovementOffset = collidingEntity.PosDelta.y * 1.25f;
                }

                /* If we intersect with the top of a tile then we're on the ground */
                if (depth.y < 0 && mPreviousRect.max.y <= colliderRect.min.y - colliderMovementOffset)
                {
                    collided = true;
                    mIsOnGround = true;
                }

                // Ignore collisions for platforms unless on ground
                if (collisionType == ColliderInfo.ColliderType.BLOCK || (depth.y < 0 && (int)mPreviousRect.max.y <= ((int)colliderRect.min.y - colliderMovementOffset)))
                {
                    collided = true;
                    newPos.y += depth.y * 1.0001f;
                    mFVel.y = 0;
                }
            }

            return newPos;
        }
    }
}
