using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Map
{
    public DR_Cell[,] Cells;
    public Vector2Int MapSize;

    //Could make this better:
    public bool[,] IsVisible;
    public bool[,] IsKnown;

    public List<DR_Entity> Entities;

    public bool AddActor(DR_Entity Actor, Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        if(!Cell.BlocksMovement() && Cell.Actor == null){
            Cell.Actor = Actor;
            Actor.Position = pos;
            Actor.isOnMap = true;
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
            Prop.isOnMap = true;
            Entities.Add(Prop);
            return true;
        }
        return false;
    }

    public bool AddItem(DR_Entity item, Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        if(Cell.Item == null){
            Cell.Item = item;
            item.Position = pos;
            item.isOnMap = true;
            Entities.Add(item);
            return true;
        }
        return false;
    }

    public void RemoveItem(DR_Entity item){
        DR_Cell Cell = Cells[item.Position.y, item.Position.x];
        Cell.Item = null;
        item.isOnMap = false;
        Entities.Remove(item);
    }

    public void RemoveProp(DR_Entity Prop){
        DR_Cell Cell = Cells[Prop.Position.y, Prop.Position.x];
        Cell.Prop = null;
        Prop.isOnMap = false;
        Entities.Remove(Prop);
    }

    public DR_Entity RemovePropAtPosition(Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        DR_Entity RemovedProp = Cell.Prop;
        Cell.Prop = null;
        RemovedProp.isOnMap = false;
        Entities.Remove(RemovedProp);

        return RemovedProp;
    }

    public void RemoveActor(DR_Entity Actor){
        DR_Cell Cell = Cells[Actor.Position.y, Actor.Position.x];
        Cell.Actor = null;
        Actor.isOnMap = false;
        Entities.Remove(Actor);
    }

    public DR_Entity RemoveActorAtPosition(Vector2Int pos){
        DR_Cell Cell = Cells[pos.y, pos.x];
        DR_Entity RemovedActor = Cell.Actor;
        Cell.Actor = null;
        RemovedActor.isOnMap = false;
        Entities.Remove(RemovedActor);

        return RemovedActor;
    }

    public bool MoveActorRelative(DR_Entity Actor, Vector2Int posChange){
        return MoveActor(Actor, Actor.Position + posChange);
    }

    public bool MoveActor(DR_Entity Actor, Vector2Int pos, bool animate = false){
        DR_Cell FromCell = Cells[Actor.Position.y, Actor.Position.x];
        DR_Cell ToCell = Cells[pos.y, pos.x];

        if(ToCell.BlocksMovement() || ToCell.Actor != null){
            return false;
        }
        
        if (animate){
            MoveAnimComponent moveAnim = Actor.GetComponent<MoveAnimComponent>();
            moveAnim.SetAnim(pos);
        }

        FromCell.Actor = null;
        ToCell.Actor = Actor;
        Actor.Position = pos;  

        return true;
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

    public bool BlocksMovement(Vector2Int pos, bool ignoreActor = false){
        return BlocksMovement(pos.x, pos.y, ignoreActor);
    }

    public bool BlocksMovement(int x, int y, bool ignoreActor = false){
        if (!ValidPosition(x, y)){
            return true;
        }

        return Cells[y,x].BlocksMovement(ignoreActor);
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

    public DR_Cell GetCell(Vector2Int pos){
        if (!ValidPosition(pos)){
            return null;
        }

        return Cells[pos.y, pos.x];
    }

    public DR_Entity GetItemAtPosition(Vector2Int pos){
        if (!ValidPosition(pos)){
            return null;
        }

        return Cells[pos.y, pos.x].Item;
    }

    public DR_Entity GetActorAtPosition(Vector2Int pos){
        if (!ValidPosition(pos)){
            return null;
        }

        return Cells[pos.y, pos.x].Actor;
    }
}
