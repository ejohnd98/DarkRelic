namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Fireball effect
    /// </summary>
    public class EffectFlame : Effect
    {
        private const int FADE_FRAMES = 5;

        private int mFrameDelay;
        private Vector2i mPos;
        private int mAlphaFade = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="frameDelay">Frames until effect appears</param>
        public EffectFlame(Vector2i pos, int frameDelay) : base(EffectType.Flame)
        {
            mFrameDelay = frameDelay;
            mFramesTotal = 20;
            mFramesRemaining = mFramesTotal;
            mPos = pos;
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectFlame(BinaryReader reader) : base(EffectType.Flame)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            if (mFrameDelay > 0)
            {
                mFrameDelay--;
            }
            else
            {
                if (mFramesRemaining >= mFramesTotal - FADE_FRAMES)
                {
                    mAlphaFade++;
                }
                else if (mFramesRemaining > FADE_FRAMES)
                {
                    mAlphaFade = FADE_FRAMES;
                }
                else
                {
                    mAlphaFade--;
                }

                mFramesRemaining--;
            }
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (mFrameDelay > 0)
            {
                return;
            }

            int drawFlags = 0;
            drawFlags |= Random.Range(0, 2) == 0 ? 0 : RB.FLIP_H;

            float alpha = mAlphaFade / (float)FADE_FRAMES;

            var renderPos = new Vector2i(mPos.x * game.assets.spriteSheet.grid.cellSize.width, mPos.y * game.assets.spriteSheet.grid.cellSize.height);

            RB.TintColorSet(new Color32(0xf7, 0x9d, 0x48, 255));
            RB.AlphaSet((byte)(alpha * 255));
            RB.DrawSprite(S.FIRE, renderPos, drawFlags);
            RB.AlphaSet(255);
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mFrameDelay);
            writer.Write(mPos);
            writer.Write(mAlphaFade);
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mFrameDelay = reader.ReadInt32();
            mPos = reader.ReadVector2i();
            mAlphaFade = reader.ReadInt32();
        }
    }
}
