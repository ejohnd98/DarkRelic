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
    /// Internal sprite sheet loading class
    /// </summary>
    public sealed class RBSpriteSheetLoader
    {
        /// <summary>
        /// Path of the asset
        /// </summary>
        public string path;

        /// <summary>
        /// Sprite sheet asset to load into
        /// </summary>
        public SpriteSheetAsset spriteSheetAsset;

        private ResourceRequest mResourceRequest = null;
        private UnityWebRequest mWebRequest = null;

#if ADDRESSABLES_PACKAGE_AVAILABLE
        private AsyncOperationHandle<Texture2D> mAddressableRequest;
#endif

        /// <summary>
        /// Check if given image format is supported
        /// </summary>
        /// <param name="path">Path to asset</param>
        /// <returns>True if supported</returns>
        public static bool ImageTypeSupported(string path)
        {
            if (path.EndsWith(".png") ||
                path.EndsWith(".jpg") ||
                path.EndsWith(".jpeg"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update asynchronous asset loading
        /// </summary>
        public void Update()
        {
            // If not loading then there is nothing to update
            if (spriteSheetAsset == null || spriteSheetAsset.status != RB.AssetStatus.Loading)
            {
                return;
            }

            if (mResourceRequest != null)
            {
                if (mResourceRequest.isDone)
                {
                    if (mResourceRequest.asset == null)
                    {
                        spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }
                    else
                    {
                        var spritesTextureOriginal = (Texture2D)mResourceRequest.asset;
                        FinalizeTexture(spritesTextureOriginal);
                    }
                }
                else
                {
                    spriteSheetAsset.progress = Mathf.Clamp01(mResourceRequest.progress);
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
                            spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NetworkError);
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

                            spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, resultError);
                        }
                        else
                        {
                            var spritesTextureOriginal = DownloadHandlerTexture.GetContent(mWebRequest);
                            if (spritesTextureOriginal == null)
                            {
                                spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                                return;
                            }

                            FinalizeTexture(spritesTextureOriginal);
                        }
                    }
                    else
                    {
                        spriteSheetAsset.progress = Mathf.Clamp01(mWebRequest.downloadProgress);
                    }
                }
                catch (Exception)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mAddressableRequest.IsValid())
            {
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    // Can't really figure out failure reason
                    Addressables.Release(mAddressableRequest);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return;
                }
                else if (mAddressableRequest.Status == AsyncOperationStatus.Succeeded)
                {
                    spriteSheetAsset.progress = 1;

                    var spritesTextureOriginal = mAddressableRequest.Result;
                    FinalizeTexture(spritesTextureOriginal);

                    // Safe to realease here, FinalizeTexture makes a copy of the asset into a RenderTexture
                    Addressables.Release(mAddressableRequest);

                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

                    return;
                }

                if (!mAddressableRequest.IsDone)
                {
                    spriteSheetAsset.progress = mAddressableRequest.PercentComplete;
                    return;
                }
            }
