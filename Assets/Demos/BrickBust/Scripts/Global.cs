namespace RetroBlitDemoBrickBust
{
    using UnityEngine;

    /// <summary>
    /// Handle class that encapsulates constant values shared throughout the game
    /// </summary>
    public class Global
    {
        /// <summary>
        /// Major version of the game
        /// </summary>
        public const int MAJOR_VER = 1;

        /// <summary>
        /// Minor version of the game
        /// </summary>
        public const int MINOR_VER = 0;

        /// <summary>
        /// Revision version of the game
        /// </summary>
        public const int REV_VER = 0;

        /// <summary>
        /// Width of a brick
        /// </summary>
        public const int BRICK_WIDTH = 20;

        /// <summary>
        /// Height of a brick
        /// </summary>
        public const int BRICK_HEIGHT = 10;

        /// <summary>
        /// Width of the ball
        /// </summary>
        public const int BALL_WIDTH = 6;

        /// <summary>
        /// Height of the ball
        /// </summary>
        public const int BALL_HEIGHT = 6;

        /// <summary>
        /// Color of gold brick
        /// </summary>
        public static Color32 COLOR_GOLD_BRICK = new Color32(255, 210, 60, 255);

        /// <summary>
        /// Color of blue brick
        /// </summary>
        public static Color32 COLOR_BLUE_BRICK = new Color32(123, 226, 223, 255);

        /// <summary>
        /// Color of green brick
        /// </summary>
        public static Color32 COLOR_GREEN_BRICK = new Color32(199, 230, 92, 255);

        /// <summary>
        /// Color of brown brick
        /// </summary>
        public static Color32 COLOR_BROWN_BRICK = new Color32(250, 213, 156, 255);

        /// <summary>
        /// Color of pink brick
        /// </summary>
        public static Color32 COLOR_PINK_BRICK = new Color32(255, 209, 190, 255);

        /// <summary>
        /// Color of black brick
        /// </summary>
        public static Color32 COLOR_BLACK_BRICK = new Color32(193, 193, 193, 255);

        /// <summary>
        /// Verb used in help text, "CLICK" for desktop, "TAP" for mobile/touchscreen
        /// </summary>
        public static string ACTION_VERB = "CLICK";
    }
}