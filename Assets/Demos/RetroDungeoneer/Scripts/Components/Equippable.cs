namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Equippable component
    /// </summary>
    public class Equippable : EntityComponent
    {
        /// <summary>
        /// Slot that the item can be equipped in
        /// </summary>
        public EquipmentSlot slot;

        /// <summary>
        /// Power bonus granted by the item
        /// </summary>
        public int powerBonus;

        /// <summary>
        /// Defense bonus granted by the item
        /// </summary>
        public int defenseBonus;

        /// <summary>
        /// Max HP bonus granted by the item
        /// </summary>
        public int maxHpBonus;

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public Equippable(BinaryReader reader) : base(ComponentType.Equippable)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Equippable() : base(ComponentType.Equippable)
        {
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write((int)slot);
            writer.Write(powerBonus);
            writer.Write(defenseBonus);
            writer.Write(maxHpBonus);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            slot = (EquipmentSlot)reader.ReadInt32();
            powerBonus = reader.ReadInt32();
            defenseBonus = reader.ReadInt32();
            maxHpBonus = reader.ReadInt32();
        }
    }
}
