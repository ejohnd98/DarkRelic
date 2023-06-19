namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;

    /// <summary>
    /// Manages sound play back for instances where RB.SoundPlay won't do, eg delayed sounds
    /// </summary>
    public class SoundBank
    {
        private static SoundBank mInstance;

        private List<PendingSound> mPendingSounds = new List<PendingSound>();

        private SoundBank()
        {
            Initialize();
        }

        /// <summary>
        /// Get the SoundBank instance
        /// </summary>
        public static SoundBank Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new SoundBank();
                }

                return mInstance;
            }
        }

        /// <summary>
        /// Play a sound after framesUntilPlay delay
        /// </summary>
        /// <param name="sound">Sound asset to play</param>
        /// <param name="volume">Volume</param>
        /// <param name="pitch">Pitch</param>
        /// <param name="framesUntilPlay">Frames until sound plays</param>
        public void SoundPlayDelayed(AudioAsset sound, float volume, float pitch, int framesUntilPlay)
        {
            PendingSound pendingSound = new PendingSound();
            pendingSound.soundSlot = sound;
            pendingSound.volume = volume;
            pendingSound.pitch = pitch;
            pendingSound.framesUntilPlay = framesUntilPlay;

            mPendingSounds.Add(pendingSound);
        }

        /// <summary>
        /// Process sound bank, playing any pending sounds
        /// </summary>
        public void Process()
        {
            while (mPendingSounds.Count > 0)
            {
                if (mPendingSounds[0].framesUntilPlay <= 0)
                {
                    var sound = mPendingSounds[0];
                    RB.SoundPlay(sound.soundSlot, sound.volume, sound.pitch);
                    mPendingSounds.RemoveAt(0);
                }
                else
                {
                    mPendingSounds[0].framesUntilPlay--;
                    break;
                }
            }
        }

        /// <summary>
        /// Clear sound bank
        /// </summary>
        public void Clear()
        {
            mPendingSounds.Clear();
        }

        private void Initialize()
        {
            mPendingSounds.Clear();
        }

        private class PendingSound
        {
            public AudioAsset soundSlot;
            public int framesUntilPlay;
            public float volume;
            public float pitch;
        }
    }
}
