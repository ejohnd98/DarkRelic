namespace RetroBlitDemoReel
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Demonstrate loading and rendering of infinite Tiled maps
    /// </summary>
    public class SceneEase : SceneDemo
    {
        private float mT = 0;
        private string[] mFuncNames;
        private SpriteSheetAsset spriteSheet1 = new SpriteSheetAsset();

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneEase()
        {
            mFuncNames = Enum.GetNames(typeof(Ease.Func));
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

            spriteSheet1.Load("DemoReel/Sprites");
            spriteSheet1.grid = new SpriteGrid(new Vector2i(16, 16));
            RB.SpriteSheetSet(spriteSheet1);

            mT = 0;
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            RB.MapClear();

            spriteSheet1.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            mT += 0.01f;
            if (mT > 2.0f)
            {
                mT = 0;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            int width = 96 + 13 - 32 + 21;
            int height = 48 + 16 - 16;

            int x = 8;
            int y = 48;
            int columns = 0;

            // Skip linear, boring
            for (int i = 1; i < mFuncNames.Length; i++)
            {
                DrawEase((Ease.Func)i, mFuncNames[i], x, y, width, height);

                x += width + 8;
                columns++;

                if (columns >= 6)
                {
                    x = 6;
                    y += height + 16;
                    columns = 0;
                }
            }

            DrawCode(4, 4);
        }

        private void DrawEase(Ease.Func func, string funcName, int x, int y, int width, int height)
        {
            byte spriteAlpha = 255;
            if (mT > 1.5f)
            {
                spriteAlpha = (byte)(((1.5f - mT) / 0.5f) * 255);
            }

            var graphColor = DemoUtil.IndexToRGB(2);
            var lineColor = DemoUtil.IndexToRGB(3);
            var scaleColor = DemoUtil.IndexToRGB(24);
            var interStartColor = DemoUtil.IndexToRGB(27);
            var interEndColor = DemoUtil.IndexToRGB(23);
            RB.CameraSet(new Vector2i(-x, -y));

            width -= 28;
            height -= 16;

            // Draw graph
            int spaceBetweenLines = (height + 8) - 8;
            RB.DrawLine(new Vector2i(0, 8), new Vector2i(width, 8), graphColor);
            RB.DrawLine(new Vector2i(0, 8 + spaceBetweenLines), new Vector2i(width, 8 + spaceBetweenLines), graphColor);
            RB.Print(new Vector2i(0, 10), graphColor, funcName);

            // Draw line
            Vector2i p0 = new Vector2i(0, 8 + height);
            for (int i = 1; i <= width; i += 1)
            {
                int val = 0;
                val = Ease.Interpolate(func, 0, height, i / (float)width);

                Vector2i p1 = new Vector2i(i, 8 + height - val);

                RB.DrawLine(p0, p1, lineColor);

                p0 = p1;
            }

            // Draw movement sprite
            float t = mT;
            if (t > 1)
            {
                t = 1;
            }

            float ti = Ease.Interpolate(func, 0, 1.0f, t);

            Vector2i pos = new Vector2i(width + 8 - 4, 8 + height - Mathf.RoundToInt(ti * height) - 4);
            RB.DrawSprite(RB.SpriteIndex(7, 12), new Vector2i(width + 8 - 4, 8 - 4));
            RB.DrawSprite(RB.SpriteIndex(7, 12), new Vector2i(width + 8 - 4, 8 + height - 4));

            RB.AlphaSet(spriteAlpha);
            var highlightPos = new Vector2i((t * width) - 1, pos.y + 3);
            RB.DrawSprite(RB.SpriteIndex(7, 13), highlightPos);
            RB.DrawSprite(RB.SpriteIndex(7, 11), pos);
            RB.AlphaSet(255);

            int scale = 4 + Mathf.RoundToInt(ti * ((spaceBetweenLines / 2) - 4));
            RB.DrawRectFill(new Rect2i(width + 16, (height / 2) - scale + 8, 3, (scale * 2) + 1), scaleColor);

            var interColor = Ease.Interpolate(func, interStartColor, interEndColor, t);
            RB.DrawRectFill(new Rect2i(width + 20, 8, 3, 3), interEndColor);
            RB.DrawRectFill(new Rect2i(width + 20, 8 + spaceBetweenLines - 4 + 2, 3, 3), interStartColor);
            RB.DrawRectFill(new Rect2i(width + 20, 9 + 3, 3, spaceBetweenLines - 7), interColor);

            RB.CameraReset();
        }

        private void DrawCode(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            mFormatStr.Set("@C// Interpolate between integers, floats, colors, vectors, and more using RetroBlit easing functions!\n");
            mFormatStr.Append("@Kint@N num = @MEase@N.Interpolate(@MEase@N.@KFunc@N.CubicOut, @L10@N, @L20@N, @L0.5f@N);\n");
            mFormatStr.Append("@KColor@N color = @MEase@N.Interpolate(@MEase@N.@KFunc@N.BounceInOut, @MColor@N.green, @MColor@N.red, @L0.75f@N);\n");
            mFormatStr.Append("@KVector2i@N vec = @MEase@N.Interpolate(@MEase@N.@KFunc@N.ElasticIn, @Knew@N @MVector2i@N(@L0@N, @L0@N), @Knew@N @MVector2i@N(@L-5@N, @L-8@N), @L0.33f@N);\n");
            mFormatStr.Append("@KRect2i@N rect = @MEase@N.Interpolate(@MEase@N.@KFunc@N.BackOut, @Knew@N @MRect2i@N(@L5@N, @L5@N, @L10@N, @L10@N), @Knew@N @MRect2i@N(@L0@N, @L0@N, @L5@N, @L5@N), @L0.1f@N);\n");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
