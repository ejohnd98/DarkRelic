namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Item component
    /// </summary>
    public class Item : EntityComponent
    {
        /// <summary>
        /// Generic attribute
        /// </summary>
        public int int1;

        /// <summary>
        /// Generic attribute
        /// </summary>
        public int int2;

        /// <summary>
        /// Whether currently targeting something with the item
        /// </summary>
        public bool targeting;

        /// <summary>
        /// Message to display when targeting
        /// </summary>
        public FastString targetingMessage;

        /// <summary>
        /// Use function to execute when item is used
        /// </summary>
        public ItemFunction useFunction = ItemFunction.NONE;

        /// <summary>
        /// Player paperdoll sprite for this item
        /// </summary>
        public PackedSpriteID paperDoll;

        /// <summary>
        /// Item type
        /// </summary>
        public ItemType type;

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public Item(BinaryReader reader) : base(ComponentType.Item)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Item() : base(ComponentType.Item)
        {
        }

        /// <summary>
        /// Use the item
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="user">User of the item</param>
        /// <param name="useParams">Usage parameters</param>
        public void Use(ResultSet resultSet, EntityID user, UseParams useParams)
        {
            ItemFunctions.DoItemFunction(resultSet, useFunction, user, owner, useParams);
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(int1);
            writer.Write(int2);
            writer.Write(targeting);
            writer.Write(targetingMessage);
            writer.Write((int)useFunction);
            writer.Write(paperDoll);
            writer.Write((int)type);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            int1 = reader.ReadInt32();
            int2 = reader.ReadInt32();
            targeting = reader.ReadBoolean();
            targetingMessage = reader.ReadFastString();
            useFunction = (ItemFunction)reader.ReadInt32();
            paperDoll = reader.ReadPackedSpriteID();
            type = (ItemType)reader.ReadInt32();
        }

        /// <summary>
        /// Item use parameters, these parameters are generic and are interpreted by each item as needed
        /// </summary>
        public class UseParams
        {
            /// <summary>
            /// A boolean parameter
            /// </summary>
            public bool bool1;

            /// <summary>
            /// First integer parameter
            /// </summary>
            public int int1;

            /// <summary>
            /// Second integer parameter
            /// </summary>
            public int int2;

            /// <summary>
            /// Vector parameter
            /// </summary>
            public Vector2i veci1;

            /// <summary>
            /// First entity parameter
            /// </summary>
            public EntityID entity1;

            /// <summary>
            /// Second entity parameter
            /// </summary>
            public EntityID entity2;

            /// <summary>
            /// Map parameter
            /// </summary>
            public GameMap map;

            /// <summary>
            /// Sound to play when used
            /// </summary>
            public int usageSound;

            /// <summary>
            /// Clear all parameters
            /// </summary>
            public void Clear()
            {
                bool1 = false;
                int1 = 0;
                int2 = 0;
                veci1 = Vector2i.zero;
                entity1 = EntityID.empty;
                entity2 = EntityID.empty;
                map = null;
            }
        }
    }
}
