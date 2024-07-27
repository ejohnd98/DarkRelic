using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

// Classes/Enums to be used by DR_MapGen (in their own file for readability)

public class MapGenInfo{
    public Vector2Int MapSize;
    public int depth = 1;

    public MapGenInfo(Vector2Int MapSize, int depth){
        this.MapSize = MapSize;
        this.depth = depth;
    }

    public bool isLastFloor = false;
    // Possible parameters:
    // - type of generator (cave, ruins, castle, etc)
    // - required rooms (min/max room size)
    // - margins between rooms?
    // - loot? enemies?
}

public class DungeonGenInfo
{
    public Vector2Int mapSize;
    public int floors = 5;
    public int itemsPerFloor = 3;
    public int roomsOnShortPath = 5;
    public int startingLevel = 5;
    public int levelIncreasePerFloor = 2;

    public int getExpectedExperience(int depth) {
        int startLevel = startingLevel + (levelIncreasePerFloor * depth);
        int endLevel = startLevel + levelIncreasePerFloor;

        int expRequired = 0;
        for (int i = startLevel; i < endLevel; i++)
        {
            expRequired += LevelComponent.GetRequiredExpForLevelUp(i);
        }

        return expRequired;
    }

    public int getFloorEnemyLevel(int depth) {
        return startingLevel + (levelIncreasePerFloor * depth);
    }

    public Vector2Int getFloorSize(int floor)
    {
        // temp hardcoded value
        return new Vector2Int(90, 90);
    }
}

public enum MapGenCellType{
    NOT_SET,
    WALL,
    FLOOR,
    DOOR,
    ENEMY,
    ITEM,
    STAIRS_UP,
    STAIRS_DOWN,
    HEALTH_ALTAR,
    ITEM_ALTAR,
    GOAL
}

//TODO: create base class to represent a state, then can avoid defining every state in this enum
// plus, this allows different generation types to have different sets of stages (ie. cave may have an erosion stage)
public enum MapGenState{
    NOT_STARTED,
    PLACING_ROOMS,
    PLACING_TUNNELS,
    PLACING_PROPS,
    PLACING_ENEMIES,
    PLACING_ITEMS,
    FINISHED
}

public class MapGenCell{
    //replace with something more robust:
    public MapGenCellType type;

    public MapGenCell(MapGenCellType cellType){
        type = cellType;
    }

    public MapGenRoom associatedRoom;
}

//TODO: possibly expand this into base class + implementations for any data needed alongside tag?
public enum RoomTag{
    START,
    END,

    NONE
}

public class MapGenRoom{

    public Vector2Int pos, size;
    public MapBlueprint mapBlueprint;
    public string roomLabel = "";
    public RoomTag roomTag = RoomTag.NONE;

    public MapGenRoom(Vector2Int pos, Vector2Int size, MapBlueprint mapBlueprint){
        this.pos = pos;
        this.size = size;
        this.mapBlueprint = mapBlueprint;
    }

    private static readonly Vector2Int[] possibleEnemyPositions = new Vector2Int[] {
        new(1,1),
        new(-1,-1),
        new(-1,1),
        new(1,-1),
        //new(0,1),
        new(0,-1),
        new(-1,0),
        new(1,-0)
    };

    public Vector2Int GetCenterPosition() {
        return pos + new Vector2Int((Mathf.FloorToInt(size.x * 0.5f)), Mathf.FloorToInt(size.y * 0.5f));
    }

    public bool IsPositionInsideRoom(Vector2Int testPos){
        return testPos.x >= pos.x && testPos.x < pos.x + size.x
            && testPos.y >= pos.y && testPos.y < pos.y + size.y;
    }

    public Vector2Int ReserveEnemyPosition() {
        // Could predetermine what spots to use? (such as defining those in a prefab image)
        foreach (Vector2Int offset in possibleEnemyPositions) {
            Vector2Int potentialPos = pos + offset + new Vector2Int((Mathf.FloorToInt(size.x * 0.5f)), Mathf.FloorToInt(size.y * 0.5f));
            if (mapBlueprint.cells[potentialPos.y, potentialPos.x].type == MapGenCellType.FLOOR) {
                mapBlueprint.cells[potentialPos.y, potentialPos.x].type = MapGenCellType.ENEMY;
                return potentialPos;
            }
        }
        return -Vector2Int.one;
    }

    public bool IsCorner(Vector2Int testPos){
        Vector2Int lowerCorner = pos;
        Vector2Int upperCorner = pos + size - Vector2Int.one;
        return testPos.Equals(lowerCorner) 
            || testPos.Equals(upperCorner) 
            || (testPos.x == lowerCorner.x && testPos.y == upperCorner.y) 
            || (testPos.x == upperCorner.x && testPos.y == lowerCorner.y);
    }

    public Vector2Int GetEdgePositionAtDir(Vector2 roomDiff, bool avoidCorners = true){
        Vector2 center = GetCenterPosition();
        Vector2 edgePos = center;
        Vector2 dir = roomDiff.normalized; 
        Vector2Int result = Vector2Int.RoundToInt(edgePos);

        while(IsPositionInsideRoom(Vector2Int.RoundToInt(edgePos + dir))){
            edgePos += dir;
            result = Vector2Int.RoundToInt(edgePos);
        }

        if(avoidCorners && IsCorner(result)){
            result.x -= (int)Mathf.Sign(dir.x);
            //result.y -= (int)Mathf.Sign(dir.y);
        }

        return result;
    }

    public bool IsValid(){
        bool isValid = pos.x > 0 && pos.x + size.x < mapBlueprint.mapSize.x
            && pos.y > 0 && pos.y + size.y < mapBlueprint.mapSize.y;
        
        if (!isValid){
            Debug.LogError("Room is outside map bounds!");
        }
        
        return isValid;
    }
}

public class MapLayoutNode{
    public Vector2Int position;
    public Vector2Int size = Vector2Int.one;
    public RoomTag roomTag = RoomTag.NONE;
    public string label = "Node";

    public MapGenRoom resultingRoom = null;
}