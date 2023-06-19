namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Simple platformer demo game, with custom physics engine
    /// </summary>
    public class SuperFlagRun : RB.IRetroBlitGame
    {
        /// <summary>
        /// Terrain layer
        /// </summary>
        public const int MAP_LAYER_TERRAIN = 0;

        /// <summary>
        /// Background layer
        /// </summary>
        public const int MAP_LAYER_BACKGROUND = 1;

        /// <summary>
        /// Clouds layer
        /// </summary>
        public const int MAP_LAYER_CLOUDS = 2;

        /// <summary>
        /// Sky layer
        /// </summary>
        public const int MAP_LAYER_SKY = 3;

        /// <summary>
        /// Title terrain layer
        /// </summary>
        public const int MAP_LAYER_TITLE_TERRAIN = 4;

        /// <summary>
        /// Title deco layer
        /// </summary>
        public const int MAP_LAYER_TITLE_DECO = 5;

        /// <summary>
        /// Title deco layer
        /// </summary>
        public const int MAP_LAYER_TITLE_SKY = 6;

        /// <summary>
        /// Game assets
        /// </summary>
        public Assets assets = new Assets();

        private readonly bool mStandalone = true;
        private readonly bool mSinglePlayer = false;

        private readonly TMXMapAsset mGameMap = new TMXMapAsset();
        private readonly TMXMapAsset mTitleMap = new TMXMapAsset();

        private Vector2i mGameMapSize;
        private Scene mCurrentScene;
        private Scene mNextScene;

        private bool mEffectsOn = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="standalone">If true the game will not render to display, used by OldDays demo which handles it's own rendering pass</param>
        /// <param name="singlePlayer">If true the game will run in single player mode</param>
        public SuperFlagRun(bool standalone = true, bool singlePlayer = false)
        {
            mStandalone = standalone;
            mSinglePlayer = singlePlayer;
        }

        /// <summary>
        /// Get the size of the game map
        /// </summary>
        public Vector2i GameMapSize
        {
            get { return mGameMapSize; }
        }

        /// <summary>
        /// Get title map
        /// </summary>
        public TMXMapAsset GameMap
        {
            get { return mGameMap; }
        }

        /// <summary>
        /// Get the game map
        /// </summary>
        public TMXMapAsset TitleMap
        {
            get { return mTitleMap; }
        }

        /// <summary>
        /// Get the current scene
        /// </summary>
        public Scene CurrentScene
        {
            get { return mCurrentScene; }
        }

        /// <summary>
        /// True if single player only
        /// </summary>
        public bool SinglePlayer
        {
            get { return mSinglePlayer; }
        }

        /// <summary>
        /// Query hardware
        /// </summary>
        /// <returns>Hardware settings</returns>
        public RB.HardwareSettings QueryHardware()
        {
            var hw = new RB.HardwareSettings
            {
                MapSize = new Vector2i(200, 32),
                MapLayers = 7,
                DisplaySize = new Vector2i(480, 270)
            };

            return hw;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public bool Initialize()
        {
            assets.LoadAll();

            if (!LoadMap())
            {
                return false;
            }

            RB.MusicVolumeSet(0.5f);
            RB.MusicPlay(assets.musicGame);

            SceneTitle scene = new SceneTitle();
            scene.Initialize();

            SwitchScene(scene);

            if (!mStandalone)
            {
                RB.PresentDisable();
            }

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (mEffectsOn)
            {
                RB.EffectSet(RB.Effect.Scanlines, 0.25f);
                RB.EffectSet(RB.Effect.Noise, 0.05f);
                RB.EffectSet(RB.Effect.ChromaticAberration, new Vector2i(50, 50));
                RB.EffectSet(RB.Effect.Saturation, 0.25f);
            }
            else
            {
                RB.EffectReset();
            }

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                Application.Quit();
            }

            if (RB.ButtonPressed(RB.BTN_MENU) || RB.KeyPressed(KeyCode.F1))
            {
                mEffectsOn = !mEffectsOn;
            }

            if (mCurrentScene != null)
            {
                mCurrentScene.Update();
            }

            if (mNextScene != null)
            {
                if (mCurrentScene.TransitionDone())
                {
                    mCurrentScene = mNextScene;
                    mCurrentScene.Enter();
                    mNextScene = null;
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            mCurrentScene.Render();
        }

        /// <summary>
        /// Switch to a new scene
        /// </summary>
        /// <param name="newScene">New scene</param>
        public void SwitchScene(Scene newScene)
        {
            if (mCurrentScene == null)
            {
                mCurrentScene = newScene;
                mCurrentScene.Enter();
            }
            else
            {
                mNextScene = newScene;
                mCurrentScene.Exit();
            }
        }

        private bool LoadMap()
        {
            mTitleMap.Load("SuperFlagRun/TitleMap");
            mGameMap.Load("SuperFlagRun/GameMap");

            if (mTitleMap != null && mTitleMap.status == RB.AssetStatus.Ready)
            {
                mTitleMap.LoadLayer("Terrain", SuperFlagRun.MAP_LAYER_TITLE_TERRAIN);
                mTitleMap.LoadLayer("Deco", SuperFlagRun.MAP_LAYER_TITLE_DECO);
                mTitleMap.LoadLayer("Sky", SuperFlagRun.MAP_LAYER_TITLE_SKY);
            }

            if (mGameMap != null)
            {
                mGameMap.LoadLayer("Sky", SuperFlagRun.MAP_LAYER_SKY);
                mGameMap.LoadLayer("Clouds", SuperFlagRun.MAP_LAYER_CLOUDS);
                mGameMap.LoadLayer("Terrain", SuperFlagRun.MAP_LAYER_TERRAIN);
                mGameMap.LoadLayer("Background", SuperFlagRun.MAP_LAYER_BACKGROUND);
            }

            RB.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_TITLE_TERRAIN, assets.spriteSheetTerrain);
            RB.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_TITLE_DECO, assets.spriteSheetDeco);
            RB.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_TITLE_SKY, assets.spriteSheetDeco);

            RB.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_SKY, assets.spriteSheetDeco);
            RB.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_CLOUDS, assets.spriteSheetDeco);
            RB.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_TERRAIN, assets.spriteSheetTerrain);
            RB.MapLayerSpriteSheetSet(SuperFlagRun.MAP_LAYER_BACKGROUND, assets.spriteSheetDeco);

            RB.SpriteSheetSet(assets.spriteSheetTerrain);

            if (mGameMap != null)
            {
                mGameMapSize = mGameMap.size;

                // Convert TMXProperties to simple ColliderInfo.ColliderType, for faster access
                for (int x = 0; x < mGameMapSize.width; x++)
                {
                    for (int y = 0; y < mGameMapSize.height; y++)
                    {
                        var tilePos = new Vector2i(x, y);
                        var tileProps = RB.MapDataGet<TMXMapAsset.TMXProperties>(MAP_LAYER_TERRAIN, tilePos);
                        if (tileProps != null)
                        {
                            RB.MapDataSet<ColliderInfo.ColliderType>(MAP_LAYER_TERRAIN, tilePos, (ColliderInfo.ColliderType)tileProps.GetInt("ColliderType"));
                        }
                        else
                        {
                            RB.MapDataSet<ColliderInfo.ColliderType>(MAP_LAYER_TERRAIN, tilePos, ColliderInfo.ColliderType.NONE);
                        }
                    }
                }
            }

            return true;
        }
    }
}
