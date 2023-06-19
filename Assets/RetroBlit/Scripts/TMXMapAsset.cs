using System;
using System.Collections.Generic;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// TMX Map asset
/// </summary>
/// <remarks>
/// Contains a map loaded from the TMX tilemap format. TMX Map assets can be loaded from various sources, including synchronous and asynchronous sources.
/// Use <see cref="TMXMapAsset.Load"/> to load a TMX Map asset. This asset contains various information about the map, its layers, and objects. It does
/// not however contain layer tile data, tile data is loaded with <see cref="TMXMapAsset.LoadLayer"/>, and <see cref="TMXMapAsset.LoadLayerChunk"/>. Because the tile
/// data is loaded and stored separately the <see cref="TMXMapAsset"/> is relatively light-weight and can be kept in memory for future reference.
/// </remarks>
public class TMXMapAsset : RBAsset
{
    /// <summary>
    /// Internal TMX map state, do not change
    /// </summary>
    public TMXMapAssetInternalState internalState;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    public TMXMapAsset()
    {
        internalState.mapDef = new RetroBlitInternal.RBTilemapTMX.TMXMapDef();
    }

    /// <summary>
    /// Infinite map flag.
    /// </summary>
    /// <remarks>
    /// Infinite map flag. This flag is set if the map is composed of chunks rather than a single block. The maximum tile coordinates of such a map range from -2147483648 to 2147483648,
    /// which in practical terms makes the map infinite.
    ///
    /// Tiles for non-infinite maps are loaded with <see cref="TMXMapAsset.LoadLayer"/>, for infinite maps they are loaded with <see cref="TMXMapAsset.LoadLayerChunk"/>.
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    public bool infinite
    {
        get { return internalState.mapDef.infinite; }
    }

    /// <summary>
    /// Size of the map in terms of tile count.
    /// </summary>
    /// <remarks>
    /// Size of the map in terms of tile count. If map is infinite this size is calculated from the minimum and maximum offsets between all chunks in the map.
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    public Vector2i size
    {
        get { return internalState.mapDef.size; }
    }

    /// <summary>
    /// Background color of the map
    /// </summary>
    /// <remarks>
    /// Background color of the map as defined in the Tiled editor.
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    public Color32 backgroundColor
    {
        get { return internalState.mapDef.backgroundColor; }
    }

    /// <summary>
    /// All layers in the map, keyed by their layer name.
    /// </summary>
    /// <remarks>
    /// All layers in the map, keyed by their layer name.
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    /// <seealso cref="TMXLayer"/>
    public Dictionary<string, TMXLayer> layers
    {
        get { return internalState.mapDef.layers; }
    }

    /// <summary>
    /// All object groups of the map, keyed by their names.
    /// </summary>
    /// <remarks>
    /// All object groups of the map, keyed by their names.
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    /// <seealso cref="TMXObjectGroup"/>
    public Dictionary<string, TMXObjectGroup> objectGroups
    {
        get { return internalState.mapDef.objectGroups; }
    }

    /// <summary>
    /// Custom properties of the map
    /// </summary>
    /// <remarks>
    /// Collection of all custom properties defined for the map.
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    /// <seealso cref="TMXProperties"/>
    public TMXProperties properties
    {
        get { return internalState.mapDef.properties; }
    }

