namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Flag slot that enemy flag can be placed into
    /// </summary>
    public class EntityFlagSlot : Entity
    {
        private int mPlayerNum;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="playerNum">Which player the slot belongs to</param>
        public EntityFlagSlot(Vector2 pos, int playerNum) : base(pos)
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
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            if (mPlayerNum == RB.PLAYER_ONE)
            {
                RB.DrawSprite(RB.SpriteIndex(4, 1), new Vector2i((int)Pos.x + 4, (int)Pos.y + (int)(Mathf.Sin(Time.time * 8) * 3) - 1), 0);
            }
            else
            {
                RB.DrawSprite(RB.SpriteIndex(9, 1), new Vector2i((int)Pos.x + 4, (int)Pos.y + (int)(Mathf.Sin(Time.time * 8) * 3) - 1), 0);
            }
        }
    }
}
