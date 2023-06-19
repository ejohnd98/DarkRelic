namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Effect manager, responsible for rendering and updating effects
    /// </summary>
    public class EffectManager
    {
        private static EffectManager mInstance;

        private List<Effect> mEffects = new List<Effect>();

        /// <summary>
        /// Get the EffectManager instance
        /// </summary>
        public static EffectManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new EffectManager();
                }

                return mInstance;
            }
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public void Update(ResultSet resultSet)
        {
            for (int i = 0; i < mEffects.Count; i++)
            {
                mEffects[i].Update(resultSet);
            }

            // Remove any finished effects
            for (int i = mEffects.Count - 1; i >= 0; i--)
            {
                if (mEffects[i].Finished())
                {
                    mEffects.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Render the effects
        /// </summary>
        /// <param name="renderOrder">Render order</param>
        public void Render(RenderFunctions.RenderOrder renderOrder)
        {
            for (int i = 0; i < mEffects.Count; i++)
            {
                if (mEffects[i].renderOrder == renderOrder)
                {
                    mEffects[i].Render();
                }
            }
        }

        /// <summary>
        /// Add a new effect
        /// </summary>
        /// <param name="effect">Effect to add</param>
        public void AddEffect(Effect effect)
        {
            mEffects.Add(effect);
        }

        /// <summary>
        /// Check if there is a player block effect in progress
        /// </summary>
        /// <returns>True if there is a blocking effect</returns>
        public bool BlockingEffectInProgress()
        {
            for (int i = 0; i < mEffects.Count; i++)
            {
                if (mEffects[i].blocking)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clear all effects
        /// </summary>
        public void Clear()
        {
            mEffects.Clear();
        }

        /// <summary>
        /// Serialize the effect manager and all the effects
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(mEffects.Count);
            for (int i = 0; i < mEffects.Count; i++)
            {
                writer.Write((ushort)mEffects[i].type);
                mEffects[i].Write(writer);
            }
        }

        /// <summary>
        /// De-serialize the effect manager and all its effects
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public void Read(BinaryReader reader)
        {
            mEffects.Clear();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                EffectType type = (EffectType)reader.ReadUInt16();
                switch (type)
                {
                    case EffectType.Aggro:
                        mEffects.Add(new EffectAggro(reader));
                        break;

                    case EffectType.Confuse:
                        mEffects.Add(new EffectConfuse(reader));
                        break;

                    case EffectType.Flame:
                        mEffects.Add(new EffectFlame(reader));
                        break;

                    case EffectType.Lightning:
                        mEffects.Add(new EffectLightning(reader));
                        break;

                    case EffectType.PlayerDeath:
                        mEffects.Add(new EffectPlayerDeath(reader));
                        break;

                    case EffectType.Swoosh:
                        mEffects.Add(new EffectSwoosh(reader));
                        break;

                    case EffectType.PlayerEntrance:
                        mEffects.Add(new EffectPlayerEntrance(reader));
                        break;

                    case EffectType.Portal:
                        mEffects.Add(new EffectPortal(reader));
                        break;

                    case EffectType.Teleport:
                        mEffects.Add(new EffectTeleport(reader));
                        break;

                    case EffectType.Throw:
                        mEffects.Add(new EffectThrow(reader));
                        break;

                    default:
                        throw new System.Exception("Unknown effect type " + type);
                }
            }
        }
    }
}
