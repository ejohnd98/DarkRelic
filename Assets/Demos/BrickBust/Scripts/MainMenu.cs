namespace RetroBlitDemoBrickBust
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Main menu screen
    /// </summary>
    public class MainMenu
    {
        private FastString mString = new FastString(256);
        private List<Blob> mBlobs = new List<Blob>();

        /// <summary>
        /// Constructor
        /// </summary>
        public MainMenu()
        {
            for (int i = 0; i < 32; i++)
            {
                mBlobs.Add(new Blob());
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (RB.ButtonPressed(RB.BTN_POINTER_A))
            {
                var game = (BrickBustGame)RB.Game;
                game.ChangeState(BrickBustGame.GameState.LEVEL);
            }

            for (int i = 0; i < mBlobs.Count; i++)
            {
                mBlobs[i].Update();
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            var game = (BrickBustGame)RB.Game;

            for (int i = 0; i < mBlobs.Count; i++)
            {
                mBlobs[i].Render();
            }

            Rect2i titleRect = new Rect2i(0, 50, 93, 17);
            int bob = (int)(Math.Sin(RB.Ticks / 10.0f) * 6);

            RB.ShaderSet(game.assets.shaderShadow);
            game.assets.shaderShadow.ColorSet("_ShadowColor", new Color32(0, 0, 0, 196));
            RB.DrawCopy(titleRect, new Rect2i((RB.DisplaySize.width / 2) - (titleRect.width / 2) + 3, 32 + bob + 3, titleRect.width, titleRect.height));
            RB.ShaderReset();

            RB.DrawCopy(titleRect, new Rect2i((RB.DisplaySize.width / 2) - (titleRect.width / 2), 32 + bob, titleRect.width, titleRect.height));

            RB.DrawCopy(new Rect2i(38, 68, 199, 188), new Rect2i(11, 96, 199, 188));

            RB.ShaderSet(game.assets.shaderShadow);
            game.assets.shaderShadow.ColorSet("_ShadowColor", new Color32(255, 255, 255, 128));
            int highlightOffset = (int)((Math.Sin(RB.Ticks / 50.0f) * RB.DisplaySize.width) + (RB.DisplaySize.width / 2));
            int highlightWidth = 20;
            RB.DrawCopy(new Rect2i(38 + (highlightOffset - 11), 68, highlightWidth, 188), new Rect2i(highlightOffset, 96, highlightWidth, 188));
            RB.ShaderReset();

            mString.Set("@w244").Append(Global.ACTION_VERB).Append(" TO PLAY!");

            RB.Print(new Rect2i(0, RB.DisplaySize.height - 80 + 1, RB.DisplaySize.width, 100 + 1), Color.black, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mString);
            RB.Print(new Rect2i(0, RB.DisplaySize.height - 80, RB.DisplaySize.width, 100), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mString);
        }

        private class Blob
        {
            private Vector2 mPos;

            private float[] mWaveLengths;
            private float[] mPhases;
            private int mWaves = 3;
            private Color32 mColorTint = Color.white;

            /// <summary>
            /// Constructor
            /// </summary>
            public Blob()
            {
                mWaveLengths = new float[mWaves * 2];
                mPhases = new float[mWaves * 2];

                for (int i = 0; i < mWaves * 2; i += 2)
                {
                    mWaveLengths[i] = UnityEngine.Random.Range(20.0f, 60.0f);
                    mWaveLengths[i + 1] = UnityEngine.Random.Range(20.0f, 60.0f);

                    mPhases[i] = UnityEngine.Random.Range(0.0f, (float)(Math.PI * 2));
                    mPhases[i + 1] = UnityEngine.Random.Range(0.0f, (float)(Math.PI * 2));
                }

                switch (UnityEngine.Random.Range(0, 6))
                {
                    case 0:
                        mColorTint = Global.COLOR_GOLD_BRICK;
                        break;

                    case 1:
                        mColorTint = Global.COLOR_BLUE_BRICK;
                        break;

                    case 2:
                        mColorTint = Global.COLOR_GREEN_BRICK;
                        break;

                    case 3:
                        mColorTint = Global.COLOR_BROWN_BRICK;
                        break;

                    case 4:
                        mColorTint = Global.COLOR_PINK_BRICK;
                        break;

                    case 5:
                        mColorTint = Global.COLOR_BLACK_BRICK;
                        break;
                }
            }

            /// <summary>
            /// Update
            /// </summary>
            public void Update()
            {
                mPos.x = 0;
                mPos.y = 0;

                // Accumulate waves
                for (int i = 0; i < mWaves * 2; i += 2)
                {
                    mPos.x += Wave(mWaveLengths[i], mPhases[i]);
                    mPos.y += Wave(mWaveLengths[i + 1], mPhases[i + 1]);
                }

                mPos.x = (mPos.x * (RB.DisplaySize.width / 2)) + (RB.DisplaySize.width / 2);
                mPos.y = (mPos.y * (RB.DisplaySize.height / 2)) + (RB.DisplaySize.height / 2);
            }

            /// <summary>
            /// Render
            /// </summary>
            public void Render()
            {
                Rect2i blobRect = new Rect2i(191, 0, 65, 65);
                RB.TintColorSet(mColorTint);
                RB.DrawCopy(blobRect, new Rect2i((int)mPos.x - (blobRect.width / 2), (int)mPos.y - (blobRect.height / 2), blobRect.width, blobRect.height));
                RB.TintColorSet(Color.white);
            }

            /// <summary>
            /// Wave utility function
            /// </summary>
            /// <param name="waveLength">Wave length</param>
            /// <param name="offset">Wave offset</param>
            /// <returns>Wave value</returns>
            private float Wave(float waveLength, float offset)
            {
                return (float)Math.Sin(((2 * Math.PI / waveLength) * (RB.Ticks / 10.0f)) + offset) * 0.5f;
            }
        }
    }
}