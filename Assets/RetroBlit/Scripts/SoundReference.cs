/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// A reference to currently playing sound
/// </summary>
/// <remarks>
/// A reference to currently playing sound. This structure is returned by <see cref = "RB.SoundPlay"/> and can be used with
/// <see cref="RB.SoundStop"/>, <see cref="RB.SoundVolumeSet"/>, <see cref="RB.SoundVolumeGet"/>, <see cref="RB.SoundPitchSet"/>
/// and <see cref="RB.SoundPitchGet"/> to manage the sound as it plays.
/// </remarks>
public struct SoundReference
{
    private SoundReferenceInternalState mInternalRef;

    /// <summary>
    /// Sound reference constructor
    /// </summary>
    /// <remarks>
    /// There is no reason to construct a SoundReference, it's usually constructed by RetroBlit and used to refer to already playing sounds.
    /// <seedoc>Features:Sound</seedoc>
    /// </remarks>
    /// <param name="internalRef">Internal sound reference</param>
    public SoundReference(SoundReferenceInternalState internalRef)
    {
        mInternalRef = internalRef;
    }

    /// <summary>
    /// Internal state, do not change.
    /// </summary>
    public SoundReferenceInternalState internalRef
    {
        get
        {
            return mInternalRef;
        }
    }

    /// <summary>
    /// Check whether the SoundReference is valid. A SoundReference can become invalid if it stopped playing and it's audio channel is now occupied by a different sound.
    /// </summary>
    /// <returns>True if valid</returns>
    public bool IsValid()
    {
        return RetroBlitInternal.RBAPI.instance.Audio.GetSourceForSoundReference(this) != null;
    }

    /// <summary>
    /// Internal state
    /// </summary>
    public struct SoundReferenceInternalState
    {
        private int mSoundChannel;
        private long mSequence;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="soundChannel">Sound channel</param>
        /// <param name="sequence">Sequence</param>
        public SoundReferenceInternalState(int soundChannel, long sequence)
        {
            mSoundChannel = soundChannel;
            mSequence = sequence;
        }

        /// <summary>
        /// Sound channel
        /// </summary>
        /// <remarks>
        /// Refers to the channel the sound is currently playing on. Used internally by RetroBlit.
        /// </remarks>
        public int SoundChannel
        {
            get
            {
                return mSoundChannel;
            }
        }

        /// <summary>
        /// Sound sequence.
        /// </summary>
        /// <remarks>
        /// Refers to the sequence of the sound used internally by RetroBlit.
        /// </remarks>
        public long Sequence
        {
            get
            {
                return mSequence;
            }
        }
    }
}
