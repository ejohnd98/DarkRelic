namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Confusion effect
    /// </summary>
    public class EffectConfuse : Effect
    {
        private const int FADE_FRAMES = 5;
        private const float FLOAT_HEIGHT = 16;
        private const float FLOAT_SPEED = 0.5f;

        private EntityID mTarget;
        private List<Vector2> mMarks = new List<Vector2>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Target of confusion effect</param>
        public EffectConfuse(EntityID target) : base(EffectType.Confuse)
        {
            mFramesTotal = 10;
            mFramesRemaining = mFramesTotal;

            mTarget = target;

            mMarks.Add(new Vector2(-6, -(FLOAT_HEIGHT / 2)));
            mMarks.Add(new Vector2(0, 0));
            mMarks.Add(new Vector2(6, -(FLOAT_HEIGHT / 4)));
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectConfuse(BinaryReader reader) : base(EffectType.Confuse)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            var target = mTarget.e;

            if (target == null)
            {
                mFramesRemaining = 0;
                return;
            }

            // Count down effect if entity no longer confused
            if (target.ai == null || target.ai.componentType != ComponentType.ConfusedMonster)
            {
                mFramesRemaining--;
            }

            for (int i = 0; i < mMarks.Count; i++)
            {
                var y = mMarks[i].y;

                y -= FLOAT_SPEED;
                if (y < -FLOAT_HEIGHT)
                {
                    y = 0;
                }

                mMarks[i] = new Vector2(mMarks[i].x, y);
            }
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var target = mTarget.e;

            if (target == null)
            {
                return;
            }

            var map = ((RetroDungeoneerGame)RB.Game).map;

            if (!map.IsInFOV(target.pos))
            {
                return;
            }

            Color32 color = new Color32(0x38, 0xd9, 0x73, 255);

            var targetPos = new Vector2i(target.pos.x * game.assets.spriteSheet.grid.cellSize.width, target.pos.y * game.assets.spriteSheet.grid.cellSize.height);

            var globalAlpha = mFramesRemaining <= FADE_FRAMES ? (mFramesRemaining / (float)FADE_FRAMES) : 1.0f;

            RB.TintColorSet(color);

            for (int i = 0; i < mMarks.Count; i++)
            {
                float alpha = 1;
                int y = (int)mMarks[i].y;
                if (-y < FADE_FRAMES)
                {
                    alpha = (-y) / (float)FADE_FRAMES;
                }

                if (-y > FLOAT_HEIGHT - FADE_FRAMES)
                {
                    alpha = (FLOAT_HEIGHT + y) / (float)FADE_FRAMES;
                }

                RB.AlphaSet((byte)(alpha * 255 * globalAlpha));
                RB.DrawSprite(S.CONFUSION, targetPos + mMarks[i]);
                RB.AlphaSet(255);
            }
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mTarget);

            writer.Write(mMarks.Count);
            for (int i = 0; i < mMarks.Count; i++)
            {
                writer.Write(mMarks[i]);
            }
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mTarget = reader.ReadEntityID();

            var count = reader.ReadInt32();
            mMarks.Clear();
            for (int i = 0; i < count; i++)
            {
                mMarks.Add(reader.ReadVector2());
            }
        }
    }
}
