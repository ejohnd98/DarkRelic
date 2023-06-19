namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate pixel buffers
    /// </summary>
    public class ScenePixelBuffer : SceneDemo
    {
        private Vector2i mFireSize;

        private Color32[] mFirePalette = new Color32[256];

        private byte[] mFire;
        private byte[] mPrevFire;
        private Color32[] mPixelBuffer;

        private Rect2i mFireRect;
        private Vector2i mFirePos;
        private float mRotation;

        private sbyte[] mRandomBuf;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            return true;
        }

        /// <summary>
        /// Handle scene entry
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            InitializeFire();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();
            FireLoop();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            int x = 4;
            int y = 12;

            DrawCode(x, y);

            DrawFire();
        }

        private void InitializeFire()
        {
            mFireSize = new Vector2i(RB.DisplaySize.width * 0.75f, RB.DisplaySize.height * 0.35f);
            mFire = new byte[mFireSize.width * mFireSize.height];
            mPrevFire = new byte[mFireSize.width * mFireSize.height];
            mPixelBuffer = new Color32[mFireSize.width * mFireSize.height];

            int i = 0;

            for (i = 0; i < mPixelBuffer.Length; i++)
            {
                mPixelBuffer[i] = Color.black;
            }

            // Setup palette
            for (i = 0; i < mFirePalette.Length; i++)
            {
                if (i < 12)
                {
                    mFirePalette[i] = new Color32((byte)(i * 21), 0, 0, 255);
                }
                else if (i < 64)
                {
                    mFirePalette[i] = new Color32(255, (byte)(i * 4), 0, 255);
                }
                else
                {
                    mFirePalette[i] = Color.white;
                }
            }

            mRandomBuf = new sbyte[251]; // Using prime number for size to lower chance of repeating pattern
            for (i = 0; i < mRandomBuf.Length; i++)
            {
                mRandomBuf[i] = (sbyte)Random.Range(-1, 5);
            }
        }

        private void FireLoop()
        {
            int width = mFireSize.width;
            int height = mFireSize.height;
            int i = 0;
            int ri = Random.Range(0, mRandomBuf.Length);

            for (i = width + 1; i < (width * (height - 1)) - 1; i++)
            {
                // Average out neighbours
                int sum =
                    mPrevFire[i - width] +
                    mPrevFire[i - 1] +
                    mPrevFire[i + 1] +
                    mPrevFire[i + width] +
                    mPrevFire[i - width - 1] +
                    mPrevFire[i - width + 1] +
                    mPrevFire[i + width - 1] +
                    mPrevFire[i + width + 1];

                int avg = sum / 8;

                // Cool down pixels if lower 2 bits are not set
                if ((sum & 3) == 0 &&
                    (avg > 0 || i >= (height - 18) * width))
                {
                    avg += mRandomBuf[ri];
                    ri++;
                    if (ri >= mRandomBuf.Length)
                    {
                        ri = 0;
                    }
                }

                mFire[i] = (byte)avg;
            }

            // Scroll up
            for (i = 0; i < width * (height - 2); i++)
            {
                mPrevFire[i] = mFire[i + width];
            }

            // Remove dark pixels from bottom row
            for (i = (height - 2) * width; i < width * height; i++)
            {
                if (mFire[i] < 12)
                {
                    mFire[i] = (byte)(20 - mFire[i]);
                }
            }

            // Update pixel buffer
            i = 0;
            var frameColor = DemoUtil.IndexToRGB(2);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x < 2 || y < 2 || x >= width - 2 || y >= height - 2)
                    {
                        mPixelBuffer[i] = frameColor;
                    }
                    else
                    {
                        mPixelBuffer[i] = mFirePalette[mFire[i]];
                    }

                    i++;
                }
            }
        }

        private void DrawCode(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 25));

            mFormatStr.Set("@C// RetroBlit lets you draw pixel buffers!\n");
            mFormatStr.Append("@MColor32@N[] pixelBuffer = @Knew @MColor32@N[@L").Append(mFireSize.width).Append("@N * @L");
            mFormatStr.Append(mFireSize.height).Append("@N];\n");
            mFormatStr.Append("@C/* ... fill pixel buffer with pixel colors ... */\n\n");

            mFormatStr.Append("@C// Draw pixel buffer at 1:1 scale, anywhere on the screen\n");
            mFormatStr.Append("@MRB@N.DrawPixelBuffer(@NpixelBuffer, @Knew @MVector2i@N(@L").Append(mFireSize.width).Append("@N, @L");
            mFormatStr.Append(mFireSize.height).Append("@N), @Knew @MVector2i@N(@L").Append(mFirePos.x).Append("@N, @L").Append(mFirePos.y);
            mFormatStr.Append("@N));\n\n");

            mFormatStr.Append("@C// Or rotate and scale it just like a sprite!\n");
            mFormatStr.Append("@MRB@N.DrawPixelBuffer(@NpixelBuffer, @Knew @MVector2i@N(@L").Append(mFireSize.width).Append("@N, @L");
            mFormatStr.Append(mFireSize.height).Append("@N), @Knew @MRect2i@N(@L").Append(mFireRect.x).Append("@N, @L").Append(mFireRect.y);
            mFormatStr.Append("@N, @L").Append(mFireRect.width).Append("@N, @L").Append(mFireRect.height).Append("@N), ");
            mFormatStr.Append("@Knew @MVector2i@N(@L").Append(mFireSize.width / 2).Append("@N, @L").Append(mFireSize.height / 2).Append("@N), ");
            mFormatStr.Append("@L").Append(mRotation, 2).Append("@N);");

            RB.CameraReset();

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawFire()
        {
            // Still frame
            mFirePos = new Vector2i((RB.DisplaySize.width / 2) - (mFireSize.width / 2), (RB.DisplaySize.height / 7 * 3) - (mFireSize.height / 2));
            RB.DrawPixelBuffer(mPixelBuffer, mFireSize, mFirePos);

            // Animated frame
            mFireRect = new Rect2i((RB.DisplaySize.width / 2) - (mFireSize.width / 2), (RB.DisplaySize.height / 5 * 4) - (mFireSize.height / 2), mFireSize.width, mFireSize.height);
            int growth = (int)(Mathf.Sin(Time.time * 1) * 32);
            mFireRect.Expand(growth);

            mRotation = Mathf.Sin(Time.time * 2) * 10;
            var pivot = new Vector2i(mFireRect.width / 2, mFireRect.height / 2);

            RB.DrawPixelBuffer(mPixelBuffer, mFireSize, mFireRect, pivot, mRotation, RB.PIXEL_BUFFER_UNCHANGED);
        }
    }
}
