namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The main menu scene
    /// </summary>
    public class SceneMainMenu : Scene
    {
        private Menu mMenuMain;
        private ResultSet mResultSet;
        private string mCheatString = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneMainMenu()
        {
            mMenuMain = new Menu(C.FSTR.Set("Main Menu"));
            mMenuMain.SetSummary(C.FSTR.Set("@FFFFFFRetro Dungeoneer"));
            mMenuMain.AddOption(C.FSTR.Set("Play a new game"));
            mMenuMain.AddOption(C.FSTR.Set("Continue last game"));
#if !UNITY_WEBGL
            mMenuMain.AddOption(C.FSTR.Set("Quit"));
#endif
        }

        /// <summary>
        /// Enter the scene
        /// </summary>
        /// <param name="parameters">Scene parameters</param>
        public override void Enter(object parameters)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            mResultSet = new ResultSet();

            RB.MusicPlay(game.assets.musicMainMenu);
            RB.MusicVolumeSet(1.0f);
        }

        /// <summary>
        /// Exit scene
        /// </summary>
        public override void Exit()
        {
        }

        /// <summary>
        /// Update the scene
        /// </summary>
        /// <returns>True if update consumed</returns>
        public override bool Update()
        {
            HandleMenuKeys(mResultSet);
            CheckCheat(mResultSet);

            for (int i = 0; i < mResultSet.Count; i++)
            {
                HandleResult(mResultSet, mResultSet[i]);
            }

            mResultSet.Clear();

            return false;
        }

        /// <summary>
        /// Render the scene
        /// </summary>
        public override void Render()
        {
            RB.DrawSprite(S.TITLE_BACKGROUND, Vector2i.zero);

            int yOffset = Mathf.RoundToInt(Mathf.Sin(RB.Ticks / 16.0f) * 2);
            RB.TintColorSet(Color.black);
            RB.DrawSprite(S.TITLE_TEXT, new Vector2i(170, 15 + yOffset));
            RB.TintColorSet(Color.white);
            RB.DrawSprite(S.TITLE_TEXT, new Vector2i(170, 12 + yOffset));
            mMenuMain.Render();
        }

        /// <summary>
        /// Close message box handler
        /// </summary>
        public void CloseMessageBoxHandler()
        {
            var game = (RetroDungeoneerGame)RB.Game;
            game.CloseMessageBox();
        }

        private void CheckCheat(ResultSet result)
        {
            mCheatString += RB.InputString().ToLowerInvariant();
            if (mCheatString.Length > 20)
            {
                mCheatString = mCheatString.Substring(mCheatString.Length - 10);
            }

            if (mCheatString.Contains("test"))
            {
                mCheatString = string.Empty;
                result.AddCheat();
            }
        }

        private void HandleMenuKeys(ResultSet resultSet)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }

            // Check keyboard input
            for (char i = 'a'; i < 'a' + mMenuMain.optionCount; i++)
            {
                if (RB.KeyPressed((KeyCode)i))
                {
                    int index = (int)(i - 'a');
                    resultSet.AddMenuIndex(index);
                    RB.SoundPlay(game.assets.soundSelectOption, 1, RandomUtils.RandomPitch(0.1f));
                }
            }

            // Check mouse input
            if (RB.ButtonReleased(RB.BTN_POINTER_A))
            {
                int index = mMenuMain.pointerIndex;
                if (index >= 0)
                {
                    resultSet.AddMenuIndex(index);
                    RB.SoundPlay(game.assets.soundSelectOption, 1, RandomUtils.RandomPitch(0.1f));
                }
            }
        }

        private void HandleResult(ResultSet resultSet, Result result)
        {
            switch (result.type)
            {
                case Result.Type.Exit:
#if !UNITY_WEBGL
                    ShowQuitDialog();
#endif
                    break;

                case Result.Type.MenuIndex:
                    {
                        var index = result.int1;

                        if (index == 0)
                        {
                            // New game
                            SceneGame.EnterParameters parameters;
                            parameters.savedFilename = null;
                            parameters.testMap = false;

                            var game = (RetroDungeoneerGame)RB.Game;
                            game.ChangeScene(SceneEnum.GAME, parameters);
                        }
                        else if (index == 1)
                        {
                            // Load game
                            string fullFilename = Application.persistentDataPath + "/" + C.SAVE_FOLDER + "/" + C.SAVE_FILENAME;

                            if (System.IO.File.Exists(fullFilename))
                            {
                                SceneGame.EnterParameters parameters;
                                parameters.savedFilename = C.SAVE_FILENAME;
                                parameters.testMap = false;

                                var game = (RetroDungeoneerGame)RB.Game;
                                game.ChangeScene(SceneEnum.GAME, parameters);
                            }
                            else
                            {
                                var game = (RetroDungeoneerGame)RB.Game;

                                var options = new List<SceneMessage.MessageBoxOption>();
                                options.Add(new SceneMessage.MessageBoxOption(C.FSTR.Set("Ok"), CloseMessageBoxHandler));

                                game.ShowMessageBox(C.FSTR.Set("Continue Game"), C.FSTR2.Set("No saved game to load"), options);
                            }
                        }
#if !UNITY_WEBGL
                        else if (index == 2)
                        {
                            ShowQuitDialog();
                        }
#endif
                    }

                    break;

                case Result.Type.Cheat:
                    {
                        // New test game
                        SceneGame.EnterParameters parameters;
                        parameters.savedFilename = null;
                        parameters.testMap = true;

                        var game = (RetroDungeoneerGame)RB.Game;
                        game.ChangeScene(SceneEnum.GAME, parameters);

                        RB.SoundPlay(game.assets.soundCheat, 1.0f);
                    }

                    break;
            }
        }

        private void ShowQuitDialog()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var options = new List<SceneMessage.MessageBoxOption>();
            options.Add(new SceneMessage.MessageBoxOption(C.FSTR.Set("Yes"), QuitHandler));
            options.Add(new SceneMessage.MessageBoxOption(C.FSTR.Set("No"), CloseMessageBoxHandler));

            game.ShowMessageBox(C.FSTR.Set("Quit"), C.FSTR2.Set("Quit game and return to desktop?"), options);
        }

        private void QuitHandler()
        {
            Application.Quit();
        }
    }
}
