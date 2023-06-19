namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Thrown item effect
    /// </summary>
    public class EffectThrow : Effect
    {
        private const int FADE_FRAMES = 2;

        private EntityID mTargetEntity;

        private int mAlphaFade = 0;
        private Color32 mProjecticleColor;

        private Vector2i mSourcePos;
        private Vector2i mTargetPos;

        private PackedSpriteID mProjecticleSprite;
        private PackedSpriteID mProjecticleSprite45;

        private AudioAsset mLaunchSound = null;
        private AudioAsset mLandSound = null;

        private bool mRotate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Source entity</param>
        /// <param name="target">Target entity</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="projectileSprite">Projectile sprite facing top</param>
        /// <param name="projecticleColor">Projectile color</param>
        /// <param name="launchSound">Launching sound</param>
        /// <param name="landSound">Landing sound</param>
        public EffectThrow(EntityID source, EntityID target, Vector2i targetPos, PackedSpriteID projectileSprite, Color32 projecticleColor, AudioAsset launchSound, AudioAsset landSound) : base(EffectType.Throw)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            mSourcePos = new Vector2i(source.e.pos.x * game.assets.spriteSheet.grid.cellSize.width, source.e.pos.y * game.assets.spriteSheet.grid.cellSize.height);

            mTargetEntity = target;
            mProjecticleColor = projecticleColor;

            mFramesTotal = 10;
            mFramesRemaining = mFramesTotal;

            mRotate = false;

            mProjecticleSprite = projectileSprite;

            mLaunchSound = launchSound;
            mLandSound = landSound;

            mTargetPos = targetPos;

            if (mLaunchSound != null)
            {
                RB.SoundPlay(mLaunchSound);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Source entity</param>
        /// <param name="target">Target entity</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="projectileSprite">Projectile sprite facing top</param>
        /// <param name="projecticleSprite45">Projectile sprite facing top-right</param>
        /// <param name="projecticleColor">Projectile color</param>
        /// <param name="launchSound">Launching sound</param>
        /// <param name="landSound">Landing sound</param>
        public EffectThrow(EntityID source, EntityID target, Vector2i targetPos, PackedSpriteID projectileSprite, PackedSpriteID projecticleSprite45, Color32 projecticleColor, AudioAsset launchSound, AudioAsset landSound) : base(EffectType.Throw)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            mSourcePos = new Vector2i(source.e.pos.x * game.assets.spriteSheet.grid.cellSize.width, source.e.pos.y * game.assets.spriteSheet.grid.cellSize.height);

            mTargetEntity = target;
            mProjecticleColor = projecticleColor;

            mFramesTotal = 10;
            mFramesRemaining = mFramesTotal;

            mRotate = true;

            mProjecticleSprite = projectileSprite;
            mProjecticleSprite45 = projecticleSprite45;

            mLaunchSound = launchSound;
            mLandSound = landSound;

            mTargetPos = targetPos;

            if (mLaunchSound != null)
            {
                RB.SoundPlay(mLaunchSound);
            }
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectThrow(BinaryReader reader) : base(EffectType.Throw)
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

            if (mFramesRemaining == 0)
            {
                if (mLandSound != null)
                {
                    RB.SoundPlay(mLandSound);
                }
            }
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var target = mTargetEntity.e;

            var targetPos = Vector2i.zero;

            // Set target to target entity if available, otherwise we'll use the passed target position
            if (target != null)
            {
                targetPos = new Vector2i(target.pos.x * game.assets.spriteSheet.grid.cellSize.width, target.pos.y * game.assets.spriteSheet.grid.cellSize.height);
            }
            else
            {
                targetPos = new Vector2i(mTargetPos.x * game.assets.spriteSheet.grid.cellSize.width, mTargetPos.y * game.assets.spriteSheet.grid.cellSize.height);
            }

            var delta = new Vector2(targetPos.x - mSourcePos.x, targetPos.y - mSourcePos.y);
            var shotPos = mSourcePos + (delta * ((mFramesTotal - mFramesRemaining) / (float)mFramesTotal));
            var deltaShotPos = targetPos - shotPos;

            var alpha = mAlphaFade / (float)FADE_FRAMES;
            RB.AlphaSet((byte)(alpha * 255));
            RB.TintColorSet(mProjecticleColor);

            var shotSprite = mProjecticleSprite;
            int shotSpriteFlags = 0;

            if (mRotate)
            {
                // Pick short sprite and sprite flags based on angle to target
                float angle = Mathf.Atan2(deltaShotPos.y, deltaShotPos.x) * 180.0f / Mathf.PI;

                if (angle > -22.5f && angle <= 22.5f)
                {
                    shotSprite = mProjecticleSprite;
                    shotSpriteFlags |= RB.ROT_90_CW;
                }
                else if (angle > 22.5f && angle <= 67.5f)
                {
                    shotSprite = mProjecticleSprite45;
                    shotSpriteFlags |= RB.FLIP_V;
                }
                else if (angle > 67.5f && angle <= 112.5f)
                {
                    shotSprite = mProjecticleSprite;
                    shotSpriteFlags |= RB.FLIP_V;
                }
                else if (angle > 112.5f && angle <= 157.5f)
                {
                    shotSprite = mProjecticleSprite45;
                    shotSpriteFlags |= RB.FLIP_V | RB.FLIP_H;
                }
                else if ((angle > 157.5f && angle <= 180.0f) || angle <= -157.5f)
                {
                    shotSprite = mProjecticleSprite;
                    shotSpriteFlags |= RB.ROT_270_CW;
                }
                else if (angle > -157.5f && angle <= -112.5f)
                {
                    shotSprite = mProjecticleSprite45;
                    shotSpriteFlags |= RB.FLIP_H;
                }
                else if (angle > -112.5f && angle <= -67.5f)
                {
                    shotSprite = mProjecticleSprite;
                    shotSpriteFlags = 0;
                }
                else if (angle > -67.5f && angle <= -22.5f)
                {
                    shotSprite = mProjecticleSprite45;
                    shotSpriteFlags = 0;
                }
            }

            RB.DrawSprite(shotSprite, shotPos, shotSpriteFlags);

            RB.AlphaSet(255);
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mSourcePos);
            writer.Write(mTargetPos);
            writer.Write(mTargetEntity);
            writer.Write(mAlphaFade);
            writer.Write(mProjecticleColor);
            writer.Write(mProjecticleSprite);
            writer.Write(mProjecticleSprite45);
            writer.Write(mRotate);
            writer.Write(SoundSerializer.Serialize(mLaunchSound));
            writer.Write(SoundSerializer.Serialize(mLandSound));
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mSourcePos = reader.ReadVector2i();
            mTargetPos = reader.ReadVector2i();
            mTargetEntity = reader.ReadEntityID();
            mAlphaFade = reader.ReadInt32();
            mProjecticleColor = reader.ReadColor32();
            mProjecticleSprite = reader.ReadPackedSpriteID();
            mProjecticleSprite45 = reader.ReadPackedSpriteID();
            mRotate = reader.ReadBoolean();
            mLaunchSound = SoundSerializer.Deserialize(reader.ReadInt32());
            mLandSound = SoundSerializer.Deserialize(reader.ReadInt32());
        }
    }
}
