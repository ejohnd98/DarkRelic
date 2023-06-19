namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// Collection of global constants and statics
    /// </summary>
    public static class C
    {
        /// <summary>
        /// Minimum room size
        /// </summary>
        public const int ROOM_MIN_SIZE = 6;

        /// <summary>
        /// Maximum room size
        /// </summary>
        public const int ROOM_MAX_SIZE = 10;

        /// <summary>
        /// Maximum amount of rooms on a single level
        /// </summary>
        public const int MAX_ROOMS = 30;

        /// <summary>
        /// Frames until key repeat starts
        /// </summary>
        public const int KEY_REPEAT_SPEED_STAGE1 = 12;

        /// <summary>
        /// Frames between keep repeats once started by stage 1
        /// </summary>
        public const int KEY_REPEAT_SPEED_STAGE2 = 6;

        /// <summary>
        /// Screen width in pixels
        /// </summary>
        public static int SCREEN_WIDTH;

        /// <summary>
        /// Screen height in pixels
        /// </summary>
        public static int SCREEN_HEIGHT;

        /// <summary>
        /// Maximum level width
        /// </summary>
        public static int MAP_WIDTH = 80;

        /// <summary>
        /// Maximum level height
        /// </summary>
        public static int MAP_HEIGHT = 45;

        /// <summary>
        /// Grid layer
        /// </summary>
        public static int LAYER_GRID = 0;

        /// <summary>
        /// Terrain layer
        /// </summary>
        public static int LAYER_TERRAIN = 1;

        /// <summary>
        /// Visibility layer
        /// </summary>
        public static int LAYER_VISIBILITY = 2;

        /// <summary>
        /// Color of menu background
        /// </summary>
        public static Color32 COLOR_MENU_BACKGROUND = new Color32(30, 30, 30, 255);

        /// <summary>
        /// Color of menu header background
        /// </summary>
        public static Color32 COLOR_MENU_HEADER_BACKGROUND = new Color32(50, 50, 50, 255);

        /// <summary>
        /// Shared fast string
        /// </summary>
        public static FastString FSTR = new FastString(8192);

        /// <summary>
        /// Secondary shared fast string
        /// </summary>
        public static FastString FSTR2 = new FastString(8192);

        /// <summary>
        /// Color to use for names of entities
        /// </summary>
        public static string STR_COLOR_DIALOG = "@AAAAAA";

        /// <summary>
        /// Color to use for names of entities
        /// </summary>
        public static string STR_COLOR_NAME = "@FFFF50";

        /// <summary>
        /// Color to use for damage text
        /// </summary>
        public static string STR_COLOR_DAMAGE = "@FF5050";

        /// <summary>
        /// Color to use for dead entity string
        /// </summary>
        public static string STR_COLOR_DEAD = "@AA2020";

        /// <summary>
        /// Color to use for corpses
        /// </summary>
        public static string STR_COLOR_CORPSE = "@C8C8C8";

        /// <summary>
        /// Field of view radius
        /// </summary>
        public static int FOV_RADIUS;

        /// <summary>
        /// Name of saved file folder
        /// </summary>
        public static string SAVE_FOLDER = "rbrl_save";

        /// <summary>
        /// Name of saved game file
        /// </summary>
        public static string SAVE_FILENAME = "saved_game.dat";
    }
}
