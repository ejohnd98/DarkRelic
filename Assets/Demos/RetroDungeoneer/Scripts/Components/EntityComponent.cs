namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Abstract class defining a generic component
    /// </summary>
    public abstract class EntityComponent
    {
        /// <summary>
        /// The owning entity of the component
        /// </summary>
        public EntityID owner;

        private ComponentType mComponentType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of the component</param>
        public EntityComponent(ComponentType type)
        {
            mComponentType = type;
        }

        /// <summary>
        /// Get the type of the component
        /// </summary>
        public ComponentType componentType
        {
            get
            {
                return mComponentType;
            }
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public abstract void Write(BinaryWriter writer);

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public abstract void Read(BinaryReader reader);
    }
}
