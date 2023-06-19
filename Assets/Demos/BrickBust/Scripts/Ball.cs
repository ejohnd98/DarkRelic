namespace RetroBlitDemoBrickBust
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Ball, wrecker of bricks, destroyer of paddles
    /// </summary>
    public class Ball : Collidable
    {
        /// <summary>
        /// Dead flag
        /// </summary>
        public bool Dead;

        /// <summary>
        /// Whether the ball is currently stuck to the player paddle
        /// </summary>
        public bool StuckToPaddle;

        /// <summary>
        /// Current speed of the ball
        /// </summary>
        private float mCurrentSpeed = 0.0f;

        /// <summary>
        /// Desired speed of the ball, the ball speed will smoothly approach this
        /// value so that there isn't a jerky speedup/slowdown effect
        /// </summary>
        private float mSpeed = 0.0f;

        /// <summary>
        /// Direction of travel, this is a unit vector
        /// </summary>
        private Vector2 mDir;

        /// <summary>
        /// Current transparency of the ball, ball fades in when it is first created
        /// </summary>
        private int mAlpha;

        /// <summary>
        /// Helps to track players double tap action
        /// </summary>
        private float mLastTapTime;

        /// <summary>
        /// Position of the ball
        /// </summary>
        private Vector2 mPos;

        /// <summary>
        /// Used in DoPhysics() to track all collidables that have collided with the ball, this could be
        /// defined in DoPhysics but keeping it here can help with garbage collection a bit
        /// </summary>
        private List<Collidable> mHitCollidable = new List<Collidable>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position of the ball</param>
        public Ball(Vector2i pos)
        {
            Rect = new Rect2i(pos.x - (Global.BALL_WIDTH / 2), pos.y - (Global.BALL_HEIGHT / 2), Global.BALL_WIDTH, Global.BALL_HEIGHT);
            mPos = new Vector2(Rect.x, Rect.y);
            mDir = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), -1.0f);
            mDir.Normalize();

            StuckToPaddle = true;
            ResetSpeed();
        }

        /// <summary>
        /// Direction the ball is travelling in, this is a unit vector
        /// </summary>
        public Vector2 Dir
        {
            get { return mDir; }
            set { mDir = value; }
        }

        /// <summary>
        /// Speed of travel
        /// </summary>
        public float Speed
        {
            get { return mSpeed; }
            set { mSpeed = value; }
        }

        /// <summary>
        /// Speed the ball up
        /// </summary>
        public void SpeedUp()
        {
            mSpeed += 0.15f;
            if (mSpeed > 10)
            {
                mSpeed = 10;
            }
        }

        /// <summary>
        /// Reset the ball speed
        /// </summary>
        public void ResetSpeed()
        {
            mSpeed = 3.0f;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Animate fade-in
            mAlpha = Math.Min(255, mAlpha + 10);

            bool doubleTap = false;
            if (RB.ButtonPressed(RB.BTN_POINTER_A))
            {
                if (Time.time - mLastTapTime < 0.5f)
                {
                    doubleTap = true;
                }

                mLastTapTime = Time.time;
            }

            // If the ball is stuck to the paddle then snap its position to the middle of the paddle
            if (StuckToPaddle)
            {
                var game = (BrickBustGame)RB.Game;
                var paddleRect = game.Level.Paddle.Rect;

                Rect.x = (int)paddleRect.center.x - (Rect.width / 2);
                Rect.y = paddleRect.y - (Global.BALL_HEIGHT / 2);
                mPos.x = Rect.x;
                mPos.y = Rect.y;

                // Release the paddle on double tap
                if (doubleTap)
                {
                    StuckToPaddle = false;
                    game.Level.Hud.HideHelp();
                }
            }
            else
            {
                // Break up the velocity into increments that are at maximum quarter the size of the ball, this will prevent the ball from
                // jumping entirely across bricks/paddle if it's moving fast. If there is a collision then break out out of the loop.
                float distanceLeft = mCurrentSpeed;
                float increment = Math.Min(Global.BALL_HEIGHT / 4.0f, Global.BALL_WIDTH / 4.0f);

                while (distanceLeft > 0)
                {
                    mPos += mDir * Math.Min(increment, distanceLeft);
                    Rect.x = (int)mPos.x;
                    Rect.y = (int)mPos.y;

                    if (DoPhysics())
                    {
                        break;
                    }

                    distanceLeft -= increment;
                }

                CheckDeath();
            }

            // Animate ball speed
            if (mCurrentSpeed < mSpeed)
            {
                mCurrentSpeed += 0.1f;
                if (mCurrentSpeed > mSpeed)
                {
                    mCurrentSpeed = mSpeed;
                }
            }
            else if (mCurrentSpeed > mSpeed)
            {
                mCurrentSpeed -= 0.1f;
                if (mCurrentSpeed < mSpeed)
                {
                    mCurrentSpeed = mSpeed;
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            byte prevAlpha = RB.AlphaGet();

            // Be sure to account for shadow render pass here
            RB.AlphaSet((byte)((mAlpha / 255.0f) * prevAlpha));

            RB.DrawSprite(RB.SpriteIndex(FlashFrame, 2), new Vector2i(Rect.x, Rect.y));
            RB.AlphaSet(prevAlpha);
        }

        /// <summary>
        /// Bounce the ball off the paddle, this causes a new direction of travel which depends on where the
        /// ball strikes the paddle, middle causes an angle close to straight up, and sides cause flat angles.
        /// The angle is clipped so that straight orthogonal angles are not allowed, they would cause odd behaviour
        /// and boring game play, and possibly a ball stuck bouncing between walls.
        /// </summary>
        private void PaddleBounce()
        {
            // Calculate new angle
            var game = (BrickBustGame)RB.Game;
            var paddleRect = game.Level.Paddle.Rect;

            var distFromPaddleMid = Rect.center.x - paddleRect.center.x;
            var distRatio = distFromPaddleMid / (float)(paddleRect.width / 2);

            float angle = 90 + (70 * distRatio);

            // Don't let the angle get too large or too small to prevent near orthogonal velocity
            if (angle < 25)
            {
                angle = 25;
            }

            if (angle > 155)
            {
                angle = 155;
            }

            if (angle < 105 && angle >= 90)
            {
                angle = 105;
            }

            if (angle > 75 && angle <= 90)
            {
                angle = 75;
            }

            mDir = Quaternion.Euler(0, 0, angle) * new Vector2(-1, 0);
        }

        /// <summary>
        /// Perform physics logic
        /// </summary>
        /// <returns>Return true if ball has hit something</returns>
        private bool DoPhysics()
        {
            var game = (BrickBustGame)RB.Game;
            bool bounceX = false;
            bool bounceY = false;

            mHitCollidable.Clear();

            // Check collisions vs bricks
            for (int i = 0; i < game.Level.Bricks.Count; i++)
            {
                CheckCollision(game.Level.Bricks[i], ref bounceX, ref bounceY);
            }

            // Check collisions vs walls
            for (int i = 0; i < game.Level.Walls.Count; i++)
            {
                CheckCollision(game.Level.Walls[i], ref bounceX, ref bounceY);
            }

            // Check collision vs paddle
            CheckCollision(game.Level.Paddle, ref bounceX, ref bounceY);

            Vector2i hitPos = new Vector2i((int)Rect.center.x, (int)Rect.center.y);

            // If we only hit one thing then no special consideration is needed
            if (mHitCollidable.Count == 1)
            {
                mHitCollidable[0].Hit(this, hitPos, mDir * mSpeed);
                Hit(this, hitPos, mDir * mSpeed);
            }

            // If we hit two things then they both must have been bricks (paddle is too far to be hit at same time
            // as a brick). If the bricks are vertically or horizontaly stacked then we might be trying
            // to bounce off the common side between the two bricks, which would be wrong. Correct for this by bouncing only
            // on X axis if bricks are stacked vertically, and only on Y axis if they're stacked horizontally.
            if (mHitCollidable.Count == 2)
            {
                var collidableDelta = mHitCollidable[0].Rect.center - mHitCollidable[1].Rect.center;

                if (collidableDelta.y != 0 && collidableDelta.x == 0)
                {
                    bounceX = true;
                    bounceY = false;
                }
                else if (collidableDelta.x != 0 && collidableDelta.y == 0)
                {
                    bounceX = false;
                    bounceY = true;
                }

                float dist0 = (mHitCollidable[0].Rect.center - Rect.center).SqrMagnitude();
                float dist1 = (mHitCollidable[1].Rect.center - Rect.center).SqrMagnitude();
                var hitCollidable = dist0 < dist1 ? mHitCollidable[0] : mHitCollidable[1];

                hitCollidable.Hit(this, hitPos, mDir * mSpeed);
                Hit(this, hitPos, mDir * mSpeed);
            }

            // If we hit 3 bricks then they must be layed out in an L pattern, the corner brick should not actually be hit,
            // we can ignore it here. We can check that a brick is not a corner brick by checking if it doesn't share
            // neither the X nor Y coordinate with one of the other bricks. Consider this illustration:
            //                                    AB
            //                                     C
            // B has same X coordinate as C, and same Y coordinate as A, it must be a corner brick. Brick A has the same X coordinate
            // as B, but does not share coordinates with C, so it is not a corner brick.
            if (mHitCollidable.Count == 3)
            {
                var a = mHitCollidable[0].Rect.center;
                var b = mHitCollidable[1].Rect.center;
                var c = mHitCollidable[2].Rect.center;

                if ((a.x != b.x && a.y != b.y) || (a.x != c.x && a.y != c.y))
                {
                    mHitCollidable[0].Hit(this, hitPos, mDir * mSpeed);
                }

                if ((b.x != a.x && b.y != a.y) || (b.x != c.x && b.y != c.y))
                {
                    mHitCollidable[1].Hit(this, hitPos, mDir * mSpeed);
                }

                if ((c.x != a.x && c.y != a.y) || (c.x != b.x && c.y != b.y))
                {
                    mHitCollidable[2].Hit(this, hitPos, mDir * mSpeed);
                }

                Hit(this, hitPos, mDir * mSpeed);
            }

            if (bounceX)
            {
                mDir.x = -mDir.x;
            }

            if (bounceY)
            {
                mDir.y = -mDir.y;
            }

            return mHitCollidable.Count > 0 ? true : false;
        }

        /// <summary>
        /// Check if a collision happened against the given collider
        /// </summary>
        /// <param name="collidable">Collider</param>
        /// <param name="bounceX">Whether we should bounce on X axis as result of the collision</param>
        /// <param name="bounceY">Whether we should bounce on Y axis as result of the collision</param>
        private void CheckCollision(Collidable collidable, ref bool bounceX, ref bool bounceY)
        {
            var game = (BrickBustGame)RB.Game;

            bool hit = false;
            bool isPaddle = collidable is Paddle;

            for (int pass = 0; pass < 2; pass++)
            {
                // If we should only collide with top side then ignore collision if already past top side
                if (isPaddle && Rect.y > collidable.Rect.y)
                {
                    continue;
                }

                var depth = collidable.CollideRect.IntersectionDepth(Rect);

                if (depth.x != 0 || depth.y != 0)
                {
                    // Resolve shallow axis first
                    if (Mathf.Abs(depth.x) != 0 && Mathf.Abs(depth.x) < Mathf.Abs(depth.y))
                    {
                        hit = true;

                        if (!mHitCollidable.Contains(collidable))
                        {
                            mHitCollidable.Add(collidable);
                        }

                        Rect.x -= (int)depth.x;
                        bounceX = true;
                    }
                    else
                    {
                        Rect.y -= (int)depth.y;

                        hit = true;

                        if (!mHitCollidable.Contains(collidable))
                        {
                            mHitCollidable.Add(collidable);
                        }

                        // Paddle bouncing in Y axis gets special treatment where we want to control the bounce angle.
                        if (isPaddle)
                        {
                            PaddleBounce();
                        }
                        else
                        {
                            bounceY = true;
                        }
                    }
                }
            }

            if (hit)
            {
                mPos.x = Rect.x;
                mPos.y = Rect.y;
            }
        }

        /// <summary>
        /// Check if ball is "dead", a dead ball is a ball that is out of bounds
        /// </summary>
        private void CheckDeath()
        {
            var game = (BrickBustGame)RB.Game;

            if (!Dead && mPos.y > RB.DisplaySize.height)
            {
                Dead = true;
                RB.SoundPlay(game.assets.soundDeath);
            }
        }
    }
}
