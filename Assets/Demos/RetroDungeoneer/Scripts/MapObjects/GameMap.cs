namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Encapsulates the game map and all its tiles, and field of view information
    /// </summary>
    public class GameMap
    {
        /// <summary>
        /// Size of the map in tiles
        /// </summary>
        public readonly Vector2i size;

        /// <summary>
        /// The tiles that make up the map
        /// </summary>
        public readonly Tile[,] terrain;

        /// <summary>
        /// Field of view information for each tile
        /// </summary>
        public readonly FOVTile[,] fov;

        /// <summary>
        /// The level/floor of the dungeon this map is for
        /// </summary>
        public int dungeonLevel = 0;

        private Color32 mBackgroundColor = Color.black;
        private Color32 mUnexploredColor;

        private AudioAsset mMusic;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of the map/level</param>
        public GameMap(Vector2i size)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            this.size = size;
            terrain = new Tile[size.width, size.height];
            fov = new FOVTile[size.width, size.height];

            mMusic = game.assets.musicGame;

            InitializeTiles();
        }

        private enum RoomShape
        {
            /// <summary>
            /// Rectangle shaped room
            /// </summary>
            Rectangle,

            /// <summary>
            /// Ellipse shaped room
            /// </summary>
            Ellipse,

            /// <summary>
            /// Ellipse shaped room with some fuzziness along the edges
            /// </summary>
            EllipseFuzzy
        }

        /// <summary>
        /// Background color for this map
        /// </summary>
        public Color32 backgroundColor
        {
            get
            {
                return mBackgroundColor;
            }

            set
            {
                mBackgroundColor = value;
                mUnexploredColor = new Color32((byte)(mBackgroundColor.r / 2), (byte)(mBackgroundColor.g / 2), (byte)(mBackgroundColor.b / 2), 255);
            }
        }

        /// <summary>
        /// Unexplored color for this map
        /// </summary>
        public Color32 unexploredColor
        {
            get
            {
                return mUnexploredColor;
            }
        }

        /// <summary>
        /// Music index for this map
        /// </summary>
        public AudioAsset music
        {
            get
            {
                return mMusic;
            }

            set
            {
                mMusic = value;
            }
        }

        /// <summary>
        /// Remove entity from an entity list at its old map location, and put it in the list at its new location
        /// </summary>
        /// <param name="entity">Entity to move</param>
        /// <param name="oldPos">Old position</param>
        public static void UpdateEntityMapPos(EntityID entity, Vector2i oldPos)
        {
            var e = entity.e;
            if (e == null)
            {
                return;
            }

            if (oldPos.x >= 0 && oldPos.y >= 0)
            {
                var entityList = RB.MapDataGet<List<EntityID>>(C.LAYER_TERRAIN, oldPos);
                if (entityList != null)
                {
                    entityList.Remove(entity);
                }
            }

            Vector2i newPos = e.pos;

            if (newPos.x >= 0 && newPos.y >= 0)
            {
                var entityList = RB.MapDataGet<List<EntityID>>(C.LAYER_TERRAIN, newPos);
                if (entityList != null)
                {
                    entityList.Add(entity);
                }
            }
        }

        /// <summary>
        /// Check if the given map position is blocked
        /// </summary>
        /// <param name="pos">Position</param>
        /// <returns>True if blocked</returns>
        public bool IsBlocked(Vector2i pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= size.width || pos.y >= size.height)
            {
                return true;
            }

            var tile = terrain[pos.x, pos.y];
            if (tile != null && tile.blocked)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Make a new map
        /// </summary>
        /// <param name="maxRooms">Maximum rooms in map</param>
        /// <param name="roomMinSize">Minimum room size</param>
        /// <param name="roomMaxSize">Maximum room size</param>
        /// <param name="mapWidth">Max width</param>
        /// <param name="mapHeight">Max height</param>
        /// <param name="player">Player to put into the map</param>
        public void MakeMap(int maxRooms, int roomMinSize, int roomMaxSize, int mapWidth, int mapHeight, EntityID player)
        {
            if (dungeonLevel == 0 || dungeonLevel == 6)
            {
                MakeForestMap(mapWidth, mapHeight, player);
            }
            else
            {
                MakeDungeonMap(maxRooms, roomMinSize, roomMaxSize, mapWidth, mapHeight, player);
            }

            return;
        }

        /// <summary>
        /// Make a starting dungeon room
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <param name="room">Room rect</param>
        public void MakeStartRoom(EntityID player, Rect2i room)
        {
            if (dungeonLevel == 1)
            {
                var daggerSpawnRect = room;
                daggerSpawnRect.Expand(-2);
                EntityFunctions.CreateItem(ItemType.Dagger, RandomUtils.RandomRectPerimeterPos(daggerSpawnRect));
            }

            // Place the player in the middle of the room
            player.e.pos = room.center;
        }

        /// <summary>
        /// Make final boss room
        /// </summary>
        /// <param name="room">Room rect</param>
        public void MakeBossRoom(Rect2i room)
        {
            EntityFunctions.CreateMonster(MonsterType.Golem, new Vector2i(room.center.x, room.center.y - (room.height / 4)));
        }

        /// <summary>
        /// Make test final boss room
        /// </summary>
        /// <param name="room">Room rect</param>
        public void MakeTestBossRoom(Rect2i room)
        {
            var boss = EntityFunctions.CreateMonster(MonsterType.Golem, new Vector2i(room.center.x, room.center.y - (room.height / 4)));
            boss.e.fighter.hp = 1;
        }

        /// <summary>
        /// Make a new test map
        /// </summary>
        /// <param name="player">Player to put into the map</param>
        public void MakeTestMap(EntityID player)
        {
            dungeonLevel = 5;
            backgroundColor = new Color32(0x47, 0x2d, 0x3c, 255);

            var playerEntity = EntityStore.Get(player);
            if (playerEntity == null)
            {
                return;
            }

            List<Rect2i> rooms = new List<Rect2i>();
            int testRoomsCount = 5;
            var testRoomSize = new Vector2i(12, 12);
            int roomXOffset = 1;

            for (int r = 0; r < testRoomsCount; r++)
            {
                // Random width and height
                int width = testRoomSize.width;
                int height = testRoomSize.height;

                // Random position
                int x = roomXOffset;
                int y = 1;

                roomXOffset += width + 1;

                var newRoom = new Rect2i(x, y, width, height);

                CreateRoom(newRoom, RoomShape.Rectangle);

                // Diggly squiggly tunnel below room
                SetTile(C.LAYER_TERRAIN, new Vector2i(newRoom.x + (newRoom.width / 2), newRoom.y + newRoom.height), new Tile(Tile.TEMPLATE_EMPTY));
                CreateHTunnel(newRoom.x + (newRoom.width / 2) - 4, newRoom.x + (newRoom.width / 2), 1 + testRoomSize.height + 1);
                SetTile(C.LAYER_TERRAIN, new Vector2i(newRoom.x + (newRoom.width / 2) - 4, newRoom.y + newRoom.height + 2), new Tile(Tile.TEMPLATE_EMPTY));

                var roomCenter = newRoom.center;

                if (rooms.Count == 0)
                {
                    // This is the first room, put the player in the center of it
                    playerEntity.pos = roomCenter;

                    PlaceEntities(newRoom, 100, 0);
                }
                else if (rooms.Count == 1)
                {
                    MakeTestBossRoom(newRoom);
                }
                else
                {
                    PlaceEntities(newRoom, 0, r * 2);
                }

                // Add the new room to list of rooms
                rooms.Add(newRoom);
            }

            // Dig connecting tunnel
            CreateHTunnel(1, size.width - 2, 1 + testRoomSize.height + 3);

            UpdateAllEntityMapPositions();

            BeautifyDungeonMap();
        }

        /// <summary>
        /// Re-compute the field of view for the map from the given origin (player)
        /// </summary>
        /// <param name="origin">Origin entity</param>
        /// <param name="radius">FOV radius</param>
        public void RecomputeFov(EntityID origin, int radius)
        {
            // Turn all tiles invisible first
            for (int x = 0; x < size.width; x++)
            {
                for (int y = 0; y < size.height; y++)
                {
                    fov[x, y].visible = false;
                }
            }

            var e = origin.e;

            // Turn the tile at origin visible
            fov[e.pos.x, e.pos.y].visible = true;

            for (int d = 0; d < Direction.Diagonal.Length; d++)
            {
                var dir = Direction.Diagonal[d];
                CastLight(e.pos, radius, 1, 1.0f, 0.0f, 0, dir.x, dir.y, 0);
                CastLight(e.pos, radius, 1, 1.0f, 0.0f, dir.x, 0, 0, dir.y);
            }

            UpdateFOVTilemap();
        }

        /// <summary>
        /// Check if the given position is in field of view
        /// </summary>
        /// <param name="pos">Position to check</param>
        /// <returns>True if it is in field of view</returns>
        public bool IsInFOV(Vector2i pos)
        {
            if (pos.x < 0 | pos.y < 0 || pos.x >= size.width || pos.y >= size.height)
            {
                return false;
            }

            var fovTile = fov[pos.x, pos.y];
            return fovTile.visible;
        }

        /// <summary>
        /// Check if given position is in an explored area, which may or may not be in field of view
        /// </summary>
        /// <param name="pos">Position to check</param>
        /// <returns>True if in explored area</returns>
        public bool IsInExplored(Vector2i pos)
        {
            if (pos.x < 0 | pos.y < 0 || pos.x >= size.width || pos.y >= size.height)
            {
                return false;
            }

            var fovTile = fov[pos.x, pos.y];
            return fovTile.explored;
        }

        /// <summary>
        /// Refresh the RetroBlit tilemap from our current terrain state
        /// </summary>
        public void RefreshTilemap()
        {
            for (int x = 0; x < size.width; x++)
            {
                for (int y = 0; y < size.height; y++)
                {
                    var tile = terrain[x, y];

                    if (tile.sprite.id != PackedSpriteID.empty.id)
                    {
                        RB.MapSpriteSet(C.LAYER_TERRAIN, new Vector2i(x, y), tile.sprite, tile.color);
                    }
                    else
                    {
                        RB.MapSpriteSet(C.LAYER_TERRAIN, new Vector2i(x, y), RB.SPRITE_EMPTY);
                    }
                }
            }

            UpdateFOVTilemap();
        }

        /// <summary>
        /// Create the next floor, this also takes care of moving the player to the next floor
        /// </summary>
        /// <param name="player">Player</param>
        public void NextFloor(EntityID player)
        {
            dungeonLevel++;

            List<EntityID> playerEntities = player.e.GetSelfAndChildren();
            EntityStore.ClearExcept(playerEntities);

            InitializeTiles();
            MakeMap(C.MAX_ROOMS, C.ROOM_MIN_SIZE, C.ROOM_MAX_SIZE, C.MAP_WIDTH, C.MAP_HEIGHT, player);

            if (player.e.moveTrail != null)
            {
                player.e.moveTrail.Clear();
            }
        }

        /// <summary>
        /// Reset all entity lists for all tiles. This is expensive and should not be done often.
        /// </summary>
        public void UpdateAllEntityMapPositions()
        {
            // Clear all entity lists
            for (int x = 0; x < size.width; x++)
            {
                for (int y = 0; y < size.height; y++)
                {
                    var entityList = RB.MapDataGet<List<EntityID>>(C.LAYER_TERRAIN, new Vector2i(x, y));
                    if (entityList == null)
                    {
                        entityList = new List<EntityID>();
                        RB.MapDataSet(C.LAYER_TERRAIN, new Vector2i(x, y), entityList);
                    }
                    else
                    {
                        entityList.Clear();
                    }
                }
            }

            for (int i = 0; i < EntityStore.entities.Count; i++)
            {
                var e = EntityStore.entities[i].e;
                if (e == null)
                {
                    continue;
                }

                if (e.pos.x < 0 || e.pos.y < 0 || e.pos.x >= size.width || e.pos.y >= size.height)
                {
                    continue;
                }

                var entityList = RB.MapDataGet<List<EntityID>>(C.LAYER_TERRAIN, e.pos);
                entityList.Add(e.id);
            }
        }

        /// <summary>
        /// Spawn the exit portal
        /// </summary>
        /// <param name="pos">Location for the portal</param>
        public void SpawnExitPortal(Vector2i pos)
        {
            int attempts = 1000;
            Vector2i randomPos = Vector2i.zero;

            while (attempts > 0)
            {
                randomPos = new Vector2i(pos.x + Random.Range(-4, 5), pos.y + Random.Range(-4, 5));

                if (IsInFOV(randomPos) && !IsBlocked(randomPos) && pos != randomPos)
                {
                    break;
                }

                attempts--;
            }

            if (attempts == 0)
            {
                Debug.Log("Could not find a place to spawn exit portal!!");
                return;
            }

            var portal = EntityFunctions.CreateInteractable(InteractableType.Portal, randomPos);
            portal.e.stairs = new Stairs(dungeonLevel + 1)
            {
                type = Stairs.StairType.PORTAL
            };

            EffectManager.Instance.AddEffect(new EffectPortal(portal));
        }

        private void MakeDungeonMap(int maxRooms, int roomMinSize, int roomMaxSize, int mapWidth, int mapHeight, EntityID player)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            backgroundColor = new Color32(0x47, 0x2d, 0x3c, 255);
            mMusic = game.assets.musicGame;

            var playerEntity = EntityStore.Get(player);
            if (playerEntity == null)
            {
                return;
            }

            List<Rect2i> rooms = new List<Rect2i>();

            for (int r = 0; r < maxRooms; r++)
            {
                // Random width and height
                int width;
                int height;

                if (dungeonLevel < 5 || rooms.Count > 0)
                {
                    width = Random.Range(roomMinSize, roomMaxSize + 1);
                    height = Random.Range(roomMinSize, roomMaxSize + 1);
                }
                else
                {
                    width = Random.Range(roomMinSize * 2, (roomMaxSize * 2) + 1);
                    height = Random.Range(roomMinSize * 2, (roomMaxSize * 2) + 1);
                }

                // Random position
                int x = Random.Range(0, mapWidth - width);
                int y = Random.Range(0, mapHeight - height);

                var newRoom = new Rect2i(x, y, width, height);

                // Check if the room intersects with other rooms
                bool intersects = false;
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (newRoom.Intersects(rooms[i]))
                    {
                        intersects = true;
                        break;
                    }
                }

                // No intersections, it's safe to create this room
                if (!intersects)
                {
                    CreateRoom(newRoom, RoomShape.Rectangle);

                    var roomCenter = newRoom.center;

                    if (rooms.Count > 0)
                    {
                        // All rooms after the first one should be connected with a tunnel to the previous room
                        var prevRoomCenter = rooms[rooms.Count - 1].center;

                        if (Random.Range(0, 2) == 1)
                        {
                            CreateHTunnel(prevRoomCenter.x, roomCenter.x, prevRoomCenter.y);
                            CreateVTunnel(prevRoomCenter.y, roomCenter.y, roomCenter.x);
                        }
                        else
                        {
                            CreateVTunnel(prevRoomCenter.y, roomCenter.y, prevRoomCenter.x);
                            CreateHTunnel(prevRoomCenter.x, roomCenter.x, roomCenter.y);
                        }
                    }

                    // Add the new room to list of rooms
                    rooms.Add(newRoom);
                }
            }

            // Populate all rooms except for first and last
            for (int i = 1; i < rooms.Count - 1; i++)
            {
                if (i != 0 && i != rooms.Count - 1)
                {
                    var numOfMonsters = RandomUtils.FromDungeonLevel(
                        new RandomUtils.Tuple<int, int>[]
                        {
                            new RandomUtils.Tuple<int, int>(2, 1),
                            new RandomUtils.Tuple<int, int>(3, 3),
                            new RandomUtils.Tuple<int, int>(5, 5)
                        },
                        dungeonLevel);

                    var numOfItems = RandomUtils.FromDungeonLevel(
                        new RandomUtils.Tuple<int, int>[]
                        {
                            new RandomUtils.Tuple<int, int>(1, 1),
                            new RandomUtils.Tuple<int, int>(2, 4)
                        },
                        dungeonLevel);

                    PlaceEntities(rooms[i], numOfItems, numOfMonsters);
                }
            }

            var firstRoom = rooms[0];
            var lastRoom = rooms[rooms.Count - 1];

            if (dungeonLevel < 5)
            {
                MakeStartRoom(player, firstRoom);
                var stairsDown = EntityFunctions.CreateInteractable(InteractableType.Stairs, lastRoom.center);
                stairsDown.e.stairs = new Stairs(dungeonLevel + 1);
            }
            else
            {
                // For final level make the player start room the last room, and boss room the first room
                // This makes it easier to ensure we can fit a nice big boss room before creating other rooms
                MakeBossRoom(firstRoom);
                MakeStartRoom(player, lastRoom);
            }

            UpdateAllEntityMapPositions();

            BeautifyDungeonMap();

            RecomputeFov(player, C.FOV_RADIUS);

            if (dungeonLevel == 1)
            {
                EffectManager.Instance.AddEffect(new EffectPlayerEntrance(player));
            }
        }

        private void MakeForestMap(int mapWidth, int mapHeight, EntityID player)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            backgroundColor = new Color32(0x25, 0x8f, 0x4a, 255);
            mMusic = game.assets.musicForest;

            var playerEntity = EntityStore.Get(player);
            if (playerEntity == null)
            {
                return;
            }

            int width = Random.Range(14, 18);
            int height = Random.Range(14, 18);

            int x = (mapWidth / 2) - (width / 2);
            int y = (mapHeight / 2) - (height / 2);

            var newRoom = new Rect2i(x, y, width, height);

            CreateRoom(newRoom, RoomShape.EllipseFuzzy);

            var roomCenter = newRoom.center;

            int randExit = Random.Range(0, 4);

            Vector2i blockadePos = Vector2i.zero;
            Vector2i thugPos = Vector2i.zero;
            Vector2i gameExitPos = Vector2i.zero;
            Vector2i gameExitDir = Vector2i.zero;

            if (randExit == 0)
            {
                CreateHTunnel(roomCenter.x, 0, roomCenter.y);
                blockadePos = new Vector2i(roomCenter.x - (newRoom.width / 2) - 2, roomCenter.y);
                gameExitPos = blockadePos;
                gameExitPos.x -= 3;
                gameExitDir = new Vector2i(-1, 0);
                thugPos = blockadePos;
                thugPos.x += 4;
            }
            else if (randExit == 1)
            {
                CreateHTunnel(roomCenter.x, mapWidth - 1, roomCenter.y);
                blockadePos = new Vector2i(roomCenter.x + (newRoom.width / 2) + 2, roomCenter.y);
                gameExitPos = blockadePos;
                gameExitPos.x += 3;
                gameExitDir = new Vector2i(1, 0);
                thugPos = blockadePos;
                thugPos.x -= 4;
            }
            else if (randExit == 2)
            {
                CreateVTunnel(0, roomCenter.y, roomCenter.x);
                blockadePos = new Vector2i(roomCenter.x, roomCenter.y - (newRoom.height / 2) - 2);
                gameExitPos = blockadePos;
                gameExitPos.y -= 3;
                gameExitDir = new Vector2i(0, -1);
                thugPos = blockadePos;
                thugPos.y += 4;
            }
            else if (randExit == 3)
            {
                CreateVTunnel(mapHeight - 1, roomCenter.y, roomCenter.x);
                blockadePos = new Vector2i(roomCenter.x, roomCenter.y + (newRoom.height / 2) + 2);
                gameExitPos = blockadePos;
                gameExitPos.y += 3;
                gameExitDir = new Vector2i(0, 1);
                thugPos = blockadePos;
                thugPos.y -= 4;
            }

            // This is the first room, put the player in the center of it
            playerEntity.pos = RandomUtils.RandomRectPerimeterPos(new Rect2i(roomCenter.x - 1, roomCenter.y - 1, 3, 3));

            EntityID thug = EntityID.empty;

            if (dungeonLevel == 0)
            {
                thug = EntityFunctions.CreateMonster(MonsterType.InvincibleThug, thugPos);
                EntityFunctions.CreateMonster(MonsterType.InvincibleBlockade, blockadePos);
            }
            else
            {
                thug = EntityFunctions.CreateMonster(MonsterType.Thug, thugPos);
                EntityFunctions.CreateMonster(MonsterType.Blockade, blockadePos);

                while (gameExitPos.x > 0 && gameExitPos.y > 0 && gameExitPos.x < size.width && gameExitPos.y < size.height)
                {
                    EntityFunctions.CreateInteractable(InteractableType.GameExit, gameExitPos);
                    gameExitPos += gameExitDir;
                }
            }

            // Give thug a dagger and armor
            var dagger = EntityFunctions.CreateItem(ItemType.Dagger, new Vector2i(-1, -1));
            thug.e.inventory = new Inventory(1);
            thug.e.equipment = new Equipment();
            thug.e.equipment.equipment[(int)dagger.e.equippable.slot] = dagger;

            var armor = EntityFunctions.CreateItem(ItemType.LeatherArmor, new Vector2i(-1, -1));
            thug.e.equipment.equipment[(int)armor.e.equippable.slot] = armor;

            var well = EntityFunctions.CreateInteractable(InteractableType.Well, new Vector2i(roomCenter.x, roomCenter.y + 2));
            well.e.stairs = new Stairs(dungeonLevel + 1);
            if (dungeonLevel == 0)
            {
                well.e.stairs.type = Stairs.StairType.WELL;
            }
            else
            {
                well.e.stairs.type = Stairs.StairType.WELL_CLOSED;
            }

            UpdateAllEntityMapPositions();

            BeautifyForestMap();

            RecomputeFov(player, C.FOV_RADIUS);

            var options = new List<SceneMessage.MessageBoxOption>
            {
                new SceneMessage.MessageBoxOption(C.FSTR.Set("Continue"), CloseMessageBox)
            };

            if (dungeonLevel == 0)
            {
                C.FSTR.Set(C.STR_COLOR_DIALOG);
                C.FSTR.Append("You've been travelling through the forest for hours and decide to rest at a nearby clearing.");
                C.FSTR.Append("\n\n");
                C.FSTR.Append("As you settle down a man appears behind you. He swings his sword at a rope tied to a tree beside him. Branches and debris fall onto the path blocking your only exit. It appears you've been ambushed.");
                C.FSTR.Append("\n\n");
                C.FSTR.Append(C.STR_COLOR_NAME);
                C.FSTR.Append("\"I'll take what you're carrying, once I'm done carving you up!\"").Append(C.STR_COLOR_DIALOG).Append(" the brigand says with a grimace.");
                C.FSTR.Append("\n\n");
                C.FSTR.Append("Your eyes dart to an old well in the middle of the clearing. Your options seem clear... jump down the well, or die here.");

                game.ShowMessageBox(
                    C.FSTR2.Set("Trapped"),
                    C.FSTR,
                    options);
            }
            else
            {
                C.FSTR.Set(C.STR_COLOR_DIALOG);
                C.FSTR.Append("You open your eyes, the swirling portal closes behind you. You find yourself back where it all started, in the forest clearing.");
                C.FSTR.Append("\n\n");
                C.FSTR.Append("The brigand who ambushed you is still here, preparing his trap for the next victim.");
                C.FSTR.Append("\n\n");
                C.FSTR.Append(C.STR_COLOR_NAME);
                C.FSTR.Append("\"How did you... nevermind, lets finish what we started!\"").Append(C.STR_COLOR_DIALOG).Append(" he says, startled by your sudden appearance.");
                C.FSTR.Append("\n\n");
                C.FSTR.Append("You tighten your grip on your weapon, you won't be jumping into the well this time...");

                game.ShowMessageBox(
                    C.FSTR2.Set("A Sudden Return"),
                    C.FSTR,
                    options);
            }
        }

        private void CloseMessageBox()
        {
            var game = (RetroDungeoneerGame)RB.Game;
            game.CloseMessageBox();
        }

        /// <summary>
        /// Go through the generate map and put some randomness and flavor into the tiles. This flavoring does not affect the map
        /// layout.
        /// </summary>
        private void BeautifyDungeonMap()
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var tile = terrain[x, y];
                    if (tile.type == Tile.Type.WALL_BRICK)
                    {
                        if (y != size.y - 1 && terrain[x, y + 1].type != Tile.Type.WALL_BRICK)
                        {
                            int random = Random.Range(0, 100);

                            if (random > 90)
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_BRICK3);
                            }
                            else if (random > 75)
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_BRICK2);
                            }
                            else
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_BRICK1);
                            }
                        }
                        else
                        {
                            int random = Random.Range(0, 3);

                            if (random == 0)
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_FILL1);
                            }
                            else if (random == 1)
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_FILL2);
                            }
                            else
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_FILL3);
                            }
                        }
                    }
                    else if (tile.type == Tile.Type.EMPTY)
                    {
                        int random = Random.Range(0, 100);

                        if (random > 95)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.FLOOR_GRASS1);
                        }
                        else if (random > 90)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.FLOOR_DIRT1);
                        }
                        else if (random > 85)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.FLOOR_DIRT2);
                        }
                        else if (random > 80)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.FLOOR_DIRT3);
                        }
                    }
                }
            }
        }

        private void BeautifyForestMap()
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var tile = terrain[x, y];
                    if (tile.type == Tile.Type.WALL_BRICK)
                    {
                        if (tile.type == Tile.Type.WALL_BRICK)
                        {
                            int random = Random.Range(0, 100);
                            Color32 color = Tile.WALL_TREE1.color;
                            byte randColor = (byte)Random.Range(-10, 30);
                            color.r += randColor;
                            color.g += randColor;
                            color.b += randColor;

                            if (random > 90)
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_TREE3, color);
                            }
                            else if (random > 75)
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_TREE2, color);
                            }
                            else
                            {
                                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.WALL_TREE1, color);
                            }
                        }
                    }
                    else if (tile.type == Tile.Type.EMPTY)
                    {
                        int random = Random.Range(0, 100);

                        if (random > 80)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.FLOOR_GRASS1);
                        }
                        else if (random > 60)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.FLOOR_GRASS2);
                        }
                        else if (random > 40)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), Tile.FLOOR_GRASS3);
                        }
                    }
                }
            }
        }

        private void InitializeTiles()
        {
            // Initialize the map to all "filled" tiles
            for (int x = 0; x < size.width; x++)
            {
                for (int y = 0; y < size.height; y++)
                {
                    var pos = new Vector2i(x, y);
                    SetTile(C.LAYER_TERRAIN, pos, new Tile(Tile.TEMPLATE_WALL_BRICK));
                    fov[pos.x, pos.y] = new FOVTile();
                }
            }
        }

        private void CastLight(Vector2i startPos, float radius, int row, float start, float end, int xx, int xy, int yx, int yy)
        {
            var newStart = 0.0f;
            if (start < end)
            {
                return;
            }

            var maxWidth = size.width;
            var maxHeight = size.height;

            var blocked = false;

            int distance;
            for (distance = row; distance <= radius && !blocked; distance++)
            {
                Vector2i delta;
                delta.y = -distance;
                for (delta.x = -distance; delta.x <= 0; delta.x++)
                {
                    Vector2i currentPos = new Vector2i(startPos.x + (delta.x * xx) + (delta.y * xy), startPos.y + (delta.x * yx) + (delta.y * yy));

                    var leftSlope = (delta.x - 0.5f) / (delta.y + 0.5f);
                    var rightSlope = (delta.x + 0.5f) / (delta.y - 0.5f);

                    if (!(currentPos.x >= 0 && currentPos.y >= 0 && currentPos.x < maxWidth && currentPos.y < maxHeight) || start < rightSlope)
                    {
                        continue;
                    }
                    else if (end > leftSlope)
                    {
                        break;
                    }

                    var currentFOVTile = fov[currentPos.x, currentPos.y];
                    var currentTile = terrain[currentPos.x, currentPos.y];

                    currentFOVTile.visible = true;
                    currentFOVTile.explored = true;

                    if (blocked)
                    {
                        // Previous cell was a blocking one
                        if (currentTile.blockSight)
                        {
                            // Hit a wall
                            newStart = rightSlope;
                            continue;
                        }
                        else
                        {
                            blocked = false;
                            start = newStart;
                        }
                    }
                    else
                    {
                        if (currentTile.blockSight && distance < radius)
                        {
                            // Hit a wall within sight line
                            blocked = true;
                            CastLight(startPos, radius, distance + 1, start, leftSlope, xx, xy, yx, yy);
                            newStart = rightSlope;
                        }
                    }
                }
            }
        }

        private void UpdateFOVTilemap()
        {
            var unexploredColor = ((RetroDungeoneerGame)RB.Game).map.unexploredColor;
            var exploredColor = unexploredColor;
            exploredColor.a = 200;

            for (int x = 0; x < size.width; x++)
            {
                for (int y = 0; y < size.height; y++)
                {
                    var currentFOVTile = fov[x, y];

                    if (currentFOVTile.visible)
                    {
                        RB.MapSpriteSet(C.LAYER_VISIBILITY, new Vector2i(x, y), RB.SPRITE_EMPTY);
                    }
                    else
                    {
                        if (currentFOVTile.explored)
                        {
                            RB.MapSpriteSet(C.LAYER_VISIBILITY, new Vector2i(x, y), S.FOG, exploredColor);
                        }
                        else
                        {
                            RB.MapSpriteSet(C.LAYER_VISIBILITY, new Vector2i(x, y), S.FOG, unexploredColor);
                        }
                    }
                }
            }
        }

        private void CreateRoom(Rect2i room, RoomShape shape)
        {
            if (shape == RoomShape.Rectangle)
            {
                for (int y = room.y + 1; y < room.max.y; y++)
                {
                    for (int x = room.x + 1; x < room.max.x; x++)
                    {
                        SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), new Tile(Tile.TEMPLATE_EMPTY));
                    }
                }
            }
            else if (shape == RoomShape.Ellipse || shape == RoomShape.EllipseFuzzy)
            {
                for (int y = room.y + 1; y < room.max.y; y++)
                {
                    for (int x = room.x + 1; x < room.max.x; x++)
                    {
                        float h = room.center.x;
                        float k = room.center.y;
                        float a = room.width;
                        float b = room.height;

                        float p = (((x - h) * (x - h)) / (a * a)) + (((y - k) * (y - k)) / (b * b));

                        float dist = 0.2f;
                        if (shape == RoomShape.EllipseFuzzy)
                        {
                            dist += Random.Range(0, 3) * 0.05f;
                        }

                        if (p <= dist)
                        {
                            SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), new Tile(Tile.TEMPLATE_EMPTY));
                        }
                    }
                }
            }
        }

        private void CreateHTunnel(int x1, int x2, int y)
        {
            int xStart = System.Math.Min(x1, x2);
            int xEnd = System.Math.Max(x1, x2);

            for (int x = xStart; x <= xEnd; x++)
            {
                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), new Tile(Tile.TEMPLATE_EMPTY));
            }
        }

        private void CreateVTunnel(int y1, int y2, int x)
        {
            int yStart = System.Math.Min(y1, y2);
            int yEnd = System.Math.Max(y1, y2);

            for (int y = yStart; y <= yEnd; y++)
            {
                SetTile(C.LAYER_TERRAIN, new Vector2i(x, y), new Tile(Tile.TEMPLATE_EMPTY));
            }
        }

        private void SetTile(int layer, Vector2i pos, Tile tile)
        {
            SetTile(layer, pos, tile, tile.color);
        }

        private void SetTile(int layer, Vector2i pos, Tile tile, Color color)
        {
            if (layer == C.LAYER_TERRAIN)
            {
                terrain[pos.x, pos.y] = tile;
            }

            if (tile.sprite.id != PackedSpriteID.empty.id)
            {
                RB.MapSpriteSet(layer, pos, tile.sprite, color);
            }
            else
            {
                RB.MapSpriteSet(layer, pos, RB.SPRITE_EMPTY);
            }
        }

        private void PlaceEntities(Rect2i room, int numOfItems, int numOfMonsters)
        {
            var mMonsterChances = EntityFunctions.GetMonsterChances(dungeonLevel);
            var mItemChances = EntityFunctions.GetItemChances(dungeonLevel);

            // Place monsters
            var existingMonsterPositions = new List<Vector2i>();
            for (int i = 0; i < numOfMonsters; i++)
            {
                var randomPos = new Vector2i(
                    Random.Range(room.min.x + 1, room.max.x),
                    Random.Range(room.min.y + 1, room.max.y));

                if (!existingMonsterPositions.Contains(randomPos))
                {
                    var monsterType = (MonsterType)RandomUtils.RandomChoiceIndex(mMonsterChances);
                    var monster = EntityFunctions.CreateMonster(monsterType, randomPos);

                    if (!monster.isEmpty)
                    {
                        existingMonsterPositions.Add(randomPos);
                    }
                }
            }

            // Place items
            var existingItemPositions = new List<Vector2i>();
            for (int i = 0; i < numOfItems; i++)
            {
                var randomPos = new Vector2i(
                    Random.Range(room.min.x + 1, room.max.x),
                    Random.Range(room.min.y + 1, room.max.y));

                if (!existingItemPositions.Contains(randomPos))
                {
                    var itemType = (ItemType)RandomUtils.RandomChoiceIndex(mItemChances);
                    var item = EntityFunctions.CreateItem(itemType, randomPos);

                    if (!item.isEmpty)
                    {
                        existingItemPositions.Add(randomPos);
                    }
                }
            }
        }
    }
}
