using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class which creates and stores the current generation progress
public class MapLayoutGen{
    private DungeonGenInfo dungeonInfo;
    private bool isDone = false;

    public MapLayoutGen(DungeonGenInfo dungeonInfo){
        this.dungeonInfo = dungeonInfo;
    }

    public void Step(){
        // Do one step of layout generation so that progress can be visualized
    }

    public bool IsDone(){
        return isDone;
    }
}
