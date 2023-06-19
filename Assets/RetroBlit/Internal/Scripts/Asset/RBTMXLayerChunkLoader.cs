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
    /// Internal TMX layer chunk loader
    /// </summary>
    public sealed class RBTMXLayerChunkLoader
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

        private Vector2i mChunkOffset;
        private Vector2i mDestPos;
        private PackedSpriteID[] mPackedSpriteLookup;
        private string mTmxSourceLayer;
        private int mDestinationLayer;
        private RBTilemapTMX.ChunkDef mChunkDef;
        private RBTilemapTMX.RetroBlitTuple<int, ulong> mTupleKey;

        private string mIndexPath;

        private Dictionary<ulong, RBTilemapTMX.ChunkDef> mChunkIndex = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="layerState">Layer state</param>
        /// <param name="tmxSourceLayer">TMX source layer</param>
        /// <param name="destinationLayer">Destination layer</param>
        /// <param name="chunkOffset">Chunk offset</param>
        /// <param name="destPos">Destination position</param>
        /// <param name="packedSpriteLookup">Packed sprite lookup</param>
        /// <param name="source">Source type</param>
        public RBTMXLayerChunkLoader(TMXMapAsset.TMXLayerLoadState layerState, string tmxSourceLayer, int destinationLayer, Vector2i chunkOffset, Vector2i destPos, PackedSpriteID[] packedSpriteLookup, RB.AssetSource source)
        {
            if (layerState == null)
            {
                Debug.LogError("LayerLoadState is null!");
                return;
            }

            this.layerState = layerState;

            mSource = source;
            mChunkOffset = chunkOffset;
            mDestPos = destPos;
            mPackedSpriteLookup = packedSpriteLookup;
            mTmxSourceLayer = tmxSourceLayer;
            mDestinationLayer = destinationLayer;

            LoadLayerChunkIndex();

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

            if (mChunkIndex == null)
            {
                UpdateChunkIndex();
            }
            else
            {
                UpdateMapInfo();
            }
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

        private bool LoadLayerChunkIndex()
        {
            RBTilemapTMX.TMXMapDef map = null;

            map = layerState.map.internalState.mapDef;

            if (map == null || map.realPathName == null || map.realPathName.Length == 0 || map.layers == null)
            {
                Debug.LogError("Can't load TMX layer, invalid map, or map not loaded yet!");
                layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return false;
            }

            if (!map.infinite)
            {
                Debug.LogError("TMX map is not infinite, use LoadTMXLayer() instead");
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
            int layerNameHash = mWorkStr.Set(mTmxSourceLayer).ToLowerInvariant().GetHashCode();

            mIndexPath = map.realPathName + "layer_" + layerNameHash.ToString("x") + "_index";
            if (mSource == RB.AssetSource.WWW)
            {
                mIndexPath += ".bytes";
            }

            var cached = map.layerIndexLRU.Get(mIndexPath);
            if (cached != null)
            {
                // Already have the chunk index in cache, no need to do anything else
                mChunkIndex = cached;
                return true;
            }

            // Not in cache, will have to load async

            // Check if this is a web request
            if (mSource == RB.AssetSource.WWW)
            {
                mWebRequest = UnityWebRequest.Get(mIndexPath);
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
                mResourceRequest = Resources.LoadAsync<TextAsset>(mIndexPath);

                if (mResourceRequest == null)
                {
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);

                    return false;
                }

                layerState.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mSource == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequest = Addressables.LoadAssetAsync<TextAsset>(mIndexPath);
                }
#pragma warning disable 0414 // Unused warning
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
#pragma warning restore 0414

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
            // This should never happen
            layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);

            return false;
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

            if (!map.infinite)
            {
                Debug.LogError("TMX map is not infinite, use LoadTMXLayer() instead");
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
            int layerNameHash = mWorkStr.Set(mTmxSourceLayer).ToLowerInvariant().GetHashCode();

            int chunkWidth = map.chunkSize.x;
            int chunkHeight = map.chunkSize.y;

            ulong part1 = (ulong)mChunkOffset.x;
            ulong part2 = (ulong)mChunkOffset.y;
            ulong offset = ((part1 << 32) & 0xFFFFFFFF00000000) | (part2 & 0xFFFFFFFF);

            mTupleKey = new RBTilemapTMX.RetroBlitTuple<int, ulong>(layerNameHash, offset);

            var decompressed = map.chunkLRU.Get(mTupleKey);
            if (decompressed != null)
            {
                RetroBlitInternal.RBAPI.instance.Tilemap.FinalizeLayerChunkLoad(map, mTmxSourceLayer, mDestinationLayer, mChunkOffset, mDestPos, mPackedSpriteLookup, layerState, decompressed);
                return true;
            }

            // If the chunk can't be found then fail silently and wipe the chunk area. This will also
            // release the chunk geometry on next draw because it will not have any vertices
            if (!mChunkIndex.ContainsKey(offset))
            {
                for (int y = mDestPos.y; y < mDestPos.y + chunkHeight; y++)
                {
                    for (int x = mDestPos.x; x < mDestPos.x + chunkWidth; x++)
                    {
                        RetroBlitInternal.RBAPI.instance.Tilemap.SpriteSet(mDestinationLayer, x, y, RB.SPRITE_EMPTY, Color.white, 0);

                        RBTilemapTMX.Tile[] tilesArr;
                        int tileIndex;
                        if (RetroBlitInternal.RBAPI.instance.Tilemap.GetTileRef(mDestinationLayer, x, y, out tilesArr, out tileIndex, true))
                        {
                            tilesArr[tileIndex].data = null;
                        }
                    }
                }

                layerState.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                return true;
            }

            mChunkDef = mChunkIndex[offset];

            var mPath = map.realPathName + "layer_" + layerNameHash.ToString("x") + "_seg_" + mChunkDef.segmentIndex;

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

                layerState.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);
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
            return true;
        }

        private void UpdateChunkIndex()
        {
            if (mChunkIndex != null)
            {
                // Chunk index is already loaded, move on
                LoadLayerInfo();
                return;
            }

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
                    layerState.progress = 1;

                    var textAsset = mAddressableRequest.Result;
                    if (textAsset != null)
                    {
                        loadedBytes = textAsset.bytes;
                        // Not safe to release yet, release after loadedBytes has been processed
                    }
                    else
                    {
                        // Can't really figure out failure reason
                        Addressables.Release(mAddressableRequest);
                        layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }

                    Addressables.Release(mAddressableRequest);
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
                if (FinalizeLayerChunkIndex(loadedBytes))
                {
#if ADDRESSABLES_PACKAGE_AVAILABLE
                    if (mAddressableRequest.IsValid())
                    {
                        Addressables.Release(mAddressableRequest);
                    }
#endif
                    LoadLayerInfo();
                }
                else
                {
#if ADDRESSABLES_PACKAGE_AVAILABLE
                    if (mAddressableRequest.IsValid())
                    {
                        Addressables.Release(mAddressableRequest);
                    }
#endif
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                }
            }
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
                    layerState.progress = 1;

                    var textAsset = mAddressableRequest.Result;
                    if (textAsset != null)
                    {
                        loadedBytes = textAsset.bytes;
                        // Do not release yet, wait until loadedBytes is processed
                    }
                    else
                    {
                        Addressables.Release(mAddressableRequest);
                        // Can't really figure out failure reason
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
                RBTilemapTMX.TMXMapDef map = layerState.map.internalState.mapDef;

                // Decompress first to match decompressed bytes coming from LRU
                byte[] decompressed = RBDeflate.Decompress(loadedBytes, mChunkDef.segmentOffset, mChunkDef.compressedLength);
                if (decompressed == null || decompressed.Length <= 0)
                {
                    Debug.LogError("Could not decompress tile data for layer " + mTmxSourceLayer);
                    layerState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return;
                }

                // Add to LRU for future cache lookup
                map.chunkLRU.Add(mTupleKey, decompressed, decompressed.Length);

                if (FinalizeLayerChunkInfo(decompressed))
                {
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

        private bool FinalizeLayerChunkInfo(byte[] decompressedBytes)
        {
            var map = layerState.map.internalState.mapDef;

            RetroBlitInternal.RBAPI.instance.Tilemap.FinalizeLayerChunkLoad(map, mTmxSourceLayer, mDestinationLayer, mChunkOffset, mDestPos, mPackedSpriteLookup, layerState, decompressedBytes);
            return true;
        }

        private bool FinalizeLayerChunkIndex(byte[] loadedBytes)
        {
            RBTilemapTMX.TMXMapDef map = null;
            map = layerState.map.internalState.mapDef;

            try
            {
                var reader = new BinaryReader(new MemoryStream(loadedBytes));

                int byteSize = 0;

                int chunkCount = reader.ReadInt32();
                byteSize += 4;

                var table = new Dictionary<ulong, RBTilemapTMX.ChunkDef>();

                // Return empty table if there are no chunks
                if (chunkCount == 0)
                {
                    map.layerIndexLRU.Add(mIndexPath, table, byteSize);
                    mChunkIndex = table;
                    return true;
                }

                for (int i = 0; i < chunkCount; i++)
                {
                    var chunkDef = new RBTilemapTMX.ChunkDef();

                    ulong offset = reader.ReadUInt64();
                    byteSize += 8;
                    chunkDef.segmentIndex = reader.ReadUInt16();
                    byteSize += 2;
                    chunkDef.segmentOffset = reader.ReadUInt16();
                    byteSize += 2;
                    chunkDef.compressedLength = reader.ReadUInt16();
                    byteSize += 2;

                    table[offset] = chunkDef;
                }

                map.layerIndexLRU.Add(mIndexPath, table, byteSize);
                mChunkIndex = table;

                return true;
            }
            catch (IOException e)
            {
                Debug.Log("Failed to load layer index from file " + mIndexPath + ", " + e.ToString());
                return false;
            }
        }
    }
}
