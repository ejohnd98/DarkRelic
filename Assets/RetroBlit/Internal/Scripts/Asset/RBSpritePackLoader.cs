namespace RetroBlitInternal
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
#if ADDRESSABLES_PACKAGE_AVAILABLE
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
#endif

    /// <summary>
    /// Internal sprite pack loading class
    /// </summary>
    public sealed class RBSpritePackLoader
    {
        /// <summary>
        /// Path of the asset
        /// </summary>
        public string path;

        /// <summary>
        /// Sprite sheet asset to load into
        /// </summary>
        public SpriteSheetAsset spriteSheetAsset;

        private const float INFO_LOAD_WORTH = 0.1f;

        private RB.AssetSource mSource = RB.AssetSource.ResourcesAsync;

        private ResourceRequest mResourceRequest = null;
        private UnityWebRequest mWebRequest = null;

#if ADDRESSABLES_PACKAGE_AVAILABLE
        private AsyncOperationHandle<Texture2D> mAddressableRequestTexture;
        private AsyncOperationHandle<TextAsset> mAddressableRequestInfo;
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        public RBSpritePackLoader()
        {
        }

        /// <summary>
        /// Update asynchronous loading
        /// </summary>
        public void Update()
        {
            // If not loading then there is nothing to update
            if (spriteSheetAsset == null || spriteSheetAsset.status != RB.AssetStatus.Loading)
            {
                return;
            }

            if (spriteSheetAsset.internalState.spritePack == null)
            {
                UpdateSpritePackInfo();
            }
            else
            {
                UpdateSpriteSheet();
            }
        }

        /// <summary>
        /// Load sprite sheet asset
        /// </summary>
        /// <param name="path">Path to load from</param>
        /// <param name="asset">SpriteSheetAsset to load into</param>
        /// <param name="source">Source type</param>
        /// <returns>True if successful</returns>
        public bool Load(string path, SpriteSheetAsset asset, RB.AssetSource source)
        {
            this.spriteSheetAsset = asset;
            this.path = path;
            mSource = source;

            if (asset == null)
            {
                Debug.LogError("SpriteSheetAsset is null!");
                return false;
            }

            LoadSpritePackInfo();

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
            Addressables.Release(mAddressableRequestInfo);
            mAddressableRequestInfo = new AsyncOperationHandle<TextAsset>();

            Addressables.Release(mAddressableRequestTexture);
            mAddressableRequestTexture = new AsyncOperationHandle<Texture2D>();
#endif
        }

        private bool LoadSpritePackInfo()
        {
            string infoPath;

            if (mSource == RB.AssetSource.WWW)
            {
                infoPath = path + ".sp.rb/info.bytes";
            }
            else if (mSource == RB.AssetSource.AddressableAssets)
            {
                infoPath = path + ".sp.rb/info.bytes";
            }
            else
            {
                infoPath = path + ".sp.rb/info";
            }

            if (mSource == RB.AssetSource.Resources)
            {
                // Synchronous load
                var infoFile = Resources.Load<TextAsset>(infoPath);
                if (infoFile == null)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }

                if (FinalizeSpritePackInfo(infoFile.bytes))
                {
                    LoadSpriteSheet();
                    return spriteSheetAsset.status == RB.AssetStatus.Ready ? true : false;
                }
            }

            if (mSource == RB.AssetSource.WWW)
            {
                mWebRequest = UnityWebRequest.Get(infoPath);
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
            else if (mSource == RB.AssetSource.ResourcesAsync)
            {
                mResourceRequest = Resources.LoadAsync<TextAsset>(infoPath);

                if (mResourceRequest == null)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mSource == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequestInfo = Addressables.LoadAssetAsync<TextAsset>(infoPath);
                }
                catch (UnityEngine.AddressableAssets.InvalidKeyException e)
                {
                    RBUtil.Unused(e);
                    Addressables.Release(mAddressableRequestInfo);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }
                catch (Exception e)
                {
                    RBUtil.Unused(e);
                    Addressables.Release(mAddressableRequestInfo);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                // Check for an immediate failure
                if (mAddressableRequestInfo.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(mAddressableRequestInfo);
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

        private bool LoadSpriteSheet()
        {
            string spriteSheetPath;

            if (mSource == RB.AssetSource.WWW)
            {
                spriteSheetPath = path + ".sp.rb/spritepack.png";
            }
            else if (mSource == RB.AssetSource.AddressableAssets)
            {
                spriteSheetPath = path + ".sp.rb/spritepack.png";
            }
            else
            {
                spriteSheetPath = path + ".sp.rb/spritepack";
            }

            if (mSource == RB.AssetSource.Resources)
            {
                // Synchronous load
                var spritesTextureOriginal = Resources.Load<Texture2D>(spriteSheetPath);
                if (spritesTextureOriginal == null)
                {
                    Debug.LogError("Could not load sprite pack from " + path + ", make sure the resource is placed somehwere in Assets/Resources folder. " +
                        "If you're trying to load a Sprite Pack then please specify \"SpriteSheetAsset.SheetType.SpritePack\" as \"sheetType\". " +
                        "If you're trying to load from an WWW address, or Addressable Assets then please specify so with the \"source\" parameter.");
                    spriteSheetAsset.internalState.texture = null;

                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);

                    return false;
                }

                FinalizeTexture(spritesTextureOriginal);

                return spriteSheetAsset.status == RB.AssetStatus.Ready ? true : false;
            }
            else if (mSource == RB.AssetSource.WWW)
            {
                mWebRequest = UnityWebRequestTexture.GetTexture(spriteSheetPath, true);
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
            else if (mSource == RB.AssetSource.ResourcesAsync)
            {
                mResourceRequest = Resources.LoadAsync<Texture2D>(spriteSheetPath);

                if (mResourceRequest == null)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mSource == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequestTexture = Addressables.LoadAssetAsync<Texture2D>(spriteSheetPath);
                }
                catch (UnityEngine.AddressableAssets.InvalidKeyException e)
                {
                    RBUtil.Unused(e);
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
                if (mAddressableRequestTexture.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(mAddressableRequestTexture);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

                return true;
            }
#endif

            spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);

            return true;
        }

        private void UpdateSpritePackInfo()
        {
            byte[] loadedBytes = null;

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
                        var textAsset = (TextAsset)mResourceRequest.asset;
                        spriteSheetAsset.progress = INFO_LOAD_WORTH;
                        loadedBytes = textAsset.bytes;
                    }
                }
                else
                {
                    spriteSheetAsset.progress = Mathf.Clamp01(mResourceRequest.progress) * INFO_LOAD_WORTH;
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
                            spriteSheetAsset.progress = INFO_LOAD_WORTH;
                            loadedBytes = mWebRequest.downloadHandler.data;
                        }
                    }
                    else
                    {
                        spriteSheetAsset.progress = Mathf.Clamp01(mWebRequest.downloadProgress) * INFO_LOAD_WORTH;
                    }
                }
                catch (Exception)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mAddressableRequestInfo.IsValid())
            {
                if (mAddressableRequestInfo.Status == AsyncOperationStatus.Failed)
                {
                    // Can't really figure out failure reason
                    Addressables.Release(mAddressableRequestInfo);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return;
                }
                else if (mAddressableRequestInfo.Status == AsyncOperationStatus.Succeeded)
                {
                    spriteSheetAsset.progress = INFO_LOAD_WORTH;

                    var textAsset = mAddressableRequestInfo.Result;
                    if (textAsset != null)
                    {
                        loadedBytes = textAsset.bytes;
                        // Do not release mAddressableRequestInfo yet, wait until loadedBytes is processed below
                    }
                    else
                    {
                        // Can't really figure out failure reason
                        Addressables.Release(mAddressableRequestInfo);
                        spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }
                }

                if (!mAddressableRequestInfo.IsDone)
                {
                    spriteSheetAsset.progress = mAddressableRequestInfo.PercentComplete * INFO_LOAD_WORTH;
                    return;
                }
            }
