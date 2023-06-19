namespace RetroBlitInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;
#if ADDRESSABLES_PACKAGE_AVAILABLE
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
#endif

    /// <summary>
    /// Internal TMX layer loading class
    /// </summary>
    public sealed class RBTMXLayerLoader
    {
        /// <summary>
        /// Layer loading state
        /// </summary>
        public TMXMapAsset.TMXLayerLoadState layerState;

        private static FastString mWorkStr = new FastString(2048);

        private RB.AssetSource mSource = RB.AssetSource.ResourcesAsync;

        private ResourceRequest mResourceRequest = null;
        private UnityWebRequest mWebRequest = null;
#if ADDRESSABLES_PACKAGE_AVAILABLE
        private AsyncOperationHandle<TextAsset> mAddressableRequest;
#endif

        private Rect2i mSourceRect;
        private Vector2i mDestPos;
        private PackedSpriteID[] mPackedSpriteLookup;
        private string mTmxSourceLayer;
        private int mDestinationLayer;

        private string mPath;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="layerState">Layer state</param>
        /// <param name="tmxSourceLayer">TMX source layer</param>
        /// <param name="destinationLayer">Destination layer</param>
        /// <param name="sourceRect">Source rectangle</param>
        /// <param name="destPos">Destination position</param>
        /// <param name="packedSpriteLookup">Packed sprite lookup</param>
        /// <param name="source">Source type</param>
        public RBTMXLayerLoader(TMXMapAsset.TMXLayerLoadState layerState, string tmxSourceLayer, int destinationLayer, Rect2i sourceRect, Vector2i destPos, PackedSpriteID[] packedSpriteLookup, RB.AssetSource source)
        {
            if (layerState == null)
            {
                Debug.LogError("LayerLoadState is null!");
                return;
            }

            this.layerState = layerState;

            mSource = source;
            mSourceRect = sourceRect;
            mDestPos = destPos;
            mPackedSpriteLookup = packedSpriteLookup;
            mTmxSourceLayer = tmxSourceLayer;
            mDestinationLayer = destinationLayer;

            LoadLayerInfo();

            return;
        }

        /// <summary>
        /// Update asynchronous loading
        /// </summary>
        public void Update()
        {
            // If not loading then there is nothing to update
            if (layerState == null || layerState.status != RB.AssetStatus.Loading)
            {
                return;
            }

            UpdateMapInfo();
        }

        /// <summary>
        /// Abort asynchronous loading
        /// </summary>
        public void Abort()
        {
            if (layerState == null)
            {
                return;
            }

            if (layerState.status != RB.AssetStatus.Loading)
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

        private bool LoadLayerInfo()
        {
            RBTilemapTMX.TMXMapDef map = null;

            map = layerState.map.internalState.mapDef;

            if (map == null || map.realPathName == null || map.realPathName.Length == 0 || map.layers == null)
            {
                Debug.LogError("Can't load TMX layer, invalid map, or map not loaded yet!");
                layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return false;
            }

            if (map.infinite)
            {
                Debug.LogError("TMX map is infinite, use MapLoadTMXLayerChunk() instead");
                layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return false;
            }

            if (!map.layers.ContainsKey(mTmxSourceLayer))
            {
                Debug.LogError("Layer " + mTmxSourceLayer + " not found");
                layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                return false;
            }

            var tmxLayer = (RBTilemapTMX.TMXLayerDef)map.layers[mTmxSourceLayer];

            mPath = map.realPathName + "layers";

            if (mSource == RB.AssetSource.WWW || mSource == RB.AssetSource.AddressableAssets)
            {
                mPath += ".bytes";
            }

            // Check if this is a web request
            if (mSource == RB.AssetSource.WWW)
            {
                mWebRequest = UnityWebRequest.Get(mPath);
                if (mWebRequest == null)
                {
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                if (mWebRequest.SendWebRequest() == null)
                {
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                layerState.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
            else if (mSource == RB.AssetSource.ResourcesAsync)
            {
                mResourceRequest = Resources.LoadAsync<TextAsset>(mPath);

                if (mResourceRequest == null)
                {
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);

                    return false;
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mSource == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequest = Addressables.LoadAssetAsync<TextAsset>(mPath);
                }
                catch (UnityEngine.AddressableAssets.InvalidKeyException e)
                {
                    RBUtil.Unused(e);
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }
                catch (Exception e)
                {
                    RBUtil.Unused(e);
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                // Check for an immediate failure
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(mAddressableRequest);
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                layerState.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);
                layerState.progress = 0;

                return true;
            }
#endif

            layerState.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

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
                        layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
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
                            layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NetworkError);
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

                            layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, resultError);
                        }
                        else
                        {
                            loadedBytes = mWebRequest.downloadHandler.data;
                        }
                    }
                    else
                    {
                        layerState.progress = Mathf.Clamp01(mWebRequest.downloadProgress);
                    }
                }
                catch (Exception)
                {
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mAddressableRequest.IsValid())
            {
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    // Can't really figure out failure reason
                    Addressables.Release(mAddressableRequest);
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return;
                }
                else if (mAddressableRequest.Status == AsyncOperationStatus.Succeeded)
                {
                    var textAsset = mAddressableRequest.Result;
                    if (textAsset != null)
                    {
                        loadedBytes = textAsset.bytes;
                        // Do not release yet, wait until loadedBytes is processed
                    }
                    else
                    {
                        // Can't really figure out failure reason
                        Addressables.Release(mAddressableRequest);
                        layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }
                }

                if (!mAddressableRequest.IsDone)
                {
                    layerState.progress = mAddressableRequest.PercentComplete;
                    return;
                }
            }
#endif

            if (loadedBytes != null)
            {
                if (FinalizeLayerInfo(loadedBytes))
                {
                    layerState.progress = 1;
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                }
                else
                {
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                }

#if ADDRESSABLES_PACKAGE_AVAILABLE
                if (mAddressableRequest.IsValid())
                {
                    Addressables.Release(mAddressableRequest);
                }
#endif
            }
        }

        private bool FinalizeLayerInfo(byte[] bytes)
        {
            var map = layerState.map.internalState.mapDef;

            var layerNameHash = mWorkStr.Set(mTmxSourceLayer).ToLowerInvariant().GetHashCode();

            byte[] byteBuf = RBTilemapTMX.GetLayerBytesFromLayerPack(layerNameHash, bytes);
            if (byteBuf == null)
            {
                return false;
            }

            RetroBlitInternal.RBAPI.instance.Tilemap.FinalizeLayerLoad(map, mTmxSourceLayer, mDestinationLayer, mSourceRect, mDestPos, mPackedSpriteLookup, layerState, byteBuf);

            return true;
        }
    }
}
