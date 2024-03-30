using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator {
    public static DR_Dungeon GenerateDungeon() {
        DungeonGenInfo dungeonGenInfo = new DungeonGenInfo();
        DR_Dungeon dungeon = new DR_Dungeon();
        dungeon.name = "Balance Test Dungeon";

        for (int i = 0; i < dungeonGenInfo.floors; i++) {
            //calculate exp per enemy from i and dungeonGenInfo.levelIncreasePerFloor
            //int expPerRoom = Mathf.RoundToInt(expectedFloorExperience / (float)dungeonGenInfo.roomsOnShortPath);
            dungeon.maps.Add(GenerateMap(dungeonGenInfo, dungeon, i));
        }

        return dungeon;
    }

    public static DR_Map GenerateMap(DungeonGenInfo dungeonGenInfo, DR_Dungeon dungeon, int depth) {
        Vector2Int mapSize = dungeonGenInfo.getFloorSize(depth);
        MapBlueprint mapBlueprint = new MapBlueprint(mapSize);
        DR_GameManager gm = DR_GameManager.instance;
        // Create map layout here
        
        //VERY TEMP
        for (int i = 0; i < 5; i++) {
            Vector2Int roomPos = new Vector2Int(8, (i * 6));
            Vector2Int roomSize = new Vector2Int(7, 7);
            MapGenRoom room = new MapGenRoom(roomPos, roomSize);
            mapBlueprint.rooms.Add(room);
            mapBlueprint.AssignRoomToCells(room);
            
            mapBlueprint.PlaceCellType(
                roomPos.x, roomPos.x+roomSize.x, 
                roomPos.y, roomPos.y+roomSize.y, 
                MapGenCellType.WALL);
            mapBlueprint.PlaceCellType(
                roomPos.x+1, roomPos.x+roomSize.x-1, 
                roomPos.y+1, roomPos.y+roomSize.y-1, 
                MapGenCellType.FLOOR);

            if (i != 0) {
                mapBlueprint.cells[roomPos.y, 11].type = MapGenCellType.DOOR;
                mapBlueprint.cells[roomPos.y -1, 11].type = MapGenCellType.FLOOR;
            }
        }
        
        mapBlueprint.cells[2, 11].type = MapGenCellType.STAIRS_UP;
        mapBlueprint.cells[28, 11].type = (depth == dungeonGenInfo.floors-1) ? MapGenCellType.GOAL : MapGenCellType.STAIRS_DOWN;

        // Create stairs + door entities
        for (int y = 0; y < mapSize.y; y++) {
            for (int x = 0; x < mapSize.x; x++) {

                DR_Entity newEntity = null;
                
                switch (mapBlueprint.cells[y, x].type) {
                    case MapGenCellType.DOOR:
                        newEntity = EntityFactory.CreateDoor(gm.OpenDoorTexture, gm.ClosedDoorTexture);
                        break;
                    case MapGenCellType.STAIRS_UP:
                        newEntity = EntityFactory.CreateStairs(gm.StairsUpTexture, false);
                        break;
                    case MapGenCellType.STAIRS_DOWN:
                        newEntity = EntityFactory.CreateStairs(gm.StairsDownTexture, true);
                        break;
                    case MapGenCellType.GOAL:
                        newEntity = EntityFactory.CreateGoal(gm.GoalTexture);
                        break;
                }

                if (newEntity != null) {
                    mapBlueprint.entitiesToPlace.Add(new Vector2Int(x, y), newEntity);
                }
            }
        }

        // Create enemy entities
        int experienceBudget = dungeonGenInfo.getExpectedExperience(depth+1);
        
        // Later: select enemies based on level, floor theme, etc
        List<Content> floorEnemies = gm.enemyContentArray;
        
        // Figure out when we stop spawning enemies
        int lowestExpEnemy = experienceBudget+1;
        foreach (var enemy in floorEnemies) {
            foreach (var comp in enemy.components) {
                if (comp is LevelComponent levelComponent) {
                    lowestExpEnemy = LevelComponent.GetLevelStats(dungeonGenInfo.getFloorEnemyLevel(depth+1), levelComponent).expGiven;
                }
            }
        }
        
        // while (experienceBudget >= lowestExpEnemy) {
        //     break;
        // }
        

        // Create item entities
        

        DR_Map newMap = CreateMapFromBlueprint(mapBlueprint);

        return newMap;
    }

    public static DR_Map CreateMapFromBlueprint(MapBlueprint mapBlueprint) {
        DR_Map newMap = new DR_Map(mapBlueprint.mapSize);
        DR_GameManager gm = DR_GameManager.instance;

        for (int y = 0; y < newMap.MapSize.y; y++) {
            for (int x = 0; x < newMap.MapSize.x; x++) {
                DR_Cell newCell = new DR_Cell();
                newMap.Cells[y, x] = newCell;

                switch (mapBlueprint.cells[y, x].type) {
                    case MapGenCellType.NOT_SET:
                    case MapGenCellType.WALL: {
                        newCell.bBlocksMovement = true;
                        break;
                    }
                    case MapGenCellType.FLOOR:
                    case MapGenCellType.DOOR:
                    case MapGenCellType.STAIRS_UP:
                    case MapGenCellType.STAIRS_DOWN:
                    default: {
                        newCell.bBlocksMovement = false;
                        break;
                    }
                }
            }
        }

        foreach (var posEntityPair in mapBlueprint.entitiesToPlace) {
            var success = false;
            if (posEntityPair.Value.HasComponent<PropComponent>()) {
                success = newMap.AddProp(posEntityPair.Value, posEntityPair.Key);
            }
            else if (posEntityPair.Value.HasComponent<ItemComponent>()) {
                success = newMap.AddItem(posEntityPair.Value, posEntityPair.Key);
            }
            else {
                success = newMap.AddActor(posEntityPair.Value, posEntityPair.Key);
            }

            if (!success) {
                Debug.LogError("CreateMapFromBlueprint: unable to place " + posEntityPair.Value.Name + " at " +
                               posEntityPair.Key);
            }
        }

        return newMap;
    }
}

public class MapBlueprint {
    public Vector2Int mapSize;
    public MapGenCell[,] cells;
    public Dictionary<Vector2Int, DR_Entity> entitiesToPlace;
    public List<MapGenRoom> rooms;

    public MapBlueprint(Vector2Int size) {
        mapSize = size;
        cells = new MapGenCell[size.y, size.x];
        for (int y = 0; y < mapSize.y; y++) {
            for (int x = 0; x < mapSize.x; x++) {
                MapGenCell cell = new MapGenCell(MapGenCellType.NOT_SET);
                cells[y, x] = cell;
            }
        }

        entitiesToPlace = new();
        rooms = new();
    }

    public void AssignRoomToCells(MapGenRoom room) {
        int x1 = room.pos.x + 1;
        int x2 = room.pos.x + room.size.x - 1;
        int y1 = room.pos.y + 1;
        int y2 = room.pos.y + room.size.y - 1;
        for (int y = y1; y < y2; y++){
            for (int x = x1; x < x2; x++){
                cells[y,x].associatedRoom = room;
            }
        }
    }
    
    public void PlaceCellType(int x1, int x2, int y1, int y2, MapGenCellType type){
        for (int y = y1; y < y2; y++){
            for (int x = x1; x < x2; x++){
                cells[y,x].type = type;
            }
        }
    }
}