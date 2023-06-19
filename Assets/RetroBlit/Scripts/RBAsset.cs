using System;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

/// <summary>
/// RetroBlit Asset
/// </summary>
/// <remarks>
/// RetroBlit base asset class. This is an abstract class inherited from by all other asset types.
/// </remarks>
public abstract class RBAsset
{
    /// <summary>
    /// Asynchronous loading progress, ranges from 0.0 to 1.0.
    /// </summary>
    /// <remarks>
    /// Asynchronous loading progress, ranges from 0.0 to 1.0. You can check this value to track loading progress of your assets.
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset mySprites = new SpriteSheetAsset();
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from Resources asynchronously. This method call will immediately return without blocking.
    ///     mySprites.Load("sprites", SpriteSheetAsset.SheetType.SpriteSheet, RB.AssetSource.ResourcesAsync);
    ///
    ///     // Sprite grid can be set before the sprite sheet is loaded
    ///     mySprites.grid = new SpriteGrid(new Vector2i(16, 16));
    /// }
    ///
    /// public void Update()
    /// {
    ///     // If the asset is not ready yet then draw loading progress bar and return
    ///     // In a more complete example you may want to check for RB.AssetStatus.Failed as well
    ///     if (mySprites.status != RB.AssetStatus.Ready)
    ///     {
    ///         /// Draw a loading progress bar
    ///         RB.DrawRectFill(new Rect2i(4, 4, (int)(mySprites.progress * 100), 16), Color.white);
    ///         return;
    ///     }
    ///
    ///     // Sprite is loaded, use it
    ///     RB.SpriteSheetSet(mySprites);
    ///     RB.DrawSprite(0, new Vector2i(100, 100));
    /// }
    /// </code>
    public float progress;

    private RB.AssetStatus mStatus = RB.AssetStatus.Invalid;
    private RB.Result mError = RB.Result.Undefined;

    /// <summary>
    /// Event called when the asset finishes loading.
    /// </summary>
    /// <remarks>
    /// When loading an asset asynchronously this event will be called when the asset finishes loading successful, or if it fails to load.
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset mySprites = new SpriteSheetAsset();
    /// bool assetsAreReady = false;
    ///
    /// public void LoadComplete(object sender, EventArgs a)
    /// {
    ///     if (sender is SpriteSheetAsset)
    ///     {
    ///         SpriteSheetAsset asset = (SpriteSheetAsset)sender;
    ///         if (asset.status == RB.AssetStatus.Ready)
    ///         {
    ///             assetsAreReady = true;
    ///         }
    ///     }
    /// }
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from Resources asynchronously. This method call will immediately return without blocking.
    ///     mySprites.Load("sprites", SpriteSheetAsset.SheetType.SpriteSheet, RB.AssetSource.ResourcesAsync);
    ///
    ///     // Sprite grid can be set before the sprite sheet is loaded
    ///     mySprites.grid = new SpriteGrid(new Vector2i(16, 16));
    ///
    ///     // Specify the event method
    ///     mySprites.OnLoadComplete += LoadComplete;
    /// }
    ///
    /// public void Update()
    /// {
    ///     // Don't do anything until assets are loaded.
    ///     if (!assetsAreReady)
    ///     {
    ///         return;
    ///     }
    ///
    ///     RB.SpriteSheetSet(mySprites);
    ///     RB.DrawSprite(0, new Vector2i(100, 100));
    /// }
    /// </code>
    public event EventHandler OnLoadComplete;

    /// <summary>
    /// Loading status of the an asset
    /// </summary>
    /// <remarks>
    /// Loading status of an asset. When the status is *Ready* the asset is ready to use.
    /// If loading fails the status will be *Failed* and <see cref="RBAsset.error"/> can be checked for more
    /// failure information.
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset mySprites = new SpriteSheetAsset();
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from WWW. This method call will immediately return without blocking.
    ///     mySprites.Load("https://mygame.com/sprites.png", SpriteSheetAsset.SheetType.SpriteSheet, RB.AssetSource.WWW);
    ///
    ///     // Sprite grid can be set before the sprite sheet is loaded
    ///     mySprites.grid = new SpriteGrid(new Vector2i(16, 16));
    /// }
    ///
    /// public void Update()
    /// {
    ///     if (mySprites.status == RB.AssetStatus.Failed)
    ///     {
    ///         if (mySprites.error = RB.Result.NetworkError) {
    ///             RB.Print(new Vector2i(4, 4), Color.white, "A network occured error while loading assets!");
    ///         }
    ///
    ///         return;
    ///     }
    ///
    ///     if (mySprites.status != RB.AssetStatus.Ready)
    ///     {
    ///         return;
    ///     }
    ///
    ///     // Sprite is loaded, use it
    ///     RB.SpriteSheetSet(mySprites);
    ///     RB.DrawSprite(0, new Vector2i(100, 100));
    /// }
    /// </code>
    public RB.AssetStatus status
    {
        get
        {
            return mStatus;
        }
    }