#endif

            if (loadedBytes != null)
            {
                if (FinalizeSpritePackInfo(loadedBytes))
                {
#if ADDRESSABLES_PACKAGE_AVAILABLE
                    if (mAddressableRequestInfo.IsValid())
                    {
                        Addressables.Release(mAddressableRequestInfo);
                    }
#endif
                    LoadSpriteSheet();
                }
                else
                {
#if ADDRESSABLES_PACKAGE_AVAILABLE
                    if (mAddressableRequestInfo.IsValid())
                    {
                        Addressables.Release(mAddressableRequestInfo);
                    }
#endif
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadFormat);
                }
            }
        }

        private void UpdateSpriteSheet()
        {
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
                        spriteSheetAsset.progress = 1;
                        FinalizeTexture(spritesTextureOriginal);
                    }
                }
                else
                {
                    spriteSheetAsset.progress = (Mathf.Clamp01(mResourceRequest.progress) * (1.0f - INFO_LOAD_WORTH)) + INFO_LOAD_WORTH;
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
                            spriteSheetAsset.progress = 1;
                            FinalizeTexture(spritesTextureOriginal);
                        }
                    }
                    else
                    {
                        spriteSheetAsset.progress = (Mathf.Clamp01(mWebRequest.downloadProgress) * (1.0f - INFO_LOAD_WORTH)) + INFO_LOAD_WORTH;
                    }
                }
                catch (Exception)
                {
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mAddressableRequestTexture.IsValid())
            {
                if (mAddressableRequestTexture.Status == AsyncOperationStatus.Failed)
                {
                    // Can't really figure out failure reason
                    Addressables.Release(mAddressableRequestTexture);
                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return;
                }
                else if (mAddressableRequestTexture.Status == AsyncOperationStatus.Succeeded)
                {
                    spriteSheetAsset.progress = 1;

                    var spritesTextureOriginal = mAddressableRequestTexture.Result;
                    FinalizeTexture(spritesTextureOriginal);

                    // Safe to realease here, FinalizeTexture makes a copy of the asset into a RenderTexture
                    Addressables.Release(mAddressableRequestTexture);

                    spriteSheetAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                }

                if (!mAddressableRequestTexture.IsDone)
                {
                    spriteSheetAsset.progress = mAddressableRequestTexture.PercentComplete * (1.0f - INFO_LOAD_WORTH) + INFO_LOAD_WORTH;
                }
            }
