namespace RetroBlitDemoReel
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Demo Reel demo program. Shows off a majority of the RetroBlit features.
    /// </summary>
    public class DemoReel : RB.IRetroBlitGame
    {
        private readonly List<SceneDemo> mScenes = new List<SceneDemo>();

        private readonly float[] mNextSceneTime = new float[]
                                                       {
                                                       8.0f, // SceneGameLoop
                                                       8.0f, // SceneDrawing
                                                       4.0f, // SceneLines
                                                       4.0f, // SceneText
                                                       4.0f, // SceneClipOffscreen
                                                       4.0f, // SceneTilemap
                                                       4.0f, // SceneTMXProps
                                                       4.0f, // SceneInfiniteMap
                                                       4.0f, // SceneSpriteSheetDraw
                                                       4.0f, // SceneSpritepack
                                                       4.0f, // ScenePixelBuffer
                                                       4.0f, // SceneAsyncAssets
                                                       4.0f, // SceneEase
                                                       4.0f, // SceneShader
                                                       // 2.0f, // Pixel Format   (Taking this out, not interesting)
                                                       // 2.0f, // Pixel Format   (Taking this out, not interesting)
                                                       4.0f, // SceneSound
                                                       4.0f, // SceneInput
                                                       2.0f, // SceneEffects(RB.Effect.Scanlines)
                                                       2.0f, // SceneEffects(RB.Effect.Noise)
                                                       2.0f, // SceneEffects(RB.Effect.ChromaticAberration)
                                                       2.0f, // SceneEffects(RB.Effect.Saturation)
                                                       2.0f, // SceneEffects(RB.Effect.Curvature)
                                                       2.0f, // SceneEffects(RB.Effect.Slide)
                                                       2.0f, // SceneEffects(RB.Effect.Wipe)
                                                       2.0f, // SceneEffects(RB.Effect.Shake)
                                                       2.0f, // SceneEffects(RB.Effect.Zoom)
                                                       2.0f, // SceneEffects(RB.Effect.Rotation)
                                                       2.0f, // SceneEffects(RB.Effect.ColorFade)
                                                       2.0f, // SceneEffects(RB.Effect.ColorTint)
                                                       2.0f, // SceneEffects(RB.Effect.Negative)
                                                       2.0f, // SceneEffects(RB.Effect.Pixelate)
                                                       2.0f, // SceneEffects(RB.Effect.Pinhole)
                                                       2.0f, // SceneEffects(RB.Effect.InvertedPinhole)
                                                       2.0f, // SceneEffects(RB.Effect.Fizzle)
                                                       4.0f, // SceneEffectShader
                                                       // 48.0f // SceneEffectApply (Taking this out, not interesting)
                                                       };

        private readonly bool MUSIC_SYNC_ENABLED = false;

        private readonly AudioAsset mMusic = new AudioAsset();

        private int mCurrentScene = 0;

        private int mCurrentTimeIndex = -1;
        private float mNextTimeStop = 0;
        private double mStartTime = 0;

        private bool mMusicIsPlaying = false;

        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware configuration</returns>
        public RB.HardwareSettings QueryHardware()
        {
            var hw = new RB.HardwareSettings
            {
                DisplaySize = new Vector2i(640, 360)
            };

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool Initialize()
        {
            if (MUSIC_SYNC_ENABLED)
            {
                Cursor.visible = false;
            }

            mMusic.Load("DemoReel/Music/Starter8bitDub");

            MusicPlay();

            mScenes.Add(new SceneGameLoop());
            mScenes.Add(new SceneDrawing());
            mScenes.Add(new SceneLines());
            mScenes.Add(new SceneText());
            mScenes.Add(new SceneClipOffscreen());
            mScenes.Add(new SceneTilemap());
            mScenes.Add(new SceneTMXProps());
            mScenes.Add(new SceneInfiniteMap());
            mScenes.Add(new SceneSpriteSheetDraw());
            mScenes.Add(new SceneSpritepack());
            mScenes.Add(new ScenePixelBuffer());
            mScenes.Add(new SceneAsyncAssets());
            mScenes.Add(new SceneEase());
            mScenes.Add(new SceneShader());

            // Cut pixel style scenes out of autoplay (used to make promo video), demo reel video was getting too long
            if (!MUSIC_SYNC_ENABLED)
            {
                mScenes.Add(new ScenePixelStyle(RB.PixelStyle.Wide));
                mScenes.Add(new ScenePixelStyle(RB.PixelStyle.Tall));
            }

            mScenes.Add(new SceneSound());
            mScenes.Add(new SceneInput());
            mScenes.Add(new SceneEffects(RB.Effect.Scanlines));
            mScenes.Add(new SceneEffects(RB.Effect.Noise));
            mScenes.Add(new SceneEffects(RB.Effect.ChromaticAberration));
            mScenes.Add(new SceneEffects(RB.Effect.Saturation));
            mScenes.Add(new SceneEffects(RB.Effect.Curvature));
            mScenes.Add(new SceneEffects(RB.Effect.Slide));
            mScenes.Add(new SceneEffects(RB.Effect.Wipe));
            mScenes.Add(new SceneEffects(RB.Effect.Shake));
            mScenes.Add(new SceneEffects(RB.Effect.Zoom));
            mScenes.Add(new SceneEffects(RB.Effect.Rotation));
            mScenes.Add(new SceneEffects(RB.Effect.ColorFade));
            mScenes.Add(new SceneEffects(RB.Effect.ColorTint));
            mScenes.Add(new SceneEffects(RB.Effect.Negative));
            mScenes.Add(new SceneEffects(RB.Effect.Pixelate));
            mScenes.Add(new SceneEffects(RB.Effect.Pinhole));
            mScenes.Add(new SceneEffects(RB.Effect.InvertedPinhole));
            mScenes.Add(new SceneEffects(RB.Effect.Fizzle));
            mScenes.Add(new SceneEffectShader());
            mScenes.Add(new SceneEffectApply());

            mCurrentScene = 0;
            mScenes[mCurrentScene].Enter();

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (MUSIC_SYNC_ENABLED)
            {
                if (mCurrentTimeIndex == -1)
                {
                    mStartTime = Time.time + 0.08f;  // + scene flips later, - sooner
                    mNextTimeStop = mNextSceneTime[0];
                    mCurrentTimeIndex = 0;
                }

                if (mCurrentTimeIndex < mNextSceneTime.Length)
                {
                    double curTime = Time.time - mStartTime;
                    if (curTime >= mNextTimeStop)
                    {
                        mCurrentTimeIndex++;
                        mNextTimeStop += mNextSceneTime[mCurrentTimeIndex];
                        NextScene();
                    }
                }
            }

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                Application.Quit();
            }

            mScenes[mCurrentScene].Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            mScenes[mCurrentScene].Render();
        }

        /// <summary>
        /// Switch to next scene, wrap around at the end
        /// </summary>
        public void NextScene()
        {
            int newScene = mCurrentScene + 1;
            if (newScene >= mScenes.Count)
            {
                newScene = 0;
            }

            mScenes[mCurrentScene].Exit();
            mCurrentScene = newScene;
            mScenes[mCurrentScene].Enter();
        }

        /// <summary>
        /// Switch to previous scene, wrap around at the start
        /// </summary>
        public void PreviousScene()
        {
            int newScene = mCurrentScene - 1;
            if (newScene < 0)
            {
                newScene = mScenes.Count - 1;
            }

            mScenes[mCurrentScene].Exit();
            mCurrentScene = newScene;
            mScenes[mCurrentScene].Enter();
        }

        /// <summary>
        /// Play music
        /// </summary>
        public void MusicPlay()
        {
            RB.MusicVolumeSet(MusicVolume());
            RB.MusicPlay(mMusic);
            mMusicIsPlaying = true;
        }

        /// <summary>
        /// Stop music
        /// </summary>
        public void MusicStop()
        {
            RB.MusicStop();
            mMusicIsPlaying = false;
        }

        /// <summary>
        /// Get the music volume
        /// </summary>
        /// <returns>Volume</returns>
        public float MusicVolume()
        {
            return 0.75f;
        }

        /// <summary>
        /// Return true if music is playing
        /// </summary>
        /// <returns>True if playing</returns>
        public bool IsMusicPlaying()
        {
            return mMusicIsPlaying;
        }
    }
}
