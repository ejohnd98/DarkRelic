namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains sprite ids of all sprites being used in the game. This is faster than passing sprite names
    /// by their string names every time.
    /// </summary>
    public static class S
    {
        /// <summary>
        /// Title background
        /// </summary>
        public static PackedSpriteID TITLE_BACKGROUND = RB.PackedSpriteID("title_background");

        /// <summary>
        /// Title text
        /// </summary>
        public static PackedSpriteID TITLE_TEXT = RB.PackedSpriteID("title_text");

        /// <summary>
        /// Fog tile
        /// </summary>
        public static PackedSpriteID FOG = RB.PackedSpriteID("fog");

        /// <summary>
        /// Tile grid
        /// </summary>
        public static PackedSpriteID GRID = RB.PackedSpriteID("grid");

        /// <summary>
        /// Wall brick 1
        /// </summary>
        public static PackedSpriteID WALL_BRICK1 = RB.PackedSpriteID("wall_brick1");

        /// <summary>
        /// Wall brick 2
        /// </summary>
        public static PackedSpriteID WALL_BRICK2 = RB.PackedSpriteID("wall_brick2");

        /// <summary>
        /// Wall brick 3
        /// </summary>
        public static PackedSpriteID WALL_BRICK3 = RB.PackedSpriteID("wall_brick3");

        /// <summary>
        /// Wall fill
        /// </summary>
        public static PackedSpriteID WALL_FILL1 = RB.PackedSpriteID("wall_fill1");

        /// <summary>
        /// Wall fill
        /// </summary>
        public static PackedSpriteID WALL_FILL2 = RB.PackedSpriteID("wall_fill2");

        /// <summary>
        /// Wall fill
        /// </summary>
        public static PackedSpriteID WALL_FILL3 = RB.PackedSpriteID("wall_fill3");

        /// <summary>
        /// Tree 1
        /// </summary>
        public static PackedSpriteID WALL_TREE1 = RB.PackedSpriteID("tree1");

        /// <summary>
        /// Tree 2
        /// </summary>
        public static PackedSpriteID WALL_TREE2 = RB.PackedSpriteID("tree2");

        /// <summary>
        /// Tree 3
        /// </summary>
        public static PackedSpriteID WALL_TREE3 = RB.PackedSpriteID("tree3");

        /// <summary>
        /// Grass 1
        /// </summary>
        public static PackedSpriteID FLOOR_GRASS1 = RB.PackedSpriteID("floor_grass1");

        /// <summary>
        /// Grass 2
        /// </summary>
        public static PackedSpriteID FLOOR_GRASS2 = RB.PackedSpriteID("floor_grass2");

        /// <summary>
        /// Grass 3
        /// </summary>
        public static PackedSpriteID FLOOR_GRASS3 = RB.PackedSpriteID("floor_grass3");

        /// <summary>
        /// Dirt 1
        /// </summary>
        public static PackedSpriteID FLOOR_DIRT1 = RB.PackedSpriteID("floor_dirt1");

        /// <summary>
        /// Dirt 2
        /// </summary>
        public static PackedSpriteID FLOOR_DIRT2 = RB.PackedSpriteID("floor_dirt2");

        /// <summary>
        /// Dirt 3
        /// </summary>
        public static PackedSpriteID FLOOR_DIRT3 = RB.PackedSpriteID("floor_dirt3");

        /// <summary>
        /// Branches
        /// </summary>
        public static PackedSpriteID BRANCHES = RB.PackedSpriteID("branches");

        /// <summary>
        /// Well
        /// </summary>
        public static PackedSpriteID WELL = RB.PackedSpriteID("well");

        /// <summary>
        /// Hero
        /// </summary>
        public static PackedSpriteID HERO = RB.PackedSpriteID("hero");

        /// <summary>
        /// Hero recovering (intro effect)
        /// </summary>
        public static PackedSpriteID HERO_RECOVERING = RB.PackedSpriteID("hero_recovering");

        /// <summary>
        /// Hero falling (intro effect)
        /// </summary>
        public static PackedSpriteID HERO_FALLING = RB.PackedSpriteID("hero_falling");

        /// <summary>
        /// Rat
        /// </summary>
        public static PackedSpriteID RAT = RB.PackedSpriteID("rat");

        /// <summary>
        /// Skeleton
        /// </summary>
        public static PackedSpriteID SKELETON = RB.PackedSpriteID("skeleton");

        /// <summary>
        /// Ghost
        /// </summary>
        public static PackedSpriteID GHOST = RB.PackedSpriteID("ghost");

        /// <summary>
        /// Bat
        /// </summary>
        public static PackedSpriteID BAT = RB.PackedSpriteID("bat");

        /// <summary>
        /// Spider
        /// </summary>
        public static PackedSpriteID SPIDER = RB.PackedSpriteID("spider");

        /// <summary>
        /// Golem
        /// </summary>
        public static PackedSpriteID GOLEM = RB.PackedSpriteID("golem");

        /// <summary>
        /// Thug
        /// </summary>
        public static PackedSpriteID THUG = RB.PackedSpriteID("thug");

        /// <summary>
        /// Slime
        /// </summary>
        public static PackedSpriteID SLIME = RB.PackedSpriteID("slime");

        /// <summary>
        /// Slime small
        /// </summary>
        public static PackedSpriteID SLIME_SMALL = RB.PackedSpriteID("slime_small");

        /// <summary>
        /// Portal
        /// </summary>
        public static PackedSpriteID PORTAL = RB.PackedSpriteID("portal");

        /// <summary>
        /// Potion
        /// </summary>
        public static PackedSpriteID POTION = RB.PackedSpriteID("potion");

        /// <summary>
        /// Scroll
        /// </summary>
        public static PackedSpriteID SCROLL = RB.PackedSpriteID("scroll");

        /// <summary>
        /// Dagger
        /// </summary>
        public static PackedSpriteID DAGGER = RB.PackedSpriteID("dagger");

        /// <summary>
        /// Sword
        /// </summary>
        public static PackedSpriteID SWORD = RB.PackedSpriteID("sword");

        /// <summary>
        /// Sword
        /// </summary>
        public static PackedSpriteID SHORT_SWORD = RB.PackedSpriteID("short_sword");

        /// <summary>
        /// Sword
        /// </summary>
        public static PackedSpriteID SERPENTINE_SWORD = RB.PackedSpriteID("serpentine_sword");

        /// <summary>
        /// Bow
        /// </summary>
        public static PackedSpriteID BOW = RB.PackedSpriteID("bow");

        /// <summary>
        /// Fine Bow
        /// </summary>
        public static PackedSpriteID BOW_FINE = RB.PackedSpriteID("bow_fine");

        /// <summary>
        /// Elven Bow
        /// </summary>
        public static PackedSpriteID BOW_ELVEN = RB.PackedSpriteID("bow_elven");

        /// <summary>
        /// Arrow
        /// </summary>
        public static PackedSpriteID ARROW = RB.PackedSpriteID("arrow");

        /// <summary>
        /// Arrow Shot
        /// </summary>
        public static PackedSpriteID ARROW_SHOT = RB.PackedSpriteID("arrow_shot");

        /// <summary>
        /// Arrow Shot
        /// </summary>
        public static PackedSpriteID ARROW_SHOT_45 = RB.PackedSpriteID("arrow_shot_45");

        /// <summary>
        /// Shield
        /// </summary>
        public static PackedSpriteID SHIELD = RB.PackedSpriteID("shield");

        /// <summary>
        /// Large shield
        /// </summary>
        public static PackedSpriteID LARGE_SHIELD = RB.PackedSpriteID("large_shield");

        /// <summary>
        /// Helmet
        /// </summary>
        public static PackedSpriteID HELMET = RB.PackedSpriteID("helmet");

        /// <summary>
        /// Armor
        /// </summary>
        public static PackedSpriteID ARMOR = RB.PackedSpriteID("armor");

        /// <summary>
        /// Gravestone
        /// </summary>
        public static PackedSpriteID GRAVESTONE = RB.PackedSpriteID("gravestone");

        /// <summary>
        /// Bones
        /// </summary>
        public static PackedSpriteID BONES = RB.PackedSpriteID("bones");

        /// <summary>
        /// Stairs up
        /// </summary>
        public static PackedSpriteID STAIRS_UP = RB.PackedSpriteID("stairs_up");

        /// <summary>
        /// Stairs down
        /// </summary>
        public static PackedSpriteID STAIRS_DOWN = RB.PackedSpriteID("stairs_down");

        /// <summary>
        /// Swoosh frame 1
        /// </summary>
        public static PackedSpriteID SWOOSH_FRAME_1 = RB.PackedSpriteID("swoosh1");

        /// <summary>
        /// Swoosh frame 2
        /// </summary>
        public static PackedSpriteID SWOOSH_FRAME_2 = RB.PackedSpriteID("swoosh2");

        /// <summary>
        /// Swoosh frame 3
        /// </summary>
        public static PackedSpriteID SWOOSH_FRAME_3 = RB.PackedSpriteID("swoosh3");

        /// <summary>
        /// Swoosh frame 4
        /// </summary>
        public static PackedSpriteID SWOOSH_FRAME_4 = RB.PackedSpriteID("swoosh4");

        /// <summary>
        /// Swoosh frame 5
        /// </summary>
        public static PackedSpriteID SWOOSH_FRAME_5 = RB.PackedSpriteID("swoosh5");

        /// <summary>
        /// Swoosh frame 6
        /// </summary>
        public static PackedSpriteID SWOOSH_FRAME_6 = RB.PackedSpriteID("swoosh6");

        /// <summary>
        /// Swoosh frame 7
        /// </summary>
        public static PackedSpriteID SWOOSH_FRAME_7 = RB.PackedSpriteID("swoosh7");

        /// <summary>
        /// Fire
        /// </summary>
        public static PackedSpriteID FIRE = RB.PackedSpriteID("fire");

        /// <summary>
        /// Confusion
        /// </summary>
        public static PackedSpriteID CONFUSION = RB.PackedSpriteID("confusion");

        /// <summary>
        /// Targeting reticle 1
        /// </summary>
        public static PackedSpriteID TARGET_1 = RB.PackedSpriteID("target1");

        /// <summary>
        /// Targeting reticle 2
        /// </summary>
        public static PackedSpriteID TARGET_2 = RB.PackedSpriteID("target2");

        /// <summary>
        /// Player ghost
        /// </summary>
        public static PackedSpriteID PLAYER_GHOST = RB.PackedSpriteID("player_ghost");

        /// <summary>
        /// Aggro icon
        /// </summary>
        public static PackedSpriteID AGGRO = RB.PackedSpriteID("aggro");

        /// <summary>
        /// Paper doll dagger
        /// </summary>
        public static PackedSpriteID PAPERDOLL_DAGGER = RB.PackedSpriteID("pd_dagger");

        /// <summary>
        /// Paper doll sword
        /// </summary>
        public static PackedSpriteID PAPERDOLL_SWORD = RB.PackedSpriteID("pd_sword");

        /// <summary>
        /// Paper doll short sword
        /// </summary>
        public static PackedSpriteID PAPERDOLL_SHORT_SWORD = RB.PackedSpriteID("pd_short_sword");

        /// <summary>
        /// Paper doll shield
        /// </summary>
        public static PackedSpriteID PAPERDOLL_SHIELD = RB.PackedSpriteID("pd_shield");

        /// <summary>
        /// Paper doll large shield
        /// </summary>
        public static PackedSpriteID PAPERDOLL_LARGE_SHIELD = RB.PackedSpriteID("pd_large_shield");

        /// <summary>
        /// Paper doll helmet
        /// </summary>
        public static PackedSpriteID PAPERDOLL_HELMET = RB.PackedSpriteID("pd_helmet");

        /// <summary>
        /// Paper doll armor
        /// </summary>
        public static PackedSpriteID PAPERDOLL_ARMOR = RB.PackedSpriteID("pd_armor");

        /// <summary>
        /// Paper doll bow
        /// </summary>
        public static PackedSpriteID PAPERDOLL_BOW = RB.PackedSpriteID("pd_bow");

        /// <summary>
        /// Web
        /// </summary>
        public static PackedSpriteID WEB = RB.PackedSpriteID("web");

        /// <summary>
        /// Font
        /// </summary>
        public static PackedSpriteID FONT_RETROBLIT_DROPSHADOW = RB.PackedSpriteID("font_retroblit_dropshadow");

        /// <summary>
        /// Swoosh animation
        /// </summary>
        public static List<PackedSpriteID> ANIM_SWOOSH = new List<PackedSpriteID>();

        /// <summary>
        /// Initialize animations
        /// </summary>
        public static void InitializeAnims()
        {
            ANIM_SWOOSH.Add(SWOOSH_FRAME_1);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_2);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_3);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_3);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_3);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_4);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_4);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_5);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_5);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_6);
            ANIM_SWOOSH.Add(SWOOSH_FRAME_7);
        }
    }
}
