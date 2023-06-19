namespace RetroBlitDemoBrickBust
{
    using UnityEngine;

    /// <summary>
    /// Base powerup class
    /// </summary>
    public class PowerUp
    {
        private bool mDead = false;
        private Vector2 mPos;
        private string mLabel;
        private Color32 mColorTint;
        private Rect2i mRect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="label">Label letter that will appear on the powerup</param>
        /// <param name="colorTint">Palette color swap</param>
        /// <param name="pos">Initial position</param>
        public PowerUp(string label, Color32 colorTint, Vector2i pos)
        {
            mLabel = label;
            mColorTint = colorTint;
            mPos = new Vector2(pos.x, pos.y);
        }

        /// <summary>
        /// Dead flag
        /// </summary>
        public bool Dead
        {
            get { return mDead; }
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            if (mDead)
            {
                return;
            }

            mPos.y += 1.0f;
            mRect = new Rect2i((int)mPos.x, (int)mPos.y, 19, 10);

            if (mPos.y > RB.DisplaySize.height)
            {
                mDead = true;
            }

            var game = (BrickBustGame)RB.Game;
            var level = game.Level;
            var paddleRect = level.Paddle.Rect;
            if (paddleRect.Intersects(mRect))
            {
                level.Particles.Explode(new Rect2i(40, 20, mRect.width, mRect.height), new Vector2i((int)mPos.x, (int)mPos.y), mColorTint);
                Activate();
                mDead = true;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public virtual void Render()
        {
            int frame = (int)((RB.Ticks % 24) / 8);

            // Draw frame
            RB.DrawCopy(new Rect2i(40 + (20 * frame), 20, mRect.width, mRect.height), mRect);

            RB.TintColorSet(mColorTint);

            // Draw color fill
            RB.DrawCopy(new Rect2i(99 + (20 * frame), 20, mRect.width, mRect.height), mRect);

            int scroll = (int)((RB.Ticks % 100) / 10);

            RB.ClipSet(mRect);
            RB.Print(new Rect2i((int)mPos.x, (int)mPos.y - 10 + scroll, mRect.width, mRect.height), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mLabel);
            RB.Print(new Rect2i((int)mPos.x, (int)mPos.y + scroll, mRect.width, mRect.height), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mLabel);
            RB.ClipReset();

            RB.TintColorSet(Color.white);
        }

        /// <summary>
        /// Activate/Pickup the power up
        /// </summary>
        protected virtual void Activate()
        {
            var game = (BrickBustGame)RB.Game;

            RB.SoundPlay(game.assets.soundPowerUp);

            var level = game.Level;
            level.Score += 10;
        }
    }
}