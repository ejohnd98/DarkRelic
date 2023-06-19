namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate tilemaps
    /// </summary>
    public class ScenePixelStyle : SceneDemo
    {
        private Vector2i mMapSize;
        private RB.PixelStyle mStyle;
        private TMXMapAsset mMap = new TMXMapAsset();
        private SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="style">Pixel style</param>
        public ScenePixelStyle(RB.PixelStyle style)
        {
            mStyle = style;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            return true;
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

            mMap.Load("DemoReel/Tilemap");

            if (mMap != null)
            {
                mMapSize = mMap.size;

                mMap.LoadLayer("Clouds", 0);
                mMap.LoadLayer("Decoration", 1);
                mMap.LoadLayer("Terrain", 2);

                RB.MapLayerSpriteSheetSet(0, mSpriteSheet1);
                RB.MapLayerSpriteSheetSet(1, mSpriteSheet1);
                RB.MapLayerSpriteSheetSet(2, mSpriteSheet1);
            }

            if (mStyle == RB.PixelStyle.Wide)
            {
                RB.DisplayModeSet(new Vector2i(640 / 2, 360), mStyle);
            }
            else
            {
                RB.DisplayModeSet(new Vector2i(640, 360 / 2), mStyle);
            }
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.MapClear();
            RB.DisplayModeSet(new Vector2i(640, 360), RB.PixelStyle.Square);
            mSpriteSheet1.Unload();
            mMap.Unload();

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
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            DrawTMX();
        }

         private void DrawTMX()
        {
            var demo = (DemoReel)RB.Game;

            int spriteFrame = (RB.Ticks % 40) > 20 ? 1 : 0;
            Rect2i clipRect = new Rect2i(0, 0, 0, 0);
            int cameraYOffset = 0;
            int cameraXOffset = 0;
            int cameraYRange = 16;

            if (mStyle == RB.PixelStyle.Wide)
            {
                clipRect = new Rect2i((RB.DisplaySize.width / 2) - 100, RB.DisplaySize.height - 220, 200, 200);
            }
            else if (mStyle == RB.PixelStyle.Tall)
            {
                clipRect = new Rect2i((RB.DisplaySize.width / 2) - 200, RB.DisplaySize.height - 110, 400, 100);
                cameraYOffset = 150;
                cameraXOffset = -120;
                cameraYRange = 8;
            }

            if (mMap != null)
            {
                int scrollPos = -(int)RB.Ticks % (mMapSize.width * RB.SpriteSheetGet().grid.cellSize.width);
                Vector2i cameraPos = new Vector2i((int)((Mathf.Sin(RB.Ticks / 100.0f) * 420) + 450 + cameraXOffset), (int)((Mathf.Sin(RB.Ticks / 10.0f) * cameraYRange) + cameraYOffset));

                RB.ClipSet(clipRect);
                RB.DrawRectFill(clipRect, DemoUtil.IndexToRGB(22));

                RB.CameraSet(cameraPos);
                RB.DrawMapLayer(0, new Vector2i(scrollPos, 0));
                RB.DrawMapLayer(0, new Vector2i(scrollPos + (mMapSize.width * RB.SpriteSheetGet().grid.cellSize.width), 0));
                RB.DrawMapLayer(1);
                RB.DrawMapLayer(2);
                RB.DrawSprite(0 + spriteFrame, new Vector2i(13 * 16, 16 * 16));
                RB.DrawSprite(RB.SpriteIndex(6, 10) + spriteFrame, new Vector2i(67 * 16, 14 * 16));
                RB.CameraReset();
            }
            else
            {
                RB.Print(new Vector2i(clipRect.x + 2, clipRect.y + 2), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map\nDemos/DemoReel/Tilemap.tmx in Unity");
            }

            RB.DrawRect(clipRect, DemoUtil.IndexToRGB(21));

            RB.ClipReset();

            mFormatStr.Set("@C// Use ").Append(mStyle == RB.PixelStyle.Wide ? "wide" : "tall").Append(" pixel format\n");
            mFormatStr.Append("@Kpublic @MHardwareSettings @NQueryHardware() {\n");
            mFormatStr.Append("   @Kvar @Nhw = @Knew @MHardwareSettings@N();\n");
            mFormatStr.Append("   @Nhw.DisplaySize = @Knew @MVector2i@N(@L").Append(RB.DisplaySize.width).Append("@N, @L").Append(RB.DisplaySize.height).Append("@N);\n");
            mFormatStr.Append("   @Nhw.PixelStyle = @KRetroBlit@N.@EPixelStyle@N.").Append(mStyle == RB.PixelStyle.Wide ? "Wide" : "Tall").Append(";\n");
            mFormatStr.Append("   @Kreturn @Nhw;\n");
            mFormatStr.Append("}");

            RB.Print(new Vector2i(4, 4), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
