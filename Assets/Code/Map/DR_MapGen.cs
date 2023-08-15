using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class which creates and stores the current generation progress
public class MapGeneration{

    public MapGenState state = MapGenState.NOT_STARTED;
    public MapGenCell[,] cells;
    public List<MapGenRoom> rooms;
    public Vector2Int mapSize;

    int placedRooms = 0;

    //temp
    int tunnellingIndex = 0;

    public MapGeneration(Vector2Int size){
        mapSize = size;
        cells = new MapGenCell[size.y,size.x];
        for (int y = 0; y < mapSize.y; y++){
            for (int x = 0; x < mapSize.x; x++){
                MapGenCell cell = new MapGenCell(MapGenCellType.NOT_SET);
                cells[y,x] = cell;
            }
        }

        rooms = new List<MapGenRoom>();
    }

    // perform one step of the generation (for aid in debugging generation progress)
    public void Step(){
        switch (state){
            case MapGenState.NOT_STARTED:{
                state = MapGenState.PLACING_ROOMS;
                break;
            }
            case MapGenState.PLACING_ROOMS:{
                int attempts = 0;
                bool hasPlacedRoom = false;
                while (!hasPlacedRoom && attempts < 6){
                    hasPlacedRoom = PlaceRoom();
                    attempts++;
                }
                
                if (!hasPlacedRoom){
                    state = MapGenState.PLACING_TUNNELS;
                }
                break;
            }
            case MapGenState.PLACING_TUNNELS:{
                //TODO create function to connect any two rooms
                if (tunnellingIndex + 1 >= rooms.Count){
                    state = MapGenState.PLACING_PROPS;
                    break;
                }
                MapGenRoom roomA = rooms[tunnellingIndex];
                MapGenRoom roomB = rooms[tunnellingIndex+1];
                Vector2Int posA = roomA.pos + roomA.size/2;
                Vector2Int posB = roomB.pos + roomB.size/2;

                int x = posA.x;
                int y = posA.y;
                while(x != posB.x){
                    int xd = (int)Mathf.Sign(posB.x - x);
                    cells[y,x].type = MapGenCellType.FLOOR;
                    x += xd;
                }

                while(y != posB.y){
                    int yd = (int)Mathf.Sign(posB.y - y);
                    cells[y,x].type = MapGenCellType.FLOOR;
                    y += yd;
                }

                tunnellingIndex++;

                break;
            }
            case MapGenState.PLACING_PROPS:{
                MapGenRoom startRoom = rooms[0];
                Vector2Int startPos = startRoom.pos + startRoom.size/2;

                float roomDist = 0.0f;
                int endRoomIndex = rooms.Count-1;
                for (int i=1; i < rooms.Count; i++){
                    float newDist = Vector2Int.Distance(startPos, rooms[i].pos);
                    if (newDist > roomDist){
                        roomDist = newDist;
                        endRoomIndex = i;
                    }
                }

                MapGenRoom endRoom = rooms[endRoomIndex];
                Vector2Int endPos = endRoom.pos + endRoom.size/2;

                cells[startPos.y, startPos.x].type = MapGenCellType.STAIRS_UP;
                cells[endPos.y, endPos.x].type = MapGenCellType.STAIRS_DOWN;

                // doors
                for (int y = 0; y < mapSize.y; y++){
                    for (int x = 0; x < mapSize.x; x++){
                        if (cells[y,x].type != MapGenCellType.FLOOR){
                            continue;
                        }
                        //randomly don't place some doors
                        if (Random.Range(0.0f, 1.0f) > 0.95f){
                            continue;
                        }
                        if (CanPlaceDoor(x,y)){
                            cells[y,x].type = MapGenCellType.DOOR;
                        }
                    }
                }

                state = MapGenState.PLACING_ENEMIES;

                break;
            }
            case MapGenState.PLACING_ENEMIES: {
                //TODO: do anything but this:
                for (int i=1; i < rooms.Count; i++){
                    int enemyCount = ((1 + i) / 3); //spawn more enemies in later rooms
                    if (rooms[i].size.x * rooms[i].size.y > 25 && enemyCount < 3){
                        enemyCount++;
                    }

                    if (rooms[i].size.x * rooms[i].size.y < 16 && enemyCount > 1){
                        enemyCount = 1;
                    }

                    int placedEnemies = 0;
                    while (placedEnemies < enemyCount){
                        int x = rooms[i].pos.x + Random.Range(1, rooms[i].size.x-1);
                        int y = rooms[i].pos.y + Random.Range(1, rooms[i].size.y-1);

                        // messy retry
                        int attempts = 10;
                        while (cells[y,x].type != MapGenCellType.FLOOR && attempts-- > 0){
                            x = rooms[i].pos.x + Random.Range(1, rooms[i].size.x-1);
                            y = rooms[i].pos.y + Random.Range(1, rooms[i].size.y-1);
                        }
                        if (cells[y,x].type == MapGenCellType.FLOOR){
                            cells[y,x].type = MapGenCellType.ENEMY;
                        }
                        //increment regardless to avoid infinite loops
                        placedEnemies++;
                    }
                }

                state = MapGenState.PLACING_ITEMS;
                break;
            }
            case MapGenState.PLACING_ITEMS: {
                //TODO: do anything but this:
                for (int i=1; i < rooms.Count; i++){
                    int itemCount = Random.Range(0, 2);

                    int placedItems = 0;
                    while (placedItems < itemCount){
                        int x = rooms[i].pos.x + Random.Range(1, rooms[i].size.x-1);
                        int y = rooms[i].pos.y + Random.Range(1, rooms[i].size.y-1);

                        // messy retry
                        int attempts = 10;
                        while (cells[y,x].type != MapGenCellType.FLOOR && attempts-- > 0){
                            x = rooms[i].pos.x + Random.Range(1, rooms[i].size.x-1);
                            y = rooms[i].pos.y + Random.Range(1, rooms[i].size.y-1);
                        }
                        if (cells[y,x].type == MapGenCellType.FLOOR){
                            cells[y,x].type = MapGenCellType.ITEM;
                        }
                        //increment regardless to avoid infinite loops
                        placedItems++;
                    }
                }

                state = MapGenState.FINISHED;
                break;
            }
            case MapGenState.FINISHED:{
                break;
            }
            default:
            break;
        }

        //if failed to place rooms 10 times, then move on to tunneling?
    }

