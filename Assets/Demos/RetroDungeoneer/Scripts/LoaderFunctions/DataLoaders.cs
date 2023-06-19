////#define DEBUG_SAVES

namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Game saving/loading functionality
    /// </summary>
    public static class DataLoaders
    {
        /// <summary>
        /// Save game
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="player">Player</param>
        /// <param name="gameMap">Map</param>
        /// <param name="console">Console</param>
        /// <param name="gameState">Current game state</param>
        public static void SaveGame(string filename, EntityID player, GameMap gameMap, Console console, GameState gameState)
        {
            // Create data folder if it doesn't exist
            if (!Directory.Exists(Application.persistentDataPath + "/" + C.SAVE_FOLDER))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + C.SAVE_FOLDER);
            }

            if (!Directory.Exists(Application.persistentDataPath + "/" + C.SAVE_FOLDER))
            {
                console.Log(C.FSTR.Set("Can't create Save folder!"));
                return;
            }

            FileStream file = null;
            string fullFilename = Application.persistentDataPath + "/" + C.SAVE_FOLDER + "/" + filename;

            try
            {
                file = File.Open(fullFilename, FileMode.Create);
            }
            catch
            {
                // Do nothing
                return;
            }

            if (file == null)
            {
                console.Log(C.FSTR.Set("Failed to save to ").Append(fullFilename).Append(", can't open file."));
                return;
            }

#if !DEBUG_SAVES
            var writer = new BinaryWriter(file);
#else
            var writer = new GameWriter(file);
#endif

            EntityStore.Write(writer);

            writer.Write(player);

            // Write GameMap
            writer.Write(gameMap.size);

            // Write terrain tiles
            for (int x = 0; x < gameMap.size.width; x++)
            {
                for (int y = 0; y < gameMap.size.height; y++)
                {
                    writer.Write(gameMap.terrain[x, y]);
                }
            }

            // Write FOV tiles
            for (int x = 0; x < gameMap.size.width; x++)
            {
                for (int y = 0; y < gameMap.size.height; y++)
                {
                    writer.Write(gameMap.fov[x, y]);
                }
            }

            // Write background color
            writer.Write(gameMap.backgroundColor);

            // Write music
            writer.Write(SoundSerializer.Serialize(gameMap.music));

            // Write dungeon level
            writer.Write(gameMap.dungeonLevel);

            // Write Console contents
            writer.Write(Console.LOG_LINES);
            var consoleLogLines = console.GetLogLines();
            var consoleLine = consoleLogLines.Last;
            while (consoleLine != null)
            {
                writer.Write(consoleLine.Value);
                consoleLine = consoleLine.Previous;
            }

            // Write the effects manager
            EffectManager.Instance.Write(writer);

            writer.Write((int)gameState);

            file.Flush();
            writer.Flush();
            writer.Close();
            file.Close();

            // On WebGL we should sync the file system by calling out to Javascript, if we don't do this
            // the game might not actually flush out to the filesystem (IndexDB) before the game quits or page is reloaded.
#if UNITY_WEBGL
            try
            {
                WebGLSyncFS();
            }
            catch
            {
                // Do nothing
            }
#endif
        }

        /// <summary>
        /// Load game
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="player">Player ref</param>
        /// <param name="gameMap">GameMap ref</param>
        /// <param name="gameState">Current game state ref</param>
        /// <param name="console">Console ref</param>
        /// <returns>True if successful</returns>
        public static bool LoadGame(string filename, ref EntityID player, ref GameMap gameMap, ref GameState gameState, Console console)
        {
            string fullFilename = Application.persistentDataPath + "/" + C.SAVE_FOLDER + "/" + filename;

            if (!File.Exists(fullFilename))
            {
                console.Log(C.FSTR.Set("File ").Append(fullFilename).Append(" doesn't exist, can't load game!"));
                return false;
            }

            EntityStore.Clear();

            player = EntityID.empty;
            gameMap = null;
            gameState = GameState.INVALID_STATE;

            FileStream file = null;

            try
            {
                file = File.Open(fullFilename, FileMode.Open);
            }
            catch
            {
                // Do nothing
            }

            if (file == null)
            {
                console.Log(C.FSTR.Set("Failed to load game from ").Append(fullFilename).Append(", can't open file."));
                return false;
            }

            try
            {
#if !DEBUG_SAVES
                var reader = new BinaryReader(file);
#else
                var reader = new GameReader(file);
#endif

                EntityStore.Read(reader);

                // Read player entityID
                player = reader.ReadEntityID();

                // Read GameMap
                var gameMapSize = reader.ReadVector2i();
                gameMap = new GameMap(gameMapSize);

                // Read terrain tiles
                for (int x = 0; x < gameMap.size.width; x++)
                {
                    for (int y = 0; y < gameMap.size.height; y++)
                    {
                        gameMap.terrain[x, y] = reader.ReadTile();
                    }
                }

                // Read FOV tiles
                for (int x = 0; x < gameMap.size.width; x++)
                {
                    for (int y = 0; y < gameMap.size.height; y++)
                    {
                        gameMap.fov[x, y] = reader.ReadFOVTile();
                    }
                }

                // Read background color
                gameMap.backgroundColor = reader.ReadColor32();

                // Read music
                gameMap.music = SoundSerializer.Deserialize(reader.ReadInt32());

                // Read dungeon level
                gameMap.dungeonLevel = reader.ReadInt32();

                gameMap.RefreshTilemap();

                // Read Console contents
                int consoleSize = reader.ReadInt32();
                console.Clear();
                for (int i = 0; i < consoleSize; i++)
                {
                    console.Log(reader.ReadFastString());
                }

                // Read the effects manager
                EffectManager.Instance.Read(reader);

                gameState = (GameState)reader.ReadInt32();

                reader.Close();
            }
            catch (System.Exception e)
            {
                throw new System.Exception("Failed loading: " + e.ToString());
            }

            gameMap.UpdateAllEntityMapPositions();

            return true;
        }

#if UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void WebGLSyncFS();
#endif
    }
}
