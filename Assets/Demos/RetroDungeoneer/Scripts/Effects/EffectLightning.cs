namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Lightning effect
    /// </summary>
    public class EffectLightning : Effect
    {
        private const int FADE_FRAMES = 5;

        private EntityID mSourceEntity;
        private EntityID mTargetEntity;

        private int mAlphaFade = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Caster</param>
        /// <param name="target">Target</param>
        public EffectLightning(EntityID source, EntityID target) : base(EffectType.Throw)
        {
            mSourceEntity = source;
            mTargetEntity = target;

            mFramesTotal = 30;
            mFramesRemaining = mFramesTotal;
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectLightning(BinaryReader reader) : base(EffectType.Throw)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
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

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var source = mSourceEntity.e;
            var target = mTargetEntity.e;

            if (source == null || target == null)
            {
                return;
            }

            var sourcePos = new Vector2i(source.pos.x * game.assets.spriteSheet.grid.cellSize.width, source.pos.y * game.assets.spriteSheet.grid.cellSize.height);
            sourcePos += new Vector2i(game.assets.spriteSheet.grid.cellSize.width / 2, game.assets.spriteSheet.grid.cellSize.height / 2);

            var targetPos = new Vector2i(target.pos.x * game.assets.spriteSheet.grid.cellSize.width, target.pos.y * game.assets.spriteSheet.grid.cellSize.height);
            targetPos += new Vector2i(game.assets.spriteSheet.grid.cellSize.width / 2, game.assets.spriteSheet.grid.cellSize.height / 2);

            var delta = new Vector2(targetPos.x - sourcePos.x, targetPos.y - sourcePos.y);
            var dist = delta.magnitude;
            var dir = delta;
            dir.Normalize();

            var perp = new Vector2(dir.y, -dir.x);

            var segmentLen = 4.0f;

            Color32[] colors = new Color32[] { new Color32(0x3c, 0xac, 0xd7, 255), new Color32(0x3c, 0xac, 0xd7, 255), Color.white, Color.white };

            var alpha = mAlphaFade / (float)FADE_FRAMES;
            RB.AlphaSet((byte)(alpha * 255));
            RB.TintColorSet(Color.white);

            for (int i = 0; i < 4; i++)
            {
                var segmentStart = sourcePos.ToVector2();
                var distleft = dist;

                while (distleft > 12)
                {
                    var len = segmentLen;

                    var segmentEnd = segmentStart + (dir * len) + (perp * Random.Range(-1.5f, 1.5f));
                    distleft -= len;

                    RB.DrawLine(segmentStart, segmentEnd, colors[i]);

                    segmentStart = segmentEnd;
                }

                RB.DrawLine(segmentStart, targetPos, colors[i]);
            }

            RB.AlphaSet(255);
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mSourceEntity);
            writer.Write(mTargetEntity);
            writer.Write(mAlphaFade);
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mSourceEntity = reader.ReadEntityID();
            mTargetEntity = reader.ReadEntityID();
            mAlphaFade = reader.ReadInt32();
        }
    }
}
