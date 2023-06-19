namespace RetroBlitInternal
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Internal wrapper class for all the RetroBlit subsystems
    /// </summary>
    public class RBAPI : MonoBehaviour
    {
        /// <summary>
        /// Major version
        /// </summary>
        public static int MAJOR_VER = 3;

        /// <summary>
        /// Minor version
        /// </summary>
        public static int MINOR_VER = 4;

        /// <summary>
        /// Revision version
        /// </summary>
        public static int REV_VER = 0;

        /// <summary>
        /// Instance
        /// </summary>
        public static RBAPI instance = null;

        /// <summary>
        /// Reference to main thread
        /// </summary>
        public static System.Threading.Thread mainThread;

        /// <summary>
        /// RetroBlitHW
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBHardware HW;

        /// <summary>
        /// RetroBlitRenderer
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBRenderer Renderer;

        /// <summary>
        /// RetroBlitPixelCamera
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBPixelCamera PixelCamera;

        /// <summary>
        /// RetroBlitPresentCamera
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBPresentCamera PresentCamera;

        /// <summary>
        /// RetroBlitFont
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBFont Font;

        /// <summary>
        /// RetroBlitTilemap
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBTilemapTMX Tilemap;

        /// <summary>
        /// RetroBlitInput
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBInput Input;

        /// <summary>
        /// RetroBlitAudio
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBAudio Audio;

        /// <summary>
        /// RetroBlitEffects
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBEffects Effects;

        /// <summary>
        /// RBAssetManager
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBAssetManager AssetManager;

        /// <summary>
        /// RetroBlitResourceBucket
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBResourceBucket ResourceBucket;

        /// <summary>
        /// RetroBlitPerf
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public RBPerf Perf;

        /// <summary>
        /// Ticks
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public ulong Ticks = 0;

        /// <summary>
        /// Ticks Internal. Same a Ticks, but cannot be reset by user.
        /// </summary>
#if !RETROBLIT_STANDALONE
        [HideInInspector]
#endif
        public ulong TicksInternal = 0;

        private bool mInitialized = false;

        /// <summary>
        /// Get initialized state
        /// </summary>
        public bool Initialized
        {
            get { return mInitialized; }
        }

        /// <summary>
        /// Reset ticks
        /// </summary>
        public void TicksReset()
        {
            Ticks = 0;
        }

        /// <summary>
        /// Initialize the subsystem wrapper
        /// </summary>
        /// <param name="settings">Hardware settings to initialize with</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RB.HardwareSettings settings)
        {
            // Store the main thread for later reference
            mainThread = System.Threading.Thread.CurrentThread;

            ResourceBucket = gameObject.GetComponent<RBResourceBucket>();
            if (ResourceBucket == null)
            {
                return false;
            }

            HW = new RBHardware();
            if (HW == null || !HW.Initialize(settings))
            {
                return false;
            }

            var pixelCameraObj = GameObject.Find("RetroBlitPixelCamera");
            if (pixelCameraObj == null)
            {
                Debug.Log("Can't find RetroBlitPixelCamera game object, is your RetroBlit scene setup correctly?");
                return false;
            }

            PixelCamera = pixelCameraObj.GetComponent<RBPixelCamera>();
            if (PixelCamera == null || !PixelCamera.Initialize(this))
            {
                return false;
            }

            var presentCameraObj = GameObject.Find("RetroBlitPresentCamera");
            if (presentCameraObj == null)
            {
                Debug.Log("Can't find RetroBlitPresentCamera game object, is your RetroBlit scene setup correctly?");
                return false;
            }

            PresentCamera = presentCameraObj.GetComponent<RBPresentCamera>();
            if (PresentCamera == null || !PresentCamera.Initialize(this))
            {
                return false;
            }

            AssetManager = new RBAssetManager();
            if (AssetManager == null)
            {
                return false;
            }

            Renderer = new RBRenderer();
            if (Renderer == null || !Renderer.Initialize(this))
            {
                return false;
            }

            Font = new RBFont();
            if (Font == null || !Font.Initialize(this))
            {
                return false;
            }

            Tilemap = new RBTilemapTMX();
            if (Tilemap == null || !Tilemap.Initialize(this))
            {
                return false;
            }

            Input = new RBInput();
            if (Input == null || !Input.Initialize(this))
            {
                return false;
            }

            var audioObj = GameObject.Find("RetroBlitAudio");
            if (audioObj == null)
            {
                Debug.Log("Can't find RetroBlitAudio game object");
                return false;
            }

            Audio = audioObj.GetComponent<RBAudio>();
            if (Audio == null || !Audio.Initialize(this))
            {
                return false;
            }

            Effects = new RBEffects();
            if (Effects == null || !Effects.Initialize(this))
            {
                return false;
            }

            Perf = new RBPerf();
            if (Perf == null || !Perf.Initialize(this))
            {
                return false;
            }

            // Unload all assets that were waiting for main thread to get unloaded. These could be from a previously
            // initialized game.
            AssetManager.UnloadAllMainThread();

            instance = this;

            // Collect garbage
            System.GC.Collect();

            return true;
        }

        /// <summary>
        /// Finalize initialization, this is called after the game is initialized
        /// </summary>
        /// <param name="initialized">True if initialized successfully</param>
        public void FinalizeInitialization(bool initialized)
        {
            // Unload all assets that were waiting for main thread to get unloaded. These could be from a previously
            // initialized game.
            AssetManager.UnloadAllMainThread();

            mInitialized = initialized;
        }

        private void Start()
        {
#if UNITY_EDITOR
            // Debug.Log("Disabling live recompilation");
            UnityEditor.EditorApplication.LockReloadAssemblies();
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            // Debug.Log("Enabling live recompilation");
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
#endif

            Renderer.CleanUp();

            // Unload all assets that were waiting for main thread to get unloaded. These could be from a previously
            // initialized game.
            AssetManager.UnloadAllMainThread();
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            // Debug.Log("Enabling live recompilation");
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
#endif

            // Unload all assets that were waiting for main thread to get unloaded. These could be from a previously
            // initialized game.
            AssetManager.UnloadAllMainThread();
        }

        // Input string has to be assembled from Update() because FixedUpdate() will probably drop characters
        private void Update()
        {
            if (!mInitialized)
            {
                return;
            }

            if (Input != null)
            {
                Input.AppendInputString(UnityEngine.Input.inputString);
                Input.UpdateScrollWheel();
            }

            // Unload all assets that were waiting for main thread to get unloaded. These could be from a previously
            // initialized game.
            AssetManager.UnloadAllMainThread();
        }

        /// <summary>
        /// Heart beat of RetroBlit. RetroBlit runs on a fixed update.
        /// </summary>
        private void FixedUpdate()
        {
            if (!mInitialized)
            {
                return;
            }

            if (Input != null)
            {
                Input.FrameStart();
            }

            var game = RB.Game;
            if (game != null)
            {
                game.Update();
                Ticks++;
                TicksInternal++;
            }

            if (Perf != null)
            {
                Perf.UpdateEvent();
            }

            if (Input != null)
            {
                Input.FrameEnd();
            }

            if (Tilemap != null)
            {
                Tilemap.FrameEnd();
            }

            if (Font != null)
            {
                Font.FrameEnd();
            }
        }
    }
}
