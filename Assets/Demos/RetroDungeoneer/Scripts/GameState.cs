namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Current game state
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Invalid game state
        /// </summary>
        INVALID_STATE,

        /// <summary>
        /// Players turn
        /// </summary>
        PLAYER_TURN,

        /// <summary>
        /// Enemies turn
        /// </summary>
        ENEMY_TURN,

        /// <summary>
        /// Player has died
        /// </summary>
        PLAYER_DEAD,

        /// <summary>
        /// Showing inventory menu
        /// </summary>
        SHOW_INVENTORY,

        /// <summary>
        /// Showing drop inventory menu
        /// </summary>
        DROP_INVENTORY,

        /// <summary>
        /// Showing targeting
        /// </summary>
        TARGETING,

        /// <summary>
        /// Showing help menu
        /// </summary>
        SHOW_HELP,

        /// <summary>
        /// Showing level up menu
        /// </summary>
        LEVEL_UP,

        /// <summary>
        /// Showing character info menu
        /// </summary>
        CHARACTER_SCREEN
    }
}