    bool CanPlaceDoor(int x, int y){
        int floorCount = GetAdjacentTilesOfType(x, y, MapGenCellType.FLOOR);
        
        if (floorCount >= 4 && floorCount <= 6 && InGateway(x,y)){
            return true;
        }
        return false;
    }

    bool InGateway(int x, int y){
        int wallCount = GetDirectlyAdjacentTilesOfType(x, y, MapGenCellType.WALL) + GetDirectlyAdjacentTilesOfType(x, y, MapGenCellType.NOT_SET);
        if (wallCount != 2){
            return false;
        }
        if ((IsCellOfType(x+1,y, MapGenCellType.WALL) || IsCellOfType(x+1,y, MapGenCellType.NOT_SET))
        && (IsCellOfType(x-1,y, MapGenCellType.WALL) || (IsCellOfType(x-1,y, MapGenCellType.NOT_SET)))){
            return true;
        }

        if ((IsCellOfType(x,y+1, MapGenCellType.WALL) || IsCellOfType(x,y+1, MapGenCellType.NOT_SET))
        && (IsCellOfType(x,y-1, MapGenCellType.WALL) || (IsCellOfType(x,y-1, MapGenCellType.NOT_SET)))){
            return true;
        }

        return false;

    }

    bool IsCellOfType(int x, int y, MapGenCellType type){
        if (x < 0 || y < 0 || x > mapSize.x-1 || y > mapSize.y-1){
            return false;
        }
        return cells[y,x].type == type;
    }

    int GetAdjacentTilesOfType(int x, int y, MapGenCellType type){
        int count = 0;
        for (int yd = -1; yd < 2; yd++){
            for (int xd = -1; xd < 2; xd++){
                if ((yd == 0 && xd == 0) || x+xd < 0 || y+yd < 0 || x+xd > mapSize.x-1 || y+yd > mapSize.y-1){
                    continue;
                }
                
                if (IsCellOfType(x+xd, y+yd, type)){
                    count++;
                }
            }
        }
        return count;
    }

    int GetDirectlyAdjacentTilesOfType(int x, int y, MapGenCellType type){
        int count = 0;
        Vector2Int[] Directions = {Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right};
        for (int i = 0; i < 4; i++){
            Vector2Int dir = Directions[i];
            if (x+dir.x < 0 || y+dir.y < 0 || x+dir.x > mapSize.x-1 || y+dir.y > mapSize.y-1){
                continue;
            }
            if (IsCellOfType(x+dir.x, y+dir.y, type)){
                count++;
            }
        }
        return count;
    }

    //TODO: make randomness deterministic
    //TODO: return roominfo (size, position, connected rooms?)
    bool PlaceRoom(){
        int width = Random.Range(5, 11);
        int height = Random.Range(5, 12);
        int x = Random.Range(1, mapSize.x-1-width);
        int y = Random.Range(1, mapSize.y-1-height);

        if (!AreCellsUnset(x, x+width, y, y+height)){
            return false;
        }

        PlaceCellType(x, x+width, y, y+height, MapGenCellType.WALL);
        PlaceCellType(x+1, x+width-1, y+1, y+height-1, MapGenCellType.FLOOR);
        
        placedRooms++;
        MapGenRoom newRoom = new MapGenRoom(new Vector2Int(x,y), new Vector2Int(width, height), placedRooms);
        rooms.Add(newRoom);

        return true;
    }

