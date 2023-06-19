namespace RetroBlitInternal
{
    using System.Collections.Generic;

    /// <summary>
    /// Built in font class
    /// </summary>
    public class RBFontBuiltin
    {
        private const int SYSTEM_FONT_RECT_STRIDE = 13;
        private const int SPACE_WIDTH = 2;
        private GlyphRect[] mSystemFontRects;
        private Vector2i mGlyphSize = new Vector2i(8, 6);

        /// <summary>
        /// Get space width
        /// </summary>
        public int SpaceWidth
        {
            get
            {
                return SPACE_WIDTH;
            }
        }

        /// <summary>
        /// Initialize all built-in fonts
        /// </summary>
        /// <param name='api'>RetroBlit API</param>
        /// <param name='systemFont'>System font asset</param>
        public void InitializeBuiltinFonts(RBAPI api, FontAsset systemFont)
        {
            List<char> characterList = new List<char>();

            for (char c = '\u0020'; c <= '\u007E'; c++)
            {
                characterList.Add(c);
            }

            characterList.Add('\u3001');
            characterList.Add('\u3002');

            for (char c = '\u30A0'; c <= '\u30FF'; c++)
            {
                characterList.Add(c);
            }

            for (char c = '\u00A2'; c <= '\u00A5'; c++)
            {
                characterList.Add(c);
            }

            characterList.Add('\u20A0');
            characterList.Add('\u20A3');
            characterList.Add('\u20A8');
            characterList.Add('\u20AC');
            characterList.Add('\u20BF');
            characterList.Add('\u2022');
            characterList.Add('\u2023');
            characterList.Add('\u25E6');
            characterList.Add('\u2043');
            characterList.Add('\u2219');
            characterList.Add('\u00A9');
            characterList.Add('\u2122');
            characterList.Add('\u2639');
            characterList.Add('\u263A');

            for (char c = '\u2190'; c <= '\u2193'; c++)
            {
                characterList.Add(c);
            }

            for (char c = '\u2196'; c <= '\u2199'; c++)
            {
                characterList.Add(c);
            }

            for (char c = '\u21E6'; c <= '\u21E9'; c++)
            {
                characterList.Add(c);
            }

            characterList.Add('\u23CE');

            for (char c = '\u2660'; c <= '\u2667'; c++)
            {
                characterList.Add(c);
            }

            for (char c = '\u2669'; c <= '\u266B'; c++)
            {
                characterList.Add(c);
            }

            characterList.Add('\u23F5');

            for (char c = '\u23F8'; c <= '\u23FA'; c++)
            {
                characterList.Add(c);
            }

            characterList.Add('\u00B0');
            characterList.Add('\u00F7');
            characterList.Add('\u00B1');
            characterList.Add('\u0394');
            characterList.Add('\u03A9');
            characterList.Add('\u03B1');
            characterList.Add('\u03B2');
            characterList.Add('\u03C0');
            characterList.Add('\u2208');
            characterList.Add('\u2209');
            characterList.Add('\u220F');
            characterList.Add('\u2211');
            characterList.Add('\u221A');
            characterList.Add('\u221E');
            characterList.Add('\u2220');
            characterList.Add('\u2221');
            characterList.Add('\u27C2');
            characterList.Add('\u221F');

            for (char c = '\u2032'; c <= '\u2034'; c++)
            {
                characterList.Add(c);
            }

            characterList.Add('\u2601');
            characterList.Add('\u2607');
            characterList.Add('\u2603');
            characterList.Add('\u263C');
            characterList.Add('\u2600');
            characterList.Add('\u263D');
            characterList.Add('\u263E');
            characterList.Add('\u2039');
            characterList.Add('\u203A');
            characterList.Add('\u00AB');
            characterList.Add('\u00BB');
            characterList.Add('\u226A');
            characterList.Add('\u226B');
            characterList.Add('\u203C');
            characterList.Add('\u2713');
            characterList.Add('\u2717');
            characterList.Add('\u00B6');
            characterList.Add('\u2020');
            characterList.Add('\u2021');
            characterList.Add('\u2423');

            api.Font.FontSetup(systemFont, '\0', '\0', characterList, new Vector2i(0, 0), null, mGlyphSize, 9999, 1, 2, false, true, false);
            InitializeSystemFont(api, systemFont);
        }

        /// <summary>
        /// Get all font glyph rects
        /// </summary>
        /// <param name='font'>Font index</param>
        /// <param name='rectStride'>Stride</param>
        /// <returns>Glyph rects</returns>
        public GlyphRect[] FontGlyphs(FontAsset font, out int rectStride)
        {
            rectStride = 0;

            if (font != null && font.internalState.isSystemFont)
            {
                rectStride = SYSTEM_FONT_RECT_STRIDE;
                return mSystemFontRects;
            }

            return null;
        }

        private void InitializeSystemFont(RBAPI api, FontAsset font)
        {
            int i = 0;
            mSystemFontRects = new GlyphRect[SYSTEM_FONT_RECT_STRIDE * 280];

            // Initialize with dummy values
            for (i = 0; i < mSystemFontRects.Length; i++)
            {
                mSystemFontRects[i] = new GlyphRect(255, 255);
            }

            // 0000:  U+0020
            i = 0;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)32, 0, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 1, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);

            // 0001:  !
            i = 13;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)33, 13, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0002:  "
            i = 26;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)34, 26, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);

            // 0003:  #
            i = 39;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)35, 39, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0004:  $
            i = 52;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)36, 52, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 3, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 4);

            // 0005:  %
            i = 65;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)37, 65, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0006:  &
            i = 78;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)38, 78, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 4, 5);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0007:  '
            i = 91;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)39, 91, new Vector2i(1, 6));
            mSystemFontRects[i++] = new GlyphRect(1, 6, 0, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);

            // 0008:  (
            i = 104;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)40, 104, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0009:  )
            i = 117;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)41, 117, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5);

            // 0010:  *
            i = 130;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)42, 130, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 4);

            // 0011:  +
            i = 143;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)43, 143, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 4);

            // 0012:  ,
            i = 156;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)44, 156, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 5);

            // 0013:  -
            i = 169;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)45, 169, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 2, 3);

            // 0014:  .
            i = 182;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)46, 182, new Vector2i(1, 6));
            mSystemFontRects[i++] = new GlyphRect(1, 6, 0, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 5);

            // 0015:  /
            i = 195;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)47, 195, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);

            // 0016:  0
            i = 208;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)48, 208, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 4, 4);

            // 0017:  1
            i = 221;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)49, 221, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);

            // 0018:  2
            i = 234;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)50, 234, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0019:  3
            i = 247;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)51, 247, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0020:  4
            i = 260;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)52, 260, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 5);

            // 0021:  5
            i = 273;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)53, 273, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);

            // 0022:  6
            i = 286;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)54, 286, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);

            // 0023:  7
            i = 299;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)55, 299, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0024:  8
            i = 312;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)56, 312, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0025:  9
            i = 325;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)57, 325, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);

            // 0026:  :
            i = 338;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)58, 338, new Vector2i(1, 6));
            mSystemFontRects[i++] = new GlyphRect(1, 6, 0, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0027:  ;
            i = 351;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)59, 351, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 5);

            // 0028:  <
            i = 364;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)60, 364, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0029:  =
            i = 377;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)61, 377, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 2, 4);

            // 0030:  >
            i = 390;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)62, 390, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 5);

            // 0031:  ?
            i = 403;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)63, 403, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0032:  @
            i = 416;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)64, 416, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 3);

            // 0033:  A
            i = 429;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)65, 429, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 5);

            // 0034:  B
            i = 442;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)66, 442, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0035:  C
            i = 455;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)67, 455, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);

            // 0036:  D
            i = 468;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)68, 468, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 4);

            // 0037:  E
            i = 481;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)69, 481, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0038:  F
            i = 494;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)70, 494, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0039:  G
            i = 507;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)71, 507, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);

            // 0040:  H
            i = 520;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)72, 520, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 5);

            // 0041:  I
            i = 533;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)73, 533, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 4);

            // 0042:  J
            i = 546;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)74, 546, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 4);

            // 0043:  K
            i = 559;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)75, 559, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0044:  L
            i = 572;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)76, 572, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 1);

            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);

            // 0045:  M
            i = 585;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)77, 585, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0046:  N
            i = 598;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)78, 598, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);

            // 0047:  O
            i = 611;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)79, 611, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 4);

            // 0048:  P
            i = 624;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)80, 624, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 2);

            // 0049:  Q
            i = 637;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)81, 637, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 3);

            // 0050:  R
            i = 650;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)82, 650, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 3, 5);

            // 0051:  S
            i = 663;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)83, 663, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);

            // 0052:  T
            i = 676;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)84, 676, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 5);

            // 0053:  U
            i = 689;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)85, 689, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 4);

            // 0054:  V
            i = 702;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)86, 702, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0055:  W
            i = 715;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)87, 715, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 4, 3);

            // 0056:  X
            i = 728;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)88, 728, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 5);

            // 0057:  Y
            i = 741;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)89, 741, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);

            // 0058:  Z
            i = 754;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)90, 754, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0059:  [
            i = 767;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)91, 767, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 1, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0060:  \
            i = 780;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)92, 780, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 2, 5);

            // 0061:  ]
            i = 793;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)93, 793, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 1, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 4);

            // 0062:  ^
            i = 806;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)94, 806, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 1);

            // 0063:  _
            i = 819;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)95, 819, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);

            // 0064:  `
            i = 832;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)96, 832, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);

            // 0065:  a
            i = 845;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)97, 845, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0066:  b
            i = 858;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)98, 858, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 4);

            // 0067:  c
            i = 871;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)99, 871, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 4);

            // 0068:  d
            i = 884;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)100, 884, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 5);

            // 0069:  e
            i = 897;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)101, 897, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2);

            // 0070:  f
            i = 910;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)102, 910, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3);

            // 0071:  g
            i = 923;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)103, 923, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2);

            // 0072:  h
            i = 936;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)104, 936, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 5);

            // 0073:  i
            i = 949;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)105, 949, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0);

            // 0074:  j
            i = 962;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)106, 962, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0);

            // 0075:  k
            i = 975;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)107, 975, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 3);

            // 0076:  l
            i = 988;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)108, 988, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0077:  m
            i = 1001;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)109, 1001, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0078:  n
            i = 1014;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)110, 1014, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 5);

            // 0079:  o
            i = 1027;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)111, 1027, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 4);

            // 0080:  p
            i = 1040;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)112, 1040, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 3);

            // 0081:  q
            i = 1053;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)113, 1053, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 5);

            // 0082:  r
            i = 1066;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)114, 1066, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0083:  s
            i = 1079;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)115, 1079, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0084:  t
            i = 1092;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)116, 1092, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);

            // 0085:  u
            i = 1105;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)117, 1105, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 4);

            // 0086:  v
            i = 1118;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)118, 1118, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0087:  w
            i = 1131;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)119, 1131, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 5);

            // 0088:  x
            i = 1144;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)120, 1144, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 3, 5);

            // 0089:  y
            i = 1157;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)121, 1157, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 4);

            // 0090:  z
            i = 1170;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)122, 1170, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2);

            // 0091:  {
            i = 1183;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)123, 1183, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0092:  |
            i = 1196;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)124, 1196, new Vector2i(1, 6));
            mSystemFontRects[i++] = new GlyphRect(1, 6, 0, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);

            // 0093:  }
            i = 1209;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)125, 1209, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 1, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4);

            // 0094:  ~
            i = 1222;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)126, 1222, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 1);

            // 0095:  U+3001
            i = 1235;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12289, 1235, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0096:  U+3002
            i = 1248;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12290, 1248, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0097:  U+30A0
            i = 1261;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12448, 1261, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 3, 4);

            // 0098:  U+30A1
            i = 1274;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12449, 1274, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0099:  U+30A2
            i = 1287;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12450, 1287, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 3);

            // 0100:  U+30A3
            i = 1300;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12451, 1300, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2);

            // 0101:  U+30A4
            i = 1313;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12452, 1313, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 3);

            // 0102:  U+30A5
            i = 1326;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12453, 1326, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0103:  U+30A6
            i = 1339;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12454, 1339, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 0);

            // 0104:  U+30A7
            i = 1352;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12455, 1352, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);

            // 0105:  U+30A8
            i = 1365;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12456, 1365, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 4);

            // 0106:  U+30A9
            i = 1378;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12457, 1378, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4);

            // 0107:  U+30AA
            i = 1391;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12458, 1391, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0108:  U+30AB
            i = 1404;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12459, 1404, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);

            // 0109:  U+30AC
            i = 1417;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12460, 1417, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 7);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0110:  U+30AD
            i = 1430;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12461, 1430, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 2);

            // 0111:  U+30AE
            i = 1443;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12462, 1443, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 2);

            // 0112:  U+30AF
            i = 1456;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12463, 1456, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0113:  U+30B0
            i = 1469;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12464, 1469, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0114:  U+30B1
            i = 1482;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12465, 1482, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 0);

            // 0115:  U+30B2
            i = 1495;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12466, 1495, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 1, 7);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0);

            // 0116:  U+30B3
            i = 1508;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12467, 1508, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);

            // 0117:  U+30B4
            i = 1521;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12468, 1521, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0118:  U+30B5
            i = 1534;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12469, 1534, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0);

            // 0119:  U+30B6
            i = 1547;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12470, 1547, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 1, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 6, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 0);

            // 0120:  U+30B7
            i = 1560;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12471, 1560, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0121:  U+30B8
            i = 1573;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12472, 1573, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 3, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0122:  U+30B9
            i = 1586;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12473, 1586, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 4, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0123:  U+30BA
            i = 1599;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12474, 1599, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 4, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 3);

            // 0124:  U+30BB
            i = 1612;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12475, 1612, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 1, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 2);

            // 0125:  U+30BC
            i = 1625;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12476, 1625, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 3, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 1, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 2);

            // 0126:  U+30BD
            i = 1638;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12477, 1638, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0127:  U+30BE
            i = 1651;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12478, 1651, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 1, 6);

            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0128:  U+30BF
            i = 1664;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12479, 1664, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 6);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 0);

            // 0129:  U+30C0
            i = 1677;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12480, 1677, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 8);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0130:  U+30C1
            i = 1690;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12481, 1690, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0131:  U+30C2
            i = 1703;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12482, 1703, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0132:  U+30C3
            i = 1716;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12483, 1716, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);

            // 0133:  U+30C4
            i = 1729;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12484, 1729, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0134:  U+30C5
            i = 1742;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12485, 1742, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 1, 6);

            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0135:  U+30C6
            i = 1755;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12486, 1755, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 5, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0136:  U+30C7
            i = 1768;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12487, 1768, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 5, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0137:  U+30C8
            i = 1781;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12488, 1781, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3);

            // 0138:  U+30C9
            i = 1794;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12489, 1794, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 3);

            // 0139:  U+30CA
            i = 1807;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12490, 1807, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 5, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0140:  U+30CB
            i = 1820;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12491, 1820, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);

            // 0141:  U+30CC
            i = 1833;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12492, 1833, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 1);

            // 0142:  U+30CD
            i = 1846;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12493, 1846, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0143:  U+30CE
            i = 1859;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12494, 1859, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 5);

            // 0144:  U+30CF
            i = 1872;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12495, 1872, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 3, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0145:  U+30D0
            i = 1885;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12496, 1885, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 0, 6);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(5, 3, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0146:  U+30D1
            i = 1898;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12497, 1898, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 6);

            mSystemFontRects[i++] = new GlyphRect(3, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 3, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0147:  U+30D2
            i = 1911;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12498, 1911, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 4);

            // 0148:  U+30D3
            i = 1924;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12499, 1924, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 0);

            // 0149:  U+30D4
            i = 1937;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12500, 1937, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0150:  U+30D5
            i = 1950;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12501, 1950, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0151:  U+30D6
            i = 1963;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12502, 1963, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0152:  U+30D7
            i = 1976;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12503, 1976, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0153:  U+30D8
            i = 1989;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12504, 1989, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 0, 7);

            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(6, 5);

            // 0154:  U+30D9
            i = 2002;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12505, 2002, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 0, 9);

            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(6, 5);

            // 0155:  U+30DA
            i = 2015;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12506, 2015, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 0, 11);

            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(6, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(6, 5);

            // 0156:  U+30DB
            i = 2028;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12507, 2028, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 3, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0157:  U+30DC
            i = 2041;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12508, 2041, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(5, 0, 6, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 3, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0158:  U+30DD
            i = 2054;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12509, 2054, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 1, 8);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 1);
            mSystemFontRects[i++] = new GlyphRect(5, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0159:  U+30DE
            i = 2067;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12510, 2067, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0160:  U+30DF
            i = 2080;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12511, 2080, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 4, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0161:  U+30E0
            i = 2093;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12512, 2093, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 5);

            // 0162:  U+30E1
            i = 2106;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12513, 2106, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 7);

            mSystemFontRects[i++] = new GlyphRect(4, 0, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 5);

            // 0163:  U+30E2
            i = 2119;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12514, 2119, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 5, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 1);

            // 0164:  U+30E3
            i = 2132;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12515, 2132, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0165:  U+30E4
            i = 2145;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12516, 2145, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 2);

            // 0166:  U+30E5
            i = 2158;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12517, 2158, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0167:  U+30E6
            i = 2171;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12518, 2171, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);

            // 0168:  U+30E7
            i = 2184;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12519, 2184, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 3, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 4);

            // 0169:  U+30E8
            i = 2197;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12520, 2197, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 4);

            // 0170:  U+30E9
            i = 2210;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12521, 2210, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 5, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 3);

            // 0171:  U+30EA
            i = 2223;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12522, 2223, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 4);

            // 0172:  U+30EB
            i = 2236;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12523, 2236, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 5);

            // 0173:  U+30EC
            i = 2249;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12524, 2249, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 3);

            // 0174:  U+30ED
            i = 2262;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12525, 2262, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 5);

            // 0175:  U+30EE
            i = 2275;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12526, 2275, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 3);

            // 0176:  U+30EF
            i = 2288;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12527, 2288, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);

            // 0177:  U+30F0
            i = 2301;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12528, 2301, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 5);

            // 0178:  U+30F1
            i = 2314;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12529, 2314, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0179:  U+30F2
            i = 2327;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12530, 2327, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 3, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0180:  U+30F3
            i = 2340;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12531, 2340, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0181:  U+30F4
            i = 2353;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12532, 2353, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 2, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 2, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(6, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0);

            // 0182:  U+30F5
            i = 2366;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12533, 2366, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0183:  U+30F6
            i = 2379;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12534, 2379, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0184:  U+30F7
            i = 2392;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12535, 2392, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 0);

            // 0185:  U+30F8
            i = 2405;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12536, 2405, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 5);

            // 0186:  U+30F9
            i = 2418;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12537, 2418, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0187:  U+30FA
            i = 2431;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12538, 2431, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0188:  U+30FB
            i = 2444;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12539, 2444, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 2, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 1, 3);

            // 0189:  U+30FC
            i = 2457;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12540, 2457, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 5, 2);

            // 0190:  U+30FD
            i = 2470;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12541, 2470, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0191:  U+30FE
            i = 2483;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12542, 2483, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0192:  U+30FF
            i = 2496;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)12543, 2496, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 4, 5);

            // 0193:  U+00A2
            i = 2509;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)162, 2509, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0194:  U+00A3
            i = 2522;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)163, 2522, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 3, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4);

            // 0195:  U+00A4
            i = 2535;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)164, 2535, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 6);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 5);

            // 0196:  U+00A5
            i = 2548;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)165, 2548, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 7);

            mSystemFontRects[i++] = new GlyphRect(1, 3, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0197:  U+20A0
            i = 2561;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8352, 2561, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 5, 1);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 5, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);

            // 0198:  U+20A3
            i = 2574;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8355, 2574, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2);

            // 0199:  U+20A8
            i = 2587;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8360, 2587, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 1, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 5, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 3, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 3);

            // 0200:  U+20AC
            i = 2600;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8364, 2600, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 4, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 1);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4);

            // 0201:  U+20BF
            i = 2613;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8383, 2613, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);

            // 0202:  U+2022
            i = 2626;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8226, 2626, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 4, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 2, 4);

            // 0203:  U+2023
            i = 2639;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8227, 2639, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 3);

            // 0204:  U+25E6
            i = 2652;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9702, 2652, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 3);

            // 0205:  U+2043
            i = 2665;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8259, 2665, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 2, 3);

            // 0206:  U+2219
            i = 2678;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8729, 2678, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 2, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 1, 3);

            // 0207:  U+00A9
            i = 2691;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)169, 2691, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 4, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 3);

            // 0208:  U+2122
            i = 2704;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8482, 2704, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 2, 5, 4);

            // 0209:  U+2639
            i = 2717;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9785, 2717, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 5, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 3);

            // 0210:  U+263A
            i = 2730;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9786, 2730, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 5, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0211:  U+2190
            i = 2743;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8592, 2743, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(3, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2);

            // 0212:  U+2191
            i = 2756;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8593, 2756, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0);

            // 0213:  U+2192
            i = 2769;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8594, 2769, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0214:  U+2193
            i = 2782;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8595, 2782, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4);

            // 0215:  U+2196
            i = 2795;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8598, 2795, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 1, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2);

            // 0216:  U+2197
            i = 2808;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8599, 2808, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(2, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0217:  U+2198
            i = 2821;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8600, 2821, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(2, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 2);

            // 0218:  U+2199
            i = 2834;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8601, 2834, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 2);

            // 0219:  U+21E6
            i = 2847;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8678, 2847, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 4, 4);

            mSystemFontRects[i++] = new GlyphRect(2, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 2);

            // 0220:  U+21E7
            i = 2860;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8679, 2860, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 7);

            mSystemFontRects[i++] = new GlyphRect(2, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);

            // 0221:  U+21E8
            i = 2873;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8680, 2873, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 4, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(5, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 2);

            // 0222:  U+21E9
            i = 2886;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8681, 2886, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 7);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0223:  U+23CE
            i = 2899;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9166, 2899, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 3, 7);

            mSystemFontRects[i++] = new GlyphRect(3, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 6, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 6, 0);
            mSystemFontRects[i++] = new GlyphRect(6, 1, 6, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 1);

            // 0224:  U+2660
            i = 2912;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9824, 2912, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 0, 7);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 2, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(6, 3, 6, 4);

            // 0225:  U+2661
            i = 2925;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9825, 2925, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 2, 8);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 2);
            mSystemFontRects[i++] = new GlyphRect(6, 1, 6, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(5, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 5);

            // 0226:  U+2662
            i = 2938;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9826, 2938, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 8);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0227:  U+2663
            i = 2951;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9827, 2951, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 5, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 6, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 6, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 5);

            // 0228:  U+2664
            i = 2964;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9828, 2964, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 0, 12);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(6, 3, 6, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 5);

            // 0229:  U+2665
            i = 2977;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9829, 2977, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 6, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 6, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 6, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 5);

            // 0230:  U+2666
            i = 2990;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9830, 2990, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);

            // 0231:  U+2667
            i = 3003;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9831, 3003, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 4, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 2, 6, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(6, 3);

            // 0232:  U+2669
            i = 3016;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9833, 3016, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 2);

            // 0233:  U+266A
            i = 3029;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9834, 3029, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 3, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 2, 5, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 1);

            // 0234:  U+266B
            i = 3042;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9835, 3042, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 1, 6);

            mSystemFontRects[i++] = new GlyphRect(2, 0, 6, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 3, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(6, 1, 6, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0235:  U+23F5
            i = 3055;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9205, 3055, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2);

            // 0236:  U+23F8
            i = 3068;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9208, 3068, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 4, 4);

            // 0237:  U+23F9
            i = 3081;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9209, 3081, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 5, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 4, 4);

            // 0238:  U+23FA
            i = 3094;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9210, 3094, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 5, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 3, 4);

            // 0239:  U+00B0
            i = 3107;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)176, 3107, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0240:  U+00F7
            i = 3120;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)247, 3120, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0241:  U+00B1
            i = 3133;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)177, 3133, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2);

            // 0242:  U+0394
            i = 3146;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)916, 3146, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0243:  U+03A9
            i = 3159;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)937, 3159, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 3, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0244:  U+03B1
            i = 3172;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)945, 3172, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 5);

            // 0245:  U+03B2
            i = 3185;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)946, 3185, new Vector2i(4, 6));
            mSystemFontRects[i++] = new GlyphRect(4, 6, 3, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 0, 2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 3);

            // 0246:  U+03C0
            i = 3198;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)960, 3198, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 5);

            // 0247:  U+2208
            i = 3211;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8712, 3211, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 3, 1);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 4);

            // 0248:  U+2209
            i = 3224;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8713, 3224, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 3, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0249:  U+220F
            i = 3237;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8719, 3237, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 1, 0, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 4, 5);

            // 0250:  U+2211
            i = 3250;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8721, 3250, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 2, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4);

            // 0251:  U+221A
            i = 3263;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8730, 3263, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 4);

            mSystemFontRects[i++] = new GlyphRect(3, 0, 5, 0);
            mSystemFontRects[i++] = new GlyphRect(2, 3, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 1, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0252:  U+221E
            i = 3276;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8734, 3276, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 4, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4, 5, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(6, 2, 6, 3);

            // 0253:  U+2220
            i = 3289;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8736, 3289, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 1, 5);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);

            // 0254:  U+2221
            i = 3302;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8737, 3302, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 5, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 3, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);

            // 0255:  U+27C2
            i = 3315;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)10178, 3315, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 4);

            // 0256:  U+221F
            i = 3328;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8735, 3328, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 1);

            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 5);

            // 0257:  U+2032
            i = 3341;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8242, 3341, new Vector2i(1, 6));
            mSystemFontRects[i++] = new GlyphRect(1, 6, 0, 1);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);

            // 0258:  U+2033
            i = 3354;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8243, 3354, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);

            // 0259:  U+2034
            i = 3367;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8244, 3367, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 0, 4, 1);

            // 0260:  U+2601
            i = 3380;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9729, 3380, new Vector2i(8, 6));
            mSystemFontRects[i++] = new GlyphRect(8, 6, 6, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 3, 7, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 7, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 2, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 6, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 7, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 6, 1);

            // 0261:  U+2607
            i = 3393;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9735, 3393, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 6);

            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0262:  U+2603
            i = 3406;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9731, 3406, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 4, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 2, 6, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 1, 5, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 2, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0263:  U+263C
            i = 3419;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9788, 3419, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 2, 6);

            mSystemFontRects[i++] = new GlyphRect(2, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 2, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 5);

            // 0264:  U+2600
            i = 3432;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9728, 3432, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 4, 4);

            mSystemFontRects[i++] = new GlyphRect(1, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 4, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 5);

            // 0265:  U+263D
            i = 3445;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9789, 3445, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 6, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 3, 0);
            mSystemFontRects[i++] = new GlyphRect(0, 5, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 1, 4, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 4, 4, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 2, 4, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 3, 4, 3);

            // 0266:  U+263E
            i = 3458;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9790, 3458, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 6, 0);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 1, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 4, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5, 4, 5);

            // 0267:  U+2039
            i = 3471;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8249, 3471, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);

            // 0268:  U+203A
            i = 3484;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8250, 3484, new Vector2i(2, 6));
            mSystemFontRects[i++] = new GlyphRect(2, 6, 0, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);

            // 0269:  U+00AB
            i = 3497;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)171, 3497, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 6);

            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(0, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);

            // 0270:  U+00BB
            i = 3510;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)187, 3510, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 6);

            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0271:  U+226A
            i = 3523;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8810, 3523, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 0, 10);

            mSystemFontRects[i++] = new GlyphRect(2, 0);
            mSystemFontRects[i++] = new GlyphRect(5, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(5, 4);

            // 0272:  U+226B
            i = 3536;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8811, 3536, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 0, 10);

            mSystemFontRects[i++] = new GlyphRect(0, 0);
            mSystemFontRects[i++] = new GlyphRect(3, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(2, 2);
            mSystemFontRects[i++] = new GlyphRect(5, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 3);
            mSystemFontRects[i++] = new GlyphRect(4, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 4);

            // 0273:  U+203C
            i = 3549;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8252, 3549, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 0, 4);

            mSystemFontRects[i++] = new GlyphRect(0, 0, 0, 3);
            mSystemFontRects[i++] = new GlyphRect(2, 0, 2, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(2, 5);

            // 0274:  U+2713
            i = 3562;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)10003, 3562, new Vector2i(6, 6));
            mSystemFontRects[i++] = new GlyphRect(6, 6, 0, 6);

            mSystemFontRects[i++] = new GlyphRect(5, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 3);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(2, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0275:  U+2717
            i = 3575;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)10007, 3575, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 0, 9);

            mSystemFontRects[i++] = new GlyphRect(0, 1);
            mSystemFontRects[i++] = new GlyphRect(4, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2);
            mSystemFontRects[i++] = new GlyphRect(3, 2);
            mSystemFontRects[i++] = new GlyphRect(2, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 4);
            mSystemFontRects[i++] = new GlyphRect(3, 4);
            mSystemFontRects[i++] = new GlyphRect(0, 5);
            mSystemFontRects[i++] = new GlyphRect(4, 5);

            // 0276:  U+00B6
            i = 3588;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)182, 3588, new Vector2i(7, 6));
            mSystemFontRects[i++] = new GlyphRect(7, 6, 4, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 3, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 2, 3, 2);
            mSystemFontRects[i++] = new GlyphRect(1, 0, 6, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 3, 3, 3);
            mSystemFontRects[i++] = new GlyphRect(3, 4, 3, 5);
            mSystemFontRects[i++] = new GlyphRect(5, 1, 5, 5);

            // 0277:  U+2020
            i = 3601;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8224, 3601, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 5);
            mSystemFontRects[i++] = new GlyphRect(1, 0);

            // 0278:  U+2021
            i = 3614;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)8225, 3614, new Vector2i(3, 6));
            mSystemFontRects[i++] = new GlyphRect(3, 6, 2, 3);

            mSystemFontRects[i++] = new GlyphRect(0, 1, 2, 1);
            mSystemFontRects[i++] = new GlyphRect(0, 4, 2, 4);
            mSystemFontRects[i++] = new GlyphRect(1, 2, 1, 3);
            mSystemFontRects[i++] = new GlyphRect(1, 0);
            mSystemFontRects[i++] = new GlyphRect(1, 5);

            // 0279:  U+2423
            i = 3627;
            api.Font.SetSystemFontGlyphRectIndex(ref font.internalState, (char)9251, 3627, new Vector2i(5, 6));
            mSystemFontRects[i++] = new GlyphRect(5, 6, 1, 2);

            mSystemFontRects[i++] = new GlyphRect(0, 5, 4, 5);
            mSystemFontRects[i++] = new GlyphRect(0, 4);
            mSystemFontRects[i++] = new GlyphRect(4, 4);
        }

        /// <summary>
        /// Glyph rectangle
        /// </summary>
        public struct GlyphRect
        {
            /// <summary>
            /// X1 coordinate
            /// </summary>
            public byte x1;

            /// <summary>
            /// X1 coordinate
            /// </summary>
            public byte y1;

            /// <summary>
            /// X1 coordinate
            /// </summary>
            public byte x2;

            /// <summary>
            /// X1 coordinate
            /// </summary>
            public byte y2;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name='x1'>X1 coordinate</param>
            /// <param name='y1'>Y1 coordinate</param>
            /// <param name='x2'>X2 coordinate</param>
            /// <param name='y2'>Y2 coordinate</param>
            public GlyphRect(byte x1, byte y1, byte x2, byte y2)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name='x'>X coordinate</param>
            /// <param name='y'>Y coordinate</param>
            public GlyphRect(byte x, byte y)
            {
                this.x1 = x;
                this.y1 = y;
                this.x2 = x;
                this.y2 = y;
            }
        }
    }
}
