namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// Handles all skills
    /// </summary>
    public static class SkillFunctions
    {
        /// <summary>
        /// Perform a melee attack, or approach
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="attacker">Attacker</param>
        /// <param name="target">Target</param>
        /// <returns>True if successful</returns>
        public static bool MeleeAttack(ResultSet resultSet, EntityID attacker, EntityID target)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (attacker.e.DistanceTo(target) >= 2)
            {
                var dir = game.floodMap.GetCheapestDirection(attacker.e.pos);
                if (dir.x != 0 || dir.y != 0)
                {
                    attacker.e.Move(dir);
                }
                else
                {
                    attacker.e.MoveTowards(target.e.pos);
                }
            }
            else if (target.e.fighter.hp > 0)
            {
                attacker.e.fighter.Attack(resultSet, target);
            }

            return true;
        }

        /// <summary>
        /// Heal self
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="user">User</param>
        /// <param name="healingAmount">Healing amount</param>
        /// <returns>True if successful</returns>
        public static bool Heal(ResultSet resultSet, EntityID user, int healingAmount)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            // May need to check if entity is a Player before printing anything
            if (user.e.fighter.hp == user.e.fighter.maxHp)
            {
                if (user.e.isPlayer)
                {
                    resultSet.AddMessage(C.FSTR.Set("You are already at full health."));
                }
            }
            else
            {
                user.e.fighter.Heal(resultSet, healingAmount);
                resultSet.AddMessage(C.FSTR.Set("Your wounds start to feel better!"));

                RB.SoundPlay(game.assets.soundDrink, 1, RandomUtils.RandomPitch(0.1f));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Cast lightning
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="caster">Caster</param>
        /// <param name="targetPos">Target pos</param>
        /// <param name="maxRange">Maximum spell range</param>
        /// <param name="damage">Damage</param>
        /// <returns>True if successful</returns>
        public static bool CastLightning(ResultSet resultSet, EntityID caster, Vector2i targetPos, int maxRange, int damage)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            EntityID target = EntityID.empty;
            float closestDist = maxRange + 1;

            if (targetPos.x == -1)
            {
                // Find the closest target in range, and in FOV
                for (int i = 0; i < EntityStore.entities.Count; i++)
                {
                    var eid = EntityStore.entities[i];
                    var e = eid.e;

                    if (e.fighter != null && eid != caster && game.map.IsInFOV(e.pos))
                    {
                        var dist = caster.e.DistanceTo(eid);

                        if (dist < closestDist)
                        {
                            target = eid;
                            closestDist = dist;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < EntityStore.entities.Count; i++)
                {
                    var eid = EntityStore.entities[i];
                    var e = eid.e;

                    if (e.pos == targetPos)
                    {
                        target = eid;
                        break;
                    }
                }
            }

            if (!target.isEmpty && CheckTargetPosValid(resultSet, target.e.pos, caster))
            {
                int actualDamage = (damage + caster.e.fighter.power) - target.e.fighter.defense;
                if (actualDamage == 0)
                {
                    actualDamage = 1;
                }

                target.e.fighter.TakeDamage(resultSet, actualDamage);
                resultSet.AddMessage(
                    C.FSTR.Set("A lightning bolt strikes the ").Append(C.STR_COLOR_NAME).Append(target.e.name).Append("@- with a loud thunder! The damage is ").Append(C.STR_COLOR_DAMAGE).Append(actualDamage).Append("@-."));

                EffectManager.Instance.AddEffect(new EffectLightning(caster, target));

                RB.SoundPlay(game.assets.soundLightning);

                return true;
            }
            else
            {
                resultSet.AddMessage(C.FSTR.Set("No enemy is close enough to strike."));
            }

            return false;
        }

        /// <summary>
        /// Cast fireball
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="caster">Caster</param>
        /// <param name="radius">Radius of explosion</param>
        /// <param name="damage">Damage</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="hurtsCaster">True if explosion should hurt caster</param>
        /// <returns>True if successful</returns>
        public static bool CastFireball(ResultSet resultSet, EntityID caster, int radius, int damage, Vector2i targetPos, bool hurtsCaster)
        {
            var game = (RetroDungeoneerGame)RB.Game;
            var map = game.map;

            if (!CheckTargetPosValid(resultSet, targetPos, caster))
            {
                return false;
            }

            resultSet.AddMessage(
                C.FSTR.Set("The fireball explodes, burning everything within ").Append(C.STR_COLOR_NAME).Append(radius).Append("@- tiles!"));

            // Apply damage to all entities in range
            for (int x = targetPos.x - radius; x <= targetPos.x + radius; x++)
            {
                if (x < 0 || x >= map.size.width)
                {
                    continue;
                }

                for (int y = targetPos.y - radius; y <= targetPos.y + radius; y++)
                {
                    if (y < 0 || y >= map.size.height)
                    {
                        continue;
                    }

                    var pos = new Vector2i(x, y);
                    var delta = pos - targetPos;
                    int dist = UnityEngine.Mathf.RoundToInt(delta.Magnitude());

                    if (dist > radius)
                    {
                        continue;
                    }

                    var tile = map.terrain[x, y];

                    if (!tile.blocked)
                    {
                        var entities = EntityFunctions.GetEntitiesAtPos(new Vector2i(x, y));

                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (entities[i] == caster && !hurtsCaster)
                            {
                                continue;
                            }

                            var e = entities[i].e;
                            if (e.fighter != null)
                            {
                                int actualDamage = (damage + caster.e.fighter.power) - e.fighter.defense;
                                if (actualDamage == 0)
                                {
                                    actualDamage = 1;
                                }

                                resultSet.AddMessage(
                                C.FSTR.Set("The ").Append(C.STR_COLOR_NAME).Append(e.name).Append("@- got burned for ").Append(C.STR_COLOR_DAMAGE).Append(actualDamage).Append("@- hit points."));

                                e.fighter.TakeDamage(resultSet, actualDamage);
                            }
                        }

                        EffectManager.Instance.AddEffect(new EffectFlame(pos, dist * 4));
                    }
                }
            }

            RB.SoundPlay(game.assets.soundFireBall);

            return true;
        }

        /// <summary>
        /// Cast confuse
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="caster">Caster</param>
        /// <param name="targetPos">Target position</param>
        /// <returns>True if successful</returns>
        public static bool CastConfuse(ResultSet resultSet, EntityID caster, Vector2i targetPos)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (!CheckTargetPosValid(resultSet, targetPos, caster))
            {
                return false;
            }

            EntityID target = EntityID.empty;

            for (int i = 0; i < EntityStore.entities.Count; i++)
            {
                var eid = EntityStore.entities[i];
                var e = eid.e;

                if (e.pos == targetPos)
                {
                    target = eid;
                    break;
                }
            }

            if (!target.isEmpty)
            {
                var confusedAi = new ConfusedMonster();
                confusedAi.previousAi = target.e.ai;
                confusedAi.numberOfTurns = 10;

                target.e.ai = confusedAi;

                resultSet.AddMessage(
                    C.FSTR.Set("The eyes of the ").Append(C.STR_COLOR_NAME).Append(target.e.name).Append("@- look vacant, as it starts to stumble around!"));

                EffectManager.Instance.AddEffect(new EffectConfuse(target));

                RB.SoundPlay(game.assets.soundConfuse);

                return true;
            }
            else
            {
                resultSet.AddMessage(
                    C.FSTR.Set("There is no targetable enemy at that location."));
            }

            return false;
        }

        /// <summary>
        /// Shoot bow
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="caster">Caster</param>
        /// <param name="arrow">Arrow to shoot</param>
        /// <param name="maxRange">Max bow range</param>
        /// <param name="damage">Damage</param>
        /// <param name="targetPos">Target position</param>
        /// <returns>True if successful</returns>
        public static bool ShootBow(ResultSet resultSet, EntityID caster, EntityID arrow, int maxRange, int damage, Vector2i targetPos)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            EntityID target = EntityID.empty;
            float closestDist = maxRange + 1;

            if (targetPos.x == -1)
            {
                // Find the closest target in range, and in FOV
                for (int i = 0; i < EntityStore.entities.Count; i++)
                {
                    var eid = EntityStore.entities[i];
                    var e = eid.e;

                    if (e.fighter != null && eid != caster && game.map.IsInFOV(e.pos))
                    {
                        var dist = caster.e.DistanceTo(eid);

                        if (dist <= maxRange && dist < closestDist)
                        {
                            target = eid;
                            closestDist = dist;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < EntityStore.entities.Count; i++)
                {
                    var eid = EntityStore.entities[i];
                    var e = eid.e;

                    if (e.pos == targetPos)
                    {
                        target = eid;
                        break;
                    }
                }

                if (target.isEmpty)
                {
                    if (caster.e.isPlayer)
                    {
                        resultSet.AddMessage(C.FSTR.Set("There is no targetable enemy at that location."));
                    }

                    return false;
                }
                else if (caster.e.DistanceTo(target) > maxRange)
                {
                    if (caster.e.isPlayer)
                    {
                        resultSet.AddMessage(C.FSTR.Set("The target is out of range of your weapon."));
                    }

                    return false;
                }
            }

            if (!target.isEmpty && CheckTargetPosValid(resultSet, target.e.pos, caster))
            {
                int actualDamage = (damage + caster.e.fighter.power) - target.e.fighter.defense;
                if (actualDamage == 0)
                {
                    actualDamage = 1;
                }

                target.e.fighter.TakeDamage(resultSet, actualDamage);
                resultSet.AddMessage(
                    C.FSTR.Set("The ").Append(C.STR_COLOR_NAME).Append(arrow.e.name).Append("@- strikes the ").Append(C.STR_COLOR_NAME).Append(target.e.name).Append("@- with a thud. The damage is ").Append(C.STR_COLOR_DAMAGE).Append(actualDamage).Append("@-."));

                EffectManager.Instance.AddEffect(new EffectThrow(caster, target, Vector2i.zero, S.ARROW_SHOT, S.ARROW_SHOT_45, arrow.e.color, game.assets.soundBowShoot, game.assets.soundBowHit));

                return true;
            }
            else
            {
                if (caster.e.isPlayer)
                {
                    resultSet.AddMessage(C.FSTR.Set("No enemy is close enough to shoot."));
                }
            }

            return false;
        }

        /// <summary>
        /// Cast web
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="caster">Caster</param>
        /// <param name="targetPos">Target position</param>
        /// <returns>True if successful</returns>
        public static bool CastWeb(ResultSet resultSet, EntityID caster, Vector2i targetPos)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (!CheckTargetPosValid(resultSet, targetPos, caster))
            {
                return false;
            }

            var entities = EntityFunctions.GetEntitiesAtPos(targetPos);
            var hasWeb = false;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].e.groundTrigger != null && entities[i].e.groundTrigger.type == GroundTrigger.GroundTriggerType.Web)
                {
                    hasWeb = true;
                }
            }

            if (!hasWeb)
            {
                EntityFunctions.CreateInteractable(InteractableType.Web, targetPos);
                EffectManager.Instance.AddEffect(new EffectThrow(caster, EntityID.empty, targetPos, S.WEB, new Color32(255, 255, 255, 100), game.assets.soundWeb, game.assets.soundWeb));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Teleport
        /// </summary>
        /// <param name="resultSet">Result set</param>
        /// <param name="caster">Caster</param>
        /// <param name="targetPos">Target position</param>
        /// <returns>True if successful</returns>
        public static bool Teleport(ResultSet resultSet, EntityID caster, Vector2i targetPos)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (!CheckTargetPosValid(resultSet, targetPos, caster))
            {
                return false;
            }

            if ((targetPos - caster.e.pos).Magnitude() > 8)
            {
                if (caster.e.isPlayer)
                {
                    resultSet.AddMessage(C.FSTR.Set("Target location is too far to teleport to."));
                    return false;
                }
            }

            if (!EntityFunctions.GetBlockingEntityAtPos(targetPos).isEmpty || game.map.IsBlocked(targetPos))
            {
                if (caster.e.isPlayer)
                {
                    resultSet.AddMessage(C.FSTR.Set("Target location is blocked, you can't teleport there!"));
                    return false;
                }
            }

            EffectManager.Instance.AddEffect(new EffectTeleport(caster.e.pos, targetPos));
            caster.e.pos = targetPos;
            resultSet.AddMessage(C.FSTR.Set(C.STR_COLOR_NAME).Append(caster.e.name).Append("@- teleported!"));

            return true;
        }

        private static bool CheckTargetPosValid(ResultSet resultSet, Vector2i targetPos, EntityID user)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            if (!game.map.IsInFOV(targetPos) || !game.map.IsInFOV(user.e.pos))
            {
                if (user.e.isPlayer)
                {
                    resultSet.AddMessage(C.FSTR.Set("You cannot target a tile outside of your field of view."));
                }

                return false;
            }

            var tile = game.map.terrain[targetPos.x, targetPos.y];
            if (tile.blocked)
            {
                if (user.e.isPlayer)
                {
                    resultSet.AddMessage(C.FSTR.Set("You cannot target a blocking tile."));
                }

                return false;
            }

            return true;
        }
    }
}
