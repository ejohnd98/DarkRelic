namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Main game scene
    /// </summary>
    public class SceneGame : Scene
    {
        /// <summary>
        /// The width of the game level, in tiles
        /// </summary>
        public const int LEVEL_WIDTH = 181;

        private EntityPlayer mPlayerOne;
        private EntityPlayer mPlayerTwo;

        private EntityFlag mFlagOne;
        private EntityFlag mFlagTwo;

        private EntityFlagSlot mFlagOneSlot;
        private EntityFlagSlot mFlagTwoSlot;

        private int mWinningPlayer = 0;
        private float mTimeoutUntilReset = 5.0f;

        /// <summary>
        /// Get enemy flag
        /// </summary>
        /// <param name="playerNum">What player to get flag for</param>
        /// <returns>Enemy flag</returns>
        public EntityFlag GetEnemyFlag(int playerNum)
        {
            if (playerNum == RB.PLAYER_ONE)
            {
                return mFlagTwo;
            }
            else if (playerNum == RB.PLAYER_TWO)
            {
                return mFlagOne;
            }

            return null;
        }

        /// <summary>
        /// Get flag slot
        /// </summary>
        /// <param name="playerNum">Whether player to get slot for</param>
        /// <returns>Flag slot</returns>
        public EntityFlagSlot GetFlagSlot(int playerNum)
        {
            if (playerNum == RB.PLAYER_ONE)
            {
                return mFlagOneSlot;
            }
            else if (playerNum == RB.PLAYER_TWO)
            {
                return mFlagTwoSlot;
            }

            return null;
        }

        /// <summary>
        /// Set the winning player
        /// </summary>
        /// <param name="playerNum">Winning player</param>
        public void SetWinner(int playerNum)
        {
            if (mWinningPlayer == 0)
            {
                mWinningPlayer = playerNum;
            }
        }

        /// <summary>
        /// Get the winning player
        /// </summary>
        /// <returns>Winning player</returns>
        public int GetWinner()
        {
            return mWinningPlayer;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public override bool Initialize()
        {
            base.Initialize();

            mPlayerOne = new EntityPlayer(new Vector2(RB.SpriteSheetGet().grid.cellSize.width * 2, 420), RB.PLAYER_ONE);
            mPlayerTwo = new EntityPlayer(new Vector2(((LEVEL_WIDTH - 1) * RB.SpriteSheetGet().grid.cellSize.width) - (RB.SpriteSheetGet().grid.cellSize.width * 2), 420), RB.PLAYER_TWO);

            mFlagOne = new EntityFlag(new Vector2(RB.SpriteSheetGet().grid.cellSize.width, (RB.SpriteSheetGet().grid.cellSize.height * 25) + 5), RB.PLAYER_ONE);
            mFlagTwo = new EntityFlag(new Vector2(((LEVEL_WIDTH - 2) * RB.SpriteSheetGet().grid.cellSize.width) - 8, (RB.SpriteSheetGet().grid.cellSize.height * 25) + 5), RB.PLAYER_TWO);

            mFlagOneSlot = new EntityFlagSlot(new Vector2((RB.SpriteSheetGet().grid.cellSize.width * 3) - 8, (RB.SpriteSheetGet().grid.cellSize.height * 25) + 5), RB.PLAYER_ONE);
            mFlagTwoSlot = new EntityFlagSlot(new Vector2((LEVEL_WIDTH - 4) * RB.SpriteSheetGet().grid.cellSize.width, (RB.SpriteSheetGet().grid.cellSize.height * 25) + 5), RB.PLAYER_TWO);

            return true;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            mPlayerOne.Update();
            mPlayerTwo.Update();
            mFlagOne.Update();
            mFlagTwo.Update();

            if (mWinningPlayer != 0)
            {
                if (mTimeoutUntilReset > 0)
                {
                    mTimeoutUntilReset -= Time.deltaTime;

                    if (mTimeoutUntilReset <= 0)
                    {
                        SceneTitle scene = new SceneTitle();
                        scene.Initialize();
                        var game = (SuperFlagRun)RB.Game;
                        game.SwitchScene(scene);
                    }
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            RB.Clear(new Color32(127, 213, 221, 255));

            var game = (SuperFlagRun)RB.Game;

            if (game.GameMap == null)
            {
                RB.Print(new Vector2i(2, 2), DemoUtil.IndexToRGB(14), "Failed to load game TMX map.\nPlease try re-importing the map Demos/SuperFlagRun/GameMap.tmx in Unity");
                return;
            }

            // Draw Player One view
            RB.CameraReset();

            Vector2i cameraOffset;

            if (!game.SinglePlayer)
            {
                RB.ClipSet(new Rect2i(0, 0, RB.DisplaySize.width, RB.DisplaySize.height / 2));
                cameraOffset = GetCameraOffset(mPlayerOne);
            }
            else
            {
                cameraOffset = GetCameraOffset(mPlayerOne);
                cameraOffset.y -= RB.DisplaySize.height / 2;
            }

            RB.CameraSet(new Vector2i((int)cameraOffset.x, (int)cameraOffset.y));
            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_SKY);

            DrawScrollingClouds((int)cameraOffset.x, (int)cameraOffset.y);

            RB.CameraSet(new Vector2i((int)cameraOffset.x, (int)cameraOffset.y));

            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_BACKGROUND);
            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_TERRAIN);

            if (mWinningPlayer == 0)
            {
                mFlagOneSlot.Render();
            }

            mFlagOne.Render();
            mFlagTwo.Render();

            mPlayerTwo.Render();
            mPlayerOne.Render();

            // Draw Castles
            RB.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i(16, RB.SpriteSheetGet().grid.cellSize.height * 26));
            RB.DrawCopy(new Rect2i(80, 64, 48, 64), new Vector2i((RB.SpriteSheetGet().grid.cellSize.width * LEVEL_WIDTH) - 64, RB.SpriteSheetGet().grid.cellSize.height * 26), 0);

            if (!game.SinglePlayer)
            {
                // Draw Player Two view
                RB.ClipSet(new Rect2i(0, RB.DisplaySize.height / 2, RB.DisplaySize.width, RB.DisplaySize.height / 2));

                cameraOffset = GetCameraOffset(mPlayerTwo);

                RB.CameraSet(new Vector2i((int)cameraOffset.x, (int)cameraOffset.y - (RB.DisplaySize.height / 2)));
                RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_SKY);

                DrawScrollingClouds((int)cameraOffset.x, (int)cameraOffset.y - (RB.DisplaySize.height / 2));

                cameraOffset = GetCameraOffset(mPlayerTwo);
                RB.CameraSet(new Vector2i((int)cameraOffset.x, (int)cameraOffset.y - (RB.DisplaySize.height / 2)));

                RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_BACKGROUND);
                RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_TERRAIN);

                if (mWinningPlayer == 0)
                {
                    mFlagTwoSlot.Render();
                }

                mFlagOne.Render();
                mFlagTwo.Render();

                mPlayerOne.Render();
                mPlayerTwo.Render();

                // Draw Castles
                RB.DrawCopy(new Rect2i(0, 64, 48, 64), new Vector2i(16, RB.SpriteSheetGet().grid.cellSize.height * 26));
                RB.DrawCopy(new Rect2i(80, 64, 48, 64), new Vector2i((RB.SpriteSheetGet().grid.cellSize.width * LEVEL_WIDTH) - 64, RB.SpriteSheetGet().grid.cellSize.height * 26), 0);

                RB.ClipReset();
                RB.CameraReset();

                // Draw divider
                for (int x = 0; x < RB.DisplaySize.width; x += 16)
                {
                    RB.DrawSprite(RB.SpriteIndex(0, 0), new Vector2i(x, (RB.DisplaySize.height / 2) - 4));
                }
            }

            RB.ClipReset();
            RB.CameraReset();

            if (mWinningPlayer != 0)
            {
                string playerOneStr = "LOSER";
                string playerTwoStr = "WINNER";
                if (mWinningPlayer == RB.PLAYER_ONE)
                {
                    playerOneStr = "WINNER";
                    playerTwoStr = "LOSER";
                }

                int textOffsetX = (int)(Mathf.Cos(Time.time * 6.0f) * 8);
                int textOffsetY = (int)(Mathf.Sin(Time.time * 6.0f) * 5);
                Vector2i textSize;
                string text = playerOneStr;
                textSize = RB.PrintMeasure(game.assets.gameFont, text);
                RB.Print(game.assets.gameFont, new Vector2i((RB.DisplaySize.width / 2) - (textSize.width / 2) + textOffsetX, (RB.DisplaySize.height / 4) - (textSize.height / 2) + textOffsetY), Color.white, text);

                text = playerTwoStr;
                textSize = RB.PrintMeasure(game.assets.gameFont, text);
                RB.Print(game.assets.gameFont, new Vector2i((RB.DisplaySize.width / 2) - (textSize.width / 2) + textOffsetX, (RB.DisplaySize.height / 4 * 3) - (textSize.height / 2) + textOffsetY), Color.white, text);
            }

            // Let base render last so it can overlay the scene
            base.Render();
        }

        /// <summary>
        /// Enter the scene
        /// </summary>
        public override void Enter()
        {
            base.Enter();
        }

        private Vector2 GetCameraOffset(EntityPlayer player)
        {
            // Clip the camera, note we clip first and last column of tiles out because they are special invisible tiles that
            // block movement, they're not meant to be shown
            int cameraX = (int)player.Pos.x - (RB.DisplaySize.width / 2) + (RB.SpriteSheetGet().grid.cellSize.width / 2);
            if (cameraX < RB.SpriteSheetGet().grid.cellSize.width)
            {
                cameraX = RB.SpriteSheetGet().grid.cellSize.width;
            }

            if (cameraX > (RB.SpriteSheetGet().grid.cellSize.width * LEVEL_WIDTH) - RB.DisplaySize.width - RB.SpriteSheetGet().grid.cellSize.width)
            {
                cameraX = (RB.SpriteSheetGet().grid.cellSize.width * LEVEL_WIDTH) - RB.DisplaySize.width - RB.SpriteSheetGet().grid.cellSize.width;
            }

            int cameraY = (int)player.Pos.y - (int)(RB.SpriteSheetGet().grid.cellSize.height * 2.5f);
            if (cameraY < RB.SpriteSheetGet().grid.cellSize.height)
            {
                cameraY = RB.SpriteSheetGet().grid.cellSize.height;
            }

            var game = (SuperFlagRun)RB.Game;

            if (game.GameMap == null)
            {
                return Vector2.zero;
            }

            if (cameraY > (RB.SpriteSheetGet().grid.cellSize.height * game.GameMapSize.height) - (RB.DisplaySize.height / 2) - RB.SpriteSheetGet().grid.cellSize.height)
            {
                cameraY = (RB.SpriteSheetGet().grid.cellSize.height * game.GameMapSize.height) - (RB.DisplaySize.height / 2) - RB.SpriteSheetGet().grid.cellSize.height;
            }

            return new Vector2(cameraX, cameraY);
        }

        private void DrawScrollingClouds(int xoffset, int yoffset)
        {
            var game = (SuperFlagRun)RB.Game;

            if (game.GameMap == null)
            {
                return;
            }

            int totalMapWidth = game.GameMapSize.width * RB.SpriteSheetGet().grid.cellSize.width;
            int offset = (int)(Time.time * 25) % totalMapWidth;

            RB.CameraSet(new Vector2i(xoffset + offset, yoffset));
            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_CLOUDS);

            RB.CameraSet(new Vector2i(xoffset + offset - totalMapWidth, yoffset));
            RB.DrawMapLayer(SuperFlagRun.MAP_LAYER_CLOUDS);
        }
    }
}
