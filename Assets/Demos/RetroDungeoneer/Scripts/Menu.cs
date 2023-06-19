namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// A generic game menu with configurable list of options
    /// </summary>
    public class Menu
    {
        private const int MINIMUM_WIDTH = 96;

        private FastString mHeader = new FastString(128);
        private FastString mSummary = new FastString(1024);

        private Option[] mOptions = new Option[26];
        private int mOptionCount = 0;

        private int mWidestOption = MINIMUM_WIDTH;
        private int mPointerIndex = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Header text</param>
        public Menu(FastString header)
        {
            mHeader.Set(header);
            for (int i = 0; i < mOptions.Length; i++)
            {
                mOptions[i] = new Option();
            }

            ClearOptions();
        }

        /// <summary>
        /// Delegate to call when an option is chosen
        /// </summary>
        public delegate void OptionDelegate();

        /// <summary>
        /// Count of options in the menu
        /// </summary>
        public int optionCount
        {
            get
            {
                return mOptionCount;
            }
        }

        /// <summary>
        /// Current menu option that the pointer is hovering over
        /// </summary>
        public int pointerIndex
        {
            get
            {
                return mPointerIndex;
            }
        }

        /// <summary>
        /// Clear all options
        /// </summary>
        public void ClearOptions()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            mOptionCount = 0;

            var headerSize = RB.PrintMeasure(game.assets.fontSmall, mHeader);
            headerSize.width += 32;

            mWidestOption = headerSize.width > MINIMUM_WIDTH ? headerSize.width : MINIMUM_WIDTH;
        }

        /// <summary>
        /// Add a new option
        /// </summary>
        /// <param name="optionText">Option text</param>
        public void AddOption(FastString optionText)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (mOptionCount >= mOptions.Length)
            {
                throw new System.Exception("Menu can only support " + (mOptions.Length + 1) + " options");
            }

            mOptions[mOptionCount].text.Set(optionText);
            mOptionCount++;

            var optionSize = RB.PrintMeasure(game.assets.fontSmall, optionText);
            if (optionSize.width > mWidestOption)
            {
                mWidestOption = optionSize.width;
            }
        }

        /// <summary>
        /// Set the summary text
        /// </summary>
        /// <param name="summary">Summary text</param>
        public void SetSummary(FastString summary)
        {
            mSummary.Set(summary);
        }

        /// <summary>
        /// Set the header text
        /// </summary>
        /// <param name="header">Header text</param>
        public void SetHeader(FastString header)
        {
            mHeader.Set(header);
        }

        /// <summary>
        /// Render the menu
        /// </summary>
        public void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            int yOffset = 0;
            int optionSpacing = 10;
            int borderSpacing = 8;
            Vector2i summarySize = Vector2i.zero;

            Rect2i optionsRect = new Rect2i(RB.DisplaySize.width / 2, RB.DisplaySize.height / 2, mWidestOption + 48, mOptionCount * (optionSpacing + 1));
            if (mSummary.Length > 0)
            {
                summarySize = RB.PrintMeasure(game.assets.fontSmall, new Rect2i(0, 0, optionsRect.width - (borderSpacing * 2), 9999), RB.ALIGN_H_CENTER | RB.TEXT_OVERFLOW_WRAP, mSummary);
                optionsRect.height += summarySize.height;
                optionsRect.height += borderSpacing * 2;
            }

            optionsRect.height += borderSpacing * 2;

            optionsRect.x -= optionsRect.width / 2;
            optionsRect.y -= optionsRect.height / 2;

            RB.DrawRectFill(optionsRect, C.COLOR_MENU_BACKGROUND);
            RB.DrawRect(optionsRect, Color.white);

            // Draw header
            var headerSize = RB.PrintMeasure(game.assets.fontSmall, mHeader);
            var headerRect = new Rect2i(optionsRect.x + (optionsRect.width / 2) - (headerSize.width / 2) - 16, optionsRect.y - 6, headerSize.width + 32, 12);

            RB.DrawRectFill(headerRect, C.COLOR_MENU_HEADER_BACKGROUND);
            RB.DrawRect(headerRect, Color.white);
            headerRect.y++;
            RB.Print(game.assets.fontSmall, headerRect, Color.white, RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mHeader);

            yOffset += borderSpacing;

            if (mSummary.Length > 0)
            {
                yOffset += borderSpacing;

                var summaryRect = new Rect2i(optionsRect.x + borderSpacing, optionsRect.y + yOffset, optionsRect.width - (borderSpacing * 2), 9999);

                RB.Print(game.assets.fontSmall, summaryRect, Color.gray, RB.ALIGN_H_CENTER | RB.TEXT_OVERFLOW_WRAP, mSummary);

                yOffset += summarySize.height + borderSpacing;
            }

            char shortCut = 'a';

            int oldPointerIndex = mPointerIndex;
            mPointerIndex = -1;

            for (int i = 0; i < mOptionCount; i++)
            {
                var optionColor = Color.white;
                var optionRect = new Rect2i(optionsRect.x + borderSpacing, optionsRect.y + yOffset, optionsRect.width - (borderSpacing * 2), optionSpacing);
                if (optionRect.Contains(RB.PointerPos()))
                {
                    optionColor = Color.yellow;
                    mPointerIndex = i;
                }

                RB.Print(
                    game.assets.fontSmall,
                    optionRect,
                    optionColor,
                    RB.ALIGN_V_CENTER,
                    C.FSTR.Set("@AAAAAA").Append(shortCut).Append(".@- ").Append(mOptions[i].text));

                yOffset += optionSpacing + 1;
                shortCut++;
            }

            if (oldPointerIndex != mPointerIndex && mPointerIndex >= 0)
            {
                RB.SoundPlay(game.assets.soundPointerSelect, 0.35f, RandomUtils.RandomPitch(0.1f));
            }
        }

        private class Option
        {
            public FastString text = new FastString(128);
        }
    }
}
