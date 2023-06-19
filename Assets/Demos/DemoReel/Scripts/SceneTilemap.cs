namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate tilemaps
    /// </summary>
    public class SceneTilemap : SceneDemo
    {
        private TMXMapAsset mMap = new TMXMapAsset();
        private SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();

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

            RB.MapLayerSpriteSheetSet(0, mSpriteSheet1);

            mMap.LoadLayer("Clouds", 1, mSpriteSheet1);
            mMap.LoadLayer("Decoration", 2, mSpriteSheet1);
            mMap.LoadLayer("Terrain", 3, mSpriteSheet1);
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
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
            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            DrawSetTile(4, 4);
            DrawTMX(286, 4);
        }

        private void DrawSetTile(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            int flags = Random.Range(0, 2) == 0 ? 0 : RB.FLIP_V;
            Rect2i clipRect = new Rect2i(x + 16, y + 82, 250, 268);
            Vector2i cameraPos = new Vector2i((int)(Mathf.Sin(RB.Ticks / 50.0f) * 128) + 128, (int)(Mathf.Sin(RB.Ticks / 10.0f) * 16) + 16);
            string flagsStr = string.Empty;

            if (flags != 0)
            {
                flagsStr = ", @MRB@N.FLIP_V";
            }

            RB.MapSpriteSet(0, new Vector2i(Random.Range(0, 38), Random.Range(0, 26)), Random.Range(RB.SpriteIndex(0, 4), RB.SpriteIndex(4, 4)), Color.white, flags);

            RB.ClipSet(clipRect);

            RB.DrawRectFill(clipRect, DemoUtil.IndexToRGB(22));

            RB.CameraSet(cameraPos);
            RB.DrawMapLayer(0);
            RB.CameraReset();
            RB.DrawRect(clipRect, DemoUtil.IndexToRGB(21));

            RB.ClipReset();

            mFormatStr.Set("@C// Set specific tile at specific map layer, sprite flags\n");
            mFormatStr.Append("@MRB@N.MapSpriteSet(@L0@N, @Knew @MVector2i@N(@L").Append(Random.Range(0, 60)).Append("@N, @L").Append(Random.Range(0, 12)).Append("@N), @L").Append(Random.Range(RB.SpriteIndex(0, 4), RB.SpriteIndex(4, 4))).Append(", 0").Append(flagsStr).Append("@N);\n");
            mFormatStr.Append("@MRB@N.ClipSet(@Knew@N @MRect2i@N(@L").Append(clipRect.x).Append("@N, @L").Append(clipRect.y).Append("@N, @L").Append(clipRect.width).Append("@N, @L").Append(clipRect.height).Append("@N));\n");
            mFormatStr.Append("@MRB@N.DrawRectFill(@Knew @MRect2i@N(@L").Append(clipRect.x).Append("@N, @L").Append(clipRect.y).Append("@N, @L").Append(clipRect.width).Append("@N, @L").Append(clipRect.height).Append("@N),\n   @I22);\n");
            mFormatStr.Append("@MRB@N.CameraSet(@Knew@N @MVector2i@N(@L").Append(cameraPos.x).Append("@N, @L").Append(cameraPos.y).Append("@N));\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L0@N);\n");
            mFormatStr.Append("@MRB@N.CameraReset();\n");
            mFormatStr.Append("@MRB@N.DrawRect(@Knew @MRect2i@N(@L").Append(clipRect.x).Append("@N, @L").Append(clipRect.y).Append("@N, @L").Append(clipRect.width).Append("@N, @L").Append(clipRect.height).Append("@N),\n");
            mFormatStr.Append("   @I21);");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawTMX(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            Rect2i clipRect = new Rect2i(x + 16, y + 142, 310, 207);
            Vector2i cameraPos = new Vector2i((int)(Mathf.Sin(RB.Ticks / 100.0f) * 450) + 200, (int)Mathf.Sin(RB.Ticks / 10.0f) * 16);
            int spriteFrame = (RB.Ticks % 40) > 20 ? 1 : 0;

            if (mMap != null && mMap.status == RB.AssetStatus.Ready)
            {
                int scrollPos = -(int)RB.Ticks % (mMap.size.width * RB.SpriteSheetGet().grid.cellSize.width);

                RB.ClipSet(clipRect);
                RB.DrawRectFill(clipRect, DemoUtil.IndexToRGB(22));

                RB.CameraSet(cameraPos);

                RB.DrawMapLayer(1, new Vector2i(scrollPos, 0));
                RB.DrawMapLayer(1, new Vector2i(scrollPos + (mMap.size.width * RB.SpriteSheetGet().grid.cellSize.width), 0));
                RB.DrawMapLayer(2);
                RB.DrawMapLayer(3);
                RB.DrawSprite(0 + spriteFrame, new Vector2i(13 * 16, 16 * 16));
                RB.DrawSprite(RB.SpriteIndex(6, 10) + spriteFrame, new Vector2i(67 * 16, 14 * 16));
            }
            else
            {
                RB.CameraReset();
                RB.ClipReset();
                RB.Print(new Vector2i(x + 18, y + 144), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/Tilemap.tmx in Unity");
            }

            RB.CameraReset();
            RB.ClipReset();
            RB.DrawRect(clipRect, DemoUtil.IndexToRGB(21));

            mMap.Load("DemoReel/Tilemap");

            mFormatStr.Set("@C// Load a map from a TMX file\n");
            mFormatStr.Append("@NmyTMXMap.Load(@S\"DemoReel/Tilemap\"@N);\n");
            mFormatStr.Append("@NmyTMXMap.LoadLayer(@S\"Clouds\"@N, @L1@N);\n");
            mFormatStr.Append("@NmyTMXMap.LoadLayer(@S\"Decoration\"@N, @L2@N);\n");
            mFormatStr.Append("@NmyTMXMap.LoadLayer(@S\"Terrain\"@N, @L3@N);\n");
            mFormatStr.Append("@MRB@N.ClipSet(@Knew @MRect2i(@L").Append(clipRect.x).Append("@N, @L").Append(clipRect.y).Append("@N, @L").Append(clipRect.width).Append("@N, @L").Append(clipRect.height).Append("@N));\n");
            mFormatStr.Append("@MRB@N.DrawRectFill(@Knew@N @MRect2i@N(@L").Append(clipRect.x).Append("@N, @L").Append(clipRect.y).Append("@N, @L").Append(clipRect.width).Append("@N, @L").Append(clipRect.height).Append("@N), @I22);\n");
            mFormatStr.Append("@MRB@N.CameraSet(@Knew @MVector2i@N(@L").Append(cameraPos.x).Append("@N, @L").Append(cameraPos.y).Append("@N));\n");
            mFormatStr.Append("@Kint@N pos = @L").Append(-(int)RB.Ticks).Append("@N % (myMap.size.width * @MRB@N.SpriteSize.width);\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L1@N, @Knew @MVector2i@N(pos, @L0@N));\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L1@N, @Knew @MVector2i@N(pos + (myMap.size.width * @MRB@N.SpriteSize.width)), @L0@N));\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L2@N);\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L3@N);\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(0 + spriteFrame).Append("@N, @Knew@N @MVector2i@N(@L").Append(13 * 16).Append("@N, @L").Append(16 * 16).Append("@N));\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(RB.SpriteIndex(6, 10) + spriteFrame).Append("@N, @Knew @MVector2i@N(@L").Append(67 * 16).Append("@N, @L").Append(14 * 16).Append("@N));\n");
            mFormatStr.Append("@MRB@N.CameraReset();\n");
            mFormatStr.Append("@MRB@N.DrawRect(@Knew @MRect2i(@L").Append(clipRect.x).Append("@N, @L").Append(clipRect.y).Append("@N, @L").Append(clipRect.width).Append("@N, @L").Append(clipRect.height).Append("@N), @I21);\n");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
