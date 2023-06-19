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
    /// Internal TMX map loader
    /// </summary>
    public sealed class RBTMXMapLoader
    {
        /// <summary>
        /// Map path
        /// </summary>
        public string path;

        /// <summary>
        /// TMX map asset to load into
        /// </summary>
        public TMXMapAsset mapAsset;

        private RB.AssetSource mSource = RB.AssetSource.ResourcesAsync;

        private ResourceRequest mResourceRequest = null;
        private UnityWebRequest mWebRequest = null;
#if ADDRESSABLES_PACKAGE_AVAILABLE
        private AsyncOperationHandle<TextAsset> mAddressableRequest;
#endif

        /// <summary>
        /// Update asynchronous loading
        /// </summary>
        public void Update()
        {
            // If not loading then there is nothing to update
            if (mapAsset == null || mapAsset.status != RB.AssetStatus.Loading)
            {
                return;
            }

            UpdateMapInfo();
        }

        /// <summary>
        /// Load Map
        /// </summary>
        /// <param name="path">Path to load from</param>
        /// <param name="asset">TMXMapAsset to load into</param>
        /// <param name="source">Asset source</param>
        /// <returns>True if successful</returns>
        public bool Load(string path, TMXMapAsset asset, RB.AssetSource source)
        {
            this.mapAsset = asset;
            this.path = path;
            mSource = source;

            if (asset == null)
            {
                Debug.LogError("TMXMapAsset is null!");
                return false;
            }

            LoadMapInfo();

            return true;
        }

        /// <summary>
        /// Abort asynchronous loading
        /// </summary>
        public void Abort()
        {
            if (mapAsset == null)
            {
                return;
            }

            if (mapAsset.status != RB.AssetStatus.Loading)
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
            mAddressableRequest = new AsyncOperationHandle<TextAsset>();
#endif
        }

        private bool LoadMapInfo()
        {
            string infoPath;

            path = path + ".tmx.rb/";

            mapAsset.internalState.mapDef.realPathName = path;

            mapAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

            if (mSource == RB.AssetSource.WWW || mSource == RB.AssetSource.AddressableAssets)
            {
                infoPath = path + "info.bytes";
            }
            else
            {
                infoPath = path + "info";
            }

            // Check if this is a web request
            if (mSource == RB.AssetSource.WWW)
            {
                mWebRequest = UnityWebRequest.Get(infoPath);
                if (mWebRequest == null)
                {
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                if (mWebRequest.SendWebRequest() == null)
                {
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                mapAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
            else if (mSource == RB.AssetSource.ResourcesAsync)
            {
                mResourceRequest = Resources.LoadAsync<TextAsset>(infoPath);

                if (mResourceRequest == null)
                {
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mSource == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequest = Addressables.LoadAssetAsync<TextAsset>(infoPath);
                }
                catch (UnityEngine.AddressableAssets.InvalidKeyException e)
                {
                    RBUtil.Unused(e);
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }
                catch (Exception e)
                {
                    RBUtil.Unused(e);
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                // Check for an immediate failure
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(mAddressableRequest);
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                mapAsset.progress = 0;
                mapAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
#endif
            mapAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

            return true;
        }

        private void UpdateMapInfo()
        {
            byte[] loadedBytes = null;

            if (mResourceRequest != null)
            {
                if (mResourceRequest.isDone)
                {
                    if (mResourceRequest.asset == null)
                    {
                        mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }
                    else
                    {
                        var textAsset = (TextAsset)mResourceRequest.asset;
                        loadedBytes = textAsset.bytes;
                    }
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
                            mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NetworkError);
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

                            mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, resultError);
                        }
                        else
                        {
                            loadedBytes = mWebRequest.downloadHandler.data;
                        }
                    }
                    else
                    {
                        mapAsset.progress = Mathf.Clamp01(mWebRequest.downloadProgress);
                    }
                }
                catch (Exception)
                {
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mAddressableRequest.IsValid())
            {
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    // Can't really figure out failure reason
                    Addressables.Release(mAddressableRequest);
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return;
                }
                else if (mAddressableRequest.Status == AsyncOperationStatus.Succeeded)
                {
                    var textAsset = mAddressableRequest.Result;
                    if (textAsset != null)
                    {
                        loadedBytes = textAsset.bytes;
                        // Don't release yet, wait for loadedBytes to be processed
                    }
                    else
                    {
                        // Can't really figure out failure reason
                        Addressables.Release(mAddressableRequest);
                        mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }
                }

                if (!mAddressableRequest.IsDone)
                {
                    mapAsset.progress = mAddressableRequest.PercentComplete;
                    return;
                }
            }
#endif

            if (loadedBytes != null)
            {
                if (FinalizeMapInfo(loadedBytes))
                {
                    mapAsset.progress = 1;
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                }
                else
                {
                    mapAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                }

#if ADDRESSABLES_PACKAGE_AVAILABLE
                if (mAddressableRequest.IsValid())
                {
                    Addressables.Release(mAddressableRequest);
                }
#endif
            }
        }

        private bool FinalizeMapInfo(byte[] bytes)
        {
            RetroBlitInternal.RBAPI.instance.Tilemap.FinalizeTMXInfo(mapAsset.internalState.mapDef, path, mapAsset, bytes);

            return true;
        }
    }
}
