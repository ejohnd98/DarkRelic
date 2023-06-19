namespace RetroBlitDemoBrickBust
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The paddle, otherwise known as the player
    /// </summary>
    public class Paddle : Collidable
    {
        private Vector2i mPrevPointerPos = Vector2i.zero;
        private bool mDragging = false;
        private Vector2i mPos;
        private Rect2i mBaseRect;
        private Rect2i mExtendedRect;
        private List<NineSlice> mNSFrame = new List<NineSlice>();
        private float mLaserOffset = 1.0f;
        private int mLaserTurn = 0;

        // Powers
        private bool mExtended = false;
        private bool mCatch = false;
        private bool mLaser = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Paddle()
        {
            var game = (BrickBustGame)RB.Game;

            // If mobile then we want to leave a bigger gap on the bottom of the screen as a touch space
            if (Application.isMobilePlatform)
            {
                Rect = new Rect2i((RB.DisplaySize.width / 2) - 12, RB.DisplaySize.height - (game.assets.spriteSheet.grid.cellSize.height * 6), game.assets.spriteSheet.grid.cellSize.width * 3, game.assets.spriteSheet.grid.cellSize.height);
            }
            else
            {
                Rect = new Rect2i((RB.DisplaySize.width / 2) - 12, RB.DisplaySize.height - (game.assets.spriteSheet.grid.cellSize.height * 3), game.assets.spriteSheet.grid.cellSize.width * 3, game.assets.spriteSheet.grid.cellSize.height);
            }

            mBaseRect = Rect;
            mExtendedRect = new Rect2i(Rect.x - 10, Rect.y, Rect.width + 20, Rect.height);

            mPos = new Vector2i((int)Rect.center.x, (int)Rect.center.y);

            int frameOffset = 0;
            mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i()));
            frameOffset += 30;
            mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i()));
            frameOffset += 30;
            mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i()));
            frameOffset += 30;
            mNSFrame.Add(new NineSlice(new Rect2i(0 + frameOffset, 10, 6, 10), new Rect2i(6 + frameOffset, 10, 18, 10), new Rect2i(24 + frameOffset, 10, 6, 10), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i(), new Rect2i()));
        }

        /// <summary>
        ///  Collision rect for the paddle
        /// </summary>
        public override Rect2i CollideRect
        {
            // Let the ball sink into a paddle a little bit, by 4px
            get { return new Rect2i(Rect.x, Rect.y + 4, Rect.width, Rect.height - 4); }
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            var game = (BrickBustGame)RB.Game;

            if (RB.ButtonDown(RB.BTN_POINTER_A))
            {
                if (mDragging)
                {
                    int dragDelta = RB.PointerPos().x - mPrevPointerPos.x;
                    mPos.x += dragDelta;

                    Rect.x = mPos.x - (Rect.width / 2);

                    int minMargin = game.assets.spriteSheet.grid.cellSize.width / 2;
                    if (Rect.x < minMargin)
                    {
                        Rect.x = minMargin;
                    }
                    else if (Rect.x + Rect.width > RB.DisplaySize.width - minMargin)
                    {
                        Rect.x = RB.DisplaySize.width - minMargin - Rect.width;
                    }
                }

                mPrevPointerPos = RB.PointerPos();
                mDragging = true;
            }
            else
            {
                mDragging = false;
            }

            if (mExtended)
            {
                if (Rect.width < mExtendedRect.width)
                {
                    Rect.x--;
                    Rect.width += 2;
                }
            }
            else
            {
                if (Rect.width > mBaseRect.width)
                {
                    Rect.x++;
                    Rect.width -= 2;
                }
            }

            if (mLaser)
            {
                mLaserOffset -= 0.1f;
                if (mLaserOffset < 0)
                {
                    mLaserOffset = 0;
                }

                if (RB.ButtonPressed(RB.BTN_POINTER_A))
                {
                    Shoot();
                }
            }
            else
            {
                mLaserOffset += 0.1f;
                if (mLaserOffset > 1)
                {
                    mLaserOffset = 1;
                }
            }

            // Rest mPos after collision rect corrections
            mPos = new Vector2i((int)Rect.center.x, (int)Rect.center.y);
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            RB.DrawNineSlice(Rect, mNSFrame[FlashFrame]);

            if (mLaserOffset < 1.0f)
            {
                Vector2i offset = Vector2i.zero;
                offset.x = (int)(16 * mLaserOffset);
                offset.y = (int)(8 * mLaserOffset);

                byte prevAlpha = RB.AlphaGet();

                RB.AlphaSet((byte)((1 - mLaserOffset) * prevAlpha));
                RB.DrawCopy(new Rect2i(120, 10, 7, 10), new Rect2i(Rect.x - 3 - offset.x, Rect.y - 2 - offset.y, 7, 10));
                RB.DrawCopy(new Rect2i(120, 10, 7, 10), new Rect2i(Rect.x + Rect.width - 4 + offset.x, Rect.y - 2 - offset.y, 7, 10));
                RB.AlphaSet(prevAlpha);
            }
        }

        /// <summary>
        /// Handle hit
        /// </summary>
        /// <param name="collider">Who hit us</param>
        /// <param name="pos">Position of impact</param>
        /// <param name="velocity">Velocity at impact</param>
        public override void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
        {
            var game = (BrickBustGame)RB.Game;

            base.Hit(collider, pos, velocity);

            RB.SoundPlay(game.assets.soundHitWall, 1, UnityEngine.Random.Range(0.9f, 1.1f));

            if (mCatch)
            {
                if (collider is Ball)
                {
                    var ball = (Ball)collider;
                    ball.StuckToPaddle = true;
                }
            }

            if (collider is Ball)
            {
                var ball = (Ball)collider;
                ball.SpeedUp();
            }
        }

        /// <summary>
        /// Apply extend power up, cancelling other powerups as needed
        /// </summary>
        public void Extend()
        {
            CancelPowerups();
            mExtended = true;
        }

        /// <summary>
        /// Apply catch power up, cancelling other powerups as needed
        /// </summary>
        public void Catch()
        {
            CancelPowerups();
            mCatch = true;
        }

        /// <summary>
        /// Apply laser power up, cancelling other powerups as needed
        /// </summary>
        public void Laser()
        {
            CancelPowerups();
            mLaser = true;
        }

        /// <summary>
        /// Cancel powerups
        /// </summary>
        public void CancelPowerups()
        {
            mExtended = false;
            mCatch = false;
            mLaser = false;
        }

        /// <summary>
        /// Shoot if we have the laser powerup
        /// </summary>
        private void Shoot()
        {
            if (!mLaser)
            {
                return;
            }

            var game = (BrickBustGame)RB.Game;
            var level = game.Level;

            LaserShot shot;

            if (mLaserTurn == 0)
            {
                shot = new LaserShot(new Vector2i(Rect.x + 1, Rect.y - 8));
                level.AddShot(shot);
            }
            else
            {
                shot = new LaserShot(new Vector2i(Rect.x + Rect.width, Rect.y - 8));
                level.AddShot(shot);
            }

            level.Particles.Impact(new Vector2i(shot.Rect.x + 3, shot.Rect.y + 5), new Vector2(0, -1), Global.COLOR_GREEN_BRICK);

            mLaserTurn = (mLaserTurn == 0) ? 1 : 0;

            RB.SoundPlay(game.assets.soundLaserShot, 1, UnityEngine.Random.Range(0.9f, 1.1f));
        }
    }
}