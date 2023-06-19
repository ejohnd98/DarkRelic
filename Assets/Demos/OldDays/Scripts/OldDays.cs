namespace RetroBlitDemo
{
    using UnityEngine;

    /// <summary>
    /// Hello World example program
    /// </summary>
    public class OldDays : RB.IRetroBlitGame
    {
        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware settings</returns>
        public RB.HardwareSettings QueryHardware()
        {
            var hw = new RB.HardwareSettings();

            hw.DisplaySize = new Vector2i(512 / 2, 384 / 2);

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>Return true if successful</returns>
        public bool Initialize()
        {
            RB.PresentDisable();

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            RB.Clear(Color.black);

            RB.Print(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, "@w3Old Days");
            RB.Print(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height), Color.white, 0, "@w3Older Days");
        }
    }
}
