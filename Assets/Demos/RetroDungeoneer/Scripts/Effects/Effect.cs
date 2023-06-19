namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Visual effect
    /// </summary>
    public abstract class Effect
    {
        /// <summary>
        /// Render layer
        /// </summary>
        public RenderFunctions.RenderOrder renderOrder = RenderFunctions.RenderOrder.ACTOR_OVERLAY_EFFECTS;

        /// <summary>
        /// Remaining frames in the animation
        /// </summary>
        protected int mFramesRemaining = 0;

        /// <summary>
        /// Total frames in the animation
        /// </summary>
        protected int mFramesTotal = 0;

        /// <summary>
        /// Type of effect
        /// </summary>
        protected EffectType mType;

        /// <summary>
        /// Does effect block player movement
        /// </summary>
        protected bool mIsBlocking = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of effect</param>
        public Effect(EffectType type)
        {
            mType = type;
        }

        /// <summary>
        /// Type of effect
        /// </summary>
        public EffectType type
        {
            get
            {
                return mType;
            }
        }

        /// <summary>
        /// Whether the effect blocks player input
        /// </summary>
        public bool blocking
        {
            get
            {
                return mIsBlocking;
            }
        }

        /// <summary>
        /// Check if effect is finished
        /// </summary>
        /// <returns>True if finished</returns>
        public bool Finished()
        {
            return mFramesRemaining <= 0;
        }

        /// <summary>
        /// Update the effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public abstract void Update(ResultSet resultSet);

        /// <summary>
        /// Render the effect
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((int)renderOrder);
            writer.Write(mFramesTotal);
            writer.Write(mFramesRemaining);
            writer.Write(mIsBlocking);
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public virtual void Read(BinaryReader reader)
        {
            renderOrder = (RenderFunctions.RenderOrder)reader.ReadInt32();
            mFramesTotal = reader.ReadInt32();
            mFramesRemaining = reader.ReadInt32();
            mIsBlocking = reader.ReadBoolean();
        }
    }
}
