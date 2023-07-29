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
    public Sprite PlayerTexture, EnemyTexture, OpenDoorTexture, ClosedDoorTexture, StairsDownTexture, StairsUpTexture;

    public bool debug_disableFOV = false;

    //Temp Camera 
    public Camera MainCamera;

    //Temp Player
    DR_Entity PlayerActor;

    TurnSystem turnSystem;

    static KeyCode[] KeyDirections = {KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow};
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

        PlayerActor = CreateActor(PlayerTexture, "Player");
        PlayerActor.AddComponent<PlayerComponent>(new PlayerComponent());

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

                        switch (entityAction){
                            case AttackAction attackAction:
                                DamageSystem.HandleAttack(attackAction.target, attackAction.attacker);
                                //NOT GOOD:
                                if (!attackAction.target.IsAlive()){
                                    CurrentMap.RemoveActor(attackAction.target.Entity);
                                }
                                break;
                            case WaitAction waitAction:
                                Debug.Log(entity.Name + " did nothing");
                                break;
                            default:
                            break;
                        }

                        LogSystem.instance.AddLog(entityAction);

                        //TODO: create system to step through an action (use by both AI and player)

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
                        bool keyPressed = false;

                        Vector2Int interactPos = Vector2Int.zero;
                        for (int i = 0; i < 4; i++)
                        {
                            if (DR_InputHandler.GetKeyPressed(KeyDirections[i]))
                            {
                                keyPressed = true;
                                interactPos = PlayerActor.Position + Directions[i];
                            }
                        }

                        if (DR_InputHandler.GetKeyPressed(KeyCode.Space)){
                            keyPressed = true;
                            interactPos = PlayerActor.Position;
                        }

                        if (keyPressed)
                        {
                            List<DR_Action> possibleActions = InteractionSystem.GetPotentialActions(PlayerActor, CurrentMap, interactPos);

                            // Just do first action for now
                            if (possibleActions.Count > 0)
                            {
                                switch (possibleActions[0])
                                {

                                    case MoveAction moveAction:
                                        {
                                            Debug.Log(PlayerActor.Name + " moved");
                                            CurrentMap.MoveActor(PlayerActor, moveAction.pos, true);
                                            break;
                                        }

                                    case AttackAction attackAction:
                                        {
                                            DamageSystem.HandleAttack(attackAction.target, attackAction.attacker);
                                            //NOT GOOD:
                                            if (!attackAction.target.IsAlive()){
                                                CurrentMap.RemoveActor(attackAction.target.Entity);
                                            }
                                            break;
                                        }

                                    case DoorAction doorAction:
                                        {
                                            doorAction.target.ToggleOpen();
                                            Debug.Log(PlayerActor.Name + " opened " + doorAction.target.Entity.Name);
                                            break;
                                        }

                                    case StairAction stairAction:
                                        {
                                            DR_Map dest = CurrentDungeon.GetNextMap(stairAction.stairs.goesDeeper);
                                            MoveLevels(CurrentMap, dest, stairAction.stairs.goesDeeper);
                                            break;
                                        }

                                    case WaitAction waitAction:
                                        {
                                            Debug.Log(PlayerActor.Name + " did nothing");
                                            break;
                                        }

                                    default:
                                        {
                                            Debug.LogWarning("WAITING_FOR_INPUT: Player tried to perform an unknown action!");
                                            break;
                                        }
                                }

                                LogSystem.instance.AddLog(possibleActions[0]);

                                PlayerActor.GetComponent<TurnComponent>().SpendTurn();
                                turnSystem.PopNextEntity();
                                SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
                                DR_Renderer.instance.UpdateTiles();
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

        UpdateCamera();
    }

    void SetGameState(GameState newState){
        if (CurrentState != newState){
            Debug.Log("Changed states from " + CurrentState.ToString() + " to " + newState.ToString());
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

    //move into other class
    public DR_Entity CreateActor(Sprite Sprite, string Name, int maxHealth = 10){
        DR_Entity NewActor = new DR_Entity();

        NewActor.Name = Name;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewActor.AddComponent<HealthComponent>(new HealthComponent(maxHealth));
        NewActor.AddComponent<TurnComponent>(new TurnComponent());
        NewActor.AddComponent<MoveAnimComponent>(new MoveAnimComponent());
        
        return NewActor;
    }

    public DR_Entity CreateProp(Sprite Sprite, string Name){
        DR_Entity NewProp = new DR_Entity();

        NewProp.Name = Name;
        NewProp.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewProp.AddComponent<PropComponent>(new PropComponent());
        
        return NewProp;
    }

    public DR_Entity CreateDoor(Sprite OpenSprite, Sprite ClosedSprite){
        DR_Entity NewProp = CreateProp(ClosedSprite, "Door");

        NewProp.AddComponent<DoorComponent>(new DoorComponent(OpenSprite, ClosedSprite));
        NewProp.GetComponent<DoorComponent>().SetOpen(false);
        
        return NewProp;
    }

    public DR_Entity CreateStairs(Sprite spr, bool goesDeeper){
        DR_Entity NewProp = CreateProp(spr, "Stairs " + (goesDeeper? "Down" : "Up"));

        NewProp.AddComponent<StairComponent>(new StairComponent(goesDeeper));
        NewProp.GetComponent<PropComponent>().blocksSight = false;
        
        return NewProp;
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
