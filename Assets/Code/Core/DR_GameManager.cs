using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_GameManager : MonoBehaviour
{
    enum GameState {
        RUNNING,
        WAITING_FOR_INPUT,

        INVALID
    }

    public static DR_GameManager instance;

    GameState CurrentState = GameState.INVALID;

    public DR_Dungeon CurrentDungeon;
    public DR_Map CurrentMap;
    public Texture2D DebugMap, DebugMap2;

    //Temp renderer stuff
    public Sprite WallTexture, FloorTexture, PlayerTexture, EnemyTexture, FogTexture, OpenDoorTexture, ClosedDoorTexture, StairsDownTexture, StairsUpTexture;
    public GameObject CellObj;
    public List<GameObject> CellObjects;
    public List<GameObject> EntityObjects;

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

        
        // Create Map, Actors
        DR_Map TestMap = DR_Map.CreateMapFromImage(DebugMap);
        DR_Map TestMap2 = DR_Map.CreateMapFromImage(DebugMap2);

        PlayerActor = CreateActor(PlayerTexture, "Player");
        PlayerActor.AddComponent<PlayerComponent>(new PlayerComponent());

        TestMap.AddActor(PlayerActor, new Vector2Int(24,28));
        TestMap.AddActor(CreateActor(EnemyTexture, "TestEnemy"), new Vector2Int(24,29));

        // Add map to Dungeon
        CurrentDungeon.maps.Add(TestMap);
        CurrentDungeon.maps.Add(TestMap2);
        UpdateCurrentMap();

        // Init Camera
        Vector3 DesiredPos = MainCamera.transform.position;
        DesiredPos.x = PlayerActor.Position.x;
        DesiredPos.y = PlayerActor.Position.y;
        MainCamera.transform.position = DesiredPos;

        // Init Renderer lists
        CellObjects = new List<GameObject>();
        EntityObjects = new List<GameObject>();

        // Create Turn System
        turnSystem = new TurnSystem();
        turnSystem.UpdateEntityLists(CurrentMap);
        SightSystem.CalculateVisibleCells(PlayerActor, CurrentMap);
        UpdateVisuals();

        CurrentState = GameState.RUNNING;
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
                            CurrentState = GameState.WAITING_FOR_INPUT;
                            break;
                        }

                        //AI TURN

                        turnSystem.GetNextEntity().SpendTurn();
                        Debug.Log(turnSystem.GetNextEntity().Entity.Name + " did nothing");
                        turnSystem.PopNextEntity();

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
                        UpdateVisuals(false);
                    }
                    break;
                }

            case GameState.WAITING_FOR_INPUT:
                {

                    //TODO change into an action based system (have struct representing an action)
                    if (turnSystem.IsPlayerTurn())
                    {
                        bool keyPressed = false;

                        Vector2Int interactPos = Vector2Int.zero;
                        for (int i = 0; i < 4; i++)
                        {
                            if (Input.GetKeyDown(KeyDirections[i]))
                            {
                                keyPressed = true;
                                interactPos = PlayerActor.Position + Directions[i];
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.Space)){
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
                                            CurrentMap.MoveActor(PlayerActor, moveAction.pos);
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
                                UpdateVisuals(true);
                            }
                        }
                    }
                    else
                    {
                        CurrentState = GameState.RUNNING;
                        break;
                    }
                    break;
                }

            default:
                break;
        }

        UpdateCamera();
    }

    void UpdateCamera()
    {
        Vector3 DesiredPos = MainCamera.transform.position;
        DesiredPos.x = PlayerActor.Position.x;
        DesiredPos.y = PlayerActor.Position.y;

        Vector3 Direction = DesiredPos - MainCamera.transform.position;
        float LerpAmount = Time.deltaTime * 1f + Mathf.Clamp01(Time.deltaTime * 4.0f / Direction.magnitude);

        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, DesiredPos, LerpAmount);
    }

    // TODO: IMPROVE THIS MESS
    // Make it only add objects within the camera
    void UpdateVisuals(bool updateTiles = true){
        // Clear old visuals
        if (updateTiles){
            foreach(GameObject obj in CellObjects){
                Destroy(obj);
            }
            CellObjects.Clear();

            // Add new visuals
            for(int y = 0; y < CurrentMap.MapSize.y; y++){
                for(int x = 0; x < CurrentMap.MapSize.x; x++){
                    GameObject NewCellObj = Instantiate(CellObj,new Vector3(x, y, 0),Quaternion.identity, transform);
                    Sprite CellSprite = FogTexture;
                    if (CurrentMap.IsVisible[y, x] || debug_disableFOV){
                        CellSprite = CurrentMap.Cells[y,x].bBlocksMovement? WallTexture : FloorTexture;
                    }else if (CurrentMap.IsKnown[y, x]){
                        CellSprite = CurrentMap.Cells[y,x].bBlocksMovement? WallTexture : FloorTexture;
                        NewCellObj.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                    }
                    NewCellObj.GetComponent<SpriteRenderer>().sprite = CellSprite;
                    CellObjects.Add(NewCellObj);
                }
            }
        }
        
        foreach(GameObject obj in EntityObjects){
            Destroy(obj);
        }
        EntityObjects.Clear();

        foreach(DR_Entity Entity in CurrentMap.Entities){
            bool isVisible = CurrentMap.IsVisible[Entity.Position.y, Entity.Position.x];
            bool isKnown = CurrentMap.IsKnown[Entity.Position.y, Entity.Position.x];
            if (!isVisible && !isKnown && !debug_disableFOV){
                continue;
            }
            
            bool isProp = Entity.HasComponent<PropComponent>();

            if (!isVisible && (!isProp || !isKnown) && !debug_disableFOV){
                continue;
            }

            SpriteComponent spriteComponent = Entity.GetComponent<SpriteComponent>();
            if (spriteComponent == null){
                continue;
            }

            // TODO: make proper system to determine z depth for each entity
            GameObject NewEntityObj = Instantiate(CellObj, Entity.GetPosFloat(isProp ? -0.9f : -1.0f), Quaternion.identity, transform);
            NewEntityObj.GetComponent<SpriteRenderer>().sprite = spriteComponent.Sprite;

            if (!isVisible && isKnown && !debug_disableFOV){
                NewEntityObj.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            }

            EntityObjects.Add(NewEntityObj);
        }  
    }

    //move into other class
    public DR_Entity CreateActor(Sprite Sprite, string Name, int maxHealth = 10){
        DR_Entity NewActor = new DR_Entity();

        NewActor.Name = Name;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewActor.AddComponent<HealthComponent>(new HealthComponent(maxHealth));
        NewActor.AddComponent<TurnComponent>(new TurnComponent());
        
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
        origin.RemoveActor(PlayerActor);
        Vector2Int newPos = destination.GetStairPosition(!goingDeeper);
        destination.AddActor(PlayerActor, newPos);
        CurrentDungeon.SetNextMap(goingDeeper);
        UpdateCurrentMap();
        UpdateVisuals(true);
    }
}
