namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing to spritesheets and how that can affect tilemaps
    /// </summary>
    public class SceneSpriteSheetDraw : SceneDemo
    {
        private TMXMapAsset mMap = new TMXMapAsset();
        private int mWaveOffset;
        private int mFishFrame;

        private SpriteSheetAsset spriteSheet1 = new SpriteSheetAsset();
        private SpriteSheetAsset spriteSheet2 = new SpriteSheetAsset();

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

            spriteSheet1.Load("DemoReel/Sprites");
            spriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));

            spriteSheet2.Load("DemoReel/Water");
            spriteSheet2.grid = new SpriteGrid(new Vector2i(16, 16));

            mMap.Load("DemoReel/TilemapOcean");

            if (mMap != null)
            {
                mMap.LoadLayer("Clouds", 1);
                mMap.LoadLayer("Terrain", 2);
                mMap.LoadLayer("Fish", 3);

                RB.MapLayerSpriteSheetSet(1, spriteSheet1);
                RB.MapLayerSpriteSheetSet(2, spriteSheet1);
                RB.MapLayerSpriteSheetSet(3, spriteSheet1);
            }
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.MapClear();

            mMap.Unload();
            spriteSheet1.Unload();
            spriteSheet2.Unload();

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

            DrawTilemap(4, 4);
            DrawCode(4, 4);
            DrawSpriteSheet(516, 12);
        }

        private void DrawTilemap(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.Offscreen(spriteSheet1);
            RB.SpriteSheetSet(spriteSheet2);
            mWaveOffset = (int)((RB.Ticks / 2) % 16);
            mFishFrame = (int)((RB.Ticks / 6) % 32);
            RB.DrawCopy(new Rect2i(mWaveOffset, 0, RB.SpriteSheetGet().grid.cellSize), new Vector2i(48, 192));
            RB.Clear(new Color32(0, 0, 0, 0), new Rect2i(80, 192, RB.SpriteSheetGet().grid.cellSize));
            if (mFishFrame < 7)
            {
                RB.DrawCopy(new Rect2i(mFishFrame * RB.SpriteSheetGet().grid.cellSize.width, RB.SpriteSheetGet().grid.cellSize.height, RB.SpriteSheetGet().grid.cellSize), new Vector2i(80, 192));
            }

            RB.Onscreen();

            Rect2i clipRect = new Rect2i(x, y + (RB.DisplaySize.height / 2) - 8, 632, 180);

            if (mMap != null)
            {
                RB.ClipSet(clipRect);
                RB.DrawRectFill(clipRect, DemoUtil.IndexToRGB(22));

                RB.CameraSet(new Vector2i(0, 0));
                RB.DrawMapLayer(2);
                RB.DrawMapLayer(3);

                RB.TintColorSet(Color.black);
                RB.AlphaSet(32);
                int scrollPos = -(int)RB.Ticks % (mMap.size.width * RB.SpriteSheetGet().grid.cellSize.width);
                RB.DrawMapLayer(1, new Vector2i(scrollPos + 8, 8));
                RB.DrawMapLayer(1, new Vector2i(scrollPos + (mMap.size.width * RB.SpriteSheetGet().grid.cellSize.width) + 8, 8));
                RB.TintColorSet(Color.white);

                RB.AlphaSet(196);
                RB.DrawMapLayer(1, new Vector2i(scrollPos, 0));
                RB.DrawMapLayer(1, new Vector2i(scrollPos + (mMap.size.width * RB.SpriteSheetGet().grid.cellSize.width), 0));
                RB.AlphaSet(255);

                RB.CameraReset();

                RB.ClipReset();
                RB.SpriteSheetSet(spriteSheet1);
            }
            else
            {
                RB.Print(new Vector2i(clipRect.x + 2, clipRect.y + 2), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/TilemapOcean.tmx in Unity");
            }

            RB.DrawRect(clipRect, DemoUtil.IndexToRGB(21));
        }

        private void DrawCode(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            mFormatStr.Set("@C// Draw into a Sprite Sheet to create simple and efficient tilemap animations!@N\n");
            mFormatStr.Append("@MRB@N.Offscreen(myTilemapSpriteSheet);\n");
            mFormatStr.Append("@MRB@N.SpriteSheetSet(myAnimationSpriteSheet);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Copy water animation@N\n");
            mFormatStr.Append("@MRB@N.DrawCopy(@Knew@N @MRect2i@N(@L").Append(mWaveOffset).Append("@N, @L0@N, @MRB@N.SpriteSize()), @Knew@N @MVector2i@N(@L48@N, @L192@N));\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Clear fish frame@N\n");
            mFormatStr.Append("@MRB@N.Clear(@Knew@N @MColor32@N(@L0@N, @L0@N, @L0@N, @L0@N), @Knew@N @MRect2i@N(@L80@N, @L192@N, @MRB@N.SpriteSize()));\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Copy fish animation@N\n");

            if (mFishFrame < 7)
            {
                mFormatStr.Append("@MRB@N.DrawCopy(@Knew@N @MRect2i@N(@L").Append(mFishFrame * RB.SpriteSheetGet().grid.cellSize.width).Append("@N, @L");
                mFormatStr.Append(RB.SpriteSheetGet().grid.cellSize.height).Append("@N, @MRB@N.SpriteSize()), @Knew@N @MVector2i@N(@L80@N, @L192@N));\n");
            }
            else
            {
                mFormatStr.Append("@MRB@N.DrawCopy(@MRect2i@N.zero, @MVector2i@N.zero);\n");
            }

            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.Onscreen();\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Draw terrain and water@N\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L0@N);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Draw fish@N\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L1@N);\n");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawSpriteSheet(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            Rect2i ss0Rect = new Rect2i(0, 0, 16 * 7, 16 * 6);
            Rect2i ss1Rect = new Rect2i(0, ss0Rect.y + ss0Rect.height + 16, 16 * 7, 16 * 2);
            Rect2i copyRectWater = new Rect2i(mWaveOffset + ss1Rect.x - 1, ss1Rect.y - 1, 16 + 2, 16 + 2);
            Rect2i destRectWater = new Rect2i(ss0Rect.x + (3 * 16), ss0Rect.y + (4 * 16), 16, 16);
            Rect2i copyRectFish = new Rect2i((mFishFrame * 16) + ss1Rect.x - 1, ss1Rect.y - 1 + 16, 16 + 2, 16 + 2);
            Rect2i destRectFish = new Rect2i(ss0Rect.x + (5 * 16), ss0Rect.y + (4 * 16), 16, 16);

            RB.CameraSet(new Vector2i(-x, -y));

            RB.Print(new Vector2i(ss0Rect.x, ss0Rect.y + ss0Rect.height + 2), Color.gray, "myTilemapSpriteSheet");
            RB.Print(new Vector2i(ss0Rect.x, ss1Rect.y + ss1Rect.height + 2), Color.gray, "myAnimationSpriteSheet");

            RB.SpriteSheetSet(spriteSheet1);

            // Draw alpha grid for sprite sheet 0
            for (int gx = 0; gx < ss0Rect.width; gx += 16)
            {
                for (int gy = 0; gy < ss0Rect.height; gy += 16)
                {
                    RB.DrawSprite(RB.SpriteIndex(7, 0), new Vector2i(gx + ss0Rect.x, gy + ss0Rect.y));
                }
            }

            // Draw alpha grid for sprite sheet 1
            for (int gx = 0; gx < ss1Rect.width; gx += 16)
            {
                for (int gy = 0; gy < ss1Rect.height; gy += 16)
                {
                    RB.DrawSprite(RB.SpriteIndex(7, 0), new Vector2i(gx + ss1Rect.x, gy + ss1Rect.y));
                }
            }

            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawCopy(new Rect2i(0, 0, RB.SpriteSheetGet().sheetSize), new Vector2i(ss1Rect.x, ss1Rect.y));
            RB.SpriteSheetSet(spriteSheet1);

            int color = 7;

            RB.DrawCopy(new Rect2i(0, 16 * 8, ss0Rect.width, ss0Rect.height), new Vector2i(ss0Rect.x, ss0Rect.y));

            // Water copy rects
            RB.DrawRect(copyRectWater, DemoUtil.IndexToRGB(color));
            destRectWater.Expand(1);
            RB.DrawRect(destRectWater, DemoUtil.IndexToRGB(color));
            RB.DrawLine(
                new Vector2i(copyRectWater.x + (copyRectWater.width / 2), copyRectWater.y),
                new Vector2i(destRectWater.x + 8, destRectWater.y + 18),
                DemoUtil.IndexToRGB(color));

            // Fish copy rects
            if (mFishFrame < 7)
            {
                RB.DrawRect(copyRectFish, DemoUtil.IndexToRGB(color));
                destRectFish.Expand(1);
                RB.DrawRect(destRectFish, DemoUtil.IndexToRGB(color));
                RB.DrawLine(
                    new Vector2i(copyRectFish.x + (copyRectFish.width / 2), copyRectFish.y),
                    new Vector2i(destRectFish.x + 8, destRectFish.y + 18),
                    DemoUtil.IndexToRGB(color));
            }

            RB.CameraReset();
        }
    }
}
