namespace RetroBlitInternal
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;
#if ADDRESSABLES_PACKAGE_AVAILABLE
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
#endif

    /// <summary>
    /// Internal audio loading class
    /// </summary>
    public sealed class RBAudioLoader
    {
        /// <summary>
        /// Path of the asset
        /// </summary>
        public string path;

        /// <summary>
        /// Audio asset to load into
        /// </summary>
        public AudioAsset audioAsset;

        private ResourceRequest mResourceRequest = null;
        private UnityWebRequest mWebRequest = null;

#if ADDRESSABLES_PACKAGE_AVAILABLE
        private AsyncOperationHandle<AudioClip> mAddressableRequest;
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        public RBAudioLoader()
        {
        }

        /// <summary>
        /// Update asynchronous loading
        /// </summary>
        public void Update()
        {
            // If not loading then there is nothing to update
            if (audioAsset == null || audioAsset.status != RB.AssetStatus.Loading)
            {
                return;
            }

            if (mResourceRequest != null)
            {
                if (mResourceRequest.isDone)
                {
                    if (mResourceRequest.asset == null)
                    {
                        audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }
                    else
                    {
                        audioAsset.audioClip = (AudioClip)mResourceRequest.asset;
                        audioAsset.progress = 1;
                        audioAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                    }
                }
                else
                {
                    audioAsset.progress = mResourceRequest.progress;
                }
            }
            else if (mWebRequest != null)
            {
                try
                {
                    if (mWebRequest.isDone)
                    {
#if UNITY_2020_1_OR_NEWER
                        if (mWebRequest.result == UnityWebRequest.Result.ConnectionError)
#else
                        if (mWebRequest.isNetworkError)
#endif
                        {
                            audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NetworkError);
                        }
#if UNITY_2020_1_OR_NEWER
                        else if (mWebRequest.result == UnityWebRequest.Result.ProtocolError)
#else
                        else if (mWebRequest.isHttpError)
#endif
                        {
                            // Start with generic "ServerError" for all HTTP errors
                            var resultError = RB.Result.ServerError;

                            // Assign specific code for common HTTP errors
                            switch (mWebRequest.responseCode)
                            {
                                case 400:
                                    resultError = RB.Result.BadParam;
                                    break;

                                case 403:
                                    resultError = RB.Result.NoPermission;
                                    break;

                                case 404:
                                    resultError = RB.Result.NotFound;
                                    break;

                                case 500:
                                    resultError = RB.Result.ServerError;
                                    break;
                            }

                            audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, resultError);
                        }
                        else
                        {
                            audioAsset.progress = 1;
                            audioAsset.audioClip = DownloadHandlerAudioClip.GetContent(mWebRequest);
                            audioAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                        }
                    }
                    else
                    {
                        audioAsset.progress = Mathf.Clamp01(mWebRequest.downloadProgress);
                    }
                }
                catch (Exception)
                {
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mAddressableRequest.IsValid())
            {
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    // Can't really figure out failure reason
                    Addressables.Release(mAddressableRequest);
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return;
                }
                else if (mAddressableRequest.Status == AsyncOperationStatus.Succeeded)
                {
                    audioAsset.progress = 1;
                    audioAsset.audioClip = mAddressableRequest.Result;
                    audioAsset.addressableHandle = mAddressableRequest;
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                    return;
                }

                if (!mAddressableRequest.IsDone)
                {
                    audioAsset.progress = mAddressableRequest.PercentComplete;
                    return;
                }
            }
#endif
        }

        /// <summary>
        /// Load audio asset
        /// </summary>
        /// <param name="path">Path to load from</param>
        /// <param name="asset">AudioAsset to load into</param>
        /// <param name="source">Source type</param>
        /// <returns>True if successful</returns>
        public bool Load(string path, AudioAsset asset, RB.AssetSource source)
        {
            audioAsset = asset;
            this.path = path;

            if (path == null)
            {
                audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
            }

            if (asset == null)
            {
                Debug.LogError("AudioAsset is null!");
                return false;
            }

            if (source == RB.AssetSource.Resources)
            {
                // Synchronous load
                if (path == null)
                {
                    Debug.LogError("Audio filename is null!");
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                    return false;
                }

#if !RETROBLIT_STANDALONE
                var clip = Resources.Load<AudioClip>(path);
#else
                var clip = Resources.LoadAudioSample(path);
#endif

                if (clip == null)
                {
                    Debug.LogError("Can't find sound file " + path + ", it must be under the Assets/Resources folder. " +
                        "If you're trying to load from an WWW address, or Addressable Assets then please specify so with the \"source\" parameter.");

                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }

                // If current music clip is affected then update the clip
                if (asset == RetroBlitInternal.RBAPI.instance.Audio.currentMusicClip)
                {
                    var channel = RetroBlitInternal.RBAPI.instance.Audio.musicChannel;
                    if (channel.Source != null)
                    {
                        channel.Source.clip = clip;
                        channel.Source.loop = true;
                        channel.Source.Play();
                    }
                }

                asset.audioClip = clip;
                asset.progress = 1;
                audioAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

                return true;
            }
            else if (source == RB.AssetSource.WWW)
            {
                var audioType = AudioTypeFromPath(path);
                if (audioType == AudioType.UNKNOWN)
                {
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
                    return false;
                }

                mWebRequest = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
                if (mWebRequest == null)
                {
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                if (mWebRequest.SendWebRequest() == null)
                {
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                audioAsset.progress = 0;
                audioAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (source == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequest = Addressables.LoadAssetAsync<AudioClip>(path);
                }
                catch (UnityEngine.AddressableAssets.InvalidKeyException e)
                {
                    RBUtil.Unused(e);
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }
                catch (Exception e)
                {
                    RBUtil.Unused(e);
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                // Check for an immediate failure
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    audioAsset.addressableHandle = mAddressableRequest;
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                audioAsset.progress = 0;
                audioAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
#endif
            else if (source == RB.AssetSource.ResourcesAsync)
            {
                // Finally attempt async resource load
                mResourceRequest = Resources.LoadAsync<AudioClip>(this.path);

                if (mResourceRequest == null)
                {
                    audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                audioAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);
            }
            else
            {
                audioAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Abort asynchronous loading
        /// </summary>
        public void Abort()
        {
            if (audioAsset == null)
            {
                return;
            }

            if (audioAsset.status != RB.AssetStatus.Loading)
            {
                return;
            }

            if (mResourceRequest != null)
            {
                // Can't abort a ResourceRequest... we will just ignore it
                mResourceRequest = null;
            }

            if (mWebRequest != null)
            {
                mWebRequest.Dispose();
                mWebRequest = null;
            }

#if ADDRESSABLES_PACKAGE_AVAILABLE
            Addressables.Release(mAddressableRequest);
            mAddressableRequest = new AsyncOperationHandle<AudioClip>();
#endif
        }

        private AudioType AudioTypeFromPath(string path)
        {
            if (path.EndsWith(".ogg"))
            {
                return AudioType.OGGVORBIS;
            }
            else if (path.EndsWith(".wav"))
            {
                return AudioType.WAV;
            }
            else if (path.EndsWith(".mp3"))
            {
                return AudioType.MPEG;
            }

            return AudioType.UNKNOWN;
        }
    }
}