    void PlaceCellType(int x1, int x2, int y1, int y2, MapGenCellType type){
        for (int y = y1; y < y2; y++){
            for (int x = x1; x < x2; x++){
                cells[y,x].type = type;
            }
        }
    }

    bool AreCellsUnset(int x1, int x2, int y1, int y2){
        for (int y = y1; y < y2; y++){
            for (int x = x1; x < x2; x++){
                if (cells[y,x].type != MapGenCellType.NOT_SET){
                    return false;
                }
            }
        }
        return true;
    }
}

public class DR_MapGen
{
    public static DR_Map CreateMapFromMapInfo(MapGenInfo mapGenInfo){
        //TODO: visualize this
        MapGeneration mapGen = new MapGeneration(mapGenInfo.MapSize);
        while (mapGen.state != MapGenState.FINISHED){
            mapGen.Step();
        }

        return CreateMapFromGeneration(mapGen);
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
                    DR_Entity door = EntityFactory.CreateDoor(gm.OpenDoorTexture, gm.ClosedDoorTexture);
                    NewMap.AddProp(door, new Vector2Int(x,y));
                }

                bool isStairsDeeper = color.r > 0.9f && color.g < 0.1f && color.b < 0.1f;
                bool isStairsShallower= color.r < 0.1f && color.g < 0.1f && color.b > 0.9f;
                if (isStairsDeeper || isStairsShallower){
                    DR_GameManager gm = DR_GameManager.instance;
                    DR_Entity stairs = EntityFactory.CreateStairs(isStairsDeeper? gm.StairsDownTexture : gm.StairsUpTexture, isStairsDeeper);
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

        // TODO: move this into constructor?
        NewMap.MapSize = size;
        NewMap.Cells = new DR_Cell[size.y,size.x];
        NewMap.IsVisible = new bool[size.y,size.x];
        NewMap.IsKnown = new bool[size.y,size.x];
        NewMap.Entities = new List<DR_Entity>();

        return NewMap;
    }

    private static DR_Map CreateMapFromGeneration (MapGeneration generation){
        DR_Map NewMap = CreateEmptyMap(generation.mapSize);
        DR_GameManager gm = DR_GameManager.instance;

        for (int y = 0; y < NewMap.MapSize.y; y++){
            for (int x = 0; x < NewMap.MapSize.x; x++){
                DR_Cell newCell = new DR_Cell();
                NewMap.Cells[y,x] = newCell;
                switch (generation.cells[y,x].type){
                    case MapGenCellType.FLOOR:
                        newCell.bBlocksMovement = false;
                        break;
                    case MapGenCellType.DOOR:
                        newCell.bBlocksMovement = false;
                        DR_Entity door = EntityFactory.CreateDoor(gm.OpenDoorTexture, gm.ClosedDoorTexture);
                        NewMap.AddProp(door, new Vector2Int(x,y));
                        break;
                    case MapGenCellType.ENEMY:
                        newCell.bBlocksMovement = false;
                        DR_Entity enemy = EntityFactory.CreateActor(gm.EnemyTexture, "Generic Enemy", 2, Alignment.ENEMY);
                        NewMap.AddActor(enemy, new Vector2Int(x,y));
                        break;
                    case MapGenCellType.ITEM:
                        newCell.bBlocksMovement = false;
                        //very temp item generation:
                        DR_Item item = null;
                        int itemIndex = Random.Range(0, 2);
                        switch(itemIndex){
                            case 0:
                                item = EntityFactory.CreateHealingItem(gm.PotionTexture, "Health Potion", 4);
                                break;
                            case 1:
                                item = EntityFactory.CreateMagicItem(gm.MagicScrollTexture, "Magic Scroll", 5);
                                break;
                            default:
                                break;
                        }
                        NewMap.AddItem(item, new Vector2Int(x,y));
                        break;
                    case MapGenCellType.STAIRS_UP:{
                        DR_Entity stairs = EntityFactory.CreateStairs(gm.StairsUpTexture, false);
                        NewMap.AddProp(stairs, new Vector2Int(x,y));
                        break;
                    }
                    case MapGenCellType.STAIRS_DOWN:{
                        DR_Entity stairs = EntityFactory.CreateStairs(gm.StairsDownTexture, true);
                        NewMap.AddProp(stairs, new Vector2Int(x,y));
                        break;
                    }
                    case MapGenCellType.NOT_SET:
                    case MapGenCellType.WALL:
                    default:
                        newCell.bBlocksMovement = true;
                    break;
                }
            }
        }

        //TODO: transfer stuff to NewMap from the generation
        // Create entities when needed (doors, stairs)

        return NewMap;
    }
}
