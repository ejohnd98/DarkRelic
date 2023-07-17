using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classes/Enums to be used by DR_MapGen (in their own file for readability)

public class MapGenInfo{
    public Vector2Int MapSize;

    public MapGenInfo(Vector2Int MapSize){
        this.MapSize = MapSize;
    }
    // Possible parameters:
    // - type of generator (cave, ruins, castle, etc)
    // - required rooms (min/max room size)
    // - margins between rooms?
    // - loot? enemies?
}

public enum MapGenCellType{
    NOT_SET,
    WALL,
    FLOOR,
    DOOR,
    ENEMY,
    STAIRS_UP,
    STAIRS_DOWN
}

public enum MapGenState{
    NOT_STARTED,
    PLACING_ROOMS,
    PLACING_TUNNELS,
    PLACING_PROPS,
    FINISHED
}

public class MapGenCell{
    //replace with something more robust:
    public MapGenCellType type;

    public MapGenCell(MapGenCellType cellType){
        type = cellType;
    }

    //TODO: link this up to room array when placing a room
    public int associatedRoomIndex = -1;
}


public class MapGenRoom{

    public Vector2Int pos, size;

    public MapGenRoom(Vector2Int pos, Vector2Int size, int id){
        this.pos = pos;
        this.size = size;
        this.roomId = id;
    }

    public int roomId = -1;
}