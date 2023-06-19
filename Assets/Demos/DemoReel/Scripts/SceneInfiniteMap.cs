namespace RetroBlitDemoReel
{
    /// <summary>
    /// Demonstrate loading and rendering of infinite Tiled maps
    /// </summary>
    public class SceneInfiniteMap : SceneDemo
    {
        private TMXMapAsset mMap = new TMXMapAsset();
        private int mWaveOffset;
        private Vector2i mCameraPos = new Vector2i(0, 0);
        private Vector2i mChunkCameraPos = new Vector2i(0, 0);

        private Rect2i mClipRect;

        private Vector2i mTopLeftChunk;

        private SpriteSheetAsset mSpriteSheet1 = new SpriteSheetAsset();
        private SpriteSheetAsset mSpriteSheet2 = new SpriteSheetAsset();

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

            mSpriteSheet1.Load("DemoReel/SpritesSmall");
            mSpriteSheet1.grid = new SpriteGrid(new Vector2i(8, 8));

            mSpriteSheet2.Load("DemoReel/WaterSmall");
            mSpriteSheet2.grid = new SpriteGrid(new Vector2i(8, 8));

            mCameraPos = Vector2i.zero;
            mChunkCameraPos = Vector2i.zero;
            mTopLeftChunk = new Vector2i(-100000, 100000);

            RB.MapClear();
            mMap.Load("DemoReel/TilemapInfinite");
            RB.MapLayerSpriteSheetSet(0, mSpriteSheet1);

            mClipRect = new Rect2i(16, 4 + 219, 400, (16 * 8) + 2);
            mCameraPos = Vector2i.zero;
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.MapClear();

            mSpriteSheet1.Unload();
            mSpriteSheet2.Unload();
            mMap.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            mCameraPos.x++;

            base.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            DrawAll(4, 4);
            DrawCode(4, 4);
        }

        private void DrawTilemap(int x, int y)
        {
            if (mMap == null)
            {
                RB.Print(new Vector2i(x + 2, y + 2), DemoUtil.IndexToRGB(14), "Failed to load TMX map.\nPlease try re-importing the map Demos/DemoReel/TilemapInfinite.tmx in Unity");
                return;
            }

            if (mMap.status != RB.AssetStatus.Ready)
            {
                return;
            }

            var chunkPixelSize = new Vector2i(RB.MapChunkSize.width * mSpriteSheet1.grid.cellSize.width, RB.MapChunkSize.height * mSpriteSheet1.grid.cellSize.height);

            // Figure out which map chunk is in the top left corner of the camera view
            var newTopLeftChunk = new Vector2i(mCameraPos.x / chunkPixelSize.width, mCameraPos.y / chunkPixelSize.height);

            if (newTopLeftChunk != mTopLeftChunk)
            {
                var shift = mTopLeftChunk - newTopLeftChunk;
                RB.MapShiftChunks(0, shift);

                for (int cy = 0; cy <= (mClipRect.height / chunkPixelSize.height) + 1; cy++)
                {
                    for (int cx = 0; cx <= (mClipRect.width / chunkPixelSize.width) + 1; cx++)
                    {
                        var chunkPos = new Vector2i(cx * RB.MapChunkSize.x, cy * RB.MapChunkSize.y);
                        var mapPos = new Vector2i(newTopLeftChunk.x * RB.MapChunkSize.x, newTopLeftChunk.y * RB.MapChunkSize.y) + chunkPos;
                        mapPos.x = mapPos.x % mMap.size.width;

                        if (RB.MapChunkEmpty(0, chunkPos))
                        {
                            mMap.LoadLayerChunk("Terrain", 0, mapPos, chunkPos);
                        }
                    }
                }

                mTopLeftChunk = newTopLeftChunk;
            }

            mChunkCameraPos = new Vector2i(mCameraPos.x % chunkPixelSize.width, mCameraPos.y % chunkPixelSize.height);

            RB.CameraSet(mChunkCameraPos);
            RB.DrawMapLayer(0, new Vector2i(x + 1, y + 1));
            RB.CameraReset();
        }

        private void DrawAll(int x, int y)
        {
            var demo = (DemoReel)RB.Game;
            var gridColor = DemoUtil.IndexToRGB(14);

            RB.Offscreen(mSpriteSheet1);
            RB.SpriteSheetSet(mSpriteSheet2);
            mWaveOffset = (int)((RB.Ticks / 2) % 8);
            RB.DrawCopy(new Rect2i(mWaveOffset, 0, RB.SpriteSheetGet().grid.cellSize), new Vector2i(24, 8));
            RB.Onscreen();

            Rect2i clipRectOverlap = mClipRect;
            clipRectOverlap.width += 400;

            if (mMap != null)
            {
                RB.DrawRectFill(mClipRect, DemoUtil.IndexToRGB(22));
            }

            RB.ClipSet(clipRectOverlap);
            DrawTilemap(mClipRect.x, mClipRect.y);
            RB.ClipReset();

            RB.CameraReset();

            // Blank out right side
            RB.AlphaSet(196);
            RB.DrawRectFill(new Rect2i(mClipRect.x + mClipRect.width, mClipRect.y, 300, mClipRect.height), DemoUtil.IndexToRGB(1));
            RB.AlphaSet(255);

            // Blank out left side
            RB.DrawRectFill(new Rect2i(0, mClipRect.y, mClipRect.x - 1, mClipRect.height), DemoUtil.IndexToRGB(1));

            RB.DrawRect(mClipRect, DemoUtil.IndexToRGB(7));

            if (mMap == null)
            {
                return;
            }

            RB.AlphaSet(128);

            mFinalStr.Set("Chunk Tile Offset:");
            RB.Print(new Vector2i(mClipRect.x, mClipRect.y - 16), gridColor, mFinalStr);

            RB.CameraSet(mChunkCameraPos - new Vector2i(mClipRect));

            int gxStart = 0;
            int gxEnd = gxStart + (RB.DisplaySize.width * 2);
            for (int gx = gxStart; gx < gxEnd; gx += RB.MapChunkSize.width * RB.SpriteSheetGet().grid.cellSize.width)
            {
                RB.DrawLine(new Vector2i(gx, -8), new Vector2i(gx, mClipRect.height + 4), gridColor);

                mFinalStr.Set(gx / RB.SpriteSheetGet().grid.cellSize.width);
                RB.Print(new Vector2i(gx + 3, -8), gridColor, mFinalStr);
            }

            RB.AlphaSet(255);

            RB.CameraReset();

            RB.SpriteSheetSet(mSpriteSheet1);
        }

        private void DrawCode(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            mFormatStr.Set("@C// Load infinite TMX maps chunk by chunk to create maps of any size!@N\n");
            mFormatStr.Append("@Kvar@N chunkPixelSize = @Knew@N @MVector2i@N(@MRB@N.MapChunkSize.width * @MRB@N.SpriteSize(@L0@N).width, @MRB@N.MapChunkSize.height * @MRB@N.SpriteSize(@L0@N).height);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Figure out which map chunk is in the top left corner of the camera view@N\n");
            mFormatStr.Append("@Kvar@N newTopLeftChunk = @Knew@N @MVector2i@N(mCameraPos.x / chunkPixelSize.width, mCameraPos.y / chunkPixelSize.height);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@C// Check if the camera moved enough so that the top left chunk has changed, if so then we need to shift the map and load a new chunk.@N\n");
            mFormatStr.Append("@Kif@N (newTopLeftChunk != mTopLeftChunk) {\n");
            mFormatStr.Append("    @MRB@N.MapShiftChunks(@L0@N, mTopLeftChunk - newTopLeftChunk);\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("    @C// After shifting loop through all potentially visible chunks and load any that are now empty.@N\n");
            mFormatStr.Append("    @Kfor@N (@Kint@N cx = @L0@N; cx <= @MRB@N.DisplaySize.width / chunkPixelSize.width + @L1@N; cx++) {\n");
            mFormatStr.Append("        @Kvar@N chunkPos = @Knew@N @MVector2i@N(cx * RB.MapChunkSize.x, cy * RB.MapChunkSize.y);\n");
            mFormatStr.Append("        @Kvar@N mapPos = @Knew@N @MVector2i@N(newTopLeftChunk.x * RB.MapChunkSize.x, newTopLeftChunk.y * RB.MapChunkSize.y) + chunkPos;\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("        @Kif@N (@MRB@N.MapChunkEmpty(@L0@N, chunkPos)) {\n");
            mFormatStr.Append("            @NmyMap.LoadLayerChunk(@S\"Terrain\"@N, @L0@N, mapPos, chunkPos);\n");
            mFormatStr.Append("        }\n");
            mFormatStr.Append("    }\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("    mTopLeftChunk = newTopLeftChunk;\n");
            mFormatStr.Append("}\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@MRB@N.CameraSet(@Knew@N @MVector2i@N(mCameraPos.x % chunkPixelSize.width, mCameraPos.y % chunkPixelSize.height));\n");
            mFormatStr.Append("@MRB@N.DrawMapLayer(@L0@N);\n");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
