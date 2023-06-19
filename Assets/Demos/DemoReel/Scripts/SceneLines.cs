namespace RetroBlitDemoReel
{
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneLines : SceneDemo
    {
        private readonly SpriteSheetAsset spriteSheet1 = new SpriteSheetAsset();
        private readonly int mMaxLineWidth = 250;

        private float mScroll = 0.0f;
        private int mThickness = 1;
        private float mWiggleScale = 1.0f;

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

            RB.SpriteSheetSet(spriteSheet1);
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            spriteSheet1.Unload();

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
            RB.Clear(DemoUtil.IndexToRGB(1));

            mThickness = (int)((Mathf.Sin(RB.Ticks * 0.02f) * 8) + 8) + 1;
            mScroll += Mathf.Sin(RB.Ticks * 0.01f) * 0.2f;

            mWiggleScale = ((Mathf.Sin(RB.Ticks * 0.015f) * 1.0f) + 1.0f) / 2.0f;

            int x = 4;
            int y = 5;

            int height = 50;
            int spacing = 60;

            DrawThinSolidLines(x, y, height);

            y += spacing;

            DrawThickSolidLines(x, y, height);

            y += spacing;

            DrawThickTexturedLines(x, y, height);

            y += spacing;

            // Do repeat texture with fixed amount of repeats
            DrawThinTexturedLinesFixedRepeat(x, y, height);

            y += spacing;

            // Do repeat texture with variable amount of repeats based on length
            DrawThinTexturedLinesVariableRepeat(x, y, height);

            y += spacing;

            DrawThickTexturedRepeatingLines(x, y, height);
        }

        private void DrawAlphaGrid(int x, int y, int cellSize, int width, int height)
        {
            int xStart = 0;
            int xEnd = xStart + width + cellSize;

            int yStart = 0;
            int yEnd = yStart + height + cellSize;

            bool darkCell;
            int row = 0;

            byte darkColor = 35;
            byte lightColor = 45;

            RB.ClipSet(new Rect2i(x, y, width, height));

            for (y = yStart; y < yEnd; y += cellSize)
            {
                darkCell = (row % 2) != 0;
                row++;

                for (x = xStart; x < xEnd; x += cellSize)
                {
                    RB.DrawRectFill(new Rect2i(x, y, cellSize, cellSize), darkCell ? new Color32(darkColor, darkColor, darkColor, 255) : new Color32(lightColor, lightColor, lightColor, 255));
                    darkCell = !darkCell;
                }
            }

            RB.ClipReset();
        }

        private void DrawThinSolidLines(int x, int y, int height)
        {
            RB.CameraSet(new Vector2i(-x, -y));

            DrawAlphaGrid(x, y, 10, mMaxLineWidth, height);

            RB.CameraSet(new Vector2i(-x, -y - (height / 2)));

            int yOffset = (int)(Mathf.Sin(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineStart = new Vector2i(0, yOffset * mWiggleScale);

            yOffset = (int)(Mathf.Cos(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineEnd = new Vector2i(lineStart.x + mMaxLineWidth - 1, yOffset * mWiggleScale);

            Color color = Color.white;
            color.r = ((Mathf.Cos(RB.Ticks * 0.03f) + 1.0f) / 2.0f) + 0.2f;
            color.g = ((Mathf.Cos(RB.Ticks * 0.04f) + 1.0f) / 2.0f) + 0.2f;
            color.b = ((Mathf.Sin(RB.Ticks * 0.06f) + 1.0f) / 2.0f) + 0.2f;

            RB.DrawLine(lineStart, lineEnd, color);

            RB.CameraSet(new Vector2i(-(x + mMaxLineWidth + 10 + 80), -y));

            RB.DrawRectFill(new Rect2i(0, 0, 292, height), new Color32(35, 35, 35, 255));

            mFormatStr.Clear();
            mFormatStr.Set("@C// Draw a perfect 1 pixel thick line\n");
            mFormatStr.Append("@Kvar@N p0 = @Knew@N @MVector2i@N(@L").Append(lineStart.x).Append("@N, @L").Append(lineStart.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N p1 = @Knew@N @MVector2i@N(@L").Append(lineEnd.x).Append("@N, @L").Append(lineEnd.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N color = @Knew@N @MColor32@N(@L").Append((int)(color.r * 255)).Append("@N, @L").Append((int)(color.g * 255)).Append("@N, @L").Append((int)(color.b * 255)).Append("@N, @L").Append((int)(color.a * 255)).Append("@N);\n");
            mFormatStr.Append("@MRB@N.DrawLine(p0, p1, color);");
            RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraReset();
        }

        private void DrawThickSolidLines(int x, int y, int height)
        {
            RB.CameraSet(new Vector2i(-x, -y));

            DrawAlphaGrid(x, y, 10, mMaxLineWidth, height);

            RB.CameraSet(new Vector2i(-x, -y - (height / 2)));

            int yOffset = (int)(Mathf.Sin(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineStart = new Vector2i(0, yOffset * mWiggleScale);

            yOffset = (int)(Mathf.Cos(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineEnd = new Vector2i(lineStart.x + mMaxLineWidth, yOffset * mWiggleScale);

            Color color = Color.white;
            color.r = ((Mathf.Cos(RB.Ticks * 0.03f) + 1.0f) / 2.0f) + 0.2f;
            color.g = ((Mathf.Cos(RB.Ticks * 0.04f) + 1.0f) / 2.0f) + 0.2f;
            color.b = ((Mathf.Sin(RB.Ticks * 0.06f) + 1.0f) / 2.0f) + 0.2f;

            RB.DrawLine(lineStart, lineEnd, mThickness, color);

            RB.CameraSet(new Vector2i(-(x + mMaxLineWidth + 10 + 80), -y));

            RB.DrawRectFill(new Rect2i(0, 0, 292, height), new Color32(35, 35, 35, 255));

            mFormatStr.Clear();
            mFormatStr.Set("@C// Draw a line with variable thickness\n");
            mFormatStr.Append("@Kvar@N p0 = @Knew@N @MVector2i@N(@L").Append(lineStart.x).Append("@N, @L").Append(lineStart.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N p1 = @Knew@N @MVector2i@N(@L").Append(lineEnd.x).Append("@N, @L").Append(lineEnd.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N color = @Knew@N @MColor32@N(@L").Append((int)(color.r * 255)).Append("@N, @L").Append((int)(color.g * 255)).Append("@N, @L").Append((int)(color.b * 255)).Append("@N, @L").Append((int)(color.a * 255)).Append("@N);\n");
            mFormatStr.Append("@MRB@N.DrawLine(p0, p1, @L").Append(mThickness).Append("@N, color);");
            RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraReset();
        }

        private void DrawThickTexturedLines(int x, int y, int height)
        {
            RB.CameraSet(new Vector2i(-x, -y));

            DrawAlphaGrid(x, y, 10, mMaxLineWidth, height);

            RB.CameraSet(new Vector2i(-x, -y - (height / 2)));

            int yOffset = (int)(Mathf.Sin(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineStart = new Vector2i(0, yOffset * mWiggleScale);

            yOffset = (int)(Mathf.Cos(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineEnd = new Vector2i(lineStart.x + mMaxLineWidth, yOffset * mWiggleScale);

            RB.DrawLineTextured(lineStart, lineEnd, new Rect2i(29, 242, 51, 10), 0, 1);

            DrawSourceSprite(new Vector2i(x + mMaxLineWidth + 10, y), new Rect2i(29, 242, 51, 10), height);

            RB.CameraSet(new Vector2i(-(x + mMaxLineWidth + 10 + 80), -y));

            RB.DrawRectFill(new Rect2i(0, 0, 292, height), new Color32(35, 35, 35, 255));

            mFormatStr.Clear();
            mFormatStr.Set("@C// Draw a textured line\n");
            mFormatStr.Append("@Kvar@N p0 = @Knew@N @MVector2i@N(@L").Append(lineStart.x).Append("@N, @L").Append(lineStart.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N p1 = @Knew@N @MVector2i@N(@L").Append(lineEnd.x).Append("@N, @L").Append(lineEnd.y).Append("@N);\n");
            mFormatStr.Append("@MRB@N.DrawLine(p0, p1, myLineSprite);");
            RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraReset();
        }

        private void DrawThinTexturedLinesFixedRepeat(int x, int y, int height)
        {
            RB.CameraSet(new Vector2i(-x, -y));

            DrawAlphaGrid(x, y, 10, mMaxLineWidth, height);

            RB.CameraSet(new Vector2i(-x, -y - (height / 2)));

            int yOffset = (int)(Mathf.Sin(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineStart = new Vector2i(0, yOffset * mWiggleScale);

            yOffset = (int)(Mathf.Cos(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineEnd = new Vector2i(lineStart.x + mMaxLineWidth, yOffset * mWiggleScale);

            float slope = ((float)lineEnd.y - (float)lineStart.y) / ((float)lineEnd.x - (float)lineStart.x);
            int xOffset = (int)(((mMaxLineWidth / 2) * Mathf.Sin(RB.Ticks * 0.025f)) + (mMaxLineWidth / 2));
            lineEnd.x = lineStart.x + xOffset;
            lineEnd.y = lineStart.y + (int)(xOffset * slope);

            RB.DrawLineTextured(lineStart, lineEnd, 1, new Rect2i(29, 224, 9, 1), 0, 10);

            DrawSourceSprite(new Vector2i(x + mMaxLineWidth + 10, y), new Rect2i(29, 224, 9, 1), height);

            RB.CameraSet(new Vector2i(-(x + mMaxLineWidth + 10 + 80), -y));

            RB.DrawRectFill(new Rect2i(0, 0, 292, height), new Color32(35, 35, 35, 255));

            mFormatStr.Clear();
            mFormatStr.Set("@C// Draw a textured line with a fixed amount of repeated segments\n");
            mFormatStr.Append("@Kvar@N p0 = @Knew@N @MVector2i@N(@L").Append(lineStart.x).Append("@N, @L").Append(lineStart.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N p1 = @Knew@N @MVector2i@N(@L").Append(lineEnd.x).Append("@N, @L").Append(lineEnd.y).Append("@N);\n");
            mFormatStr.Append("@MRB@N.DrawLine(p0, p1, myLineSprite, @L0@N, @L10@N);");
            RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraReset();
        }

        private void DrawThinTexturedLinesVariableRepeat(int x, int y, int height)
        {
            RB.CameraSet(new Vector2i(-x, -y));

            DrawAlphaGrid(x, y, 10, mMaxLineWidth, height);

            RB.CameraSet(new Vector2i(-x, -y - (height / 2)));

            int yOffset = (int)(Mathf.Sin(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineStart = new Vector2i(0, yOffset * mWiggleScale);

            yOffset = (int)(Mathf.Cos(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineEnd = new Vector2i(lineStart.x + mMaxLineWidth, yOffset * mWiggleScale);

            float slope = ((float)lineEnd.y - (float)lineStart.y) / ((float)lineEnd.x - (float)lineStart.x);
            int xOffset = (int)(((mMaxLineWidth / 2) * Mathf.Sin(RB.Ticks * 0.025f)) + (mMaxLineWidth / 2));
            lineEnd.x = lineStart.x + xOffset;
            lineEnd.y = lineStart.y + (int)(xOffset * slope);

            float len = (lineEnd - lineStart).Magnitude() + 1.0f;

            RB.DrawLineTextured(lineStart, lineEnd, new Rect2i(29, 224, 9, 1), 0, len / 9);

            DrawSourceSprite(new Vector2i(x + mMaxLineWidth + 10, y), new Rect2i(29, 224, 9, 1), height);

            RB.CameraSet(new Vector2i(-(x + mMaxLineWidth + 10 + 80), -y));

            RB.DrawRectFill(new Rect2i(0, 0, 292, height), new Color32(35, 35, 35, 255));

            mFormatStr.Clear();
            mFormatStr.Set("@C// Draw a textured line with length dependent repeated segments\n");
            mFormatStr.Append("@Kvar@N p0 = @Knew@N @MVector2i@N(@L").Append(lineStart.x).Append("@N, @L").Append(lineStart.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N p1 = @Knew@N @MVector2i@N(@L").Append(lineEnd.x).Append("@N, @L").Append(lineEnd.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N lineLen = (lineEnd - lineStart).Magnitude() + @L1.0f@N;\n");
            mFormatStr.Append("@Kvar@N repeats = lineLen / (float)myLineSprite.Size.width;\n");
            mFormatStr.Append("@MRB@N.DrawLine(p0, p1, myLineSprite, @L0@N, repeats);");
            RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraReset();
        }

        private void DrawThickTexturedRepeatingLines(int x, int y, int height)
        {
            RB.CameraSet(new Vector2i(-x, -y));

            DrawAlphaGrid(x, y, 10, mMaxLineWidth, height);

            RB.CameraSet(new Vector2i(-x, -y - (height / 2)));

            int yOffset = (int)(Mathf.Sin(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineStart = new Vector2i(0, yOffset * mWiggleScale);

            yOffset = (int)(Mathf.Cos(RB.Ticks * 0.05f) * (height / 2));
            Vector2i lineEnd = new Vector2i(lineStart.x + mMaxLineWidth, yOffset * mWiggleScale);

            float len = (lineEnd - lineStart).Magnitude();

            RB.DrawLineTextured(lineStart, lineEnd, new Rect2i(29, 225, 16, 17), mScroll, len / 16);

            DrawSourceSprite(new Vector2i(x + mMaxLineWidth + 10, y), new Rect2i(29, 225, 16, 17), height);

            RB.CameraSet(new Vector2i(-(x + mMaxLineWidth + 10 + 80), -y));

            RB.DrawRectFill(new Rect2i(0, 0, 292, height), new Color32(35, 35, 35, 255));

            mFormatStr.Clear();
            mFormatStr.Set("@C// Draw a line with a scrolling texture\n");
            mFormatStr.Append("@Kvar@N p0 = @Knew@N @MVector2i@N(@L").Append(lineStart.x).Append("@N, @L").Append(lineStart.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N p1 = @Knew@N @MVector2i@N(@L").Append(lineEnd.x).Append("@N, @L").Append(lineEnd.y).Append("@N);\n");
            mFormatStr.Append("@Kvar@N lineLen = (lineEnd - lineStart).Magnitude() + @L1.0f@N;\n");
            mFormatStr.Append("@Kvar@N repeats = lineLen / (float)myLineSprite.Size.width;\n");
            mFormatStr.Append("@MRB@N.DrawLine(p0, p1, myLineSprite, @L").Append(mScroll, 2).Append("f@N, repeats);");
            RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));

            RB.CameraReset();
        }

        private void DrawSourceSprite(Vector2i pos, Rect2i srcRect, int height)
        {
            Vector2i oldCamPos = RB.CameraGet();
            RB.CameraSet(-pos);

            DrawAlphaGrid(pos.x, pos.y, 10, 70, height);
            RB.Print(new Vector2i(2, 2), Color.white, "Source Sprite:");

            RB.DrawCopy(srcRect, new Vector2i((70 / 2) - (srcRect.width / 2), (height / 2) - (srcRect.height / 2)));

            RB.CameraSet(oldCamPos);
        }
    }
}
