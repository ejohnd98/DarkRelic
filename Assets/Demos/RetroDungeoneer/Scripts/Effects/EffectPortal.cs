namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Confusion effect
    /// </summary>
    public class EffectPortal : Effect
    {
        private const float RISE_SPEED = 0.01f;

        // Does not need to be persisted
        private int mShrinkLight = 0;

        private EntityID mPortal;
        private float mRiseTime;
        private List<Vector2> mParticles = new List<Vector2>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portal">Portal</param>
        public EffectPortal(EntityID portal) : base(EffectType.Portal)
        {
            mFramesTotal = 10;
            mFramesRemaining = mFramesTotal;

            mPortal = portal;

            mRiseTime = 0;

            renderOrder = RenderFunctions.RenderOrder.ACTOR_UNDERLAY_EFFECTS;
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectPortal(BinaryReader reader) : base(EffectType.Confuse)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            mRiseTime += RISE_SPEED;

            if (mRiseTime > 1.0f)
            {
                mRiseTime = 1.0f;
            }

            if (mRiseTime < 1.0f)
            {
                mPortal.e.color.a = 0;
            }
            else
            {
                mPortal.e.color.a = 255;
            }
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var cameraPos = RB.CameraGet();

            var portalClipRect = new Rect2i(mPortal.e.pos.x * game.assets.spriteSheet.grid.cellSize.width, mPortal.e.pos.y * game.assets.spriteSheet.grid.cellSize.height, game.assets.spriteSheet.grid.cellSize.width, game.assets.spriteSheet.grid.cellSize.height);
            portalClipRect.x -= cameraPos.x;
            portalClipRect.y -= cameraPos.y;

            float rise = Ease.Interpolate(Ease.Func.QuadOut, 0.0f, 1.0f, mRiseTime);

            var portalPos = new Vector2i(mPortal.e.pos.x * game.assets.spriteSheet.grid.cellSize.width, (mPortal.e.pos.y + (1.0f - rise)) * game.assets.spriteSheet.grid.cellSize.height);
            var portalCenter = new Vector2i(portalPos.x + (game.assets.spriteSheet.grid.cellSize.width / 2), (mPortal.e.pos.y * game.assets.spriteSheet.grid.cellSize.height) + game.assets.spriteSheet.grid.cellSize.height - 2);

            if (Random.Range(0, 10) == 0)
            {
                mShrinkLight = mShrinkLight == 0 ? 1 : 0;
            }

            var outerLightSize = new Vector2i((game.assets.spriteSheet.grid.cellSize.width * 2 * 0.8f) - mShrinkLight, (game.assets.spriteSheet.grid.cellSize.height * 0.8f) - mShrinkLight);
            var innerLightSize = new Vector2i((game.assets.spriteSheet.grid.cellSize.width * 2 * 0.5f) - mShrinkLight, (game.assets.spriteSheet.grid.cellSize.height * 0.5f) - mShrinkLight);

            outerLightSize *= rise;
            innerLightSize *= rise;

            RB.DrawEllipseFill(portalCenter, outerLightSize, new Color32(255, 255, 255, 32));
            RB.DrawEllipseFill(portalCenter, innerLightSize, new Color32(255, 255, 255, 32));

            if (mRiseTime != 1.0f)
            {
                Color32 color = new Color32(0x3c, 0xac, 0xd7, 255);

                RB.TintColorSet(color);

                RB.ClipSet(portalClipRect);
                RB.DrawSprite(S.PORTAL, portalPos);
                RB.ClipReset();
            }
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mPortal);

            writer.Write(mParticles.Count);
            for (int i = 0; i < mParticles.Count; i++)
            {
                writer.Write(mParticles[i]);
            }
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mPortal = reader.ReadEntityID();

            var count = reader.ReadInt32();
            mParticles.Clear();
            for (int i = 0; i < count; i++)
            {
                mParticles.Add(reader.ReadVector2());
            }
        }
    }
}
