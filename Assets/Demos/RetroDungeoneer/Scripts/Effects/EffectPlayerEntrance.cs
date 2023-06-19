namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Confusion effect
    /// </summary>
    public class EffectPlayerEntrance : Effect
    {
        private const float FLOAT_SPEED = 0.01f;

        // No need to persist this
        private int mShrinkLight = 0;

        private int mStage;
        private EntityID mPlayer;
        private Vector2 mStartPos;
        private Vector2 mLandingPos;
        private float mFallProgress;
        private bool mLanded;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player">Player</param>
        public EffectPlayerEntrance(EntityID player) : base(EffectType.PlayerEntrance)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            mFramesTotal = 1;
            mFramesRemaining = 1;

            mStage = 0;
            mPlayer = player;
            mLandingPos = new Vector2(player.e.pos.x * game.assets.spriteSheet.grid.cellSize.width, player.e.pos.y * game.assets.spriteSheet.grid.cellSize.height);
            mStartPos = new Vector2(mLandingPos.x, mLandingPos.y - (RB.DisplaySize.height / 1.75f));
            mFallProgress = 0.0f;
            mLanded = false;

            // Animation blocks player input until it's done
            mIsBlocking = true;

            renderOrder = RenderFunctions.RenderOrder.TOP_MOST;
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectPlayerEntrance(BinaryReader reader) : base(EffectType.PlayerEntrance)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (mFallProgress > 0.3f && mStage == 0)
            {
                RB.SoundPlay(game.assets.soundPlayerFallYell);
                mStage = 1;
            }

            if (mFallProgress >= 0.9f && mStage == 1)
            {
                RB.SoundPlay(game.assets.soundMonsterAttack);
                mStage = 2;
            }

            if (mFallProgress >= 1.0f && mStage == 2)
            {
                mStage = 3;
            }

            if (mFallProgress > 2.0f && mStage == 3)
            {
                mIsBlocking = false;
                mStage = 4;
            }

            mFallProgress += FLOAT_SPEED;

            if (mStage < 4)
            {
                // Make the player entity invisible while he's still falling, we'll custom render the player falling
                mPlayer.e.color.a = 0;
            }
            else
            {
                mPlayer.e.color.a = 255;
            }

            if (!mLanded && mStage >= 4)
            {
                mLanded = true;
                resultSet.AddFellDownWell();
            }
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            RB.TintColorSet(Color.white);

            var landingCenter = new Vector2i(mLandingPos.x + (game.assets.spriteSheet.grid.cellSize.width / 2), mLandingPos.y + game.assets.spriteSheet.grid.cellSize.height);

            if (Random.Range(0, 10) == 0)
            {
                mShrinkLight = mShrinkLight == 0 ? 1 : 0;
            }

            RB.DrawEllipseFill(landingCenter, new Vector2i((game.assets.spriteSheet.grid.cellSize.width * 2 * 0.8f) - mShrinkLight, (game.assets.spriteSheet.grid.cellSize.height * 0.8f) - mShrinkLight), new Color32(255, 255, 255, 32));
            RB.DrawEllipseFill(landingCenter, new Vector2i((game.assets.spriteSheet.grid.cellSize.width * 2 * 0.5f) - mShrinkLight, (game.assets.spriteSheet.grid.cellSize.height * 0.5f) - mShrinkLight), new Color32(255, 255, 255, 32));

            var renderPos = Ease.Interpolate(Ease.Func.QuarticIn, mStartPos, mLandingPos, mFallProgress > 1.0f ? 1.0f : mFallProgress);

            if (mStage < 4)
            {
                RB.DrawSprite(mStage == 3 ? S.HERO_RECOVERING : S.HERO_FALLING, renderPos);
            }
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mPlayer);
            writer.Write(mStartPos);
            writer.Write(mLandingPos);
            writer.Write(mFallProgress);
            writer.Write((byte)mStage);
            writer.Write(mLanded);
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mPlayer = reader.ReadEntityID();
            mStartPos = reader.ReadVector2();
            mLandingPos = reader.ReadVector2();
            mFallProgress = reader.ReadSingle();
            mStage = reader.ReadByte();
            mLanded = reader.ReadBoolean();
        }
    }
}
