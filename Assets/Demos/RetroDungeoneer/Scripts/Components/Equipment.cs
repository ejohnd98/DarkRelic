namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Equipment component
    /// </summary>
    public class Equipment : EntityComponent
    {
        /// <summary>
        /// Array of all equipment slots
        /// </summary>
        public EntityID[] equipment = new EntityID[(int)EquipmentSlot.Count];

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public Equipment(BinaryReader reader) : base(ComponentType.Equipment)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Equipment() : base(ComponentType.Equipment)
        {
        }

        /// <summary>
        /// Get max hitpoints bonus tallied from all equipment
        /// </summary>
        public int maxHpBonus
        {
            get
            {
                int bonus = 0;
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (!equipment[i].isEmpty && equipment[i].e.equippable != null)
                    {
                        bonus += equipment[i].e.equippable.maxHpBonus;
                    }
                }

                return bonus;
            }
        }

        /// <summary>
        /// Get total power bonus tallied from all equipment
        /// </summary>
        public int powerBonus
        {
            get
            {
                int bonus = 0;
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (!equipment[i].isEmpty && equipment[i].e.equippable != null)
                    {
                        bonus += equipment[i].e.equippable.powerBonus;
                    }
                }

                return bonus;
            }
        }

        /// <summary>
        /// Get total defense bonus tallied from all equipment
        /// </summary>
        public int defenseBonus
        {
            get
            {
                int bonus = 0;
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (!equipment[i].isEmpty && equipment[i].e.equippable != null)
                    {
                        bonus += equipment[i].e.equippable.defenseBonus;
                    }
                }

                return bonus;
            }
        }

        /// <summary>
        /// Toggle equipped status of entity, this will generate the proper result to potential add or remove the entity
        /// from the equipment array
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="equippableEntity">Entity to equip</param>
        public void ToggleEquip(ResultSet resultSet, EntityID equippableEntity)
        {
            if (equippableEntity.e == null || equippableEntity.e.equippable == null)
            {
                return;
            }

            var slot = equippableEntity.e.equippable.slot;

            if (equipment[(int)slot] == equippableEntity)
            {
                resultSet.AddDequipped(owner, equippableEntity);
                resultSet.AddMessage(
                    C.FSTR.Set(C.STR_COLOR_NAME).Append(owner.e.name).Append("@- dequipped the ").Append(C.STR_COLOR_NAME).Append(equipment[(int)slot].e.name).Append("@-."));

                equipment[(int)slot] = EntityID.empty;
            }
            else
            {
                if (!equipment[(int)slot].isEmpty)
                {
                    resultSet.AddDequipped(owner, equipment[(int)slot]);
                    resultSet.AddMessage(
                        C.FSTR.Set(C.STR_COLOR_NAME).Append(owner.e.name).Append("@- dequipped the ").Append(C.STR_COLOR_NAME).Append(equipment[(int)slot].e.name).Append("@-."));
                }

                equipment[(int)slot] = equippableEntity;
                resultSet.AddEquipped(owner, equipment[(int)slot]);
                resultSet.AddMessage(
                    C.FSTR.Set(C.STR_COLOR_NAME).Append(owner.e.name).Append("@- equipped the ").Append(C.STR_COLOR_NAME).Append(equippableEntity.e.name).Append("@-."));
            }
        }

        /// <summary>
        /// Check if equipment contains the given item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>True if item is in equipment</returns>
        public bool ContainsItem(EntityID item)
        {
            if (item.e == null || item.e.equippable == null)
            {
                return false;
            }

            if (equipment[(int)item.e.equippable.slot] == item)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            for (int i = 0; i < equipment.Length; i++)
            {
                writer.Write(equipment[i]);
            }
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            for (int i = 0; i < equipment.Length; i++)
            {
                equipment[i] = reader.ReadEntityID();
            }
        }
    }
}
