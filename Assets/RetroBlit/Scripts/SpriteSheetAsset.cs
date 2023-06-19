using System;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// Spritesheet asset
/// </summary>
/// <remarks>
/// Contains a spritesheet asset. Spritesheet assets can be loaded from various sources, including synchronous and asynchronous sources. Use <see cref="SpriteSheetAsset.Load"/> to load a spritesheet asset.
/// </remarks>
public class SpriteSheetAsset : RBAsset
{
    /// <summary>
    /// Internal state, do not change
    /// </summary>
    public SpriteSheetInternalState internalState;

    ~SpriteSheetAsset()
    {
        // Careful here, this finalizer could be called at any point, even when RetroBlit is not initialized anymore! Also must be on main thread.
        if (RetroBlitInternal.RBAPI.instance != null && RetroBlitInternal.RBAPI.instance.Audio != null && RetroBlitInternal.RBUtil.OnMainThread())
        {
            RetroBlitInternal.RBAPI.instance.AssetManager.SpriteSheetUnload(this);
        }
        else
        {
            // Otherwise queue up the resource for later release
            if (internalState.texture != null)
            {
                RetroBlitInternal.RBAPI.instance.AssetManager.TextureUnloadMainThread(internalState.texture);
            }
        }

        ResetInternalState();
    }

    /// <summary>
    /// Sprite sheet type
    /// </summary>
    public enum SheetType
    {
        /// <summary>
        /// Normal sprite sheet. You can use <see cref="SpriteGrid"/> to define sprite sheet regions to draw from.
        /// </summary>
        SpriteSheet,

        /// <summary>
        /// A sprite pack in which each sprite it referred to by its name or <see cref="PackedSpriteID"/>.
        /// </summary>
        SpritePack
    }

    /// <summary>
    /// The size of the sprite sheet
    /// </summary>
    /// <remarks>
    /// The size of the sprite sheet.
    /// </remarks>
    public Vector2i sheetSize
    {
        get
        {
            return new Vector2i(internalState.textureWidth, internalState.textureHeight);
        }
    }

    /// <summary>
    /// Current <see cref="SpriteGrid"/> set for the sprite.
    /// </summary>
    /// <remarks>
    /// Current <see cref="SpriteGrid"/> set for the sprite. This grid will be used while drawing the sprite with <see cref="RB.DrawSprite"/>
    /// calls which take sprite index as a parameter. The grid can be changed at any time.
    /// </remarks>
    public SpriteGrid grid
    {
        get
        {
            return internalState.spriteGrid;
        }

        set
        {
            internalState.spriteGrid = value;

            if (internalState.spriteGrid.region.width < 0 || internalState.spriteGrid.region.height < 0)
            {
                internalState.spriteGrid.region.width = internalState.textureWidth;
                internalState.spriteGrid.region.height = internalState.textureHeight;
            }

            if (internalState.spriteGrid.region.width > internalState.textureWidth)
            {
                internalState.spriteGrid.region.width = internalState.textureWidth;
            }

            if (internalState.spriteGrid.region.height > internalState.textureHeight)
            {
                internalState.spriteGrid.region.height = internalState.textureHeight;
            }

            // If we can't figure out the cellSize then make it 1 to avoid div/0
            if (internalState.spriteGrid.region.width < 0 || internalState.spriteGrid.region.height < 0)
            {
                internalState.spriteGrid.region.width = 1;
                internalState.spriteGrid.region.height = 1;
            }

            if (internalState.spriteGrid.cellSize.width < 0 || internalState.spriteGrid.cellSize.width < 0)
            {
                internalState.spriteGrid.cellSize.width = internalState.spriteGrid.region.width;
                internalState.spriteGrid.cellSize.height = internalState.spriteGrid.region.height;
            }

            // If we can't figure out the cellSize then make it 1 to avoid div/0
            if (internalState.spriteGrid.cellSize.width <= 0 || internalState.spriteGrid.cellSize.width <= 0)
            {
                internalState.spriteGrid.cellSize.width = 1;
                internalState.spriteGrid.cellSize.height = 1;
            }

            internalState.columns = (ushort)(internalState.spriteGrid.region.width / internalState.spriteGrid.cellSize.width);
            internalState.rows = (ushort)(internalState.spriteGrid.region.height / internalState.spriteGrid.cellSize.height);

            if (internalState.columns <= 0)
            {
                internalState.columns = 1;
            }

            if (internalState.rows <= 0)
            {
                internalState.rows = 1;
            }

            internalState.spriteGridSet = true;
        }
    }

