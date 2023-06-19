namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneDrawing : SceneDemo
    {
        private int mOutputBackground = 22;
        private int mOutputFrame = 21;
        private Vector2i[] mShapeSize = new Vector2i[7];
        private Vector2i[] mLinePoint = new Vector2i[32 / 2];
        private SpriteSheetAsset spriteSheet1 = new SpriteSheetAsset();
        private FontAsset font = new FontAsset();

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

            var demo = (DemoReel)RB.Game;

            font.Setup('A', 'Z', new Vector2i(0, 16), spriteSheet1, new Vector2i(12, 12), 10, 1, 2, false);
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            spriteSheet1.Unload();
            font.Unload();

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

            int spriteIndex = (RB.Ticks % 40) > 20 ? 1 : 0;

            int x = 4;
            int y = 4;

            DrawSprite(x, y, spriteIndex);

            y += 38;

            DrawScale(x, y, spriteIndex);

            y += 64;

            DrawCopy(x, y, spriteIndex);

            y += 40;

            DrawRotate(x, y, spriteIndex);

            y += 43;

            DrawTint(x, y, spriteIndex);

            y += 72;

            DrawAlpha(x, y, spriteIndex);

            y += 58;

            DrawSystemFont(x, y);

            x = 300;
            y = 4;

            DrawFontEffects(x, y);

            y += 42;

            DrawCustomFont(x, y);

            y += 60;

            DrawPixels(x, y);

            y += 59;

            DrawShapes(x, y);

            y += 100;

            DrawLines(x, y);
        }

        private void DrawSprite(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 17));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 16, 16), -1, mOutputFrame, mOutputBackground);

            mFormatStr.Clear();

            if (RB.Ticks % 200 < 100)
            {
                mFormatStr.Append("@C// Draw sprite from sprite sheet by its index\n");
                mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MVector2i@N(@L0@N, @L0@N));");
                RB.DrawSprite(spriteIndex, new Vector2i(0, 0));
            }
            else
            {
                mFormatStr.Append("@C// Draw sprite from sprite sheet by its index\n");
                mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MVector2i@N(@L0@N, @L0@N), @MRB@N.FLIP_H);");
                RB.DrawSprite(spriteIndex, new Vector2i(0, 0), RB.FLIP_H);
            }

            RB.CameraReset();

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawScale(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 25));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 235, 32), -1, mOutputFrame, mOutputBackground);

            Vector2i scaleSize = new Vector2i();
            scaleSize.x = (int)((Mathf.Sin(RB.Ticks / 20.0f) * 96) + 100);
            scaleSize.y = (int)((Mathf.Sin(RB.Ticks / 2 / 20.0f) * 16) + 17);

            RB.DrawSprite(spriteIndex, new Rect2i(0, 0, 32, 32));
            RB.DrawSprite(spriteIndex, new Rect2i(40, 0, scaleSize.x, scaleSize.y));

            RB.CameraReset();

            mFormatStr.Set("@C// Scale sprites by specifying destination rectangle\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MRect2i@N(@L0@N, @L0@N, @L32@N, @L32@N));\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MRect2i@N(@L40@N, @L0@N, @L").Append(scaleSize.x).Append("@N, @L").Append(scaleSize.y).Append("@N));");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawCopy(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 17));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 16, 16), -1, mOutputFrame, mOutputBackground);

            int copySize = (int)((Mathf.Sin(RB.Ticks / 20.0f) * 8) + 9);

            RB.DrawCopy(new Rect2i(0, 0, copySize, copySize), new Vector2i(0, 0));

            RB.CameraReset();

            mFormatStr.Set("@C// Draw rectangular areas from sprite sheet\n");
            mFormatStr.Append("@MRB@N.DrawCopy(@Knew @MRect2i(@L0@N, @L0@N, @L").Append(copySize).Append("@N, @L").Append(copySize).Append("@N), @Knew @MVector2i@N(@L0@N, @L0@N));");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawRotate(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 17));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 18, 18), -1, mOutputFrame, mOutputBackground);

            RB.DrawSprite(spriteIndex, new Vector2i(1, 1), new Vector2i(8, 8), (RB.Ticks * 4) % 360);

            RB.CameraReset();

            mFormatStr.Set("@C// Rotate sprites around pivot point\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@Knew @MVector2i(@L1@N, @L1@N), @Knew @MVector2i@N(@L8@N, @L8@N), @L").Append((RB.Ticks * 4) % 360).Append("@N);");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawTint(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 50));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 64, 16), -1, mOutputFrame, mOutputBackground);

            RB.TintColorSet(Color.white);
            RB.DrawSprite(spriteIndex, new Vector2i(0, 0));
            RB.TintColorSet(DemoUtil.IndexToRGB(12));
            RB.DrawSprite(spriteIndex, new Vector2i(24, 0));
            RB.TintColorSet(DemoUtil.IndexToRGB((int)(RB.Ticks / 10) % 32));
            RB.DrawSprite(spriteIndex, new Vector2i(48, 0));
            RB.TintColorSet(Color.white);

            RB.CameraReset();

            mFormatStr.Set("@C// Set tint color for drawing\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MVector2i@N(@L0@N, @L0@N));\n");
            mFormatStr.Append("@MRB@N.TintColorSet(@I12);\n").Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MVector2i@N(@L24@N, @L0@N));\n");
            mFormatStr.Append("@MRB@N.TintColorSet(@I").Append((RB.Ticks / 10) % 32, 2).Append(");\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MVector2i(@L48@N, @L0@N));");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawAlpha(int x, int y, int spriteIndex)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 35));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 16, 16), -1, mOutputFrame, mOutputBackground);

            byte alpha = (byte)((Mathf.Sin(RB.Ticks / 20.0f) * 127) + 127);

            RB.DrawSprite(2, new Vector2i(0, 0));
            RB.AlphaSet(alpha);
            RB.DrawSprite(spriteIndex, new Vector2i(0, 0));
            RB.AlphaSet(255);

            RB.CameraReset();

            mFormatStr.Set("@C// Alpha transparency\n").Append("@MRB@N.DrawSprite(@L2@N, @Knew @MVector2i@N(@L0@N, @L0@N));\n");
            mFormatStr.Append("@MRB@N.AlphaSet(@L").Append(alpha).Append("@N);\n");
            mFormatStr.Append("@MRB@N.DrawSprite(@L").Append(spriteIndex).Append("@N, @Knew @MVector2i(@L0@N, @L0@N));");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawSystemFont(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 25));

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 49, 6), -1, mOutputFrame, mOutputBackground);

            int c1 = ((int)RB.Ticks / 10) % 32;
            int c2 = (((int)RB.Ticks / 10) + 10) % 32;
            int c3 = (((int)RB.Ticks / 10) + 20) % 32;

            var color1 = DemoUtil.IndexToRGB(c1);
            var color2 = DemoUtil.IndexToRGB(c2);
            var color3 = DemoUtil.IndexToRGB(c3);

            mFormatStr.Set("Hello @");
            mFormatStr.Append(color1.r, 2, FastString.FORMAT_HEX_CAPS).Append(color1.g, 2, FastString.FORMAT_HEX_CAPS).Append(color1.b, 2, FastString.FORMAT_HEX_CAPS).Append("fr@");
            mFormatStr.Append(color2.r, 2, FastString.FORMAT_HEX_CAPS).Append(color2.g, 2, FastString.FORMAT_HEX_CAPS).Append(color2.b, 2, FastString.FORMAT_HEX_CAPS).Append("ie@");
            mFormatStr.Append(color3.r, 2, FastString.FORMAT_HEX_CAPS).Append(color3.g, 2, FastString.FORMAT_HEX_CAPS).Append(color3.b, 2, FastString.FORMAT_HEX_CAPS).Append("nd");

            RB.Print(new Vector2i(0, 0), DemoUtil.IndexToRGB(6), mFormatStr);

            RB.CameraReset();

            mFormatStr.Set("@C// Print text with default system font, with inline color support\n");
            mFormatStr.Append("@MRB@N.Print(@Knew @MVector2i@N(@L0@N, @L0@N), @I06@N,\n").Append("   @S\"Hello @s@@");
            mFormatStr.Append(color1.r, 2, FastString.FORMAT_HEX_CAPS).Append(color1.g, 2, FastString.FORMAT_HEX_CAPS).Append(color1.b, 2, FastString.FORMAT_HEX_CAPS).Append("@Sfr@s@@");
            mFormatStr.Append(color2.r, 2, FastString.FORMAT_HEX_CAPS).Append(color2.g, 2, FastString.FORMAT_HEX_CAPS).Append(color2.b, 2, FastString.FORMAT_HEX_CAPS).Append("@Sie@s@@");
            mFormatStr.Append(color3.r, 2, FastString.FORMAT_HEX_CAPS).Append(color3.g, 2, FastString.FORMAT_HEX_CAPS).Append(color3.b, 2, FastString.FORMAT_HEX_CAPS).Append("@Snd\"@N);");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawFontEffects(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 25));

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 136, 12), -1, mOutputFrame, mOutputBackground);

            mFormatStr.Set("@w243Wavy@w000 and @s1shaky!@s0. Why not @w243@s1both?");

            RB.Print(new Vector2i(0, 3), DemoUtil.IndexToRGB(6), mFormatStr);

            RB.CameraReset();

            mFormatStr.Set("@C// Apply wavy and shaky text effects!\n");
            mFormatStr.Append("@MRB@N.Print(@Knew @MVector2i@N(@L0@N, @L0@N), @I06@N,\n").Append("   @S\"@s@@w243@SWavy@s@@w000@S and @s@@s1@Sshaky!@s@@s0@S. Why not @s@@w243@@s1@Sboth?\"").Append("@N);");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawCustomFont(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 42));

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 100, 11), -1, mOutputFrame, mOutputBackground);

            int c1 = ((int)RB.Ticks / 10) % 32;
            int c2 = (((int)RB.Ticks / 10) + 10) % 32;
            int c3 = (((int)RB.Ticks / 10) + 20) % 32;

            var color1 = DemoUtil.IndexToRGB(c1);
            var color2 = DemoUtil.IndexToRGB(c2);
            var color3 = DemoUtil.IndexToRGB(c3);

            mFormatStr.Set("HELLO @");
            mFormatStr.Append(color1.r, 2, FastString.FORMAT_HEX_CAPS).Append(color1.g, 2, FastString.FORMAT_HEX_CAPS).Append(color1.b, 2, FastString.FORMAT_HEX_CAPS).Append("FR@");
            mFormatStr.Append(color2.r, 2, FastString.FORMAT_HEX_CAPS).Append(color2.g, 2, FastString.FORMAT_HEX_CAPS).Append(color2.b, 2, FastString.FORMAT_HEX_CAPS).Append("IE@");
            mFormatStr.Append(color3.r, 2, FastString.FORMAT_HEX_CAPS).Append(color3.g, 2, FastString.FORMAT_HEX_CAPS).Append(color3.b, 2, FastString.FORMAT_HEX_CAPS).Append("ND");

            RB.Print(font, new Vector2i(0, 0), Color.white, mFormatStr);

            RB.CameraReset();

            mFormatStr.Set("@C// Print text with custom font from sprite sheet\n");
            mFormatStr.Append("@NmyFont.Setup(@S'A'@N, @S'Z'@N, @Knew @MVector2i@N(@L0@N, @L16@N), @L0@N,\n");
            mFormatStr.Append("    @Knew @MVector2i@N(@L12@N, @L12@N), @L10@N, @L1@N, @L2@N, @Kfalse@N, @Ktrue@N);\n");
            mFormatStr.Append("@MRB@N.Print(myFont, @Knew @MVector2i@N(@L0@N, @L0@N), @I01,\n");
            mFormatStr.Append("   @S\"HELLO @s@@");
            mFormatStr.Append(color1.r, 2, FastString.FORMAT_HEX_CAPS).Append(color1.g, 2, FastString.FORMAT_HEX_CAPS).Append(color1.b, 2, FastString.FORMAT_HEX_CAPS).Append("@SFR@s@@");
            mFormatStr.Append(color2.r, 2, FastString.FORMAT_HEX_CAPS).Append(color2.g, 2, FastString.FORMAT_HEX_CAPS).Append(color2.b, 2, FastString.FORMAT_HEX_CAPS).Append("@SIE@s@@");
            mFormatStr.Append(color3.r, 2, FastString.FORMAT_HEX_CAPS).Append(color3.g, 2, FastString.FORMAT_HEX_CAPS).Append(color3.b, 2, FastString.FORMAT_HEX_CAPS).Append("@SND\"@N);");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawPixels(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 41));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 256, 12), -1, mOutputFrame, mOutputBackground);

            for (int i = 0; i < 32; i++)
            {
                RB.DrawPixel(new Vector2i((int)(Mathf.Sin((RB.Ticks / 100.0f) + (i / 10.0f)) * 128) + 128, (int)(Mathf.Sin((RB.Ticks / 10.0f) + (i / 2.0f)) * 6) + 6), DemoUtil.IndexToRGB(i));
            }

            RB.CameraReset();

            mFormatStr.Set("@C// Draw individual pixels\n");
            mFormatStr.Append("@Kfor @N(@Kint @Ni = @L0@N; i < @L32@N; i++) {\n");
            mFormatStr.Append("    @MRB@N.DrawPixel(@Knew @MVector2i@N(@MMathf@N.Sin(@L").Append(RB.Ticks / 100.0f, 2).Append("f@N + i / @L10.0f@N) * @L128@N + @L128@N,\n");
            mFormatStr.Append("         @MMathf@N.Sin(@L").Append(RB.Ticks / 10.0f, 2).Append("f@N + i / @L2.0f@N) * @L6@N + @L6@N), MyRGBColor(i));\n}");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawShapes(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 66));

            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 263, 27), -1, mOutputFrame, mOutputBackground);

            for (int i = 0; i < mShapeSize.Length; i++)
            {
                mShapeSize[i].x = (int)((Mathf.Sin((RB.Ticks / 20.0f) + i) * 6) + 8);
                mShapeSize[i].y = (int)((Mathf.Sin((RB.Ticks / 2 / 20.0f) + i) * 6) + 8);
            }

            RB.DrawRect(new Rect2i(-mShapeSize[0].x + 13, -mShapeSize[0].y + 13, mShapeSize[0].x * 2, mShapeSize[0].y * 2), DemoUtil.IndexToRGB(6));
            RB.DrawRectFill(new Rect2i(40 - mShapeSize[1].x + 13, -mShapeSize[1].y + 13, mShapeSize[1].x * 2, mShapeSize[1].y * 2), DemoUtil.IndexToRGB(27));
            RB.DrawEllipse(new Vector2i(80 + 13, 13), new Vector2i(mShapeSize[2].x, mShapeSize[2].y), DemoUtil.IndexToRGB(8));
            RB.DrawEllipseFill(new Vector2i(120 + 13, 13), new Vector2i(mShapeSize[3].x, mShapeSize[3].y), DemoUtil.IndexToRGB(23));
            RB.DrawEllipseInvertedFill(new Vector2i(160 + 13, 13), new Vector2i(mShapeSize[4].x, mShapeSize[4].y), DemoUtil.IndexToRGB(2));
            Rect2i triRect = new Rect2i(-mShapeSize[5].x + 210, -mShapeSize[5].y + 13, mShapeSize[5].x * 2, mShapeSize[5].y * 2);
            RB.DrawTriangle(new Vector2i(triRect.x + (triRect.width / 2), triRect.y), new Vector2i(triRect.x, triRect.y + triRect.height), new Vector2i(triRect.x + triRect.width, triRect.y + triRect.height), DemoUtil.IndexToRGB(15));
            triRect = new Rect2i(-mShapeSize[6].x + 250, -mShapeSize[6].y + 13, mShapeSize[6].x * 2, mShapeSize[6].y * 2);
            RB.DrawTriangleFill(new Vector2i(triRect.x, triRect.y), new Vector2i(triRect.x + triRect.width, triRect.y), new Vector2i(triRect.x + (triRect.width / 2), triRect.y + triRect.height), DemoUtil.IndexToRGB(9));

            RB.CameraReset();

            mFormatStr.Set("@C// Draw primitive shapes\n");
            mFormatStr.Append("@MRB@N.DrawRect(@Knew @MRect2i@N(@L").Append(-mShapeSize[0].x + 13).Append("@N, @L").Append(-mShapeSize[0].y + 13).Append("@N, @L").Append(mShapeSize[0].x * 2).Append("@N, @L").Append(mShapeSize[0].y * 2).Append("@N), @I6));\n");
            mFormatStr.Append("@MRB@N.DrawRectFill(@Knew @MRect2i(@L").Append(40 - mShapeSize[1].x + 13).Append("@N, @L").Append(-mShapeSize[1].y + 13).Append("@N, @L").Append(mShapeSize[1].x * 2).Append("@N, @L").Append(mShapeSize[1].y * 2).Append("@N), @I27);\n");
            mFormatStr.Append("@MRB@N.DrawEllipse(@Knew @MVector2i(@L").Append(80 + 13).Append("@N, @L13@N), @Knew @MVector2i@N(@L").Append(mShapeSize[2].x).Append("@N, @L").Append(mShapeSize[2].y).Append("@N), @I8);\n");
            mFormatStr.Append("@MRB@N.DrawEllipseFill(@Knew @MVector2i@N(@L").Append(120 + 13).Append("@N, @L13@N), @Knew @MVector2i@N(@L").Append(mShapeSize[3].x).Append("@N, @L").Append(mShapeSize[3].y).Append("@N),\n   @I31);\n");
            mFormatStr.Append("@MRB@N.DrawEllipseInvertedFill(@Knew @MVector2i@N(@L").Append(160 + 13).Append("@N, @L13@N),\n    @Knew @MVector2i@N(@L").Append(mShapeSize[4].x).Append("@N, @L").Append(mShapeSize[4].y).Append("@N), @I2));");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawLines(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 58));
            DemoUtil.DrawOutputFrame(new Rect2i(0, 0, 260, 34), -1, mOutputFrame, mOutputBackground);

            Vector2i lineCenter = new Vector2i(130, 33);
            for (int i = 0; i < mLinePoint.Length; i++)
            {
                mLinePoint[i].x = (int)(Mathf.Cos((i / (float)mLinePoint.Length * Mathf.PI) + Mathf.PI) * 128);
                mLinePoint[i].y = -(int)(Mathf.Sin(i / ((float)mLinePoint.Length - 1) * Mathf.PI) * 34);
                mLinePoint[i] += lineCenter;
            }

            for (int i = 0; i < mLinePoint.Length; i++)
            {
                RB.DrawLine(lineCenter, mLinePoint[i], DemoUtil.IndexToRGB((i + (((int)RB.Ticks / 10) % 32)) % 32));
            }

            RB.CameraReset();

            mFormatStr.Set("@C// Draw lines\n");
            mFormatStr.Append("@Kfor @N(@Kint@N i = @L0@N; i < @L32@N; i++) {\n");
            mFormatStr.Append("    @MVector2i@N p = @Knew @MVector2i@N(\n");
            mFormatStr.Append("        (@Kint@N)(@MMathf@N.Cos(i / @L38.0f@N * @MMathf@N.PI + @MMathf@N.PI) * @L128@N, \n");
            mFormatStr.Append("        @L-@N(@Kint@N)(@MMathf@N.Sin(i / @L37.0f@N * @MMathf@N.PI) * @L34h@N));\n");
            mFormatStr.Append("    @MRB@N.DrawLine(@Knew @MVector2i@N(@L").Append(lineCenter.x).Append("@N, @L").Append(lineCenter.y).Append("@N), p, MyRGBColor(i + @L").Append(((int)RB.Ticks / 10) % 32).Append("@N) % @L32@N));\n};");

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
