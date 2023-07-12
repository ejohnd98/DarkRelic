using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class 
public class MapGenInfo{
    public Vector2Int MapSize;

    // Possible parameters:
    // - type of generator (cave, ruins, castle, etc)
    // - required rooms
    // - loot? enemies?
}

public class DR_MapGen
{
    public static DR_Map CreateMapFromMapInfo(MapGenInfo mapGenInfo){
        DR_Map NewMap = CreateEmptyMap(mapGenInfo.MapSize);

        // do stuff based on map info

        return NewMap;
    }

    public static DR_Map CreateMapFromImage(Texture2D MapTexture){
        int Width = MapTexture.width;
        int Height = MapTexture.height;

        DR_Map NewMap = CreateEmptyMap(new Vector2Int(Width, Height));

        Color[] Pixels = MapTexture.GetPixels();

        for (int y = 0; y < Height; y++){
            for (int x = 0; x < Width; x++){
                int Index1D = y*Width + x;
                NewMap.Cells[y,x] = new DR_Cell();
                Color color = Pixels[Index1D];
                bool isWall = color.r < 0.1f && color.g < 0.1f && color.b < 0.1f;
                NewMap.Cells[y,x].bBlocksMovement = isWall;

                bool isDoor = color.r < 0.1f && color.g > 0.9f && color.b < 0.1f;
                if (isDoor){
                    DR_GameManager gm = DR_GameManager.instance;
                    DR_Entity door = gm.CreateDoor(gm.OpenDoorTexture, gm.ClosedDoorTexture);
                    NewMap.AddProp(door, new Vector2Int(x,y));
                }

                bool isStairsDeeper = color.r > 0.9f && color.g < 0.1f && color.b < 0.1f;
                bool isStairsShallower= color.r < 0.1f && color.g < 0.1f && color.b > 0.9f;
                if (isStairsDeeper || isStairsShallower){
                    DR_GameManager gm = DR_GameManager.instance;
                    DR_Entity stairs = gm.CreateStairs(isStairsDeeper? gm.StairsDownTexture : gm.StairsUpTexture, isStairsDeeper);
                    NewMap.AddProp(stairs, new Vector2Int(x,y));
                }

                NewMap.IsVisible[y,x] = false;
                NewMap.IsKnown[y,x] = false;
            }
        }

        return NewMap;
    }

    private static DR_Map CreateEmptyMap (Vector2Int size){
        DR_Map NewMap = new DR_Map();

        NewMap.MapSize = size;
        NewMap.Cells = new DR_Cell[size.y,size.x];
        NewMap.IsVisible = new bool[size.y,size.x];
        NewMap.IsKnown = new bool[size.y,size.x];
        NewMap.Entities = new List<DR_Entity>();

        return NewMap;
    }
}
