using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

    public delegate void DungeonGeneratedCallback(DR_Dungeon dungeon);
    public delegate void MapGeneratedCallback(DR_Map map);

    public bool visualizeGeneration = true;

    private MapBlueprint visualizationTarget = null; 
    private MapLayout visualizationLayoutTarget = null; 
    private List<GameObject> visualizationObjects = new();

    public GraphGenerator graphGenerator;

    public Transform visualizationParent;
    public SpriteRenderer visualizationPrefab;
    public Sprite roomSpr;
    public Sprite wallSpr, floorSpr, notSetSpr;
    private const float maxVisualizationHeight = 22.0f;

    private float visualizationSpeedMod = 1.0f;

    public void GenerateDungeon(DungeonGeneratedCallback callback) {
        StartCoroutine(GenerateDungeonCoroutine(callback));
    }

    void Update()
    {
        visualizationSpeedMod = visualizeGeneration ? (Input.GetKey(KeyCode.Space)? 0.1f : 0.5f) : 0.0f;
    }


    private IEnumerator VisualizationCoroutine(){
        while(true){
            VisualizeGeneration();
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    private void ClearVisualization(){
        foreach (var obj in visualizationObjects) {
            Destroy(obj);
        }
        visualizationObjects.Clear();
    }

    private void VisualizeGeneration(){
        if (visualizationTarget == null && visualizationLayoutTarget == null){
            return;
        }

        ClearVisualization();

        if (visualizationTarget != null){
            for (int y = 0; y < visualizationTarget.mapSize.y; y++) {
                for (int x = 0; x < visualizationTarget.mapSize.x; x++) {

                    switch (visualizationTarget.cells[y, x].type) {
                        case MapGenCellType.NOT_SET:{
                            //CreateSpriteAt(new Vector2Int(x,y), notSetSpr, 0.0f);
                            break;
                        }
                        case MapGenCellType.WALL: {
                            CreateSpriteAt(new Vector2Int(x,y), wallSpr, 0.0f);
                            break;
                        }
                        default: {
                            CreateSpriteAt(new Vector2Int(x,y), floorSpr, 0.0f);
                            break;
                        }
                    }
                }
            }

            foreach (var posEntityPair in visualizationTarget.entitiesToPlace) {
                CreateSpriteAt(posEntityPair.Key, posEntityPair.Value.GetComponent<SpriteComponent>().GetCurrentSprite(), -1.0f);
            }

            visualizationParent.localScale = Vector3.Min(Vector3.one, Vector3.one * (maxVisualizationHeight / visualizationTarget.mapSize.y));
        }
        else if (visualizationLayoutTarget != null){
            foreach (var node in visualizationLayoutTarget.nodes){
                DrawLayoutNode(node);
            }

            visualizationParent.localScale = Vector3.Min(Vector3.one, Vector3.one * (maxVisualizationHeight / visualizationLayoutTarget.mapSize.y));
        }
        
    }

    private void DrawLayoutNode(MapLayoutNode node){
        Vector3 offset = new Vector3(-visualizationLayoutTarget.mapSize.x / 2.0f, -visualizationLayoutTarget.mapSize.y / 2.0f, -10);
        offset += new Vector3(node.size.x, node.size.y, 0) * 0.5f;
        SpriteRenderer spriteRenderer = Instantiate(visualizationPrefab, visualizationParent);
        spriteRenderer.sprite = roomSpr;
        spriteRenderer.transform.localPosition = offset + new Vector3(node.position.x, node.position.y, 0.0f);
        spriteRenderer.transform.localScale = node.size - (Vector2.one*2);
        visualizationObjects.Add(spriteRenderer.gameObject);
    }

    private void CreateSpriteAt(Vector2Int pos, Sprite spr, float height){
        Vector3 offset = new Vector3(-visualizationTarget.mapSize.x / 2.0f, -visualizationTarget.mapSize.y / 2.0f, -10);


        SpriteRenderer spriteRenderer = Instantiate(visualizationPrefab, visualizationParent);
        spriteRenderer.sprite = spr;
        spriteRenderer.transform.localPosition = offset + new Vector3(pos.x, pos.y, height);
        visualizationObjects.Add(spriteRenderer.gameObject);
    }

    IEnumerator GenerateDungeonCoroutine(DungeonGeneratedCallback callback){
        DungeonGenInfo dungeonGenInfo = new DungeonGenInfo();
        DR_Dungeon dungeon = new DR_Dungeon
        {
            name = "Balance Test Dungeon"
        };

        for (int i = 0; i < dungeonGenInfo.floors; i++) {
            //calculate exp per enemy from i and dungeonGenInfo.levelIncreasePerFloor
            //int expPerRoom = Mathf.RoundToInt(expectedFloorExperience / (float)dungeonGenInfo.roomsOnShortPath);
            bool generatingMap = true; // Is this really okay?
            StartCoroutine(GenerateMap(dungeonGenInfo, dungeon, i, (DR_Map map) => {
                dungeon.maps.Add(map);
                generatingMap = false;
            }));
            
            yield return new WaitUntil(() => !generatingMap);
        }

        callback(dungeon);
    }

    public IEnumerator GenerateMap(DungeonGenInfo dungeonGenInfo, DR_Dungeon dungeon, int depth, MapGeneratedCallback callback) {
        Vector2Int mapSize = dungeonGenInfo.getFloorSize(depth);
        DR_GameManager gm = DR_GameManager.instance;

        // TODO: separate steps out into their own coroutines?
        graphGenerator.GenerateGraph(visualizeGeneration);
        yield return new WaitWhile(() => graphGenerator.isGeneratingGraph);


        // Create map layout here
        MapLayout mapLayout = graphGenerator.GetGraphAsMapLayout(mapSize, new Vector2Int(7,7));//new MapLayout(mapSize);
        MapBlueprint mapBlueprint = new MapBlueprint(mapSize);
        Coroutine visualizer = null;
        if (visualizeGeneration){
            visualizationLayoutTarget = mapLayout;
            visualizer = StartCoroutine(VisualizationCoroutine());
        }

        if (visualizeGeneration)
            yield return new WaitForSeconds(2.0f * visualizationSpeedMod);


        if (visualizeGeneration){
            visualizationLayoutTarget = null;
            visualizationTarget = mapBlueprint;
        }

        // Create rooms based on nodes and carve out
        foreach(var node in mapLayout.nodes){
            Vector2Int roomPos = node.position;
            Vector2Int roomSize = node.size;
            MapGenRoom room = new(roomPos, roomSize, mapBlueprint);
            node.resultingRoom = room;
            room.roomTag = node.roomTag;
            room.roomLabel = node.label;
            mapBlueprint.rooms.Add(room);

            if (!room.IsValid()){
                Debug.LogError("Room is outside map bounds!");
            }

            mapBlueprint.AssignRoomToCells(room);

            mapBlueprint.PlaceCellType(
                roomPos.x, roomPos.x+roomSize.x, 
                roomPos.y, roomPos.y+roomSize.y, 
                MapGenCellType.WALL);
            mapBlueprint.PlaceCellType(
                roomPos.x+1, roomPos.x+roomSize.x-1, 
                roomPos.y+1, roomPos.y+roomSize.y-1, 
                MapGenCellType.FLOOR);
            
            switch (room.roomTag){
                case RoomTag.START:
                    mapBlueprint.GetCell(room.GetCenterPosition()).type = MapGenCellType.STAIRS_UP;
                    break;
                case RoomTag.END:
                    mapBlueprint.GetCell(room.GetCenterPosition()).type = (depth == dungeonGenInfo.floors-1) ? MapGenCellType.GOAL : MapGenCellType.STAIRS_DOWN;
                    mapBlueprint.GetCell(room.GetCenterPosition() + Vector2Int.up).type = MapGenCellType.HEALTH_ALTAR;
                    mapBlueprint.GetCell(room.GetCenterPosition() + Vector2Int.up + Vector2Int.right).type = MapGenCellType.ITEM_ALTAR;
                    mapBlueprint.GetCell(room.GetCenterPosition() + Vector2Int.up + Vector2Int.left).type = MapGenCellType.ITEM_ALTAR;
                    break;
                case RoomTag.RELIC:
                    mapBlueprint.GetCell(room.GetCenterPosition() + Vector2Int.right).type = MapGenCellType.ITEM_ALTAR;
                    mapBlueprint.GetCell(room.GetCenterPosition()).type = MapGenCellType.ITEM_ALTAR;
                    mapBlueprint.GetCell(room.GetCenterPosition() + Vector2Int.left).type = MapGenCellType.ITEM_ALTAR;
                    break;
                case RoomTag.HEALTH:
                    mapBlueprint.GetCell(room.GetCenterPosition()).type = MapGenCellType.HEALTH_ALTAR;
                    break;
                default:
                break;
            }
            
            if (visualizeGeneration)
                yield return new WaitForEndOfFrame();
        }

        // Mark door tiles and carve out hallways:
        foreach(var connection in mapLayout.connections){
            var roomA = connection.Item1.resultingRoom;
            var roomB = connection.Item2.resultingRoom;
            Vector2Int posDiff = roomB.pos - roomA.pos;
            
            Vector2Int dir = new Vector2Int(
                (int)Mathf.Sign(posDiff.x), 
                (int)Mathf.Sign(posDiff.y));
            
            Vector2Int doorPosA = roomA.GetEdgePositionAtDir(posDiff);
            Vector2Int doorPosB = roomB.GetEdgePositionAtDir(-posDiff);

            Vector2Int currentTarget = doorPosB;
            if (!roomB.IsPositionInsideRoom(doorPosB - (Vector2Int.right * dir.x))){
                currentTarget.x -= dir.x;
            }else{
                currentTarget.y -= dir.y;
            }

            Vector2Int currentPos = doorPosA;

            for(int i = 0; currentPos != doorPosB && i < 100; i++){

                Vector2Int diff = currentTarget - currentPos;

                bool ShouldMoveHorizontal(){
                    // Get away from room for first few iterations
                    if (i < 3){
                        return !roomA.IsPositionInsideRoom(currentPos + Vector2Int.right * MathF.Sign(diff.x));
                    }
                    // Prefer not overlapping room until needed
                    if (diff.x != 0 && diff.y != 0){
                        return !roomB.IsPositionInsideRoom(currentPos + Vector2Int.right * MathF.Sign(diff.x));
                    }
                    // Prefer moving in direction of larger difference
                    return Mathf.Abs(diff.x) > Mathf.Abs(diff.y);
                }

                if (ShouldMoveHorizontal()){
                    currentPos.x +=  MathF.Sign(diff.x);
                }else{
                    currentPos.y += MathF.Sign(diff.y);
                }

                if (currentPos == currentTarget){
                    currentTarget = doorPosB;
                }

                mapBlueprint.GetCell(currentPos).type = MapGenCellType.FLOOR;
                if (visualizeGeneration){
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    
                }
            }

            if (visualizeGeneration){
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }
                

            mapBlueprint.GetCell(doorPosA).type = MapGenCellType.DOOR;
            mapBlueprint.GetCell(doorPosB).type = MapGenCellType.DOOR;
        }

        int tempChestIndex = 0;

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
                    case MapGenCellType.HEALTH_ALTAR:
                        newEntity = EntityFactory.CreateEntityFromContent(gm.healthAltarContent);
                        break;
                    case MapGenCellType.ITEM_ALTAR:
                        //TODO: this is a chest and not an item altar!
                        newEntity = EntityFactory.CreateEntityFromContent(gm.itemAltars[tempChestIndex]);
                        tempChestIndex = (tempChestIndex + 1) % gm.itemAltars.Count;
                        newEntity.GetComponent<AltarComponent>().altarAbilityContent = LootHandler.instance.GetRandomAbility(newEntity.GetComponent<AltarComponent>().chestType);
                        break;
                    // Temporarily do this here
                    case MapGenCellType.ITEM:
                        //Vector2Int itemPos = new Vector2Int(x,y);
                        //int itemIndex = UnityEngine.Random.Range(0, gm.relicPickupContentArray.Count);
                        //var item = EntityFactory.CreateEntityFromContent(gm.relicPickupContentArray[itemIndex]);
                        //mapBlueprint.entitiesToPlace.Add(itemPos, item);
                        break;
                }

                if (newEntity != null) {
                    mapBlueprint.entitiesToPlace.Add(new Vector2Int(x, y), newEntity);
                    if (visualizeGeneration)
                        yield return new WaitForEndOfFrame();
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
                int chosenIndex = UnityEngine.Random.Range(0, floorEnemies.Count);
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
            if (visualizeGeneration)
                yield return new WaitForEndOfFrame();
            mapBlueprint.entitiesToPlace.Add(enemyPos, enemy);

            // Decrement room index, subtract exp cost
            experienceBudget -= enemy.GetComponent<LevelComponent>().stats.expGiven;
            if (--roomIndex < 1) { 
                roomIndex = mapBlueprint.rooms.Count - 1;
            }
        }
        
        Debug.Log("Floor " + (depth + 1) + ": leftover budget " + experienceBudget + "/" + dungeonGenInfo.getExpectedExperience(depth+1) + ". lowest exp enemy is " + lowestExpEnemy);
        
        if (visualizeGeneration)
            yield return new WaitForSeconds(2f * visualizationSpeedMod);

        if (visualizeGeneration){
            StopCoroutine(visualizer);
            visualizationTarget = null;
            ClearVisualization();
        }

        DR_Map newMap = CreateMapFromBlueprint(mapBlueprint);
        yield return null;
        callback(newMap);
    }
    
    public static DR_Map CreateMapFromBlueprint(MapBlueprint mapBlueprint) {
        DR_Map newMap = new DR_Map(mapBlueprint.mapSize);
        DR_GameManager gm = DR_GameManager.instance;


        for (int y = 0; y < newMap.MapSize.y; y++) {
            for (int x = 0; x < newMap.MapSize.x; x++) {
                DR_Cell newCell = new DR_Cell();
                newMap.Cells[y, x] = newCell;

                newCell.associatedRoom = mapBlueprint.cells[y, x].associatedRoom;

                switch (mapBlueprint.cells[y, x].type) {
                    case MapGenCellType.NOT_SET:
                        newCell.bBlocksMovement = true;
                        newCell.neverRender = !mapBlueprint.RequiresRendering(x,y);
                        break;
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
                Debug.LogError("CreateMapFromBlueprint: unable to place " + posEntityPair.Value.Name + " at " + posEntityPair.Key);
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

    public bool RequiresRendering(int x, int y)
    {
        // Check adjacent cells
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip self
                if (dx == 0 && dy == 0){
                    continue;
                }

                int cellX = x + dx;
                int cellY = y + dy;

                if (cellX >= 0 && cellX < mapSize.x && cellY >= 0 && cellY < mapSize.y)
                {
                    if (cells[cellY, cellX].type != MapGenCellType.NOT_SET)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}



public class MapLayout {
    public Vector2Int mapSize;

    public List<MapLayoutNode> nodes = new();
    public List<Tuple<MapLayoutNode, MapLayoutNode>> connections = new();

    public Dictionary<EpForceDirectedGraph.cs.Node, MapLayoutNode> NodeDict = new();

    public MapLayout(Vector2Int size){
        mapSize = size;
    }
}