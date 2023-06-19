namespace RetroBlitInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Tilemap subsystem
    /// </summary>
    public partial class RBTilemapTMX
    {
        /// <summary>
        /// Magic number to quickly verify that this is a valid RetroBlit file
        /// </summary>
        public const ushort RetroBlit_TMX_MAGIC = 0x0FE5;

        /// <summary>
        /// Version
        /// </summary>
        public const ushort RetroBlit_TMX_VERSION = 0x0001;

        /// <summary>
        /// Declares that this TMX file represents a map
        /// </summary>
        public const byte RetroBlit_TMX_TYPE_MAP = 0x01;

        /// <summary>
        /// Declares a tile layer section in file
        /// </summary>
        public const byte RetroBlit_TMX_SECTION_TILE_LAYER = 0x01;

        /// <summary>
        /// Declares an object group section in file
        /// </summary>
        public const byte RetroBlit_TMX_SECTION_OBJECTGROUP = 0x02;

        /// <summary>
        /// Declares TSX properties in file
        /// </summary>
        public const byte RetroBlit_TSX_PROPERTIES = 0x03;

        /// <summary>
        /// Declares the end of TMX file
        /// </summary>
        public const byte RetroBlit_TMX_SECTION_END = 0xFF;

        private FastString mWorkStr = new FastString(2048);

        private List<RBTMXMapLoader> mASyncTMXMaps = new List<RBTMXMapLoader>();
        private List<RBTMXLayerLoader> mASyncTMXLayers = new List<RBTMXLayerLoader>();
        private List<RBTMXLayerChunkLoader> mASyncTMXLayerChunks = new List<RBTMXLayerChunkLoader>();

        /// <summary>
        /// Load a map definition from a parsed binary TMX file
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="asset">TMXMapAsset to load into</param>
        public void LoadTMX(string fileName, TMXMapAsset asset)
        {
            if (asset == null)
            {
                return;
            }

            asset.InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

            if (fileName == null)
            {
                asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return;
            }

            // Discard old mapDef is any, and create new one
            asset.internalState.mapDef = new TMXMapDef();

            var map = asset.internalState.mapDef;
            asset.internalState.fileName = fileName;

            map.realPathName = Path.GetDirectoryName(fileName) + "/" + Path.GetFileNameWithoutExtension(fileName) + ".tmx.rb/";
            map.realPathName = map.realPathName.Replace('\\', '/');

            string infoFileName = map.realPathName + "info";
            var tmxFile = Resources.Load<TextAsset>(infoFileName);

            if (tmxFile == null)
            {
                Debug.LogError("Can't find TMX map at " + fileName + ". If TMX file exists then please try re-importing your TMX file.");
                asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                return;
            }

            FinalizeTMXInfo(map, fileName, asset, tmxFile.bytes);
        }

        /// <summary>
        /// Finalize TMX info from buffer blob
        /// </summary>
        /// <param name="map">TMX Map that is being processed</param>
        /// <param name="fileName">Filename of the map</param>
        /// <param name="asset">TMXMapAsset to use</param>
        /// <param name="byteBuf">Buffer</param>
        public void FinalizeTMXInfo(TMXMapDef map, string fileName, TMXMapAsset asset, byte[] byteBuf)
        {
            try
            {
                var reader = new BinaryReader(new MemoryStream(byteBuf));

                var magicNum = reader.ReadUInt16();
                var version = reader.ReadUInt16();

                if (magicNum != RetroBlit_TMX_MAGIC)
                {
                    Debug.Log(fileName + " is not a TMX RetroBlit binary file");
                    Debug.Log("Magic: " + magicNum + " expected " + RetroBlit_TMX_MAGIC);
                    asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return;
                }

                if (version > RetroBlit_TMX_VERSION)
                {
                    Debug.Log(fileName + " is of a newer version than this version of RetroBlit supports, try reimporting your TMX file into Unity.");
                    asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return;
                }

                byte type = reader.ReadByte();

                if (type != RetroBlit_TMX_TYPE_MAP)
                {
                    Debug.Log(fileName + " is a RetroBlit TMX file but it is of the wrong type.");
                    asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return;
                }

                int mapWidth = reader.ReadInt32();
                int mapHeight = reader.ReadInt32();
                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();
                bool infinite = reader.ReadBoolean();

                map.SetSize(new Vector2i(mapWidth, mapHeight));
                map.SetBackgroundColor(new Color32(r, g, b, a));
                map.SetInfinite(infinite);

                int chunkWidth = reader.ReadInt32();
                int chunkHeight = reader.ReadInt32();

                map.chunkSize = new Vector2i(chunkWidth, chunkHeight);

                // Load properties if available
                bool propsAvailable = reader.ReadBoolean();
                if (propsAvailable)
                {
                    var props = new TMXMapAsset.TMXProperties();
                    LoadProperties(reader, props);

                    map.SetProperties(props);
                }

                if (!LoadTMXSections(reader, ref map))
                {
                    Debug.Log("Failed to load TMX sections from " + fileName);
                    asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return;
                }
            }
            catch (IOException e)
            {
                Debug.Log("Failed to load TMX from file " + fileName + ", " + e.ToString());
                asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                return;
            }

            asset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
        }

        /// <summary>
        /// Load TMX map asynchronously
        /// </summary>
        /// <param name="fileName">Filename to load from</param>
        /// <param name="asset">TMXMapAsset to load into</param>
        /// <param name="source">Source type</param>
        public void LoadTMXAsync(string fileName, TMXMapAsset asset, RB.AssetSource source)
        {
            if (asset == null)
            {
                return;
            }

            asset.InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

            if (fileName == null)
            {
                asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return;
            }

            // Discard old mapDef is any, and create new one
            asset.internalState.mapDef = new TMXMapDef();

            // Abort any existing async load for this asset
            AbortAsyncTMXMapLoad(asset);

            var asyncTMXMapResource = new RBTMXMapLoader();
            asyncTMXMapResource.Load(fileName, asset, source);

            // Always add to async queue, even if immediately failed. This gives out consistent async method of error checking
            mASyncTMXMaps.Add(asyncTMXMapResource);
        }

        /// <summary>
        /// Load a layer definition from an map definition
        /// </summary>
        /// <param name="tmx">Map definition</param>
        /// <param name="tmxSourceLayer">Name of the layer to load</param>
        /// <param name="destinationLayer">Destination RetroBlit layer</param>
        /// <param name="sourceRect">Source rectangle</param>
        /// <param name="destPos">Destination position</param>
        /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
        /// <returns>True if successful</returns>
        public TMXMapAsset.TMXLayerLoadState LoadTMXLayer(TMXMapAsset tmx, string tmxSourceLayer, int destinationLayer, Rect2i sourceRect, Vector2i destPos, PackedSpriteID[] packedSpriteLookup)
        {
            TMXMapAsset.TMXLayerLoadState loadState = new TMXMapAsset.TMXLayerLoadState();
            loadState.map = tmx;

            if (tmx.internalState.source != RB.AssetSource.Resources)
            {
                var asyncLoader = new RBTMXLayerLoader(loadState, tmxSourceLayer, destinationLayer, sourceRect, destPos, packedSpriteLookup, tmx.internalState.source);
                mASyncTMXLayers.Add(asyncLoader);

                return loadState;
            }

            TMXMapDef map = null;

            map = tmx.internalState.mapDef;

            if (map == null || map.realPathName == null || map.realPathName.Length == 0 || map.layers == null)
            {
                Debug.LogError("Can't load TMX layer, invalid map, or map not open yet!");
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return loadState;
            }

            if (map.infinite)
            {
                Debug.LogError("TMX map is infinite, use MapLoadTMXLayerChunk() instead");
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return loadState;
            }

            if (!map.layers.ContainsKey(tmxSourceLayer))
            {
                Debug.LogError("Layer " + tmxSourceLayer + " not found");
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                return loadState;
            }

            var layerNameHash = mWorkStr.Set(tmxSourceLayer).ToLowerInvariant().GetHashCode();

            var tmxFileName = map.realPathName + "layers";
            var tmxFile = Resources.Load<TextAsset>(tmxFileName);

            if (tmxFile == null)
            {
                Debug.LogError("Can't find TMX file when loading TMX layer!");
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                return loadState;
            }

            byte[] byteBuf = GetLayerBytesFromLayerPack(layerNameHash, tmxFile.bytes);
            if (byteBuf == null)
            {
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                return loadState;
            }

            return FinalizeLayerLoad(map, tmxSourceLayer, destinationLayer, sourceRect, destPos, packedSpriteLookup, loadState, byteBuf);
        }

        /// <summary>
        /// Finalize layer load information from a buffer blob
        /// </summary>
        /// <param name="map">TMXMap definition</param>
        /// <param name="tmxSourceLayer">Source Tiled layer</param>
        /// <param name="destinationLayer">Destination RetroBlit layer</param>
        /// <param name="sourceRect">Source rectangular area</param>
        /// <param name="destPos">Destination position</param>
        /// <param name="packedSpriteLookup">Packed sprite lookup array</param>
        /// <param name="loadState">Loading state</param>
        /// <param name="byteBuf">Buffer</param>
        /// <returns>True if successful</returns>
        public TMXMapAsset.TMXLayerLoadState FinalizeLayerLoad(TMXMapDef map, string tmxSourceLayer, int destinationLayer, Rect2i sourceRect, Vector2i destPos, PackedSpriteID[] packedSpriteLookup, TMXMapAsset.TMXLayerLoadState loadState, byte[] byteBuf)
        {
            try
            {
                var tmxBytes = byteBuf;

                var tmxLayer = (TMXLayerDef)map.layers[tmxSourceLayer];

                var decompressed = RBDeflate.Decompress(tmxBytes, 0, tmxBytes.Length);
                if (decompressed == null || decompressed.Length <= 0)
                {
                    Debug.LogError("Could not decompress tile data for layer " + tmxSourceLayer);
                    loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return loadState;
                }

                var tileDataReader = new BinaryReader(new MemoryStream(decompressed));

                if (tileDataReader == null)
                {
                    Debug.LogError("Could not read tile data for layer " + tmxSourceLayer);
                    loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return loadState;
                }

                Color32 color = Color.white;

                int sx = 0;
                int sy = 0;

                int sx0 = sourceRect.x;
                int sx1 = sourceRect.x + sourceRect.width;

                int sy0 = sourceRect.y;
                int sy1 = sourceRect.y + sourceRect.height;

                int dx = destPos.x;
                int dy = destPos.y;

                while (tileDataReader.PeekChar() >= 0)
                {
                    byte tsxIndex = tileDataReader.ReadByte();
                    byte flags = tileDataReader.ReadByte();
                    int tileId = tileDataReader.ReadInt32();
                    int origTileId = tileId;

                    if (packedSpriteLookup != null)
                    {
                        if (tileId < packedSpriteLookup.Length && tileId >= 0)
                        {
                            tileId = packedSpriteLookup[tileId].id;
                            flags |= RetroBlitInternal.RBTilemapTMX.SPRITEPACK;
                        }
                    }

                    if (sx >= sx0 && sx <= sx1 && sy >= sy0 && sy <= sy1)
                    {
                        SpriteSet(destinationLayer, dx, dy, tileId, color, flags);

                        // Set properties if available
                        if (tsxIndex >= 0 && tsxIndex < map.allTileProperties.Count)
                        {
                            var props = map.allTileProperties[tsxIndex];
                            if (props != null)
                            {
                                if (props.ContainsKey(origTileId))
                                {
                                    DataSet<TMXMapAsset.TMXProperties>(destinationLayer, dx, dy, props[origTileId]);
                                }
                            }
                        }

                        dx++;
                    }

                    sx++;
                    if (sx >= tmxLayer.size.x)
                    {
                        sx = 0;
                        dx = destPos.x;
                        sy++;
                        dy++;
                    }

                    if (sy >= tmxLayer.size.y || sy >= sourceRect.y + sourceRect.height)
                    {
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Failed to load TMX info, " + e.ToString());
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                return loadState;
            }

            loadState.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

            return loadState;
        }

        /// <summary>
        /// Load a single layer chunk
        /// </summary>
        /// <param name="tmx">Map definition</param>
        /// <param name="tmxSourceLayer">Name of source layer</param>
        /// <param name="destinationLayer">RetroBlit destination layer</param>
        /// <param name="chunkOffset">Chunk offset</param>
        /// <param name="destPos">Destination position</param>
        /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
        /// <returns>Load state</returns>
        public TMXMapAsset.TMXLayerLoadState LoadTMXLayerChunk(TMXMapAsset tmx, string tmxSourceLayer, int destinationLayer, Vector2i chunkOffset, Vector2i destPos, PackedSpriteID[] packedSpriteLookup)
        {
            TMXMapAsset.TMXLayerLoadState loadState = new TMXMapAsset.TMXLayerLoadState();
            loadState.map = tmx;

            if (tmx.internalState.source != RB.AssetSource.Resources)
            {
                var asyncLoader = new RBTMXLayerChunkLoader(loadState, tmxSourceLayer, destinationLayer, chunkOffset, destPos, packedSpriteLookup, tmx.internalState.source);
                mASyncTMXLayerChunks.Add(asyncLoader);

                return loadState;
            }

            TMXMapDef map = null;

            map = tmx.internalState.mapDef;

            if (map == null || map.realPathName == null || map.realPathName.Length == 0 || map.layers == null)
            {
                Debug.LogError("Can't load TMX layer, invalid map, or map not open yet!");
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return loadState;
            }

            if (!map.infinite)
            {
                Debug.LogError("TMX map is not infinite, use LoadTMXLayer() instead");
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return loadState;
            }

            if (!map.layers.ContainsKey(tmxSourceLayer))
            {
                Debug.LogError("Layer " + tmxSourceLayer + " not found");
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                return loadState;
            }

            int chunkWidth = map.chunkSize.x;
            int chunkHeight = map.chunkSize.y;

            var tmxLayer = (TMXLayerDef)map.layers[tmxSourceLayer];

            ulong part1 = (ulong)chunkOffset.x;
            ulong part2 = (ulong)chunkOffset.y;
            ulong offset = ((part1 << 32) & 0xFFFFFFFF00000000) | (part2 & 0xFFFFFFFF);

            int layerNameHash = mWorkStr.Set(tmxSourceLayer).ToLowerInvariant().GetHashCode();
            var tupleKey = new RetroBlitTuple<int, ulong>(layerNameHash, offset);

            var decompressed = map.chunkLRU.Get(tupleKey);
            if (decompressed == null)
            {
                var chunkTable = GetLayerIndexTable(map, layerNameHash);

                if (chunkTable == null)
                {
                    Debug.LogError("TMX could not load chunk index table for layer " + tmxSourceLayer);
                    loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return loadState;
                }

                // If the chunk can't be found then fail silently and wipe the chunk area. This will also
                // release the chunk geometry on next draw because it will not have any vertices
                if (!chunkTable.ContainsKey(offset))
                {
                    for (int y = destPos.y; y < destPos.y + chunkHeight; y++)
                    {
                        for (int x = destPos.x; x < destPos.x + chunkWidth; x++)
                        {
                            mRetroBlitAPI.Tilemap.SpriteSet(destinationLayer, x, y, RB.SPRITE_EMPTY, Color.white, 0);

                            Tile[] tilesArr;
                            int tileIndex;
                            if (GetTileRef(destinationLayer, x, y, out tilesArr, out tileIndex, true))
                            {
                                tilesArr[tileIndex].data = null;
                            }
                        }
                    }

                    loadState.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                    return loadState;
                }

                var chunkDef = chunkTable[offset];

                var chunkFileName = map.realPathName + "layer_" + layerNameHash.ToString("x") + "_seg_" + chunkDef.segmentIndex;

                var chunkFile = Resources.Load<TextAsset>(chunkFileName);

                if (chunkFile == null)
                {
                    Debug.LogError("Can't find TMX file when loading TMX layer!");
                    loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return loadState;
                }

                var chunkBytes = chunkFile.bytes;

                decompressed = RBDeflate.Decompress(chunkBytes, chunkDef.segmentOffset, chunkDef.compressedLength);
                if (decompressed == null || decompressed.Length <= 0)
                {
                    Debug.LogError("Could not decompress tile data for layer " + tmxSourceLayer);
                    loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                    return loadState;
                }

                map.chunkLRU.Add(tupleKey, decompressed, decompressed.Length);
            }

            return FinalizeLayerChunkLoad(map, tmxSourceLayer, destinationLayer, chunkOffset, destPos, packedSpriteLookup, loadState, decompressed);
        }

        /// <summary>
        /// Finalize layer chunk load
        /// </summary>
        /// <param name="map">TMX Map to load from</param>
        /// <param name="tmxSourceLayer">Tiled source layer</param>
        /// <param name="destinationLayer">RetroBlit destination layer</param>
        /// <param name="chunkOffset">Chunk offset in the Tiled map</param>
        /// <param name="destPos">Destination position</param>
        /// <param name="packedSpriteLookup">Packed sprite lookup array</param>
        /// <param name="loadState">Loading state</param>
        /// <param name="decompressedByteBuf">Buffer</param>
        /// <returns>Load state</returns>
        public TMXMapAsset.TMXLayerLoadState FinalizeLayerChunkLoad(TMXMapDef map, string tmxSourceLayer, int destinationLayer, Vector2i chunkOffset, Vector2i destPos, PackedSpriteID[] packedSpriteLookup, TMXMapAsset.TMXLayerLoadState loadState, byte[] decompressedByteBuf)
        {
            int chunkWidth = map.chunkSize.x;
            int chunkHeight = map.chunkSize.y;

            var tileDataReader = new BinaryReader(new MemoryStream(decompressedByteBuf));

            if (tileDataReader == null)
            {
                Debug.LogError("Could not read tile data for layer " + tmxSourceLayer);
                loadState.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                return loadState;
            }

            Color32 color = Color.white;

            int sx = 0;
            int sy = 0;

            int dx = destPos.x;
            int dy = destPos.y;

            while (tileDataReader.PeekChar() >= 0)
            {
                // Skip tsxIndex, don't need it for now
                tileDataReader.ReadByte();

                byte flags = tileDataReader.ReadByte();
                int tileId = tileDataReader.ReadInt32();

                if (packedSpriteLookup != null)
                {
                    if (packedSpriteLookup != null)
                    {
                        if (tileId < packedSpriteLookup.Length && tileId >= 0)
                        {
                            tileId = packedSpriteLookup[tileId].id;
                            flags |= RetroBlitInternal.RBTilemapTMX.SPRITEPACK;
                        }
                    }
                }

                SpriteSet(destinationLayer, dx, dy, tileId, color, flags);
                dx++;
                sx++;

                if (sx >= chunkWidth)
                {
                    sx = 0;
                    dx = destPos.x;
                    sy++;
                    dy++;
                }

                if (sy >= chunkHeight)
                {
                    break;
                }
            }

            loadState.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
            return loadState;
        }

        /// <summary>
        /// Get layer byte buffer from layer name hash
        /// </summary>
        /// <param name="layerHash">Layer name hash</param>
        /// <param name="packBytes">Bytes of the entire pack</param>
        /// <returns>Layer bytes</returns>
        public static byte[] GetLayerBytesFromLayerPack(int layerHash, byte[] packBytes)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(packBytes));

            int layerCount = br.ReadInt32();
            for (int i = 0; i < layerCount; i++)
            {
                int hash = br.ReadInt32();
                int offset = br.ReadInt32();
                int size = br.ReadInt32();

                if (hash == layerHash)
                {
                    // Found layer
                    br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    var layerBytes = br.ReadBytes(size);
                    br.Close();
                    return layerBytes;
                }
            }

            Debug.LogError("Could not find layer to load!");
            return null;
        }

        /// <summary>
        /// Update asynchronous resources
        /// </summary>
        public void UpdateAsyncResources()
        {
            for (int i = mASyncTMXMaps.Count - 1; i >= 0; i--)
            {
                var asyncTMXMap = mASyncTMXMaps[i];

                asyncTMXMap.Update();

                if (asyncTMXMap.mapAsset == null)
                {
                    mASyncTMXMaps.RemoveAt(i);
                    continue;
                }

                if (asyncTMXMap.mapAsset.status == RB.AssetStatus.Failed)
                {
                    Debug.LogError("TMX map " + asyncTMXMap.path + " async load failed! Result = " + asyncTMXMap.mapAsset.error.ToString());
                }

                // If state is anything except loading then remove it
                if (asyncTMXMap.mapAsset.status != RB.AssetStatus.Loading)
                {
                    mASyncTMXMaps.RemoveAt(i);
                }
            }

            for (int i = mASyncTMXLayers.Count - 1; i >= 0; i--)
            {
                var asyncTMXLayer = mASyncTMXLayers[i];

                asyncTMXLayer.Update();

                if (asyncTMXLayer.layerState == null)
                {
                    mASyncTMXLayers.RemoveAt(i);
                    continue;
                }

                if (asyncTMXLayer.layerState.status == RB.AssetStatus.Failed)
                {
                    Debug.LogError("TMX map layer async load failed! Result = " + asyncTMXLayer.layerState.error.ToString());
                }

                // If state is anything except loading then remove it
                if (asyncTMXLayer.layerState.status != RB.AssetStatus.Loading)
                {
                    mASyncTMXLayers.RemoveAt(i);
                }
            }

            for (int i = mASyncTMXLayerChunks.Count - 1; i >= 0; i--)
            {
                var asyncTMXLayerChunk = mASyncTMXLayerChunks[i];

                asyncTMXLayerChunk.Update();

                if (asyncTMXLayerChunk.layerState == null)
                {
                    mASyncTMXLayerChunks.RemoveAt(i);
                    continue;
                }

                if (asyncTMXLayerChunk.layerState.status == RB.AssetStatus.Failed)
                {
                    Debug.LogError("TMX map layer chunk async load failed! Result = " + asyncTMXLayerChunk.layerState.error.ToString());
                }

                // If state is anything except loading then remove it
                if (asyncTMXLayerChunk.layerState.status != RB.AssetStatus.Loading)
                {
                    mASyncTMXLayerChunks.RemoveAt(i);
                }
            }
        }

        private void LoadProperties(BinaryReader reader, TMXMapAsset.TMXProperties props)
        {
            var strCount = reader.ReadInt32();
            for (int i = 0; i < strCount; i++)
            {
                var key = reader.ReadString();
                var val = reader.ReadString();
                props.Add(key, val);
            }

            var boolCount = reader.ReadInt32();
            for (int i = 0; i < boolCount; i++)
            {
                var key = reader.ReadString();
                bool val = reader.ReadBoolean();
                props.Add(key, val);
            }

            var intCount = reader.ReadInt32();
            for (int i = 0; i < intCount; i++)
            {
                var key = reader.ReadString();
                int val = reader.ReadInt32();
                props.Add(key, val);
            }

            var floatCount = reader.ReadInt32();
            for (int i = 0; i < floatCount; i++)
            {
                var key = reader.ReadString();
                float val = reader.ReadSingle();
                props.Add(key, val);
            }

            var colorCount = reader.ReadInt32();
            for (int i = 0; i < colorCount; i++)
            {
                var key = reader.ReadString();
                Color32 val = new Color32();

                val.r = reader.ReadByte();
                val.g = reader.ReadByte();
                val.b = reader.ReadByte();
                val.a = reader.ReadByte();

                props.Add(key, val);
            }
        }

        private Dictionary<ulong, ChunkDef> GetLayerIndexTable(TMXMapDef tmx, int layerName)
        {
            if (tmx == null)
            {
                return null;
            }

            var fileName = tmx.realPathName + "layer_" + layerName.ToString("x") + "_index";

            var cached = tmx.layerIndexLRU.Get(fileName);
            if (cached != null)
            {
                return cached;
            }

            var indexTableFile = Resources.Load<TextAsset>(fileName);

            if (indexTableFile == null)
            {
                Debug.Log("TMX could not find layer index table");
                return null;
            }

            try
            {
                var reader = new BinaryReader(new MemoryStream(indexTableFile.bytes));

                int byteSize = 0;

                int chunkCount = reader.ReadInt32();
                byteSize += 4;

                var table = new Dictionary<ulong, ChunkDef>();

                // Return empty table if there are no chunks
                if (chunkCount == 0)
                {
                    tmx.layerIndexLRU.Add(fileName, table, byteSize);
                    return table;
                }

                for (int i = 0; i < chunkCount; i++)
                {
                    var chunkDef = new ChunkDef();

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

                tmx.layerIndexLRU.Add(fileName, table, byteSize);

                return table;
            }
            catch (IOException e)
            {
                Debug.Log("Failed to load layer index from file " + fileName + ", " + e.ToString());
                return null;
            }
        }

        private bool LoadTMXSections(BinaryReader reader, ref TMXMapDef map)
        {
            while (true)
            {
                int section = reader.ReadByte();
                if (section == RetroBlit_TMX_SECTION_TILE_LAYER)
                {
                    if (!LoadTMXTileLayerDef(reader, ref map))
                    {
                        return false;
                    }
                }
                else if (section == RetroBlit_TMX_SECTION_OBJECTGROUP)
                {
                    if (!LoadTMXObjectGroupDef(reader, ref map))
                    {
                        return false;
                    }
                }
                else if (section == RetroBlit_TSX_PROPERTIES)
                {
                    if (!LoadTSXProperties(reader, ref map))
                    {
                        return false;
                    }
                }
                else if (section == RetroBlit_TMX_SECTION_END)
                {
                    break;
                }
            }

            return true;
        }

        private bool LoadTMXTileLayerDef(BinaryReader reader, ref TMXMapDef map)
        {
            TMXLayerDef layerDef = new TMXLayerDef();
            string name = reader.ReadString();
            int layerWidth = reader.ReadInt32();
            int layerHeight = reader.ReadInt32();
            int layerOffsetX = reader.ReadInt32();
            int layerOffsetY = reader.ReadInt32();
            bool layerVisible = reader.ReadBoolean();
            byte layerAlpha = reader.ReadByte();
            int chunkCount = 0;

            // Load properties if available
            bool propsAvailable = reader.ReadBoolean();
            if (propsAvailable)
            {
                var props = new TMXMapAsset.TMXProperties();
                LoadProperties(reader, props);

                layerDef.SetProperties(props);
            }

            if (map.infinite)
            {
                chunkCount = reader.ReadInt32();
            }

            layerDef.chunkCount = chunkCount;
            layerDef.SetSize(new Vector2i(layerWidth, layerHeight));
            layerDef.SetOffset(new Vector2i(layerOffsetX, layerOffsetY));
            layerDef.SetVisible(layerVisible);
            layerDef.SetAlpha(layerAlpha);

            map.layers[name] = layerDef;

            return true;
        }

        private bool LoadTMXObjectGroupDef(BinaryReader reader, ref TMXMapDef map)
        {
            var objectGroup = new TMXObjectGroupDef();

            var name = reader.ReadString();
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();
            var a = reader.ReadByte();
            var alpha = reader.ReadByte();
            var visible = reader.ReadBoolean();
            var offsetX = reader.ReadInt32();
            var offsetY = reader.ReadInt32();

            objectGroup.SetName(name);
            objectGroup.SetColor(new Color32(r, g, b, a));
            objectGroup.SetAlpha(alpha);
            objectGroup.SetVisible(visible);
            objectGroup.SetOffset(new Vector2i(offsetX, offsetY));

            // Load properties if available
            bool propsAvailable = reader.ReadBoolean();
            if (propsAvailable)
            {
                var props = new TMXMapAsset.TMXProperties();
                LoadProperties(reader, props);

                objectGroup.SetProperties(props);
            }

            // Now load objects
            var objects = new List<TMXMapAsset.TMXObject>();
            var objectCount = reader.ReadInt32();

            for (int i = 0; i < objectCount; i++)
            {
                var objName = reader.ReadString();
                var objType = reader.ReadString();
                var rectX = reader.ReadInt32();
                var rectY = reader.ReadInt32();
                var rectWidth = reader.ReadInt32();
                var rectHeight = reader.ReadInt32();
                var rotation = reader.ReadSingle();
                var objVisible = reader.ReadBoolean();
                var tileId = reader.ReadInt32();
                var shape = reader.ReadInt32();

                var points = new List<Vector2i>();
                var pointsCount = reader.ReadInt32();
                for (int j = 0; j < pointsCount; j++)
                {
                    var pointX = reader.ReadInt32();
                    var pointY = reader.ReadInt32();
                    points.Add(new Vector2i(pointX, pointY));
                }

                var tmxObject = new TMXObjectDef();
                tmxObject.SetName(objName);
                tmxObject.SetType(objType);
                tmxObject.SetShape((TMXMapAsset.TMXObject.Shape)shape);
                tmxObject.SetRect(new Rect2i(rectX, rectY, rectWidth, rectHeight));
                tmxObject.SetRotation(rotation);
                tmxObject.SetVisible(objVisible);
                tmxObject.SetPoints(points);
                tmxObject.SetTileId(tileId);

                // Load properties if available
                propsAvailable = reader.ReadBoolean();
                if (propsAvailable)
                {
                    var props = new TMXMapAsset.TMXProperties();
                    LoadProperties(reader, props);

                    tmxObject.SetProperties(props);
                }

                objects.Add(tmxObject);
            }

            objectGroup.SetObjects(objects);

            map.objectGroups[name] = objectGroup;

            return true;
        }

        private bool LoadTSXProperties(BinaryReader reader, ref TMXMapDef map)
        {
            int tsxLoops = 0;

            var allProps = new List<Dictionary<int, TMXMapAsset.TMXProperties>>();

            int prevTsxIndex = -1;

            while (true)
            {
                var tsxIndex = reader.ReadInt32();
                if (tsxIndex == -1)
                {
                    // All done
                    break;
                }

                if (tsxIndex != prevTsxIndex + 1)
                {
                    Debug.LogError("TMX binary files had non-consequitive TSX property sets, TMX import is likely corrupt, please try reimporting " + map.realPathName);
                    return false;
                }

                var propsSet = new Dictionary<int, TMXMapAsset.TMXProperties>();

                while (true)
                {
                    var tid = reader.ReadInt32();
                    if (tid == -1)
                    {
                        // All done this tsx
                        break;
                    }

                    var props = new TMXMapAsset.TMXProperties();

                    LoadProperties(reader, props);

                    propsSet.Add(tid, props);
                }

                allProps.Add(propsSet);

                prevTsxIndex = tsxIndex;

                // Break out if we loop for too long, TSX data could be corrupt
                tsxLoops++;
                if (tsxLoops > 256)
                {
                    Debug.Log("TSX properties data is invalid, please try to reimport " + map.realPathName);
                    return false;
                }
            }

            map.allTileProperties = allProps;

            return true;
        }

        private void AbortAsyncTMXMapLoad(TMXMapAsset asset)
        {
            /* Check if any of the existing pending async assets are loading for map asset, if so we abandon them */
            for (int i = mASyncTMXMaps.Count - 1; i >= 0; i--)
            {
                if (mASyncTMXMaps[i].mapAsset == asset)
                {
                    mASyncTMXMaps.RemoveAt(i);

                    // There should never be more than one
                    break;
                }
            }
        }

        private void AbortAsyncTMXLayerLoad(TMXMapAsset.TMXLayerLoadState asset)
        {
            /* Check if any of the existing pending async assets are loading for the same tmx layer asset, if so we abandon them */
            for (int i = mASyncTMXLayers.Count - 1; i >= 0; i--)
            {
                if (mASyncTMXLayers[i].layerState == asset)
                {
                    mASyncTMXLayers.RemoveAt(i);

                    // There should never be more than one
                    break;
                }
            }
        }

        private void AbortAsyncTMXLayerChunkLoad(TMXMapAsset.TMXLayerLoadState asset)
        {
            /* Check if any of the existing pending async assets are loading for the same tmx layer asset, if so we abandon them */
            for (int i = mASyncTMXLayerChunks.Count - 1; i >= 0; i--)
            {
                if (mASyncTMXLayers[i].layerState == asset)
                {
                    mASyncTMXLayerChunks.RemoveAt(i);

                    // There should never be more than one
                    break;
                }
            }
        }

        /// <summary>
        /// Map chunk definition
        /// </summary>
        public struct ChunkDef
        {
            /// <summary>
            /// Segment index of chunk
            /// </summary>
            public ushort segmentIndex;

            /// <summary>
            /// Segment offset of chunk
            /// </summary>
            public ushort segmentOffset;

            /// <summary>
            /// Compressed chunk length
            /// </summary>
            public ushort compressedLength;
        }

        /// <summary>
        /// Simple tuple class implementation.
        /// </summary>
        /// <typeparam name="FT">First type</typeparam>
        /// <typeparam name="ST">Second type</typeparam>
        public class RetroBlitTuple<FT, ST> : IEquatable<RetroBlitTuple<FT, ST>>
        {
            /// <summary>
            /// First type
            /// </summary>
            public FT First;

            /// <summary>
            /// Second type
            /// </summary>
            public ST Second;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="first">First value</param>
            /// <param name="second">Second value</param>
            public RetroBlitTuple(FT first, ST second)
            {
                First = first;
                Second = second;
            }

            /// <summary>
            /// Combined hash code of the values
            /// </summary>
            /// <returns>Hashcode</returns>
            public override int GetHashCode()
            {
                int hash = 17;
                hash = (hash * 31) + First.GetHashCode();
                hash = (hash * 31) + Second.GetHashCode();
                return hash;
            }

            /// <summary>
            /// Equality check
            /// </summary>
            /// <param name="tuple">Other tuple</param>
            /// <returns>True if equal</returns>
            public bool Equals(RetroBlitTuple<FT, ST> tuple)
            {
                return tuple.First.Equals(First) && tuple.Second.Equals(Second);
            }

            /// <summary>
            /// Equality check
            /// </summary>
            /// <param name="o">Other object</param>
            /// <returns>True if equal</returns>
            public override bool Equals(object o)
            {
                return this.Equals(o as RetroBlitTuple<FT, ST>);
            }
        }

        /// <summary>
        /// TMX layer definition
        /// </summary>
        public class TMXLayerDef : TMXMapAsset.TMXLayer
        {
            /// <summary>
            /// Count of chunks in this layer, if the map is infinite
            /// </summary>
            public int chunkCount;

            /// <summary>
            /// Set the size of layer
            /// </summary>
            /// <param name="size">Size</param>
            public void SetSize(Vector2i size)
            {
                mSize = size;
            }

            /// <summary>
            /// Set the offset of layer
            /// </summary>
            /// <param name="offset">Offset</param>
            public void SetOffset(Vector2i offset)
            {
                mOffset = offset;
            }

            /// <summary>
            /// Set the visible flag of layer
            /// </summary>
            /// <param name="visible">Visible flag</param>
            public void SetVisible(bool visible)
            {
                mVisible = visible;
            }

            /// <summary>
            /// Set the alpha transparency of layer
            /// </summary>
            /// <param name="alpha">Alpha</param>
            public void SetAlpha(byte alpha)
            {
                mAlpha = alpha;
            }

            /// <summary>
            /// Set the custom properties of the layer
            /// </summary>
            /// <param name="props">Properties</param>
            public void SetProperties(TMXMapAsset.TMXProperties props)
            {
                mProperties = props;
            }
        }

        /// <summary>
        /// TMX Object group class
        /// </summary>
        public class TMXObjectGroupDef : TMXMapAsset.TMXObjectGroup
        {
            /// <summary>
            /// Set the name
            /// </summary>
            /// <param name="name">Name</param>
            public void SetName(string name)
            {
                mName = name;
            }

            /// <summary>
            /// Set color
            /// </summary>
            /// <param name="color">Color</param>
            public void SetColor(Color32 color)
            {
                mColor = color;
            }

            /// <summary>
            /// Set alpha transparency
            /// </summary>
            /// <param name="alpha">Alpha transparency</param>
            public void SetAlpha(byte alpha)
            {
                mAlpha = alpha;
            }

            /// <summary>
            /// Set visible flag
            /// </summary>
            /// <param name="visible">Visible flag</param>
            public void SetVisible(bool visible)
            {
                mVisible = visible;
            }

            /// <summary>
            /// Set offset
            /// </summary>
            /// <param name="offset">Offset</param>
            public void SetOffset(Vector2i offset)
            {
                mOffset = offset;
            }

            /// <summary>
            /// Set objects list
            /// </summary>
            /// <param name="objects">Objects list</param>
            public void SetObjects(List<TMXMapAsset.TMXObject> objects)
            {
                mObjects = objects;
            }

            /// <summary>
            /// Set custom properties
            /// </summary>
            /// <param name="props">Custom properties</param>
            public void SetProperties(TMXMapAsset.TMXProperties props)
            {
                mProperties = props;
            }
        }

        /// <summary>
        /// TMX map definition
        /// </summary>
        public class TMXMapDef
        {
            /// <summary>
            /// Size of the tilemap in tiles
            /// </summary>
            public Vector2i size;

            /// <summary>
            /// Infinite flag
            /// </summary>
            public bool infinite;

            /// <summary>
            /// Background color of the tilemap
            /// </summary>
            public Color32 backgroundColor;

            /// <summary>
            /// Custom properties of the tilemap
            /// </summary>
            public TMXMapAsset.TMXProperties properties = new TMXMapAsset.TMXProperties();

            /// <summary>
            /// Layers in the map
            /// </summary>
            public Dictionary<string, TMXMapAsset.TMXLayer> layers = new Dictionary<string, TMXMapAsset.TMXLayer>();

            /// <summary>
            /// Object groups in the map
            /// </summary>
            public Dictionary<string, TMXMapAsset.TMXObjectGroup> objectGroups = new Dictionary<string, TMXMapAsset.TMXObjectGroup>();

            /// <summary>
            /// Actual file path of the map
            /// </summary>
            public string realPathName;

            /// <summary>
            /// Size of chunks in this map
            /// </summary>
            public Vector2i chunkSize;

            /// <summary>
            /// All tile properties for the map. Properties are stored in a list per tileset.
            /// </summary>
            public List<Dictionary<int, TMXMapAsset.TMXProperties>> allTileProperties = new List<Dictionary<int, TMXMapAsset.TMXProperties>>();

            /// <summary>
            /// LRU cache of layer index tables, null for non-infinite maps.
            /// </summary>
            public RBLRUCache<string, Dictionary<ulong, ChunkDef>> layerIndexLRU = null;

            /// <summary>
            /// LRU cache of chunks, null for non-infinite maps.
            /// </summary>
            public RBLRUCache<RetroBlitTuple<int, ulong>, byte[]> chunkLRU = null;

            /// <summary>
            /// Set size
            /// </summary>
            /// <param name="size">Size</param>
            public void SetSize(Vector2i size)
            {
                this.size = size;
            }

            /// <summary>
            /// Set infinite flag, if true then also allocate the LRUs
            /// </summary>
            /// <param name="infinite">Infinite flag</param>
            public void SetInfinite(bool infinite)
            {
                this.infinite = infinite;
                if (this.infinite && (layerIndexLRU == null || chunkLRU == null))
                {
                    layerIndexLRU = new RBLRUCache<string, Dictionary<ulong, ChunkDef>>(1024 * 512); // 512k cache
                    chunkLRU = new RBLRUCache<RetroBlitTuple<int, ulong>, byte[]>(1024 * 1024 * 2); // 2M cache
                }
            }

            /// <summary>
            /// Set background color
            /// </summary>
            /// <param name="color">Color</param>
            public void SetBackgroundColor(Color32 color)
            {
                backgroundColor = color;
            }

            /// <summary>
            /// Set custom properties
            /// </summary>
            /// <param name="props">Properties</param>
            public void SetProperties(TMXMapAsset.TMXProperties props)
            {
                properties = props;
            }
        }

        private class TMXObjectDef : TMXMapAsset.TMXObject
        {
            /// <summary>
            /// Set name of object
            /// </summary>
            /// <param name="name">Name</param>
            public void SetName(string name)
            {
                mName = name;
            }

            /// <summary>
            /// Set type of object
            /// </summary>
            /// <param name="type">Type</param>
            public void SetType(string type)
            {
                mType = type;
            }

            /// <summary>
            /// Set shape of object
            /// </summary>
            /// <param name="shape">Shape</param>
            public void SetShape(Shape shape)
            {
                mShape = shape;
            }

            /// <summary>
            /// Set rect of object
            /// </summary>
            /// <param name="rect">Rect</param>
            public void SetRect(Rect2i rect)
            {
                mRect = rect;
            }

            /// <summary>
            /// Set rotation of object
            /// </summary>
            /// <param name="rotation">Rotation</param>
            public void SetRotation(float rotation)
            {
                mRotation = rotation;
            }

            /// <summary>
            /// Set visible flag of object
            /// </summary>
            /// <param name="visible">Visible flag</param>
            public void SetVisible(bool visible)
            {
                mVisible = visible;
            }

            public void SetTileId(int tileId)
            {
                mTileId = tileId;
            }

            /// <summary>
            /// Set points of object
            /// </summary>
            /// <param name="points">Points list</param>
            public void SetPoints(List<Vector2i> points)
            {
                mPoints = points;
            }

            /// <summary>
            /// Set custom properties of the object
            /// </summary>
            /// <param name="props">Properties</param>
            public void SetProperties(TMXMapAsset.TMXProperties props)
            {
                mProperties = props;
            }
        }
    }
}
