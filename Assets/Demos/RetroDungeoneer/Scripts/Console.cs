namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Game console, displays game messages
    /// </summary>
    public class Console
    {
        /// <summary>
        /// Total lines stored in the log. Old lines are thrown out when capacity is reached
        /// </summary>
        public const int LOG_LINES = 128;

        /// <summary>
        /// Size of the console in pixels
        /// </summary>
        public readonly Vector2i size;

        private const int LOG_LINE_WIDTH = 256;

        private LinkedList<FastString> mLogLines;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Dimensions of the console in pixels</param>
        public Console(Vector2i size)
        {
            mLogLines = new LinkedList<FastString>();

            this.size = size;
            for (int i = 0; i < LOG_LINES; i++)
            {
                mLogLines.AddLast(new FastString(LOG_LINE_WIDTH));
            }
        }

        /// <summary>
        /// Clear the console
        /// </summary>
        public void Clear()
        {
            var curLine = mLogLines.First;

            while (curLine != null)
            {
                curLine.Value.Clear();
                curLine = curLine.Next;
            }
        }

        /// <summary>
        /// Add a log line to console
        /// </summary>
        /// <param name="str">Line</param>
        public void Log(FastString str)
        {
            // Rotate a log line from the end of the log to the start
            var line = mLogLines.Last;
            mLogLines.RemoveLast();

            line.Value.Set(str);

            mLogLines.AddFirst(line.Value);
        }

        /// <summary>
        /// Get all log lines
        /// </summary>
        /// <returns>Lines</returns>
        public LinkedList<FastString> GetLogLines()
        {
            return mLogLines;
        }

        /// <summary>
        /// Render the console
        /// </summary>
        public void Render()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var cameraPos = RB.CameraGet();
            RB.CameraReset();

            int yOffset = 0;
            var curLine = mLogLines.First;

            byte alpha = 255;

            // Keep looping and printing log lines until we the console max height
            while (yOffset < size.height && curLine != null && curLine.Value.Length > 0)
            {
                var lineSize = RB.PrintMeasure(game.assets.fontSmall, new Rect2i(0, 0, size.width, 9999), RB.TEXT_OVERFLOW_WRAP, curLine.Value);

                // Draw text
                RB.AlphaSet(alpha);
                RB.Print(game.assets.fontSmall, new Rect2i(4, RB.DisplaySize.height - yOffset - 4 - lineSize.height, size.width, 9999), Color.white, RB.TEXT_OVERFLOW_WRAP, curLine.Value);
                RB.AlphaSet(255);
                alpha -= 15;
                if (alpha < 15)
                {
                    alpha = 15;
                }

                yOffset += lineSize.height + 2;

                curLine = curLine.Next;
            }

            RB.CameraSet(cameraPos);
        }
    }
}
