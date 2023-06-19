namespace RetroBlitInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Tilemap subsystem
    /// </summary>
    public partial class RBTilemapTMX
    {
        /// <summary>
        /// Flag indicates that the sprite index passed it actually a sprite id from a sprite pack
        /// </summary>
        public const int SPRITEPACK = 0x80;

        /// <summary>
        /// Maximum cached meshes before we start releasing them
        /// </summary>
        private const int MAX_CACHED_MESHES = 128;

        private static Tile[] mClearingTiles; // Used to quickly clear blocks of tiles

        private readonly SpriteSheetAsset[] mLayerSpriteSheets = new SpriteSheetAsset[RBHardware.HW_MAX_MAP_LAYERS];

        private readonly List<Chunk> mActiveChunks = new List<Chunk>(1024);
        private readonly List<Mesh> mReleasedMeshes = new List<Mesh>(1024);

        private readonly List<ArraySet> mArraySets = new List<ArraySet>(1024);

        private readonly FastString mDebugString = new FastString(128);
        private readonly RBFont.TextWrapFastString mDebugStringWrap = new RBFont.TextWrapFastString();

        private int CHUNK_WIDTH = 0;
        private int CHUNK_HEIGHT = 0;
        private int MAX_QUADS_PER_CHUNK_LAYER = 0;
        private int MAX_VERTEX_PER_MESH = 0;

        private RBAPI mRetroBlitAPI;

        private ulong mFrameCounter = 0;
        private TileLayer[] mTileLayers;

        private Chunk[] mChunks;

        private int mChunksStride;
        private int mChunksPerLayer;

        private Vector2i mActualMapSize;

        /// <summary>
        /// Initialize the subsystem
        /// </summary>
        /// <param name="api">Subsystem wrapper reference</param>
        /// <returns>True if successful</returns>
        public bool Initialize(RBAPI api)
        {
            mRetroBlitAPI = api;

            CHUNK_WIDTH = mRetroBlitAPI.HW.MapChunkSize.x;
            CHUNK_HEIGHT = mRetroBlitAPI.HW.MapChunkSize.y;

            MAX_QUADS_PER_CHUNK_LAYER = CHUNK_WIDTH * CHUNK_HEIGHT;
            MAX_VERTEX_PER_MESH = 4 * MAX_QUADS_PER_CHUNK_LAYER;

            mClearingTiles = new Tile[8192];
            for (int i = 0; i < mClearingTiles.Length; i++)
            {
                mClearingTiles[i].Clear();
            }

            mActualMapSize = mRetroBlitAPI.HW.MapSize;

            // Round up map size to next multiple of chunk size
            if (mActualMapSize.width % CHUNK_WIDTH > 0)
            {
                mActualMapSize.width -= mActualMapSize.width % CHUNK_WIDTH;
                mActualMapSize.width += CHUNK_WIDTH;
            }

            if (mActualMapSize.height % CHUNK_HEIGHT > 0)
            {
                mActualMapSize.height -= mActualMapSize.height % CHUNK_HEIGHT;
                mActualMapSize.height += CHUNK_HEIGHT;
            }

            mTileLayers = new TileLayer[mRetroBlitAPI.HW.MapLayers];

            mChunksStride = (mActualMapSize.width / CHUNK_WIDTH) + 1;
            mChunksPerLayer = mChunksStride * ((mActualMapSize.height / CHUNK_HEIGHT) + 1);
            mChunks = new Chunk[mChunksPerLayer * mRetroBlitAPI.HW.MapLayers];

            if (mTileLayers == null || mChunks == null)
            {
                return false;
            }

            for (int i = 0; i < mChunks.Length; i++)
            {
                mChunks[i] = null;
            }

            int size = 4;
            while (true)
            {
                mArraySets.Add(new ArraySet(size));

                if (size >= MAX_VERTEX_PER_MESH)
                {
                    break;
                }

                size *= 2;
                if (size > MAX_VERTEX_PER_MESH)
                {
                    size = MAX_VERTEX_PER_MESH;
                }
            }

            return true;
        }

        /// <summary>
        /// Set the sprite sheet for the map layer
        /// </summary>
        /// <param name="layer">Map layer</param>
        /// <param name="spriteSheet">Sprite sheet</param>
        public void MapLayerSpriteSheetSet(int layer, SpriteSheetAsset spriteSheet)
        {
            if (mLayerSpriteSheets[layer] != spriteSheet)
            {
                Texture oldTexture = null;
                if (mLayerSpriteSheets[layer] != null)
                {
                    oldTexture = mLayerSpriteSheets[layer].internalState.texture;
                }

                Texture newTexture = null;
                if (spriteSheet != null)
                {
                    newTexture = spriteSheet.internalState.texture;
                }

                mLayerSpriteSheets[layer] = spriteSheet;

                // Regen not required
                if (oldTexture == newTexture)
                {
                    return;
                }

                // Regen required
                if ((oldTexture == null && newTexture != null) || (oldTexture != null && newTexture == null))
                {
                    RegenLayerChunks(layer);
                    return;
                }
                else if (oldTexture.width != newTexture.width || oldTexture.height != newTexture.height)
                {
                    RegenLayerChunks(layer);
                    return;
                }
            }
        }

        /// <summary>
        /// Draw map layer at given position offset
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="pos">Position</param>
        public void DrawMapLayer(int layer, Vector2i pos)
        {
            var renderer = mRetroBlitAPI.Renderer;
            var renderTarget = renderer.CurrentRenderTexture();

            var oldCamera = renderer.CameraGet();
            var newCamera = renderer.CameraGet();
            newCamera.x -= pos.x;
            newCamera.y -= pos.y;
            renderer.CameraSet(newCamera);

            var spriteSheet = mLayerSpriteSheets[layer];

            if (spriteSheet == null)
            {
                RBUtil.LogErrorOnce("Map layer doesn't have a sprite sheet set! Use RB.MapLayerSpriteSheetSet to set layer sprite sheet.");
                return;
            }

            if (spriteSheet.internalState.spriteGrid.cellSize.width <= 0 || spriteSheet.internalState.spriteGrid.cellSize.height <= 0)
            {
                RBUtil.LogErrorOnce("Can't draw tilemap, sprite size of the layer spritesheet is invalid!");
                return;
            }

            int drawX = newCamera.x;
            int drawY = newCamera.y;
            int drawYStart = drawY;

            int chunkDrawWidth = spriteSheet.internalState.spriteGrid.cellSize.width * CHUNK_WIDTH;
            int chunkDrawHeight = spriteSheet.internalState.spriteGrid.cellSize.height * CHUNK_HEIGHT;

            int x0 = drawX;
            int y0 = drawY;
            int x1 = x0 + renderTarget.width;
            int y1 = y0 + renderTarget.height;

            if (newCamera.x > 0 && newCamera.x % chunkDrawWidth != 0)
            {
                x1 += newCamera.x % chunkDrawWidth;
            }

            if (newCamera.x < 0)
            {
                x1 += chunkDrawWidth;
            }

            if (newCamera.y > 0 && newCamera.y % chunkDrawHeight != 0)
            {
                y1 += newCamera.y % chunkDrawHeight;
            }

            if (newCamera.y < 0)
            {
                y1 += chunkDrawHeight;
            }

            for (int x = x0; x < x1; x += chunkDrawWidth)
            {
                int cx = x / spriteSheet.internalState.spriteGrid.cellSize.width;

                // Correct for negatives because -1 should not be the same chunk as 1!
                if (x < 0)
                {
                    cx--;
                }

                for (int y = y0; y < y1; y += chunkDrawHeight)
                {
                    int cy = y / spriteSheet.internalState.spriteGrid.cellSize.height;
                    if (y < 0)
                    {
                        cy--;
                    }

                    var chunk = GetChunk(layer, cx, cy, false);
                    if (chunk != null)
                    {
                        // Generate chunk if needed
                        if (chunk.dirty || chunk.released)
                        {
                            if (chunk.released && chunk.mesh == null)
                            {
                                chunk.mesh = GetNewMesh();
                                if (chunk.mesh == null)
                                {
                                    Debug.LogError("Could not get chunk for tilemap mesh!");
                                }
                            }

                            GenerateChunk(
                                layer,
                                cx,
                                cy,
                                chunk.x * spriteSheet.internalState.spriteGrid.cellSize.width * CHUNK_WIDTH,
                                chunk.y * spriteSheet.internalState.spriteGrid.cellSize.height * CHUNK_HEIGHT);

                            chunk.dirty = false;
                            chunk.released = false;
                        }

                        if (mLayerSpriteSheets[layer] != null)
                        {
                            var chunkPos = new Vector2i(x, y);

                            chunkPos -= newCamera;
                            chunkPos.x -= x % (spriteSheet.internalState.spriteGrid.cellSize.width * CHUNK_WIDTH);
                            chunkPos.y -= y % (spriteSheet.internalState.spriteGrid.cellSize.height * CHUNK_HEIGHT);

                            renderer.DrawPreparedMesh(
                                chunk.mesh,
                                chunk.x * spriteSheet.internalState.spriteGrid.cellSize.width * CHUNK_WIDTH,
                                chunk.y * spriteSheet.internalState.spriteGrid.cellSize.height * CHUNK_HEIGHT,
                                new Rect2i(chunkPos.x, chunkPos.y, chunkDrawWidth, chunkDrawHeight),
                                true,
                                mLayerSpriteSheets[layer].internalState.texture);
                        }
                    }

                    drawY += chunkDrawHeight;
                }

                drawX += chunkDrawWidth;
                drawY = drawYStart;
            }

            renderer.CameraSet(oldCamera);
        }

        /// <summary>
        /// Set sprite for the tile at x, y
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sprite">Sprite index</param>
        /// <param name="tintColor">Tint color</param>
        /// <param name="flags">Flags</param>
        public void SpriteSet(int layer, int x, int y, int sprite, Color32 tintColor, int flags = 0)
        {
            if (sprite < 0 && (flags & SPRITEPACK) == 0)
            {
                return;
            }

            if (layer < 0 || layer >= mRetroBlitAPI.HW.MapLayers)
            {
                return;
            }

            // Bounds check on user defined map size, not our internal map size
            if (x < 0 || y < 0 || x >= mRetroBlitAPI.HW.MapSize.width || y >= mRetroBlitAPI.HW.MapSize.height)
            {
                return;
            }

            Tile[] tilesArr;
            int tileIndex;
            if (!GetTileRef(layer, x, y, out tilesArr, out tileIndex, true))
            {
                return;
            }

            Tile t = tilesArr[tileIndex];

            // If there are no changes then do nothing
            if (t.sprite == sprite &&
                (t.tintColor.r == tintColor.r && t.tintColor.g == tintColor.g && t.tintColor.b == tintColor.b && t.tintColor.a == tintColor.a) &&
                t.flags == flags)
            {
                return;
            }

            int oldSprite = t.sprite;

            tilesArr[tileIndex].sprite = sprite;
            tilesArr[tileIndex].tintColor = tintColor;
            tilesArr[tileIndex].flags = (byte)flags;

            var chunk = GetChunk(layer, x, y, true);
            chunk.dirty = true;

            // Update count of nonempty tiles
            if (oldSprite >= RB.SPRITE_EMPTY && sprite < RB.SPRITE_EMPTY)
            {
                chunk.nonEmptyTiles++;
            }
            else if (oldSprite < RB.SPRITE_EMPTY && sprite >= RB.SPRITE_EMPTY)
            {
                chunk.nonEmptyTiles--;
            }
        }

        /// <summary>
        /// Get sprite index at position
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Sprite index</returns>
        public int SpriteGet(int layer, int x, int y)
        {
            Tile t = new Tile();
            if (!GetTile(layer, x, y, ref t, true))
            {
                return RB.SPRITE_EMPTY;
            }

            return t.sprite;
        }

        /// <summary>
        /// Set user data for tile at x, y
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="layer">Layer</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="data">Data</param>
        public void DataSet<T>(int layer, int x, int y, T data)
        {
            Tile[] tilesArr;
            int tileIndex;

            if (!GetTileRef(layer, x, y, out tilesArr, out tileIndex, true))
            {
                return;
            }

            tilesArr[tileIndex].data = (object)data;
        }

        /// <summary>
        /// Get data for sprite at x, y
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="layer">Layer</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>User data</returns>
        public object DataGet<T>(int layer, int x, int y)
        {
            Tile t = new Tile();

            if (!GetTile(layer, x, y, ref t, false))
            {
                return null;
            }

            return (T)t.data;
        }

        /// <summary>
        /// Clear a map layer, or all layers if -1
        /// </summary>
        /// <param name="layer">Layer</param>
        public void Clear(int layer = -1)
        {
            // Wipe all layers
            if (layer == -1)
            {
                for (int i = 0; i < mTileLayers.Length; i++)
                {
                    mTileLayers[i] = null;
                }

                for (int i = 0; i < mChunks.Length; i++)
                {
                    if (mChunks[i] != null)
                    {
                        if (mChunks[i].mesh != null)
                        {
                            mChunks[i].mesh.Clear();
                            ReleaseMesh(mChunks[i].mesh);
                            mChunks[i].mesh = null;
                        }

                        mChunks[i] = null;
                    }
                }

                mActiveChunks.Clear();

                return;
            }

            // Wipe specific layer
            if (layer < 0 || layer >= mRetroBlitAPI.HW.MapLayers)
            {
                return;
            }

            mTileLayers[layer] = null;

            int chunksPerLayer = ((mActualMapSize.width / CHUNK_WIDTH) + 1) * ((mActualMapSize.height / CHUNK_HEIGHT) + 1);
            for (int i = chunksPerLayer * layer; i < chunksPerLayer * (layer + 1); i++)
            {
                if (mChunks[i] != null)
                {
                    if (mChunks[i].mesh != null)
                    {
                        mChunks[i].mesh.Clear();
                        ReleaseMesh(mChunks[i].mesh);
                        mChunks[i].mesh = null;
                    }

                    mChunks[i] = null;
                }
            }

            // Remove all tracked active chunks for this layer
            for (int i = mActiveChunks.Count - 1; i >= 0; i--)
            {
                if (mActiveChunks[i].chunkLayer == layer)
                {
                    mActiveChunks.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Do some cleanup at frame end
        /// </summary>
        public void FrameEnd()
        {
            mFrameCounter++;

            // Every 100 frames do a bit of maintenance, release stale chunks
            if (mFrameCounter % 100 == 0)
            {
                for (int i = mActiveChunks.Count - 1; i >= 0; i--)
                {
                    var chunk = mActiveChunks[i];
                    if (mFrameCounter - chunk.lastRelevantFrame > 200)
                    {
                        if (chunk.mesh != null)
                        {
                            chunk.mesh.Clear();
                            ReleaseMesh(chunk.mesh);
                            chunk.mesh = null;
                        }

                        // Mark the chunk released so its recreated next time we fetch it
                        chunk.released = true;

                        mActiveChunks.RemoveAt(i);
                    }
                }
            }

            UpdateAsyncResources();
        }

        /// <summary>
        /// Check if chunk is empty
        /// </summary>
        /// <param name="layer">Chunk layer</param>
        /// <param name="pos">Offset of chunk in tile coordinates</param>
        /// <returns>True if empty</returns>
        public bool ChunkEmptyGet(int layer, Vector2i pos)
        {
            var chunk = GetChunk(layer, pos.x, pos.y, false);

            if (chunk == null)
            {
                return true;
            }

            if (chunk.mesh == null || chunk.nonEmptyTiles == 0 || chunk.released == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shift map chunks for the given layer by the given x, y amount
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="shift">Shift amount</param>
        public void ShiftChunks(int layer, Vector2i shift)
        {
            if (layer < 0 || layer >= mRetroBlitAPI.HW.MapLayers)
            {
                return;
            }

            if (shift.x == 0 && shift.y == 0)
            {
                // Do nothing
                return;
            }

            int chunkColumns = mActualMapSize.width / CHUNK_WIDTH;
            int chunkRows = mActualMapSize.height / CHUNK_HEIGHT;

            if (shift.x >= chunkColumns || shift.x <= -chunkColumns ||
                shift.y >= chunkRows || shift.y <= -chunkRows)
            {
                // If shift is greater than chunk count then we can just wipe the layer instead
                Clear(layer);
                return;
            }

            // *** Shift tile data
            var tiles = mTileLayers[layer].tiles;

            // Shift columns first, this is the slower operation of the two as we have to shift each row individually
            if (shift.x < 0)
            {
                // Shift left
                for (int row = 0; row < mActualMapSize.height; row++)
                {
                    int xOffset = -shift.x;
                    int writeIndex = row * mActualMapSize.width;
                    int copyLength = (chunkColumns - xOffset) * CHUNK_WIDTH;
                    System.Array.Copy(tiles, writeIndex + (xOffset * CHUNK_WIDTH), tiles, writeIndex, copyLength);

                    int clearIndex = (row * mActualMapSize.width) + ((chunkColumns - xOffset) * CHUNK_WIDTH);
                    int clearLength = xOffset * CHUNK_WIDTH;

                    ClearTiles(tiles, clearIndex, clearLength);
                }
            }
            else if (shift.x > 0)
            {
                // Shift right
                for (int row = 0; row < mActualMapSize.height; row++)
                {
                    int xOffset = shift.x;
                    int rowOffset = row * mActualMapSize.width;
                    int copyLength = (chunkColumns - xOffset) * CHUNK_WIDTH;
                    System.Array.Copy(tiles, rowOffset, tiles, rowOffset + (xOffset * CHUNK_WIDTH), copyLength);

                    int clearIndex = row * mActualMapSize.width;
                    int clearLength = xOffset * CHUNK_WIDTH;

                    ClearTiles(tiles, clearIndex, clearLength);
                }
            }

            // Shift rows
            if (shift.y < 0)
            {
                // Shift up
                int srcOffset = mActualMapSize.width * CHUNK_HEIGHT * (-shift.y);
                int copyLength = (mActualMapSize.width * mActualMapSize.height) - srcOffset;
                System.Array.Copy(tiles, srcOffset, tiles, 0, copyLength);
                ClearTiles(tiles, copyLength, (mActualMapSize.width * mActualMapSize.height) - copyLength);
            }
            else if (shift.y > 0)
            {
                // Shift down
                int destOffset = mActualMapSize.width * CHUNK_HEIGHT * shift.y;
                int copyLength = (chunkRows - shift.y) * CHUNK_HEIGHT * mActualMapSize.width;
                System.Array.Copy(tiles, 0, tiles, destOffset, copyLength);
                ClearTiles(tiles, 0, mActualMapSize.width * CHUNK_HEIGHT * shift.y);
            }

            // *** Shift chunks
            if (shift.x < 0)
            {
                // Shift left, fill in nulls on the right
                int xOffset = -shift.x;
                for (int y = 0; y < chunkRows; y++)
                {
                    int writeIndex = (y * mChunksStride) + (layer * mChunksPerLayer);

                    int x;

                    for (x = 0; x < chunkColumns - xOffset; x++)
                    {
                        // Release chunks that will be overwritten (up to xOffset)
                        if (x < xOffset && mChunks[writeIndex] != null)
                        {
                            ReleaseMesh(mChunks[writeIndex].mesh);
                            mActiveChunks.Remove(mChunks[writeIndex]);
                        }

                        int readIndex = writeIndex + xOffset;

                        mChunks[writeIndex] = mChunks[readIndex];
                        if (mChunks[writeIndex] != null)
                        {
                            mChunks[writeIndex].x = mChunks[readIndex].x - xOffset;
                        }

                        writeIndex++;
                    }

                    // Release chunks on the right which should now be empty
                    // Remove from right to left, which should be faster because elements don't need
                    // to be shifted
                    writeIndex = (y * mChunksStride) + (layer * mChunksPerLayer) + chunkColumns - 1;
                    for (int i = 0; i < xOffset; i++)
                    {
                        mChunks[writeIndex] = null;
                        writeIndex--;
                    }
                }
            }
            else if (shift.x > 0)
            {
                // Shift right, fill in nulls on the left
                int xOffset = shift.x;
                for (int y = 0; y < chunkRows; y++)
                {
                    int writeIndex = (y * mChunksStride) + (layer * mChunksPerLayer) + chunkColumns - 1;

                    int x;

                    for (x = chunkColumns - 1; x >= xOffset; x--)
                    {
                        // Release chunks that will be overwritten (up to xOffset)
                        if (x >= chunkColumns - xOffset && mChunks[writeIndex] != null)
                        {
                            ReleaseMesh(mChunks[writeIndex].mesh);
                            mActiveChunks.Remove(mChunks[writeIndex]);
                        }

                        int readIndex = writeIndex - xOffset;

                        mChunks[writeIndex] = mChunks[readIndex];
                        if (mChunks[writeIndex] != null)
                        {
                            mChunks[writeIndex].x = mChunks[readIndex].x + xOffset;
                        }

                        writeIndex--;
                    }

                    // Release chunks on the left
                    writeIndex = (y * mChunksStride) + (layer * mChunksPerLayer);
                    for (int i = 0; i < xOffset; i++)
                    {
                        mChunks[writeIndex] = null;
                        writeIndex++;
                    }
                }
            }

            if (shift.y < 0)
            {
                // Shift up, fill in nulls on the bottom
                int yOffset = -shift.y;
                for (int x = 0; x < chunkColumns; x++)
                {
                    int writeIndex = x + (layer * mChunksPerLayer);

                    int y;

                    for (y = 0; y < chunkRows - yOffset; y++)
                    {
                        // Release chunks that will be overwritten (up to yOffset)
                        if (y < yOffset && mChunks[writeIndex] != null)
                        {
                            ReleaseMesh(mChunks[writeIndex].mesh);
                            mActiveChunks.Remove(mChunks[writeIndex]);
                        }

                        int readIndex = writeIndex + (yOffset * mChunksStride);

                        mChunks[writeIndex] = mChunks[readIndex];
                        if (mChunks[writeIndex] != null)
                        {
                            mChunks[writeIndex].y = mChunks[readIndex].y - yOffset;
                        }

                        writeIndex += mChunksStride;
                    }

                    // Release chunks on the right which should now be empty
                    // Remove from right to left, which should be faster because elements don't need
                    // to be shifted
                    writeIndex = x + (layer * mChunksPerLayer) + ((chunkRows - 1) * mChunksStride);
                    for (int i = 0; i < yOffset; i++)
                    {
                        mChunks[writeIndex] = null;
                        writeIndex -= mChunksStride;
                    }
                }
            }
            else if (shift.y > 0)
            {
                // Shift down, fill in nulls on the top
                int yOffset = shift.y;
                for (int x = 0; x < chunkColumns; x++)
                {
                    int writeIndex = x + ((chunkRows - 1) * mChunksStride) + (layer * mChunksPerLayer);

                    int y;

                    for (y = chunkRows - 1; y >= yOffset; y--)
                    {
                        // Release chunks that will be overwritten (up to xOffset)
                        if (y >= chunkRows - yOffset && mChunks[writeIndex] != null)
                        {
                            ReleaseMesh(mChunks[writeIndex].mesh);
                            mActiveChunks.Remove(mChunks[writeIndex]);
                        }

                        int readIndex = writeIndex - (yOffset * mChunksStride);

                        mChunks[writeIndex] = mChunks[readIndex];
                        if (mChunks[writeIndex] != null)
                        {
                            mChunks[writeIndex].y = mChunks[readIndex].y + yOffset;
                        }

                        writeIndex -= mChunksStride;
                    }

                    // Release chunks on the left
                    writeIndex = x + (layer * mChunksPerLayer);
                    for (int i = 0; i < yOffset; i++)
                    {
                        mChunks[writeIndex] = null;
                        writeIndex += mChunksStride;
                    }
                }
            }
        }

        /// <summary>
        /// Render some layer debug information. For debugging only.
        /// </summary>
        public void DebugRender()
        {
            int size = 32;
            int i = 0;
            int tempWidth, tempHeight;

            mDebugString.Clear();
            mDebugStringWrap.Set(mDebugString);

            for (int y = 0; y < RB.MapSize.height / RB.MapChunkSize.y; y++)
            {
                for (int x = 0; x < RB.MapSize.width / RB.MapChunkSize.x; x++)
                {
                    var chunk = mChunks[x + (y * mChunksStride)];

                    if (chunk == null)
                    {
                        mRetroBlitAPI.Renderer.DrawRectFill(x * (size + 1), y * (size + 1), size, size, Color.white, 0, 0);
                    }
                    else
                    {
                        var rect = new Rect2i(x * (size + 1), y * (size + 1), size, size);
                        mRetroBlitAPI.Renderer.DrawRectFill(x * (size + 1), y * (size + 1), size, size, new Color32(255, 0, 255, 255), 0, 0);

                        mDebugString.Append(chunk.nonEmptyTiles).Append("\n").Append(mFrameCounter - chunk.lastRelevantFrame);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                        mRetroBlitAPI.Font.Print(null, rect, new Color32(0, 0, 0, 255), RB.ALIGN_H_CENTER | RB.ALIGN_V_CENTER, mDebugStringWrap, false, out tempWidth, out tempHeight);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                    }

                    i++;
                }
            }

            mDebugString.Set("Active Chunks:\n");
            for (i = 0; i < mActiveChunks.Count; i++)
            {
                mDebugString.Append(mActiveChunks[i].x).Append(", ").Append(mActiveChunks[i].y).Append("\n");
            }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
            mRetroBlitAPI.Font.Print(null, new Rect2i(200, 4, 100, 10000), new Color32(0, 0, 0, 255), 0, mDebugStringWrap, false, out tempWidth, out tempHeight);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        /// <summary>
        /// Get tile references for given layer, and tile index
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="tilesArr">Result tiles array</param>
        /// <param name="index">Result index</param>
        /// <param name="create">Create the tiles if they don't exist</param>
        /// <returns>True if successful</returns>
        public bool GetTileRef(int layer, int x, int y, out Tile[] tilesArr, out int index, bool create = false)
        {
            // Boundary check on user defined size not internal one
            if (x < 0 || x >= mRetroBlitAPI.HW.MapSize.width || y < 0 || y >= mRetroBlitAPI.HW.MapSize.height)
            {
                tilesArr = null;
                index = 0;
                return false;
            }

            var tileLayer = mTileLayers[layer];
            if (tileLayer == null)
            {
                if (create)
                {
                    tileLayer = new TileLayer(mActualMapSize.width * mActualMapSize.height);
                    if (tileLayer == null)
                    {
                        tilesArr = null;
                        index = 0;
                        return false;
                    }

                    mTileLayers[layer] = tileLayer;
                }
                else
                {
                    tilesArr = null;
                    index = 0;
                    return false;
                }
            }

            tilesArr = mTileLayers[layer].tiles;
            index = x + (y * mActualMapSize.width);

            return true;
        }

        private static void ClearTiles(Tile[] tiles, int startIndex, int length)
        {
            int clearIncr = mClearingTiles.Length;

            while (length > 0)
            {
                int clearLength = length < clearIncr ? length : clearIncr;
                System.Array.Copy(mClearingTiles, 0, tiles, startIndex, clearLength);
                length -= clearLength;
                startIndex += clearLength;
            }
        }

        private ArraySet GetArraySet(int minVerts)
        {
            for (int i = 0; i < mArraySets.Count; i++)
            {
                var arraySet = mArraySets[i];
                if (arraySet.MaxVerts >= minVerts)
                {
                    return arraySet;
                }
            }

            return null;
        }

        private Chunk GetChunk(int layer, int x, int y, bool create)
        {
            if (x < 0 || x >= mActualMapSize.width || y < 0 || y >= mActualMapSize.height)
            {
                return null;
            }

            x /= CHUNK_WIDTH;
            y /= CHUNK_HEIGHT;

            int i = x + (y * mChunksStride);
            i += layer * mChunksPerLayer;

            if (i < 0 || i >= mChunks.Length)
            {
                Debug.LogError("Chunk out of bounds");
                return null;
            }

            var chunk = mChunks[i];

            if (chunk == null && create)
            {
                chunk = new Chunk
                {
                    mesh = GetNewMesh()
                };

                if (chunk.mesh == null)
                {
                    Debug.LogError("Could not get chunk for tilemap mesh!");
                }

                chunk.chunkLayer = layer;
                chunk.x = x;
                chunk.y = y;
                mChunks[i] = chunk;
            }

            if (chunk != null)
            {
                chunk.lastRelevantFrame = mFrameCounter;
            }

            return chunk;
        }

        private bool GetTile(int layer, int x, int y, ref Tile tile, bool create = false)
        {
            // Boundary check on user defined size not internal one
            if (x < 0 || x >= mRetroBlitAPI.HW.MapSize.width || y < 0 || y >= mRetroBlitAPI.HW.MapSize.height)
            {
                return false;
            }

            var tileLayer = mTileLayers[layer];
            if (tileLayer == null)
            {
                if (create)
                {
                    tileLayer = new TileLayer(mActualMapSize.width * mActualMapSize.height);
                    if (tileLayer == null)
                    {
                        return false;
                    }

                    mTileLayers[layer] = tileLayer;
                }
                else
                {
                    return false;
                }
            }

            tile = mTileLayers[layer].tiles[x + (y * mActualMapSize.width)];

            return true;
        }

        private Mesh GetNewMesh()
        {
            Mesh mesh;
            if (mReleasedMeshes.Count > 0)
            {
                mesh = mReleasedMeshes[mReleasedMeshes.Count - 1];
                mReleasedMeshes.RemoveAt(mReleasedMeshes.Count - 1);
                return mesh;
            }

            mesh = new Mesh();
            mesh.MarkDynamic();
            return mesh;
        }

        private void GenerateChunk(int layer, int offsetX, int offsetY, int pixelOffsetX, int pixelOffsetY)
        {
            // Clip to upper left corner of the chunk
            int x0 = offsetX - (offsetX % CHUNK_WIDTH);
            int y0 = offsetY - (offsetY % CHUNK_HEIGHT);
            int x1 = x0 + CHUNK_WIDTH;
            int y1 = y0 + CHUNK_HEIGHT;

            int i = 0;
            int j = 0;

            var spriteSheet = mLayerSpriteSheets[layer];
            if (spriteSheet == null)
            {
                Debug.LogError("Layer has no sprite sheet assigned!");
                return;
            }

            int spriteSheetWidth = spriteSheet.internalState.spriteGrid.cellSize.width;
            int spriteSheetHeight = spriteSheet.internalState.spriteGrid.cellSize.height;
            int spriteSheetTextureWidth = spriteSheet.internalState.textureWidth;
            int spriteSheetTextureHeight = spriteSheet.internalState.textureHeight;

            int maxSpriteIndex = spriteSheet.internalState.columns * spriteSheet.internalState.rows;

            var chunk = GetChunk(layer, x0, y0, true);
            var arraySet = GetArraySet(chunk.nonEmptyTiles * 4);

            if (arraySet == null)
            {
                Debug.LogError("Failed to generate tilemap chunk!");
                return;
            }

            var ChunkVerticies = arraySet.ChunkVerticies;
            var ChunkUvs = arraySet.ChunkUvs;
            var ChunkTexRegion0 = arraySet.ChunkTexRegion0;
            var ChunkTexRegion1 = arraySet.ChunkTexRegion1;
            var ChunkColors = arraySet.ChunkColors;
            var ChunkIndecies = arraySet.ChunkIndecies;

            int spritesGenerated = 0;
            bool doneEarly = false;
            for (int x = x0; x < x1 && !doneEarly; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    Tile t = new Tile();
                    if (!GetTile(layer, x, y, ref t, false))
                    {
                        continue;
                    }

                    int flags = (int)t.flags;
                    int spritePack = t.flags & SPRITEPACK;

                    int spriteIndex = t.sprite;

                    if ((spriteIndex < 0 || spriteIndex >= maxSpriteIndex) && (spritePack == 0))
                    {
                        continue;
                    }

                    int dx0 = (x * spriteSheetWidth) - pixelOffsetX;
                    int dy0 = (y * spriteSheetHeight) - pixelOffsetY;
                    int dx1 = ((x + 1) * spriteSheetWidth) - pixelOffsetX;
                    int dy1 = ((y + 1) * spriteSheetHeight) - pixelOffsetY;

                    float ux0raw = 0;
                    float uy0raw = 1;
                    float ux1raw = 1;
                    float uy1raw = 0;

                    float ux0, uy0, ux1, uy1;

                    Color32 color = t.tintColor;

                    if (spritePack == 0)
                    {
                        float tex_x0 = ((spriteIndex % spriteSheet.internalState.columns) * spriteSheetWidth) / (float)spriteSheetTextureWidth;
                        float tex_y0 = 1.0f - (((spriteIndex / spriteSheet.internalState.columns) * spriteSheetHeight) / ((float)spriteSheetTextureHeight));
                        float tex_x1 = tex_x0 + (spriteSheetWidth / (float)spriteSheetTextureWidth);
                        float tex_y1 = tex_y0 - (spriteSheetHeight / (float)spriteSheetTextureHeight);

                        if ((flags & RB.FLIP_H) == 0)
                        {
                            ux0 = ux0raw;
                            ux1 = ux1raw;
                        }
                        else
                        {
                            ux0 = ux1raw;
                            ux1 = ux0raw;
                        }

                        if ((flags & RB.FLIP_V) == 0)
                        {
                            uy0 = uy0raw;
                            uy1 = uy1raw;
                        }
                        else
                        {
                            uy0 = uy1raw;
                            uy1 = uy0raw;
                        }

                        uy0 = 1.0f - uy0;
                        uy1 = 1.0f - uy1;

                        if ((flags & RB.ROT_90_CW) == 0)
                        {
                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;
                        }
                        else
                        {
                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;
                        }
                    }
                    else
                    {
                        var packedSprite = mRetroBlitAPI.Renderer.PackedSpriteGet(spriteIndex, spriteSheet);

                        float tex_x0 = packedSprite.SourceRect.x / (float)spriteSheetTextureWidth;
                        float tex_y0 = 1.0f - (packedSprite.SourceRect.y / (float)spriteSheetTextureHeight);
                        float tex_x1 = (packedSprite.SourceRect.x + packedSprite.SourceRect.width) / (float)spriteSheetTextureWidth;
                        float tex_y1 = 1.0f - ((packedSprite.SourceRect.y + packedSprite.SourceRect.height) / (float)spriteSheetTextureHeight);

                        int sx0;
                        int sy0;
                        int sx1;
                        int sy1;

                        dx1 = dx0;
                        dy1 = dy0;

                        if ((flags & RB.ROT_90_CW) == 0)
                        {
                            if ((flags & RB.FLIP_H) == 0)
                            {
                                ux0 = ux0raw;
                                ux1 = ux1raw;

                                sx0 = packedSprite.TrimOffset.x;
                                sx1 = sx0 + packedSprite.SourceRect.width;
                            }
                            else
                            {
                                ux0 = ux1raw;
                                ux1 = ux0raw;

                                sx1 = spriteSheetWidth - packedSprite.TrimOffset.x;
                                sx0 = sx1 - packedSprite.SourceRect.width;
                            }

                            if ((flags & RB.FLIP_V) == 0)
                            {
                                uy0 = uy0raw;
                                uy1 = uy1raw;

                                sy0 = packedSprite.TrimOffset.y;
                                sy1 = sy0 + packedSprite.SourceRect.height;
                            }
                            else
                            {
                                uy0 = uy1raw;
                                uy1 = uy0raw;

                                sy1 = spriteSheetHeight - packedSprite.TrimOffset.y;
                                sy0 = sy1 - packedSprite.SourceRect.height;
                            }

                            dx0 += sx0;
                            dx1 += sx1;
                            dy0 += sy0;
                            dy1 += sy1;

                            uy0 = 1.0f - uy0;
                            uy1 = 1.0f - uy1;

                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;
                        }
                        else
                        {
                            if ((flags & RB.FLIP_V) == 0)
                            {
                                ux0 = ux0raw;
                                ux1 = ux1raw;

                                sy0 = packedSprite.TrimOffset.x;
                                sy1 = sy0 + packedSprite.SourceRect.width;
                            }
                            else
                            {
                                ux0 = ux1raw;
                                ux1 = ux0raw;

                                sy1 = spriteSheetWidth - packedSprite.TrimOffset.x;
                                sy0 = sy1 - packedSprite.SourceRect.width;
                            }

                            if ((flags & RB.FLIP_H) == 0)
                            {
                                uy0 = uy0raw;
                                uy1 = uy1raw;

                                sx1 = spriteSheetHeight - packedSprite.TrimOffset.y;
                                sx0 = sx1 - packedSprite.SourceRect.height;
                            }
                            else
                            {
                                uy0 = uy1raw;
                                uy1 = uy0raw;

                                sx0 = packedSprite.TrimOffset.y;
                                sx1 = sx0 + packedSprite.SourceRect.height;
                            }

                            dx0 += sx0;
                            dx1 += sx1;
                            dy0 += sy0;
                            dy1 += sy1;

                            uy0 = 1.0f - uy0;
                            uy1 = 1.0f - uy1;

                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy0;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux0;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx1;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy0;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;

                            ChunkVerticies[i].x = dx0;
                            ChunkVerticies[i].y = dy1;
                            ChunkVerticies[i].z = 1;
                            ChunkUvs[i].x = ux1;
                            ChunkUvs[i].y = uy1;
                            ChunkTexRegion0[i].x = tex_x0;
                            ChunkTexRegion0[i].y = tex_y0;
                            ChunkTexRegion1[i].x = tex_x1;
                            ChunkTexRegion1[i].y = tex_y1;
                            ChunkColors[i] = color;

                            i++;
                        }
                    }

                    ChunkIndecies[j++] = (ushort)(i - 4);
                    ChunkIndecies[j++] = (ushort)(i - 3);
                    ChunkIndecies[j++] = (ushort)(i - 2);

                    ChunkIndecies[j++] = (ushort)(i - 2);
                    ChunkIndecies[j++] = (ushort)(i - 1);
                    ChunkIndecies[j++] = (ushort)(i - 4);

                    spritesGenerated++;

                    if (spritesGenerated >= chunk.nonEmptyTiles)
                    {
                        doneEarly = true;
                        break;
                    }
                }
            }

            if (j > 0)
            {
                if (chunk != null && chunk.mesh != null)
                {
                    System.Array.Clear(ChunkIndecies, j, arraySet.MaxIndecies - j);

                    // No need to clear mesh if it's the same size as new vertex data, the values will just
                    // be overwritten
                    if (chunk.prevVertCount != ChunkVerticies.Length)
                    {
                        chunk.mesh.Clear();
                    }

                    chunk.mesh.vertices = ChunkVerticies;
                    chunk.mesh.uv = ChunkUvs;
                    chunk.mesh.uv2 = ChunkTexRegion0;
                    chunk.mesh.uv3 = ChunkTexRegion1;
                    chunk.mesh.colors32 = ChunkColors;
                    chunk.mesh.SetIndices(ChunkIndecies, MeshTopology.Triangles, 0, false);

                    chunk.prevVertCount = ChunkVerticies.Length;

                    chunk.mesh.UploadMeshData(false);

                    mActiveChunks.Add(chunk);
                }
            }
            else
            {
                ReleaseChunk(layer, x0, y0);
            }
        }

        private void RegenLayerChunks(int layer)
        {
            if (layer < 0 || layer >= mRetroBlitAPI.HW.MapLayers)
            {
                return;
            }

            int chunksPerLayer = mChunksStride * ((mActualMapSize.height / CHUNK_HEIGHT) + 1);
            for (int i = chunksPerLayer * layer; i < chunksPerLayer * (layer + 1); i++)
            {
                if (mChunks[i] != null)
                {
                    mChunks[i].dirty = true;
                }
            }
        }

        private void ReleaseChunk(int layer, int x, int y)
        {
            if (x < 0 || x >= mActualMapSize.width || y < 0 || y >= mActualMapSize.height)
            {
                return;
            }

            x /= CHUNK_WIDTH;
            y /= CHUNK_HEIGHT;

            int i = x + (y * mChunksStride);
            i += layer * mChunksPerLayer;

            if (i < 0 || i >= mChunks.Length)
            {
                Debug.LogError("Chunk out of bounds");
                return;
            }

            if (mChunks[i] != null && mChunks[i].mesh != null)
            {
                ReleaseMesh(mChunks[i].mesh);
                mChunks[i].mesh = null;
            }

            mChunks[i] = null;
        }

        private void ReleaseMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                return;
            }

            if (mReleasedMeshes.Count < MAX_CACHED_MESHES)
            {
                mReleasedMeshes.Add(mesh);
            }
            else
            {
                GameObject.DestroyImmediate(mesh, true);
            }
        }

        /// <summary>
        /// Map tile definition
        /// </summary>
        public struct Tile
        {
            /// <summary>
            /// Sprite index
            /// </summary>
            public int sprite;

            /// <summary>
            /// Tint color
            /// </summary>
            public Color32 tintColor;

            /// <summary>
            /// Tile flags
            /// </summary>
            public byte flags;

            /// <summary>
            /// User data
            /// </summary>
            public object data;

            /// <summary>
            /// Clear tile
            /// </summary>
            public void Clear()
            {
                sprite = RB.SPRITE_EMPTY;
                tintColor = Color.white;
                flags = 0;

                data = null;
            }
        }

        private class TileLayer
        {
            public Tile[] tiles = null;

            public TileLayer(int tileCount)
            {
                tiles = new Tile[tileCount];

                RBTilemapTMX.ClearTiles(tiles, 0, tileCount);
            }
        }

        private class Chunk
        {
            public Mesh mesh;
            public bool dirty = false;
            public bool released = false;
            public int chunkLayer = -1;
            public int nonEmptyTiles = 0;
            public int prevVertCount = 0;

            /// <summary>
            /// Last frame in which the chunk was still needed
            /// </summary>
            public ulong lastRelevantFrame = 0;
            public int x, y;
        }

        private class ArraySet
        {
            public Vector3[] ChunkVerticies;
            public Vector2[] ChunkUvs;
            public Vector2[] ChunkTexRegion0;
            public Vector2[] ChunkTexRegion1;
            public Color32[] ChunkColors;
            public ushort[] ChunkIndecies;

            public int MaxVerts;
            public int MaxIndecies;

            public ArraySet(int maxVerts)
            {
                MaxVerts = maxVerts;
                MaxIndecies = maxVerts / 4 * 6;

                ChunkVerticies = new Vector3[MaxVerts];
                ChunkUvs = new Vector2[MaxVerts];
                ChunkTexRegion0 = new Vector2[MaxVerts];
                ChunkTexRegion1 = new Vector2[MaxVerts];
                ChunkColors = new Color32[MaxVerts];
                ChunkIndecies = new ushort[MaxIndecies];
            }
        }
    }
}
