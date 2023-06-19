namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// A basic UI button for demo purposes
    /// </summary>
    public class Button
    {
        private Rect2i mRect;
        private Rect2i mHotZone;
        private KeyCode mKeyCode;
        private int mFaceColor;
        private int mLabelColor;
        private ButtonPressedCB mButtonPressedCB;
        private ButtonReleasedCB mButtonReleasedCB;
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
        /// <param name="faceColor">Color of button face</param>
        /// <param name="labelColor">Color of button label text</param>
        /// <param name="label">Button label</param>
        /// <param name="keyCode">KeyCode to map to</param>
        /// <param name="userData">User data</param>
        /// <param name="buttonPressed">Button pressed delegate</param>
        /// <param name="buttonReleased">Button released delegate</param>
        /// <param name="labelBottomAligned">Is label bottom aligned, or centered?</param>
        public Button(Rect2i rect, Rect2i hotZone, int faceColor, int labelColor, string label, KeyCode keyCode, object userData, ButtonPressedCB buttonPressed = null, ButtonReleasedCB buttonReleased = null, bool labelBottomAligned = false)
        {
            mRect = rect;
            mHotZone = hotZone;
            mKeyCode = keyCode;
            mFaceColor = faceColor;
            mLabelColor = labelColor;
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
        public delegate void ButtonPressedCB(Button button, object userData);

        /// <summary>
        /// Button released delegate
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="userData">User data</param>
        public delegate void ButtonReleasedCB(Button button, object userData);

        /// <summary>
        /// Get/Set the button label
        /// </summary>
        public string Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        /// <summary>
        /// Get/Set label color
        /// </summary>
        public int LabelColor
        {
            get { return mLabelColor; }
            set { mLabelColor = value; }
        }

        /// <summary>
        /// True if button is pressed
        /// </summary>
        public bool Pressed
        {
            get { return mPressed; }
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
            var demo = (DemoReel)RB.Game;

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
                RB.DrawRectFill(new Rect2i(mRect.x + 2, mRect.y + 2, mRect.width - 2, mRect.height - 2), DemoUtil.IndexToRGB(5));
                RB.Print(new Vector2i(mRect.x + (mRect.width / 2) - (labelSize.x / 2) + 1, yPos + 1), DemoUtil.IndexToRGB(mLabelColor), mLabel);
            }
            else
            {
                RB.DrawRectFill(mRect, DemoUtil.IndexToRGB(mFaceColor));
                RB.Print(new Vector2i(mRect.x + (mRect.width / 2) - (labelSize.x / 2), yPos), DemoUtil.IndexToRGB(mLabelColor), mLabel);
                RB.DrawRect(mRect, DemoUtil.IndexToRGB(5));
            }

            mHitRect = new Rect2i(-RB.CameraGet().x + mHotZone.x, -RB.CameraGet().y + mHotZone.y, mHotZone.width, mHotZone.height);
        }
    }
}
