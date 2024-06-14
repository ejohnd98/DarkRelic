using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DR_GameManager : MonoBehaviour
{
    public enum GameState {
        ADVANCE_GAME,
        ANIMATING,
        HANDLING_TURN,
        GAME_OVER,

        INVALID,
    }

    public bool isFadeActive = false;

    public int entitesCreated = 0; //used to give IDs

    public static DR_GameManager instance;

    public GameState CurrentState = GameState.INVALID;

    public DungeonGenerator dungeonGenerator;

    public DR_Dungeon CurrentDungeon;
    public DR_Map CurrentMap;
    public Texture2D DebugMap, DebugMap2, pathfindTestMap, balanceTestMap;
    public Sprite OpenDoorTexture, ClosedDoorTexture, StairsDownTexture, StairsUpTexture,
        PotionTexture, FireboltTexture, ShockTexture, GoalTexture, AmuletTexture, FireProjectile, SparkProjectile, BossTexture;

    public Content playerContent;
    public List<Content> enemyContentArray;
    public List<Content> relicPickupContentArray;
    public Content healthAltarContent, itemAltarContent;

    public bool debug_disableFOV = false;

    //Temp Player
    DR_Entity PlayerActor;
    DR_Entity BossActor;

    [HideInInspector]
    public TurnSystem turnSystem;
    public ImageFadeToggle blackOverlay;

    public static KeyCode[] KeyDirections = {KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow};
    public static KeyCode[] NumberKeys = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9};
    public Vector2Int[] Directions = {Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};

    private void Awake() {
        if (instance != null){
            Debug.LogError("There are multiple game managers!");
            Destroy(this);
        }else{
            instance = this;
        }
    }

    void Start()
    {
        // Create Dungeon
        blackOverlay.SetShouldBeVisible(true);
        UISystem.instance.gameObject.SetActive(false);
        dungeonGenerator.GenerateDungeon(PostDungeonGenStep);
    }

    private void PostDungeonGenStep(DR_Dungeon dungeon){
        CurrentDungeon = dungeon;
        PlayerActor = EntityFactory.CreateEntityFromContent(playerContent);
        PlayerActor.GetComponent<LevelComponent>().level = 5;
        PlayerActor.GetComponent<LevelComponent>().UpdateStats();

        UISystem.instance.gameObject.SetActive(true);
        UISystem.instance.currentState = UISystem.UIState.NORMAL;
        UISystem.instance.UpdateInventoryUI(PlayerActor);

        BossActor = EntityFactory.CreateActor(BossTexture, "Boss", Alignment.ENEMY, 20);
        BossActor.AddComponent<AIComponent>(new AIComponent());
        
        MoveLevels(null, CurrentDungeon.maps[0], true, false);

        //Temp placement of boss enemy besides goal
        DR_Map lastMap = CurrentDungeon.maps[CurrentDungeon.maps.Count-1];
        foreach (DR_Entity entity in lastMap.Entities){
            if (entity.HasComponent<GoalComponent>()){
                lastMap.AddActor(BossActor, lastMap.GetAdjacentPosition(entity.Position));
                break;
            }
        }

        UpdateCurrentMap();

        // Init Camera
        GameRenderer.instance.UpdateCamera(true);

        // Create Turn System
        turnSystem = gameObject.AddComponent<TurnSystem>();
        turnSystem.UpdateEntityLists(CurrentMap);
        SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);

        SetGameState(GameState.ADVANCE_GAME);
        UISystem.instance.RefreshDetailsUI();
        UISystem.instance.UpdateDepthUI();

        blackOverlay.SetShouldBeVisible(false);
        GameRenderer.instance.FullyUpdateRenderer(true);
    }

    void Update()
    {
        if (CurrentState == GameState.INVALID){
            return;
        }

        bool finishedIterating = false;

        int iterations = 0;
        while (!finishedIterating && iterations++ < 100){
            switch (CurrentState)
            {
                case GameState.ADVANCE_GAME:
                    {
                        if (turnSystem.CanEntityAct())
                        {
                            DR_Entity entity = turnSystem.GetNextEntity().Entity;
                            if (entity.HasComponent<PlayerComponent>() && GameRenderer.instance.HasActionsQueued()){
                                GameRenderer.instance.FullyUpdateRenderer();
                                SetGameState(GameState.ANIMATING);
                                break;
                            }
                            turnSystem.HandleTurn(this, entity);
                        }
                        else
                        {
                            //reduce debts until an entity can act
                            int limit = 50;
                            while (!turnSystem.CanEntityAct() && limit-- > 0)
                            {
                                turnSystem.RecoverDebts(1);
                                turnSystem.UpdateEntityLists(CurrentMap);
                            }
                        }
                        break;
                    }
                case GameState.ANIMATING:
                {
                    if (!GameRenderer.instance.currentlyUpdating){
                        SetGameState(GameState.ADVANCE_GAME);
                    }
                    finishedIterating = true;
                    break;
                }
                case GameState.HANDLING_TURN:
                {
                    finishedIterating = true;
                    break;
                }

                default:
                    break;
            }
        }

        // if (CurrentState != GameState.ANIMATING && AnimationSystem.IsAnimating()){
        //     SetGameState(GameState.ANIMATING);
        // }

        if (CurrentState != GameState.GAME_OVER && CurrentState != GameState.ANIMATING &&!PlayerActor.GetComponent<HealthComponent>().IsAlive()){
            //TODO: let this happen after animations are finished
            OnPlayerDied();
        }
    }

    public void OnPlayerDied(){
        CurrentState = GameState.GAME_OVER;
        UISystem.instance.ShowGameOver();
    }

    public void OnGameWon(){
        CurrentState = GameState.GAME_OVER;
        UISystem.instance.ShowVictory();
    }

    public void OnTurnHandled(){
        SetGameState(GameState.ADVANCE_GAME);
    }

    private void LateUpdate() {
        if (CurrentState == GameState.INVALID){
            return;
        }
        GameRenderer.instance.UpdateCamera();
    }

    public void SetGameState(GameState newState){
        if (CurrentState != newState){
            //Debug.Log("Changed states from " + CurrentState.ToString() + " to " + newState.ToString());
        }
        CurrentState = newState;
    }

    public void UpdateCurrentMap(){
        CurrentMap = CurrentDungeon.GetCurrentMap();
        UISystem.instance.UpdateDepthUI();
    }

    // TODO:
    // --- BUG! ---
    // There is a bug where enemies aren't being cleaned up correctly when moving floors (or at least that's when it's been noticed)
    // they appear when mousing over in details panel, but have no sprite (perhaps not being removed from entities array)
    // they can still attack player
    // --- BUG! ---
    public void MoveLevels(DR_Map origin, DR_Map destination, bool goingDeeper, bool useTransition = false) {
        if (origin == destination) {
            return;
        }

        if (!useTransition) {
            LoadNextLevel(origin, destination, goingDeeper);
            return;
        }
        
        Action OnFadeOut = null;
        OnFadeOut = () => {
            blackOverlay.OnVisibleComplete -= OnFadeOut;
            LoadNextLevel(origin, destination, goingDeeper);
            blackOverlay.SetShouldBeVisible(false);
            turnSystem.UpdateEntityLists(CurrentMap);
            SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
            GameRenderer.instance.FullyUpdateRenderer(true);
            //DR_Renderer.instance.CreateTiles();
            isFadeActive = false;
        };
        Action OnFadeIn = null;
        OnFadeIn = () => {
            blackOverlay.OnVisibleComplete -= OnFadeIn;
        };
        
        blackOverlay.OnVisibleComplete += OnFadeOut;
        blackOverlay.OnInvisibleComplete += OnFadeIn;
        
        isFadeActive = true;
        blackOverlay.SetShouldBeVisible(true);
    }

    private void LoadNextLevel(DR_Map origin, DR_Map destination, bool goingDeeper) {
        
        if (origin != null){
            origin.RemoveActor(PlayerActor);
            CurrentDungeon.SetNextMap(goingDeeper);
        }
        
        Vector2Int newPos = destination.GetStairPosition(!goingDeeper);
        destination.AddActor(PlayerActor, newPos);
        
        UpdateCurrentMap();
        GameRenderer.instance.UpdateCamera(true);
        GameRenderer.instance.ClearAllObjects();
        //DR_Renderer.instance.ClearAllObjects();
        //DR_Renderer.instance.CreateTiles();
    }
    
    public DR_Entity GetPlayer(){
        return PlayerActor;
    }

    public DR_Cell TryGetPlayerCell()
    {
        if (PlayerActor != null && CurrentMap != null){
            return CurrentMap.GetCell(PlayerActor.Position);
        }
        return null;
    }

    public DR_Entity GetBoss(){
        return BossActor;
    }
}
