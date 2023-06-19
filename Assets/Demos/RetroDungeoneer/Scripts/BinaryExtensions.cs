namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Extension methods for BinaryReader and BinaryWriter to ease serializing game state
    /// </summary>
    public static class BinaryExtensions
    {
        /// <summary>
        /// Write a Vector2i
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="vec2i">Vector to write</param>
        public static void Write(this BinaryWriter writer, Vector2i vec2i)
        {
            writer.Write(vec2i.x);
            writer.Write(vec2i.y);
        }

        /// <summary>
        /// Read a Vector2i
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Vector</returns>
        public static Vector2i ReadVector2i(this BinaryReader reader)
        {
            var vec = new Vector2i();
            vec.x = reader.ReadInt32();
            vec.y = reader.ReadInt32();

            return vec;
        }

        /// <summary>
        /// Write a Vector2
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="vec2">Vector to write</param>
        public static void Write(this BinaryWriter writer, Vector2 vec2)
        {
            writer.Write(vec2.x);
            writer.Write(vec2.y);
        }

        /// <summary>
        /// Read a Vector2
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Vector</returns>
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            var vec = new Vector2();
            vec.x = reader.ReadSingle();
            vec.y = reader.ReadSingle();

            return vec;
        }

        /// <summary>
        /// Write Color32
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="color">Color</param>
        public static void Write(this BinaryWriter writer, Color32 color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        /// <summary>
        /// Read Color32
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Color</returns>
        public static Color32 ReadColor32(this BinaryReader reader)
        {
            var color = new Color32();
            color.r = reader.ReadByte();
            color.g = reader.ReadByte();
            color.b = reader.ReadByte();
            color.a = reader.ReadByte();

            return color;
        }

        /// <summary>
        /// Write sprite id
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="spriteID">Sprite id</param>
        public static void Write(this BinaryWriter writer, PackedSpriteID spriteID)
        {
            writer.Write(spriteID.id);
        }

        /// <summary>
        /// Read sprite id
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Sprite id</returns>
        public static PackedSpriteID ReadPackedSpriteID(this BinaryReader reader)
        {
            int id = reader.ReadInt32();
            return new PackedSpriteID(id);
        }

        /// <summary>
        /// Write FastString
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="str">String</param>
        public static void Write(this BinaryWriter writer, FastString str)
        {
            writer.Write(str != null ? true : false);

            if (str != null)
            {
                writer.Write(str.Capacity);
                writer.Write(str.Length);
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];
                    writer.Write(c);
                }
            }
        }

        /// <summary>
        /// Read FastString
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>String</returns>
        public static FastString ReadFastString(this BinaryReader reader)
        {
            bool exists = reader.ReadBoolean();
            if (!exists)
            {
                return null;
            }

            int capacity = reader.ReadInt32();
            int length = reader.ReadInt32();

            var str = new FastString((uint)capacity);

            for (int i = 0; i < length; i++)
            {
                str.Append(reader.ReadChar());
            }

            return str;
        }

        /// <summary>
        /// Write a tile
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="tile">Tile</param>
        public static void Write(this BinaryWriter writer, Tile tile)
        {
            writer.Write((int)tile.type);
            writer.Write(tile.blocked);
            writer.Write(tile.blockSight);
            writer.Write(tile.sprite);
            writer.Write(tile.color);
        }

        /// <summary>
        /// Read a tile
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Tile</returns>
        public static Tile ReadTile(this BinaryReader reader)
        {
            var tile = new Tile();
            tile.type = (Tile.Type)reader.ReadInt32();
            tile.blocked = reader.ReadBoolean();
            tile.blockSight = reader.ReadBoolean();
            tile.sprite = reader.ReadPackedSpriteID();
            tile.color = reader.ReadColor32();

            return tile;
        }

        /// <summary>
        /// Write FOV tile
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="tile">Tile</param>
        public static void Write(this BinaryWriter writer, FOVTile tile)
        {
            writer.Write(tile.visible);
            writer.Write(tile.explored);
        }

        /// <summary>
        /// Read FOV tile
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>FOV tile</returns>
        public static FOVTile ReadFOVTile(this BinaryReader reader)
        {
            var tile = new FOVTile();
            tile.visible = reader.ReadBoolean();
            tile.explored = reader.ReadBoolean();

            return tile;
        }

        /// <summary>
        /// Write entity
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="entity">Entity</param>
        public static void Write(this BinaryWriter writer, Entity entity)
        {
            writer.Write(entity != null ? true : false);
            if (entity == null)
            {
                return;
            }

            writer.Write(entity.id);

            writer.Write(entity.pos);
            writer.Write(entity.sprite);
            writer.Write(entity.color);
            writer.Write(entity.blocks);
            writer.Write((int)entity.renderOrder);
            writer.Write(entity.name);

            writer.Write(entity.fighter);
            writer.Write(entity.ai);
            writer.Write(entity.inventory);
            writer.Write(entity.item);
            writer.Write(entity.stairs);
            writer.Write(entity.level);
            writer.Write(entity.equippable);
            writer.Write(entity.equipment);
            writer.Write(entity.groundTrigger);
            writer.Write(entity.moveTrail);
        }

        /// <summary>
        /// Read entity
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Entity</returns>
        public static Entity ReadEntity(this BinaryReader reader)
        {
            bool exists = reader.ReadBoolean();
            if (!exists)
            {
                return null;
            }

            var id = reader.ReadEntityID();

            Entity entity = new Entity(id);

            entity.pos = reader.ReadVector2i();
            entity.sprite = reader.ReadPackedSpriteID();
            entity.color = reader.ReadColor32();
            entity.blocks = reader.ReadBoolean();
            entity.renderOrder = (RenderFunctions.RenderOrder)reader.ReadInt32();
            entity.name = reader.ReadFastString();

            entity.fighter = (Fighter)reader.ReadComponent();
            entity.ai = (Ai)reader.ReadComponent();
            entity.inventory = (Inventory)reader.ReadComponent();
            entity.item = (Item)reader.ReadComponent();
            entity.stairs = (Stairs)reader.ReadComponent();
            entity.level = (Level)reader.ReadComponent();
            entity.equippable = (Equippable)reader.ReadComponent();
            entity.equipment = (Equipment)reader.ReadComponent();
            entity.groundTrigger = (GroundTrigger)reader.ReadComponent();
            entity.moveTrail = (MoveTrail)reader.ReadComponent();

            return entity;
        }

        /// <summary>
        /// Write entity id
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="id">Entity id</param>
        public static void Write(this BinaryWriter writer, EntityID id)
        {
            writer.Write(id.index);
            writer.Write(id.gen);
        }

        /// <summary>
        /// Read entity id
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Entity id</returns>
        public static EntityID ReadEntityID(this BinaryReader reader)
        {
            var index = reader.ReadInt32();
            var gen = reader.ReadInt32();

            return new EntityID(index, gen);
        }

        /// <summary>
        /// Write entity component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        /// <param name="component">Component</param>
        public static void Write(this BinaryWriter writer, EntityComponent component)
        {
            writer.Write(component != null ? true : false);
            if (component == null)
            {
                return;
            }

            writer.Write((int)component.componentType);
            component.Write(writer);
        }

        /// <summary>
        /// Read component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <returns>Entity component</returns>
        public static EntityComponent ReadComponent(this BinaryReader reader)
        {
            bool exists = reader.ReadBoolean();
            if (!exists)
            {
                return null;
            }

            EntityComponent component = null;

            var componentType = (ComponentType)reader.ReadInt32();

            switch (componentType)
            {
                case ComponentType.Fighter:
                    component = new Fighter(reader);
                    break;

                case ComponentType.BasicMonster:
                    component = new BasicMonster(reader);
                    break;

                case ComponentType.ConfusedMonster:
                    component = new ConfusedMonster(reader);
                    break;

                case ComponentType.GolemMonster:
                    component = new GolemMonster(reader);
                    break;

                case ComponentType.SlimeMonster:
                    component = new SlimeMonster(reader);
                    break;

                case ComponentType.Inventory:
                    component = new Inventory(reader);
                    break;

                case ComponentType.Item:
                    component = new Item(reader);
                    break;

                case ComponentType.Stairs:
                    component = new Stairs(reader);
                    break;

                case ComponentType.Level:
                    component = new Level(reader);
                    break;

                case ComponentType.Equipment:
                    component = new Equipment(reader);
                    break;

                case ComponentType.Equippable:
                    component = new Equippable(reader);
                    break;

                case ComponentType.GroundTrigger:
                    component = new GroundTrigger(reader);
                    break;

                case ComponentType.MoveTrail:
                    component = new MoveTrail(reader);
                    break;

                default:
                    throw new System.Exception("Unknown component type " + (int)componentType);
            }

            return component;
        }
    }
}
