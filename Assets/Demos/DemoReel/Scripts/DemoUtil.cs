namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demo utility methods
    /// </summary>
    public class DemoUtil
    {
        private static string mSymbols = "{}()=+-*/&^%|[],;<>?";

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
        /// Highlight code in given string
        /// </summary>
        /// <param name="inStr">Input string</param>
        /// <param name="outStr">Output string</param>
        /// <param name="highlightSymbols">Highlight symbols</param>
        /// <returns>Highlighted code</returns>
        public static FastString HighlightCode(FastString inStr, FastString outStr, bool highlightSymbols = true)
        {
            var game = (DemoReel)RB.Game;

            var buf = inStr.Buf;
            outStr.Clear();

            int lastColor = -1;

            for (int i = 0; i < inStr.Length; i++)
            {
                if (buf[i] == '@')
                {
                    i++;
                    switch (buf[i])
                    {
                        case 'I': // Indexed color
                            int colorIndex = ((int)(buf[i + 1] - '0') * 10) + (int)(buf[i + 2] - '0');

                            Color32 rgb = IndexToRGB(colorIndex);
                            IndexToColorString(21, outStr).Append("new");
                            IndexToColorString(22, outStr).Append(" Color32");
                            IndexToColorString(3, outStr).Append("(");
                            IndexToColorString(24, outStr).Append(rgb.r);
                            IndexToColorString(5, outStr).Append(", ");
                            IndexToColorString(24, outStr).Append(rgb.g);
                            IndexToColorString(5, outStr).Append(", ");
                            IndexToColorString(24, outStr).Append(rgb.b);
                            IndexToColorString(5, outStr).Append(", ");
                            IndexToColorString(24, outStr).Append(255);
                            IndexToColorString(3, outStr).Append(")");

                            i += 2;

                            break;

                        case 'K': // Keyword
                            IndexToColorString(21, outStr);
                            lastColor = 21;
                            break;

                        case 'M': // Class or Method
                            IndexToColorString(22, outStr);
                            lastColor = 22;
                            break;

                        case 'L': // Literal
                            IndexToColorString(24, outStr);
                            lastColor = 24;
                            break;

                        case 'C': // Comment
                            IndexToColorString(25, outStr);
                            lastColor = 25;
                            break;

                        case 'S': // String literal
                            IndexToColorString(15, outStr);
                            lastColor = 15;
                            break;

                        case 's': // String escape code
                            IndexToColorString(14, outStr);
                            lastColor = 14;
                            break;

                        case 'E':
                            IndexToColorString(23, outStr);
                            lastColor = 23;
                            break;

                        case 'N': // Normal/Other
                            IndexToColorString(5, outStr);
                            lastColor = 5;
                            break;

                        case 'D': // Dark
                            IndexToColorString(3, outStr);
                            lastColor = 3;
                            break;

                        case '@':
                            outStr.Append("@@");
                            break;

                        default:
                            IndexToColorString(5, outStr);
                            lastColor = 5;
                            break;
                    }
                }
                else
                {
                    var c = buf[i];

                    var symbolFound = false;

                    // Don't check for symbol color if in comment, or in string literal
                    if (highlightSymbols && lastColor != 25 && lastColor != 24)
                    {
                        for (int j = 0; j < mSymbols.Length; j++)
                        {
                            if (c == mSymbols[j])
                            {
                                symbolFound = true;
                                break;
                            }
                        }
                    }

                    if (symbolFound)
                    {
                        IndexToColorString(3, outStr);
                        outStr.Append(c);
                        IndexToColorString(5, outStr);
                    }
                    else
                    {
                        outStr.Append(buf[i]);
                    }
                }
            }

            return outStr;
        }

        /// <summary>
        /// Draw output frame
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="frameOuter">Outer frame color</param>
        /// <param name="frameInner">Inner frame color</param>
        /// <param name="fillColorIndex">Fill color</param>
        public static void DrawOutputFrame(Rect2i rect, int frameOuter, int frameInner, int fillColorIndex)
        {
            var demo = (DemoReel)RB.Game;

            if (frameOuter != -1)
            {
                RB.DrawRect(new Rect2i(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4), IndexToRGB(frameOuter));
            }

            if (frameInner != -1)
            {
                RB.DrawRect(new Rect2i(rect.x - 1, rect.y - 1, rect.width + 2, rect.height + 2), IndexToRGB(frameInner));
            }

            RB.DrawRectFill(rect, IndexToRGB(fillColorIndex));
        }

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

        /// <summary>
        /// Color index to color string
        /// </summary>
        /// <param name="color">Color index</param>
        /// <param name="outStr">Output string</param>
        /// <returns>String</returns>
        public static FastString IndexToColorString(int color, FastString outStr)
        {
            var demo = (DemoReel)RB.Game;

            Color32 rgb = IndexToRGB(color);
            outStr.Append("@").Append(rgb.r, 2, FastString.FORMAT_HEX_CAPS).Append(rgb.g, 2, FastString.FORMAT_HEX_CAPS).Append(rgb.b, 2, FastString.FORMAT_HEX_CAPS);

            return outStr;
        }
    }
}
