namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate clipping and offscreen rendering
    /// </summary>
    public class SceneClipOffscreen : SceneDemo
    {
        private string mText =
            "Lorem ipsum dolor sit amet, aenen ligula, elit\n" +
            "donec urna sit lectus, nonummy aliquam maecenas\n" +
            "aliquam nonummy, vulpute pellentesque ante. Est\n" +
            "mattis feugiat leo sem dolor, nunc rhoncus ornare\n" +
            "lectus morbi, pelesque blandit, apent phasellus.\n" +
            "Et ornare, sed odi impiet scelerisque urna, quis\n" +
            "porttitor posuer vestibu. Quisque primis ridicul\n" +
            "eget cras elit, amet facilisis, cras sed varius.\n" +
            "Alias velit fermetum quisque aliquet, nunc ante\n" +
            "sem quisque, mollis tortor quisque ultrices non\n" +
            "placerat, vitae vulutate, at nonummy quis mattis\n" +
            "odio. Duis aliquet purus lorem suspendise sit.\n" +
            "Duis mi lacus a non, mauris vitae proin turpis\n" +
            "quis maecenas, lacus felis inceptos ut aenean.";

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
            spriteSheet1.Load("DemoReel/Sprites");
            spriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));

            spriteSheet2.Create(RB.DisplaySize);

            RB.SpriteSheetSet(spriteSheet1);

            base.Enter();
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            spriteSheet1.Unload();
            spriteSheet2.Unload();

            base.Exit();
        }

        /// <summary>
        /// Handle scene exit
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

            int x = 4;
            int y = 4;

            DrawClip(x, y);
            DrawOffscreen(x + 320, y);
        }

        private void DrawClip(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            int clipWidth = (int)((Mathf.Sin(RB.Ticks / 50.0f) * 64) + 64 + 4);
            int clipHeight = (int)((Mathf.Sin((RB.Ticks / 50.0f) + 1) * 64) + 64 + 4);
            Rect2i clipRect = new Rect2i(x + 150 - clipWidth + 8, y + 212 - clipHeight + 8, clipWidth * 2, clipHeight * 2);

            RB.ClipSet(clipRect);

            RB.DrawRectFill(new Rect2i(0, 0, 500, 500), DemoUtil.IndexToRGB(22));

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int sx = (int)((Mathf.Sin((RB.Ticks / 100.0f) + (j * (Mathf.PI * 2 / 8)) + (RB.Ticks * 0.001f * i)) * i * 8) + 150 + x);
                    int sy = (int)((Mathf.Cos((RB.Ticks / 100.0f) + (j * (Mathf.PI * 2 / 8)) + (RB.Ticks * 0.001f * i)) * i * 8) + 212 + y);
                    RB.DrawSprite(3, new Vector2i(sx, sy));
                }
            }

            RB.ClipReset();

            mFormatStr.Set("@C// Set a clipping region\n");
            mFormatStr.Append("@MRB@N.ClipSet(@Knew @MRect2i@N(@L").Append(clipRect.x).Append("@N, @L").Append(clipRect.y).Append("@N, @L").Append(clipRect.width).Append("@N, @L").Append(clipRect.height).Append("@N));\n");
            mFormatStr.Append("@MRB@N.DrawRectFill@N(@Knew @MRect2i@N(@L0@N, @L0@N, @L500@N, @L500@N),\n");
            mFormatStr.Append("  @I22);\n");
            mFormatStr.Append("@Kfor @N(@Kint@N i = @L0@N; i < @L20@N; i++) {\n");
            mFormatStr.Append("  @Kfor @N(@Kint@N j = @L0@N; j < @L8@N; j++) {\n");
            mFormatStr.Append("    @MRB@N.DrawSprite(@L3@N, @Knew @MVector2i@N(\n");
            mFormatStr.Append("      ((@Kint@N)(@MMathf@N.Sin(@L").Append(RB.Ticks / 100.0f, 2).Append("f@N + j * (@MMathf@N.PI * @L2@N / @L8@N) + (@L").Append(RB.Ticks * 0.001f, 3).Append("f@N * i)) * i * @L8@N)@N),\n");
            mFormatStr.Append("      ((@Kint@N)(@MMathf@N.Cos(@L").Append(RB.Ticks / 100.0f, 2).Append("f@N + j * (@MMathf@N.PI * @L2@N / @L8@N) + (@L").Append(RB.Ticks * 0.001f, 3).Append("f@N * i)) * i * @L8@N)@N)));\n");
            mFormatStr.Append("  }\n");
            mFormatStr.Append("}");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawOffscreen(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            Vector2i textSize = RB.PrintMeasure(mText);
            RB.Offscreen(spriteSheet2);
            RB.Clear(new Color32(0, 0, 0, 0));

            RB.DrawNineSlice(new Rect2i(0, 0, textSize.width + 12, textSize.height + 12), new Rect2i(80, 0, 8, 8), new Rect2i(88, 0, 8, 8), new Rect2i(96, 0, 16, 16));

            RB.Print(new Vector2i(6, 6), DemoUtil.IndexToRGB(23), mText);

            RB.Onscreen();
            RB.SpriteSheetSet(spriteSheet2);
            RB.DrawCopy(new Rect2i(0, 0, textSize.width + 12, textSize.height + 12), new Rect2i(x, y + 165, textSize.width + 12, textSize.height + 12), new Vector2i((textSize.width + 12) / 2, (textSize.height + 12) / 2), RB.Ticks);
            RB.SpriteSheetSet(spriteSheet1);

            mFormatStr.Set("@C// Draw to offscreen surface and copy to screen\n");
            mFormatStr.Append("@MVector2i @Nsize = @MRB@N.PrintMeasure(textStr);\n");
            mFormatStr.Append("@MRB@N.Offscreen(myOffscreenSurface);\n");
            mFormatStr.Append("@MRB@N.DrawNineSlice(@Knew @MRect2i@N(@L0@N, @L0@N, size.width + @L12@N, size.height + @L12@N),\n");
            mFormatStr.Append("  @Knew @MRect2i@N(@L80@N, @L0@N, @L8@N, @L8@N), @Knew @MRect2i@N(@L88@N, @L0@N, @L8@N, @L8@N),\n");
            mFormatStr.Append("  @Knew @MRect2i@N(@L96@N, @L0@N, @L16@N, @L16@N));\n");
            mFormatStr.Append("@MRB@N.Print(@Knew @MVector2i@N(@L4@N, @L4@N), @I22@N, textStr);\n");
            mFormatStr.Append("@MRB@N.Onscreen();\n");
            mFormatStr.Append("@MRB@N.SpriteSheetSet(myOffscreenSurface);\n");
            mFormatStr.Append("@MRB@N.DrawCopy(\n");
            mFormatStr.Append("  @Knew@M Rect2i@N(@L0@N, @L0@N, size.width + @L7@N, size.height + @L8@N),\n");
            mFormatStr.Append("  @Knew@M Rect2i@N(@L").Append(x).Append("@N, @L").Append(y + 165).Append("@N, size.width + @L8@N, size.height + @L8@N),\n");
            mFormatStr.Append("  @Knew@M Vector2i@N(size.width / @L2@N, size.height / @L2@N), @L").Append(RB.Ticks % 360).Append("@N);\n");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
