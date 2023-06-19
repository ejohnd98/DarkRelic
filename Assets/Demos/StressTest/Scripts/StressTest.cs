namespace RetroBlitDemoStressTest
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Stress Test. Tests blitting performance of RetroBlit
    /// </summary>
    public class StressTest : RB.IRetroBlitGame
    {
        /// <summary>
        /// Sprite sheet
        /// </summary>
        public SpriteSheetAsset spriteSheet = new SpriteSheetAsset();

        private int mCurrentScene = 0;
        private List<SceneDemo> mScenes = new List<SceneDemo>();

        /// <summary>
        /// Constructor
        /// </summary>
        public StressTest()
        {
        }

        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware configuration</returns>
        public RB.HardwareSettings QueryHardware()
        {
            var hw = new RB.HardwareSettings();

            hw.DisplaySize = new Vector2i(640, 360);

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool Initialize()
        {
            spriteSheet.Load("StressTest/Sprites");
            spriteSheet.grid = new SpriteGrid(new Vector2i(16, 16));
            RB.SpriteSheetSet(spriteSheet);

            mScenes.Add(new SceneSpriteStress());
            mScenes.Add(new SceneTextStress());
            mScenes.Add(new SceneSpriteTextStress());
            mScenes.Add(new ScenePrimitiveStress());
            mScenes.Add(new ScenePixelStress());
            mScenes.Add(new SceneEllipseStress());
            mScenes.Add(new SceneOffscreenTest());

            mCurrentScene = 0;
            mScenes[mCurrentScene].Enter();

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
        /// Switch to previous scene, wrap around at the end
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
    }
}
