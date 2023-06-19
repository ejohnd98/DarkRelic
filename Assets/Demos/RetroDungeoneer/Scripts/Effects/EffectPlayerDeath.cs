namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Confusion effect
    /// </summary>
    public class EffectPlayerDeath : Effect
    {
        private const int FADE_FRAMES = 100;
        private const float FLOAT_HEIGHT = 96;
        private const float FLOAT_SPEED = 0.1f;

        private Vector2 mGhostPos;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Target of confusion effect</param>
        public EffectPlayerDeath(EntityID target) : base(EffectType.PlayerDeath)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            mFramesTotal = (int)(FLOAT_HEIGHT / FLOAT_SPEED);
            mFramesRemaining = mFramesTotal;

            mGhostPos = new Vector2(target.e.pos.x * game.assets.spriteSheet.grid.cellSize.width, target.e.pos.y * game.assets.spriteSheet.grid.cellSize.height);

            renderOrder = RenderFunctions.RenderOrder.TOP_MOST;
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectPlayerDeath(BinaryReader reader) : base(EffectType.PlayerDeath)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            var y = mGhostPos.y;

            y -= FLOAT_SPEED;
            if (y < -FLOAT_HEIGHT)
            {
                y = 0;
            }

            mGhostPos = new Vector2(mGhostPos.x, y);

            mFramesRemaining--;
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            RB.TintColorSet(Color.white);

            float alpha = 1;
            if (mFramesRemaining < FADE_FRAMES)
            {
                alpha = mFramesRemaining / (float)FADE_FRAMES;
            }

            if (mFramesRemaining > mFramesTotal - FADE_FRAMES)
            {
                alpha = (mFramesTotal - mFramesRemaining) / (float)FADE_FRAMES;
            }

            RB.AlphaSet((byte)(alpha * 210));
            RB.DrawSprite(S.PLAYER_GHOST, new Vector2i(mGhostPos.x + (Mathf.Sin((mFramesTotal - mFramesRemaining) / 50.0f) * 5.0f), mGhostPos.y));
            RB.AlphaSet(255);
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mGhostPos);
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mGhostPos = reader.ReadVector2();
        }
    }
}
