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
    public Sprite WallTexture, FloorTexture, PlayerTexture, EnemyTexture;
    public GameObject CellObj;
    public List<GameObject> CellObjects;
    public List<GameObject> EntityObjects;

    //Temp Camera 
    public Camera MainCamera;

    //Temp Player
    DR_Actor PlayerActor;

    TurnSystem turnSystem;

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
        UpdateVisuals();

        CurrentState = GameState.RUNNING;
    }

    void Update()
    {
        switch(CurrentState){
            case GameState.RUNNING:{
                if (turnSystem.CanEntityAct()){
                    if (turnSystem.IsPlayerTurn()){
                        CurrentState = GameState.WAITING_FOR_INPUT;
                        break;
                    }

                    //AI TURN
                    turnSystem.GetNextEntity().SpendTurn();
                    turnSystem.PopNextEntity();

                }else{
                    //advance game

                    //reduce debts
                    int limit = 50;
                    while(!turnSystem.CanEntityAct() && limit-- > 0){
                        turnSystem.RecoverDebts(1);
                        turnSystem.UpdateEntityLists(CurrentMap);
                    }
                    UpdateVisuals();
                }
                break;
            }

            case GameState.WAITING_FOR_INPUT:{

                //TODO change into an action based system (have struct representing an action)
                if (turnSystem.IsPlayerTurn()){
                    bool hasMoved = false;
                    if(Input.GetKeyDown(KeyCode.W)){
                        hasMoved = CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.up);
                    }else if(Input.GetKeyDown(KeyCode.S)){
                        hasMoved = CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.down);
                    }else if(Input.GetKeyDown(KeyCode.D)){
                        hasMoved = CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.right);
                    }else if(Input.GetKeyDown(KeyCode.A)){
                        hasMoved = CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.left);
                    }

                    if (hasMoved){
                        PlayerActor.GetComponent<TurnComponent>().SpendTurn();
                        turnSystem.PopNextEntity();
                    }
                }else{
                    CurrentState = GameState.RUNNING;
                    break;
                }
                break;
            }

            default:
            break;
        }

        // Camera Movement
        Vector3 DesiredPos = MainCamera.transform.position;
        DesiredPos.x = PlayerActor.Position.x;
        DesiredPos.y = PlayerActor.Position.y;

        Vector3 Direction = DesiredPos - MainCamera.transform.position;
        float LerpAmount = Time.deltaTime * 1f + Mathf.Clamp01(Time.deltaTime * 4.0f / Direction.magnitude);

        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, DesiredPos, LerpAmount);
    }

    private void LateUpdate() {
        
    }

    // TODO: IMPROVE THIS MESS
    void UpdateVisuals(){
        // Clear old visuals
        foreach(GameObject obj in CellObjects){
            Destroy(obj);
        }
        CellObjects.Clear();
        foreach(GameObject obj in EntityObjects){
            Destroy(obj);
        }
        EntityObjects.Clear();

        // Add new visuals
        for(int y = 0; y < CurrentMap.MapSize.y; y++){
            for(int x = 0; x < CurrentMap.MapSize.x; x++){
                GameObject NewCellObj = Instantiate(CellObj,new Vector3(x, y, 0),Quaternion.identity, transform);
                NewCellObj.GetComponent<SpriteRenderer>().sprite = CurrentMap.Cells[y,x].bBlocksMovement? WallTexture : FloorTexture;
                CellObjects.Add(NewCellObj);
            }
        }

        foreach(DR_Entity Entity in CurrentMap.Entities){
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
