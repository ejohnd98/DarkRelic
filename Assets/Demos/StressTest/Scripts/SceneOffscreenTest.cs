namespace RetroBlitDemoStressTest
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneOffscreenTest : SceneStress
    {
        private SpriteSheetAsset spriteSheet1 = new SpriteSheetAsset();
        private SpriteSheetAsset spriteSheet2 = new SpriteSheetAsset();

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            spriteSheet1.Create(new Vector2i(32, 32));
            spriteSheet2.Create(new Vector2i(64, 64));
        }

        /// <summary>
        /// Run the stress test
        /// </summary>
        protected override void StressTest()
        {
            Random.InitState(0);

            RB.Offscreen(spriteSheet2);
            RB.Clear(new Color32(0, 0, 0, 0));

            RB.Offscreen(spriteSheet1);
            RB.Clear(new Color32(0, 0, 0, 0));

            // Generate texture on first offscreen surface
            for (int i = 0; i < 64; i++)
            {
                int radius = Random.Range(2, 5);
                RB.DrawEllipseFill(
                    new Vector2i(Random.Range(-4, 36), Random.Range(-4, 36)),
                    new Vector2i(radius, radius),
                    mRandomColor[i]);
            }

            // Copy the generate texture mirrored on second offscreen surface
            RB.Offscreen(spriteSheet2);
            RB.SpriteSheetSet(spriteSheet1);
            RB.DrawCopy(new Rect2i(0, 0, 32, 32), Vector2i.zero, RB.FLIP_H | RB.FLIP_V);

            RB.Onscreen();

            for (int i = 0; i < mStressLevel; i++)
            {
                var randPos = mRandomPos[i];
                randPos += GetWiggle();

                // Draw half the sprites from offscreen 0 and half from 1.
                SpriteSheetAsset offscreen = spriteSheet1;
                if (i >= mStressLevel / 2)
                {
                    offscreen = spriteSheet2;
                }

                RB.SpriteSheetSet(offscreen);
                RB.DrawCopy(new Rect2i(0, 0, 32, 32), randPos);
            }
        }

        /// <summary>
        /// Draw information overlay
        /// </summary>
        protected override void Overlay()
        {
            mString.Clear();
            mString.Append("Offscreen Stress Test\n@7F7F7FSprites: @FFFFFF").Append(mStressLevel).Append("\n@7F7F7FFPS: @FFFFFF").Append(mFPS, 2).Append("\n\n@FFFFFF");
            mString.Append("\u2191 \u2193 @7F7F7FChange Stress Level\n").Append("@FFFFFF");
            mString.Append("\u2190 \u2192 @7F7F7FChange Test");

            RB.DrawRectFill(new Rect2i(4, 4, 115, 51), new Color32(32, 32, 32, 255));
            RB.Print(new Vector2i(6, 6), Color.white, mString);
        }
    }
}
