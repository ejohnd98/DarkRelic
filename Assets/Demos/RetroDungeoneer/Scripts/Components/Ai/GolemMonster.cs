namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Basic monster Ai. Moves around randomly, attacks nearby player
    /// </summary>
    public class GolemMonster : BasicMonster
    {
        /// <summary>
        /// Whether player encountered this golem already
        /// </summary>
        public bool encounteredOnce = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public GolemMonster() : base(ComponentType.GolemMonster)
        {
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public GolemMonster(BinaryReader reader) : base(ComponentType.GolemMonster)
        {
            Read(reader);
        }

        /// <summary>
        /// Take a turn.
        /// </summary>
        /// <param name="resultSet">Result of taking a turn</param>
        /// <param name="target">Target entity being tracked by the Ai</param>
        public override void TakeTurn(ResultSet resultSet, EntityID target)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            base.TakeTurn(resultSet, target);

            if (aggroed && !encounteredOnce)
            {
                encounteredOnce = true;
                resultSet.AddBossEncounter();
            }
        }

        /// <summary>
        /// Handle death
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void HandleDeath(ResultSet resultSet)
        {
            resultSet.AddBossDefeated(owner);
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(encounteredOnce);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            encounteredOnce = reader.ReadBoolean();
        }
    }
}
