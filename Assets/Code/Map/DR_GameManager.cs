using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_GameManager : MonoBehaviour
{
    public DR_Map CurrentMap;
    public Texture2D DebugMap;


    //Temp renderer stuff
    public Sprite WallTexture, FloorTexture;
    public GameObject CellObj;
    public GameObject[,] CellObjects;

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
    }

    void Update()
    {
        
    }
}
