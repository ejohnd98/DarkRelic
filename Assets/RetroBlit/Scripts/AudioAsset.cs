using System;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

#if ADDRESSABLES_PACKAGE_AVAILABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

/// <summary>
/// Audio asset
/// </summary>
/// <remarks>
/// Audio asset which can hold a sound effect or music. Audio assets can be loaded from various sources, including synchronous and asynchronous sources. Use <see cref="AudioAsset.Load"/> to load an audio asset.
/// </remarks>
public class AudioAsset : RBAsset
{
    /// <summary>
    /// AudioClip containing the sound data
    /// </summary>
    /// <remarks>
    /// A Unity <b>AudioClip</b> object containing the audio data. In most cases it should not be necessary to ever reference this directly,
    /// RetroBlit will use this value when playing the sound.
    /// </remarks>
    public AudioClip audioClip;

#if ADDRESSABLES_PACKAGE_AVAILABLE
    private AsyncOperationHandle<AudioClip> mAddressableRequest;
#endif

    ~AudioAsset()
    {
        if (RetroBlitInternal.RBAPI.instance.Audio.IsClipPlaying(audioClip))
        {
            // Do nothing if the audio clip is playing on some channel
            return;
        }

        // Careful here, this finalizer could be called at any point, even when RetroBlit is not initialized anymore! Also must be on main thread.
        if (RetroBlitInternal.RBAPI.instance != null && RetroBlitInternal.RBAPI.instance.Audio != null && RetroBlitInternal.RBUtil.OnMainThread())
        {
            RetroBlitInternal.RBAPI.instance.AssetManager.SoundUnload(this);
        }
        else
        {
            // Otherwise queue up the resource for later release
            RetroBlitInternal.RBAPI.instance.AssetManager.AudioClipUnloadMainThread(audioClip);
        }
    }

#if ADDRESSABLES_PACKAGE_AVAILABLE
    /// <summary>
    /// Addressable Asset handle, used internally by RetroBlit
    /// </summary>
    public AsyncOperationHandle<AudioClip> addressableHandle
    {
        set
        {
            mAddressableRequest = value;
        }
    }
#endif

    /// <summary>
    /// Load an audio asset from the given source
    /// </summary>
    /// <remarks>
    /// Load an audio asset which can be used to play a sound or play music. There are various asset sources supported:
    /// <list type="bullet">
    /// <item><b>Resources</b> - Synchronously loaded audio assets from a <b>Resources</b> folder. This was the only asset source supported in RetroBlit prior to 3.0.</item>
    /// <item><b>ResourcesAsync</b> - Asynchronously loaded audio assets from a <b>Resources</b> folder.</item>
    /// <item><b>WWW</b> - Asynchronously loaded audio assets from a URL.</item>
    /// <item><b>AddressableAssets</b> - Asynchronously loaded audio assets from Unity Addressable Assets.</item>
    /// <item><b>Existing Assets</b> - Synchronously loaded audio assets from an existing Unity <b>AudioClip</b>.</item>
    /// </list>
    ///
    /// If the asset is loaded via a synchronous method then <b>Load</b> will block until the loading is complete.
    /// If the asset is loaded via an asynchronous method then <b>Load</b> will immediately return and the asset loading will
    /// continue in a background thread. The status of an asynchronous loading asset can be check by looking at <see cref="RBAsset.status"/>,
    /// or by using the event system with <see cref="RBAsset.OnLoadComplete"/> to get a callback when the asset is done loading.
    ///
    /// <seedoc>Features:Audio</seedoc>
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <code>
    /// AudioAsset sndBleepBloop = new AudioAsset();
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from Resources asynchronously. This method call will immediately return without blocking.
    ///     sndBleepBloop.Load("bleepbloop", RB.AssetSource.ResourcesAsync);
    /// }
    ///
    /// public void Update()
    /// {
    ///     if (RB.AnyKeyPressed() && sndBleepBloop.status == RB.AssetStatus.Ready)
    ///     {
    ///         RB.SoundPlay(sndBleepBloop);
    ///     }
    /// }
    /// </code>
    /// <param name="filename">File name</param>
    /// <param name="source">Asset source type</param>
    /// <returns>Load status</returns>
    /// <seealso cref="RB.Result"/>
    /// <seealso cref="RB.AssetStatus"/>
    /// <seealso cref="RB.AssetSource"/>
    public RB.AssetStatus Load(string filename, RB.AssetSource source = RB.AssetSource.Resources)
    {
        Unload();

        if (!RetroBlitInternal.RBAssetManager.CheckSourceSupport(source))
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
            return status;
        }

        RetroBlitInternal.RBAPI.instance.AssetManager.SoundLoad(filename, this, source);

        return status;
    }

    /// <summary>
    /// Load an audio asset from an existing Unity AudioClip object.
    /// </summary>
    /// <param name="existingClip">Existing AudioClip</param>
    /// <returns>Load status</returns>
    public RB.AssetStatus Load(AudioClip existingClip)
    {
        Unload();
        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

        if (existingClip == null)
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
            return status;
        }

        audioClip = existingClip;
        progress = 1;

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        return status;
    }

    /// <summary>
    /// Unload a previously loaded audio asset.
    /// </summary>
    public override void Unload()
    {
        RetroBlitInternal.RBAPI.instance.AssetManager.SoundUnload(this);
        progress = 0;

#if ADDRESSABLES_PACKAGE_AVAILABLE
        if (mAddressableRequest.IsValid())
        {
            Addressables.Release(mAddressableRequest);
        }
#endif

        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);
    }
}