#endif
        }

        /// <summary>
        /// Load sprite sheet asset
        /// </summary>
        /// <param name="path">Path to asset</param>
        /// <param name="existingTexture">Existing texture to load from</param>
        /// <param name="size">Size of the texture</param>
        /// <param name="asset">SpriteSheetAsset to load into</param>
        /// <param name="source">Asset source type</param>
        /// <returns>True if successful</returns>
        public bool Load(string path, RenderTexture existingTexture, Vector2i size, SpriteSheetAsset asset, RB.AssetSource source)
        {
            this.spriteSheetAsset = asset;
            this.path = path;

            if (asset == null)
            {
                Debug.LogError("SpriteSheetAsset is null!");
                return false;
            }

            if (source == RB.AssetSource.Resources)
            {
                // Empty texture
                if (path == null && existingTexture == null)
                {
                    if (size.x <= 0 || size.y <= 0)
                    {
                        spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                        return false;
                    }

                    RenderTexture tex = new RenderTexture(size.x, size.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                    if (tex == null)
                    {
                        spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                        return false;
                    }

                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.anisoLevel = 0;
                    tex.antiAliasing = 1;

                    tex.autoGenerateMips = false;
                    tex.depth = 0;
                    tex.useMipMap = false;

                    tex.Create();

                    asset.internalState.texture = tex;
                    asset.internalState.textureWidth = (ushort)tex.width;
                    asset.internalState.textureHeight = (ushort)tex.height;
                    asset.internalState.spriteGrid.cellSize.width = (ushort)tex.width;
                    asset.internalState.spriteGrid.cellSize.height = (ushort)tex.height;
                    asset.internalState.columns = (ushort)(asset.internalState.textureWidth / asset.internalState.spriteGrid.cellSize.width);
                    asset.internalState.rows = (ushort)(asset.internalState.textureHeight / asset.internalState.spriteGrid.cellSize.height);
                    asset.internalState.needsClear = true;

                    if (asset.internalState.columns < 1)
                    {
                        asset.internalState.columns = 1;
                    }

                    if (asset.internalState.rows < 1)
                    {
                        asset.internalState.rows = 1;
                    }

                    // If there is no spritesheet set then set this one as the current one
                    if (RetroBlitInternal.RBAPI.instance.Renderer.CurrentSpriteSheet == null)
                    {
                        RetroBlitInternal.RBAPI.instance.Renderer.SpriteSheetSet(asset);
                    }

                    asset.progress = 1;
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

                    return true;
                }
                else if (existingTexture != null)
                {
                    asset.internalState.texture = existingTexture;
                    asset.internalState.textureWidth = (ushort)existingTexture.width;
                    asset.internalState.textureHeight = (ushort)existingTexture.height;
                    asset.internalState.spriteGrid.cellSize.width = (ushort)existingTexture.width;
                    asset.internalState.spriteGrid.cellSize.height = (ushort)existingTexture.height;
                    asset.internalState.columns = (ushort)(asset.internalState.textureWidth / asset.internalState.spriteGrid.cellSize.width);
                    asset.internalState.rows = (ushort)(asset.internalState.textureHeight / asset.internalState.spriteGrid.cellSize.height);

                    if (asset.internalState.columns < 1)
                    {
                        asset.internalState.columns = 1;
                    }

                    if (asset.internalState.rows < 1)
                    {
                        asset.internalState.rows = 1;
                    }

                    // If there is no spritesheet set then set this one as the current one
                    if (RetroBlitInternal.RBAPI.instance.Renderer.CurrentSpriteSheet == null)
                    {
                        RetroBlitInternal.RBAPI.instance.Renderer.SpriteSheetSet(asset);
                    }

                    asset.progress = 1;
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

                    return true;
                }
                else
                {
                    // Synchronous load
                    var spritesTextureOriginal = Resources.Load<Texture2D>(path);
                    if (spritesTextureOriginal == null)
                    {
                        Debug.LogError("Could not load sprite sheet from " + path + ", make sure the resource is placed somehwere in Assets/Resources folder. " +
                            "If you're trying to load a Sprite Pack then please specify \"SpriteSheetAsset.SheetType.SpritePack\" as \"sheetType\". " +
                            "If you're trying to load from an WWW address, or Addressable Assets then please specify so with the \"source\" parameter.");
                        asset.internalState.texture = null;
                        spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);

                        return false;
                    }

                    FinalizeTexture(spritesTextureOriginal);

                    return asset.status == RB.AssetStatus.Ready ? true : false;
                }
            }
            else if (source == RB.AssetSource.WWW)
            {
                if (!ImageTypeSupported(path))
                {
                    Debug.LogError("WWW source supports only PNG and JPG images");
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
                    return false;
                }

                mWebRequest = UnityWebRequestTexture.GetTexture(path, true);
                if (mWebRequest == null)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                if (mWebRequest.SendWebRequest() == null)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
            else if (source == RB.AssetSource.ResourcesAsync)
            {
                mResourceRequest = Resources.LoadAsync<Texture2D>(this.path);

                if (mResourceRequest == null)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);

                    return false;
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (source == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequest = Addressables.LoadAssetAsync<Texture2D>(path);
                }
                catch (UnityEngine.AddressableAssets.InvalidKeyException e)
                {
                    RBUtil.Unused(e);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }
                catch (Exception e)
                {
                    RBUtil.Unused(e);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                // Check for an immediate failure
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(mAddressableRequest);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                spriteSheetAsset.progress = 0;
                spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
#endif

            spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

            return true;
        }

        /// <summary>
        /// Abort asynchronous loading
        /// </summary>
        public void Abort()
        {
            if (spriteSheetAsset == null)
            {
                return;
            }

            if (spriteSheetAsset.status != RB.AssetStatus.Loading)
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
            mAddressableRequest = new AsyncOperationHandle<Texture2D>();
#endif
        }

        private void FinalizeTexture(Texture2D spritesTextureOriginal)
        {
            RenderTexture newTexture;

            newTexture = new RenderTexture(spritesTextureOriginal.width, spritesTextureOriginal.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            if (newTexture == null)
            {
                spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                return;
            }

            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.anisoLevel = 0;
            newTexture.antiAliasing = 1;
            newTexture.autoGenerateMips = false;
            newTexture.depth = 0;
            newTexture.useMipMap = false;

            newTexture.Create();

            if (spritesTextureOriginal != null)
            {
                var oldActive = RenderTexture.active;
                RenderTexture.active = newTexture;
                Graphics.Blit(spritesTextureOriginal, newTexture);
                RenderTexture.active = oldActive;
            }

            spriteSheetAsset.internalState.texture = newTexture;
            spriteSheetAsset.internalState.textureWidth = (ushort)newTexture.width;
            spriteSheetAsset.internalState.textureHeight = (ushort)newTexture.height;

            if (!spriteSheetAsset.internalState.spriteGridSet)
            {
                spriteSheetAsset.internalState.spriteGrid.cellSize.width = (ushort)newTexture.width;
                spriteSheetAsset.internalState.spriteGrid.cellSize.height = (ushort)newTexture.height;
            }

            spriteSheetAsset.internalState.columns = (ushort)(spriteSheetAsset.internalState.textureWidth / spriteSheetAsset.internalState.spriteGrid.cellSize.width);
            spriteSheetAsset.internalState.rows = (ushort)(spriteSheetAsset.internalState.textureHeight / spriteSheetAsset.internalState.spriteGrid.cellSize.height);

            if (spriteSheetAsset.internalState.columns < 1)
            {
                spriteSheetAsset.internalState.columns = 1;
            }

            if (spriteSheetAsset.internalState.rows < 1)
            {
                spriteSheetAsset.internalState.rows = 1;
            }

            spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
        }
    }
}
