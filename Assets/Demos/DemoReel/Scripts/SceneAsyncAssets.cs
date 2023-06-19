namespace RetroBlitDemoReel
{
    using UnityEngine;

    /// <summary>
    /// Demonstrate pixel buffers
    /// </summary>
    public class SceneAsyncAssets : SceneDemo
    {
        private SpriteSheetAsset mSpriteSheet = new SpriteSheetAsset();
        private AudioAsset mAudio = new AudioAsset();

        private float mSpriteSheetProgress = 0.0f;
        private float mAudioProgress = 0.0f;

        private bool mSoundPlayed;

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
            mSpriteSheet.Load("DemoReel/MockSpriteSheetDownload");
            mAudio.Load("DemoReel/MockAudioDownload");

            mSoundPlayed = false;
            mSpriteSheetProgress = 0;
            mAudioProgress = 0;

            base.Enter();
        }

        /// <summary>
        /// Handle scene exit
        /// </summary>
        public override void Exit()
        {
            mSpriteSheet.Unload();
            mAudio.Unload();

            base.Exit();
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (mAudioProgress >= 0.999f && !mSoundPlayed)
            {
                RB.SoundPlay(mAudio);
                mSoundPlayed = true;
            }

            if (mSpriteSheetProgress < 1.0f)
            {
                mSpriteSheetProgress = Mathf.Clamp01(mSpriteSheetProgress + (Time.deltaTime * Random.Range(0.2f, 1.0f)));
            }

            if (mAudioProgress < 1.0f)
            {
                mAudioProgress = Mathf.Clamp01(mAudioProgress + (Time.deltaTime * Random.Range(0.2f, 0.5f)));
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            var demo = (DemoReel)RB.Game;

            RB.Clear(DemoUtil.IndexToRGB(1));

            int x = 4;
            int y = 4;

            DrawCode(x, y);

            x += 76;
            y += 200;

            DrawSpriteSheet(x, y);

            y += mSpriteSheet.sheetSize.height + 16;

            DrawSound(x, y);
        }

        private void DrawCode(int x, int y)
        {
            var demo = (DemoReel)RB.Game;

            RB.CameraSet(new Vector2i(-x, -y - 25));

            mFormatStr.Set("@C// RetroBlit assets can also be loaded asynchronously from Resources, WWW, and Addressable Assets (with Unity AA package installed)!@N\n");
            mFormatStr.Append("@MSpriteSheetAsset@N spriteSheet = @Knew@N SpriteSheetAsset();\n");
            mFormatStr.Append("@MAudioAsset@N audio = @Knew@N AudioAsset();\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@Kvoid @MInitialize@N() {\n");
            mFormatStr.Append("    spriteSheet.Load(@S\"https://www.retroblit.com/superflagrun.png\"@N, @MRB@N.@MAssetSource@N.@LWWW@N);\n");
            mFormatStr.Append("    audio.Load(@S\"ding\"@N, @MRB@N.@MAssetSource@N.@LAddressableAssets@N);\n");
            mFormatStr.Append("};\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@Kvoid@N Update() {\n");
            mFormatStr.Append("    @Kif@N (audio.status == @MRB@N.@MAssetStatus@N.@LReady@N) {\n");
            mFormatStr.Append("        @MRB@N.SoundPlay(audio);\n");
            mFormatStr.Append("    }\n");
            mFormatStr.Append("};\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("@Kvoid@N Render() {\n");
            mFormatStr.Append("    @Kif@N (spriteSheet.status == @MRB@N.@MAssetStatus@N.@LReady@N) {\n");
            mFormatStr.Append("        @MRB@N.SpriteSheetSet(spriteSheet);\n");
            mFormatStr.Append("        @MRB@N.DrawSprite(@L0@N, @Knew@N Vector2i(@L100@N, @L50@N));\n");
            mFormatStr.Append("    }\n");
            mFormatStr.Append("\n");
            mFormatStr.Append("    @Kif@N (spriteSheet.status != @MRB@N.@MAssetStatus@N.@LReady@N) DrawSpriteSheetProgress(spriteSheet.progress);\n");
            mFormatStr.Append("    @Kif@N (audio.status != @MRB@N.@MAssetStatus@N.@LReady@N) DrawAudioProgress(audio.progress);\n");
            mFormatStr.Append("};\n");

            RB.CameraReset();

            RB.Print(new Vector2i(x, y), DemoUtil.IndexToRGB(5), DemoUtil.HighlightCode(mFormatStr, mFinalStr));
        }

        private void DrawSpriteSheet(int x, int y)
        {
            if (mSpriteSheetProgress >= 0.999f)
            {
                RB.SpriteSheetSet(mSpriteSheet);
                RB.DrawSprite(0, new Vector2i(x, y));

                var rect = new Rect2i(x, y, mSpriteSheet.sheetSize);
                RB.DrawRect(rect, DemoUtil.IndexToRGB(3));
            }
            else
            {
                var rect = new Rect2i(x, y, (int)(mSpriteSheet.sheetSize.width * mSpriteSheetProgress), mSpriteSheet.sheetSize.height);
                RB.DrawRectFill(rect, DemoUtil.IndexToRGB(4));

                rect = new Rect2i(x, y, mSpriteSheet.sheetSize);
                RB.DrawRect(rect, DemoUtil.IndexToRGB(3));

                mFormatStr.Clear();
                mFormatStr.Append("Sprite Sheet ").Append((int)(mSpriteSheetProgress * 100)).Append("%");
                RB.Print(new Vector2i(rect.x + 4, rect.y + 4), DemoUtil.IndexToRGB(5), mFormatStr);
            }
        }

        private void DrawSound(int x, int y)
        {
            int barHeight = 24;

            if (mAudioProgress >= 0.999f)
            {
                var rect = new Rect2i(x, y, mSpriteSheet.sheetSize.width, barHeight);
                RB.DrawRectFill(rect, DemoUtil.IndexToRGB(6));

                var prev = new Vector2i(0, 0);
                var offset = new Vector2i(rect.x, rect.y + (rect.height / 2));
                var freq = (Mathf.Sin(RB.Ticks / 20.0f) * 0.4f) + 0.25f;

                for (int i = 0; i <= rect.width; i += 4)
                {
                    float j = (rect.width / 4) - (i / 2);
                    float freq2 = Mathf.Sin(j * freq);
                    float amp = ((rect.height - 2) / 2) * Mathf.Cos(j / 10.0f) * Mathf.Sin(j / 20.0f);

                    if (i > rect.width / 2)
                    {
                        amp = -amp;
                    }

                    var point = new Vector2i(i, freq2 * amp);
                    RB.DrawLine(prev + offset, point + offset, DemoUtil.IndexToRGB(1));
                    prev = point;
                }

                rect = new Rect2i(x, y, mSpriteSheet.sheetSize.width, barHeight);
                RB.DrawRect(rect, DemoUtil.IndexToRGB(4));
            }
            else
            {
                var rect = new Rect2i(x, y, (int)(mSpriteSheet.sheetSize.width * mAudioProgress), barHeight);
                RB.DrawRectFill(rect, DemoUtil.IndexToRGB(4));

                rect = new Rect2i(x, y, mSpriteSheet.sheetSize.width, barHeight);
                RB.DrawRect(rect, DemoUtil.IndexToRGB(3));

                mFormatStr.Clear();
                mFormatStr.Append("Audio ").Append((int)(mAudioProgress * 100)).Append("%");
                RB.Print(new Vector2i(rect.x + 4, rect.y + 4), DemoUtil.IndexToRGB(5), mFormatStr);
            }
        }
    }
}
