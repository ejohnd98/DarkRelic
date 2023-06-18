using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Map
{
    public const int MAX_MAP_SIZE = 50;

    public DR_Cell[,] Cells;
    public Vector2Int MapSize;

    //Could make this better:
    public bool[,] IsVisible;
    public bool[,] IsKnown;

    public List<DR_Entity> Entities;

    public static DR_Map CreateMapFromImage(Texture2D MapTexture){
        DR_Map NewMap = new DR_Map();

        int Width = MapTexture.width;
        int Height = MapTexture.height;
        NewMap.MapSize = new Vector2Int(Width, Height);

        Color[] Pixels = MapTexture.GetPixels();
        //Create test map
        NewMap.Cells = new DR_Cell[Height,Width];
        NewMap.IsVisible = new bool[Height,Width];
        NewMap.IsKnown = new bool[Height,Width];
        NewMap.Entities = new List<DR_Entity>();

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

    public bool AddActor(DR_Entity Actor, Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        if(!Cell.BlocksMovement() && Cell.Actor == null){
            Cell.Actor = Actor;
            Actor.Position = pos;
            Entities.Add(Actor);
            return true;
        }
        return false;
    }

    public bool AddProp(DR_Entity Prop, Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        if(!Cell.BlocksMovement() && Cell.Actor == null){
            Cell.Prop = Prop;
            Prop.Position = pos;
            Entities.Add(Prop);
            return true;
        }
        return false;
    }

    public void RemoveProp(DR_Entity Prop){
        DR_Cell Cell = Cells[Prop.Position.y, Prop.Position.x];
        Cell.Prop = null;
        Entities.Remove(Prop);
    }

    public DR_Entity RemovePropAtPosition(Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        DR_Entity RemovedProp = Cell.Prop;
        Cell.Prop = null;
        Entities.Remove(RemovedProp);

        return RemovedProp;
    }

    public void RemoveActor(DR_Entity Actor){
        DR_Cell Cell = Cells[Actor.Position.y, Actor.Position.x];
        Cell.Actor = null;
        Entities.Remove(Actor);
    }

    public DR_Entity RemoveActorAtPosition(Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        DR_Entity RemovedActor = Cell.Actor;
        Cell.Actor = null;
        Entities.Remove(RemovedActor);

        return RemovedActor;
    }

    public bool MoveActorRelative(DR_Entity Actor, Vector2Int posChange){
        return MoveActor(Actor, Actor.Position + posChange);
    }

    public bool MoveActor(DR_Entity Actor, Vector2Int pos){
        DR_Cell FromCell = Cells[Actor.Position.y, Actor.Position.x];
        DR_Cell ToCell = Cells[pos.y, pos.x];
        if(!ToCell.BlocksMovement() && ToCell.Actor == null){
            FromCell.Actor = null;
            ToCell.Actor = Actor;
            Actor.Position = pos;
            return true;
        }
        return false;
    }

    public void ClearVisible(){
        for (int y = 0; y < MapSize.y; y++){
            for (int x = 0; x < MapSize.x; x++){
                IsVisible[y,x] = false;
            }
        }
    }

    public bool ValidPosition(Vector2Int pos){
        return ValidPosition(pos.x, pos.y);
    }

    public bool ValidPosition(int x, int y){
        return (x >= 0 && x < MapSize.x && y >= 0 && y < MapSize.y);
    }

    public bool BlocksMovement(Vector2Int pos){
        return BlocksMovement(pos.x, pos.y);
    }

    public bool BlocksMovement(int x, int y){
        if (!ValidPosition(x, y)){
            return true;
        }

        return Cells[y,x].BlocksMovement();
    }

    public bool BlocksSight(Vector2Int pos){
        return BlocksSight(pos.x, pos.y);
    }

    public bool BlocksSight(int x, int y){
        if (!ValidPosition(x, y)){
            return true;
        }

        return Cells[y,x].BlocksSight();
    }

    // Messy temp function to get stair position
    public Vector2Int GetStairPosition(bool deeper){
        foreach (DR_Entity entity in Entities){
            StairComponent stair = entity.GetComponent<StairComponent>();
            if(stair == null || stair.goesDeeper != deeper){
                continue;
            }

            Vector2Int newPos = entity.Position;
            foreach (Vector2Int dir in DR_GameManager.instance.Directions){
                if (!BlocksMovement(newPos + dir)){
                    return newPos + dir;
                }
            }
        }
        return Vector2Int.zero;
    }
}
