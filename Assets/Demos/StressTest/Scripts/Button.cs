namespace RetroBlitDemoStressTest
{
    using UnityEngine;

    /// <summary>
    /// A basic UI button for demo purposes
    /// </summary>
    public class StressButton
    {
        private Rect2i mRect;
        private Rect2i mHotZone;
        private KeyCode mKeyCode;
        private StressButtonPressedCB mButtonPressedCB;
        private StressButtonReleasedCB mButtonReleasedCB;
        private Rect2i mHitRect;
        private string mLabel;
        private bool mLabelBottomAligned;
        private object mUserData;
        private bool mTouchArmed = false;

        private bool mPressed;
        private bool mTouched;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect">Rectangular area of button</param>
        /// <param name="hotZone">Interactable hot zone</param>
        /// <param name="label">Button label</param>
        /// <param name="keyCode">KeyCode to map to</param>
        /// <param name="userData">User data</param>
        /// <param name="buttonPressed">Button pressed delegate</param>
        /// <param name="buttonReleased">Button released delegate</param>
        /// <param name="labelBottomAligned">Is label bottom aligned, or centered?</param>
        public StressButton(Rect2i rect, Rect2i hotZone, string label, KeyCode keyCode, object userData, StressButtonPressedCB buttonPressed = null, StressButtonReleasedCB buttonReleased = null, bool labelBottomAligned = false)
        {
            mRect = rect;
            mHotZone = hotZone;
            mKeyCode = keyCode;
            mLabel = label;
            mUserData = userData;
            mLabelBottomAligned = labelBottomAligned;

            mButtonPressedCB = buttonPressed;
            mButtonReleasedCB = buttonReleased;
        }

        /// <summary>
        /// Button pressed delegate
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="userData">User data</param>
        public delegate void StressButtonPressedCB(StressButton button, object userData);

        /// <summary>
        /// Button released delegate
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="userData">User data</param>
        public delegate void StressButtonReleasedCB(StressButton button, object userData);

        /// <summary>
        /// Get/Set the button label
        /// </summary>
        public string Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        /// <summary>
        /// Reset button state
        /// </summary>
        public void Reset()
        {
            mPressed = mTouched = mTouchArmed = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (RB.KeyPressed(mKeyCode))
            {
                if (!mPressed && mButtonPressedCB != null)
                {
                    mButtonPressedCB(this, mUserData);
                }

                mPressed = true;
            }
            else if (RB.KeyReleased(mKeyCode))
            {
                if (mPressed && mButtonReleasedCB != null)
                {
                    mButtonReleasedCB(this, mUserData);
                }

                mPressed = false;
            }

            if (mTouchArmed)
            {
                if (RB.ButtonDown(RB.BTN_POINTER_A) && mHitRect.Contains(RB.PointerPos()))
                {
                    if (!mPressed && mButtonPressedCB != null)
                    {
                        mButtonPressedCB(this, mUserData);
                    }

                    mPressed = true;
                    mTouched = true;
                }
                else if ((!RB.ButtonDown(RB.BTN_POINTER_A) || !mHitRect.Contains(RB.PointerPos())) && mTouched)
                {
                    if (mPressed && mButtonReleasedCB != null)
                    {
                        mButtonReleasedCB(this, mUserData);
                    }

                    mTouched = false;
                    mPressed = false;
                }
            }
            else if (!RB.ButtonDown(RB.BTN_POINTER_A))
            {
                mTouchArmed = true;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            Vector2i labelSize = RB.PrintMeasure(mLabel);

            int yPos;
            if (mLabelBottomAligned)
            {
                yPos = mRect.y + mRect.height - labelSize.y - 4;
            }
            else
            {
                yPos = mRect.y + (mRect.height / 2) - (labelSize.y / 2);
            }

            if (mPressed)
            {
                RB.DrawRectFill(new Rect2i(mRect.x + 2, mRect.y + 2, mRect.width - 2, mRect.height - 2), Color.gray);
                RB.Print(new Vector2i(mRect.x + (mRect.width / 2) - (labelSize.x / 2) + 1, yPos + 1), Color.black, mLabel);
            }
            else
            {
                RB.DrawRectFill(mRect, Color.gray);
                RB.Print(new Vector2i(mRect.x + (mRect.width / 2) - (labelSize.x / 2), yPos), Color.black, mLabel);
                RB.DrawRect(mRect, Color.white);
            }

            mHitRect = new Rect2i(-RB.CameraGet().x + mHotZone.x, -RB.CameraGet().y + mHotZone.y, mHotZone.width, mHotZone.height);
        }
    }
}
