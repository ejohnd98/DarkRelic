namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Confusion effect
    /// </summary>
    public class EffectAggro : Effect
    {
        private const int FADE_FRAMES = 5;
        private const float FLOAT_HEIGHT = 16;
        private const float FLOAT_SPEED = 0.5f;

        private EntityID mTarget;
        private float mFloatPos;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Target of confusion effect</param>
        public EffectAggro(EntityID target) : base(EffectType.Aggro)
        {
            mFramesTotal = (int)(FLOAT_HEIGHT / FLOAT_SPEED);
            mFramesRemaining = mFramesTotal;
            mTarget = target;
            mFloatPos = 0;
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectAggro(BinaryReader reader) : base(EffectType.Aggro)
        {
            Read(reader);
        }

        /// <summary>
        /// Update the effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            var y = mFloatPos;

            y -= FLOAT_SPEED;
            if (y < -FLOAT_HEIGHT)
            {
                y = 0;
            }

            mFloatPos = y;

            mFramesRemaining--;
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var e = mTarget.e;
            if (e == null)
            {
                return;
            }

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

            RB.AlphaSet((byte)(alpha * 255));
            RB.TintColorSet(new Color32(0xe6, 0x48, 0x2e, 255));
            RB.DrawSprite(S.AGGRO, new Vector2i(e.pos.x * game.assets.spriteSheet.grid.cellSize.width, (e.pos.y * game.assets.spriteSheet.grid.cellSize.height) + mFloatPos));
            RB.AlphaSet(255);
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mTarget);
            writer.Write(mFloatPos);
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mTarget = reader.ReadEntityID();
            mFloatPos = reader.ReadSingle();
        }
    }
}
