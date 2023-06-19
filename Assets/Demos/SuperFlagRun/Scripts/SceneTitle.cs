namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Title scene
    /// </summary>
    public class SceneTitle : Scene
    {
        private EntityFlag mFlagOne;
        private EntityFlag mFlagTwo;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            mFlagOne = new EntityFlag(new Vector2((RB.SpriteSheetGet().grid.cellSize.width * 2) - 1, (RB.SpriteSheetGet().grid.cellSize.height * 3) + 8), RB.PLAYER_ONE);
            mFlagTwo = new EntityFlag(new Vector2(RB.DisplaySize.width - RB.SpriteSheetGet().grid.cellSize.width - 8, (RB.SpriteSheetGet().grid.cellSize.height * 3) + 8), RB.PLAYER_TWO);

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (!TransitionDone())
            {
                return;
            }

            if (RB.ButtonPressed(RB.BTN_ABXY, RB.PLAYER_ANY) || RB.ButtonPressed(RB.BTN_POINTER_ANY, RB.PLAYER_ANY))
            {
                SceneGame scene = new SceneGame();
                scene.Initialize();
                var game = (SuperFlagRun)RB.Game;
                game.SwitchScene(scene);

                RB.SoundPlay(game.assets.soundStartGame);
            }

            mFlagOne.Update();
            mFlagTwo.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            RB.Clear(new Color32(127, 213, 221, 255));

            RB.CameraReset();

            var game = (SuperFlagRun)RB.Game;

            if (game.TitleMap == null)
            {
                RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(14), "Failed to load title TMX map.\nPlease try re-importing the map Demos/SuperFlagRun/TitleMap.tmx in Unity");
                return;
            }

            RB.CameraSet(new Vector2i(RB.SpriteSheetGet().grid.cellSize.width, 0));

            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_TITLE_SKY);

            DrawScrollingClouds();

            RB.CameraSet(new Vector2i(RB.SpriteSheetGet().grid.cellSize.width, RB.SpriteSheetGet().grid.cellSize.height * 12));

            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_TITLE_DECO);
            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_TITLE_TERRAIN);

            RB.CameraSet(new Vector2i(RB.SpriteSheetGet().grid.cellSize.width, -RB.SpriteSheetGet().grid.cellSize.height * 7));

            // Draw Flags
            mFlagOne.Render();
            mFlagTwo.Render();

            // Draw Players
            int x = (RB.SpriteSheetGet().grid.cellSize.width * 3) + 8;
            int y = RB.SpriteSheetGet().grid.cellSize.height * 3;
            RB.DrawSprite(RB.SpriteIndex(0, 2), new Vector2i(x, y), 0);
            RB.DrawSprite(RB.SpriteIndex(0, 3), new Vector2i(x, y + RB.SpriteSheetGet().grid.cellSize.height), 0);

            x = RB.DisplaySize.width - (RB.SpriteSheetGet().grid.cellSize.width * 2) - 8;
            RB.DrawSprite(RB.SpriteIndex(5, 2), new Vector2i(x, y), RB.FLIP_H);
            RB.DrawSprite(RB.SpriteIndex(5, 3), new Vector2i(x, y + RB.SpriteSheetGet().grid.cellSize.height), RB.FLIP_H);

            // Draw Castles
            RB.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i(RB.SpriteSheetGet().grid.cellSize.width * 2, RB.SpriteSheetGet().grid.cellSize.height * 4));
            RB.DrawCopy(new Rect2i(80, 64, 48, 64), new Vector2i(RB.DisplaySize.width - (RB.SpriteSheetGet().grid.cellSize.width * 3), RB.SpriteSheetGet().grid.cellSize.height * 4), 0);

            // Draw Title
            RB.CameraReset();
            RB.SpriteSheetSet(game.assets.spriteSheetTitle);
            byte tint = (byte)((Mathf.Sin(Time.time * 2) * 60) + 196);
            RB.TintColorSet(new Color32(tint, tint, tint, 255));
            RB.DrawCopy(new Rect2i(0, 0, 323, 103), new Vector2i((RB.DisplaySize.width / 2) - (323 / 2), (int)(Mathf.Sin(Time.time * 2) * 6) + 15));
            RB.TintColorSet(Color.white);
            RB.SpriteSheetSet(game.assets.spriteSheetSprites);

            // Draw Press Any Button
            string str = "PRESS ANY BUTTON";
            Vector2i textSize = RB.PrintMeasure(game.assets.gameFont, str);
            RB.Print(game.assets.gameFont, new Vector2i((RB.DisplaySize.width / 2) - (textSize.width / 2), (int)(RB.DisplaySize.height * 0.55f)), Color.white, str);

            // Let base render last so it can overlay the scene
            base.Render();
        }

        private void DrawScrollingClouds()
        {
            var game = (SuperFlagRun)RB.Game;

            if (game.GameMap == null)
            {
                return;
            }

            int totalMapWidth = game.GameMapSize.width * RB.SpriteSheetGet().grid.cellSize.width;
            int offset = (int)(Time.time * 25) % totalMapWidth;

            RB.CameraSet(new Vector2i(offset, 0));
            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_CLOUDS);

            RB.CameraSet(new Vector2i(offset - totalMapWidth, 0));
            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_CLOUDS);
        }
    }
}