#endif
        }

        private bool FinalizeSpritePackInfo(byte[] bytes)
        {
            var spritePack = new RBRenderer.SpritePack();
            spritePack.sprites = new Dictionary<int, PackedSprite>();

            try
            {
                var reader = new System.IO.BinaryReader(new System.IO.MemoryStream(bytes));

                if (reader.ReadUInt16() != RBRenderer.RetroBlit_SP_MAGIC)
                {
                    Debug.Log("Sprite pack index file " + path + " is invalid!");
                    return false;
                }

                if (reader.ReadUInt16() != RBRenderer.RetroBlit_SP_VERSION)
                {
                    Debug.Log("Sprite pack file " + path + " version is not supported!");
                    return false;
                }

                int spriteCount = reader.ReadInt32();
                if (spriteCount < 0 || spriteCount > 200000)
                {
                    Debug.Log("Sprite pack sprite count is invalid! Please try reimporting" + path + ".rb");
                    return false;
                }

                for (int i = 0; i < spriteCount; i++)
                {
                    int hash = reader.ReadInt32();

                    var size = new Vector2i();
                    size.x = (int)reader.ReadUInt16();
                    size.y = (int)reader.ReadUInt16();

                    var sourceRect = new Rect2i();
                    sourceRect.x = (int)reader.ReadUInt16();
                    sourceRect.y = (int)reader.ReadUInt16();
                    sourceRect.width = (int)reader.ReadUInt16();
                    sourceRect.height = (int)reader.ReadUInt16();

                    var trimOffset = new Vector2i();
                    trimOffset.x = (int)reader.ReadUInt16();
                    trimOffset.y = (int)reader.ReadUInt16();

                    var packedSprite = new PackedSprite(new PackedSpriteID(hash), size, sourceRect, trimOffset);

                    spritePack.sprites.Add(hash, packedSprite);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Sprite pack index file " + path + " is invalid! Exception: " + e.ToString());
                return false;
            }

            spriteSheetAsset.internalState.spritePack = spritePack;

            return true;
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
