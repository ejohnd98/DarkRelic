namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Inventory component
    /// </summary>
    public class Inventory : EntityComponent
    {
        /// <summary>
        /// Items in the inventory
        /// </summary>
        public EntityID[] items;

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public Inventory(BinaryReader reader) : base(ComponentType.Inventory)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">Maximum capacity of the inventory</param>
        public Inventory(int capacity) : base(ComponentType.Inventory)
        {
            items = new EntityID[capacity];
            for (int i = 0; i < capacity; i++)
            {
                items[i] = EntityID.empty;
            }
        }

        /// <summary>
        /// Add item to the inventory
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="item">Item to add</param>
        public void AddItem(ResultSet resultSet, EntityID item)
        {
            int emptySlot = -1;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].isEmpty)
                {
                    emptySlot = i;
                    break;
                }
            }

            if (emptySlot == -1)
            {
                if (resultSet != null)
                {
                    resultSet.AddMessage(C.FSTR.Set("You cannot carry any more, your inventory is full!"));
                }
            }
            else
            {
                if (resultSet != null)
                {
                    resultSet.AddMessage(C.FSTR.Set("You pickup the ").Append(C.STR_COLOR_NAME).Append(item.e.name).Append("@-!"));
                    resultSet.AddItemAdded(item);
                }

                items[emptySlot] = item;
            }
        }

        /// <summary>
        /// Remove item from the inventory
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="item">Item to remove</param>
        public void RemoveItem(ResultSet resultSet, EntityID item)
        {
            DeleteItemFromList(item);
        }

        /// <summary>
        /// Drop item on the ground from inventory
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="item">Item to drop</param>
        public void DropItem(ResultSet resultSet, EntityID item)
        {
            DeleteItemFromList(item);

            item.e.pos = owner.e.pos;

            // Dequip item first if its currently equipped
            if (owner.e.equipment != null && item.e.equippable != null)
            {
                var slot = item.e.equippable.slot;
                if (owner.e.equipment.equipment[(int)slot] == item)
                {
                    owner.e.equipment.ToggleEquip(resultSet, item);
                }
            }

            resultSet.AddItemDropped(item);
            resultSet.AddMessage(C.FSTR.Set("You dropped the ").Append(C.STR_COLOR_NAME).Append(item.e.name).Append("@-."));
        }

        /// <summary>
        /// Use item in the inventory, if its useable
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="item">Item to use</param>
        /// <param name="useParams">Usage parameters</param>
        public void UseItem(ResultSet resultSet, EntityID item, Item.UseParams useParams)
        {
            var itemComp = item.e.item;

            // Not an item
            if (itemComp == null)
            {
                return;
            }

            var targetingReady = useParams.bool1;

            if (itemComp.targeting && !targetingReady && item.e.equippable == null)
            {
                resultSet.AddTargeting(item);
            }
            else
            {
                if (itemComp.useFunction == ItemFunction.NONE)
                {
                    if (item.e.equippable != null)
                    {
                        resultSet.AddEquip(owner, item);
                    }
                    else
                    {
                        resultSet.AddMessage(C.FSTR.Set("The ").Append(C.STR_COLOR_NAME).Append(item.e.name).Append("@- cannot be used"));
                    }
                }
                else
                {
                    itemComp.Use(resultSet, owner, useParams);
                }
            }
        }

        /// <summary>
        /// Get an arrow in the inventory
        /// </summary>
        /// <returns>Arrow, if any</returns>
        public EntityID GetArrow()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (!items[i].isEmpty && items[i].e.item.type == ItemType.Arrow)
                {
                    return items[i];
                }
            }

            return EntityID.empty;
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                writer.Write(items[i]);
            }
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            items = new EntityID[length];

            for (int i = 0; i < length; i++)
            {
                items[i] = reader.ReadEntityID();
            }
        }

        private void DeleteItemFromList(EntityID item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == item)
                {
                    // Shift following items down
                    int j;
                    for (j = i; j < items.Length - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }

                    items[j] = EntityID.empty;

                    break;
                }
            }
        }
    }
}
