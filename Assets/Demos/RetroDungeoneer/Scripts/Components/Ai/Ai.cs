namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Abstract class for an Ai component
    /// </summary>
    public abstract class Ai : EntityComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">AI type</param>
        public Ai(ComponentType type) : base(type)
        {
        }

        /// <summary>
        /// Take a turn
        /// </summary>
        /// <param name="resultSet">Result of taking a turn</param>
        /// <param name="target">Target entity being tracked by the Ai</param>
        public abstract void TakeTurn(ResultSet resultSet, EntityID target);

        /// <summary>
        /// Handle death
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public virtual void HandleDeath(ResultSet resultSet)
        {
            // Do nothing by default
        }
    }
}
