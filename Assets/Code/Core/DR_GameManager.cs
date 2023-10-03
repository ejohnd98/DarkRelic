using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_GameManager : MonoBehaviour
{
    enum GameState {
        RUNNING,
        WAITING_FOR_INPUT,
        FURTHER_INPUT_REQUIRED,
        ANIMATING,
        GAME_OVER,

        INVALID
    }

    public int entitesCreated = 0; //used to give IDs

    public static DR_GameManager instance;

    GameState CurrentState = GameState.INVALID;
    DR_Action currentAction;

    public DR_Dungeon CurrentDungeon;
    public DR_Map CurrentMap;
    public Texture2D DebugMap, DebugMap2, pathfindTestMap;
    public Sprite PlayerTexture, EnemyTexture, OpenDoorTexture, ClosedDoorTexture, StairsDownTexture, StairsUpTexture,
        PotionTexture, FireboltTexture, ShockTexture, GoalTexture, AmuletTexture, FireProjectile, SparkProjectile, BossTexture;

    public bool debug_disableFOV = false;

    //Temp Camera 
    public Camera MainCamera;

    //Temp Player
    DR_Entity PlayerActor;
    DR_Entity BossActor;

    public TurnSystem turnSystem;

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
        CurrentDungeon = new DR_Dungeon();
        CurrentDungeon.name = "Testing Dungeon";

        PlayerActor = EntityFactory.CreateActor(PlayerTexture, "Player", Alignment.PLAYER, 1);
        PlayerActor.AddComponent<PlayerComponent>(new PlayerComponent());
        UISystem.instance.UpdateInventoryUI(PlayerActor);

        BossActor = EntityFactory.CreateActor(BossTexture, "Boss", Alignment.ENEMY, 10);
        BossActor.AddComponent<AIComponent>(new AIComponent());

        MapGenInfo mapGenInfo = new MapGenInfo(new Vector2Int(35,35), 1);

        // pathfinding debug map
        //CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromImage(pathfindTestMap));

        // Add maps to Dungeon
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));
        mapGenInfo.depth = 2;
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));
        mapGenInfo.depth = 3;
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));

        mapGenInfo.isLastFloor = true;
        mapGenInfo.depth = 4;
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));

        //temp:
        DR_Entity item1 = EntityFactory.CreateHealingItem(PotionTexture, "Health Potion", 10);
        DR_Entity item2 = EntityFactory.CreateMagicItem(ShockTexture, "Shock Scroll", 5);
        DR_Entity item3 = EntityFactory.CreateTargetedMagicItem(FireboltTexture, "Firebolt Scroll", 5);
        DR_Entity testEquipment = EntityFactory.CreateEquipmentItem(AmuletTexture, "Amulet of Double Damage");
        testEquipment.GetComponent<EquippableComponent>().modifiers.Add(new AttackMultiplierModifier(4.0f));

        PlayerActor.GetComponent<InventoryComponent>().AddItem(item1);
        PlayerActor.GetComponent<InventoryComponent>().AddItem(item2);
        PlayerActor.GetComponent<InventoryComponent>().AddItem(item3);
        PlayerActor.GetComponent<InventoryComponent>().AddItem(testEquipment);
        
        MoveLevels(null, CurrentDungeon.maps[0], true);

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
        UpdateCamera(true);

        // Create Turn System
        turnSystem = new TurnSystem();
        turnSystem.UpdateEntityLists(CurrentMap);
        SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
        DR_Renderer.instance.CreateTiles();

        SetGameState(GameState.RUNNING);
        UISystem.instance.RefreshDetailsUI();
        UISystem.instance.UpdateDepthUI();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case GameState.RUNNING:
                {
                    if (turnSystem.CanEntityAct())
                    {
                        if (turnSystem.IsPlayerTurn())
                        {
                            SetGameState(GameState.WAITING_FOR_INPUT);
                            break;
                        }

                        //AI TURN

                        turnSystem.GetNextEntity().SpendTurn();
                        DR_Entity entity = turnSystem.PopNextEntity().Entity;
                        DR_Action entityAction = AISystem.DetermineAIAction(this, entity);
                        if (entityAction != null){
                            entityAction.Perform(this);
                        }
                        UISystem.instance.RefreshDetailsUI();
                    }
                    else
                    {
                        //advance game

                        //reduce debts
                        int limit = 50;
                        while (!turnSystem.CanEntityAct() && limit-- > 0)
                        {
                            turnSystem.RecoverDebts(1);
                            turnSystem.UpdateEntityLists(CurrentMap);
                        }
                    }
                    break;
                }

            case GameState.WAITING_FOR_INPUT:
                {
                    //TODO make input handler class, and have key presses linger so that you can press arrow keys slightly before allowed
                    if (turnSystem.IsPlayerTurn())
                    {
                        KeyCode key = KeyCode.None;

                        Vector2Int interactPos = Vector2Int.zero;
                        for (int i = 0; i < KeyDirections.Length; i++)
                        {
                            if (DR_InputHandler.GetKeyPressed(KeyDirections[i]))
                            {
                                key = KeyDirections[i];
                                interactPos = PlayerActor.Position + Directions[i];
                            }
                        }

                        for (int i = 0; i < NumberKeys.Length; i++)
                        {
                            if (DR_InputHandler.GetKeyPressed(NumberKeys[i]))
                            {
                                key = NumberKeys[i];
                                interactPos = PlayerActor.Position;
                            }
                        }

                        if (DR_InputHandler.GetKeyPressed(KeyCode.Space)){
                            key = KeyCode.Space;
                            interactPos = PlayerActor.Position;
                        }

                        if (DR_InputHandler.GetKeyPressed(KeyCode.G)){
                            key = KeyCode.G;
                            interactPos = PlayerActor.Position;
                        }

                        DR_Action UIAction = UISystem.instance.GetUIAction();

                        if (key != KeyCode.None || UIAction != null)
                        {
                            List<DR_Action> possibleActions = InteractionSystem.GetPotentialActions(PlayerActor, CurrentMap, interactPos, key);
                            DR_Action selectedAction = null;

                            if (UIAction != null){
                                selectedAction = UIAction;
                            }else if (possibleActions.Count > 0){
                                selectedAction = possibleActions[0] ;
                            }

                            // Just do first action for now
                            if (selectedAction != null)
                            {
                                if (selectedAction.requiresFurtherInput){
                                    CurrentState = GameState.FURTHER_INPUT_REQUIRED;
                                    LogSystem.instance.AddTextLog("Please select a target...");
                                    currentAction = selectedAction;
                                    UISystem.instance.BeginTargetSelection();
                                    break;
                                }
                                selectedAction.Perform(this);

                                PlayerActor.GetComponent<TurnComponent>().SpendTurn();
                                turnSystem.PopNextEntity();
                                SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
                                DR_Renderer.instance.UpdateTiles();
                                UISystem.instance.RefreshDetailsUI();
                            }
                            
                                if (PlayerActor != null){
                                    LevelComponent levelComp = PlayerActor.GetComponent<LevelComponent>();
                                    if (levelComp.RequiresLevelUp()){
                                        LogSystem.instance.AddTextLog("Player leveled up!");
                                        levelComp.AdvanceLevel();

                                        //todo: create new game state for this where player can choose skills. etc
                                        //ie. this should be a choice later:
                                    }
                                }
                        }
                    }
                    else
                    {
                        SetGameState(GameState.RUNNING);
                        break;
                    }
                    break;
                }
            case GameState.FURTHER_INPUT_REQUIRED:
            {
                if (currentAction.hasReceivedFurtherInput){
                    bool actionSuccess = currentAction.Perform(this);
                    if (!actionSuccess){
                        SetGameState(GameState.WAITING_FOR_INPUT);
                        currentAction = null;
                        break;
                    }

                    PlayerActor.GetComponent<TurnComponent>().SpendTurn();
                    turnSystem.PopNextEntity();
                    SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
                    DR_Renderer.instance.UpdateTiles();
                    UISystem.instance.RefreshDetailsUI();

                    SetGameState(GameState.RUNNING);
                    currentAction = null;
                }
                break;
            }

            case GameState.ANIMATING:
            {
                if (DR_Renderer.animsActive <= 0){
                    SetGameState(GameState.RUNNING);
                }
                break;
            }

            default:
                break;
        }

        if (CurrentState != GameState.ANIMATING && DR_Renderer.animsActive > 0){
            SetGameState(GameState.ANIMATING);
        }

        if (CurrentState != GameState.GAME_OVER && !PlayerActor.GetComponent<HealthComponent>().IsAlive()){
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

    private void LateUpdate() {
        UpdateCamera();
    }

    public bool ProvideAdditionalInput(Vector2Int pos){
        if (CurrentState != GameState.FURTHER_INPUT_REQUIRED){
            Debug.LogError("Tried to provide further input but not in correct state!");
            return false;
        }
        if (currentAction == null){
            Debug.LogError("Tried to provide further input but currentAction is null!");
            return false;
        }

        return currentAction.GiveAdditionalInput(this, pos);
    }

    void SetGameState(GameState newState){
        if (CurrentState != newState){
            //Debug.Log("Changed states from " + CurrentState.ToString() + " to " + newState.ToString());
        }
        CurrentState = newState;
    }

    public void UpdateCamera(bool forcePos = false)
    {
        Vector3 DesiredPos = MainCamera.transform.position;
        if (PlayerActor.HasComponent<MoveAnimComponent>()){
            DesiredPos.x = PlayerActor.GetComponent<MoveAnimComponent>().GetAnimPosition().x;
            DesiredPos.y = PlayerActor.GetComponent<MoveAnimComponent>().GetAnimPosition().y;
        }else{
            DesiredPos.x = PlayerActor.Position.x;
            DesiredPos.y = PlayerActor.Position.y;
        }
        

        if (forcePos){
            MainCamera.transform.position = DesiredPos;
            return;
        }
        Vector3 Direction = DesiredPos - MainCamera.transform.position;
        float LerpAmount = Time.deltaTime * 3.0f;

        MainCamera.transform.position = Easings.QuadEaseOut(MainCamera.transform.position, DesiredPos, LerpAmount);
    }

    public void UpdateCurrentMap(){
        CurrentMap = CurrentDungeon.GetCurrentMap();
        UISystem.instance.UpdateDepthUI();
    }

    public void MoveLevels(DR_Map origin, DR_Map destination, bool goingDeeper){
        if (origin == destination){
            return;
        }

        if (origin != null){
            origin.RemoveActor(PlayerActor);
            CurrentDungeon.SetNextMap(goingDeeper);
        }

        Vector2Int newPos = destination.GetStairPosition(!goingDeeper);
        destination.AddActor(PlayerActor, newPos);
        
        UpdateCurrentMap();
        UpdateCamera(true);
        DR_Renderer.instance.ClearAllObjects();
        DR_Renderer.instance.CreateTiles();
    }
    
    public DR_Entity GetPlayer(){
        return PlayerActor;
    }

    public DR_Entity GetBoss(){
        return BossActor;
    }
}
