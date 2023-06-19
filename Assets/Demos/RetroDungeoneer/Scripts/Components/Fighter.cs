namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// A fighter component, makes this entity a potential combatant
    /// </summary>
    public class Fighter : EntityComponent
    {
        /// <summary>
        /// Maximum hitpoints
        /// </summary>
        public int baseMaxHp;

        /// <summary>
        /// Current hit points
        /// </summary>
        public int hp;

        /// <summary>
        /// Base defense
        /// </summary>
        public int baseDefense;

        /// <summary>
        /// Base power
        /// </summary>
        public int basePower;

        /// <summary>
        /// Experience worth when killed
        /// </summary>
        public int xp;

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public Fighter(BinaryReader reader) : base(ComponentType.Fighter)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hp">Total hitpoints</param>
        /// <param name="defense">Defense</param>
        /// <param name="power">Power</param>
        /// <param name="xp">Experience granted when killed</param>
        public Fighter(int hp, int defense, int power, int xp = 0) : base(ComponentType.Fighter)
        {
            baseMaxHp = hp;
            this.hp = hp;
            baseDefense = defense;
            basePower = power;
            this.xp = xp;
        }

        /// <summary>
        /// Total hitpoints
        /// </summary>
        public int maxHp
        {
            get
            {
                if (owner.e.equipment != null)
                {
                    return baseMaxHp + owner.e.equipment.maxHpBonus;
                }

                return baseMaxHp;
            }
        }

        /// <summary>
        /// Power
        /// </summary>
        public int power
        {
            get
            {
                if (owner.e.equipment != null)
                {
                    return basePower + owner.e.equipment.powerBonus;
                }

                return basePower;
            }
        }

        /// <summary>
        /// Defense
        /// </summary>
        public int defense
        {
            get
            {
                if (owner.e.equipment != null)
                {
                    return baseDefense + owner.e.equipment.defenseBonus;
                }

                return baseDefense;
            }
        }

        /// <summary>
        /// Apply damage to the entity. Potentially killing it
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="amount">Amount of damage to apply</param>
        public void TakeDamage(ResultSet resultSet, int amount)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            hp -= amount;

            if (hp <= 0)
            {
                hp = 0;
                resultSet.AddDead(owner);
                resultSet.AddXp(xp);
            }

            if (owner.e.ai == null)
            {
                // If this is a player taking damage then play monster hit sound
                SoundBank.Instance.SoundPlayDelayed(game.assets.soundMonsterAttack, 0.5f, RandomUtils.RandomPitch(0.1f), UnityEngine.Random.Range(1, 5));

                EffectManager.Instance.AddEffect(new EffectSwoosh(owner));
            }
            else
            {
                // Otherwise play monster hit sound
                SoundBank.Instance.SoundPlayDelayed(game.assets.soundPlayerAttack, 0.5f, RandomUtils.RandomPitch(0.1f), UnityEngine.Random.Range(1, 5));

                EffectManager.Instance.AddEffect(new EffectSwoosh(owner));
            }
        }

        /// <summary>
        /// Make this entity attack another entity
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="target">Target entity</param>
        public void Attack(ResultSet resultSet, EntityID target)
        {
            if (target.isEmpty)
            {
                return;
            }

            int damage = power - target.e.fighter.defense;

            if (damage > 0)
            {
                target.e.fighter.TakeDamage(resultSet, damage);
                resultSet.AddMessage(
                    C.FSTR.Clear()
                    .Append(C.STR_COLOR_NAME)
                    .Append(owner.e.name)
                    .Append("@- attacks ")
                    .Append(C.STR_COLOR_NAME)
                    .Append(target.e.name)
                    .Append("@- for ")
                    .Append(C.STR_COLOR_DAMAGE)
                    .Append(damage)
                    .Append("@- hit points."));
            }
            else
            {
                if (Random.Range(0, 3) == 0)
                {
                    resultSet.AddMessage(
                        C.FSTR.Clear()
                        .Append(C.STR_COLOR_NAME)
                        .Append(owner.e.name)
                        .Append("@- attacks ")
                        .Append(C.STR_COLOR_NAME)
                        .Append(target.e.name)
                        .Append("@- but does no damage."));
                }
                else
                {
                    damage = 1;
                    target.e.fighter.TakeDamage(resultSet, damage);
                    resultSet.AddMessage(
                        C.FSTR.Clear()
                        .Append(C.STR_COLOR_NAME)
                        .Append(owner.e.name)
                        .Append("@- hits ")
                        .Append(C.STR_COLOR_NAME)
                        .Append(target.e.name)
                        .Append("@- with a glancing blow for ")
                        .Append(C.STR_COLOR_DAMAGE)
                        .Append(damage)
                        .Append("@- hit points."));
                }
            }
        }

        /// <summary>
        /// Heal this entity
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="amount">Amount of hitpoints to heal</param>
        public void Heal(ResultSet resultSet, int amount)
        {
            hp += amount;
            if (hp > maxHp)
            {
                hp = maxHp;
            }
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(hp);
            writer.Write(baseMaxHp);
            writer.Write(baseDefense);
            writer.Write(basePower);
            writer.Write(xp);
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            hp = reader.ReadInt32();
            baseMaxHp = reader.ReadInt32();
            baseDefense = reader.ReadInt32();
            basePower = reader.ReadInt32();
            xp = reader.ReadInt32();
        }
    }
}