    /// <summary>
    /// Load sprites from the given source
    /// </summary>
    /// <remarks>
    /// Load sprites which can be drawn to display or offscreen surfaces. This method is used to load both <b>SheetType.SpriteSheet</b> and <b>SheetType.SpritePack</b>.
    /// There are various asset sources supported:
    /// <list type="bullet">
    /// <item><b>Resources</b> - Synchronously loaded sprite assets from a <b>Resources</b> folder. This was the only asset source supported in RetroBlit prior to 3.0.</item>
    /// <item><b>ResourcesAsync</b> - Asynchronously loaded sprite assets from a <b>Resources</b> folder.</item>
    /// <item><b>WWW</b> - Asynchronously loaded sprite assets from a URL.</item>
    /// <item><b>AddressableAssets</b> - Asynchronously loaded sprite assets from Unity Addressable Assets.</item>
    /// <item><b>Existing Assets</b> - Synchronously loaded sprite assets from an existing Unity <b>Texture2D</b> or <b>RenderTexture</b>.</item>
    /// </list>
    ///
    /// If the asset is loaded via a synchronous method then <b>Load</b> will block until the loading is complete.
    /// If the asset is loaded via an asynchronous method then <b>Load</b> will immediately return and the asset loading will
    /// continue in a background thread. The status of an asynchronous loading asset can be check by looking at <see cref="RBAsset.status"/>,
    /// or by using the event system with <see cref="RBAsset.OnLoadComplete"/> to get a callback when the asset is done loading.
    ///
    /// <seedoc>Features:Sprites</seedoc>
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset spriteMain = new SpriteSheetAsset();
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from Resources asynchronously. This method call will immediately return without blocking.
    ///     spriteMain.Load("main_spritepack", RB.SheetType.SpritePack, RB.AssetSource.ResourcesAsync);
    /// }
    ///
    /// public void Render()
    /// {
    ///     // Don't draw anything until sprites are loaded
    ///     if (spriteMain.status != RB.AssetStatus.Ready)
    ///     {
    ///         return;
    ///     }
    ///
    ///     RB.SpriteSheetSet(spriteMain);
    ///     RB.DrawSprite("hero/walk1", playerPos);
    /// }
    /// </code>
    /// <param name="path">Path of the sprite sheet</param>
    /// <param name="sheetType">The type of sprite sheet, either <see cref="SheetType.SpriteSheet"/> or <see cref="SheetType.SpritePack"/></param>
    /// <param name="source">Source type of the asset</param>
    /// <returns>Load status</returns>
    /// <seealso cref="RB.Result"/>
    /// <seealso cref="RB.AssetStatus"/>
    /// <seealso cref="RB.AssetSource"/>
    /// <seealso cref="RB.SheetType"/>
    public RB.AssetStatus Load(string path, SheetType sheetType = SheetType.SpriteSheet, RB.AssetSource source = RB.AssetSource.Resources)
    {
        Unload();

        if (!RetroBlitInternal.RBAssetManager.CheckSourceSupport(source))
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
            return status;
        }

        RetroBlitInternal.RBAPI.instance.AssetManager.SpriteSheetLoad(this, new Vector2i(0, 0), path, null, source, sheetType);

        grid = SpriteGrid.fullSheet;

