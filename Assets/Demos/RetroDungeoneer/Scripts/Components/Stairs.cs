namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Stairs component
    /// </summary>
    public class Stairs : EntityComponent
    {
        /// <summary>
        /// The floor the stairs lead to
        /// </summary>
        public int floor;

        /// <summary>
        /// Type of stair
        /// </summary>
        public StairType type;

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public Stairs(BinaryReader reader) : base(ComponentType.Stairs)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="floor">Floor that the stairs lead to</param>
        public Stairs(int floor) : base(ComponentType.Stairs)
        {
            this.floor = floor;
        }

        /// <summary>
        /// Stair type enum
        /// </summary>
        public enum StairType
        {
            /// <summary>
            /// Normal stairs
            /// </summary>
            STAIRS,

            /// <summary>
            /// A portal
            /// </summary>
            PORTAL,

            /// <summary>
            /// A well
            /// </summary>
            WELL,

            /// <summary>
            /// A closed off well
            /// </summary>
            WELL_CLOSED
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(floor);
            writer.Write((int)type);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            floor = reader.ReadInt32();
            type = (StairType)reader.ReadInt32();
        }
    }
}
