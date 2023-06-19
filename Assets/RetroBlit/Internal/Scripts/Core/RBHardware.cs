namespace RetroBlitInternal
{
    using UnityEngine;

    /// <summary>
    /// Hardware subsystem
    /// </summary>
    public sealed class RBHardware
    {
        /// <summary>
        /// Max display dimension
        /// </summary>
        public const int HW_MAX_DISPLAY_DIMENSION = 16384;

        /// <summary>
        /// System texture width
        /// </summary>
        public const int HW_SYSTEM_TEXTURE_WIDTH = 512;

        /// <summary>
        /// System texture height
        /// </summary>
        public const int HW_SYSTEM_TEXTURE_HEIGHT = 512;

        /// <summary>
        /// Maximum supported players. This is tied into Input subsystem
        /// </summary>
        public const int HW_MAX_PLAYERS = 4;

        /// <summary>
        /// Maximum sound channels, which represent maximum simultaneous sounds
        /// </summary>
        public const int HW_MAX_SOUND_CHANNELS = 16;

        /// <summary>
        /// Maximum map layers
        /// </summary>
        public const int HW_MAX_MAP_LAYERS = 64;

        /// <summary>
        /// Maximum total map tiles
        /// </summary>
        public const int HW_MAX_MAP_TILES = 100000000;

        /// <summary>
        /// Maximum points in a polygon
        /// </summary>
        public const int HW_MAX_POLY_POINTS = 16384;

        private int mFPS;
        private float mSecondsPerUpdate;

        private Vector2i mDisplaySize;
        private Vector2i mMapSize;
        private int mMapLayers;
        private Vector2i mMapChunkSize;

        private RB.PixelStyle mPixelStyle;

        /// <summary>
        /// Get display size
        /// </summary>
        public Vector2i DisplaySize
        {
            get { return mDisplaySize; }
            set { mDisplaySize = value; }
        }

        /// <summary>
        /// Get map size
        /// </summary>
        public Vector2i MapSize
        {
            get { return mMapSize; }
        }

        /// <summary>
        /// Get amount of map layers
        /// </summary>
        public int MapLayers
        {
            get { return mMapLayers; }
        }

        /// <summary>
        /// Get size of a single map chunk
        /// </summary>
        public Vector2i MapChunkSize
        {
            get { return mMapChunkSize; }
        }

        /// <summary>
        /// Get the target FPS
        /// </summary>
        public int FPS
        {
            get { return mFPS; }
        }

        /// <summary>
        /// Get the interval between updates in milliseconds
        /// </summary>
        public float UpdateInterval
        {
            get { return mSecondsPerUpdate; }
        }

        /// <summary>
        /// Get the pixel style
        /// </summary>
        public RB.PixelStyle PixelStyle
        {
            get { return mPixelStyle; }
            set { mPixelStyle = value; }
        }

        /// <summary>
        /// Initialize the hardware subsystem
        /// </summary>
        /// <param name="hardwareSettings">Hardware settings</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RB.HardwareSettings hardwareSettings)
        {
            if (!ValidateHWSettings(hardwareSettings))
            {
                return false;
            }

            mDisplaySize = hardwareSettings.DisplaySize;

            mMapSize = hardwareSettings.MapSize;
            mMapLayers = hardwareSettings.MapLayers;
            mMapChunkSize = hardwareSettings.MapChunkSize;

            mFPS = hardwareSettings.FPS;
            mSecondsPerUpdate = 1.0f / mFPS;

            mPixelStyle = hardwareSettings.PixelStyle;

            return true;
        }

        private bool ValidateHWSettings(RB.HardwareSettings hw)
        {
            if (hw.DisplaySize.width <= 0 || hw.DisplaySize.width >= HW_MAX_DISPLAY_DIMENSION || hw.DisplaySize.height <= 0 || hw.DisplaySize.height >= HW_MAX_DISPLAY_DIMENSION)
            {
                Debug.LogError("Display resolution is invalid");
                return false;
            }

            if (hw.DisplaySize.width % 2 != 0 || hw.DisplaySize.height % 2 != 0)
            {
                Debug.LogError("Display width and height must both be divisible by 2!");
                return false;
            }

            if (hw.MapSize.width <= 0 || hw.MapSize.height <= 0 || hw.MapLayers <= 0)
            {
                Debug.LogError("Invalid map size");
                return false;
            }

            if (hw.MapLayers > HW_MAX_MAP_LAYERS)
            {
                Debug.LogError("Maximum map layers cannot exceed " + HW_MAX_MAP_LAYERS);
                return false;
            }

            if (hw.MapSize.width * hw.MapSize.height * hw.MapLayers > HW_MAX_MAP_TILES)
            {
                Debug.LogError("Maximum map tiles (width * height * layers) cannot exceed " + HW_MAX_MAP_TILES);
                return false;
            }

            if (hw.MapChunkSize.x < 1 || hw.MapChunkSize.x > 1024)
            {
                Debug.LogError("Map chunk width must be between 1 and 1024");
                return false;
            }

            if (hw.MapChunkSize.y < 1 || hw.MapChunkSize.y > 1024)
            {
                Debug.LogError("Map chunk height must be between 1 and 1024");
                return false;
            }

            if (hw.FPS < 20 || hw.FPS > 200)
            {
                Debug.LogError("FPS is invalid, should be between 20 and 200 frames per second");
                return false;
            }

            if ((int)hw.PixelStyle < (int)RB.PixelStyle.Square || (int)hw.PixelStyle > (int)RB.PixelStyle.Tall)
            {
                Debug.LogError("Pixel style is invalid");
                return false;
            }

            return true;
        }
    }
}
