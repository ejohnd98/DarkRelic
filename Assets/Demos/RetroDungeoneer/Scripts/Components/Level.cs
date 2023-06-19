namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Level component. Defines the entities current level
    /// </summary>
    public class Level : EntityComponent
    {
        /// <summary>
        /// Current level
        /// </summary>
        public int currentLevel;

        /// <summary>
        /// Current experience
        /// </summary>
        public int currentXp;

        /// <summary>
        /// Base experience requirement for leveling up
        /// </summary>
        public int levelUpBase;

        /// <summary>
        /// Level up experience factor
        /// </summary>
        public int levelUpFactor;

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public Level(BinaryReader reader) : base(ComponentType.Level)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="currentLevel">Current level</param>
        /// <param name="currentXp">Current experience</param>
        /// <param name="levelUpBase">Base experience requirement for leveling up</param>
        /// <param name="levelUpFactor">Level up experience factor</param>
        public Level(int currentLevel = 1, int currentXp = 0, int levelUpBase = 200, int levelUpFactor = 150) : base(ComponentType.Level)
        {
            this.currentLevel = currentLevel;
            this.currentXp = currentXp;
            this.levelUpBase = levelUpBase;
            this.levelUpFactor = levelUpFactor;
        }

        /// <summary>
        /// Get experience until next level is reached
        /// </summary>
        /// <returns>Experience</returns>
        public int ExperienceToNextLevel()
        {
            return levelUpBase + (currentLevel * levelUpFactor);
        }

        /// <summary>
        /// Add experience, potentially leveling up
        /// </summary>
        /// <param name="xp">Amount of experience to add</param>
        /// <returns>True if leveled up</returns>
        public bool AddXp(int xp)
        {
            currentXp += xp;
            if (currentXp > ExperienceToNextLevel())
            {
                currentXp -= ExperienceToNextLevel();
                currentLevel += 1;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(currentLevel);
            writer.Write(currentXp);
            writer.Write(levelUpBase);
            writer.Write(levelUpFactor);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            currentLevel = reader.ReadInt32();
            currentXp = reader.ReadInt32();
            levelUpBase = reader.ReadInt32();
            levelUpFactor = reader.ReadInt32();
        }
    }
}
