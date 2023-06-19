#if UNITY_WII || UNITY_SWITCH
#define ABXY_SWITCHED
#endif

namespace RetroBlitInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Input subsystem
    /// </summary>
    public class RBInput
    {
        /// <summary>
        /// Total tracked touches. If mouse is also connected then the first touch is always the mouse
        /// </summary>
        public const int MAX_TOUCHES = 4;

        private const int BUTTON_LAST = RB.BTN_POINTER_D;
        private const int MAX_KEY_CODE = (int)KeyCode.Menu;

        private int mButtonCount = 0;

        private RBAPI mRetroBlitAPI = null;

        private Dictionary<int, KeyState>[] mButtonPreviousState = new Dictionary<int, KeyState>[RBHardware.HW_MAX_PLAYERS];
        private Dictionary<int, KeyState>[] mButtonCurrentState = new Dictionary<int, KeyState>[RBHardware.HW_MAX_PLAYERS];

        private KeyState[] mKeyPreviousState = new KeyState[MAX_KEY_CODE + 1];
        private KeyState[] mKeyCurrentState = new KeyState[MAX_KEY_CODE + 1];

        private KeyState mAnyKeyPreviousState = new KeyState();
        private KeyState mAnyKeyCurrentState = new KeyState();

        private string mInputString = string.Empty;

        private PointerInfo[] mPointer = new PointerInfo[MAX_TOUCHES];
        private bool[] mProcessedTouch = new bool[16];

        private float mScrollDelta;

        private RB.InputOverrideMethod mOverrideMethod = null;

        private class KeyState
        {
            private bool down = false;
            private ulong tick = 0;

            public bool IsPressed()
            {
                return down;
            }

            public bool IsRepeat(RBAPI rbApi, int repeatRate)
            {
                if (down && repeatRate > 0)
                {
                    var ticksSince = rbApi.TicksInternal - tick;
                    if (ticksSince % (ulong)repeatRate == 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void SetPressed(RBAPI rbApi, bool pressed)
            {
                // Check if no change in state
                if (down == pressed) return;

                down = pressed;
                if (down)
                {
                    tick = rbApi.TicksInternal;
                }
                else
                {
                    tick = 0;
                }
            }

            public void Copy(KeyState other)
            {
                down = other.down;
                tick = other.tick;
            }

            public override string ToString()
            {
                return down.ToString() + " : " + tick.ToString();
            }
        }

        /// <summary>
        /// Initialize subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RBAPI api)
        {
            mRetroBlitAPI = api;

            if (!VerifyInputManagerSettings())
            {
                Debug.LogError("InputManager is not setup properly for RetroBlit. Please import RetroBlit project as a complete project from the Asset Manager in order to also import the RetroBlit InputManager settings. Alternatively you can copy ProjectSettings/InputManager.asset from another RetroBlit project.");
                return false;
            }

            mButtonCount = (int)Mathf.Log(BUTTON_LAST, 2) + 1;

            for (int i = 0; i < RBHardware.HW_MAX_PLAYERS; i++)
            {
                mButtonCurrentState[i] = new Dictionary<int, KeyState>();
                mButtonPreviousState[i] = new Dictionary<int, KeyState>();

                for (int b = 1; b <= BUTTON_LAST;)
                {
                    mButtonCurrentState[i].Add(b, new KeyState());
                    mButtonPreviousState[i].Add(b, new KeyState());
                    b = b << 1;
                }
            }

            for (int i = 0; i < MAX_TOUCHES; i++)
            {
                mPointer[i].Reset();
            }

            for (int i = 0; i < MAX_KEY_CODE; i++)
            {
                mKeyCurrentState[i] = new KeyState();
                mKeyPreviousState[i] = new KeyState();
            }

            GetAllButtonStates(mButtonCurrentState);

            // Disable Unity mouse simulation with touches, we'll handle this ourselves.
            UnityEngine.Input.simulateMouseWithTouches = false;

            return true;
        }

        /// <summary>
        /// Check if button is down
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="player">Player number</param>
        /// <returns>True if down</returns>
        public bool ButtonDown(int button, int player)
        {
            return FetchButtonState(button, player);
        }

        /// <summary>
        /// Check if button was pressed since last update
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="player">Player number</param>
        /// <returns>True if pressed</returns>
        public bool ButtonPressed(int button, int player, int repeatRate)
        {
            if (CheckButtonState(button, player, mButtonCurrentState, 0) && !CheckButtonState(button, player, mButtonPreviousState, 0))
            {
                return true;
            }

            if (repeatRate > 0)
            {
                if (CheckButtonState(button, player, mButtonCurrentState, repeatRate))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if button was released since last update
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="player">Player number</param>
        /// <returns>True if released</returns>
        public bool ButtonReleased(int button, int player)
        {
            if (!CheckButtonState(button, player, mButtonCurrentState, 0) && CheckButtonState(button, player, mButtonPreviousState, 0))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if key is down
        /// </summary>
        /// <param name="keyCode">Keycode</param>
        /// <returns>True if down</returns>
        public bool KeyDown(KeyCode keyCode)
        {
            if ((int)keyCode < 0 || (int)keyCode >= mKeyCurrentState.Length)
            {
                return false;
            }

            if ((int)keyCode > MAX_KEY_CODE)
            {
                return false;
            }

            return UnityEngine.Input.GetKey(keyCode);
        }

        /// <summary>
        /// Check if key was pressed since last update
        /// </summary>
        /// <param name="keyCode">Keycode</param>
        /// <returns>True if pressed</returns>
        public bool KeyPressed(KeyCode keyCode, int repeatRate = 0)
        {
            if ((int)keyCode < 0 || (int)keyCode >= mKeyCurrentState.Length)
            {
                return false;
            }

            if (mKeyCurrentState[(int)keyCode].IsPressed() && !mKeyPreviousState[(int)keyCode].IsPressed())
            {
                return true;
            }

            if (mKeyCurrentState[(int)keyCode].IsRepeat(mRetroBlitAPI, repeatRate))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if key was released since last update
        /// </summary>
        /// <param name="keyCode">Keycode</param>
        /// <returns>True if released</returns>
        public bool KeyReleased(KeyCode keyCode)
        {
            if ((int)keyCode < 0 || (int)keyCode >= mKeyCurrentState.Length)
            {
                return false;
            }

            if (!mKeyCurrentState[(int)keyCode].IsPressed() && mKeyPreviousState[(int)keyCode].IsPressed())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if any key is down
        /// </summary>
        /// <returns>True if down</returns>
        public bool AnyKeyDown()
        {
            return mAnyKeyCurrentState.IsPressed();
        }

        /// <summary>
        /// Check if any key was pressed since last update
        /// </summary>
        /// <returns>True if pressed</returns>
        public bool AnyKeyPressed(int repeatRate = 0)
        {
            if (mAnyKeyCurrentState.IsPressed() && !mAnyKeyPreviousState.IsPressed())
            {
                return true;
            }

            if (mAnyKeyCurrentState.IsRepeat(mRetroBlitAPI, repeatRate))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if any key was released since last update
        /// </summary>
        /// <returns>True if released</returns>
        public bool AnyKeyReleased()
        {
            if (!mAnyKeyCurrentState.IsPressed() && mAnyKeyPreviousState.IsPressed())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get pointer (mouse or touch) position
        /// </summary>
        /// <param name="touchIndex">Touch index</param>
        /// <returns>Position</returns>
        public Vector2i PointerPos(int touchIndex)
        {
            return mPointer[touchIndex].pos;
        }

        /// <summary>
        /// Check if pointer position is valid. It's not valid if there is no pointer devices, or touch screen is not pressed
        /// </summary>
        /// <param name="touchIndex">Touch index</param>
        /// <returns>True if valid</returns>
        public bool PointerPosValid(int touchIndex)
        {
            return mPointer[touchIndex].valid;
        }

        /// <summary>
        /// Set input override method
        /// </summary>
        /// <param name="overrideMethod">Override method</param>
        public void InputOverride(RB.InputOverrideMethod overrideMethod)
        {
            mOverrideMethod = overrideMethod;
        }

        /// <summary>
        /// New update frame just started, setup input states
        /// </summary>
        public void FrameStart()
        {
            bool anyKeyDown = false;

            // Process touches and touch/mouse positions first
            for (int i = 0; i < MAX_TOUCHES; i++)
            {
                mPointer[i].updatedThisFrame = false;
            }

            Vector2 mousePos = Vector2.zero;

            // Mouse goes first, and always takes first slot if its available
            if (UnityEngine.Input.mousePresent)
            {
                var rawMousePos = UnityEngine.Input.mousePosition;
                mPointer[0].pos = mRetroBlitAPI.PresentCamera.ScreenToViewportPoint(rawMousePos);
                mPointer[0].fingerId = int.MinValue;
                mPointer[0].valid = true;
                mPointer[0].updatedThisFrame = true;
            }

            if (UnityEngine.Input.touchCount > 0)
            {
                anyKeyDown = true;

                for (int i = 0; i < mProcessedTouch.Length; i++)
                {
                    mProcessedTouch[i] = false;
                }

                for (int i = 0; i < UnityEngine.Input.touchCount && i < mProcessedTouch.Length; i++)
                {
                    var touch = UnityEngine.Input.touches[i];
                    bool pressed = touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;

                    // Find existing touch to update
                    for (int j = 0; j < MAX_TOUCHES; j++)
                    {
                        if (mPointer[j].fingerId == touch.fingerId)
                        {
                            mPointer[j].pos = mRetroBlitAPI.PresentCamera.ScreenToViewportPoint(touch.position);
                            mPointer[j].pressed = pressed;
                            mPointer[j].valid = true;
                            mPointer[j].fingerId = touch.fingerId;
                            mPointer[j].updatedThisFrame = true;

                            mProcessedTouch[i] = true;
                            break;
                        }
                    }
                }

                for (int i = 0; i < UnityEngine.Input.touchCount && i < mProcessedTouch.Length; i++)
                {
                    if (mProcessedTouch[i])
                    {
                        continue;
                    }

                    var touch = UnityEngine.Input.touches[i];
                    bool pressed = touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;

                    // If this is a new touch then add it in the first empty spot, or drop it if full
                    if (pressed)
                    {
                        for (int j = 0; j < MAX_TOUCHES; j++)
                        {
                            if (!mPointer[j].valid)
                            {
                                mPointer[j].pos = mRetroBlitAPI.PresentCamera.ScreenToViewportPoint(touch.position);
                                mPointer[j].pressed = pressed;
                                mPointer[j].valid = true;
                                mPointer[j].fingerId = touch.fingerId;
                                mPointer[j].updatedThisFrame = true;

                                mProcessedTouch[i] = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Reset all pointer information for pointers that did not have any information updated this frame
            for (int i = 0; i < MAX_TOUCHES; i++)
            {
                if (!mPointer[i].updatedThisFrame)
                {
                    mPointer[i].Reset();
                }
            }

            // Copy all previous button values
            for (int p = 0; p < RBHardware.HW_MAX_PLAYERS; p++)
            {
                for (int k = 0; k < mButtonCount; k++)
                {
                    int key = 1 << k;
                    mButtonPreviousState[p][key].Copy(mButtonCurrentState[p][key]);
                }
            }

            // Copy all previous key values
            for (int i = 0; i < MAX_KEY_CODE; i++)
            {
                mKeyPreviousState[i].Copy(mKeyCurrentState[i]);
            }

            if (GetAllKeyStates(mKeyCurrentState))
            {
                anyKeyDown = true;
            }

            // Update current values
            if (GetAllButtonStates(mButtonCurrentState))
            {
                anyKeyDown = true;
            }

            mAnyKeyPreviousState.Copy(mAnyKeyCurrentState);
            mAnyKeyCurrentState.SetPressed(mRetroBlitAPI, anyKeyDown);
        }

        /// <summary>
        /// Update frame just ended, reset some parameters
        /// </summary>
        public void FrameEnd()
        {
            mInputString = string.Empty;
            mScrollDelta = 0;
        }

        /// <summary>
        /// Append to input string for the frame
        /// </summary>
        /// <param name="str">String to append with</param>
        public void AppendInputString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                // Filter out invalid ascii characters
                if ((str[i] >= ' ' && str[i] <= 127) || str[i] == 0x8 || str[i] == 0x9 || str[i] == 0xD || str[i] == 0x1B)
                {
                    mInputString += str[i];
                }
            }
        }

        /// <summary>
        /// Get input string for this frame
        /// </summary>
        /// <returns>Input string</returns>
        public string InputString()
        {
            return mInputString;
        }

        /// <summary>
        /// Update scroll wheel delta
        /// </summary>
        public void UpdateScrollWheel()
        {
            mScrollDelta += UnityEngine.Input.mouseScrollDelta.y;
        }

        /// <summary>
        /// Get scroll delta
        /// </summary>
        /// <returns>Scroll delta</returns>
        public float PointerScrollDelta()
        {
            return mScrollDelta;
        }

        /// <summary>
        /// Verify that we can read all inputs, if not then InputManager is probably not configured correctly
        /// </summary>
        /// <returns>True if we can read inputs</returns>
        private bool VerifyInputManagerSettings()
        {
            try
            {
                // Poke all named buttons and axes, if any are not defined they will throw an exception, at which point we
                // can report a failure
                UnityEngine.Input.GetButton("SYSTEM");

                // Player 1
                UnityEngine.Input.GetAxisRaw("P1_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P1_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P1_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P1_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL_DPAD");
                UnityEngine.Input.GetButton("P1_A");
                UnityEngine.Input.GetButton("P1_B");
                UnityEngine.Input.GetButton("P1_X");
                UnityEngine.Input.GetButton("P1_Y");
                UnityEngine.Input.GetButton("P1_LS");
                UnityEngine.Input.GetAxisRaw("P1_LS_TRIGGER");
                UnityEngine.Input.GetButton("P1_RS");
                UnityEngine.Input.GetAxisRaw("P1_RS_TRIGGER");
                UnityEngine.Input.GetButton("P1_MENU");

                // Player 2
                UnityEngine.Input.GetAxisRaw("P2_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P2_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P2_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P2_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL_DPAD");
                UnityEngine.Input.GetButton("P2_A");
                UnityEngine.Input.GetButton("P2_B");
                UnityEngine.Input.GetButton("P2_X");
                UnityEngine.Input.GetButton("P2_Y");
                UnityEngine.Input.GetButton("P2_LS");
                UnityEngine.Input.GetAxisRaw("P2_LS_TRIGGER");
                UnityEngine.Input.GetButton("P2_RS");
                UnityEngine.Input.GetAxisRaw("P2_RS_TRIGGER");
                UnityEngine.Input.GetButton("P2_MENU");

                // Player 3
                UnityEngine.Input.GetAxisRaw("P3_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P3_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P3_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P3_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL_DPAD");
                UnityEngine.Input.GetButton("P3_A");
                UnityEngine.Input.GetButton("P3_B");
                UnityEngine.Input.GetButton("P3_X");
                UnityEngine.Input.GetButton("P3_Y");
                UnityEngine.Input.GetButton("P3_LS");
                UnityEngine.Input.GetAxisRaw("P3_LS_TRIGGER");
                UnityEngine.Input.GetButton("P3_RS");
                UnityEngine.Input.GetAxisRaw("P3_RS_TRIGGER");
                UnityEngine.Input.GetButton("P3_MENU");

                // Player 4
                UnityEngine.Input.GetAxisRaw("P4_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P4_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P4_VERTICAL");
                UnityEngine.Input.GetAxisRaw("P4_VERTICAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL_DPAD");
                UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL");
                UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL_DPAD");
                UnityEngine.Input.GetButton("P4_A");
                UnityEngine.Input.GetButton("P4_B");
                UnityEngine.Input.GetButton("P4_X");
                UnityEngine.Input.GetButton("P4_Y");
                UnityEngine.Input.GetButton("P4_LS");
                UnityEngine.Input.GetAxisRaw("P4_LS_TRIGGER");
                UnityEngine.Input.GetButton("P4_RS");
                UnityEngine.Input.GetAxisRaw("P4_RS_TRIGGER");
                UnityEngine.Input.GetButton("P4_MENU");

                return true;
            }
            catch (System.Exception e)
            {
                // If an axis or button is undefined we expect an ArgumentException
                if (e is System.ArgumentException)
                {
                    return false;
                }

                // Other exception are unexpected, log them.
                Debug.LogError(e.ToString());

                return false;
            }
        }

        private bool CheckButtonState(int button, int player, Dictionary<int, KeyState>[] states, int repeatRate)
        {
            // Check if any buttons are pressed for any of the given players, if so then return true
            for (int p = 0; p < RBHardware.HW_MAX_PLAYERS; p++)
            {
                if ((player & (1 << p)) != 0)
                {
                    for (int b = 1; b <= BUTTON_LAST;)
                    {
                        if (repeatRate <= 0)
                        {
                            if ((button & b) != 0 && states[p][b].IsPressed())
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if ((button & b) != 0 && states[p][b].IsRepeat(mRetroBlitAPI, repeatRate))
                            {
                                return true;
                            }
                        }

                        b = b << 1;
                    }
                }
            }

            return false;
        }

        private bool FetchButtonState(int button, int player)
        {
            float joystickThreshold = 0.75f;

            // Check if user input overwrite already handles this input
            if (mOverrideMethod != null)
            {
                bool overrideHandled = false;
                bool ret = mOverrideMethod(button, player, out overrideHandled);
                if (overrideHandled)
                {
                    return ret;
                }
            }

            if ((button & RB.BTN_SYSTEM) != 0)
            {
                if (UnityEngine.Input.GetKey(KeyCode.Escape))
                {
                    return true;
                }

                if (UnityEngine.Input.GetButton("SYSTEM"))
                {
                    return true;
                }
            }

            if ((player & RB.PLAYER_ONE) != 0)
            {
                if ((button & RB.BTN_UP) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.W))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P1_VERTICAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P1_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_DOWN) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.S))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P1_VERTICAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P1_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_LEFT) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.A))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RIGHT) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.D))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P1_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_A) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.B))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Space))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P1_B"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P1_A"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_B) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.N))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P1_A"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P1_B"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_X) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.G))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P1_Y"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P1_X"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_Y) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.H))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P1_X"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P1_Y"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_LS) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.T))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetButton("P1_LS") || UnityEngine.Input.GetAxisRaw("P1_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RS) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.Y))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetButton("P1_RS") || UnityEngine.Input.GetAxisRaw("P1_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_MENU) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.Alpha5))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetButton("P1_MENU"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_POINTER_A) != 0)
                {
                    if (UnityEngine.Input.GetMouseButton(0) || mPointer[0].pressed)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_POINTER_B) != 0)
                {
                    if (UnityEngine.Input.GetMouseButton(1) || mPointer[1].pressed)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_POINTER_C) != 0)
                {
                    if (UnityEngine.Input.GetMouseButton(2) || mPointer[2].pressed)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_POINTER_D) != 0)
                {
                    if (mPointer[3].pressed)
                    {
                        return true;
                    }
                }
            }

            if ((player & RB.PLAYER_TWO) != 0)
            {
                if ((button & RB.BTN_UP) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P2_VERTICAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P2_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_DOWN) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P2_VERTICAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P2_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_LEFT) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RIGHT) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P2_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_A) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.Semicolon))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Keypad1))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.RightControl))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P2_B"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P2_A"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_B) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.Quote))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Keypad2))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P2_A"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P2_B"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_X) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.P))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Keypad4))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P2_Y"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P2_X"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_Y) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.LeftBracket))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Keypad5))
                    {
                        return true;
                    }

