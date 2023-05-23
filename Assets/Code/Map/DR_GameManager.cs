using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_GameManager : MonoBehaviour
{
    public DR_Map CurrentMap;
    public Texture2D DebugMap;


    //Temp renderer stuff
    public Sprite WallTexture, FloorTexture, PlayerTexture;
    public GameObject CellObj;
    public GameObject[,] CellObjects;

    //Temp Camera 
    public Camera MainCamera;

    //Temp Player
    DR_Actor PlayerActor;
    GameObject PlayerObj;

    void Start()
    {
        if(DebugMap != null){
            CurrentMap = DR_Map.CreateMapFromImage(DebugMap);
        }

        CellObjects = new GameObject[CurrentMap.MapSize.y,CurrentMap.MapSize.x];

        //separate out into renderer class eventually:
        for (int y = 0; y < CurrentMap.MapSize.y; y++){
            for (int x = 0; x < CurrentMap.MapSize.x; x++){
                int Index1D = y*CurrentMap.MapSize.x + x;
                GameObject NewCell = Instantiate(CellObj,new Vector3(x, y, 0),Quaternion.identity, transform);
                NewCell.GetComponent<SpriteRenderer>().sprite = CurrentMap.Cells[y,x].bBlocksMovement? WallTexture : FloorTexture;
                CellObjects[y,x] = NewCell;
            }
        }

        PlayerActor = new DR_Actor(new Vector2Int(24,28), PlayerTexture);
        CurrentMap.Cells[24,28].Actor = PlayerActor;

        PlayerObj = Instantiate(CellObj,new Vector3(28, 24, -1),Quaternion.identity, transform);
        PlayerObj.GetComponent<SpriteRenderer>().sprite = PlayerTexture;

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)){
            CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.up);
        }else if(Input.GetKeyDown(KeyCode.S)){
            CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.down);
        }else if(Input.GetKeyDown(KeyCode.D)){
            CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.right);
        }else if(Input.GetKeyDown(KeyCode.A)){
            CurrentMap.MoveActorRelative(PlayerActor, Vector2Int.left);
        }

        //figure out how to keep gameobjects in sync with board:
        Vector3 PlayerPos = PlayerObj.transform.position;
        PlayerPos.x = PlayerActor.Position.x;
        PlayerPos.y = PlayerActor.Position.y;
        PlayerObj.transform.position = PlayerPos;

        Vector3 CamPos = MainCamera.transform.position;
        CamPos.x = PlayerActor.Position.x;
        CamPos.y = PlayerActor.Position.y;
        MainCamera.transform.position = CamPos;
    }
}
