using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_GameManager : MonoBehaviour
{
    enum GameState {
        RUNNING,
        WAITING_FOR_INPUT,
        ANIMATING,

        INVALID
    }

    public int entitesCreated = 0; //used to give IDs

    public static DR_GameManager instance;

    GameState CurrentState = GameState.INVALID;

    public DR_Dungeon CurrentDungeon;
    public DR_Map CurrentMap;
    public Texture2D DebugMap, DebugMap2;
    public Sprite PlayerTexture, EnemyTexture, OpenDoorTexture, ClosedDoorTexture, StairsDownTexture, StairsUpTexture,
        PotionTexture, MagicScrollTexture;

    public bool debug_disableFOV = false;

    //Temp Camera 
    public Camera MainCamera;

    //Temp Player
    DR_Entity PlayerActor;

    TurnSystem turnSystem;

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

        PlayerActor = EntityFactory.CreateActor(PlayerTexture, "Player", 20, Alignment.PLAYER);
        PlayerActor.AddComponent<PlayerComponent>(new PlayerComponent());
        UISystem.instance.UpdateInventoryUI(PlayerActor);

        MapGenInfo mapGenInfo = new MapGenInfo(new Vector2Int(35,35));

        // Add maps to Dungeon
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));
        CurrentDungeon.maps.Add(DR_MapGen.CreateMapFromMapInfo(mapGenInfo));
        
        MoveLevels(null, CurrentDungeon.maps[0], true);
        UpdateCurrentMap();

        // Init Camera
        UpdateCamera(true);

        // Create Turn System
        turnSystem = new TurnSystem();
        turnSystem.UpdateEntityLists(CurrentMap);
        SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
        DR_Renderer.instance.UpdateTiles();

        SetGameState(GameState.RUNNING);
        UISystem.instance.RefreshDetailsUI();
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
                        
                        DR_Action entityAction = AISystem.DetermineAIAction(entity, CurrentMap);
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
                                selectedAction.Perform(this);

                                PlayerActor.GetComponent<TurnComponent>().SpendTurn();
                                turnSystem.PopNextEntity();
                                SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
                                DR_Renderer.instance.UpdateTiles();
                                UISystem.instance.RefreshDetailsUI();
                            }
                            break;
                        }
                    }
                    else
                    {
                        SetGameState(GameState.RUNNING);
                        break;
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
    }

    private void LateUpdate() {
        UpdateCamera();
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
        DR_Renderer.instance.UpdateTiles();
    }
    
    //  UI FUNCTIONS
    public DR_Entity GetPlayer(){
        return PlayerActor;
    }
}