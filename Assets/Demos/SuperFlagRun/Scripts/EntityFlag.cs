namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Flag entity
    /// </summary>
    public class EntityFlag : Entity
    {
        private int mPlayerNum;
        private float mFrameIndex = 0;
        private float mFlagAnimSpeed = 0.125f;
        private int[] mFlagFrames = new int[] { 0, 1, 2, 3, };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="playerNum">Which player the flag belongs to.</param>
        public EntityFlag(Vector2 pos, int playerNum) : base(pos)
        {
            mPlayerNum = playerNum;
            mColliderInfo.Rect = new Rect2i(2, 3, RB.SpriteSheetGet().grid.cellSize.width - 6, (RB.SpriteSheetGet().grid.cellSize.height * 2) - 4);
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            mFrameIndex += mFlagAnimSpeed;
            if (mFrameIndex >= mFlagFrames.Length)
            {
                mFrameIndex = 0;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            int flip = 0;

            int frame = (int)mFlagFrames[(int)mFrameIndex];

            if (mPlayerNum == RB.PLAYER_ONE)
            {
                RB.DrawSprite(RB.SpriteIndex(frame, 1), new Vector2i((int)Pos.x, (int)Pos.y), flip);
            }
            else
            {
                RB.DrawSprite(RB.SpriteIndex(frame + 5, 1), new Vector2i((int)Pos.x, (int)Pos.y), flip);
            }

            base.Render();
        }
    }
}