    /// <summary>
    /// Loading error of the audio asset
    /// </summary>
    /// <remarks>
    /// If an asset fails to load then it's <see cref="RBAsset.status"/> will be *Failed*, and this error will
    /// contain one of the possible error values in the enum <see cref="RB.Result"/>.
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset mySprites = new SpriteSheetAsset();
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from WWW. This method call will immediately return without blocking.
    ///     mySprites.Load("https://mygame.com/sprites.png", SpriteSheetAsset.SheetType.SpriteSheet, RB.AssetSource.WWW);
    ///
    ///     // Sprite grid can be set before the sprite sheet is loaded
    ///     mySprites.grid = new SpriteGrid(new Vector2i(16, 16));
    /// }
    ///
    /// public void Update()
    /// {
    ///     if (mySprites.status == RB.AssetStatus.Failed)
    ///     {
    ///         if (mySprites.error = RB.Result.NetworkError) {
    ///             RB.Print(new Vector2i(4, 4), Color.white, "A network occured error while loading assets!");
    ///         }
    ///
    ///         return;
    ///     }
    ///
    ///     if (mySprites.status != RB.AssetStatus.Ready)
    ///     {
    ///         return;
    ///     }
    ///
    ///     // Sprite is loaded, use it
    ///     RB.SpriteSheetSet(mySprites);
    ///     RB.DrawSprite(0, new Vector2i(100, 100));
    /// }
    /// </code>
    public RB.Result error
    {
        get
        {
            return mError;
        }
    }

    /// <summary>
    /// Unload previously loaded asset
    /// </summary>
    /// <remarks>
    /// Unload previously loaded asset and release any system resources it may be using. After an asset is unloaded
    /// it's <b>status</b> becomes <see cref="RB.AssetStatus.Invalid"/>.
    /// </remarks>
    /// <code>
    /// SpriteSheetAsset mySprites = new SpriteSheetAsset();
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from WWW. This method call will immediately return without blocking.
    ///     mySprites.Load("sprites.png", SpriteSheetAsset.SheetType.SpriteSheet, RB.AssetSource.ResourcesAsync);
    ///
    ///     // Sprite grid can be set before the sprite sheet is loaded
    ///     mySprites.grid = new SpriteGrid(new Vector2i(16, 16));
    /// }
    ///
    /// public void Update()
    /// {
    ///     // Unload the asset if any key is pressed
    ///     if (RB.AnyKeyDown())
    ///     {
    ///         mySprites.Unload();
    ///     }
    ///
    ///     if (mySprites.status != RB.AssetStatus.Ready)
    ///     {
    ///         return;
    ///     }
    ///
    ///     // Sprite is loaded, use it
    ///     RB.SpriteSheetSet(mySprites);
    ///     RB.DrawSprite(0, new Vector2i(100, 100));
    /// }
    /// </code>
    public virtual void Unload()
    {
        // Do nothing
    }

    /// <summary>
    /// Set the status and error of the asset, this is used internally by RetroBlit during asset loading.
    /// </summary>
    /// <param name="newStatus">New status code</param>
    /// <param name="newError">New error code</param>
    public void InternalSetErrorStatus(RB.AssetStatus newStatus, RB.Result newError)
    {
        if ((newStatus == RB.AssetStatus.Ready || newStatus == RB.AssetStatus.Failed) &&
            (mStatus != RB.AssetStatus.Ready && mStatus != RB.AssetStatus.Failed))
        {
            mStatus = newStatus;
            mError = newError;
            if (OnLoadComplete != null)
            {
                OnLoadComplete.Invoke(this, null);
            }
        }

        mStatus = newStatus;
        mError = newError;
    }
}
