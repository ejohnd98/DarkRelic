using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Dungeon
{
    public List<DR_Map> maps;
    public int mapIndex = 0;
    public string name = "Untitled Dungeon";

    public DR_Dungeon(){
        maps = new List<DR_Map>();
    }

    public DR_Map GetCurrentMap(){
        return maps[mapIndex];
    }

    public int GetFloorCount(){
        return maps.Count;
    }

    public bool HasNextMap(bool deeper){
        if ((deeper && mapIndex == maps.Count - 1) || !deeper && mapIndex == 0){
            return false;
        }
        return true;
    }

    public void SetNextMap(bool deeper){
        if (HasNextMap(deeper)){
            mapIndex += (deeper ? 1 : -1);
        }
    }

    public DR_Map GetNextMap(bool deeper){
        if (HasNextMap(deeper)){
            return maps[mapIndex + (deeper ? 1 : -1)];
        }
        return GetCurrentMap();
    }
}
