namespace RetroBlitInternal
{
    using UnityEngine;

    /// <summary>
    /// Pixel Camera subsystem that renders to an offscreen "pixel surface" that has the pixel dimensions of an old videogame console
    /// </summary>
    public sealed class RBPixelCamera : MonoBehaviour
    {
        private UnityEngine.Camera mPixelCamera;
        private UnityEngine.Camera mPresentCamera;

        private Vector2i mPreviousScreenSize = new Vector2i(-1, -1);

        private RBAPI mRetroBlitAPI = null;

        /// <summary>
        /// Initialize subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RBAPI api)
        {
            mRetroBlitAPI = api;

            var pixelCameraGameObj = GameObject.Find("RetroBlitPixelCamera");
            if (pixelCameraGameObj == null)
            {
                Debug.LogError("Can't find RetroBlitPixelCamera gameObject! Is your scene setup correctly for RetroBlit?");
                return false;
            }

            mPixelCamera = pixelCameraGameObj.GetComponent<Camera>();
            if (mPixelCamera == null)
            {
                Debug.LogError("RetroBlitPixelCamera gameObject does not contain a camera! Is your scene setup correctly for RetroBlit?");
                return false;
            }

            var presentCameraGameObj = GameObject.Find("RetroBlitPresentCamera");
            if (pixelCameraGameObj == null)
            {
                Debug.LogError("Can't find RetroBlitPresentCamera gameObject! Is your scene setup correctly for RetroBlit?");
                return false;
            }

            mPresentCamera = presentCameraGameObj.GetComponent<Camera>();
            if (mPresentCamera == null)
            {
                Debug.LogError("RetroBlitPresentCamera gameObject does not contain a camera! Is your scene setup correctly for RetroBlit?");
                return false;
            }

            if (RB.DisplaySize.width <= 0 || RB.DisplaySize.height <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get Unity camera
        /// </summary>
        /// <returns>Camera</returns>
        public UnityEngine.Camera GetCamera()
        {
            return mPixelCamera;
        }

        /// <summary>
        /// Get the current render target
        /// </summary>
        /// <returns>Current render target</returns>
        public RenderTexture GetRenderTarget()
        {
            return mPixelCamera.targetTexture;
        }

        /// <summary>
        /// Set the current render target
        /// </summary>
        /// <param name="renderTarget">Render target</param>
        public void SetRenderTarget(RenderTexture renderTarget)
        {
            mPixelCamera.targetTexture = renderTarget;
            WindowResize();
        }

        private void WindowResize()
        {
            if (mPixelCamera.targetTexture == null)
            {
                return;
            }

            mPixelCamera.orthographicSize = mPixelCamera.targetTexture.height * 0.5f;
            mPixelCamera.rect = new Rect(0, 0, mPixelCamera.targetTexture.width, mPixelCamera.targetTexture.height);
            mPixelCamera.transform.position = new Vector3(0, 0, -10);
            mPixelCamera.transform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Good place to update window size, and shader variables
        /// </summary>
        private void Update()
        {
            if (mPixelCamera == null || mPixelCamera.targetTexture == null)
            {
                return;
            }

            // Check for window resize
            if (mPreviousScreenSize.y != Screen.height ||
                mPreviousScreenSize.x != Screen.width)
            {
                mPreviousScreenSize.x = Screen.width;
                mPreviousScreenSize.y = Screen.height;

                WindowResize();
            }
        }

        private void RenderUser()
        {
            if (mPixelCamera != null && mPixelCamera.targetTexture != null && mRetroBlitAPI != null && mRetroBlitAPI.Renderer != null && mRetroBlitAPI.Initialized)
            {
                mRetroBlitAPI.Renderer.RenderEnabled = true;

                mRetroBlitAPI.Renderer.StartRender();

                if (RB.Game != null)
                {
                    RB.Game.Render();
                }

                if (mRetroBlitAPI.Perf != null)
                {
                    mRetroBlitAPI.Perf.RenderEvent();
                }

                mRetroBlitAPI.Perf.Draw();

                mRetroBlitAPI.Renderer.FrameEnd();

                mRetroBlitAPI.Renderer.RenderEnabled = false;

                mRetroBlitAPI.AssetManager.UpdateAsyncResources();
            }
        }

#if RETROBLIT_STANDALONE
        private void OnPostRender()
        {
            RenderUser();
        }
#else
        /// <summary>
        /// First ask the user to do their rendering, then wait for end of frame, and finally display to screen
        /// </summary>
        private void OnPostRender()
        {
            RenderUser();
        }
#endif
    }
}