        return status;
    }

    /// <summary>
    /// Load SpriteSheet from an already existing Unity *RenderTexture* object.
    /// </summary>
    /// <param name="existingTexture">Existing texture</param>
    /// <returns>Load status</returns>
    public RB.AssetStatus Load(RenderTexture existingTexture)
    {
        Unload();

        grid = SpriteGrid.fullSheet;

        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

        if (existingTexture == null)
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
            return status;
        }

        SetExistingRenderTexture(existingTexture);
        progress = 1;

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        return status;
    }

    /// <summary>
    /// Load SpriteSheet from an already existing Unity *Texture2D* object.
    /// </summary>
    /// <remarks>
    /// Load SpriteSheet from an already existing Unity *Texture2D* object. During the loading process the *existingTexture* is copied,
    /// and the resulting SpriteSheetAsset no longer references the original *existingTexture*.
    /// </remarks>
    /// <param name="existingTexture">Existing texture</param>
    /// <returns>Load status</returns>
    public RB.AssetStatus Load(Texture2D existingTexture)
    {
        Unload();

        grid = SpriteGrid.fullSheet;

        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

        if (existingTexture == null)
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
            return status;
        }

        // First need to convert to RenderTexture
        RenderTexture newTexture;

        newTexture = new RenderTexture(existingTexture.width, existingTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        if (newTexture == null)
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
            return status;
        }

        newTexture.filterMode = existingTexture.filterMode;
        newTexture.wrapMode = existingTexture.wrapMode;
        newTexture.anisoLevel = existingTexture.anisoLevel;
        newTexture.antiAliasing = 1;
        newTexture.autoGenerateMips = false;
        newTexture.depth = 0;
        newTexture.useMipMap = false;

        newTexture.Create();

        var oldActive = RenderTexture.active;
        RenderTexture.active = newTexture;
        Graphics.Blit(existingTexture, newTexture);
        RenderTexture.active = oldActive;

        SetExistingRenderTexture(newTexture);
        progress = 1;

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        return status;
    }

    /// <summary>
    /// Create a blank SpriteSheet
    /// </summary>
    /// <remarks>
    /// Create a blank SpriteSheet. This SpriteSheet can then be used as a render target with <see cref="RB.Offscreen"/>.
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset blankSheet = new SpriteSheetAsset();
    ///
    /// public void Initialize()
    /// {
    ///     blankSheet.Create(new Vector2i(256, 256));
    /// }
    ///
    /// public void Render()
    /// {
    ///     // Draw an ellipse to the offscreen first
    ///     RB.Offscreen(blankSheet);
    ///     RB.DrawEllipse(new Vector2i(100, 50), new Vector2i(40, 20), Color.white));
    ///     RB.Onscreen();
    ///
    ///     // Now draw the offscreen to display
    ///     RB.SpriteSheetSet(blankSheet);
    ///     RB.DrawCopy(new Rect2i(0, 0, 256, 256), Vector2i.zero);
    /// }
    /// </code>
    /// <param name="spriteSheetSize">Size of the Sprite Sheet</param>
    /// <returns>Load status</returns>
    /// <seealso cref="RB.Offscreen"/>
    public RB.AssetStatus Create(Vector2i spriteSheetSize)
    {
        RetroBlitInternal.RBAPI.instance.AssetManager.SpriteSheetLoad(this, spriteSheetSize, null, null, RB.AssetSource.Resources, SheetType.SpriteSheet);
        grid = SpriteGrid.fullSheet;

        return status;
    }

    /// <summary>
    /// Unload a previously loaded Sprite Sheet
    /// </summary>
    public override void Unload()
    {
        RetroBlitInternal.RBAPI.instance.Renderer.SpriteSheetUnset(this);
        RetroBlitInternal.RBAPI.instance.AssetManager.SpriteSheetUnload(this);
        ResetInternalState();
    }

    private void SetExistingRenderTexture(RenderTexture existingTexure)
    {
        internalState.texture = existingTexure;
        internalState.textureWidth = (ushort)existingTexure.width;
        internalState.textureHeight = (ushort)existingTexure.height;

        if (!internalState.spriteGridSet)
        {
            internalState.spriteGrid = SpriteGrid.fullSheet;
        }

        internalState.columns = (ushort)(internalState.spriteGrid.region.width / internalState.spriteGrid.cellSize.width);
        internalState.rows = (ushort)(internalState.spriteGrid.region.height / internalState.spriteGrid.cellSize.height);

        if (internalState.columns < 1)
        {
            internalState.columns = 1;
        }

        if (internalState.rows < 1)
        {
            internalState.rows = 1;
        }
    }

    private void ResetInternalState()
    {
        if (internalState.texture != null)
        {
            RetroBlitInternal.RBAPI.instance.Renderer.SpriteSheetUnset(this);
            internalState.texture = null;
        }

        internalState.spriteGrid = SpriteGrid.fullSheet;
        internalState.textureWidth = 0;
        internalState.textureHeight = 0;

        // Important, set to 1 to prevent /div0 without a != 0 check in RetroBlit.cs drawing methods
        internalState.columns = 1;
        internalState.rows = 1;

        internalState.spritePack = null;

        internalState.spriteGridSet = false;
    }

    /// <summary>
    /// Internal sprite sheet state
    /// </summary>
    public struct SpriteSheetInternalState
    {
        /// <summary>
        /// The texture for the spritesheet
        /// </summary>
        public RenderTexture texture;

        /// <summary>
        /// Width of the texture for quick lookup
        /// </summary>
        public ushort textureWidth;

        /// <summary>
        /// Height of the texture for quick lookup
        /// </summary>
        public ushort textureHeight;

        /// <summary>
        /// Sprite columns in the texture
        /// </summary>
        public ushort columns;

        /// <summary>
        /// Sprite rows in the texture
        /// </summary>
        public ushort rows;

        /// <summary>
        /// Indicates if spritesheet needs clear
        /// </summary>
        public bool needsClear;

        /// <summary>
        /// Sprite pack lookup
        /// </summary>
        public RetroBlitInternal.RBRenderer.SpritePack spritePack;

        /// <summary>
        /// Is sprite grid set
        /// </summary>
        public bool spriteGridSet;

        /// <summary>
        /// Sprite grid
        /// </summary>
        public SpriteGrid spriteGrid;
    }
}
