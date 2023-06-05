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

        for (int y = 0; y < Height; y++){
            for (int x = 0; x < Width; x++){
                int Index1D = y*Width + x;
                NewMap.Cells[y,x] = new DR_Cell();
                NewMap.Cells[y,x].bBlocksMovement = Pixels[Index1D].r < 0.5;

                NewMap.IsVisible[y,x] = false;
            }
        }

        

        NewMap.Entities = new List<DR_Entity>();

        return NewMap;
    }

    public bool AddActor(DR_Actor Actor, Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        if(Cell.IsTraversable() && Cell.Actor == null){
            Cell.Actor = Actor;
            Actor.Position = pos;
            Entities.Add(Actor);
            return true;
        }
        return false;
    }

    public void RemoveActor(DR_Entity Actor){
        DR_Cell Cell = Cells[Actor.Position.y, Actor.Position.x];
        Cell.Actor = null;
        Entities.Remove(Actor);
    }

    public DR_Actor RemoveActorAtPosition(Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        DR_Actor RemovedActor = Cell.Actor;
        Cell.Actor = null;
        Entities.Remove(RemovedActor);

        return RemovedActor;
    }

    public bool MoveActorRelative(DR_Actor Actor, Vector2Int posChange){
        return MoveActor(Actor, Actor.Position + posChange);
    }

    public bool MoveActor(DR_Actor Actor, Vector2Int pos){
        DR_Cell FromCell = Cells[Actor.Position.y, Actor.Position.x];
        DR_Cell ToCell = Cells[pos.y, pos.x];
        if(ToCell.IsTraversable() && ToCell.Actor == null){
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

    public bool BlocksSight(int x, int y){
        if (!ValidPosition(x, y)){
            return true;
        }

        return Cells[y,x].BlocksSight();
    }
}
