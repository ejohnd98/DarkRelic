namespace RetroBlitDemoStressTest
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demonstrate drawing apis
    /// </summary>
    public class SceneStress : SceneDemo
    {
        /// <summary>
        /// Level of the stress test, the higher the more is done
        /// </summary>
        protected int mStressLevel = 500;

        /// <summary>
        /// Test stress increments
        /// </summary>
        protected int mStressIncrement = 500;

        /// <summary>
        /// Calculated FPS
        /// </summary>
        protected float mFPS = 0;

        /// <summary>
        /// Random position
        /// </summary>
        protected Vector2i[] mRandomPos;

        /// <summary>
        /// Random sprite
        /// </summary>
        protected int[] mRandomSprite;

        /// <summary>
        /// Random color
        /// </summary>
        protected Color32[] mRandomColor;

        /// <summary>
        /// Random radius
        /// </summary>
        protected Vector2i[] mRandomRadius;

        /// <summary>
        /// String
        /// </summary>
        protected FastString mString = new FastString(1024);

        private const int MAX_STRESS = 300000;
        private const int FPS_SLOTS = 32;

        private static Color32[] mPaletteLookup = new Color32[]
        {
            new Color32(0, 0, 0, 255),
            new Color32(29, 29, 29, 255),
            new Color32(49, 49, 49, 255),
            new Color32(116, 116, 116, 255),
            new Color32(169, 169, 169, 255),
            new Color32(222, 222, 222, 255),
            new Color32(255, 255, 255, 255),
            new Color32(219, 171, 119, 255),
            new Color32(164, 119, 70, 255),
            new Color32(79, 51, 21, 255),
            new Color32(41, 29, 17, 255),
            new Color32(41, 17, 21, 255),
            new Color32(79, 21, 29, 255),
            new Color32(122, 52, 62, 255),
            new Color32(164, 70, 83, 255),
            new Color32(219, 119, 133, 255),
            new Color32(219, 150, 119, 255),
            new Color32(164, 99, 70, 255),
            new Color32(79, 37, 21, 255),
            new Color32(13, 27, 29, 255),
            new Color32(17, 52, 55, 255),
            new Color32(50, 104, 108, 255),
            new Color32(127, 213, 221, 255),
            new Color32(74, 198, 138, 255),
            new Color32(54, 150, 104, 255),
            new Color32(35, 101, 71, 255),
            new Color32(25, 69, 49, 255),
            new Color32(20, 56, 39, 255),
            new Color32(71, 27, 67, 255),
            new Color32(121, 47, 115, 255),
            new Color32(153, 59, 145, 255),
            new Color32(182, 70, 173, 255)
        };

        private Vector2i[] mRandomWiggle;
        private uint mWiggleCounter = 0;

        private int mFPSSlot = 0;
        private float[] mFPSSlots = new float[FPS_SLOTS];

        private float mPreviousFrame = 0;
        private float mFrameDelta = 0;

        private bool mWiggleOn = false;

        private StressButton mStressUpButton;
        private StressButton mStressDownButton;

        private StressButton mNextButton;
        private StressButton mPreviousButton;

        private StressButton mWiggleButton;

        private List<StressButton> mButtons = new List<StressButton>();

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneStress()
        {
            Rect2i buttonRect;

            Vector2i pos = new Vector2i(12, RB.DisplaySize.height - 32);
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mPreviousButton = new StressButton(buttonRect, buttonRect, "Previous Test", (KeyCode)555, 0, PreviousScreenButtonCB);

            pos.x += 100;
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mNextButton = new StressButton(buttonRect, buttonRect, "Next Test", (KeyCode)555, 0, NextScreenButtonCB);

            pos.x += 100;
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mStressDownButton = new StressButton(buttonRect, buttonRect, "Stress Down", (KeyCode)555, 0, StressDownButtonCB);

            pos.x += 100;
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mStressUpButton = new StressButton(buttonRect, buttonRect, "Stress Up", (KeyCode)555, 0, StressUpButtonCB);

            pos.x += 100;
            buttonRect = new Rect2i(pos.x, pos.y, 87, 23);
            mWiggleButton = new StressButton(buttonRect, buttonRect, "Toggle Wiggle", (KeyCode)555, 0, WiggleButtonCB);

            mButtons.Add(mNextButton);
            mButtons.Add(mPreviousButton);
            mButtons.Add(mStressDownButton);
            mButtons.Add(mStressUpButton);
            mButtons.Add(mWiggleButton);

            // Pregen some random positions and wiggles, and colors
            mRandomPos = new Vector2i[MAX_STRESS];
            for (int i = 0; i < MAX_STRESS; i++)
            {
                mRandomPos[i] = new Vector2i(
                    (int)Random.Range(-16, RB.DisplaySize.width),
                    (int)Random.Range(-16, RB.DisplaySize.height));
            }

            mRandomWiggle = new Vector2i[113];
            for (int i = 0; i < mRandomWiggle.Length; i++)
            {
                mRandomWiggle[i] = new Vector2i(Random.Range(-1, 2), Random.Range(-1, 2));
            }

            mRandomSprite = new int[MAX_STRESS];
            for (int i = 0; i < MAX_STRESS; i++)
            {
                mRandomSprite[i] = Random.Range(0, 4);
            }

            mRandomColor = new Color32[MAX_STRESS];
            for (int i = 0; i < MAX_STRESS; i++)
            {
                mRandomColor[i] = mPaletteLookup[Random.Range(1, mPaletteLookup.Length)];
            }

            mRandomRadius = new Vector2i[MAX_STRESS];
            for (int i = 0; i < MAX_STRESS; i++)
            {
                mRandomRadius[i] = new Vector2i(Random.Range(4, 64), Random.Range(4, 64));
            }
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

            for (int i = 0; i < mButtons.Count; i++)
            {
                mButtons[i].Reset();
            }

            mStressLevel = 500;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                Application.Quit();
            }

            if (RB.ButtonPressed(RB.BTN_UP, RB.PLAYER_ANY))
            {
                StressUp();
            }
            else if (RB.ButtonPressed(RB.BTN_DOWN, RB.PLAYER_ANY))
            {
                StressDown();
            }

            var demo = (StressTest)RB.Game;

            if (RB.ButtonPressed(RB.BTN_LEFT, RB.PLAYER_ANY))
            {
                demo.PreviousScene();
            }
            else if (RB.ButtonPressed(RB.BTN_RIGHT, RB.PLAYER_ANY))
            {
                demo.NextScene();
            }

            for (int i = 0; i < mButtons.Count; i++)
            {
                mButtons[i].Update();
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            RB.Clear(Color.black);

            StressTest();
            Overlay();
            if (RB.DisplaySize.width < 600)
            {
                return;
            }

            for (int i = 0; i < mButtons.Count; i++)
            {
                mButtons[i].Render();
            }

            mFrameDelta = Time.time - mPreviousFrame;
            mPreviousFrame = Time.time;

            if (mFrameDelta > 0)
            {
                mFPSSlots[mFPSSlot] = 1.0f / mFrameDelta;

                mFPSSlot++;

                if (mFPSSlot >= mFPSSlots.Length)
                {
                    mFPSSlot = 0;

                    float fpsTotal = 0;
                    for (int i = 0; i < mFPSSlots.Length; i++)
                    {
                        fpsTotal += mFPSSlots[i];
                    }

                    mFPS = fpsTotal / mFPSSlots.Length;
                }
            }
        }

        /// <summary>
        /// Run the stress test
        /// </summary>
        protected virtual void StressTest()
        {
        }

        /// <summary>
        /// Show informative overlay
        /// </summary>
        protected virtual void Overlay()
        {
        }

        /// <summary>
        /// Return random wiggle
        /// </summary>
        /// <returns>Random wiggle</returns>
        protected Vector2i GetWiggle()
        {
            if (!mWiggleOn)
            {
                return Vector2i.zero;
            }

            mWiggleCounter++;
            return mRandomWiggle[mWiggleCounter % mRandomWiggle.Length];
        }

        private void StressUp()
        {
            mStressLevel += mStressIncrement;
            if (mStressLevel > MAX_STRESS)
            {
                mStressLevel = MAX_STRESS;
            }
        }

        private void StressDown()
        {
            if (mStressLevel > mStressIncrement)
            {
                mStressLevel -= mStressIncrement;
                if (mStressLevel < 0)
                {
                    mStressLevel = 0;
                }
            }
        }

        private void NextScreenButtonCB(StressButton button, object userData)
        {
            var demo = (StressTest)RB.Game;
            demo.NextScene();
        }

        private void PreviousScreenButtonCB(StressButton button, object userData)
        {
            var demo = (StressTest)RB.Game;
            demo.PreviousScene();
        }

        private void StressUpButtonCB(StressButton button, object userData)
        {
            StressUp();
        }

        private void StressDownButtonCB(StressButton button, object userData)
        {
            StressDown();
        }

        private void WiggleButtonCB(StressButton button, object userData)
        {
            mWiggleOn = !mWiggleOn;
        }
    }
}
