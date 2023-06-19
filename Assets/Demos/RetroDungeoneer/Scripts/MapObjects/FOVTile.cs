namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Field of view tile information
    /// </summary>
    public class FOVTile
    {
        /// <summary>
        /// Whether the tile is visible
        /// </summary>
        public bool visible = false;

        /// <summary>
        /// Whether the tile is explored, even though it may be invisible again
        /// </summary>
        public bool explored = false;
    }
}
