namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneText : SceneDemo
    {
        private FastString mDialogStr = new FastString(512);

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
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
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

            RB.Clear(DemoUtil.IndexToRGB(0));

            int x = 4;
            int y = 12;

            DrawAlign(x, y);

            x += 335;

            DrawClip(x, y);
        }

        private void DrawTextFrame(Rect2i rect, FastString subTitle)
        {
            var demo = (DemoReel)RB.Game;

            RB.DrawRectFill(rect, DemoUtil.IndexToRGB(1));
            RB.DrawRect(rect, DemoUtil.IndexToRGB(2));

            if (subTitle != null)
            {
                var subTitleRect = new Rect2i(rect.x, rect.y + rect.height + 5, rect.width, rect.height);
                RB.Print(subTitleRect, DemoUtil.IndexToRGB(3), RB.ALIGN_H_CENTER | RB.TEXT_OVERFLOW_WRAP, subTitle);
            }
        }

        private void DrawAlign(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 25));

            mDialogStr.Set("\u30B3\u30F3\u30CB\u30C1\u30EF\n@dbab77RetroBlit@- supports\nunicode fonts!");

            var textRect = new Rect2i(2, 12, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_LEFT | @MRB@N.ALIGN_V_TOP"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_LEFT | RB.ALIGN_V_TOP, mDialogStr);

            textRect = new Rect2i(112, 12, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_CENTER | @MRB@N.ALIGN_V_TOP"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_CENTER | RB.ALIGN_V_TOP, mDialogStr);

            textRect = new Rect2i(222, 12, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_RIGHT | @MRB@N.ALIGN_V_TOP"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_RIGHT | RB.ALIGN_V_TOP, mDialogStr);

            textRect = new Rect2i(2, 112, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_LEFT | @MRB@N.ALIGN_V_CENTER"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_LEFT | RB.ALIGN_V_CENTER, mDialogStr);

            textRect = new Rect2i(112, 112, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_CENTER | @MRB@N.ALIGN_V_CENTER"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mDialogStr);

            textRect = new Rect2i(222, 112, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_RIGHT | @MRB@N.ALIGN_V_CENTER"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_RIGHT | RB.ALIGN_V_CENTER, mDialogStr);

            textRect = new Rect2i(2, 212, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_LEFT | @MRB@N.ALIGN_V_BOTTOM"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_LEFT | RB.ALIGN_V_BOTTOM, mDialogStr);

            textRect = new Rect2i(112, 212, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_CENTER | @MRB@N.ALIGN_V_BOTTOM"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_CENTER | RB.ALIGN_V_BOTTOM, mDialogStr);

            textRect = new Rect2i(222, 212, 96, 48);
            DrawTextFrame(textRect, DemoUtil.HighlightCode(mFormatStr.Set("alignFlags = \n@MRB@N.ALIGN_H_RIGHT | @MRB@N.ALIGN_V_BOTTOM"), mFinalStr));
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_RIGHT | RB.ALIGN_V_BOTTOM, mDialogStr);

            mFormatStr.Set("@C// Print text within a text rectangle, with specific alignment\n");
            mFormatStr.Append("@MRB@N.Print(@Knew @MRect2i@N(@L8@N, @L8@N, @L96@N, @L48@N), alignFlags,\n");
            mFormatStr.Append("   @S\"\u30B3\u30F3\u30CB\u30C1\u30EF\\n@@dbab77RetroBlit@@- supports\\nunicode fonts!\"@N);");

            RB.CameraReset();

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawClip(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 25));

            mDialogStr.Set("You there!\nThat's a nice\ngolden sword,\nand dragon\nhide armor!\nYou look like\nyou could use a\nfew coppers,\nhow about you\nclean out\nthe @dbab7710@- rats in\nmy basement?");

            var size = (int)(Mathf.Sin(RB.Ticks / 20.0f) * 22) * 2;
            var textRect = new Rect2i(32 - (size / 2), 45, 80 + size, 128);
            DrawTextFrame(textRect, null);
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER | RB.TEXT_OVERFLOW_CLIP, mDialogStr);

            textRect = new Rect2i(32 - (size / 2) + 140, 45, 80 + size, 128);
            DrawTextFrame(textRect, null);
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER | RB.TEXT_OVERFLOW_WRAP | RB.TEXT_OVERFLOW_CLIP, mDialogStr);

            textRect = new Rect2i(32 - (size / 2) + 70, 180, 80 + size, 128);
            DrawTextFrame(textRect, null);
            RB.Print(textRect, DemoUtil.IndexToRGB(4), RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER | RB.TEXT_OVERFLOW_WRAP, mDialogStr);

            mFormatStr.Set("@C// Clip and wrap text in a text rectangle\n").Append("@Kint@N center = @MRB@N.ALIGN_H_CENTER | @MRB@N.ALIGN_V_CENTER;\n");
            mFormatStr.Append("@MRB@N.Print(@Knew@N Rect2i(@L20@N, @L20@N, @L").Append(80 + size).Append("@N, @L128@N),\n   center | @MRB@N.TEXT_OVERFLOW_CLIP, myText);\n");
            mFormatStr.Append("@MRB@N.Print(@Knew@N Rect2i(@L160@N, @L20@N, @L").Append(80 + size).Append("@N, @L128@N),\n   center | @MRB@N.TEXT_OVERFLOW_WRAP | @MRB@N.TEXT_OVERFLOW_CLIP, myText);\n");
            mFormatStr.Append("@MRB@N.Print(@Knew@N Rect2i(@L90@N, @L155@N, @L").Append(80 + size).Append("@N, @L128@N),\n   center | @MRB@N.TEXT_OVERFLOW_WRAP, myText);\n");

            RB.CameraReset();

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }
    }
}
