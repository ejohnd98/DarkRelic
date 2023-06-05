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

    GameState CurrentState = GameState.INVALID;

    public DR_Map CurrentMap;
    public Texture2D DebugMap;

    //Temp renderer stuff
    public Sprite WallTexture, FloorTexture, PlayerTexture, EnemyTexture, FogTexture;
    public GameObject CellObj;
    public List<GameObject> CellObjects;
    public List<GameObject> EntityObjects;

    //Temp Camera 
    public Camera MainCamera;

    //Temp Player
    DR_Actor PlayerActor;

    TurnSystem turnSystem;

    static KeyCode[] KeyDirections = {KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow};
    static Vector2Int[] Directions = {Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};

    void Start()
    {
        if(DebugMap != null){
            CurrentMap = DR_Map.CreateMapFromImage(DebugMap);
        }
        CellObjects = new List<GameObject>();
        EntityObjects = new List<GameObject>();

        PlayerActor = CreateActor(PlayerTexture, "Player");
        PlayerActor.AddComponent<PlayerComponent>(new PlayerComponent());
        CurrentMap.AddActor(PlayerActor, new Vector2Int(24,28));

        CurrentMap.AddActor(CreateActor(EnemyTexture, "TestEnemy"), new Vector2Int(24,29));

        Vector3 DesiredPos = MainCamera.transform.position;
        DesiredPos.x = PlayerActor.Position.x;
        DesiredPos.y = PlayerActor.Position.y;
        MainCamera.transform.position = DesiredPos;

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
                    if (CurrentMap.IsVisible[y, x]){
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
            if (!CurrentMap.IsVisible[Entity.Position.y, Entity.Position.x]){
                continue;
            }
            SpriteComponent spriteComponent = Entity.GetComponent<SpriteComponent>();
            if (spriteComponent == null){
                continue;
            }

            GameObject NewEntityObj = Instantiate(CellObj, Entity.GetPosFloat(-1.0f), Quaternion.identity, transform);
            NewEntityObj.GetComponent<SpriteRenderer>().sprite = spriteComponent.Sprite;
            EntityObjects.Add(NewEntityObj);
        }  
    }

    //move into other class
    DR_Actor CreateActor(Sprite Sprite, string Name, int maxHealth = 10){
        DR_Actor NewActor = new DR_Actor();

        NewActor.Name = Name;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewActor.AddComponent<HealthComponent>(new HealthComponent(maxHealth));
        NewActor.AddComponent<TurnComponent>(new TurnComponent());
        
        return NewActor;
    }
}
