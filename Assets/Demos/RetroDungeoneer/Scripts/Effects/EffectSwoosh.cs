namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Attack swoosh effect
    /// </summary>
    public class EffectSwoosh : Effect
    {
        private EntityID mTargetEntity;
        private int mDrawFlags;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Target entity to place to swoosh on</param>
        public EffectSwoosh(EntityID target) : base(EffectType.Swoosh)
        {
            mFramesTotal = S.ANIM_SWOOSH.Count;
            mFramesRemaining = mFramesTotal;

            mTargetEntity = target;

            mDrawFlags = 0;
            mDrawFlags |= Random.Range(0, 2) == 0 ? 0 : RB.FLIP_H;
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectSwoosh(BinaryReader reader) : base(EffectType.Swoosh)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            mFramesRemaining--;
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (mTargetEntity.isEmpty)
            {
                return;
            }

            var entityPos = mTargetEntity.e.pos;
            var renderPos = new Vector2i(entityPos.x * game.assets.spriteSheet.grid.cellSize.width, entityPos.y * game.assets.spriteSheet.grid.cellSize.height);

            RB.TintColorSet(Color.white);

            int frame = ((mFramesTotal - mFramesRemaining) / 1) - 1;
            RB.DrawSprite(S.ANIM_SWOOSH[frame], renderPos, mDrawFlags);
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mTargetEntity);
            writer.Write(mDrawFlags);
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mTargetEntity = reader.ReadEntityID();
            mDrawFlags = reader.ReadInt32();
        }
    }
}