#if ABXY_SWITCHED
                    if (UnityEngine.Input.GetButton("P2_X"))
                    {
                        return true;
                    }
#else
                    if (UnityEngine.Input.GetButton("P2_Y"))
                    {
                        return true;
                    }
#endif
                }

                if ((button & RB.BTN_LS) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.Alpha0))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Keypad7))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetButton("P2_LS") || UnityEngine.Input.GetAxisRaw("P2_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RS) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.Minus))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Keypad8))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetButton("P2_RS") || UnityEngine.Input.GetAxisRaw("P2_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_MENU) != 0)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.Backspace))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.KeypadDivide))
                    {
                        return true;
                    }

                    if (UnityEngine.Input.GetButton("P2_MENU"))
                    {
                        return true;
                    }
                }
            }

            if ((player & RB.PLAYER_THREE) != 0)
            {
                if ((button & RB.BTN_UP) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P3_VERTICAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P3_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_DOWN) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P3_VERTICAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P3_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_LEFT) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RIGHT) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P3_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_A) != 0)
                {
                    if (UnityEngine.Input.GetButton("P3_A"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_B) != 0)
                {
                    if (UnityEngine.Input.GetButton("P3_B"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_X) != 0)
                {
                    if (UnityEngine.Input.GetButton("P3_X"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_Y) != 0)
                {
                    if (UnityEngine.Input.GetButton("P3_Y"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_LS) != 0)
                {
                    if (UnityEngine.Input.GetButton("P3_LS") || UnityEngine.Input.GetAxisRaw("P3_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RS) != 0)
                {
                    if (UnityEngine.Input.GetButton("P3_RS") || UnityEngine.Input.GetAxisRaw("P3_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_MENU) != 0)
                {
                    if (UnityEngine.Input.GetButton("P3_MENU"))
                    {
                        return true;
                    }
                }
            }

            if ((player & RB.PLAYER_FOUR) != 0)
            {
                if ((button & RB.BTN_UP) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P4_VERTICAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P4_VERTICAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_DOWN) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P4_VERTICAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P4_VERTICAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_LEFT) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL") < -joystickThreshold || UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL_DPAD") < -joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RIGHT) != 0)
                {
                    if (UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL") > joystickThreshold || UnityEngine.Input.GetAxisRaw("P4_HORIZONTAL_DPAD") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_A) != 0)
                {
                    if (UnityEngine.Input.GetButton("P4_A"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_B) != 0)
                {
                    if (UnityEngine.Input.GetButton("P4_B"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_X) != 0)
                {
                    if (UnityEngine.Input.GetButton("P4_X"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_Y) != 0)
                {
                    if (UnityEngine.Input.GetButton("P4_Y"))
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_LS) != 0)
                {
                    if (UnityEngine.Input.GetButton("P4_LS") || UnityEngine.Input.GetAxisRaw("P4_LS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_RS) != 0)
                {
                    if (UnityEngine.Input.GetButton("P4_RS") || UnityEngine.Input.GetAxisRaw("P4_RS_TRIGGER") > joystickThreshold)
                    {
                        return true;
                    }
                }

                if ((button & RB.BTN_MENU) != 0)
                {
                    if (UnityEngine.Input.GetButton("P4_MENU"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GetAllButtonStates(Dictionary<int, KeyState>[] states)
        {
            bool down = false;
            bool any = false;

            for (int p = 0; p < RBHardware.HW_MAX_PLAYERS; p++)
            {
                for (int b = 1; b <= BUTTON_LAST;)
                {
                    down = FetchButtonState(b, 1 << p);
                    states[p][b].SetPressed(mRetroBlitAPI, down);
                    b = b << 1;

                    if (down)
                    {
                        any = true;
                    }
                }
            }

            return any;
        }

        private bool GetAllKeyStates(KeyState[] keyStates)
        {
            bool down = false;
            bool any = false;

            for (int i = 0; i < MAX_KEY_CODE; i++)
            {
                down = UnityEngine.Input.GetKey((KeyCode)i);
                keyStates[i].SetPressed(mRetroBlitAPI, down);

                if (down)
                {
                    any = true;
                }
            }

            return any;
        }

        private struct PointerInfo
        {
            public Vector2 pos;
            public bool valid;
            public int fingerId;
            public bool pressed;
            public bool updatedThisFrame;

            public void Reset()
            {
                pos = Vector2.zero;
                valid = false;
                fingerId = int.MinValue;
                pressed = false;
                updatedThisFrame = false;
            }
        }
    }
}
