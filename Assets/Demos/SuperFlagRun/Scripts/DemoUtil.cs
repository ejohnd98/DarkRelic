namespace RetroBlitDemoSuperFlagRun
{
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Demo utility methods
    /// </summary>
    public class DemoUtil
    {
        // RetroBlit default color palette if no palette is provided by user
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

        /// <summary>
        /// Color index to RGB
        /// </summary>
        /// <param name="color">Color index</param>
        /// <returns>RGB color</returns>
        public static Color32 IndexToRGB(int color)
        {
            if (color < 0 || color >= mPaletteLookup.Length)
            {
                return new Color32(255, 0, 255, 255);
            }

            return mPaletteLookup[color];
        }
    }
}
