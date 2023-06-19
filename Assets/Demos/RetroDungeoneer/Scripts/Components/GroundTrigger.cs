namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Item component
    /// </summary>
    public class GroundTrigger : EntityComponent
    {
        /// <summary>
        /// Ground trigger type
        /// </summary>
        public GroundTriggerType type;

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public GroundTrigger(BinaryReader reader) : base(ComponentType.GroundTrigger)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Ground trigger type</param>
        public GroundTrigger(GroundTriggerType type) : base(ComponentType.GroundTrigger)
        {
            this.type = type;
        }

        /// <summary>
        /// Ground trigger enum
        /// </summary>
        public enum GroundTriggerType
        {
            /// <summary>
            /// Game winning trigger
            /// </summary>
            GameWon,

            /// <summary>
            /// Web trap
            /// </summary>
            Web,
        }

        /// <summary>
        /// Trigger
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public void Trigger(ResultSet resultSet)
        {
            switch (type)
            {
                case GroundTriggerType.GameWon:
                    {
                        resultSet.AddGameWon();
                    }

                    break;
            }
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write((int)type);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            type = (GroundTriggerType)reader.ReadInt32();
        }
    }
}
