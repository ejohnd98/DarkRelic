namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Responsible for initializing a new game
    /// </summary>
    public static class InitializeNewGame
    {
        /// <summary>
        /// Initialize game constants. These are not known at build time, so we initialize them here, they should not
        /// change from this point on.
        /// </summary>
        public static void InitializeConstants()
        {
            var game = (RetroDungeoneerGame)RB.Game;

            C.SCREEN_WIDTH = RB.DisplaySize.width / game.assets.spriteSheet.grid.cellSize.width;
            C.SCREEN_HEIGHT = RB.DisplaySize.height / game.assets.spriteSheet.grid.cellSize.height;
            C.FOV_RADIUS = (C.SCREEN_WIDTH / 2) + 2;

            var rand = new System.Random();
            int seed = rand.Next();
            UnityEngine.Random.InitState(seed);
        }

        /// <summary>
        /// Setup new game variables. This involves creating the player, and generating the game map.
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="gameMap">Game map</param>
        public static void SetupGameVariables(ref EntityID player, ref GameMap gameMap)
        {
            EntityStore.Clear();

            player = EntityFunctions.CreatePlayer(new Vector2i(-1, -1));

            gameMap = new GameMap(new Vector2i(C.MAP_WIDTH, C.MAP_HEIGHT));
            gameMap.MakeMap(C.MAX_ROOMS, C.ROOM_MIN_SIZE, C.ROOM_MAX_SIZE, C.MAP_WIDTH, C.MAP_HEIGHT, player);

            if (player.e.moveTrail != null)
            {
                player.e.moveTrail.Clear();
            }
        }

        /// <summary>
        /// Setup new test game variables. This involves creating the player, and generating the game map used for testing only.
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="gameMap">Game map</param>
        public static void SetupTestGameVariables(ref EntityID player, ref GameMap gameMap)
        {
            EntityStore.Clear();

            player = EntityFunctions.CreatePlayer(new Vector2i(C.SCREEN_WIDTH / 2, C.SCREEN_HEIGHT / 2));

            gameMap = new GameMap(new Vector2i(C.MAP_WIDTH, C.MAP_HEIGHT));
            gameMap.MakeTestMap(player);
        }
    }
}
