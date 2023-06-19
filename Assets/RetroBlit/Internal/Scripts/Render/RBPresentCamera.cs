namespace RetroBlitInternal
{
    using UnityEngine;

    /// <summary>
    /// Pixel Camera subsystem that renders to an offscreen "pixel surface" that has the pixel dimensions of an old videogame console
    /// </summary>
    public sealed class RBPresentCamera : MonoBehaviour
    {
        private const float SHAKE_INTERVAL = 0.01f;

        private bool mPresentEnabled = true;

        private Material mPresentMaterial;
        private Material mCurrentPresentMaterial;

        private UnityEngine.Camera mPixelCamera;
        private UnityEngine.Camera mPresentCamera;

        private RBAPI mRetroBlitAPI = null;

        private Vector2i mPresentSize = Vector2i.zero;
        private Vector2i mPresentOffset = Vector2i.zero;

        private Vector2i mLastShakeOffset = Vector2i.zero;
        private float mShakeDelay = 0;

        private int mPropIDPixelTexture;
        private int mPropIDPixelTextureSize;
        private int mPropIDPixelTextureSizeInverse;
        private int mPropIDPixelTextureSizeRatio;
        private int mPropIDPresentSize;
        private int mPropIDSampleFactor;
        private int mPropIDScanlineIntensity;
        private int mPropIDScanlineOffset;
        private int mPropIDScanlineLength;
        private int mPropIDSaturationIntensity;
        private int mPropIDNoiseIntensity;
        private int mPropIDNoiseSeed;
        private int mPropIDCurvatureIntensity;
        private int mPropIDColorFade;
        private int mPropIDColorFadeIntensity;
        private int mPropIDColorTint;
        private int mPropIDColorTintIntensity;
        private int mPropIDFizzleColor;
        private int mPropIDFizzleIntensity;
        private int mPropIDNegativeIntensity;
        private int mPropIDPixelateIntensity;
        private int mPropIDPixelateIntensityInverse;
        private int mPropIDChromaticAberration;

        /// <summary>
        /// Initialize subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RBAPI api)
        {
            mPropIDPixelTexture = Shader.PropertyToID("_PixelTexture");
            mPropIDPixelTextureSize = Shader.PropertyToID("_PixelTextureSize");
            mPropIDPixelTextureSizeInverse = Shader.PropertyToID("_PixelTextureSizeInverse");
            mPropIDPixelTextureSizeRatio = Shader.PropertyToID("_PixelTextureSizeRatio");
            mPropIDPresentSize = Shader.PropertyToID("_PresentTextureSize");
            mPropIDSampleFactor = Shader.PropertyToID("_SampleFactor");
            mPropIDScanlineIntensity = Shader.PropertyToID("_ScanlineIntensity");
            mPropIDScanlineOffset = Shader.PropertyToID("_ScanlineOffset");
            mPropIDScanlineLength = Shader.PropertyToID("_ScanlineLength");
            mPropIDSaturationIntensity = Shader.PropertyToID("_SaturationIntensity");
            mPropIDNoiseIntensity = Shader.PropertyToID("_NoiseIntensity");
            mPropIDNoiseSeed = Shader.PropertyToID("_NoiseSeed");
            mPropIDCurvatureIntensity = Shader.PropertyToID("_CurvatureIntensity");
            mPropIDColorFade = Shader.PropertyToID("_ColorFade");
            mPropIDColorFadeIntensity = Shader.PropertyToID("_ColorFadeIntensity");
            mPropIDColorTint = Shader.PropertyToID("_ColorTint");
            mPropIDColorTintIntensity = Shader.PropertyToID("_ColorTintIntensity");
            mPropIDFizzleColor = Shader.PropertyToID("_FizzleColor");
            mPropIDFizzleIntensity = Shader.PropertyToID("_FizzleIntensity");
            mPropIDNegativeIntensity = Shader.PropertyToID("_NegativeIntensity");
            mPropIDPixelateIntensity = Shader.PropertyToID("_PixelateIntensity");
            mPropIDPixelateIntensityInverse = Shader.PropertyToID("_PixelateIntensityInverse");
            mPropIDChromaticAberration = Shader.PropertyToID("_ChromaticAberration");

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

#if !RETROBLIT_STANDALONE
            mPresentCamera.gameObject.SetActive(true);
#endif

            if (RB.DisplaySize.width <= 0 || RB.DisplaySize.height <= 0)
            {
                return false;
            }

            var material = mRetroBlitAPI.ResourceBucket.LoadMaterial("PresentMaterial");
            mPresentMaterial = new Material(material);

            mCurrentPresentMaterial = mPresentMaterial;

            // Optimize present camera settings
            mPresentCamera.clearFlags = CameraClearFlags.SolidColor;
            mPresentCamera.backgroundColor = Color.black;
            mPresentCamera.allowMSAA = false;
            mPresentCamera.renderingPath = RenderingPath.VertexLit;
            mPresentCamera.allowHDR = false;
            mPresentCamera.allowDynamicResolution = false;

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
        /// Convert screen point to viewport point
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>Converted position</returns>
        public Vector3 ScreenToViewportPoint(Vector3 p)
        {
            if (mPresentSize.width < 1 || mPresentSize.height < 1)
            {
                return new Vector3(0, 0, 0);
            }

            p.z = 0;

            p.x -= mPresentOffset.x;
            p.y += mPresentOffset.y;
            p.y = Screen.height - p.y;

            p = mPixelCamera.ScreenToViewportPoint(p);

            p.x *= mPixelCamera.pixelRect.width / (Screen.width - (mPresentOffset.x * 2));
            p.y *= mPixelCamera.pixelRect.height / (Screen.height - (mPresentOffset.y * 2));

            p.x *= mPixelCamera.pixelRect.width;
            p.y *= mPixelCamera.pixelRect.height;

            return p;
        }

        /// <summary>
        /// Set presenting to display
        /// </summary>
        /// <param name="enabled">Present if true, don't if false</param>
        public void PresentEnabledSet(bool enabled)
        {
            mPresentEnabled = enabled;

#if !RETROBLIT_STANDALONE
            mPresentCamera.gameObject.SetActive(enabled);
#endif
        }

        /// <summary>
        /// Setup all shader global variables, there is a bunch, most are tied to post processing effects
        /// </summary>
        /// <param name="effectParams">Array of all current effects</param>
        /// <param name="pixelTexture">Reference to the pixel texture being rendered</param>
        private void SetShaderGlobals(RBEffects.EffectParams[] effectParams, RenderTexture pixelTexture)
        {
            var customShader = mRetroBlitAPI.Renderer.ShaderGetMaterial(effectParams[(int)RBEffects.TOTAL_EFFECTS].Shader);

            if (customShader != null)
            {
                mCurrentPresentMaterial = customShader;

                mCurrentPresentMaterial.SetTexture(mPropIDPixelTexture, pixelTexture);
                mCurrentPresentMaterial.SetVector(mPropIDPixelTextureSize, new Vector2(RB.DisplaySize.width, RB.DisplaySize.height));
                mCurrentPresentMaterial.SetVector(mPropIDPixelTextureSizeInverse, new Vector2(1.0f / RB.DisplaySize.width, 1.0f / RB.DisplaySize.height));
                mCurrentPresentMaterial.SetVector(mPropIDPixelTextureSizeRatio, new Vector2((float)RB.DisplaySize.width / (float)RB.DisplaySize.height, (float)RB.DisplaySize.height / (float)RB.DisplaySize.width));
                mCurrentPresentMaterial.SetVector(mPropIDPresentSize, new Vector2(Screen.width, Screen.height));

                float sampleFactor = 0;

                if (mPresentSize.width % RB.DisplaySize.width != 0 || mPresentSize.height % RB.DisplaySize.height != 0)
                {
                    sampleFactor = 1.0f / (((float)Screen.width / RB.DisplaySize.width) * 2.5f);
                }

                mCurrentPresentMaterial.SetFloat(mPropIDSampleFactor, sampleFactor);

                FilterMode filterMode = effectParams[(int)RBEffects.TOTAL_EFFECTS + 1].FilterMode;
                pixelTexture.filterMode = filterMode;
            }
            else
            {
                var scanlineParams = effectParams[(int)RB.Effect.Scanlines];
                var noiseParams = effectParams[(int)RB.Effect.Noise];
                var desatParams = effectParams[(int)RB.Effect.Saturation];
                var curvParams = effectParams[(int)RB.Effect.Curvature];
                var fizzleParams = effectParams[(int)RB.Effect.Fizzle];
                var zoomParams = effectParams[(int)RB.Effect.Zoom];
                var pixelateParams = effectParams[(int)RB.Effect.Pixelate];
                var colorTintParams = effectParams[(int)RB.Effect.ColorTint];
                var colorFadeParams = effectParams[(int)RB.Effect.ColorFade];
                float negativeIntensity = effectParams[(int)RB.Effect.Negative].Intensity;
                float pixelateIntensity = effectParams[(int)RB.Effect.Pixelate].Intensity;
                var chromaticAberration = effectParams[(int)RB.Effect.ChromaticAberration];

                mCurrentPresentMaterial = mPresentMaterial;

                // Enable/Disable keyboard depending on present parameters
                if (noiseParams.Intensity > 0 || fizzleParams.Intensity > 0)
                {
                    mCurrentPresentMaterial.EnableKeyword("NOISE_OR_FIZZLE");
                }
                else
                {
                    mCurrentPresentMaterial.DisableKeyword("NOISE_OR_FIZZLE");
                }

                if (scanlineParams.Intensity > 0)
                {
                    mCurrentPresentMaterial.EnableKeyword("SCANLINE");
                }
                else
                {
                    mCurrentPresentMaterial.DisableKeyword("SCANLINE");
                }

                if (chromaticAberration.Vector.x != 0 || chromaticAberration.Vector.y != 0)
                {
                    mCurrentPresentMaterial.EnableKeyword("CHROMA");
                }
                else
                {
                    mCurrentPresentMaterial.DisableKeyword("CHROMA");
                }

                if (curvParams.Intensity > 0)
                {
                    mCurrentPresentMaterial.EnableKeyword("CURVATURE");
                }
                else
                {
                    mCurrentPresentMaterial.DisableKeyword("CURVATURE");
                }

                if (pixelateIntensity > 0)
                {
                    mCurrentPresentMaterial.EnableKeyword("PIXELATE");
                }
                else
                {
                    mCurrentPresentMaterial.DisableKeyword("PIXELATE");
                }

                if (desatParams.Intensity != 0)
                {
                    mCurrentPresentMaterial.EnableKeyword("SATURATE");
                }
                else
                {
                    mCurrentPresentMaterial.DisableKeyword("SATURATE");
                }

                if ((Screen.width % RB.DisplaySize.width) != 0 || (Screen.height % RB.DisplaySize.height) != 0 || curvParams.Intensity > 0)
                {
                    mCurrentPresentMaterial.EnableKeyword("SMOOTHING");
                }
                else
                {
                    mCurrentPresentMaterial.DisableKeyword("SMOOTHING");
                }

                mCurrentPresentMaterial.SetTexture(mPropIDPixelTexture, pixelTexture);
                mCurrentPresentMaterial.SetVector(mPropIDPixelTextureSize, new Vector2(RB.DisplaySize.width, RB.DisplaySize.height));
                mCurrentPresentMaterial.SetVector(mPropIDPixelTextureSizeInverse, new Vector2(1.0f / RB.DisplaySize.width, 1.0f / RB.DisplaySize.height));
                mCurrentPresentMaterial.SetVector(mPropIDPixelTextureSizeRatio, new Vector2((float)RB.DisplaySize.width / (float)RB.DisplaySize.height, (float)RB.DisplaySize.height / (float)RB.DisplaySize.width));
                mCurrentPresentMaterial.SetVector(mPropIDPresentSize, new Vector2(Screen.width, Screen.height));

                float sampleFactor = 0;

                if (pixelateParams.Intensity == 0)
                {
                    if (mPresentSize.width % RB.DisplaySize.width != 0 || mPresentSize.height % RB.DisplaySize.height != 0 ||
                        curvParams.Intensity != 0)
                    {
                        sampleFactor = 1.0f / (((float)Screen.width / RB.DisplaySize.width) * 2.5f);
                        if (zoomParams.Intensity != 1)
                        {
                            sampleFactor /= zoomParams.Intensity;
                        }
                    }
                }

                mCurrentPresentMaterial.SetFloat(mPropIDSampleFactor, sampleFactor);

                // Apply retroness
                mCurrentPresentMaterial.SetFloat(mPropIDScanlineIntensity, scanlineParams.Intensity);
                if (scanlineParams.Intensity > 0)
                {
                    int offset;
                    int length;
                    float pixelSize = mPresentSize.height / (float)RB.DisplaySize.height;
                    mRetroBlitAPI.Renderer.GetScanlineOffsetLength(pixelSize, out offset, out length);

                    mCurrentPresentMaterial.SetFloat(mPropIDScanlineOffset, offset);
                    mCurrentPresentMaterial.SetFloat(mPropIDScanlineLength, length);
                }

                mCurrentPresentMaterial.SetFloat(mPropIDSaturationIntensity, desatParams.Intensity);

                if (noiseParams.Intensity > 0)
                {
                    mCurrentPresentMaterial.SetFloat(mPropIDNoiseIntensity, noiseParams.Intensity);

                    var oldRandState = Random.state;
                    Random.InitState((int)RB.Ticks);

                    mCurrentPresentMaterial.SetVector(
                        mPropIDNoiseSeed,
                        new Vector2(Random.Range(0, RB.DisplaySize.width) / (float)RB.DisplaySize.width, Random.Range(0, RB.DisplaySize.height) / (float)RB.DisplaySize.height));

                    Random.state = oldRandState;
                }
                else
                {
                    mCurrentPresentMaterial.SetFloat(mPropIDNoiseIntensity, 0);
                }

                mCurrentPresentMaterial.SetFloat(mPropIDCurvatureIntensity, curvParams.Intensity);

                // Color Fade
                Color32 colorFade;
                colorFade = colorFadeParams.Color;

                mCurrentPresentMaterial.SetVector(mPropIDColorFade, new Vector3(colorFade.r / 255.0f, colorFade.g / 255.0f, colorFade.b / 255.0f));
                mCurrentPresentMaterial.SetFloat(mPropIDColorFadeIntensity, colorFadeParams.Intensity);

                // Color Tint
                Color32 colorTint;
                colorTint = colorTintParams.Color;

                mCurrentPresentMaterial.SetVector(mPropIDColorTint, new Vector3(colorTint.r / 255.0f, colorTint.g / 255.0f, colorTint.b / 255.0f));
                mCurrentPresentMaterial.SetFloat(mPropIDColorTintIntensity, colorTintParams.Intensity);

                // Fizzle
                Color32 colorFizzle;
                colorFizzle = fizzleParams.Color;

                mCurrentPresentMaterial.SetVector(mPropIDFizzleColor, new Vector3(colorFizzle.r / 255.0f, colorFizzle.g / 255.0f, colorFizzle.b / 255.0f));
                mCurrentPresentMaterial.SetFloat(mPropIDFizzleIntensity, fizzleParams.Intensity);

                // Negative
                mCurrentPresentMaterial.SetFloat(mPropIDNegativeIntensity, negativeIntensity);

                // Pixelate
                float pixelateScaled = 1.0f + (pixelateIntensity * 100 * 30);
                mCurrentPresentMaterial.SetFloat(mPropIDPixelateIntensity, pixelateScaled);
                mCurrentPresentMaterial.SetFloat(mPropIDPixelateIntensityInverse, 1.0f / pixelateScaled);

                // Chromatic Aberration
                mCurrentPresentMaterial.SetVector(mPropIDChromaticAberration, new Vector2(chromaticAberration.Vector.x * 0.00002f, chromaticAberration.Vector.y * 0.00002f));
            }
        }

        private void RenderPixelSurfaces(RenderTexture srcTexture, RenderTexture dstTexture)
        {
            if (mPixelCamera == null || mPixelCamera.targetTexture == null || mRetroBlitAPI == null || mRetroBlitAPI.Renderer == null || !mRetroBlitAPI.Initialized)
            {
                return;
            }

            RenderTexture.active = dstTexture;

            int usedBuffers;
            var frontBuffers = mRetroBlitAPI.Renderer.GetFrontBuffer().GetBuffers(out usedBuffers);

            for (int bufferIndex = 0; bufferIndex < usedBuffers; bufferIndex++)
            {
                var buffer = frontBuffers[bufferIndex];
                var effectParams = buffer.effectParams;

                SetShaderGlobals(effectParams, buffer.tex);

                Vector2i displaySize = RB.DisplaySize;
                if (mRetroBlitAPI.HW.PixelStyle == RB.PixelStyle.Wide)
                {
                    displaySize.x *= 2;
                }
                else if (mRetroBlitAPI.HW.PixelStyle == RB.PixelStyle.Tall)
                {
                    displaySize.y *= 2;
                }

                mPresentSize.width = Screen.width;
                mPresentSize.height = (int)(Screen.width * ((float)displaySize.y / (float)displaySize.x));
                if (mPresentSize.height > Screen.height)
                {
                    mPresentSize.width = (int)(Screen.height * ((float)displaySize.x / (float)displaySize.y));
                    mPresentSize.height = Screen.height;
                }

                // Round up present size to the next multiple of scanline pattern length. Without this we can get bad repetition
                // patterns in the scanline effect
                // At most this will cut off a part of 1 pixel.
                int offset;
                int length;
                float pixelSize = mPresentSize.height / (float)displaySize.y;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                mRetroBlitAPI.Renderer.GetScanlineOffsetLength(pixelSize, out offset, out length);
#pragma warning restore IDE0059 // Unnecessary assignment of a value

                if (mPresentSize.width % length > 0)
                {
                    mPresentSize.width += length - (mPresentSize.width % length);
                }

                if (mPresentSize.height % length > 0)
                {
                    mPresentSize.height += length - (mPresentSize.height % length);
                }

                mPresentOffset.x = (int)((Screen.width - mPresentSize.width) * 0.5f);
                mPresentOffset.y = (int)((Screen.height - mPresentSize.height) * 0.5f);

                var clipRect = new Rect(mPresentOffset.x, mPresentOffset.y, mPresentSize.width, mPresentSize.height);

                // Slide effect
                Vector2i slideOffset = effectParams[(int)RB.Effect.Slide].Vector;
                Vector2 slideOffsetf = new Vector2(
                    ((float)slideOffset.x / (float)RB.DisplaySize.width) * mPresentSize.width,
                    ((float)slideOffset.y / (float)RB.DisplaySize.height) * mPresentSize.height);
                mPresentOffset += new Vector2i((int)slideOffsetf.x, (int)slideOffsetf.y);

                // Clear, but only on first buffer
                if (bufferIndex == 0)
                {
                    GL.Clear(true, true, new Color32(0, 0, 0, 255));
                }

                // Wipe effect
                Rect destRect = new Rect(mPresentOffset.x, mPresentOffset.y, mPresentSize.width, mPresentSize.height);
                Rect srcRect = new Rect(0, 0, 1, 1);

                Vector2i wipe = effectParams[(int)RB.Effect.Wipe].Vector;
                Vector2 wipef = new Vector2((float)wipe.x / (float)RB.DisplaySize.width, (float)wipe.y / (float)RB.DisplaySize.height);

                if (wipe.x > 0)
                {
                    destRect.x = mPresentOffset.x + (mPresentSize.width * wipef.x);
                    destRect.width = mPresentSize.width - (mPresentSize.width * wipef.x);
                    srcRect.x = wipef.x;
                    srcRect.width = 1f - wipef.x;
                }
                else if (wipe.x < 0)
                {
                    destRect.x = mPresentOffset.x;
                    destRect.width = mPresentSize.width - (mPresentSize.width * (-wipef.x));
                    srcRect.x = 0;
                    srcRect.width = 1f - (-wipef.x);
                }

                if (wipe.y > 0)
                {
                    destRect.y = mPresentOffset.y + (mPresentSize.height * wipef.y);
                    destRect.height = mPresentSize.height - (mPresentSize.height * wipef.y);
                    srcRect.y = 0;
                    srcRect.height = 1f - wipef.y;
                }
                else if (wipe.y < 0)
                {
                    destRect.y = mPresentOffset.y;
                    destRect.height = mPresentSize.height - (mPresentSize.height * (-wipef.y));
                    srcRect.y = -wipef.y;
                    srcRect.height = 1f - (-wipef.y);
                }

                // Shake
                float shake = effectParams[(int)RB.Effect.Shake].Intensity;
                if (shake > 0)
                {
                    // Don't shake every frame, shake at a set interval, and decay the shake offset
                    // between intervals
                    if (mShakeDelay <= 0)
                    {
                        var oldRandState = Random.state;
                        Random.InitState((int)RB.Ticks);

                        float maxMag = mPresentSize.width * 0.05f;
                        destRect.x += maxMag * shake * Random.Range(-1.0f, 1.0f);
                        destRect.y += maxMag * shake * Random.Range(-1.0f, 1.0f);

                        mLastShakeOffset = new Vector2i((int)destRect.x, (int)destRect.y);
                        mShakeDelay = SHAKE_INTERVAL;

                        Random.state = oldRandState;
                    }
                    else
                    {
                        destRect.x = mLastShakeOffset.x;
                        destRect.y = mLastShakeOffset.y;
                        mLastShakeOffset.x = (int)(mLastShakeOffset.x * 0.75f);
                        mLastShakeOffset.y = (int)(mLastShakeOffset.y * 0.75f);
                        mShakeDelay -= mRetroBlitAPI.HW.UpdateInterval;
                    }
                }
                else
                {
                    mLastShakeOffset = Vector2i.zero;
                    mShakeDelay = 0;
                }

                // Zoom
                float zoom = effectParams[(int)RB.Effect.Zoom].Intensity;
                if (zoom != 1)
                {
                    destRect.width *= zoom;
                    destRect.height *= zoom;
                    destRect.x += (mPresentSize.width - destRect.width) * 0.5f;
                    destRect.y += (mPresentSize.height - destRect.height) * 0.5f;
                }

                GL.PushMatrix();

                float rotation = effectParams[(int)RB.Effect.Rotation].Intensity;

                if (rotation != 0)
                {
                    GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);

                    var matrix = Matrix4x4.TRS(new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity, Vector3.one);
                    matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotation), Vector3.one);
                    matrix *= Matrix4x4.TRS(new Vector3(-Screen.width / 2, -Screen.height / 2, 0), Quaternion.identity, Vector3.one);
                    GL.MultMatrix(matrix);
                }
                else
                {
                    GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                }

                // Do not try to render at all if currently offscreen (eg due to slide effect)
                if (!(destRect.x + destRect.width < clipRect.x ||
                    destRect.x >= clipRect.x + clipRect.width ||
                    destRect.y + destRect.height < clipRect.y ||
                    destRect.y >= clipRect.y + clipRect.height))
                {
                    if ((int)destRect.x < (int)clipRect.x)
                    {
                        var correction = clipRect.x - destRect.x;
                        srcRect.x += correction / destRect.width;
                        srcRect.width -= correction / destRect.width;
                        destRect.x = clipRect.x;
                        destRect.width -= correction;
                    }
                    else if ((int)(destRect.x + destRect.width) > (int)(clipRect.x + clipRect.width))
                    {
                        var correction = (destRect.x + destRect.width) - (clipRect.x + clipRect.width);
                        srcRect.width = (destRect.width - correction) / destRect.width;
                        destRect.width -= correction;
                    }

                    if ((int)destRect.y < (int)clipRect.y)
                    {
                        var correction = clipRect.y - destRect.y;
                        srcRect.height = (destRect.height - correction) / destRect.height;
                        destRect.y = clipRect.y;
                        destRect.height -= correction;
                    }
                    else if ((int)(destRect.y + destRect.height) > (int)(clipRect.y + clipRect.height))
                    {
                        var correction = (destRect.y + destRect.height) - (clipRect.y + clipRect.height);
                        srcRect.y += correction / destRect.height;
                        srcRect.height -= correction / destRect.height;
                        destRect.height -= correction;
                    }

                    Graphics.DrawTexture(destRect, srcTexture, srcRect, 0, 0, 0, 0, mCurrentPresentMaterial);
                }

                GL.PopMatrix();

                buffer.tex.filterMode = FilterMode.Point;
            }
        }

#if RETROBLIT_STANDALONE
        private void OnRenderImage(RenderTexture renderTextureIn, RenderTexture renderTextureOut)
        {
            RenderPixelSurfaces(mPixelCamera.targetTexture, mPresentCamera.targetTexture);
        }
#else
        private void OnRenderImage(RenderTexture renderTextureIn, RenderTexture renderTextureOut)
        {
            if (mPresentEnabled)
            {
                RenderPixelSurfaces(mPixelCamera.activeTexture, renderTextureOut);

                // Make sure we can never get the warning "OnRenderImage() possibly didn't write anything to the destination texture!"
                RenderTexture.active = renderTextureOut;
            }
        }
#endif
    }
}
