namespace RetroBlitDemoStressTest
{
    using UnityEngine;

    /// <summary>
    /// Parent class of all DemoReel scenes
    /// </summary>
    public class SceneDemo
    {
        /// <summary>
        /// Formatting string
        /// </summary>
        protected FastString mFormatStr = new FastString(4192);

        /// <summary>
        /// Final string to print to screen
        /// </summary>
        protected FastString mFinalStr = new FastString(4192);

        private long[] mTouchTimestamp = new long[4] { long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue };

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public virtual bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public virtual void Enter()
        {
            RB.TicksReset();
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// Update, handles switching to next scene, and quitting the demo
        /// </summary>
        public virtual void Update()
        {
            var demo = (StressTest)RB.Game;

            if (RB.KeyPressed(KeyCode.Return) || (UnityEngine.Input.mousePresent && RB.ButtonPressed(RB.BTN_POINTER_A)) || RB.ButtonPressed(RB.BTN_A))
            {
                demo.NextScene();
            }

            if ((UnityEngine.Input.mousePresent && RB.ButtonPressed(RB.BTN_POINTER_B)) || RB.ButtonPressed(RB.BTN_X))
            {
                demo.PreviousScene();
            }

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                Application.Quit();
            }

            if (!UnityEngine.Input.mousePresent)
            {
                if (RB.ButtonReleased(RB.BTN_POINTER_A, RB.PLAYER_ANY))
                {
                    mTouchTimestamp[0] = (long)RB.Ticks;
                }

                if (RB.ButtonReleased(RB.BTN_POINTER_B, RB.PLAYER_ANY))
                {
                    mTouchTimestamp[1] = (long)RB.Ticks;
                }

                if (RB.ButtonReleased(RB.BTN_POINTER_C, RB.PLAYER_ANY))
                {
                    mTouchTimestamp[2] = (long)RB.Ticks;
                }

                if (RB.ButtonReleased(RB.BTN_POINTER_D, RB.PLAYER_ANY))
                {
                    mTouchTimestamp[3] = (long)RB.Ticks;
                }

                // If all fingers are up then check how many fingers have close timestamps
                if (!RB.ButtonDown(RB.BTN_POINTER_A, RB.PLAYER_ANY) &&
                    !RB.ButtonDown(RB.BTN_POINTER_B, RB.PLAYER_ANY) &&
                    !RB.ButtonDown(RB.BTN_POINTER_C, RB.PLAYER_ANY) &&
                    !RB.ButtonDown(RB.BTN_POINTER_D, RB.PLAYER_ANY))
                {
                    int fingerCount = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        long delta = (long)RB.Ticks - (long)mTouchTimestamp[i];
                        if (delta >= 0 && delta < 5)
                        {
                            fingerCount++;
                        }
                    }

                    if (fingerCount == 1)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            mTouchTimestamp[i] = long.MaxValue;
                        }

                        demo.NextScene();
                    }
                    else if (fingerCount > 1)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            mTouchTimestamp[i] = long.MaxValue;
                        }

                        demo.PreviousScene();
                    }
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public virtual void Render()
        {
        }
    }
}
