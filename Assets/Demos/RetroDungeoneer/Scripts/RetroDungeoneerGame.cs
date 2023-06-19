namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Retro Dungeoneer
    /// </summary>
    public class RetroDungeoneerGame : RB.IRetroBlitGame
    {
        /// <summary>
        /// Game assets
        /// </summary>
        public Assets assets = new Assets();

        private SceneGame mSceneGame;
        private SceneMainMenu mSceneMainMenu;

        private SceneMessage mSceneMessage;

        private Scene mCurrentScene = null;

        /// <summary>
        /// Get map
        /// </summary>
        public GameMap map
        {
            get
            {
                return mSceneGame.map;
            }
        }

        /// <summary>
        /// Get Flood Map
        /// </summary>
        public FloodMap floodMap
        {
            get
            {
                return mSceneGame.floodMap;
            }
        }

        /// <summary>
        /// Game camera
        /// </summary>
        public GameCamera camera
        {
            get
            {
                return mSceneGame.camera;
            }
        }

        /// <summary>
        /// Player
        /// </summary>
        public EntityID player
        {
            get
            {
                return mSceneGame.player;
            }
        }

        /// <summary>
        /// Query hardware. Here you initialize your retro game hardware.
        /// </summary>
        /// <returns>Hardware settings</returns>
        public RB.HardwareSettings QueryHardware()
        {
            var hw = new RB.HardwareSettings
            {
                // Set your display size
                DisplaySize = new Vector2i(1280 / 2, 720 / 2)
            };

            // Set tilemap maximum size, default is 256, 256. Keep this close to your minimum required size to save on memory
            //// hw.MapSize = new Vector2i(256, 256);

            // Set tilemap maximum layers, default is 8. Keep this close to your minimum required size to save on memory
            //// hw.MapLayers = 8;

            return hw;
        }

        /// <summary>
        /// Initialize your game here.
        /// </summary>
        /// <returns>Return true if successful</returns>
        public bool Initialize()
        {
            assets.LoadAll();

            // You can load a spritesheet here
            RB.SpriteSheetSet(assets.spriteSheet);

            RB.EffectSet(RB.Effect.Scanlines, 0.5f);
            RB.EffectSet(RB.Effect.ChromaticAberration, new Vector2i(20, 20));

            EntityFunctions.Initialize();
            S.InitializeAnims();

            SoundSerializer.Initialize();

            InitializeNewGame.InitializeConstants();

            RenderFunctions.Initialize();

            mSceneGame = new SceneGame();
            mSceneMainMenu = new SceneMainMenu();
            mSceneMessage = new SceneMessage();

            // Generate tile grid, this is a one time operation, we can keep reusing the grid
            var gridColor = Color.white;
            for (int x = 0; x < RB.MapSize.width; x++)
            {
                for (int y = 0; y < RB.MapSize.height; y++)
                {
                    RB.MapSpriteSet(C.LAYER_GRID, new Vector2i(x, y), S.GRID, gridColor);
                }
            }

            ChangeScene(SceneEnum.MAIN_MENU);

            RB.MapLayerSpriteSheetSet(C.LAYER_GRID, assets.spriteSheet);
            RB.MapLayerSpriteSheetSet(C.LAYER_TERRAIN, assets.spriteSheet);
            RB.MapLayerSpriteSheetSet(C.LAYER_VISIBILITY, assets.spriteSheet);

            // Collect any garbage created during initilization to avoid a performance hiccup later.
            System.GC.Collect();

            return true;
        }

        /// <summary>
        /// Update, your game logic should live here. Update is called at a fixed interval of 60 times per second.
        /// </summary>
        public void Update()
        {
            // First process message box scene, and if it consumes the update event then
            // don't update any other scene
            if (mSceneMessage.Update())
            {
                return;
            }

            if (mCurrentScene != null)
            {
                mCurrentScene.Update();
            }
        }

        /// <summary>
        /// Render, your drawing code should go here.
        /// </summary>
        public void Render()
        {
            if (mCurrentScene != null)
            {
                mCurrentScene.Render();
            }

            mSceneMessage.Render();
        }

        /// <summary>
        /// Change the current game scene
        /// </summary>
        /// <param name="sceneEnum">Scene enum to change to</param>
        /// <param name="sceneParameters">Scene parameters</param>
        public void ChangeScene(SceneEnum sceneEnum, object sceneParameters = null)
        {
            Scene newScene = null;

            switch (sceneEnum)
            {
                case SceneEnum.MAIN_MENU:
                    newScene = mSceneMainMenu;
                    break;

                case SceneEnum.GAME:
                    newScene = mSceneGame;
                    break;
            }

            if (newScene != null)
            {
                if (mCurrentScene != null)
                {
                    mCurrentScene.Exit();
                }

                newScene.Enter(sceneParameters);

                mCurrentScene = newScene;
            }
        }

        /// <summary>
        /// Show a message box
        /// </summary>
        /// <param name="header">Header text</param>
        /// <param name="message">Message text</param>
        /// <param name="options">Options</param>
        public void ShowMessageBox(FastString header, FastString message, List<SceneMessage.MessageBoxOption> options)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            RB.SoundPlay(game.assets.soundMenuOpen, 1, RandomUtils.RandomPitch(0.1f));
            mSceneMessage.ShowMessageBox(header, message, options);
        }

        /// <summary>
        /// Close message box
        /// </summary>
        public void CloseMessageBox()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            RB.SoundPlay(game.assets.soundMenuClose, 1, RandomUtils.RandomPitch(0.1f));
            mSceneMessage.CloseMessageBox();
        }
    }
}
