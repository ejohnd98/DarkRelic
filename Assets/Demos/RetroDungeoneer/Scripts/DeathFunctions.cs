namespace RetroBlitDemoRetroDungeoneer
{
    using UnityEngine;

    /// <summary>
    /// Functions for handling player or monster deaths
    /// </summary>
    public struct DeathFunctions
    {
        /// <summary>
        /// Kill the player
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="player">Player entity</param>
        public static void KillPlayer(ResultSet resultSet, EntityID player)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var pe = player.e;

            pe.sprite = S.GRAVESTONE;
            pe.color = new Color32(200, 200, 200, 255);
            C.FSTR.Set(pe.name);
            pe.name.Set("grave of ").Append(C.FSTR);
            pe.renderOrder = RenderFunctions.RenderOrder.CORPSE;

            // Delete all equipment, this prevents paperDoll rendering on the grave
            pe.equipment = null;

            resultSet.AddMessage(C.FSTR.Set("You ").Append(C.STR_COLOR_DEAD).Append("@w145died@w000@-!"));

            RB.SoundPlay(game.assets.soundPlayerDeath, 1.0f);
            RB.MusicVolumeSet(1.0f);
            RB.MusicPlay(game.assets.musicDeath);

            EffectManager.Instance.AddEffect(new EffectPlayerDeath(player));
        }

        /// <summary>
        /// Kill a monster
        /// </summary>
        /// <param name="resultSet">Result</param>
        /// <param name="monster">Monster entity</param>
        public static void KillMonster(ResultSet resultSet, EntityID monster)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            var e = monster.e;

            if (e.ai != null)
            {
                e.ai.HandleDeath(resultSet);
            }

            e.sprite = S.BONES;
            e.color = new Color32(200, 200, 200, 255);
            e.blocks = false;
            e.fighter = null;
            e.ai = null;
            e.renderOrder = RenderFunctions.RenderOrder.CORPSE;

            // Delete all equipment, this prevents paperDoll rendering on the grave
            e.equipment = null;

            C.FSTR.Set(e.name);
            e.name.Set("remains of ").Append(C.FSTR);

            resultSet.AddMessage(C.FSTR2.Set(C.STR_COLOR_NAME).Append(C.FSTR).Append("@- is ").Append(C.STR_COLOR_DEAD).Append("@w145dead@w000@-!"));

            RB.SoundPlay(game.assets.soundMonsterDeath, 1.0f, RandomUtils.RandomPitch(0.1f));
        }
    }
}
