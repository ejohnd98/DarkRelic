namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate post processing effects
    /// </summary>
    public class SceneEffectApply : SceneDemo
    {
        private readonly SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();
        private readonly ShaderAsset mShader1 = new ShaderAsset();
        private readonly TMXMapAsset mMap = new TMXMapAsset();

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

            if (mMap != null)
            {
                mMap.LoadLayer("Decoration", 0);
                mMap.LoadLayer("Terrain", 1);

                RB.MapLayerSpriteSheetSet(0, mSpriteSheet1);
                RB.MapLayerSpriteSheetSet(1, mSpriteSheet1);
            }

            mShader1.Load("DemoReel/PresentRippleShader");
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
            mShader1.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            RB.Clear(DemoUtil.IndexToRGB(22));

            int spriteFrame = ((int)RB.Ticks % 40) > 20 ? 1 : 0;

            if (mMap != null)
            {
                RB.DrawMapLayer(0);
                RB.DrawMapLayer(1);
            }

            RB.EffectSet(RB.Effect.Noise, 0.15f);
            RB.EffectSet(RB.Effect.Scanlines, 1.0f);
            RB.EffectSet(RB.Effect.Saturation, (Mathf.Sin(RB.Ticks / 50.0f) * 0.5f) + 0.5f);

            RB.EffectApplyNow();
            RB.EffectReset();

            if (mMap != null)
            {
                RB.DrawSprite(0 + spriteFrame, new Vector2i(13 * 16, 16 * 16));
            }
            else
            {
                RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/Tilemap.tmx in Unity");
            }

            mFormatStr.Set("@C// Specify when post-processing effects should be applied\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L0@N);\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L1@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.EffectSet(@MRB@N.@MEffect@N.Noise, @L0.15f@N);\n");
            mFormatStr.Append("@MRB@N.EffectSet(@MRB@N.@MEffect@N.Scanlines, @L1.0f@N);\n");
            mFormatStr.Append("@MRB@N.EffectSet(@MRB@N.@MEffect@N.Saturation, @L").Append((Mathf.Sin(RB.Ticks / 50.0f) * 0.5f) + 0.5f, 2).Append("f@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.EffectApplyNow();\n");
            mFormatStr.Append("@MRB@N.EffectReset();\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(0 + spriteFrame).Append("@N, @Knew@N Vector2i(@L").Append(13 * 16).Append("@N, @L").Append(16 * 16).Append("@N));");

            var size = RB.PrintMeasure(DemoUtil.HighlightCode(mFormatStr, mFinalStr));
            size.x += 4;
            size.y += 4;

            var rect = new Rect2i((RB.DisplaySize.width / 2) - (size.x / 2), (RB.DisplaySize.height / 2) - (size.y / 2), size.x, size.y);
            rect.y -= 64;

            RB.DrawRectFill(rect, DemoUtil.IndexToRGB(1));
            RB.DrawRect(rect, DemoUtil.IndexToRGB(4));
            RB.Print(new Vector2i(rect.x + 2, rect.y + 2), DemoUtil.IndexToRGB(0), mFinalStr);
        }
    }
}
