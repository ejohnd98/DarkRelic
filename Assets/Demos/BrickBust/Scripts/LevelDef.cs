namespace RetroBlitDemoBrickBust
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the definition of all the level layouts
    /// </summary>
    public class LevelDef
    {
        private static List<LevelInfo> mLevels;

        /// <summary>
        /// Initialize all the level definitions. Levels are defined as simple ascii strings that
        /// we will parse later
        /// </summary>
        public static void Initialize()
        {
            mLevels = new List<LevelInfo>();

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                ".........." +
                "3333333333" +
                "2222222222" +
                "1111111111"));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".1.21.2.1." +
                ".1.2..2.1." +
                ".1.2.12.1." +
                ".1.2..2.1." +
                ".1.21.2.1." +
                ".1.2..2.1." +
                ".1.2.12.1." +
                ".1.2..2.1." +
                ".1.21.2.1."));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                "....33...." +
                "...3223..." +
                "..321123.." +
                ".32111123." +
                "..321123.." +
                "...3223..." +
                "....33...."));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                ".XXX..XXX." +
                ".X11..11X." +
                ".X22..22X." +
                ".X33..33X." +
                ".X33..33X." +
                ".X22..22X." +
                ".X11..11X." +
                ".XXX..XXX."));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                "1111111111" +
                "1222222221" +
                "1233333321" +
                "1234444321" +
                "1234554321" +
                "1234444321" +
                "1233333321" +
                "1222222221" +
                "1111111111"));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                ".........." +
                "....66...." +
                "....66...." +
                ".........." +
                ".........." +
                ".........." +
                ".........." +
                ".........." +
                ".........." +
                ".........." +
                "XXXXXXXXX." +
                ".........." +
                ".XXXXXXXXX"));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                ".XXXXXXXX." +
                ".555.555.." +
                "..5...5..." +
                ".........." +
                "...5...5.." +
                "..555.555." +
                ".XXXXXXXX."));

            mLevels.Add(new LevelInfo(
                ".........." +
                "6666666666" +
                "5555555555" +
                "4444444444" +
                "3333333333" +
                "2222222222" +
                "1111111111"));

            mLevels.Add(new LevelInfo(
                "....242424" +
                "....424242" +
                "....242424" +
                "....424242" +
                "....242424" +
                "....424242" +
                ".........." +
                ".........." +
                "131313...." +
                "313131...." +
                "131313...." +
                "313131...." +
                "131313...." +
                "313131...."));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                "..555555.." +
                "...5555..." +
                "....55...." +
                "...5555..." +
                "..555555.." +
                ".........." +
                "X........X" +
                ".X......X." +
                "..X....X.." +
                "...X..X..."));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".66666666." +
                ".61111116." +
                ".61111116." +
                ".61111116." +
                ".61111116." +
                ".61111116." +
                ".61111116." +
                ".61111116." +
                ".61111116." +
                ".66666666."));

            mLevels.Add(new LevelInfo(
                "..14....13" +
                "..32.14.42" +
                "..XX.32.XX" +
                ".....XX..." +
                ".........." +
                ".......42." +
                ".43....13." +
                ".12.12.XX." +
                ".XX.34...." +
                "....XX...." +
                "32........" +
                "14...34..." +
                "XX...12..." +
                ".....XX..."));

            mLevels.Add(new LevelInfo(
                ".........." +
                "...6666..." +
                "..666666.." +
                ".66566566." +
                ".66666666." +
                ".65666656." +
                ".66566566." +
                "..665566.." +
                "...6666..."));

            mLevels.Add(new LevelInfo(
                ".........." +
                ".........." +
                "...X..X..." +
                "...X..X..." +
                "...X..X..." +
                "666X..X666" +
                "454X..X545" +
                "343X..X434" +
                "232X..X323" +
                "121X..X212" +
                "XXXX..XXXX"));

            mLevels.Add(new LevelInfo(
                "1122334455" +
                "1122334455" +
                "2233445566" +
                "2233445566" +
                "3344556655" +
                "3344556655" +
                "4455665544" +
                "4455665544" +
                "5566554433" +
                "5566554433" +
                "6655443322" +
                "6655443322" +
                "5544332211" +
                "5544332211" +
                "4433221122" +
                "4433221122" +
                "3322112233" +
                "3322112233" +
                "2211223344" +
                "2211223344"));

            mLevels.Add(new LevelInfo(
                "1234554321" +
                ".........." +
                ".XXXXXXXXX" +
                ".........." +
                "XXXXXX.XXX" +
                ".........." +
                "XXX.XXXXXX" +
                ".........." +
                ".XXXXXXXXX"));

            mLevels.Add(new LevelInfo(
                "XXXXXXXXXX" +
                "X5X5XX5X5X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X" +
                "X.X.XX.X.X"));
        }

        /// <summary>
        /// Get level info for level of given index
        /// </summary>
        /// <param name="index">Level index</param>
        /// <returns>Level info</returns>
        public static LevelInfo GetLevel(int index)
        {
            if (mLevels == null || index < 0)
            {
                return null;
            }

            // Level wrap
            index = index % mLevels.Count;

            return mLevels[index];
        }

        /// <summary>
        /// Definition of a single level
        /// </summary>
        public class LevelInfo
        {
            private string mLayout = null;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="layout">Level layout as an ascii string</param>
            public LevelInfo(string layout)
            {
                mLayout = layout;
            }

            /// <summary>
            /// Get the level layout
            /// </summary>
            public string Layout
            {
                get { return mLayout; }
            }

            /// <summary>
            /// Get the brick at the given location
            /// </summary>
            /// <param name="x">X coord</param>
            /// <param name="y">Y coord</param>
            /// <returns>Brick found at location</returns>
            public Brick.BrickType GetBrickAt(int x, int y)
            {
                int stride = RB.DisplaySize.width / Global.BRICK_WIDTH;
                int i = x + (y * stride);

                if (mLayout != null && i < mLayout.Length)
                {
                    var c = mLayout[i];
                    switch (c)
                    {
                        case '1':
                            return Brick.BrickType.Green;
                        case '2':
                            return Brick.BrickType.Blue;
                        case '3':
                            return Brick.BrickType.Brown;
                        case '4':
                            return Brick.BrickType.Pink;
                        case '5':
                            return Brick.BrickType.Gold;
                        case '6':
                            return Brick.BrickType.Black;
                        case 'X':
                            return Brick.BrickType.Block;
                        default:
                            return Brick.BrickType.None;
                    }
                }

                return Brick.BrickType.None;
            }
        }
    }
}