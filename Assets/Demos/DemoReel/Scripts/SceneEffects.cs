namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate post processing effects
    /// </summary>
    public class SceneEffects : SceneDemo
    {
        private static int mCloudTicks = 0;

        private readonly RB.Effect mEffect = 0;
        private readonly FastString mParamsText = new FastString(256);
        private readonly FastString mConvertString = new FastString(256);

        private readonly string[] mEffectNames = new string[(int)RB.Effect.ChromaticAberration + 1];

        private readonly TMXMapAsset mMap = new TMXMapAsset();

        private readonly SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();

        private Vector2i mMapSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="effect">Which effect to demonstrate</param>
        public SceneEffects(RB.Effect effect)
        {
            mEffect = effect;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            mSpriteSheet1.Load("DemoReel/Sprites");
            mSpriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));
            RB.SpriteSheetSet(mSpriteSheet1);

            RB.EffectReset();

            mMap.Load("DemoReel/Tilemap");

            RB.MapLayerSpriteSheetSet(0, mSpriteSheet1);
            RB.MapLayerSpriteSheetSet(1, mSpriteSheet1);
            RB.MapLayerSpriteSheetSet(2, mSpriteSheet1);

            if (mMap != null)
            {
                mMapSize = mMap.size;

                mMap.LoadLayer("Clouds", 0);
                mMap.LoadLayer("Decoration", 1);
                mMap.LoadLayer("Terrain", 2);
            }

            mEffectNames[(int)RB.Effect.Scanlines] = "Scanlines";
            mEffectNames[(int)RB.Effect.Noise] = "Noise";
            mEffectNames[(int)RB.Effect.Saturation] = "Saturation";
            mEffectNames[(int)RB.Effect.Curvature] = "Curvature";
            mEffectNames[(int)RB.Effect.Slide] = "Slide";
            mEffectNames[(int)RB.Effect.Wipe] = "Wipe";
            mEffectNames[(int)RB.Effect.Shake] = "Shake";
            mEffectNames[(int)RB.Effect.Zoom] = "Zoom";
            mEffectNames[(int)RB.Effect.Rotation] = "Rotation";
            mEffectNames[(int)RB.Effect.ColorFade] = "ColorFade";
            mEffectNames[(int)RB.Effect.ColorTint] = "ColorTint";
            mEffectNames[(int)RB.Effect.Negative] = "Negative";
            mEffectNames[(int)RB.Effect.Pixelate] = "Pixelate";
            mEffectNames[(int)RB.Effect.Pinhole] = "Pinhole";
            mEffectNames[(int)RB.Effect.InvertedPinhole] = "InvertedPinhole";
            mEffectNames[(int)RB.Effect.Fizzle] = "Fizzle";
            mEffectNames[(int)RB.Effect.ChromaticAberration] = "ChromaticAberration";
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.EffectReset();
            RB.MapClear();

            mSpriteSheet1.Unload();
            mMap.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            mCloudTicks++;
            ApplyEffect();

            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            RB.Clear(DemoUtil.IndexToRGB(22));

            if (mMap != null)
            {
                int cloudScrollPos = -mCloudTicks % (mMapSize.width * RB.SpriteSheetGet().grid.cellSize.width);

                RB.CameraSet(new Vector2i(mMapSize.width * RB.SpriteSheetGet().grid.cellSize.width / 4, 0));
                RB.DrawMapLayer(0, new Vector2i(cloudScrollPos, 0));
                RB.DrawMapLayer(0, new Vector2i(cloudScrollPos + (mMapSize.width * RB.SpriteSheetGet().grid.cellSize.width), 0));
                RB.DrawMapLayer(1);
                RB.DrawMapLayer(2);
                RB.CameraReset();
            }
            else
            {
                RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/Tilemap.tmx in Unity");
            }

            var color = DemoUtil.IndexToRGB(2);
            mFormatStr.Set(mEffectNames[(int)mEffect]).Append("\n@");
            mFormatStr.Append(color.r, 2, FastString.FORMAT_HEX_CAPS).Append(color.g, 2, FastString.FORMAT_HEX_CAPS).Append(color.b, 2, FastString.FORMAT_HEX_CAPS);
            mConvertString.Set(mEffectNames[(int)mEffect]).ToUpperInvariant();
            mFormatStr.Append("RB.EffectSet(RB.Effect.").Append(mConvertString).Append(mParamsText).Append(");");

            RB.Print(new Vector2i((RB.DisplaySize.width / 2) - 120, (RB.DisplaySize.height / 2) - 10), DemoUtil.IndexToRGB(0), mFormatStr);
        }

        private void ApplyEffect()
        {
            float progress = Ease.Interpolate(Ease.Func.SineInOut, 0.0f, 1.0f, Time.time / 2.0f);

            Color32 rgb;

            switch (mEffect)
            {
                case RB.Effect.Scanlines:
                case RB.Effect.Noise:
                case RB.Effect.Negative:
                case RB.Effect.Pixelate:
                    RB.EffectSet(mEffect, progress);                    
                    mParamsText.Set(", ").Append(progress, 2);
                    break;

                case RB.Effect.Curvature:
                    RB.EffectSet(mEffect, progress, Vector2i.zero, new Color32(32, 32, 32, 255));
                    mParamsText.Set(", ").Append(progress, 2).Append(", new Color32(32, 32, 32, 255)");
                    break;

                case RB.Effect.Saturation:
                    float saturation = (progress * 2.0f) - 1.0f;
                    RB.EffectSet(mEffect, saturation);
                    mParamsText.Set(", ").Append(saturation, 2);
                    break;

                case RB.Effect.Shake:
                    RB.EffectSet(mEffect, progress * 0.1f);
                    mParamsText.Set(", ").Append(progress * 0.1f, 2);
                    break;

                case RB.Effect.Zoom:
                    RB.EffectSet(mEffect, (progress * 5.0f) + 0.5f);
                    mParamsText.Set(", ").Append((progress * 5.0f) + 0.5f, 2);
                    break;

                case RB.Effect.Slide:
                case RB.Effect.Wipe:
                    Vector2i slide = new Vector2i((int)(progress * RB.DisplaySize.width), 0);
                    RB.EffectSet(mEffect, slide);
                    mParamsText.Set(", new Vector2i(").Append(slide.x).Append(", ").Append(slide.y).Append(")");
                    break;

                case RB.Effect.Rotation:
                    RB.EffectSet(mEffect, progress * 360.0f);
                    mParamsText.Set(", ").Append(progress * 360.0f, 0);
                    break;

                case RB.Effect.ColorFade:
                    RB.EffectSet(mEffect, progress, Vector2i.zero, DemoUtil.IndexToRGB(20));
                    rgb = DemoUtil.IndexToRGB(20);
                    mParamsText.Set(", ").Append(progress, 2).Append(", Vector2i.zero, new Color32").Append(rgb);

                    break;

                case RB.Effect.ColorTint:
                    RB.EffectSet(mEffect, progress, Vector2i.zero, DemoUtil.IndexToRGB(31));
                    rgb = DemoUtil.IndexToRGB(31);
                    mParamsText.Set(", ").Append(progress, 2).Append(", Vector2i.zero, new Color32").Append(rgb);

                    break;

                case RB.Effect.Fizzle:
                    RB.EffectSet(mEffect, progress, Vector2i.zero, DemoUtil.IndexToRGB(11));
                    rgb = DemoUtil.IndexToRGB(11);
                    mParamsText.Set(", ").Append(progress, 2).Append(", Vector2i.zero, new Color32").Append(rgb);

                    break;

                case RB.Effect.Pinhole:
                case RB.Effect.InvertedPinhole:
                    Vector2i pos =
                        new Vector2i(
                            (int)((Mathf.Sin(progress * 8) * (RB.DisplaySize.width / 6.0f)) + (RB.DisplaySize.width / 6.0f)),
                            (int)((Mathf.Cos(progress * 8) * (RB.DisplaySize.width / 6.0f)) + (RB.DisplaySize.width / 6.0f)));

                    RB.EffectSet(mEffect, progress, pos, DemoUtil.IndexToRGB(0));
                    rgb = DemoUtil.IndexToRGB(0);
                    mParamsText.Set(", ").Append(progress, 2).Append(", new Vector2i").Append(pos).Append(", new Color32").Append(rgb);

                    break;

                case RB.Effect.ChromaticAberration:
                    Vector2i ca = new Vector2i(progress * 500, progress * 500);
                    RB.EffectSet(mEffect, ca);
                    mParamsText.Set(", new Vector2i(").Append(ca.x).Append(", ").Append(ca.y).Append(")");

                    break;
            }
        }
    }
}