    /// <summary>
    /// Load a TMX Map asset
    /// </summary>
    /// <remarks>
    /// Load a TMX Map asset which can be used to populate RetroBlit tilemap. There are various asset sources supported:
    /// <list type="bullet">
    /// <item><b>Resources</b> - Synchronously loaded TMX Map assets from a <b>Resources</b> folder. This was the only asset source supported in RetroBlit prior to 3.0.</item>
    /// <item><b>ResourcesAsync</b> - Asynchronously loaded TMX Map assets from a <b>Resources</b> folder.</item>
    /// <item><b>WWW</b> - Asynchronously loaded TMX Map assets from a URL.</item>
    /// <item><b>AddressableAssets</b> - Asynchronously loaded TMX Map assets from Unity Addressable Assets.</item>
    /// </list>
    ///
    /// If the asset is loaded via a synchronous method then <b>Load</b> will block until the loading is complete.
    /// If the asset is loaded via an asynchronous method then <b>Load</b> will immediately return and the asset loading will
    /// continue in a background thread. The status of an asynchronous loading asset can be check by looking at <see cref="RBAsset.status"/>,
    /// or by using the event system with <see cref="RBAsset.OnLoadComplete"/> to get a callback when the asset is done loading.
    ///
    /// This method only loads the TMX Map definition, but not individual map layers. To load map layers use <see cref="TMXMapAsset.LoadLayer"/> or <see cref="TMXMapAsset.LoadLayerChunk"/>.
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// TMXMapAsset mapAsset = new TMXMapAsset();
    /// TMXMapAsset.TMXLayerLoadState terrainLayerState = new TMXMapAsset.TMXLayerLoadState();
    /// bool mapSet = false;
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from Resources asynchronously. This method call will immediately return without blocking.
    ///     mapAsset.Load("main_map", RB.AssetSource.ResourcesAsync);
    /// }
    ///
    /// public void Update()
    /// {
    ///     // If the map finished loading, but the layer has not started loading yet then load it now
    ///     if (mapAsset.status == RB.AssetStatus.Ready && layerState.status == RB.AssetStatus.Invalid)
    ///     {
    ///         layerState = mapAsset.LoadLayer("Terrain", LAYER_TERRAIN, mySpriteSheet);
    ///     }
    /// }
    ///
    /// public void Render()
    /// {
    ///     // Don't draw anything until the map layer is loaded
    ///     if (layerState.status != RB.AssetStatus.Ready)
    ///     {
    ///         return;
    ///     }
    ///
    ///     RB.SpriteSheetSet(spriteMain);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    /// }
    /// </code>
    /// <param name="filename">File name to load from</param>
    /// <param name="source">Source type</param>
    /// <returns>Load status</returns>
    /// <seealso cref="TMXMapAsset.LoadLayer"/>
    /// <seealso cref="TMXMapAsset.LoadLayerChunk"/>
    /// <seealso cref="RB.Result"/>
    /// <seealso cref="RB.AssetStatus"/>
    /// <seealso cref="RB.AssetSource"/>
    public RB.AssetStatus Load(string filename, RB.AssetSource source = RB.AssetSource.Resources)
    {
        ResetInternalState();

        internalState.source = source;

        if (source == RB.AssetSource.Resources)
        {
            RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMX(filename, this);
        }
        else
        {
            if (!RetroBlitInternal.RBAssetManager.CheckSourceSupport(source))
            {
                InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
                return status;
            }

            RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXAsync(filename, this, source);
        }

        return status;
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>.
    /// </summary>
    /// <remarks>
    /// Load a TMX map layer from a standard map. For infinite maps use <see cref="TMXMapAsset.LoadLayerChunk"/> instead.
    /// There are various asset sources supported:
    /// <list type="bullet">
    /// <item><b>Resources</b> - Synchronously loaded TMX Map assets from a <b>Resources</b> folder. This was the only asset source supported in RetroBlit prior to 3.0.</item>
    /// <item><b>ResourcesAsync</b> - Asynchronously loaded TMX Map assets from a <b>Resources</b> folder.</item>
    /// <item><b>WWW</b> - Asynchronously loaded TMX Map assets from a URL.</item>
    /// <item><b>AddressableAssets</b> - Asynchronously loaded TMX Map assets from Unity Addressable Assets.</item>
    /// </list>
    ///
    /// The layer into which the layer tile data is loaded if specified by *destinationLayer*. Optionally a sub-section of a layer can be loaded by specifying
    /// *sourceRect*. The destination position of the tile data can also be specified with *destPos*.
    ///
    /// The sprite sheet or sprite pack can be specified for this layer by passing *spriteSheet* parameter, or later by <see cref="RB.MapLayerSpriteSheetSet"/>. If a sprite pack is specified
    /// then a lookup array can be passed using *packedSpriteLookup*, which will help RetroBlit translate from tilemap sprite indices to packed sprites.
    ///
    /// Besides tile sprite information <see cref="TMXMapAsset.LoadLayer"/> also loads <see cref="TMXProperties"/> for each tile. These properties can be set inside of
    /// *Tiled*. This can be very useful for defining gameplay affecting properties such as whether a particular tile is "blocking".
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <param name="tmx">TMX tilemap definition</param>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="spriteSheet">Sprite sheet to use for this layer</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    ///
    /// TMXMapAsset mapAsset = new TMXMapAsset();
    /// TMXMapAsset.TMXLayerLoadState terrainLayerState = new TMXLayerLoadState();
    /// bool mapSet = false;
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from Resources asynchronously. This method call will immediately return without blocking.
    ///     mapAsset.Load("main_map", RB.AssetSource.ResourcesAsync);
    /// }
    ///
    /// public void Update()
    /// {
    ///     // If the map finished loading, but the layer has not started loading yet then load it now
    ///     if (mapAsset.status == RB.AssetStatus.Ready && layerState.status == RB.AssetStatus.Invalid)
    ///     {
    ///         layerState = mapAsset.LoadLayer("Terrain", LAYER_TERRAIN, mySpriteSheet);
    ///     }
    /// }
    ///
    /// public void Render()
    /// {
    ///     // Don't draw anything until the map layer is loaded
    ///     if (layerState.status != RB.AssetStatus.Ready)
    ///     {
    ///         return;
    ///     }
    ///
    ///     RB.SpriteSheetSet(spriteMain);
    ///     RB.DrawMapLayer(LAYER_TERRAIN);
    /// }
    /// </code>
    /// <returns>Layer loading state</returns>
    /// <seealso cref="TMXMapAsset.Load"/>
    /// <seealso cref="TMXMapAsset.LoadLayerChunk"/>
    /// <seealso cref="RB.Result"/>
    /// <seealso cref="RB.AssetStatus"/>
    /// <seealso cref="RB.AssetSource"/>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, SpriteSheetAsset spriteSheet = null)
    {
        var ret = RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, new Rect2i(0, 0, 0x7FFFFFFF, 0x7FFFFFFF), new Vector2i(0, 0), null);
        if (spriteSheet != null)
        {
            RB.MapLayerSpriteSheetSet(destinationLayer, spriteSheet);
        }

        return ret;
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>. Can be used to load only a section of the layer.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="sourceRect">Source rectangular tilemap area to load from</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, Rect2i sourceRect, Vector2i destPos)
    {
        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, sourceRect, destPos, null);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, PackedSpriteID[] packedSpriteLookup)
    {
        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, new Rect2i(0, 0, 0x7FFFFFFF, 0x7FFFFFFF), new Vector2i(0, 0), packedSpriteLookup);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>. Can be used to load only a section of the layer.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="sourceRect">Source rectangular tilemap area to load from</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, Rect2i sourceRect, Vector2i destPos, PackedSpriteID[] packedSpriteLookup)
    {
        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, sourceRect, destPos, packedSpriteLookup);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, PackedSprite[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = packedSpriteLookup[i].id;
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, new Rect2i(0, 0, 0x7FFFFFFF, 0x7FFFFFFF), new Vector2i(0, 0), packedSpriteLookup2);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>. Can be used to load only a section of the layer.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="sourceRect">Source rectangular tilemap area to load from</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, Rect2i sourceRect, Vector2i destPos, PackedSprite[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = packedSpriteLookup[i].id;
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, sourceRect, destPos, packedSpriteLookup2);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, FastString[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = RB.PackedSpriteID(packedSpriteLookup[i]);
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, new Rect2i(0, 0, 0x7FFFFFFF, 0x7FFFFFFF), new Vector2i(0, 0), packedSpriteLookup2);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>. Can be used to load only a section of the layer.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="sourceRect">Source rectangular tilemap area to load from</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, Rect2i sourceRect, Vector2i destPos, FastString[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = RB.PackedSpriteID(packedSpriteLookup[i]);
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, sourceRect, destPos, packedSpriteLookup2);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, string[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = RB.PackedSpriteID(packedSpriteLookup[i]);
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, new Rect2i(0, 0, 0x7FFFFFFF, 0x7FFFFFFF), new Vector2i(0, 0), packedSpriteLookup2);
    }

    /// <summary>
    /// Load a layer from a TMX tilemap definition previously loaded by <see cref="TMXMapAsset.Load"/>. Can be used to load only a section of the layer.
    /// </summary>
    /// <remarks>Only valid with non-infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="sourceRect">Source rectangular tilemap area to load from</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayer(string sourceLayerName, int destinationLayer, Rect2i sourceRect, Vector2i destPos, string[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = RB.PackedSpriteID(packedSpriteLookup[i]);
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayer(this, sourceLayerName, destinationLayer, sourceRect, destPos, packedSpriteLookup2);
    }

    /// <summary>
    /// Load a single layer chunk from an infinite TMX tilemap definition loaded by <see cref="TMXMapAsset.Load"/>
    /// </summary>
    /// <remarks>
    /// Load a TMX map layer chunk from an infinite map. For standard fixed maps use <see cref="TMXMapAsset.LoadLayer"/> instead.
    /// There are various asset sources supported:
    /// <list type="bullet">
    /// <item><b>Resources</b> - Synchronously loaded TMX Map assets from a <b>Resources</b> folder. This was the only asset source supported in RetroBlit prior to 3.0.</item>
    /// <item><b>ResourcesAsync</b> - Asynchronously loaded TMX Map assets from a <b>Resources</b> folder.</item>
    /// <item><b>WWW</b> - Asynchronously loaded TMX Map assets from a URL.</item>
    /// <item><b>AddressableAssets</b> - Asynchronously loaded TMX Map assets from Unity Addressable Assets.</item>
    /// </list>
    ///
    /// The layer into which the layer chunk tile data is loaded if specified by *destinationLayer*. The source is specified by *chunkOffset*,
    /// and the destination position of the tile data is specified with *destPos*.
    ///
    /// The sprite sheet or sprite pack can be specified for this layer by passing *spriteSheet* parameter, or later by <see cref="RB.MapLayerSpriteSheetSet"/>. If a sprite pack is specified
    /// then a lookup array can be passed using *packedSpriteLookup*, which will help RetroBlit translate from tilemap sprite indices to packed sprites.
    ///
    /// Besides tile sprite information <see cref="TMXMapAsset.LoadLayerChunk"/> also loads <see cref="TMXProperties"/> for each tile. These properties can be set inside of
    /// *Tiled*. This can be very useful for defining gameplay affecting properties such as whether a particular tile is "blocking".
    /// <seedoc>Features:Tilemaps</seedoc>
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="chunkOffset">Chunk offset in the TMX layer in tile coordinates</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <code>
    /// const int LAYER_TERRAIN = 0;
    /// TMXMapAsset tmxMap = new TMXMapAsset;
    ///
    /// Vector2i cameraPos
    ///
    /// // Track the top-left-most currently loaded chunk
    /// Vector2i topLeftChunk;
    ///
    /// void Initialize() {
    ///     tmxMap.Load("tilemaps/world");
    /// }
    ///
    /// void Render() {
    ///     // Calculate the size of a single map chunk
    ///     var chunkPixelSize = new Vector2i(
    ///         RB.MapChunkSize.width * RB.SpriteSize(0).width,
    ///         RB.MapChunkSize.height * RB.SpriteSize(0).height);
    ///
    ///     // Figure out which map chunk is in the top left corner of the camera view
    ///     var newTopLeftChunk = new Vector2i(
    ///         cameraPos.x / chunkPixelSize.width,
    ///         cameraPos.y / chunkPixelSize.height);
    ///
    ///     if (newTopLeftChunk != topLeftChunk) {
    ///         // Calculate how much the chunks should be shifted
    ///         var shift = topLeftChunk - newTopLeftChunk;
    ///         RB.MapShiftChunks(0, shift);
    ///
    ///         // Iterate through all potentially visible chunks. If any are currently empty
    ///         // then load them
    ///         for (int cy = 0; cy <= (RB.DisplaySize.height / chunkPixelSize.height) + 1; cy++) {
    ///             for (int cx = 0; cx <= (RB.DisplaySize.width / chunkPixelSize.width) + 1; cx++) {
    ///                 var chunkPos = new Vector2i(cx * RB.MapChunkSize.x, cy * RB.MapChunkSize.y);
    ///                 var mapPos = new Vector2i(
    ///                     newTopLeftChunk.x * RB.MapChunkSize.x,
    ///                     newTopLeftChunk.y * RB.MapChunkSize.y) + chunkPos;
    ///                 mapPos.x = mapPos.x % tmxMap.size.width;
    ///
    ///                 if (RB.MapChunkEmpty(LAYER_TERRAIN, chunkPos)) {
    ///                     tmxMap.LoadTMXLayerChunk("Terrain", LAYER_TERRAIN, mapPos, chunkPos);
    ///                 }
    ///             }
    ///         }
    ///
    ///         topLeftChunk = newTopLeftChunk;
    ///     }
    ///
    ///     // Calculate the new camera position
    ///     var newCameraPos = new Vector2i(
    ///         cameraPos.x % chunkPixelSize.width,
    ///         cameraPos.y % chunkPixelSize.height);
    ///
    ///     cameraPos = newCameraPos;
    ///
    ///     // Update the camera position before drawing
    ///     RB.CameraSet(cameraPos);
    ///
    ///     RB.DrawMapLayer(LAYER_TERRAIN, new Vector2i(x + 1, y + 1));
    ///
    ///     RB.CameraReset();
    /// }
    /// </code>
    /// <returns>Layer loading state</returns>
    /// <seealso cref="TMXMapAsset.Load"/>
    /// <seealso cref="TMXMapAsset.LoadLayerChunk"/>
    /// <seealso cref="RB.MapChunkEmpty"/>
    /// <seealso cref="RB.MapShiftChunks"/>
    /// <seealso cref="RB.Result"/>
    /// <seealso cref="RB.AssetStatus"/>
    /// <seealso cref="RB.AssetSource"/>
    public TMXLayerLoadState LoadLayerChunk(string sourceLayerName, int destinationLayer, Vector2i chunkOffset, Vector2i destPos)
    {
        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayerChunk(this, sourceLayerName, destinationLayer, chunkOffset, destPos, null);
    }

    /// <summary>
    /// Load a single layer chunk from a TMX tilemap definition loaded by <see cref="TMXMapAsset.Load"/>. This method is used for loading infinite maps
    /// chunk by chunk.
    /// </summary>
    /// <remarks>Only valid with infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="chunkOffset">Chunk offset in the TMX layer in tile coordinates</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayerChunk(string sourceLayerName, int destinationLayer, Vector2i chunkOffset, Vector2i destPos, PackedSpriteID[] packedSpriteLookup)
    {
        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayerChunk(this, sourceLayerName, destinationLayer, chunkOffset, destPos, packedSpriteLookup);
    }

    /// <summary>
    /// Load a single layer chunk from a TMX tilemap definition loaded by <see cref="TMXMapAsset.Load"/>. This method is used for loading infinite maps
    /// chunk by chunk.
    /// </summary>
    /// <remarks>Only valid with infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="chunkOffset">Chunk offset in the TMX layer in tile coordinates</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayerChunk(string sourceLayerName, int destinationLayer, Vector2i chunkOffset, Vector2i destPos, PackedSprite[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = packedSpriteLookup[i].id;
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayerChunk(this, sourceLayerName, destinationLayer, chunkOffset, destPos, packedSpriteLookup2);
    }

    /// <summary>
    /// Load a single layer chunk from a TMX tilemap definition loaded by <see cref="TMXMapAsset.Load"/>. This method is used for loading infinite maps
    /// chunk by chunk.
    /// </summary>
    /// <remarks>Only valid with infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="chunkOffset">Chunk offset in the TMX layer in tile coordinates</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayerChunk(string sourceLayerName, int destinationLayer, Vector2i chunkOffset, Vector2i destPos, FastString[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = RB.PackedSpriteID(packedSpriteLookup[i]);
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayerChunk(this, sourceLayerName, destinationLayer, chunkOffset, destPos, packedSpriteLookup2);
    }

    /// <summary>
    /// Load a single layer chunk from a TMX tilemap definition loaded by <see cref="TMXMapAsset.Load"/>. This method is used for loading infinite maps
    /// chunk by chunk.
    /// </summary>
    /// <remarks>Only valid with infinite maps</remarks>
    /// <param name="sourceLayerName">Name of the TMX layer. Duplicate layer names are not supported</param>
    /// <param name="destinationLayer">The RetroBlit layer to load into</param>
    /// <param name="chunkOffset">Chunk offset in the TMX layer in tile coordinates</param>
    /// <param name="destPos">Destination position in the RetroBlit layer in tile coordinates</param>
    /// <param name="packedSpriteLookup">Lookup table for translating TMX tile indexes to packed sprites</param>
    /// <returns>True if successful</returns>
    public TMXLayerLoadState LoadLayerChunk(string sourceLayerName, int destinationLayer, Vector2i chunkOffset, Vector2i destPos, string[] packedSpriteLookup)
    {
        PackedSpriteID[] packedSpriteLookup2 = null;

        if (packedSpriteLookup != null)
        {
            packedSpriteLookup2 = new PackedSpriteID[packedSpriteLookup.Length];
            for (int i = 0; i < packedSpriteLookup.Length; i++)
            {
                packedSpriteLookup2[i] = RB.PackedSpriteID(packedSpriteLookup[i]);
            }
        }

        return RetroBlitInternal.RBAPI.instance.Tilemap.LoadTMXLayerChunk(this, sourceLayerName, destinationLayer, chunkOffset, destPos, packedSpriteLookup2);
    }

    /// <summary>
    /// Unload a previously loaded TMX Map
    /// </summary>
    public override void Unload()
    {
        ResetInternalState();
    }

    private void ResetInternalState()
    {
        internalState.fileName = null;
        internalState.mapDef = null;
        internalState.source = RB.AssetSource.Resources;
        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);
    }

    /// <summary>
    /// Internal state of the TMX Map asset, do not change
    /// </summary>
    public struct TMXMapAssetInternalState
    {
        /// <summary>
        /// TMX Map definition
        /// </summary>
        public RetroBlitInternal.RBTilemapTMX.TMXMapDef mapDef;

        /// <summary>
        /// Asset source, will be used when further loading layers or objects.
        /// </summary>
        public RB.AssetSource source;

        /// <summary>
        /// File name of the TMX tilemap
        /// </summary>
        public string fileName;
    }

    /// <summary>
    /// Current loading state of a TMX layer
    /// </summary>
    /// <remarks>
    /// Current loading state of a TMX layer. If the layer is being loaded asynchronously then this state can be used to check the loading progress.
    /// Once the layer is loaded it can be accessed via the <see cref="TMXLayerLoadState.map"/> member.
    /// </remarks>
    public class TMXLayerLoadState : RBAsset
    {
        /// <summary>
        /// TMX Map asset that this layer belongs to
        /// </summary>
        /// <remarks>
        /// TMX Map asset that this layer belongs to.
        /// </remarks>
        /// <seealso cref="TMXMapAsset"/>
        public TMXMapAsset map;
    }

    /// <summary>
    /// A definition of a TMX layer
    /// </summary>
    /// <remarks>
    /// A definition of a TMX layer. These layers are stored in a loaded <see cref="TMXMapAsset"/>.
    /// <seedoc>Features:Tiled TMX Support</seedoc>
    /// </remarks>
    public class TMXLayer
    {
        /// <summary>
        /// Size of layer in tiles
        /// </summary>
        protected Vector2i mSize;

        /// <summary>
        /// Pixel offset of layer
        /// </summary>
        protected Vector2i mOffset;

        /// <summary>
        /// Visibility flag of layer
        /// </summary>
        protected bool mVisible;

        /// <summary>
        /// Alpha transparency of layer
        /// </summary>
        protected byte mAlpha;

        /// <summary>
        /// Custom properties of layer
        /// </summary>
        protected TMXProperties mProperties = new TMXProperties();

        /// <summary>
        /// Size of layer in terms of tile count.
        /// </summary>
        /// <remarks>
        /// Size of layer in terms of tile count. If map is infinite this size is calculated from the minimum and maximum offsets between all chunks in the layer.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public Vector2i size
        {
            get { return mSize; }
        }

        /// <summary>
        /// Offset of the layer in pixels.
        /// </summary>
        /// <remarks>
        /// Offset of the layer in pixels. If the layer is a child of Tiled Groups then the offset of those parents is merged into this offset.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public Vector2i offset
        {
            get { return mOffset; }
        }

        /// <summary>
        /// Visibility flag.
        /// </summary>
        /// <remarks>
        /// Visibility flag of the layer. If any Tiled Group parent of this layer has visibility flag off then this visible flag will also be off.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public bool visible
        {
            get { return mVisible; }
        }

        /// <summary>
        /// Alpha transparency of the layer
        /// </summary>
        /// <remarks>
        /// Alpha transparency of the layer. The alpha values of any parent Tiled Groups are multiplied into this alpha value.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public byte alpha
        {
            get { return mAlpha; }
        }

        /// <summary>
        /// Custom properties of the layer
        /// </summary>
        /// <remarks>
        /// A collection of all the custom properties defined for this layer.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXProperties"/>
        public TMXProperties properties
        {
            get { return mProperties; }
        }
    }

    /// <summary>
    /// A definition of a TMX object
    /// </summary>
    /// <remarks>
    /// A definition of a TMX object. TMX objects are geometric shapes and can be used for a variety of things. For example a rectangular
    /// shape could be used to outline a trigger area of some event in the game.
    /// </remarks>
    public class TMXObject
    {
        /// <summary>
        /// Name of the object
        /// </summary>
        protected string mName;

        /// <summary>
        /// Type of the object
        /// </summary>
        /// <remarks>This is the type property as defined by Tiled, it is not the same as the Shape of the object</remarks>
        protected string mType;

        /// <summary>
        /// Shape of the object
        /// </summary>
        protected Shape mShape = Shape.Rectangle;

        /// <summary>
        /// Rectangular area of the object
        /// </summary>
        protected Rect2i mRect = new Rect2i();

        /// <summary>
        /// Rotation of the object in clock-wise degrees
        /// </summary>
        protected float mRotation = 0;

        /// <summary>
        /// Visible flag of the object
        /// </summary>
        protected bool mVisible = true;

        /// <summary>
        /// Points in the object
        /// </summary>
        protected List<Vector2i> mPoints = new List<Vector2i>();

        /// <summary>
        /// Custom properties of the object
        /// </summary>
        protected TMXProperties mProperties = new TMXProperties();

        /// <summary>
        /// Tile ID if this is a tile object.
        /// </summary>
        protected int mTileId;

        /// <summary>
        /// Shape of the TMX object
        /// </summary>
        /// <remarks>
        /// Shape of a TMX object.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public enum Shape
        {
            /// <summary>
            /// Rectangle shape.
            /// </summary>
            /// <remarks>
            /// Rectangular shape defined by <see cref="TMXObject.rect"/>.
            /// <seedoc>Features:Tiled TMX Support</seedoc>
            /// </remarks>
            Rectangle,

            /// <summary>
            /// Ellipse shape.
            /// </summary>
            /// <remarks>
            /// Elliptical shape defined by <see cref="TMXObject.rect"/>
            /// <seedoc>Features:Tiled TMX Support</seedoc>
            /// </remarks>
            Ellipse,

            /// <summary>
            /// Point shape.
            /// </summary>
            /// <remarks>
            /// A point defined by <see cref="TMXObject.rect"/>.x, and <see cref="TMXObject.rect"/>.y.
            /// <seedoc>Features:Tiled TMX Support</seedoc>
            /// </remarks>
            Point,

            /// <summary>
            /// Polygon shape, made of connected points where first and last points are joined.
            /// </summary>
            /// <remarks>
            /// Polygon shape, made of connected points where first and last points are joined. The points are defined by <see cref="TMXObject.points"/>.
            /// <seedoc>Features:Tiled TMX Support</seedoc>
            /// </remarks>
            Polygon,

            /// <summary>
            /// Polyline shape made of connected points where first and last points are not joined.
            /// </summary>
            /// <remarks>
            /// Polyline shape made of connected points where first and last points are not joined. The points are defined by <see cref="TMXObject.points"/>.
            /// <seedoc>Features:Tiled TMX Support</seedoc>
            /// </remarks>
            Polyline,
        }

        /// <summary>
        /// Name of the object
        /// </summary>
        /// <remarks>
        /// Name of the object.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public string name
        {
            get { return mName; }
        }

        /// <summary>
        /// Type of the object as defined in Tiled
        /// </summary>
        /// <remarks>
        /// Type of the object as defined in Tiled editor.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public string type
        {
            get { return mType; }
        }

        /// <summary>
        /// Shape of the object.
        /// </summary>
        /// <remarks>
        /// The shape of the object, one of <see cref="TMXObject.Shape"/>
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public Shape shape
        {
            get { return mShape; }
        }

        /// <summary>
        /// Rectangular bounding area of the shape.
        /// </summary>
        /// <remarks>
        /// Rectangular area defining the object. For <mref refid="TMXObject.Shape.Point">TMXObject.Shape.Point</mref> only the x and y coordinates are valid.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public Rect2i rect
        {
            get { return mRect; }
        }

        /// <summary>
        /// Rotation of the shape in degrees, clockwise
        /// </summary>
        /// <remarks>
        /// Rotation of the shape in degrees, clockwise
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public float rotation
        {
            get { return mRotation; }
        }

        /// <summary>
        /// Visible flag
        /// </summary>
        /// <remarks>
        /// The visibility flag of the object.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public bool visible
        {
            get { return mVisible; }
        }

        /// <summary>
        /// List of points that make up a <see cref="TMXObject.Shape.Polygon"/> or <see cref="TMXObject.Shape.Polyline"/>.
        /// </summary>
        /// <remarks>
        /// List of points that make up a <see cref="TMXObject.Shape.Polygon"/> or <see cref="TMXObject.Shape.Polyline"/>.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public List<Vector2i> points
        {
            get { return mPoints; }
        }

        /// <summary>
        /// Custom properties of the shape
        /// </summary>
        /// <remarks>
        /// Collection of custom properties defined for this object.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXProperties"/>
        public TMXProperties properties
        {
            get { return mProperties; }
        }

        /// <summary>
        /// TMX Object tile id
        /// </summary>
        /// <remarks>
        /// TMX Object tile id as defined in Tiled
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        public int tileId
        {
            get { return mTileId; }
        }
    }

    /// <summary>
    /// A definition of a TMX object group
    /// </summary>
    /// <remarks>
    /// A definition of a TMX object group. Object groups are loaded and stored in a <see cref="TMXMapAsset"/>.
    /// </remarks>
    public class TMXObjectGroup
    {
        /// <summary>
        /// Name of the object group
        /// </summary>
        protected string mName;

        /// <summary>
        /// Color of the objects in this group
        /// </summary>
        protected Color32 mColor;

        /// <summary>
        /// Alpha transparency of the group
        /// </summary>
        protected byte mAlpha = 255;

        /// <summary>
        /// Visible flag of the group
        /// </summary>
        protected bool mVisible = true;

        /// <summary>
        /// Pixel offset of the group
        /// </summary>
        protected Vector2i mOffset = Vector2i.zero;

        /// <summary>
        /// List of objects in the group
        /// </summary>
        protected List<TMXObject> mObjects = new List<TMXObject>();

        /// <summary>
        /// Custom properties of the group
        /// </summary>
        protected TMXProperties mProperties = new TMXProperties();

        /// <summary>
        /// Name of the object group
        /// </summary>
        /// <remarks>
        /// Name of the object group.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXObject"/>
        public string name
        {
            get { return mName; }
        }

        /// <summary>
        /// Color used for objects in this group
        /// </summary>
        /// <remarks>
        /// The color used for the objects in this group, as specified in Tiled editor.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXObject"/>
        public Color32 color
        {
            get { return mColor; }
        }

        /// <summary>
        /// Alpha transparency of the objects in this group.
        /// </summary>
        /// <remarks>
        /// The alpha transparency of the objects in this group. The alpha values of any parent Tiled Groups are multiplied into this alpha value.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXObject"/>
        public byte alpha
        {
            get { return mAlpha; }
        }

        /// <summary>
        /// Visibility flag.
        /// </summary>
        /// <remarks>
        /// The visibility flag of the objects in this group. If any Tiled Group parent of this object group has visibility flag off then this visible flag will also be off.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXObject"/>
        public bool visible
        {
            get { return mVisible; }
        }

        /// <summary>
        /// Offset of the object group in pixels.
        /// </summary>
        /// <remarks>
        /// Offset of the object group in pixels. If this object group is a child of Tiled Groups then the offset of those parents is merged into this offset.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXObject"/>
        public Vector2i offset
        {
            get { return mOffset; }
        }

        /// <summary>
        /// List of all objects in this group
        /// </summary>
        /// <remarks>
        /// List of all objects in this group.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXObject"/>
        public List<TMXObject> objects
        {
            get { return mObjects; }
        }

        /// <summary>
        /// Custom properties for this object group
        /// </summary>
        /// <remarks>
        /// A collection of all custom properties defined for this group.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <seealso cref="TMXProperties"/>
        public TMXProperties properties
        {
            get { return mProperties; }
        }
    }

    /// <summary>
    /// Contains TMX Properties and methods to access them
    /// </summary>
    /// <remarks>
    /// Contains TMX Properties and methods to access them.
    /// </remarks>
    public class TMXProperties
    {
        /// <summary>
        /// Dictionary containing all string values
        /// </summary>
        protected Dictionary<string, string> mStrings = null;

        /// <summary>
        /// Dictionary containing all boolean values
        /// </summary>
        protected Dictionary<string, bool> mBooleans = null;

        /// <summary>
        /// Dictionary containing all integer values
        /// </summary>
        protected Dictionary<string, int> mIntegers = null;

        /// <summary>
        /// Dictionary containing all floats values
        /// </summary>
        protected Dictionary<string, float> mFloats = null;

        /// <summary>
        /// Dictionary containing all color values
        /// </summary>
        protected Dictionary<string, Color32> mColors = null;

        /// <summary>
        /// Add a new property
        /// </summary>
        /// <remarks>
        /// Add a new property to this property collection.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, string value)
        {
            if (mStrings == null)
            {
                mStrings = new Dictionary<string, string>();
            }

            mStrings[key] = value;
        }

        /// <summary>
        /// Add a new boolean property
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, bool value)
        {
            if (mBooleans == null)
            {
                mBooleans = new Dictionary<string, bool>();
            }

            mBooleans[key] = value;
        }

        /// <summary>
        /// Add a new integer property
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, int value)
        {
            if (mIntegers == null)
            {
                mIntegers = new Dictionary<string, int>();
            }

            mIntegers[key] = value;
        }

        /// <summary>
        /// Add a new float property
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, float value)
        {
            if (mFloats == null)
            {
                mFloats = new Dictionary<string, float>();
            }

            mFloats[key] = value;
        }

        /// <summary>
        /// Add a new color property
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, Color32 value)
        {
            if (mColors == null)
            {
                mColors = new Dictionary<string, Color32>();
            }

            mColors[key] = value;
        }

        /// <summary>
        /// Get a string property by its key
        /// </summary>
        /// <remarks>
        /// Get a string property by its key.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">A default value to return if the key is not found</param>
        /// <returns>String value, if not found returns defaultValue</returns>
        public string GetString(string key, string defaultValue = null)
        {
            if (mStrings == null)
            {
                return defaultValue;
            }

            if (!mStrings.ContainsKey(key))
            {
                return defaultValue;
            }

            return mStrings[key];
        }

        /// <summary>
        /// Get a boolean property by its key
        /// </summary>
        /// <remarks>
        /// Get a boolean property by its key.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">A default value to return if the key is not found</param>
        /// <returns>Boolean value, if not found returns defaultValue</returns>
        public bool GetBool(string key, bool defaultValue = false)
        {
            if (mBooleans == null)
            {
                return defaultValue;
            }

            if (!mBooleans.ContainsKey(key))
            {
                return defaultValue;
            }

            return mBooleans[key];
        }

        /// <summary>
        /// Get an integer property by its key
        /// </summary>
        /// <remarks>
        /// Get an integer property by its key.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">A default value to return if the key is not found</param>
        /// <returns>Integer value, if not found returns defaultValue</returns>
        public int GetInt(string key, int defaultValue = 0)
        {
            if (mIntegers == null)
            {
                return defaultValue;
            }

            if (!mIntegers.ContainsKey(key))
            {
                return defaultValue;
            }

            return mIntegers[key];
        }

        /// <summary>
        /// Get a float property by its key
        /// </summary>
        /// <remarks>
        /// Get a float property by its key.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">A default value to return if the key is not found</param>
        /// <returns>Float value, if not found returns defaultValue</returns>
        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            if (mFloats == null)
            {
                return defaultValue;
            }

            if (!mFloats.ContainsKey(key))
            {
                return defaultValue;
            }

            return mFloats[key];
        }

        /// <summary>
        /// Get a color property by its key
        /// </summary>
        /// <remarks>
        /// Get a color property by its key.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <param name="key">Key</param>
        /// <returns>Color value, if not found returns defaultValue or <see cref="Color32.black"/> if defaultValue not provided</returns>
        public Color32 GetColor(string key)
        {
            if (mColors == null)
            {
                return Color.black;
            }

            return mColors[key];
        }

        /// <summary>
        /// Get a color property by its key
        /// </summary>
        /// <remarks>
        /// Get a color property by its key.
        /// <seedoc>Features:Tiled TMX Support</seedoc>
        /// </remarks>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">A default value to return if the key is not found</param>
        /// <returns>Color value, if not found returns defaultValue or <see cref="Color32.black"/> if defaultValue not provided</returns>
        public Color32 GetColor(string key, Color32 defaultValue)
        {
            if (mColors == null)
            {
                return defaultValue;
            }

            if (!mColors.ContainsKey(key))
            {
                return defaultValue;
            }

            return mColors[key];
        }
    }
}
