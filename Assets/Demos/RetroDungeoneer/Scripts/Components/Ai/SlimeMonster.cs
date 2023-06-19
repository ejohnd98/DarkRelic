namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Slime monster Ai. Similar to basic, but spawns baby slimes on death
    /// </summary>
    public class SlimeMonster : BasicMonster
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SlimeMonster() : base(ComponentType.SlimeMonster)
        {
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public SlimeMonster(BinaryReader reader) : base(ComponentType.SlimeMonster)
        {
            Read(reader);
        }

        /// <summary>
        /// Handle death
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void HandleDeath(ResultSet resultSet)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var pos = owner.e.pos;

            int attempts = 40;
            int babyCount = Random.Range(2, 5);

            while (attempts > 0 && babyCount > 0)
            {
                var randomPos = new Vector2i(Random.Range(pos.x - 1, pos.x + 2), Random.Range(pos.y - 1, pos.y + 2));

                if (randomPos.x != pos.x || randomPos.y != pos.y)
                {
                    if (EntityFunctions.GetBlockingEntityAtPos(randomPos).isEmpty && !game.map.IsBlocked(randomPos))
                    {
                        var babySlime = EntityFunctions.CreateMonster(MonsterType.SlimeSmall, randomPos);
                        babySlime.e.Move(Vector2i.zero); // Cause map update

                        SoundBank.Instance.SoundPlayDelayed(game.assets.soundSlime, 1.0f, RandomUtils.RandomPitch(0.1f), (babyCount - 1) * 3);
                        babyCount--;
                    }
                }

                attempts--;
            }
        }
    }
}
