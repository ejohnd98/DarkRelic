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

        INVALID
    }

    public int entitesCreated = 0; //used to give IDs

    public static DR_GameManager instance;

    GameState CurrentState = GameState.INVALID;

    public DR_Dungeon CurrentDungeon;
    public DR_Map CurrentMap;
    public Texture2D DebugMap, DebugMap2, pathfindTestMap;
    public Sprite OpenDoorTexture, ClosedDoorTexture, StairsDownTexture, StairsUpTexture,
        PotionTexture, FireboltTexture, ShockTexture, GoalTexture, AmuletTexture, FireProjectile, SparkProjectile, BossTexture;

    public Content playerContent;
    public List<Content> enemyContentArray;
    public List<Content> relicPickupContentArray;

    public bool debug_disableFOV = false;

    //Temp Camera 
    public Camera MainCamera;
    public Vector3 cameraOffset;

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

        PlayerActor = EntityFactory.CreateEntityFromContent(playerContent);

        UISystem.instance.UpdateInventoryUI(PlayerActor);

        BossActor = EntityFactory.CreateActor(BossTexture, "Boss", Alignment.ENEMY, 10);
        BossActor.AddComponent<AIComponent>(new AIComponent());

        MapGenInfo mapGenInfo = new MapGenInfo(new Vector2Int(35,35), 1);

        // pathfinding debug map
        //CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromImage(DebugMap2));

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
        turnSystem = gameObject.AddComponent<TurnSystem>();
        turnSystem.UpdateEntityLists(CurrentMap);
        SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
        DR_Renderer.instance.CreateTiles();

        SetGameState(GameState.ADVANCE_GAME);
        UISystem.instance.RefreshDetailsUI();
        UISystem.instance.UpdateDepthUI();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case GameState.ADVANCE_GAME:
                {
                    if (turnSystem.CanEntityAct())
                    {
                        DR_Entity entity = turnSystem.GetNextEntity().Entity;
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
                if (DR_Renderer.animsActive <= 0){
                    SetGameState(GameState.ADVANCE_GAME);
                }
                break;
            }
            case GameState.HANDLING_TURN:
            {
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

    public void OnTurnHandled(){
        SetGameState(GameState.ADVANCE_GAME);
    }

    private void LateUpdate() {
        UpdateCamera();
    }

    public void SetGameState(GameState newState){
        if (CurrentState != newState){
            //Debug.Log("Changed states from " + CurrentState.ToString() + " to " + newState.ToString());
        }
        CurrentState = newState;
    }

    public void UpdateCamera(bool forcePos = false)
    {
        Vector3 DesiredPos = MainCamera.transform.position;
        if (PlayerActor.HasComponent<MoveAnimation>()){
            DesiredPos.x = PlayerActor.GetComponent<MoveAnimation>().GetAnimPosition().x;
            DesiredPos.y = PlayerActor.GetComponent<MoveAnimation>().GetAnimPosition().y;
        }else{
            DesiredPos.x = PlayerActor.Position.x;
            DesiredPos.y = PlayerActor.Position.y;
        }

        DesiredPos += cameraOffset;
        

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

    // TODO:
    // --- BUG! ---
    // There is a bug where enemies aren't being cleaned up correctly when moving floors (or at least that's when it's been noticed)
    // they appear when mousing over in details panel, but have no sprite (perhaps not being removed from entities array)
    // they can still attack player
    // --- BUG! ---
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
