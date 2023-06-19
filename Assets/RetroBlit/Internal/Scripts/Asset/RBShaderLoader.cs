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
    /// Internal shader loading class
    /// </summary>
    public sealed class RBShaderLoader
    {
        /// <summary>
        /// Path to the asset
        /// </summary>
        public string path;

        /// <summary>
        /// Shader asset to load into
        /// </summary>
        public ShaderAsset shaderAsset;

        private ResourceRequest mResourceRequest = null;

        //// Note, WWW not supported for shaders, don't need  UnityWebRequest here

#if ADDRESSABLES_PACKAGE_AVAILABLE
        private AsyncOperationHandle<Shader> mAddressableRequest;
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        public RBShaderLoader()
        {
        }

        /// <summary>
        /// Update asynchronous loading
        /// </summary>
        public void Update()
        {
            // If not loading then there is nothing to update
            if (shaderAsset == null || shaderAsset.status != RB.AssetStatus.Loading)
            {
                return;
            }

            if (mResourceRequest != null)
            {
                if (mResourceRequest.isDone)
                {
                    if (mResourceRequest.asset == null)
                    {
                        Debug.LogError("Could not load shader from " + path + ", make sure the resource is placed somehwere in Assets/Resources folder");
                        shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    }
                    else
                    {
                        var shader = (Shader)mResourceRequest.asset;
                        if (shader == null)
                        {
                            Debug.LogError("Could not load shader from " + path + ", make sure the resource is placed somehwere in Assets/Resources folder");
                            shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                            return;
                        }

                        FinalizeShader(shader);
                    }
                }
                else
                {
                    shaderAsset.progress = mResourceRequest.progress;
                }
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (mAddressableRequest.IsValid())
            {
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    // Can't really figure out failure reason
                    Addressables.Release(mAddressableRequest);
                    shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return;
                }
                else if (mAddressableRequest.Status == AsyncOperationStatus.Succeeded)
                {
                    shaderAsset.progress = 1;
                    var shader = mAddressableRequest.Result;

                    if (shader == null)
                    {
                        Debug.Log("Could not load shader from " + path);
                        Addressables.Release(mAddressableRequest);
                        shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    }
                    else
                    {
                        FinalizeShader(shader);
                        shaderAsset.addressableHandle = mAddressableRequest;
                        shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);
                    }

                    return;
                }

                if (!mAddressableRequest.IsDone)
                {
                    shaderAsset.progress = mAddressableRequest.PercentComplete;
                    return;
                }
            }
#endif
            // WWW not supported for shaders
        }

        /// <summary>
        /// Load shader asset
        /// </summary>
        /// <param name="path">Path to load from</param>
        /// <param name="asset">ShaderAsset to load into</param>
        /// <param name="source">Source type</param>
        /// <returns>True if successful</returns>
        public bool Load(string path, ShaderAsset asset, RB.AssetSource source)
        {
            shaderAsset = asset;
            this.path = path;

            if (path == null)
            {
                shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
            }

            if (asset == null)
            {
                Debug.LogError("ShaderAsset is null!");
                return false;
            }

            if (source == RB.AssetSource.Resources)
            {
                // Synchronous load
                if (path == null)
                {
                    Debug.LogError("Shader filename is null!");
                    shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
                    return false;
                }

                var shader = Resources.Load<Shader>(path);

                if (shader == null)
                {
                    Debug.LogError("Could not load shader from " + path + ", make sure the resource is placed somehwere in Assets/Resources folder");
                    shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }

                return FinalizeShader(shader);
            }
            else if (source == RB.AssetSource.WWW)
            {
                // Not a supported source, should never get here
                shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
                return false;
            }
            else if (source == RB.AssetSource.ResourcesAsync)
            {
                // Finally attempt async resource load
                mResourceRequest = Resources.LoadAsync<Shader>(this.path);

                if (mResourceRequest == null)
                {
                    shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                    return false;
                }

                shaderAsset.progress = 0;
                shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);
            }
#if ADDRESSABLES_PACKAGE_AVAILABLE
            else if (source == RB.AssetSource.AddressableAssets)
            {
                // Exceptions on LoadAssetAsync can't actually be caught... this might work in the future so leaving it here
                try
                {
                    mAddressableRequest = Addressables.LoadAssetAsync<Shader>(this.path);
                }
                catch (UnityEngine.AddressableAssets.InvalidKeyException e)
                {
                    RBUtil.Unused(e);
                    shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotFound);
                    return false;
                }
                catch (Exception e)
                {
                    RBUtil.Unused(e);
                    shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                // Check for an immediate failure
                if (mAddressableRequest.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(mAddressableRequest);
                    shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.Undefined);
                    return false;
                }

                shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Loading, RB.Result.Pending);
                shaderAsset.progress = 0;

                return true;
            }
#endif
            else
            {
                shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Abort asynchronous loading
        /// </summary>
        public void Abort()
        {
            if (shaderAsset == null)
            {
                return;
            }

            if (shaderAsset.status != RB.AssetStatus.Loading)
            {
                return;
            }

            if (mResourceRequest != null)
            {
                // Can't abort a ResourceRequest... we will just ignore it
                mResourceRequest = null;
            }

#if ADDRESSABLES_PACKAGE_AVAILABLE
            Addressables.Release(mAddressableRequest);
            mAddressableRequest = new AsyncOperationHandle<Shader>();
#endif
        }

        private bool FinalizeShader(Shader loadedShader)
        {
            var material = new RBRenderer.RetroBlitShader(loadedShader);
            if (material == null)
            {
                Debug.Log("Could not create material for shader");
                shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NoResources);
                return false;
            }

            shaderAsset.shader = material;
            shaderAsset.progress = 1;
            shaderAsset.InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

            return true;
        }
    }
}
