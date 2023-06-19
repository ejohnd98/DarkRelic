namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Basic monster Ai. Moves around randomly, attacks nearby player
    /// </summary>
    public class BasicMonster : Ai
    {
        /// <summary>
        /// True if the AI is aggroed
        /// </summary>
        public bool aggroed = false;

        /// <summary>
        /// Turns before aggro breaks when out range or FOV
        /// </summary>
        public int aggroTimeout;

        /// <summary>
        /// Aggro range
        /// </summary>
        public int aggroRange = 8;

        private const int AGGRO_MAX_TIMEOUT = 10;

        private readonly List<Skill> mSkills = new List<Skill>();
        private readonly List<int> mSkillChances = new List<int>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public BasicMonster(ComponentType type = ComponentType.BasicMonster) : base(type)
        {
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        /// <param name="type">Type</param>
        public BasicMonster(BinaryReader reader, ComponentType type = ComponentType.BasicMonster) : base(type)
        {
            Read(reader);
        }

        /// <summary>
        /// Add a skill ability to this AI
        /// </summary>
        /// <param name="skill">Skill type</param>
        /// <param name="chance">Chance of it being used in a turn</param>
        public void AddSkill(Skill skill, int chance)
        {
            mSkills.Add(skill);
            mSkillChances.Add(chance);
        }

        /// <summary>
        /// Take a turn.
        /// </summary>
        /// <param name="resultSet">Result of taking a turn</param>
        /// <param name="target">Target entity being tracked by the Ai</param>
        public override void TakeTurn(ResultSet resultSet, EntityID target)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var monster = owner;

            // No aggro, see if we can aquire aggro
            if (!aggroed)
            {
                if (game.map.IsInFOV(monster.e.pos))
                {
                    float dist = monster.e.DistanceTo(target);
                    if (dist <= aggroRange)
                    {
                        aggroed = true;
                        aggroTimeout = AGGRO_MAX_TIMEOUT;

                        resultSet.AddMessage(C.FSTR.Set("The ").Append(C.STR_COLOR_NAME).Append(owner.e.name).Append("@- spots you!"));

                        EffectManager.Instance.AddEffect(new EffectAggro(monster));

                        AudioAsset aggroSound = UnityEngine.Random.Range(0, 2) == 0 ? game.assets.soundAggro1 : game.assets.soundAggro2;
                        SoundBank.Instance.SoundPlayDelayed(aggroSound, 0.66f, RandomUtils.RandomPitch(0.15f), UnityEngine.Random.Range(0, 8));
                    }
                }
            }
            else
            {
                if (aggroTimeout == 0)
                {
                    resultSet.AddMessage(C.FSTR.Set("The ").Append(C.STR_COLOR_NAME).Append(owner.e.name).Append("@- gives up the chase."));
                    aggroed = false;
                }
                else
                {
                    if (!game.map.IsInFOV(monster.e.pos) || monster.e.DistanceTo(target) > aggroRange)
                    {
                        aggroTimeout--;
                    }
                    else
                    {
                        aggroTimeout = AGGRO_MAX_TIMEOUT;
                    }

                    DoSkill(resultSet, target);
                }
            }
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(aggroed);
            writer.Write(aggroTimeout);
            writer.Write(aggroRange);

            writer.Write(mSkills.Count);
            for (int i = 0; i < mSkills.Count; i++)
            {
                writer.Write((int)mSkills[i]);
                writer.Write(mSkillChances[i]);
            }
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            aggroed = reader.ReadBoolean();
            aggroTimeout = reader.ReadInt32();
            aggroRange = reader.ReadInt32();

            int skillCount = reader.ReadInt32();
            for (int i = 0; i < skillCount; i++)
            {
                mSkills.Add((Skill)reader.ReadInt32());
                mSkillChances.Add(reader.ReadInt32());
            }
        }

        private bool DoSkill(ResultSet resultSet, EntityID target)
        {
            if (mSkills.Count == 0)
            {
                return false;
            }

            int skillIndex = RandomUtils.RandomChoiceIndex(mSkillChances);

            Skill randomSkill = mSkills[skillIndex];

            // Check if there is a backup melee skill
            bool hasMeleeSkill = false;
            for (int i = 0; i < mSkills.Count; i++)
            {
                if (mSkills[i] == Skill.MeleeAttack)
                {
                    hasMeleeSkill = true;
                    break;
                }
            }

            var game = (RetroDungeoneerGame)RB.Game;

            bool skillResult = false;

            switch (randomSkill)
            {
                case Skill.MeleeAttack:
                    return SkillFunctions.MeleeAttack(resultSet, owner, target);

                case Skill.CastWeb:
                    skillResult = SkillFunctions.CastWeb(resultSet, owner, target.e.pos);
                    break;

                case Skill.CastLightning:
                    skillResult = SkillFunctions.CastLightning(resultSet, owner, target.e.pos, 6, 12);
                    break;

                case Skill.ShootBow:
                    {
                        var arrow = owner.e.inventory.GetArrow();
                        if (!arrow.isEmpty)
                        {
                            skillResult = SkillFunctions.ShootBow(resultSet, owner, arrow, 6, 10, target.e.pos);
                            if (skillResult)
                            {
                                owner.e.inventory.RemoveItem(null, arrow);
                                EntityStore.DestroyEntity(arrow);
                            }
                        }
                    }

                    break;

                case Skill.CastRandomFireball:
                    {
                        int attempts = 40;

                        while (attempts > 0)
                        {
                            var fireballPos = new Vector2i(target.e.pos.x, target.e.pos.y);
                            fireballPos.x += Random.Range(-20, 21);
                            fireballPos.y += Random.Range(-20, 21);

                            if (game.map.IsInFOV(fireballPos))
                            {
                                skillResult = SkillFunctions.CastFireball(resultSet, owner, 4, 10, fireballPos, false);
                                break;
                            }

                            attempts--;
                        }
                    }

                    break;

                case Skill.Teleport:
                    {
                        // Find a location to teleport on the furthest side of target from where the entity is now
                        // Limited attempts are made, so this may not end up being furthest
                        var furthestPos = new Vector2i(-1, -1);
                        var furthestDist = 0.0f;

                        int attempts = 40;
                        Vector2i targetPos = target.e.pos;

                        while (attempts > 0)
                        {
                            var randomPos = new Vector2i(Random.Range(targetPos.x - 1, targetPos.x + 2), Random.Range(targetPos.y - 1, targetPos.y + 2));

                            if (EntityFunctions.GetBlockingEntityAtPos(randomPos).isEmpty && !game.map.IsBlocked(randomPos))
                            {
                                float dist = (owner.e.pos - randomPos).SqrMagnitude();
                                if (dist > furthestDist)
                                {
                                    furthestDist = dist;
                                    furthestPos = randomPos;
                                }
                            }

                            attempts--;
                        }

                        if (furthestPos.x != -1)
                        {
                            skillResult = SkillFunctions.Teleport(resultSet, owner, furthestPos);
                        }
                        else
                        {
                            Debug.Log("Failed");
                        }
                    }

                    break;
            }

            if (skillResult == false && hasMeleeSkill)
            {
                skillResult = SkillFunctions.MeleeAttack(resultSet, owner, target);
            }

            return skillResult;
        }

        private struct SkillChance
        {
            public Skill skill;
            public int chance;

            public SkillChance(Skill skill, int chance)
            {
                this.skill = skill;
                this.chance = chance;
            }
        }
    }
}
