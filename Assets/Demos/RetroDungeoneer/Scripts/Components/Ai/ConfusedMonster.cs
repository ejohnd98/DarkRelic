namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// AI for a confused monster. Moves around randomly, does not attack.
    /// </summary>
    public class ConfusedMonster : Ai
    {
        /// <summary>
        /// Previous AI to go back to when no longer confused
        /// </summary>
        public Ai previousAi;

        /// <summary>
        /// Turns remaining for the confusion
        /// </summary>
        public int numberOfTurns;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfusedMonster() : base(ComponentType.ConfusedMonster)
        {
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public ConfusedMonster(BinaryReader reader) : base(ComponentType.ConfusedMonster)
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
            if (numberOfTurns > 0)
            {
                var randomPos = owner.e.pos;
                randomPos.x += Random.Range(-1, 2);
                randomPos.y += Random.Range(-1, 2);

                if (randomPos != owner.e.pos)
                {
                    owner.e.MoveTowards(randomPos);
                }

                numberOfTurns--;
            }
            else
            {
                owner.e.ai = previousAi;
                resultSet.AddMessage(C.FSTR.Set("The ").Append(C.STR_COLOR_NAME).Append(owner.e.name).Append("@- is no longer confused!"));
            }
        }

        /// <summary>
        /// Handle death
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void HandleDeath(ResultSet resultSet)
        {
            // Restore previous AI and let it handle the death
            owner.e.ai = previousAi;
            owner.e.ai.HandleDeath(resultSet);
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(numberOfTurns);
            writer.Write(previousAi);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            numberOfTurns = reader.ReadInt32();
            previousAi = (Ai)reader.ReadComponent();
        }
    }
}
