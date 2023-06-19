namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Helps serialize sound asset references while saving game state
    /// </summary>
    public static class SoundSerializer
    {
        private static List<Mapping> mMapping;

        private enum SoundType
        {
            soundInvalid,
            soundMonsterDeath,
            soundPlayerDeath,
            soundFootStep,
            soundMonsterAttack,
            soundPlayerAttack,
            soundInventory,
            soundDrink,
            soundMenuOpen,
            soundMenuClose,
            soundStairs,
            soundPointerSelect,
            soundSelectOption,
            soundLevelUp,
            soundFireBall,
            soundLightning,
            soundConfuse,
            soundCheat,
            soundAggro1,
            soundAggro2,
            soundPlayerFallYell,
            soundPortal,
            soundJump,
            soundBowShoot,
            soundBowHit,
            soundWeb,
            soundTeleport,
            soundSlime,
            musicMainMenu,
            musicGame,
            musicDeath,
            musicForest
        }

        /// <summary>
        /// Initialize sound lookup
        /// </summary>
        public static void Initialize()
        {
            var game = (RetroDungeoneerGame)RB.Game;
            mMapping = new List<Mapping>();

            mMapping.Add(new Mapping(SoundType.soundInvalid, null));
            mMapping.Add(new Mapping(SoundType.soundMonsterDeath, game.assets.soundMonsterDeath));
            mMapping.Add(new Mapping(SoundType.soundPlayerDeath, game.assets.soundPlayerDeath));
            mMapping.Add(new Mapping(SoundType.soundFootStep, game.assets.soundFootStep));
            mMapping.Add(new Mapping(SoundType.soundMonsterAttack, game.assets.soundMonsterAttack));
            mMapping.Add(new Mapping(SoundType.soundPlayerAttack, game.assets.soundPlayerAttack));
            mMapping.Add(new Mapping(SoundType.soundInventory, game.assets.soundInventory));
            mMapping.Add(new Mapping(SoundType.soundDrink, game.assets.soundDrink));
            mMapping.Add(new Mapping(SoundType.soundMenuOpen, game.assets.soundMenuOpen));
            mMapping.Add(new Mapping(SoundType.soundMenuClose, game.assets.soundMenuClose));
            mMapping.Add(new Mapping(SoundType.soundStairs, game.assets.soundStairs));
            mMapping.Add(new Mapping(SoundType.soundPointerSelect, game.assets.soundPointerSelect));
            mMapping.Add(new Mapping(SoundType.soundSelectOption, game.assets.soundSelectOption));
            mMapping.Add(new Mapping(SoundType.soundLevelUp, game.assets.soundLevelUp));
            mMapping.Add(new Mapping(SoundType.soundFireBall, game.assets.soundFireBall));
            mMapping.Add(new Mapping(SoundType.soundLightning, game.assets.soundLightning));
            mMapping.Add(new Mapping(SoundType.soundConfuse, game.assets.soundConfuse));
            mMapping.Add(new Mapping(SoundType.soundCheat, game.assets.soundCheat));
            mMapping.Add(new Mapping(SoundType.soundAggro1, game.assets.soundAggro1));
            mMapping.Add(new Mapping(SoundType.soundAggro2, game.assets.soundAggro2));
            mMapping.Add(new Mapping(SoundType.soundPlayerFallYell, game.assets.soundPlayerFallYell));
            mMapping.Add(new Mapping(SoundType.soundPortal, game.assets.soundPortal));
            mMapping.Add(new Mapping(SoundType.soundJump, game.assets.soundJump));
            mMapping.Add(new Mapping(SoundType.soundBowShoot, game.assets.soundBowShoot));
            mMapping.Add(new Mapping(SoundType.soundBowHit, game.assets.soundBowHit));
            mMapping.Add(new Mapping(SoundType.soundWeb, game.assets.soundWeb));
            mMapping.Add(new Mapping(SoundType.soundTeleport, game.assets.soundTeleport));
            mMapping.Add(new Mapping(SoundType.soundSlime, game.assets.soundSlime));

            mMapping.Add(new Mapping(SoundType.musicMainMenu, game.assets.musicMainMenu));
            mMapping.Add(new Mapping(SoundType.musicGame, game.assets.musicGame));
            mMapping.Add(new Mapping(SoundType.musicDeath, game.assets.musicDeath));
            mMapping.Add(new Mapping(SoundType.musicForest, game.assets.musicForest));
        }

        /// <summary>
        /// Serialize audio asset into an index
        /// </summary>
        /// <param name="sound">Sound asset</param>
        /// <returns>Index</returns>
        public static int Serialize(AudioAsset sound)
        {
            if (sound == null)
            {
                return (int)SoundType.soundInvalid;
            }

            for (int i = 0; i < mMapping.Count; i++)
            {
                if (mMapping[i].sound == sound)
                {
                    return (int)mMapping[i].type;
                }
            }

            Debug.LogError("Can't seralize sound, not in the mapping!");

            return (int)SoundType.soundInvalid;
        }

        /// <summary>
        /// Deserialize sound from an index
        /// </summary>
        /// <param name="serialized">Index</param>
        /// <returns>Sound asset</returns>
        public static AudioAsset Deserialize(int serialized)
        {
            SoundType type = (SoundType)serialized;

            for (int i = 0; i < mMapping.Count; i++)
            {
                if (mMapping[i].type == type)
                {
                    return mMapping[i].sound;
                }
            }

            Debug.LogError("Can't deseralize sound, not in the mapping!");

            return null;
        }

        private struct Mapping
        {
            public SoundType type;
            public AudioAsset sound;

            public Mapping(SoundType type, AudioAsset sound)
            {
                this.type = type;
                this.sound = sound;
            }
        }
    }
}
