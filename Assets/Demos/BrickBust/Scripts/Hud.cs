namespace RetroBlitDemoBrickBust
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Game HUD, displays score, lives left, and game logo
    /// </summary>
    public class GameHud
    {
        private FastString mString = new FastString(512);

        private bool mShowHelp = true;
        private bool mShowGameOver = false;
        private bool mShowWin = false;
        private int mInfoFade = 255;
        private float mLastTapTime;

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            bool doubleTapped = false;
            if (RB.ButtonPressed(RB.BTN_POINTER_A))
            {
                if (Time.time - mLastTapTime < 0.5f)
                {
                    doubleTapped = true;
                }

                mLastTapTime = UnityEngine.Time.time;
            }

            if (!mShowHelp && !mShowGameOver && !mShowWin)
            {
                mInfoFade = Math.Max(0, mInfoFade - 8);
            }
            else if (mShowGameOver)
            {
                mInfoFade = Math.Min(255, mInfoFade + 8);

                if (doubleTapped)
                {
                    var game = (BrickBustGame)RB.Game;
                    game.ChangeState(BrickBustGame.GameState.MAIN_MENU);
                }
            }
            else if (mShowWin)
            {
                mInfoFade = Math.Min(255, mInfoFade + 8);

                if (doubleTapped)
                {
                    var game = (BrickBustGame)RB.Game;
                    game.ChangeState(BrickBustGame.GameState.LEVEL);
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            var game = (BrickBustGame)RB.Game;
            var level = game.Level;

            for (int i = 0; i < level.Lives; i++)
            {
                RB.DrawCopy(new Rect2i(0, 40, 15, 7), new Rect2i(10 + (i * 17), RB.DisplaySize.height - 10, 15, 7));
            }

            Rect2i titleRect = new Rect2i(0, 50, 93, 17);
            RB.DrawCopy(titleRect, new Rect2i((RB.DisplaySize.width / 2) - (titleRect.width / 2), 2, titleRect.width, titleRect.height));

            mString.Set("@64C132SCORE@-\n").Append(level.Score);
            RB.Print(new Rect2i(0, 0, RB.DisplaySize.width / 4, game.assets.spriteSheet.grid.cellSize.height * 2), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mString);
            mString.Set("@64C132HISCORE@-\n").Append(level.HiScore);
            RB.Print(new Rect2i(RB.DisplaySize.width - (RB.DisplaySize.width / 4), 0, RB.DisplaySize.width / 4, game.assets.spriteSheet.grid.cellSize.height * 2), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mString);

            mString.Set("DRAG TO MOVE PADDLE\n\nDOUBLE ").Append(Global.ACTION_VERB).Append(" TO RELEASE THE BALL!");

            if (mShowGameOver)
            {
                mString.Set("GAME OVER!\n\nDOUBLE ").Append(Global.ACTION_VERB).Append(" TO EXIT!");
            }
            else if (mShowWin)
            {
                mString.Set("LEVEL CLEARED!\n\nDOUBLE ").Append(Global.ACTION_VERB).Append(" TO PROCEED!");
            }

            if (mInfoFade > 0)
            {
                int bob = (int)(Math.Sin(RB.Ticks / 10.0f) * 5);

                RB.AlphaSet((byte)mInfoFade);
                RB.Print(new Rect2i(0 + 2, 260 + 2 + bob, RB.DisplaySize.width, 40), Color.black, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mString);
                RB.Print(new Rect2i(0, 260 + bob, RB.DisplaySize.width, 40), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mString);
                RB.AlphaSet(255);
            }
        }

        /// <summary>
        /// Hide help text
        /// </summary>
        public void HideHelp()
        {
            mShowHelp = false;
        }

        /// <summary>
        /// Show game over text
        /// </summary>
        public void ShowGameOver()
        {
            mShowGameOver = true;
        }

        /// <summary>
        /// Show Won text
        /// </summary>
        public void ShowWin()
        {
            mShowWin = true;
        }
    }
}