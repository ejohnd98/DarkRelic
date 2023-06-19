namespace RetroBlitInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Internal asset management.
    /// </summary>
    public class RBAssetManager
    {
        private List<RBAudioLoader> mASyncSoundClips = new List<RBAudioLoader>();
        private List<RBSpriteSheetLoader> mASyncSpriteSheets = new List<RBSpriteSheetLoader>();
        private List<RBSpritePackLoader> mASyncSpritePacks = new List<RBSpritePackLoader>();
        private List<RBShaderLoader> mASyncShaders = new List<RBShaderLoader>();
        private RBAssetUnloader mMainThreadUnloader = new RBAssetUnloader();

        /// <summary>
        /// Check if given source type is supported
        /// </summary>
        /// <param name="source">Source type</param>
        /// <returns>True if supported, false otherwise</returns>
        public static bool CheckSourceSupport(RB.AssetSource source)
        {
            if (source == RB.AssetSource.Resources ||
                source == RB.AssetSource.ResourcesAsync ||
                source == RB.AssetSource.WWW)
            {
                return true;
            }
            else if (source == RB.AssetSource.AddressableAssets)
            {
#if ADDRESSABLES_PACKAGE_AVAILABLE
                return true;
#else
                Debug.LogError("Addressable Assets support is disabled. Please go to \"File->Build Settings->Player Settings->Other Settings->Scripting Define Symbols\"" +
                " and make sure ADDRESSABLES_PACKAGE_AVAILABLE is defined. If you have not done so already you may also need to install Addressable Assets Unity package for your project.");
                return false;
#endif
            }

            Debug.LogError(source.ToString() + " asset source is not supported!");
            return false;
        }

        /// <summary>
        /// Unload all files scheduled in main thread unloader
        /// </summary>
        public void UnloadAllMainThread()
        {
            mMainThreadUnloader.UnloadAll();
        }

        /// <summary>
        /// Schedule an audio clip to be unloaded on main thread
        /// </summary>
        /// <param name="asset">Audio clip</param>
        public void AudioClipUnloadMainThread(AudioClip asset)
        {
            mMainThreadUnloader.audioClips.Add(asset);
        }

        /// <summary>
        /// Schedule a shader to be unloaded on main thread
        /// </summary>
        /// <param name="asset">Shader</param>
        public void ShaderUnloadMainThread(RetroBlitInternal.RBRenderer.RetroBlitShader asset)
        {
            mMainThreadUnloader.materials.Add(asset);
        }

        /// <summary>
        /// Schedule a texture to be unloaded on main thread
        /// </summary>
        /// <param name="asset">Texture</param>
        public void TextureUnloadMainThread(RenderTexture asset)
        {
            mMainThreadUnloader.renderTextures.Add(asset);
        }

        /// <summary>
        /// Load a sound asset from the given location
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="asset">Asset object to load into</param>
        /// <param name="source">Source type</param>
        /// <returns>True if successful</returns>
        public bool SoundLoad(string fileName, AudioAsset asset, RB.AssetSource source)
        {
            if (asset == null)
            {
                return false;
            }

            asset.progress = 0;

            if (asset.audioClip != null)
            {
                asset.audioClip.UnloadAudioData();
            }

            // Abort any existing async load for this asset
            AbortAsyncSoundLoad(asset);

            if (fileName == null)
            {
                Debug.LogError("Audio filename is null!");
                asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return false;
            }

            fileName.Replace('\\', '/');

            var loader = new RBAudioLoader();
            loader.Load(fileName, asset, source);

            if (asset.status == RB.AssetStatus.Ready)
            {
                return true;
            }

            // Always add to async queue, even if immediately failed. This gives out consistent async method of error checking
            mASyncSoundClips.Add(loader);

            return true;
        }

        /// <summary>
        /// Unload a previously loaded audio asset
        /// </summary>
        /// <param name="asset">Audio asset</param>
        public void SoundUnload(AudioAsset asset)
        {
            if (asset == null)
            {
                return;
            }

            // Let audio know this sound is about to be unloaded so it can react as needed
            RetroBlitInternal.RBAPI.instance.Audio.SoundAssetWasUnloaded(asset);

            // Abort any existing async load for this asset
            AbortAsyncSoundLoad(asset);

            asset.InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

            if (asset.audioClip != null)
            {
                asset.audioClip.UnloadAudioData();
                asset.audioClip = null;
            }
        }

        /// <summary>
        /// Load a shader asset from given location
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="asset">Asset object to load into</param>
        /// <param name="source">Source type</param>
        /// <returns>True if successful</returns>
        public bool ShaderLoad(string fileName, ShaderAsset asset, RB.AssetSource source)
        {
            if (asset == null)
            {
                return false;
            }

            asset.progress = 0;

            if (asset.shader != null)
            {
                asset.shader = null;
            }

            // Abort any existing async load for this asset
            AbortAsyncShaderLoad(asset);

            if (fileName == null)
            {
                Debug.LogError("Shader filename is null!");
                asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                return false;
            }

            fileName.Replace('\\', '/');

            var asyncResource = new RBShaderLoader();
            asyncResource.Load(fileName, asset, source);

            if (asset.status == RB.AssetStatus.Ready)
            {
                return true;
            }

            // Always add to async queue, even if immediately failed. This gives out consistent async method of error checking
            mASyncShaders.Add(asyncResource);

            return true;
        }

        /// <summary>
        /// Unload previously loaded shader
        /// </summary>
        /// <param name="asset">Shader asset</param>
        public void ShaderUnload(ShaderAsset asset)
        {
            if (asset == null)
            {
                return;
            }

            // Abort any existing async load for this asset
            AbortAsyncShaderLoad(asset);

            asset.InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

            if (asset.shader != null)
            {
                asset.shader = null;
            }
        }

        /// <summary>
        /// Load a sprite sheet or sprite pack from the given file, or create a blank one of given size, or load from an existing texture
        /// </summary>
        /// <param name="asset">Asset object to load into</param>
        /// <param name="size">Size of new empty spritesheet if no filename is specified</param>
        /// <param name="fileName">Filename</param>
        /// <param name="texture">Existing texture to use</param>
        /// <param name="source">Source type</param>
        /// <param name="sheetType">Sheet type</param>
        /// <returns>True if successful</returns>
        public bool SpriteSheetLoad(SpriteSheetAsset asset, Vector2i size, string fileName, RenderTexture texture, RB.AssetSource source, SpriteSheetAsset.SheetType sheetType)
        {
            if (asset == null)
            {
                return false;
            }

            asset.progress = 0;

            // Release any existing asset
            SpriteSheetUnload(asset);

            asset.InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

            if (fileName != null)
            {
                fileName.Replace('\\', '/');

                if (fileName.EndsWith(".sp.rb"))
                {
                    Debug.LogError("Do not specify the .sp.rb file extension when loading a sprite pack");
                    asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                    return false;
                }
                else if ((sheetType == SpriteSheetAsset.SheetType.SpriteSheet && source != RB.AssetSource.WWW && source != RB.AssetSource.AddressableAssets) && (fileName.EndsWith(".png") || fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".gif") || fileName.EndsWith(".tif") || fileName.EndsWith(".tga") || fileName.EndsWith(".psd")))
                {
                    // Does not apply to WWW, must specify extension there to form valid http request
                    Debug.LogError("Do not specify the image file extension when loading a sprite. For example, use \"hero\", instead of \"hero.png\"");
                    asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                    return false;
                }
            }

            // Async loading implies we are not creating an empty texture, nor initializing from an existing texture
            if (source != RB.AssetSource.Resources && fileName == null)
            {
                asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
            }
            else
            {
                if (sheetType == SpriteSheetAsset.SheetType.SpritePack)
                {
                    // Abort any existing async load for this asset
                    AbortAsyncSpritePackLoad(asset);

                    var asyncSpritePackResource = new RBSpritePackLoader();
                    asyncSpritePackResource.Load(fileName, asset, source);

                    if (asset.status == RB.AssetStatus.Ready)
                    {
                        return true;
                    }

                    // Always add to async queue, even if immediately failed. This gives out consistent async method of error checking
                    mASyncSpritePacks.Add(asyncSpritePackResource);

                    return true;
                }
                else if (sheetType == SpriteSheetAsset.SheetType.SpriteSheet)
                {
                    // Abort any existing async load for this asset
                    AbortAsyncSpriteSheetLoad(asset);

                    var asyncResource = new RBSpriteSheetLoader();
                    asyncResource.Load(fileName, texture, size, asset, source);

                    if (asset.status == RB.AssetStatus.Ready)
                    {
                        return true;
                    }

                    // Always add to async queue, even if immediately failed. This gives out consistent async method of error checking
                    mASyncSpriteSheets.Add(asyncResource);

                    return true;
                }
                else
                {
                    asset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Unload previously loaded spritesheet asset
        /// </summary>
        /// <param name="asset">Spritesheet asset</param>
        /// <returns>True if successful</returns>
        public bool SpriteSheetUnload(SpriteSheetAsset asset)
        {
            if (asset == null)
            {
                return false;
            }

            if (asset.internalState.texture != null)
            {
                asset.internalState.texture.DiscardContents();
                asset.internalState.texture = null;
            }

            asset.InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

            asset.internalState.rows = 1; // Columns / Rows should always be 1 to avoid /div0
            asset.internalState.columns = 1;
            asset.internalState.needsClear = false;
            asset.internalState.spriteGrid.cellSize.width = 0;
            asset.internalState.spriteGrid.cellSize.height = 0;
            asset.internalState.textureWidth = 0;
            asset.internalState.textureHeight = 0;

            return true;
        }

        /// <summary>
        /// Update assets being loaded async
        /// </summary>
        public void UpdateAsyncResources()
        {
            for (int i = mASyncSoundClips.Count - 1; i >= 0; i--)
            {
                var asyncSoundClip = mASyncSoundClips[i];

                asyncSoundClip.Update();

                if (asyncSoundClip.audioAsset == null)
                {
                    mASyncSoundClips.RemoveAt(i);
                    continue;
                }

                if (asyncSoundClip.audioAsset.status == RB.AssetStatus.Ready)
                {
                    // If crossfading music clip is affected then set it to null
                    if (RetroBlitInternal.RBAPI.instance.Audio.previousMusicClip != null && asyncSoundClip.audioAsset == RetroBlitInternal.RBAPI.instance.Audio.previousMusicClip)
                    {
                        RetroBlitInternal.RBAPI.instance.Audio.previousMusicClip = null;
                    }

                    // If current music clip is affected then update the clip
                    if (RetroBlitInternal.RBAPI.instance.Audio.currentMusicClip != null && asyncSoundClip.audioAsset == RetroBlitInternal.RBAPI.instance.Audio.currentMusicClip)
                    {
                        var channel = RetroBlitInternal.RBAPI.instance.Audio.musicChannel;
                        if (channel.Source != null)
                        {
                            channel.Source.clip = asyncSoundClip.audioAsset.audioClip;
                            channel.Source.loop = true;
                            channel.Source.Play();
                        }
                    }
                }

                if (asyncSoundClip.audioAsset.status == RB.AssetStatus.Failed)
                {
                    Debug.LogError("Sound clip " + asyncSoundClip.path + " async load failed! Result = " + asyncSoundClip.audioAsset.error.ToString());
                }

                // If state is anything except loading then remove it
                if (asyncSoundClip.audioAsset.status != RB.AssetStatus.Loading)
                {
                    mASyncSoundClips.RemoveAt(i);
                }
            }

            for (int i = mASyncShaders.Count - 1; i >= 0; i--)
            {
                var asyncShader = mASyncShaders[i];

                asyncShader.Update();

                if (asyncShader.shaderAsset == null)
                {
                    mASyncShaders.RemoveAt(i);
                    continue;
                }

                if (asyncShader.shaderAsset.status == RB.AssetStatus.Failed)
                {
                    Debug.LogError("Shader " + asyncShader.path + " async load failed! Result = " + asyncShader.shaderAsset.error.ToString());
                }

                // If state is anything except loading then remove it
                if (asyncShader.shaderAsset.status != RB.AssetStatus.Loading)
                {
                    mASyncShaders.RemoveAt(i);
                }
            }

            for (int i = mASyncSpriteSheets.Count - 1; i >= 0; i--)
            {
                var asyncSpriteSheet = mASyncSpriteSheets[i];

                asyncSpriteSheet.Update();

                if (asyncSpriteSheet.spriteSheetAsset == null)
                {
                    mASyncSpriteSheets.RemoveAt(i);
                    continue;
                }

                if (asyncSpriteSheet.spriteSheetAsset.status == RB.AssetStatus.Failed)
                {
                    Debug.LogError("Sprite sheet " + asyncSpriteSheet.path + " async load failed! Result = " + asyncSpriteSheet.spriteSheetAsset.error.ToString());
                }

                // If state is anything except loading then remove it
                if (asyncSpriteSheet.spriteSheetAsset.status != RB.AssetStatus.Loading)
                {
                    mASyncSpriteSheets.RemoveAt(i);
                }
            }

            for (int i = mASyncSpritePacks.Count - 1; i >= 0; i--)
            {
                var asyncSpritePack = mASyncSpritePacks[i];

                asyncSpritePack.Update();

                if (asyncSpritePack.spriteSheetAsset == null)
                {
                    mASyncSpritePacks.RemoveAt(i);
                    continue;
                }

                if (asyncSpritePack.spriteSheetAsset.status == RB.AssetStatus.Failed)
                {
                    Debug.LogError("Sprite pack " + asyncSpritePack.path + " async load failed! Result = " + asyncSpritePack.spriteSheetAsset.error.ToString());
                }

                // If state is anything except loading then remove it
                if (asyncSpritePack.spriteSheetAsset.status != RB.AssetStatus.Loading)
                {
                    mASyncSpritePacks.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns a count of currently loading assets
        /// </summary>
        /// <returns>Count of loading assets</returns>
        public int LoadingAssetsCount()
        {
            int loadingAssetCount = 0;

            for (int i = mASyncSoundClips.Count - 1; i >= 0; i--)
            {
                if (mASyncSoundClips[i].audioAsset.status == RB.AssetStatus.Loading)
                {
                    loadingAssetCount++;
                }
            }

            for (int i = mASyncShaders.Count - 1; i >= 0; i--)
            {
                if (mASyncShaders[i].shaderAsset.status == RB.AssetStatus.Loading)
                {
                    loadingAssetCount++;
                }
            }

            for (int i = mASyncSpriteSheets.Count - 1; i >= 0; i--)
            {
                if (mASyncSpriteSheets[i].spriteSheetAsset.status == RB.AssetStatus.Loading)
                {
                    loadingAssetCount++;
                }
            }

            for (int i = mASyncSpritePacks.Count - 1; i >= 0; i--)
            {
                if (mASyncSpritePacks[i].spriteSheetAsset.status == RB.AssetStatus.Loading)
                {
                    loadingAssetCount++;
                }
            }

            return loadingAssetCount;
        }

        private void AbortAsyncSoundLoad(AudioAsset asset)
        {
            /* Check if any of the existing pending async assets are loading for the same slot, if so we abandon them */
            for (int i = mASyncSoundClips.Count - 1; i >= 0; i--)
            {
                if (mASyncSoundClips[i].audioAsset == asset)
                {
                    // If it was already loaded then be sure to release resources
                    if (mASyncSoundClips[i].audioAsset.status == RB.AssetStatus.Ready)
                    {
                        mASyncSoundClips[i].audioAsset.audioClip.UnloadAudioData();
                    }

                    mASyncSoundClips.RemoveAt(i);

                    // There should never be more than one
                    break;
                }
            }
        }

        private void AbortAsyncShaderLoad(ShaderAsset asset)
        {
            /* Check if any of the existing pending async assets are loading for the same slot, if so we abandon them */
            for (int i = mASyncShaders.Count - 1; i >= 0; i--)
            {
                if (mASyncShaders[i].shaderAsset == asset)
                {
                    // If it was already loaded then be sure to release resources
                    if (mASyncShaders[i].shaderAsset.status == RB.AssetStatus.Ready)
                    {
                        mASyncShaders[i].shaderAsset.shader = null;
                    }

                    mASyncShaders.RemoveAt(i);

                    // There should never be more than one
                    break;
                }
            }
        }

        private void AbortAsyncSpritePackLoad(SpriteSheetAsset asset)
        {
            /* Check if any of the existing pending async assets are loading for the same slot, if so we abandon them */
            for (int i = mASyncSpritePacks.Count - 1; i >= 0; i--)
            {
                if (mASyncSpritePacks[i].spriteSheetAsset == asset)
                {
                    // If it was already loaded then be sure to release resources
                    if (mASyncSpritePacks[i].spriteSheetAsset.status == RB.AssetStatus.Ready)
                    {
                        mASyncSpritePacks[i].spriteSheetAsset.internalState.texture.DiscardContents();
                    }

                    mASyncSpritePacks.RemoveAt(i);

                    // There should never be more than one
                    break;
                }
            }
        }

        private void AbortAsyncSpriteSheetLoad(SpriteSheetAsset asset)
        {
            /* Check if any of the existing pending async assets are loading for the same slot, if so we abandon them */
            for (int i = mASyncSpriteSheets.Count - 1; i >= 0; i--)
            {
                if (mASyncSpriteSheets[i].spriteSheetAsset == asset)
                {
                    // If it was already loaded then be sure to release resources
                    if (mASyncSpriteSheets[i].spriteSheetAsset.status == RB.AssetStatus.Ready)
                    {
                        mASyncSpriteSheets[i].spriteSheetAsset.internalState.texture.DiscardContents();
                    }

                    mASyncSpriteSheets.RemoveAt(i);

                    // There should never be more than one
                    break;
                }
            }
        }

        private class RBAssetUnloader
        {
            public List<AudioClip> audioClips = new List<AudioClip>();
            public List<RenderTexture> renderTextures = new List<RenderTexture>();
            public List<Material> materials = new List<Material>();

            public void UnloadAll()
            {
                for (int i = 0; i < audioClips.Count; i++)
                {
                    if (audioClips[i] != null)
                    {
                        audioClips[i].UnloadAudioData();
                    }
                }

                audioClips.Clear();

                for (int i = 0; i < renderTextures.Count; i++)
                {
                    renderTextures[i].DiscardContents();
                }

                renderTextures.Clear();

                for (int i = 0; i < materials.Count; i++)
                {
                    // Nothing to be done
                }

                materials.Clear();
            }
        }
    }
}