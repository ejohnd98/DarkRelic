namespace RetroBlitInternal
{
    using UnityEngine;

    /// <summary>
    /// Effects subsystem
    /// </summary>
    public class RBEffects
    {
        /// <summary>
        /// Total count of effects, not including custom shader
        /// </summary>
        public static int TOTAL_EFFECTS = System.Enum.GetNames(typeof(RB.Effect)).Length;

        private readonly EffectParams[] mParams = new EffectParams[System.Enum.GetNames(typeof(RB.Effect)).Length + 2]; // +2 for custom shader and filter type
#pragma warning disable 0414 // Unused warning
        private RBAPI mRetroBlitAPI = null;
#pragma warning restore 0414

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Reference to subsystem wrapper</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RBAPI api)
        {
            mRetroBlitAPI = api;

            for (int i = 0; i < mParams.Length; i++)
            {
                mParams[i] = new EffectParams();
            }

            EffectReset();

            return true;
        }

        /// <summary>
        /// Get parameters for effect
        /// </summary>
        /// <param name="effect">Effect</param>
        /// <returns>Parameters</returns>
        public EffectParams ParamsGet(RB.Effect effect)
        {
            return mParams[(int)effect];
        }

        /// <summary>
        /// Set parameters for effect
        /// </summary>
        /// <param name="type">Effect</param>
        /// <param name="intensity">Intensity</param>
        /// <param name="vec">Vector data</param>
        /// <param name="color">RGB color</param>
        public void EffectSet(RB.Effect type, float intensity, Vector2i vec, Color32 color)
        {
            switch (type)
            {
                case RB.Effect.Noise:
                case RB.Effect.Shake:
                case RB.Effect.Negative:
                case RB.Effect.Pixelate:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    break;

                case RB.Effect.Curvature:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    ParamsGet(type).Color = color;
                    break;

                case RB.Effect.Saturation:
                    ParamsGet(type).Intensity = Mathf.Clamp(intensity, -1.0f, 1.0f);
                    break;

                case RB.Effect.Scanlines:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    break;

                case RB.Effect.Zoom:
                    ParamsGet(type).Intensity = Mathf.Clamp(intensity, 0, 10000.0f);
                    break;

                case RB.Effect.Slide:
                case RB.Effect.Wipe:
                    ParamsGet(type).Vector = new Vector2i(
                        Mathf.Clamp(vec.x, -RB.DisplaySize.width, RB.DisplaySize.width),
                        Mathf.Clamp(vec.y, -RB.DisplaySize.height, RB.DisplaySize.height));
                    break;

                case RB.Effect.Rotation:
                    ParamsGet(type).Intensity = RBUtil.WrapAngle(intensity);
                    break;

                case RB.Effect.ColorFade:
                case RB.Effect.ColorTint:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    ParamsGet(type).Color = color;
                    break;

                case RB.Effect.Fizzle:
                    // Increase intensity by 1% to ensure full pixel coverage
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity) * 1.01f;
                    ParamsGet(type).Color = color;
                    break;

                case RB.Effect.Pinhole:
                case RB.Effect.InvertedPinhole:
                    ParamsGet(type).Intensity = Mathf.Clamp01(intensity);
                    ParamsGet(type).Vector = new Vector2i((int)Mathf.Clamp(vec.x, 0, RB.DisplaySize.width - 1), (int)Mathf.Clamp(vec.y, 0, RB.DisplaySize.height - 1));
                    ParamsGet(type).Color = color;
                    break;

                case RB.Effect.ChromaticAberration:
                    ParamsGet(type).Vector = new Vector2i(
                        Mathf.Clamp(vec.x, -10000, 10000),
                        Mathf.Clamp(vec.y, -10000, 10000));
                    break;
            }
        }

        /// <summary>
        /// Set a custom shader effect
        /// </summary>
        /// <param name="shader">Shader asset to use</param>
        public void EffectShaderSet(ShaderAsset shader)
        {
            ParamsGet((RB.Effect)TOTAL_EFFECTS).Shader = shader;
        }

        /// <summary>
        /// Set texture filter to use with custom shader effect
        /// </summary>
        /// <param name="filterMode">Filter</param>
        public void EffectFilterSet(FilterMode filterMode)
        {
            ParamsGet((RB.Effect)TOTAL_EFFECTS + 1).FilterMode = filterMode;
        }

        /// <summary>
        /// Get a copy of the current effect states
        /// </summary>
        /// <param name="paramsCopy">Parameters to copy</param>
        public void CopyState(ref RBEffects.EffectParams[] paramsCopy)
        {
            if (paramsCopy == null)
            {
                paramsCopy = new EffectParams[mParams.Length];
            }

            for (int i = 0; i < paramsCopy.Length; i++)
            {
                mParams[i].ShallowCopy(ref paramsCopy[i]);
            }
        }

        /// <summary>
        /// Apply render time post processing effects, these are just drawing operations and must
        /// be ran before the other shader-time effects are applied
        /// </summary>
        public void ApplyRenderTimeEffects()
        {
            var renderer = mRetroBlitAPI.Renderer;

            // Pinhole effect
            if (mRetroBlitAPI.Effects.ParamsGet(RB.Effect.Pinhole).Intensity > 0)
            {
                var p = mRetroBlitAPI.Effects.ParamsGet(RB.Effect.Pinhole);

                if (mRetroBlitAPI.Effects.ParamsGet(RB.Effect.Pinhole).Intensity >= 1)
                {
                    renderer.DrawRectFill(0, 0, RB.DisplaySize.width, RB.DisplaySize.height, p.Color, 0, 0);
                }
                else
                {
                    Vector2i c = new Vector2i((int)(p.Vector.x + 0.5f), (int)(p.Vector.y + 0.5f));

                    int r = (int)((1.0f - p.Intensity) * renderer.MaxCircleRadiusForCenter(c));

                    renderer.DrawEllipseFill(c.x, c.y, r, r, p.Color, true);
                    if (c.x < RB.DisplaySize.width)
                    {
                        renderer.DrawRectFill(c.x + r + 1, c.y - r, RB.DisplaySize.width - c.x - r - 1, (r * 2) + 1, p.Color, 0, 0);
                    }

                    if (c.x > 0)
                    {
                        renderer.DrawRectFill(0, c.y - r, c.x - r, (r * 2) + 1, p.Color, 0, 0);
                    }

                    if (c.y > 0)
                    {
                        renderer.DrawRectFill(0, 0, RB.DisplaySize.width, c.y - r, p.Color, 0, 0);
                    }

                    if (c.y < RB.DisplaySize.height)
                    {
                        renderer.DrawRectFill(0, c.y + r + 1, RB.DisplaySize.width, RB.DisplaySize.height - (c.y + r + 1), p.Color, 0, 0);
                    }
                }
            }
            else if (mRetroBlitAPI.Effects.ParamsGet(RB.Effect.InvertedPinhole).Intensity > 0)
            {
                var p = mRetroBlitAPI.Effects.ParamsGet(RB.Effect.InvertedPinhole);

                Vector2i c = new Vector2i((int)(p.Vector.x + 0.5f), (int)(p.Vector.y + 0.5f));
                int r = (int)(p.Intensity * renderer.MaxCircleRadiusForCenter(c));

                renderer.DrawEllipseFill(c.x, c.y, r, r, p.Color, false);
            }

            // Curvature
            var curvEffect = mRetroBlitAPI.Effects.ParamsGet(RB.Effect.Curvature);
            if (curvEffect.Intensity > 0)
            {
                renderer.DrawRect(0, 0, RB.DisplaySize.width, RB.DisplaySize.height, curvEffect.Color);
            }

            renderer.DrawClipRegions();
        }

        /// <summary>
        /// Reset all effects back to default/off states
        /// </summary>
        public void EffectReset()
        {
            for (int i = 0; i < mParams.Length; i++)
            {
                mParams[i].Color = Color.white;
                mParams[i].Shader = null;
                mParams[i].Intensity = 0.0f;
                mParams[i].Vector = Vector2i.zero;
            }

            mParams[(int)RB.Effect.Zoom].Intensity = 1.0f;
            mParams[TOTAL_EFFECTS].Shader = null;
            ParamsGet((RB.Effect)TOTAL_EFFECTS + 1).FilterMode = (int)FilterMode.Point;
        }

        /// <summary>
        /// Effect parameters
        /// </summary>
        public class EffectParams
        {
            /// <summary>
            /// Intensity of effect, usually 0.0 to 1.0
            /// </summary>
            public float Intensity;

            /// <summary>
            /// Generic vector data
            /// </summary>
            public Vector2i Vector;

            /// <summary>
            /// RGB color
            /// </summary>
            public Color32 Color;

            /// <summary>
            /// Shader asset
            /// </summary>
            public ShaderAsset Shader;

            /// <summary>
            /// Filter mode to use with shader
            /// </summary>
            public FilterMode FilterMode;

            /// <summary>
            /// Shallow copy of an effect
            /// </summary>
            /// <param name="paramsCopy">Parameters to copy</param>
            public void ShallowCopy(ref EffectParams paramsCopy)
            {
                if (paramsCopy == null)
                {
                    paramsCopy = new EffectParams();
                }

                paramsCopy.Intensity = Intensity;
                paramsCopy.Vector = Vector;
                paramsCopy.Color = Color;
                paramsCopy.Shader = Shader;
            }
        }
    }
}
