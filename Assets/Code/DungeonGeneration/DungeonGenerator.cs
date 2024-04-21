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
            MapGenRoom room = new MapGenRoom(roomPos, roomSize, mapBlueprint);
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

        mapBlueprint.GetCell(mapBlueprint.rooms[0].GetCenterPosition()).type = MapGenCellType.STAIRS_UP;
        mapBlueprint.GetCell(mapBlueprint.rooms[^1].GetCenterPosition()).type = (depth == dungeonGenInfo.floors-1) ? MapGenCellType.GOAL : MapGenCellType.STAIRS_DOWN;

        mapBlueprint.GetCell(mapBlueprint.rooms[2].GetCenterPosition()).type = MapGenCellType.ALTAR;
        mapBlueprint.GetCell(mapBlueprint.rooms[^1].GetCenterPosition() + Vector2Int.up).type = MapGenCellType.ALTAR;
        mapBlueprint.GetCell(mapBlueprint.rooms[^1].GetCenterPosition() + Vector2Int.up * 3).type = MapGenCellType.ITEM;
        mapBlueprint.GetCell(mapBlueprint.rooms[^1].GetCenterPosition() + Vector2Int.right * 3).type = MapGenCellType.ITEM;
        mapBlueprint.GetCell(mapBlueprint.rooms[^1].GetCenterPosition() + Vector2Int.left * 3).type = MapGenCellType.ITEM;
        

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
                    case MapGenCellType.ALTAR:
                        newEntity = EntityFactory.CreateEntityFromContent(gm.altarContent);
                        break;
                    // Temporarily do this here
                    case MapGenCellType.ITEM:
                        Vector2Int itemPos = new Vector2Int(x,y);
                        int itemIndex = Random.Range(0, gm.relicPickupContentArray.Count);
                        var item = EntityFactory.CreateEntityFromContent(gm.relicPickupContentArray[itemIndex]);
                        mapBlueprint.entitiesToPlace.Add(itemPos, item);
                        break;
                }

                if (newEntity != null) {
                    mapBlueprint.entitiesToPlace.Add(new Vector2Int(x, y), newEntity);
                }
            }
        }

        // Create enemy entities
        int experienceBudget = dungeonGenInfo.getExpectedExperience(depth);
        
        // TODO: select enemies based on level, floor theme, etc
        List<Content> floorEnemies = gm.enemyContentArray;
        
        // Figure out when we stop spawning enemies
        int lowestExpEnemy = experienceBudget+1;
        Content lowestExpEnemyContent = null;
        foreach (var enemy in floorEnemies) {
            foreach (var comp in enemy.components) {
                if (comp is LevelComponent levelComponent) {
                    int expGiven = LevelComponent.GetLevelStats(dungeonGenInfo.getFloorEnemyLevel(depth), levelComponent).expGiven;
                    if (expGiven < lowestExpEnemy) {
                        lowestExpEnemy = expGiven;
                        lowestExpEnemyContent = enemy;
                    }
                }
            }
        }

        int roomIndex = mapBlueprint.rooms.Count - 1;
        int failedAttempts = 0;
        while (experienceBudget > 0 && failedAttempts < 10) {

            Content chosenEnemy = lowestExpEnemyContent;

            // TODO: more properly choose enemy to use based on available budget
            if (experienceBudget > lowestExpEnemy) {
                // Choose enemy type
                int chosenIndex = Random.Range(0, floorEnemies.Count);
                chosenEnemy = floorEnemies[chosenIndex];
            }
            
            // Create enemy and set level
            DR_Entity enemy = EntityFactory.CreateEntityFromContent(chosenEnemy);
            enemy.GetComponent<LevelComponent>().level = dungeonGenInfo.getFloorEnemyLevel(depth);
            enemy.GetComponent<LevelComponent>().UpdateStats();

            // Determine spawn position
            Vector2Int enemyPos = mapBlueprint.rooms[roomIndex].ReserveEnemyPosition();
            if (enemyPos == -Vector2Int.one) {
                Debug.LogError("ReserveEnemyPosition could not determine enemy position");
                failedAttempts++;
                continue;
            }
            mapBlueprint.entitiesToPlace.Add(enemyPos, enemy);

            // Decrement room index, subtract exp cost
            experienceBudget -= enemy.GetComponent<LevelComponent>().stats.expGiven;
            if (--roomIndex < 0) { 
                roomIndex = mapBlueprint.rooms.Count - 1;
            }
        }
        
        Debug.Log("Floor " + (depth + 1) + ": leftover budget " + experienceBudget + "/" + dungeonGenInfo.getExpectedExperience(depth+1) + ". lowest exp enemy is " + lowestExpEnemy);
        

        // TODO: Create item entities here later
        

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

    public MapGenCell GetCell(Vector2Int pos) {
        return cells[pos.y, pos.x];
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