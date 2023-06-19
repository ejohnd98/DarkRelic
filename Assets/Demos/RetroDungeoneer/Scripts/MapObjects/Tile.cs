namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// Represents a single tile in the map
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Template for a wall brick, this is a generic empty tile used during gen, it is later switched for a specific non-blocking tile
        /// </summary>
        public static readonly Tile TEMPLATE_EMPTY = new Tile(Type.EMPTY, PackedSpriteID.empty, Color.white, false);

        /// <summary>
        /// Template for a wall brick, this is a generic wall tile used during gen, it is later switched for a specific wall tile
        /// </summary>
        public static readonly Tile TEMPLATE_WALL_BRICK = new Tile(Type.WALL_BRICK, S.WALL_BRICK1, Color.gray, true);

        /// <summary>
        /// Wall brick 1
        /// </summary>
        public static readonly Tile WALL_BRICK1 = new Tile(Type.WALL_BRICK, S.WALL_BRICK1, Color.gray, true);

        /// <summary>
        /// Wall brick 2
        /// </summary>
        public static readonly Tile WALL_BRICK2 = new Tile(Type.WALL_BRICK, S.WALL_BRICK2, Color.gray, true);

        /// <summary>
        /// Wall brick 3
        /// </summary>
        public static readonly Tile WALL_BRICK3 = new Tile(Type.WALL_BRICK, S.WALL_BRICK3, Color.gray, true);

        /// <summary>
        /// Wall brick 1
        /// </summary>
        public static readonly Tile WALL_TREE1 = new Tile(Type.WALL_BRICK, S.WALL_TREE1, new Color32(0x38, 0xd9, 0x73, 255), true);

        /// <summary>
        /// Wall brick 2
        /// </summary>
        public static readonly Tile WALL_TREE2 = new Tile(Type.WALL_BRICK, S.WALL_TREE2, new Color32(0x38, 0xd9, 0x73, 255), true);

        /// <summary>
        /// Wall brick 3
        /// </summary>
        public static readonly Tile WALL_TREE3 = new Tile(Type.WALL_BRICK, S.WALL_TREE3, new Color32(0x38, 0xd9, 0x73, 255), true);

        /// <summary>
        /// Wall fill 1
        /// </summary>
        public static readonly Tile WALL_FILL1 = new Tile(Type.WALL_BRICK, S.WALL_FILL1, Color.gray, true);

        /// <summary>
        /// Wall fill 2
        /// </summary>
        public static readonly Tile WALL_FILL2 = new Tile(Type.WALL_BRICK, S.WALL_FILL2, Color.gray, true);

        /// <summary>
        /// Wall fill 3
        /// </summary>
        public static readonly Tile WALL_FILL3 = new Tile(Type.WALL_BRICK, S.WALL_FILL3, Color.gray, true);

        /// <summary>
        /// Grass floor
        /// </summary>
        public static readonly Tile FLOOR_GRASS1 = new Tile(Type.EMPTY, S.FLOOR_GRASS1, new Color32(0x38, 0xd9, 0x73, 255), false);

        /// <summary>
        /// Grass floor
        /// </summary>
        public static readonly Tile FLOOR_GRASS2 = new Tile(Type.EMPTY, S.FLOOR_GRASS2, new Color32(0x38, 0xd9, 0x73, 255), false);

        /// <summary>
        /// Grass floor
        /// </summary>
        public static readonly Tile FLOOR_GRASS3 = new Tile(Type.EMPTY, S.FLOOR_GRASS3, new Color32(0x38, 0xd9, 0x73, 255), false);

        /// <summary>
        /// Dirt floor 1
        /// </summary>
        public static readonly Tile FLOOR_DIRT1 = new Tile(Type.EMPTY, S.FLOOR_DIRT1, Color.gray, false);

        /// <summary>
        /// Dirt floor 2
        /// </summary>
        public static readonly Tile FLOOR_DIRT2 = new Tile(Type.EMPTY, S.FLOOR_DIRT2, Color.gray, false);

        /// <summary>
        /// Dirt floor 3
        /// </summary>
        public static readonly Tile FLOOR_DIRT3 = new Tile(Type.EMPTY, S.FLOOR_DIRT3, Color.gray, false);

        /// <summary>
        /// Tile type
        /// </summary>
        public Type type;

        /// <summary>
        /// Whether it blocks movement
        /// </summary>
        public bool blocked;

        /// <summary>
        /// Whether is blocks fight (field of view)
        /// </summary>
        public bool blockSight;

        /// <summary>
        /// Sprite id to use when rendering
        /// </summary>
        public PackedSpriteID sprite;

        /// <summary>
        /// Color to use when rendering
        /// </summary>
        public Color32 color;

        /// <summary>
        /// Constructor
        /// </summary>
        public Tile()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="template">Template tile to copy</param>
        public Tile(Tile template)
        {
            type = template.type;
            blocked = template.blocked;
            blockSight = template.blockSight;

            sprite = template.sprite;
            color = template.color;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Tile type</param>
        /// <param name="sprite">Sprite to use when rendering this tile</param>
        /// <param name="color">Color of the tile</param>
        /// <param name="blocked">Whether the tile is blocking</param>
        public Tile(Type type, PackedSpriteID sprite, Color32 color, bool blocked)
        {
            this.type = type;

            this.blocked = blocked;
            this.blockSight = blocked;

            this.sprite = sprite;
            this.color = color;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sprite">Sprite to use when rendering this tile</param>
        /// <param name="color">Color of the tile</param>
        /// <param name="blocked">Whether the tile is blocking</param>
        /// <param name="blockSight">Whether the tile blocks sight (field of view)</param>
        public Tile(PackedSpriteID sprite, Color32 color, bool blocked, bool blockSight)
        {
            this.blocked = blocked;
            this.blockSight = blockSight;

            this.sprite = sprite;
            this.color = color;
        }

        /// <summary>
        /// Tile type
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Empty tile
            /// </summary>
            EMPTY = 0,

            /// <summary>
            /// A wall brick tile
            /// </summary>
            WALL_BRICK,
        }
    }
}
