namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A scene containing a message box
    /// </summary>
    public class SceneMessage : Scene
    {
        private Menu mMenuMessageBox;
        private ResultSet mResultSet;
        private List<OptionDelegate> mHandlers = new List<OptionDelegate>();

        private bool mShowingMessageBox = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneMessage()
        {
            mMenuMessageBox = new Menu(C.FSTR.Set("MessageBox"));
            mResultSet = new ResultSet();
        }

        /// <summary>
        /// Option delegate to call
        /// </summary>
        public delegate void OptionDelegate();

        /// <summary>
        /// Enter the scene
        /// </summary>
        /// <param name="parameters">Scene parameters</param>
        public override void Enter(object parameters)
        {
        }

        /// <summary>
        /// Exit scene
        /// </summary>
        public override void Exit()
        {
        }

        /// <summary>
        /// Show a message box
        /// </summary>
        /// <param name="header">Header text</param>
        /// <param name="message">Message text</param>
        /// <param name="options">Options</param>
        public void ShowMessageBox(FastString header, FastString message, List<MessageBoxOption> options)
        {
            mMenuMessageBox.SetHeader(header);
            mMenuMessageBox.SetSummary(message);
            mMenuMessageBox.ClearOptions();
            mMenuMessageBox.AddOption(C.FSTR.Set("Ok"));

            mMenuMessageBox.ClearOptions();
            mHandlers.Clear();

            for (int i = 0; i < options.Count; i++)
            {
                mMenuMessageBox.AddOption(options[i].option);
                mHandlers.Add(options[i].handler);
            }

            mShowingMessageBox = true;
        }

        /// <summary>
        /// Close message box
        /// </summary>
        public void CloseMessageBox()
        {
            mMenuMessageBox.ClearOptions();
            mHandlers.Clear();
            mShowingMessageBox = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns>True if update is consumed and should not be propagated to parent scenes</returns>
        public override bool Update()
        {
            bool wasShowing = mShowingMessageBox;

            HandleMenuKeys(mResultSet);

            if (wasShowing)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            if (mShowingMessageBox)
            {
                RB.DrawRectFill(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), new Color32(0, 0, 0, 200));
                mMenuMessageBox.Render();
            }
        }

        private void HandleMenuKeys(ResultSet resultSet)
        {
            if (!mShowingMessageBox)
            {
                return;
            }

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }

            // Check keyboard input
            for (char i = 'a'; i < 'a' + mMenuMessageBox.optionCount; i++)
            {
                if (RB.KeyPressed((KeyCode)i))
                {
                    int index = (int)(i - 'a');
                    if (mHandlers[index] != null)
                    {
                        mHandlers[index]();
                    }
                }
            }

            // Check mouse input
            if (RB.ButtonReleased(RB.BTN_POINTER_A))
            {
                int index = mMenuMessageBox.pointerIndex;
                if (index >= 0)
                {
                    if (mHandlers[index] != null)
                    {
                        mHandlers[index]();
                    }
                }
            }
        }

        /// <summary>
        /// A message box option
        /// </summary>
        public class MessageBoxOption
        {
            /// <summary>
            /// Option test
            /// </summary>
            public FastString option = new FastString(128);

            /// <summary>
            /// Option delegate to call
            /// </summary>
            public OptionDelegate handler;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="option">Option</param>
            /// <param name="handler">Delegate to call for the option</param>
            public MessageBoxOption(FastString option, OptionDelegate handler)
            {
                this.option.Set(option);
                this.handler = handler;
            }
        }
    }
}
