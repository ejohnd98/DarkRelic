namespace RetroBlitInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// RetroBlit Audio subsystem, responsible for playing sound and music
    /// </summary>
    public class RBAudio : MonoBehaviour
    {
        /// <summary>
        /// Previously playing music clip
        /// </summary>
        public AudioAsset previousMusicClip = null;

        /// <summary>
        /// Currently playing music clip
        /// </summary>
        public AudioAsset currentMusicClip = null;

        /// <summary>
        /// The music channel
        /// </summary>
        public SoundChannel musicChannel;

        private static long mSequenceCounter = 1;

        /// <summary>
        /// Previous music channel
        /// </summary>
        private SoundChannel mPrevMusicChannel;

        private float mPrevMusicVolume;

#pragma warning disable 0414 // Unused warning
        private RBAPI mRetroBlitAPI = null;
#pragma warning restore 0414

        private SoundChannel[] mSoundChannels = new SoundChannel[RBHardware.HW_MAX_SOUND_CHANNELS];

        private float mTargetMusicVolume = 1.0f;
        private float mTargetMusicPitch = 1.0f;

        private GameObject mAudioListenerObj = null;

        private float mCrossfadeProgress = 1.0f;
        private float mCrossfadeDuration = 2.0f;
        private float mCrossfadeOverlap = 0.5f;
        private Ease.Func mCrossfadeFadeOutFunc = Ease.Func.QuadOut;
        private Ease.Func mCrossfadeFadeInFunc = Ease.Func.QuadOut;
        private bool mCrossfadeNoPrevious = true;

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Reference to subsystem wrapper</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RBAPI api)
        {
            mRetroBlitAPI = api;

            var audioSourceObj = GameObject.Find("RetroBlitAudio");
            if (audioSourceObj == null)
            {
                Debug.Log("Can't find RetroBlitAudio! Does your RetroBlit object no longer match the RetroBlit prefab?");
                return false;
            }

            mAudioListenerObj = GameObject.Find("RetroBlitAudioListener");
            if (mAudioListenerObj == null)
            {
                Debug.Log("Can't find RetroBlitAudioListener! Does your RetroBlit object no longer match the RetroBlit prefab?");
                return false;
            }

            InitializeChannels();

            SoundListenerPosSet(Vector2i.zero);

            previousMusicClip = null;
            currentMusicClip = null;

            mCrossfadeProgress = 1.0f;

            return true;
        }

        /// <summary>
        /// Play sound from audio asset, with given volume and pitch
        /// </summary>
        /// <param name="audio">Audio Asset to play</param>
        /// <param name="volume">Volume</param>
        /// <param name="pitch">Pitch</param>
        /// <param name="priority">Priority of the sound</param>
        /// <returns>Sound reference</returns>
        public SoundReference SoundPlay(AudioAsset audio, float volume, float pitch, int priority = 0)
        {
            if (audio == null)
            {
                return new SoundReference(new SoundReference.SoundReferenceInternalState(-1, -1));
            }

            var clip = audio.audioClip;
            if (clip == null)
            {
                return new SoundReference(new SoundReference.SoundReferenceInternalState(-1, -1));
            }

            int channelSlot = GetFreeSoundChannel(priority);

            if (channelSlot != -1)
            {
                var channel = mSoundChannels[channelSlot];
                if (channel.Source != null)
                {
                    channel.Source.volume = volume;
                    channel.Source.pitch = pitch;
                    channel.Source.clip = clip;
                    channel.currentClip = clip;
                    channel.Source.loop = false;
                    channel.Source.transform.position = new Vector3(0, 0, 0);
                    channel.Source.minDistance = 50;
                    channel.Source.maxDistance = 5000;
                    channel.Source.spread = 90;
                    channel.Source.bypassEffects = true;
                    channel.Source.bypassListenerEffects = true;
                    channel.Source.bypassReverbZones = true;
                    channel.Source.dopplerLevel = 0;
                    channel.Source.spatialBlend = 0;
                    channel.Source.transform.position = Vector3.zero;

                    channel.Source.Play();

                    long seq = mSequenceCounter++;
                    channel.Sequence = seq;
                    channel.Priority = priority;
                    return new SoundReference(new SoundReference.SoundReferenceInternalState(channelSlot, seq));
                }
            }

            return new SoundReference(new SoundReference.SoundReferenceInternalState(-1, -1));
        }

        /// <summary>
        /// Checks if sound is playing
        /// </summary>
        /// <param name="soundReference">Sound reference</param>
        /// <returns>True if playing</returns>
        public bool SoundIsPlaying(SoundReference soundReference)
        {
            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return false;
            }

            return source.isPlaying;
        }

        /// <summary>
        /// Set volume of playing sound
        /// </summary>
        /// <param name="soundReference">Sound reference</param>
        /// <param name="volume">Volume</param>
        public void SoundVolumeSet(SoundReference soundReference, float volume)
        {
            if (volume < 0)
            {
                return;
            }

            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return;
            }

            source.volume = volume;
        }

        /// <summary>
        /// Set pitch of playing sound
        /// </summary>
        /// <param name="soundReference">Sound reference</param>
        /// <param name="pitch">Pitch</param>
        public void SoundPitchSet(SoundReference soundReference, float pitch)
        {
            if (pitch < 0)
            {
                return;
            }

            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return;
            }

            source.pitch = pitch;
        }

        /// <summary>
        /// Get volume of playing sound
        /// </summary>
        /// <param name="soundReference">Sound reference</param>
        /// <returns>Volume</returns>
        public float SoundVolumeGet(SoundReference soundReference)
        {
            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return 0;
            }

            return source.volume;
        }

        /// <summary>
        /// Get pitch of playing sound
        /// </summary>
        /// <param name="soundReference">Sound reference</param>
        /// <returns>Pitch</returns>
        public float SoundPitchGet(SoundReference soundReference)
        {
            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return 0;
            }

            return source.pitch;
        }

        /// <summary>
        /// Stop playing a sound
        /// </summary>
        /// <param name="soundReference">Sound reference</param>
        public void SoundStop(SoundReference soundReference)
        {
            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return;
            }

            var channel = mSoundChannels[soundReference.internalRef.SoundChannel];
            channel.Sequence = -1;

            source.Stop();
        }

        /// <summary>
        /// Set loop flag for playing sound
        /// </summary>
        /// <param name="soundReference">Sound reference</param>
        /// <param name="loop">True if should loop</param>
        public void SoundLoopSet(SoundReference soundReference, bool loop)
        {
            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return;
            }

            source.loop = loop;
        }

        /// <summary>
        /// Set sound position of a playing sound
        /// </summary>
        /// <param name="soundReference">Sound reference of playing sound</param>
        /// <param name="pos">Position to set</param>
        public void SoundPosSet(SoundReference soundReference, Vector3 pos)
        {
            var source = GetSourceForSoundReference(soundReference);
            if (source == null)
            {
                return;
            }

            Vector3 center = new Vector3(RB.DisplaySize.width / 2, RB.DisplaySize.height / 2, 0);
            center = Vector3.zero;

            source.transform.position = pos;
            source.spatialBlend = 1;
        }

        /// <summary>
        /// Play music from given <see cref="AudioAsset"/>
        /// </summary>
        /// <param name="asset">Audio asset to play</param>
        public void MusicPlay(AudioAsset asset)
        {
            // If asset is passed by it has no audioClip then treat the asset as null
            if (asset != null && asset.audioClip == null)
            {
                asset = null;
            }

            // Stop existing previous channel
            if (mPrevMusicChannel.Source != null)
            {
                mPrevMusicChannel.Source.Stop();
                mPrevMusicChannel.Source.clip = null;
            }

            mCrossfadeNoPrevious = false;
            if (musicChannel.Source == null || !musicChannel.Source.isPlaying)
            {
                mCrossfadeNoPrevious = true;
            }

            // Swap music channels
            var temp = musicChannel;
            musicChannel = mPrevMusicChannel;
            mPrevMusicChannel = temp;
            previousMusicClip = currentMusicClip;
            mPrevMusicVolume = mPrevMusicChannel.Source.volume;

            currentMusicClip = asset;
            musicChannel.currentClip = asset != null ? asset.audioClip : null;
            musicChannel.Source.volume = mTargetMusicVolume;
            musicChannel.Source.pitch = mTargetMusicPitch;
            musicChannel.Source.clip = asset != null ? asset.audioClip : null;

            mCrossfadeProgress = 0.0f;
        }

        /// <summary>
        /// Stop playing music
        /// </summary>
        public void MusicStop()
        {
            // Stopping music is the same as playing null music
            MusicPlay(null);
        }

        /// <summary>
        /// Check if music is currently playing
        /// </summary>
        /// <returns>True if playing</returns>
        public bool MusicIsPlaying()
        {
            if ((musicChannel == null || !musicChannel.Source.isPlaying) &&
                (mPrevMusicChannel == null || !mPrevMusicChannel.Source.isPlaying))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set volume of playing music
        /// </summary>
        /// <param name="volume">Volume</param>
        public void MusicVolumeSet(float volume)
        {
            mTargetMusicVolume = volume;

            // If not already playing music then snap the volume to request value instead of lerping
            if (musicChannel != null && currentMusicClip == null && musicChannel.Source != null)
            {
                musicChannel.Source.volume = volume;
            }
        }

        /// <summary>
        /// Set pitch of playing music
        /// </summary>
        /// <param name="pitch">Pitch</param>
        public void MusicPitchSet(float pitch)
        {
            mTargetMusicPitch = pitch;

            // If not already playing music then snap the pitch to request value instead of lerping
            if (musicChannel != null && currentMusicClip == null && musicChannel.Source != null)
            {
                musicChannel.Source.pitch = pitch;
            }
        }

        /// <summary>
        /// Get volume of playing music
        /// </summary>
        /// <returns>Volume</returns>
        public float MusicVolumeGet()
        {
            return mTargetMusicVolume;
        }

        /// <summary>
        /// Get pitch of playing music
        /// </summary>
        /// <returns>Pitch</returns>
        public float MusicPitchGet()
        {
            return mTargetMusicPitch;
        }

        /// <summary>
        /// Set music crossfade duration and overlap
        /// </summary>
        /// <remarks>
        /// With overlap of 0.0 the previous music will completely fade out before new music starts fading in.
        /// 
        /// With overlap of 1.0 the previous and new music will fade at the same time.
        /// 
        /// With overlap of -1.0 the previous music will fade out completely, a moment a silence will follow, and then the new music will start fading in.
        /// </remarks>
        /// <param name="fadeDuration">Fade duration in seconds, default 1s</param>
        /// <param name="overlap">Overlap from -1.0 to 1.0, default 0</param>
        /// <param name="fadeOutFunc">Easing function to use while fading out, default Ease.QuadOut</param>
        /// <param name="fadeInFunc">Easing function to use while fading in, default Ease.QuadOut</param>
        public void MusicCrossFadeSet(float fadeDuration, float overlap, Ease.Func fadeOutFunc, Ease.Func fadeInFunc)
        {
            overlap = Mathf.Clamp(overlap, -1.0f, 1.0f);
            if (fadeDuration < 0)
            {
                fadeDuration = 0;
            }

            mCrossfadeDuration = fadeDuration;
            mCrossfadeOverlap = overlap;

            mCrossfadeFadeOutFunc = fadeOutFunc;
            mCrossfadeFadeInFunc = fadeInFunc;
        }

        /// <summary>
        /// Called when sound asset was unloaded, gives chance to stop the sound source
        /// </summary>
        /// <param name="asset">Unloaded asset</param>
        public void SoundAssetWasUnloaded(AudioAsset asset)
        {
            // Free up music channel
            if (currentMusicClip == asset)
            {
                currentMusicClip = null;
                musicChannel.Source.clip = null;
                musicChannel.currentClip = null;
                musicChannel.Source.time = 0;
                musicChannel.Source.Stop();
            }

            // Free up music channel
            if (previousMusicClip == asset)
            {
                previousMusicClip = null;
                mPrevMusicChannel.Source.clip = null;
                mPrevMusicChannel.currentClip = null;
                mPrevMusicChannel.Source.time = 0;
                mPrevMusicChannel.Source.Stop();
            }

            // Free up audio channels
            for (int i = 0; i < mSoundChannels.Length; i++)
            {
                if (mSoundChannels[i].Source.clip == asset.audioClip)
                {
                    mSoundChannels[i].Sequence = -1;
                    mSoundChannels[i].Source.clip = null;
                    mSoundChannels[i].currentClip = null;
                    mSoundChannels[i].Source.time = 0;
                    mSoundChannels[i].Source.Stop();
                }
            }
        }

        /// <summary>
        /// Get AudioSource from a sound reference
        /// </summary>
        /// <param name="soundRef">Sound reference</param>
        /// <returns>AudioSource</returns>
        public AudioSource GetSourceForSoundReference(SoundReference soundRef)
        {
            if (soundRef.internalRef.Sequence < 0)
            {
                return null;
            }

            SoundChannel channel;

            if (soundRef.internalRef.SoundChannel >= 0 && soundRef.internalRef.SoundChannel < RBHardware.HW_MAX_SOUND_CHANNELS)
            {
                channel = mSoundChannels[soundRef.internalRef.SoundChannel];
            }
            else
            {
                return null;
            }

            // Check if this reference is outdated
            if (channel.Sequence != soundRef.internalRef.Sequence)
            {
                return null;
            }

            return channel.Source;
        }

        /// <summary>
        /// Set sound listener position
        /// </summary>
        /// <param name="pos">New position</param>
        public void SoundListenerPosSet(Vector2i pos)
        {
            mAudioListenerObj.transform.position = new Vector3(pos.x, pos.y, 0);
        }

        /// <summary>
        /// Check if given audio clip is playing on any channel
        /// </summary>
        /// <param name="clip">Audio clip to check</param>
        /// <returns>True if playing, false if not</returns>
        public bool IsClipPlaying(AudioClip clip)
        {
            for (int i = 0; i < mSoundChannels.Length; i++)
            {
                if (mSoundChannels[i].currentClip == clip)
                {
                    return true;
                }
            }

            if (musicChannel.currentClip == clip)
            {
                return true;
            }

            if (mPrevMusicChannel.currentClip == clip)
            {
                return true;
            }

            return false;
        }

        private void Update()
        {
            if (mCrossfadeDuration >= 0.001f)
            {
                if (mCrossfadeNoPrevious)
                {
                    mCrossfadeProgress = 1.0f;
                }
                else
                {
                    mCrossfadeProgress += Time.deltaTime / mCrossfadeDuration;
                    mCrossfadeProgress = Mathf.Clamp01(mCrossfadeProgress);
                }
            }
            else
            {
                mCrossfadeProgress = 1.0f;
            }

            float fadeOutProgress = 1.0f - Mathf.Clamp01(1.0f - ((1.0f + (1.0f - mCrossfadeOverlap)) * mCrossfadeProgress));
            float fadeOutVolume = Ease.Interpolate(mCrossfadeFadeOutFunc, 0.0f, 1.0f, fadeOutProgress);

            float fadeInProgress = Mathf.Clamp01(1.0f - ((1.0f + (1.0f - mCrossfadeOverlap)) * (1.0f - mCrossfadeProgress)));
            float fadeInVolume = Ease.Interpolate(mCrossfadeFadeInFunc, 0.0f, 1.0f, fadeInProgress);

            if (mPrevMusicChannel != null && mPrevMusicChannel.Source != null)
            {
                // Previous music channel finished fading out, make sure it stops playing
                if (fadeOutProgress >= 1.0f && mPrevMusicChannel.Source != null && mPrevMusicChannel.Source.isPlaying)
                {
                    mPrevMusicChannel.Source.Stop();
                    mPrevMusicChannel.Source.clip = null;
                    previousMusicClip = null;
                }
                else
                {
                    mPrevMusicChannel.Source.volume = mPrevMusicVolume * (1.0f - fadeOutVolume);
                }
            }

            if (musicChannel != null && musicChannel.Source != null)
            {
                // Music channel started fading in, make sure its playing
                if (fadeInProgress > 0.0f && musicChannel.Source != null && !musicChannel.Source.isPlaying && musicChannel.Source.clip != null)
                {
                    musicChannel.Source.Play();
                }

                musicChannel.Source.volume = mTargetMusicVolume * fadeInVolume;
            }
        }

        private AudioSource CreateAudioChannelChild(string name)
        {
            var channelObj = new GameObject();
            channelObj.transform.parent = transform;

            var audioSource = channelObj.AddComponent<AudioSource>();
            audioSource.name = name;

            return audioSource;
        }

        private void InitializeChannels()
        {
            // Delete any existing audio sources
            var destroyList = new List<GameObject>();

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                if (child.name.Contains("Channel"))
                {
                    destroyList.Add(child.gameObject);
                }
            }

            for (int i = 0; i < destroyList.Count; i++)
            {
                Destroy(destroyList[i]);
            }

            destroyList.Clear();

            AudioSource audioSource;

            for (int i = 0; i < mSoundChannels.Length; i++)
            {
                audioSource = CreateAudioChannelChild("RetroBlitSoundChannel" + i);
                audioSource.bypassEffects = true;
                audioSource.bypassListenerEffects = true;
                audioSource.bypassReverbZones = true;
                audioSource.dopplerLevel = 0;
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0;
                audioSource.volume = 1.0f;

                mSoundChannels[i] = new SoundChannel(audioSource);
            }

            audioSource = CreateAudioChannelChild("RetroBlitMusicChannel");
            audioSource.bypassEffects = true;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.dopplerLevel = 0;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0;
            audioSource.priority = 255;
            audioSource.volume = 0.0f;

            musicChannel = new SoundChannel(audioSource);

            audioSource = CreateAudioChannelChild("RetroBlitPreviousMusicChannel");
            audioSource.bypassEffects = true;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.dopplerLevel = 0;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0;
            audioSource.priority = 255;
            audioSource.volume = 0.0f;

            mPrevMusicChannel = new SoundChannel(audioSource);
        }

        private int GetFreeSoundChannel(int priority)
        {
            if (mSoundChannels[0] == null)
            {
                InitializeChannels();
            }

            int lowestPriority = priority;
            int lowestPriorityIndex = -1;
            long lowestPrioritySeq = long.MaxValue;

            for (int i = 0; i < mSoundChannels.Length; i++)
            {
                var channel = mSoundChannels[i];

                if (channel.Source == null)
                {
                    continue;
                }

                if (channel.Source.isPlaying == false)
                {
                    return i;
                }

                if (channel.Priority < lowestPriority || (channel.Priority == lowestPriority && channel.Sequence < lowestPrioritySeq))
                {
                    lowestPriority = channel.Priority;
                    lowestPriorityIndex = i;
                    lowestPrioritySeq = channel.Sequence;
                }
            }

            // No more channels available, boot out oldest sound of equal or lower priority
            if (lowestPriorityIndex != -1)
            {
                var channel = mSoundChannels[lowestPriorityIndex];

                channel.Sequence = -1;
                channel.Source.Stop();

                return lowestPriorityIndex;
            }

            return -1;
        }

        /// <summary>
        /// Sound channel info
        /// </summary>
        public class SoundChannel
        {
            /// <summary>
            /// AudioSource object
            /// </summary>
            public AudioSource Source;

            /// <summary>
            /// Sequence
            /// </summary>
            public long Sequence;

            /// <summary>
            /// Priority
            /// </summary>
            public int Priority;

            /// <summary>
            /// A duplication of Source.clip to be used on threads that are not main thread
            /// </summary>
            public AudioClip currentClip;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="source">Audio source</param>
            public SoundChannel(AudioSource source)
            {
                Source = source;
                Sequence = 0;
                Priority = -1;
            }
        }
    }
}
