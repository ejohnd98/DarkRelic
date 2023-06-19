namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The main game scene
    /// </summary>
    public class SceneGame : Scene
    {
        private static readonly GameCamera mCamera = new GameCamera();

        private readonly Menu mMenuInventory;
        private readonly Menu mMenuLevelUp;
        private readonly Menu mMenuCharacterScreen;
        private readonly Menu mMenuHelp;

        private readonly Item.UseParams mUseParams = new Item.UseParams(); // Used to communicate extra parameters to item use functions

        private readonly KeyCode[] PlayerKeyCodes = new KeyCode[]
        {
            KeyCode.W,
            KeyCode.A,
            KeyCode.S,
            KeyCode.D,
            KeyCode.Q,
            KeyCode.E,
            KeyCode.Z,
            KeyCode.C,
            KeyCode.X,
            KeyCode.I,
            KeyCode.O,
            KeyCode.P,
            KeyCode.KeypadEnter,
            KeyCode.Return,
            KeyCode.Keypad0,
            KeyCode.Keypad1,
            KeyCode.Keypad2,
            KeyCode.Keypad3,
            KeyCode.Keypad5,
            KeyCode.Keypad6,
            KeyCode.Keypad7,
            KeyCode.Keypad8,
            KeyCode.Keypad9
        };

        private bool mGameReady = false;
        private bool mSaveOnExit = false;

        private bool mFOVRecompute = true;
        private bool mFloodRecompute = true;

        private GameMap mGameMap;
        private FloodMap mFloodMap;

        private EntityID mPlayer;
        private EntityID mTargetingItem;
        private ulong mLastMoveTimestamp;

        private int mKeyRepeatStage = 0;
        private bool mBlockMoveUntilKeyUp = false;

        private ResultSet mResultSet;
        private Console mConsole;

        private GameState mGameState;
        private GameState mPreviousGameState = GameState.INVALID_STATE;

        private bool mInputLocked = false;

        private AudioAsset mCurrentMusic;

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneGame()
        {
            mMenuInventory = new Menu(C.FSTR.Set("Inventory"));

            mMenuHelp = new Menu(C.FSTR.Set("Help"));
            C.FSTR.Clear();
            C.FSTR.Append("@CCCCCCMovement Keys:@-\n===================\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 2)).Append(" - @FFFF90W@- or @FFFF90Numpad 8@-\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 4)).Append(" - @FFFF90S@- or @FFFF90Numpad 2@-\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 1)).Append(" - @FFFF90A@- or @FFFF90Numpad 4@-\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 3)).Append(" - @FFFF90D@- or @FFFF90Numpad 6@-\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 5)).Append(" - @FFFF90Q@- or @FFFF90Numpad 7@-\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 6)).Append(" - @FFFF90E@- or @FFFF90Numpad 9@-\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 8)).Append(" - @FFFF90Z@- or @FFFF90Numpad 1@-\n");
            C.FSTR.Append("@FFFFFF").Append((char)('~' + 7)).Append(" - @FFFF90C@- or @FFFF90Numpad 3@-\n");
            C.FSTR.Append("@FFFFFFWait@- - @FFFF90X@- or @FFFF90Numpad 5@-\n");
            C.FSTR.Append("\n@CCCCCCAttack Keys:@-\n===================\n");
            C.FSTR.Append("@FFFFFFMelee@- - Bump into enemy\n");
            C.FSTR.Append("@FFFFFFShoot Bow@- - @FFFF90F@- or @FFFF90Numpad /@-\n(double-tap to shoot nearest)\n");
            C.FSTR.Append("\n@CCCCCCInteraction Keys:@-\n===================\n");
            C.FSTR.Append("@FFFFFFPickup@- - @FFFF90G@-\n");
            C.FSTR.Append("@FFFFFFTake Stairs@- - @FFFF90Enter@-\n");
            C.FSTR.Append("\n@CCCCCCMenu Keys:@-\n===================\n");
            C.FSTR.Append("@FFFFFFHelp@- - @FFFF90H@-\n");
            C.FSTR.Append("@FFFFFFUse Item@- - @FFFF90I@-\n");
            C.FSTR.Append("@FFFFFFDrop Item@- - @FFFF90O@-\n");
            C.FSTR.Append("@FFFFFFCharacter Screen@- - @FFFF90P@-\n");
            C.FSTR.Append("@FFFFFFClose Menu@- - @FFFF90Esc@-");

            mMenuHelp.SetSummary(C.FSTR);

            mMenuLevelUp = new Menu(C.FSTR.Set("Level Up"));
            mMenuLevelUp.SetSummary(C.FSTR.Set("Level up! Choose a stat to raise:"));

            mMenuCharacterScreen = new Menu(C.FSTR.Set("Character Information"));
        }

        /// <summary>
        /// Get GameMap instance
        /// </summary>
        public GameMap map
        {
            get
            {
                return mGameMap;
            }
        }

        /// <summary>
        /// Get Flood Map
        /// </summary>
        public FloodMap floodMap
        {
            get
            {
                return mFloodMap;
            }
        }

        /// <summary>
        /// Game camera
        /// </summary>
        public GameCamera camera
        {
            get
            {
                return mCamera;
            }
        }

        /// <summary>
        /// Player
        /// </summary>
        public EntityID player
        {
            get
            {
                return mPlayer;
            }
        }

        /// <summary>
        /// Get tile position at current pointer position. This takes into account camera position.
        /// </summary>
        /// <returns>Tile position</returns>
        public static Vector2i GetMouseTilePos()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (!RB.PointerPosValid())
            {
                return new Vector2i(-1, -1);
            }

            var mousePos = RB.PointerPos() + mCamera.GetPos();

            mousePos.x -= (RB.DisplaySize.width / 2) - (game.assets.spriteSheet.grid.cellSize.width / 2);
            mousePos.y -= (RB.DisplaySize.height / 2) - (game.assets.spriteSheet.grid.cellSize.height / 2);

            var tilePos = new Vector2i(mousePos.x / game.assets.spriteSheet.grid.cellSize.width, mousePos.y / game.assets.spriteSheet.grid.cellSize.height);

            if (tilePos.x < 0 || tilePos.y < 0 || tilePos.x >= game.map.size.width || tilePos.y >= game.map.size.height)
            {
                return new Vector2i(-1, -1);
            }

            return tilePos;
        }

        /// <summary>
        /// Enter the scene
        /// </summary>
        /// <param name="parameters">Scene parameters</param>
        public override void Enter(object parameters)
        {
            mGameReady = false;

            var enterParams = (EnterParameters)parameters;

            mResultSet = new ResultSet();
            mConsole = new Console(new Vector2i((int)(RB.DisplaySize.width * 0.66f), RB.DisplaySize.height / 3));

            if (enterParams.testMap)
            {
                InitializeNewGame.SetupTestGameVariables(ref mPlayer, ref mGameMap);
                mGameState = GameState.PLAYER_TURN;
                mGameReady = true;
                mSaveOnExit = true;
            }
            else if (enterParams.savedFilename == null)
            {
                InitializeNewGame.SetupGameVariables(ref mPlayer, ref mGameMap);
                mGameState = GameState.PLAYER_TURN;
                mGameReady = true;
                mSaveOnExit = true;
            }
            else
            {
                var ret = DataLoaders.LoadGame(enterParams.savedFilename, ref mPlayer, ref mGameMap, ref mGameState, mConsole);
                if (ret == false)
                {
                    return;
                }
                else
                {
                    mGameReady = true;
                    mSaveOnExit = true;
                }
            }

            mFOVRecompute = true;
            mFloodMap = new FloodMap(mGameMap.size);

            mCamera.SetPos(mPlayer);
            ChangeState(GameState.PLAYER_TURN);

            mConsole.Log(C.FSTR.Set("Welcome! Press H for help."));

            RB.MusicPlay(mGameMap.music);
            mCurrentMusic = mGameMap.music;

            RB.MusicVolumeSet(0.75f);

            // If some key is down when entering scene then lock input until all keys are up. This prevents
            // the player accidentally moving
            if (RB.AnyKeyDown())
            {
                mInputLocked = true;
            }
        }

        /// <summary>
        /// Exit scene
        /// </summary>
        public override void Exit()
        {
            if (mGameReady && mSaveOnExit)
            {
                DataLoaders.SaveGame(C.SAVE_FILENAME, mPlayer, mGameMap, mConsole, mGameState);
            }

            SoundBank.Instance.Clear();
            EffectManager.Instance.Clear();
        }

        /// <summary>
        /// Update scene
        /// </summary>
        /// <returns>True if update consumed</returns>
        public override bool Update()
        {
            // If game is not ready for some reason then bail out to Main Menu
            if (!mGameReady)
            {
                var game = (RetroDungeoneerGame)RB.Game;
                game.ChangeScene(SceneEnum.MAIN_MENU);
                return false;
            }

            if (mFOVRecompute)
            {
                mGameMap.RecomputeFov(mPlayer, C.FOV_RADIUS);
                mFOVRecompute = false;
            }

            if (mFloodRecompute)
            {
                mFloodMap.Refresh(mPlayer.e.pos, (C.SCREEN_WIDTH / 2) + 2);
                mFloodRecompute = false;
            }

            if (mGameState == GameState.ENEMY_TURN)
            {
                DoEnemyTurn();
                ChangeState(GameState.PLAYER_TURN);
            }
            else
            {
                if (!mInputLocked && DoPlayerTurn())
                {
                    if (mGameState != GameState.LEVEL_UP)
                    {
                        ChangeState(GameState.ENEMY_TURN);
                    }
                }
            }

            mCamera.Follow(mPlayer);

            RenderFunctions.Update();

            SoundBank.Instance.Process();
            EffectManager.Instance.Update(mResultSet);
            HandleEffectResults();

            if (mInputLocked && !RB.AnyKeyDown())
            {
                mInputLocked = false;
            }

            return false;
        }

        /// <summary>
        /// Do players turn, return true is player moved
        /// </summary>
        /// <returns>True if moved</returns>
        public bool DoPlayerTurn()
        {
            bool playerTookTurn = false;

            mResultSet.Clear();
            HandleKeys(mResultSet);
            HandleMouse(mResultSet);

            for (int i = 0; i < mResultSet.Count; i++)
            {
                bool tookTurn;
                HandleResult(mResultSet, mResultSet[i], out tookTurn);

                if (tookTurn)
                {
                    playerTookTurn = true;
                }
            }

            mResultSet.Clear();

            return playerTookTurn;
        }

        /// <summary>
        /// Perform the enemy turns
        /// </summary>
        public void DoEnemyTurn()
        {
            for (int i = 0; i < EntityStore.entities.Count; i++)
            {
                var entity = EntityStore.entities[i];
                if (entity.e.ai != null)
                {
                    mResultSet.Clear();
                    entity.e.ai.TakeTurn(mResultSet, mPlayer);

                    for (int j = 0; j < mResultSet.Count; j++)
                    {
                        bool tookTurn;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                        HandleResult(mResultSet, mResultSet[j], out tookTurn);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                    }
                }
            }

            mResultSet.Clear();

            mFloodRecompute = true;
        }

        /// <summary>
        /// Render the scene
        /// </summary>
        public override void Render()
        {
            if (!mGameReady)
            {
                return;
            }

            mCamera.Apply();

            Menu menu = null;

            if (mGameState == GameState.SHOW_INVENTORY || mGameState == GameState.DROP_INVENTORY)
            {
                menu = mMenuInventory;
            }
            else if (mGameState == GameState.SHOW_HELP)
            {
                menu = mMenuHelp;
            }
            else if (mGameState == GameState.LEVEL_UP)
            {
                menu = mMenuLevelUp;
            }
            else if (mGameState == GameState.CHARACTER_SCREEN)
            {
                menu = mMenuCharacterScreen;
            }

            RenderFunctions.RenderAll(mGameState, mPlayer, menu, mConsole);

            //// mFloodMap.DebugRender();

            RB.CameraReset();
        }

        private void HandleEffectResults()
        {
            for (int i = 0; i < mResultSet.Count; i++)
            {
                bool tookTurn;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                HandleResult(mResultSet, mResultSet[i], out tookTurn);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            }

            mResultSet.Clear();
        }

        private void HandleResult(ResultSet resultSet, Result result, out bool tookTurn)
        {
            tookTurn = false;
            var game = (RetroDungeoneerGame)RB.Game;

            switch (result.type)
            {
                case Result.Type.Move:
                    {
                        Vector2i delta = result.veci1;
                        if ((delta.x != 0 || delta.y != 0) && !mGameMap.IsBlocked(mPlayer.e.pos + delta))
                        {
                            var destPos = mPlayer.e.pos + delta;
                            var targetEntity = EntityFunctions.GetBlockingEntityAtPos(destPos);

                            if (!targetEntity.isEmpty)
                            {
                                mPlayer.e.fighter.Attack(mResultSet, targetEntity);
                            }
                            else
                            {
                                var entities = EntityFunctions.GetEntitiesAtPos(mPlayer.e.pos);
                                bool stuckInWeb = false;
                                for (int i = 0; i < entities.Count; i++)
                                {
                                    if (entities[i].e.groundTrigger != null && entities[i].e.groundTrigger.type == GroundTrigger.GroundTriggerType.Web)
                                    {
                                        stuckInWeb = true;
                                        break;
                                    }
                                }

                                // Attempt to free from web
                                if (stuckInWeb)
                                {
                                    if (Random.Range(0, 6) == 0)
                                    {
                                        stuckInWeb = false;
                                        resultSet.AddMessage(C.FSTR.Set("You manage to free yourself from the web."));
                                    }
                                }

                                if (!stuckInWeb)
                                {
                                    mPlayer.e.Move(delta);
                                    mFOVRecompute = true;

                                    RB.SoundPlay(game.assets.soundFootStep, 0.66f, RandomUtils.RandomPitch(0.1f));

                                    var entitesAtDest = EntityFunctions.GetEntitiesAtPos(mPlayer.e.pos);
                                    for (int i = 0; i < entitesAtDest.Count; i++)
                                    {
                                        var trigger = entitesAtDest[i].e.groundTrigger;
                                        if (trigger != null)
                                        {
                                            trigger.Trigger(resultSet);
                                        }
                                    }
                                }
                                else
                                {
                                    resultSet.AddMessage(C.FSTR.Set("You fail to free yourself from the web, keep trying."));
                                    RB.SoundPlay(game.assets.soundWeb, 0.5f, RandomUtils.RandomPitch(0.1f));
                                }
                            }

                            mFloodRecompute = true;

                            tookTurn = true;
                        }
                        else if (delta.x == 0 && delta.y == 0)
                        {
                            // Do nothing, wait
                            tookTurn = true;
                        }
                    }

                    break;

                case Result.Type.Exit:
                    if (mGameState == GameState.SHOW_INVENTORY ||
                        mGameState == GameState.DROP_INVENTORY ||
                        mGameState == GameState.SHOW_HELP ||
                        mGameState == GameState.CHARACTER_SCREEN)
                    {
                        RB.SoundPlay(game.assets.soundMenuClose, 1, RandomUtils.RandomPitch(0.1f));
                        ChangeState(mPreviousGameState);
                    }
                    else if (mGameState == GameState.TARGETING)
                    {
                        resultSet.AddTargetingCancelled();
                    }
                    else
                    {
                        // Exit back to main menu
                        game.ChangeScene(SceneEnum.MAIN_MENU);
                    }

                    break;

                case Result.Type.Message:
                    mConsole.Log(result.str1);
                    break;

                case Result.Type.Dead:
                    if (result.entity1 == mPlayer)
                    {
                        DeathFunctions.KillPlayer(mResultSet, result.entity1);
                        ChangeState(GameState.PLAYER_DEAD);
                    }
                    else
                    {
                        DeathFunctions.KillMonster(mResultSet, result.entity1);
                    }

                    break;

                case Result.Type.Pickup:
                    {
                        var receiver = result.entity1;
                        if (receiver.e.inventory == null)
                        {
                            break;
                        }

                        var entityList = EntityFunctions.GetEntitiesAtPos(receiver.e.pos);
                        if (entityList != null)
                        {
                            for (int j = 0; j < entityList.Count; j++)
                            {
                                var e = entityList[j].e;

                                if (e.item != null)
                                {
                                    receiver.e.inventory.AddItem(resultSet, e.id);
                                }
                            }
                        }
                    }

                    break;

                case Result.Type.ItemAdded:
                    result.entity1.e.renderOrder = RenderFunctions.RenderOrder.HIDDEN;
                    result.entity1.e.pos = new Vector2i(-1, -1);

                    RB.SoundPlay(game.assets.soundInventory, 1, RandomUtils.RandomPitch(0.1f));

                    break;

                case Result.Type.ShowInventory:
                    if (mGameState != GameState.SHOW_INVENTORY)
                    {
                        ChangeState(GameState.SHOW_INVENTORY);
                    }
                    else
                    {
                        ChangeState(mPreviousGameState);
                    }

                    break;

                case Result.Type.InventoryIndex:
                    {
                        var entity = result.entity1;
                        var item = entity.e.inventory.items[result.int1];

                        mUseParams.Clear();
                        mUseParams.map = mGameMap;

                        if (mGameState == GameState.SHOW_INVENTORY)
                        {
                            entity.e.inventory.UseItem(resultSet, item, mUseParams);
                        }
                        else if (mGameState == GameState.DROP_INVENTORY)
                        {
                            entity.e.inventory.DropItem(resultSet, item);
                        }
                    }

                    break;

                case Result.Type.Consumed:
                    {
                        var entity = result.entity1;
                        var item = result.entity2;

                        entity.e.inventory.RemoveItem(resultSet, item);

                        tookTurn = true;
                    }

                    break;

                case Result.Type.DropInventory:
                    if (mGameState != GameState.DROP_INVENTORY)
                    {
                        ChangeState(GameState.DROP_INVENTORY);
                    }
                    else
                    {
                        ChangeState(mPreviousGameState);
                    }

                    break;

                case Result.Type.ItemDropped:
                    result.entity1.e.renderOrder = RenderFunctions.RenderOrder.ITEM;
                    tookTurn = true;

                    RB.SoundPlay(game.assets.soundInventory, 1, RandomUtils.RandomPitch(0.1f));
                    break;

                case Result.Type.Equip:
                    {
                        var equipper = result.entity1;
                        var item = result.entity2;

                        if (equipper.e == null || equipper.e.equipment == null || item.e == null || item.e.equippable == null)
                        {
                            break;
                        }

                        equipper.e.equipment.ToggleEquip(resultSet, item);
                    }

                    break;

                case Result.Type.Equipped:
                    {
                        var equipper = result.entity1;
                        var item = result.entity2;

                        if (equipper.e == null || equipper.e.equipment == null || item.e == null || item.e.equippable == null)
                        {
                            break;
                        }

                        RB.SoundPlay(game.assets.soundInventory, 1, RandomUtils.RandomPitch(0.1f));
                    }

                    break;

                case Result.Type.Dequipped:
                    {
                        var equipper = result.entity1;
                        var item = result.entity2;

                        if (equipper.e == null || equipper.e.equipment == null || item.e == null || item.e.equippable == null)
                        {
                            break;
                        }

                        RB.SoundPlay(game.assets.soundInventory, 1, RandomUtils.RandomPitch(0.1f));
                    }

                    break;

                case Result.Type.Targeting:
                    // Set previous state to players turn to go back to players turn if targeting is cancelled
                    mPreviousGameState = GameState.PLAYER_TURN;
                    ChangeState(GameState.TARGETING);
                    mTargetingItem = result.entity1;

                    mConsole.Log(mTargetingItem.e.item.targetingMessage);
                    break;

                case Result.Type.LeftClick:
                    {
                        if (mGameState == GameState.TARGETING && !mTargetingItem.isEmpty)
                        {
                            var targetingPos = result.veci1;

                            if (mTargetingItem.e.equippable != null && mTargetingItem.e.equippable.slot == EquipmentSlot.Ranged)
                            {
                                var arrow = mPlayer.e.inventory.GetArrow();
                                if (SkillFunctions.ShootBow(resultSet, mPlayer, arrow, mTargetingItem.e.item.int2, mTargetingItem.e.item.int1, targetingPos))
                                {
                                    mPlayer.e.inventory.RemoveItem(resultSet, arrow);
                                    EntityStore.DestroyEntity(arrow);
                                }

                                ChangeState(GameState.ENEMY_TURN);
                            }
                            else
                            {
                                mUseParams.Clear();
                                mUseParams.map = mGameMap;
                                mUseParams.veci1 = targetingPos;
                                mUseParams.bool1 = true;

                                mPlayer.e.inventory.UseItem(resultSet, mTargetingItem, mUseParams);
                            }
                        }
                    }

                    break;

                case Result.Type.RightClick:
                    if (mGameState == GameState.TARGETING)
                    {
                        resultSet.AddTargetingCancelled();
                    }

                    break;

                case Result.Type.TargetingCancelled:
                    ChangeState(GameState.PLAYER_TURN);
                    mTargetingItem = EntityID.empty;
                    mConsole.Log(C.FSTR.Set("Targeting cancelled."));
                    break;

                case Result.Type.ShowHelp:
                    if (mGameState != GameState.SHOW_HELP)
                    {
                        ChangeState(GameState.SHOW_HELP);
                    }
                    else
                    {
                        ChangeState(mPreviousGameState);
                    }

                    break;

                case Result.Type.TakeStairs:
                    {
                        int i;
                        bool foundStairs = false;

                        var entityList = EntityFunctions.GetEntitiesAtPos(mPlayer.e.pos);
                        if (entityList != null)
                        {
                            for (i = 0; i < entityList.Count; i++)
                            {
                                var e = entityList[i].e;
                                if (e.stairs != null && e.stairs.type != Stairs.StairType.WELL_CLOSED)
                                {
                                    EffectManager.Instance.Clear();

                                    mGameMap.NextFloor(mPlayer);
                                    mFOVRecompute = true;
                                    mFloodRecompute = true;

                                    // Snap the camera to player
                                    mCamera.SetPos(mPlayer);

                                    foundStairs = true;

                                    if (e.stairs.type == Stairs.StairType.STAIRS)
                                    {
                                        RB.SoundPlay(game.assets.soundStairs);

                                        mPlayer.e.fighter.Heal(resultSet, mPlayer.e.fighter.maxHp / 2);
                                        mConsole.Log(C.FSTR.Set("You take a moment to rest, and recover your strength."));
                                    }
                                    else if (e.stairs.type == Stairs.StairType.PORTAL)
                                    {
                                        RB.SoundPlay(game.assets.soundPortal);
                                        mPlayer.e.fighter.Heal(resultSet, mPlayer.e.fighter.maxHp / 2);
                                        mConsole.Log(C.FSTR.Set("You step into the mysterious portal..."));
                                    }
                                    else if (e.stairs.type == Stairs.StairType.WELL)
                                    {
                                        RB.SoundPlay(game.assets.soundJump);
                                        mConsole.Log(C.FSTR.Set("You jump down the well into the darkness below..."));
                                    }

                                    if (mCurrentMusic != mGameMap.music)
                                    {
                                        RB.MusicPlay(mGameMap.music);
                                        mCurrentMusic = mGameMap.music;
                                    }

                                    // Save the game after descending
                                    DataLoaders.SaveGame(C.SAVE_FILENAME, mPlayer, mGameMap, mConsole, mGameState);

                                    break;
                                }
                                else if (e.stairs != null && e.stairs.type == Stairs.StairType.WELL_CLOSED)
                                {
                                    foundStairs = true;

                                    mConsole.Log(C.FSTR.Set("You look down the well. That won't be necessary, not this time..."));
                                }
                            }
                        }

                        if (!foundStairs)
                        {
                            mConsole.Log(C.FSTR.Set("There are no stairs here."));
                        }
                    }

                    break;

                case Result.Type.Xp:
                    int xp = result.int1;
                    var leveledUp = mPlayer.e.level.AddXp(xp);
                    mConsole.Log(C.FSTR.Set("You gain ").Append(xp).Append(" experience points."));

                    if (leveledUp)
                    {
                        mConsole.Log(C.FSTR.Set("Your battle skills grow stronger! You reached level ").Append(mPlayer.e.level.currentLevel).Append("!"));
                        mPreviousGameState = mGameState;
                        ChangeState(GameState.LEVEL_UP);

                        mMenuLevelUp.ClearOptions();
                        mMenuLevelUp.AddOption(C.FSTR.Set("Constitution (+20 HP, from ").Append(mPlayer.e.fighter.maxHp).Append(")"));
                        mMenuLevelUp.AddOption(C.FSTR.Set("Strength (+1 attack, from ").Append(mPlayer.e.fighter.power).Append(")"));
                        mMenuLevelUp.AddOption(C.FSTR.Set("Agility (+1 defense, from ").Append(mPlayer.e.fighter.defense).Append(")"));

                        RB.SoundPlay(game.assets.soundLevelUp);
                    }

                    break;

                case Result.Type.LevelUp:
                    var levelUp = (LevelUp)result.int1;

                    switch (levelUp)
                    {
                        case LevelUp.Hp:
                            mPlayer.e.fighter.baseMaxHp += 20;
                            mPlayer.e.fighter.hp += 20;
                            break;

                        case LevelUp.Str:
                            mPlayer.e.fighter.basePower += 1;
                            break;

                        case LevelUp.Def:
                            mPlayer.e.fighter.baseDefense += 1;
                            break;
                    }

                    mGameState = mPreviousGameState;

                    break;

                case Result.Type.ShowCharacterScreen:
                    mPreviousGameState = mGameState;
                    ChangeState(GameState.CHARACTER_SCREEN);

                    C.FSTR.Clear();
                    C.FSTR.Append("@FFFFFFLevel: ").Append(C.STR_COLOR_NAME).Append(mPlayer.e.level.currentLevel).Append("\n");
                    C.FSTR.Append("@FFFFFFExperience: ").Append(C.STR_COLOR_NAME).Append(mPlayer.e.level.currentXp).Append("\n");
                    C.FSTR.Append("@FFFFFFExperience to Level: ").Append(C.STR_COLOR_NAME).Append(mPlayer.e.level.ExperienceToNextLevel()).Append("\n");
                    C.FSTR.Append("@FFFFFFMaximum HP: ").Append(C.STR_COLOR_NAME).Append(mPlayer.e.fighter.maxHp).Append("\n");
                    C.FSTR.Append("@FFFFFFAttack: ").Append(C.STR_COLOR_NAME).Append(mPlayer.e.fighter.power).Append("\n");
                    C.FSTR.Append("@FFFFFFDefense: ").Append(C.STR_COLOR_NAME).Append(mPlayer.e.fighter.defense);
                    mMenuCharacterScreen.SetSummary(C.FSTR);

                    break;

                case Result.Type.BossDefeated:
                    var boss = result.entity1;
                    map.SpawnExitPortal(boss.e.pos);
                    break;

                case Result.Type.GameWon:
                    {
                        var options = new List<SceneMessage.MessageBoxOption>
                        {
                            new SceneMessage.MessageBoxOption(C.FSTR.Set("Return to Main Menu"), CloseGameWonMessageBox)
                        };

                        C.FSTR.Set(C.STR_COLOR_DIALOG);
                        C.FSTR.Append("You make it out of the forest clearing with adrenaline still pumping through your veins.");
                        C.FSTR.Append("\n\n");
                        C.FSTR.Append("Your adventure flashes through your mind and you realize you're not the same person you were before. ");
                        C.FSTR.Append("The concerns you once had seem insignificant now.");
                        C.FSTR.Append("\n\n");
                        C.FSTR.Append("Moments later you calm down, and you find yourself wondering what lies at the bottom of other old wells...");
                        C.FSTR.Append("\n\n");
                        C.FSTR.Append(C.STR_COLOR_NAME);
                        C.FSTR.Append("You've won!\nCongratulations!");

                        game.ShowMessageBox(
                            C.FSTR2.Set("A New Life"),
                            C.FSTR,
                            options);
                        break;
                    }

                case Result.Type.FellDownWell:
                    {
                        var options = new List<SceneMessage.MessageBoxOption>
                        {
                            new SceneMessage.MessageBoxOption(C.FSTR.Set("Continue"), CloseMessageBox)
                        };

                        C.FSTR.Set(C.STR_COLOR_DIALOG);
                        C.FSTR.Append("This is not what you expected. It seems that not only has the bottom of this well ran dry, it has collapsed into some old dungeon! ");
                        C.FSTR.Append("There must be some way out of this place, there has to be.");
                        C.FSTR.Append("\n\n");
                        C.FSTR.Append("A sound of skittering creatures can be heard coming from the nearby rooms. ");
                        C.FSTR.Append("You spot a rusty dagger lying beside you.");
                        C.FSTR.Append("\n\n");
                        C.FSTR.Append("Finding an exit may prove... difficult.");

                        game.ShowMessageBox(
                            C.FSTR2.Set("Bottom of the Well"),
                            C.FSTR,
                            options);
                        break;
                    }

                case Result.Type.BossEncounter:
                    {
                        var options = new List<SceneMessage.MessageBoxOption>
                        {
                            new SceneMessage.MessageBoxOption(C.FSTR.Set("Continue"), CloseMessageBox)
                        };

                        C.FSTR.Set(C.STR_COLOR_DIALOG);
                        C.FSTR.Append("Before you stands a towering golem. It lurches forward, slowly at first, as if it has not moved in ages.");
                        C.FSTR.Append("\n\n");
                        C.FSTR.Append(C.STR_COLOR_NAME);
                        C.FSTR.Append("\"Who awakens the Gatekeeper?\"").Append(C.STR_COLOR_DIALOG).Append(" it belows.");
                        C.FSTR.Append("\n\n");
                        C.FSTR.Append("A Gatekeeper is just what you were looking for. This is your chance to escape the dungeon! You steady yourself, this will not be an easy fight.");

                        game.ShowMessageBox(
                            C.FSTR2.Set("The Gatekeeper"),
                            C.FSTR,
                            options);
                        break;
                    }
            }
        }

        private void CloseMessageBox()
        {
            var game = (RetroDungeoneerGame)RB.Game;
            game.CloseMessageBox();
            mInputLocked = true;
        }

        private void CloseGameWonMessageBox()
        {
            var game = (RetroDungeoneerGame)RB.Game;
            game.CloseMessageBox();
            mInputLocked = true;

            mSaveOnExit = false;

            // Exit back to main menu
            game.ChangeScene(SceneEnum.MAIN_MENU);
        }

        private void HandleKeys(ResultSet resultSet)
        {
            if (mGameState == GameState.PLAYER_TURN)
            {
                if (!EffectManager.Instance.BlockingEffectInProgress())
                {
                    HandlePlayerTurnKeys(resultSet);
                }
            }
            else if (mGameState == GameState.PLAYER_DEAD)
            {
                HandlePlayerDeadKeys(resultSet);
            }
            else if (mGameState == GameState.TARGETING)
            {
                mBlockMoveUntilKeyUp = true;
                HandleTargetingKeys(resultSet);
            }
            else if (mGameState == GameState.SHOW_INVENTORY || mGameState == GameState.DROP_INVENTORY)
            {
                mBlockMoveUntilKeyUp = true;
                HandleInventoryKeys(resultSet);
            }
            else if (mGameState == GameState.SHOW_HELP)
            {
                mBlockMoveUntilKeyUp = true;
                HandleMenuKeys(resultSet);
            }
            else if (mGameState == GameState.LEVEL_UP)
            {
                mBlockMoveUntilKeyUp = true;
                HandleLevelUpKeys(resultSet);
            }
            else if (mGameState == GameState.CHARACTER_SCREEN)
            {
                mBlockMoveUntilKeyUp = true;
                HandleMenuKeys(resultSet);
            }
        }

        private void HandlePlayerTurnKeys(ResultSet resultSet)
        {
            // Check if we should block all player input until all keys are up
            // This is useful to block player input when exiting menus
            if (mBlockMoveUntilKeyUp)
            {
                if (!AnyPlayerKeyDown())
                {
                    mBlockMoveUntilKeyUp = false;
                }
                else
                {
                    return;
                }
            }

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }

            if (RB.KeyPressed(KeyCode.I))
            {
                resultSet.AddShowInventory();
            }

            if (RB.KeyPressed(KeyCode.O))
            {
                resultSet.AddDropInventory();
            }

            if (RB.KeyPressed(KeyCode.P))
            {
                resultSet.AddShowCharacterScreen();
            }

            if (RB.KeyPressed(KeyCode.Return) || RB.KeyPressed(KeyCode.KeypadEnter))
            {
                resultSet.AddTakeStairs();
            }

            if (RB.KeyPressed(KeyCode.H))
            {
                resultSet.AddShowHelp();
            }

            Vector2i delta = Vector2i.zero;

            if (RB.KeyDown(KeyCode.W) || RB.KeyDown(KeyCode.Keypad8))
            {
                delta.y--;
            }
            else if (RB.KeyDown(KeyCode.S) || RB.KeyDown(KeyCode.Keypad2))
            {
                delta.y++;
            }
            else if (RB.KeyDown(KeyCode.A) || RB.KeyDown(KeyCode.Keypad4))
            {
                delta.x--;
            }
            else if (RB.KeyDown(KeyCode.D) || RB.KeyDown(KeyCode.Keypad6))
            {
                delta.x++;
            }
            else if (RB.KeyDown(KeyCode.Q) || RB.KeyDown(KeyCode.Keypad7))
            {
                delta.x--;
                delta.y--;
            }
            else if (RB.KeyDown(KeyCode.E) || RB.KeyDown(KeyCode.Keypad9))
            {
                delta.x++;
                delta.y--;
            }
            else if (RB.KeyDown(KeyCode.Z) || RB.KeyDown(KeyCode.Keypad1))
            {
                delta.x--;
                delta.y++;
            }
            else if (RB.KeyDown(KeyCode.C) || RB.KeyDown(KeyCode.Keypad3))
            {
                delta.x++;
                delta.y++;
            }

            if (delta.x > 1)
            {
                delta.x = 1;
            }
            else if (delta.x < -1)
            {
                delta.x = -1;
            }

            if (delta.y > 1)
            {
                delta.y = 1;
            }
            else if (delta.y < -1)
            {
                delta.y = -1;
            }

            if (delta.x == 0 && delta.y == 0)
            {
                mKeyRepeatStage = 0;
            }
            else
            {
                var ticksDelta = RB.Ticks - mLastMoveTimestamp;

                if (mKeyRepeatStage == 0 || (mKeyRepeatStage == 1 && ticksDelta > C.KEY_REPEAT_SPEED_STAGE1) || (mKeyRepeatStage > 1 && ticksDelta > C.KEY_REPEAT_SPEED_STAGE2))
                {
                    if (delta.x != 0 || delta.y != 0)
                    {
                        mKeyRepeatStage++;
                        mLastMoveTimestamp = RB.Ticks;
                        resultSet.AddMove(delta);
                    }
                }
            }

            // Wait
            if (RB.KeyPressed(KeyCode.X) || RB.KeyPressed(KeyCode.Keypad5))
            {
                resultSet.AddMove(Vector2i.zero);
            }

            if (RB.KeyPressed(KeyCode.G) || RB.KeyPressed(KeyCode.Keypad0))
            {
                resultSet.AddPickup(mPlayer);
            }

            if (RB.KeyPressed(KeyCode.F))
            {
                var ranged = mPlayer.e.equipment.equipment[(int)EquipmentSlot.Ranged];
                var arrow = mPlayer.e.inventory.GetArrow();
                if (!ranged.isEmpty && !arrow.isEmpty)
                {
                    resultSet.AddTargeting(ranged);
                }
                else
                {
                    if (ranged.isEmpty)
                    {
                        resultSet.AddMessage(C.FSTR.Set("You do not have a ranged weapon equipped."));
                    }
                    else if (arrow.isEmpty)
                    {
                        resultSet.AddMessage(C.FSTR.Set("You do not have an arrow to shoot."));
                    }
                }
            }
        }

        private void HandlePlayerDeadKeys(ResultSet resultSet)
        {
            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }

            if (RB.KeyPressed(KeyCode.P))
            {
                resultSet.AddShowCharacterScreen();
            }
        }

        private void HandleTargetingKeys(ResultSet resultSet)
        {
            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }

            if (RB.KeyPressed(KeyCode.F))
            {
                var ranged = mPlayer.e.equipment.equipment[(int)EquipmentSlot.Ranged];

                if (mGameState == GameState.TARGETING && mTargetingItem == ranged)
                {
                    // Already targeting, do a quick shot by injecting an invalid position which will make
                    // ShootBow() look for nearest enemy
                    resultSet.AddLeftClick(new Vector2i(-1, -1));
                }
            }
        }

        private void HandleInventoryKeys(ResultSet resultSet)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }

            for (char i = 'a'; i <= 'z'; i++)
            {
                if (RB.KeyPressed((KeyCode)i))
                {
                    int index = (int)(i - 'a');
                    if (!mPlayer.isEmpty && mPlayer.e.inventory.items.Length > index && !mPlayer.e.inventory.items[index].isEmpty)
                    {
                        resultSet.AddInventoryIndex(mPlayer, index);
                        RB.SoundPlay(game.assets.soundSelectOption, 1, RandomUtils.RandomPitch(0.1f));
                        return;
                    }
                }
            }

            // Check if pointer clicked on any of the items
            if (RB.ButtonReleased(RB.BTN_POINTER_A))
            {
                int index = mMenuInventory.pointerIndex;
                if (index >= 0 && !mPlayer.isEmpty && mPlayer.e.inventory.items.Length > index && !mPlayer.e.inventory.items[index].isEmpty)
                {
                    resultSet.AddInventoryIndex(mPlayer, index);
                    RB.SoundPlay(game.assets.soundSelectOption, 1, RandomUtils.RandomPitch(0.1f));
                    return;
                }
            }
        }

        private void HandleLevelUpKeys(ResultSet resultSet)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }

            int index = -1;

            for (char i = 'a'; i <= 'c'; i++)
            {
                if (RB.KeyPressed((KeyCode)i))
                {
                    index = (int)(i - 'a');
                    break;
                }
            }

            // Check if pointer clicked on any of the items
            if (RB.ButtonReleased(RB.BTN_POINTER_A))
            {
                if (mMenuInventory.pointerIndex >= 0 && mMenuInventory.pointerIndex <= 2)
                {
                    index = mMenuInventory.pointerIndex;
                }
            }

            LevelUp levelUp = LevelUp.None;
            if (index >= 0 && index <= 2)
            {
                switch (index)
                {
                    case 0:
                        levelUp = LevelUp.Hp;
                        break;

                    case 1:
                        levelUp = LevelUp.Str;
                        break;

                    case 2:
                        levelUp = LevelUp.Def;
                        break;
                }

                resultSet.AddLevelUp(levelUp);

                RB.SoundPlay(game.assets.soundSelectOption, 1, RandomUtils.RandomPitch(0.1f));
            }
        }

        private void HandleMenuKeys(ResultSet resultSet)
        {
            if (RB.ButtonPressed(RB.BTN_SYSTEM))
            {
                resultSet.AddExit();
                return;
            }
        }

        private void HandleMouse(ResultSet resultSet)
        {
            Vector2i tilePos = GetMouseTilePos();
            if (tilePos.x == -1)
            {
                return;
            }

            if (RB.ButtonPressed(RB.BTN_POINTER_A))
            {
                resultSet.AddLeftClick(tilePos);
            }
            else if (RB.ButtonPressed(RB.BTN_POINTER_B))
            {
                resultSet.AddRightClick(tilePos);
            }
        }

        private void ChangeState(GameState newState)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var prevState = mGameState;

            // Only allow turn switch if player is not dead
            if (newState == GameState.ENEMY_TURN || newState == GameState.PLAYER_TURN)
            {
                if (mGameState != GameState.PLAYER_DEAD)
                {
                    mGameState = newState;
                }
            }
            else
            {
                mGameState = newState;
            }

            if (prevState != mGameState)
            {
                switch (mGameState)
                {
                    case GameState.SHOW_INVENTORY:
                    case GameState.SHOW_HELP:
                    case GameState.CHARACTER_SCREEN:
                    case GameState.DROP_INVENTORY:
                        RB.SoundPlay(game.assets.soundMenuOpen, 1, RandomUtils.RandomPitch(0.1f));
                        break;
                }

                mPreviousGameState = prevState;
            }
        }

        private bool AnyPlayerKeyDown()
        {
            for (int i = 0; i < PlayerKeyCodes.Length; i++)
            {
                if (RB.KeyDown(PlayerKeyCodes[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Scene entrance parameters
        /// </summary>
        public struct EnterParameters
        {
            /// <summary>
            /// Saved file name to load
            /// </summary>
            public string savedFilename;

            /// <summary>
            /// Create a test map
            /// </summary>
            public bool testMap;
        }
    }
}
