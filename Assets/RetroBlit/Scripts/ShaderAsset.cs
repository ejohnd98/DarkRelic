using System;
using System.Collections.Generic;
using UnityEngine;

/*********************************************************************************
* The comments in this file are used to generate the API documentation. Please see
* Assets/RetroBlit/Docs for much easier reading!
*********************************************************************************/

#if ADDRESSABLES_PACKAGE_AVAILABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

/// <summary>
/// Shader asset
/// </summary>
/// <remarks>
/// Contains a shader asset. Shader assets can be loaded from various sources, including synchronous and asynchronous sources. Use <see cref="ShaderAsset.Load"/> to load a shader asset.
/// </remarks>
public class ShaderAsset : RBAsset
{
    /// <summary>
    /// RetroBlit shader wrapper for this shader
    /// </summary>
    public RetroBlitInternal.RBRenderer.RetroBlitShader shader;

#if ADDRESSABLES_PACKAGE_AVAILABLE
    private AsyncOperationHandle<Shader> mAddressableRequest;
#endif

    ~ShaderAsset()
    {
        // Careful here, this finalizer could be called at any point, even when RetroBlit is not initialized anymore! Also must be on main thread.
        if (RetroBlitInternal.RBAPI.instance != null && RetroBlitInternal.RBAPI.instance.Audio != null && RetroBlitInternal.RBUtil.OnMainThread())
        {
            RetroBlitInternal.RBAPI.instance.AssetManager.ShaderUnload(this);
        }
        else
        {
            // Otherwise queue up the resource for later release
            if (shader != null)
            {
                RetroBlitInternal.RBAPI.instance.AssetManager.ShaderUnloadMainThread(shader);
            }
        }

        ResetInternalState();
    }

#if ADDRESSABLES_PACKAGE_AVAILABLE
    /// <summary>
    /// Addressable Asset handle, used internally by RetroBlit
    /// </summary>
    public AsyncOperationHandle<Shader> addressableHandle
    {
        set
        {
            mAddressableRequest = value;
        }
    }
#endif

    /// <summary>
    /// Get a unique ID of a shader property
    /// </summary>
    /// <remarks>
    /// Get a unique ID of a shader property. Property IDs are more efficient than property name strings, and should be used instead for optimal
    /// performance.
    ///
    /// Note that property IDs are tied to the property name, not to any particular shader. The same property ID can be used with multiple shaders that
    /// have the same property names.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Name of the property</param>
    /// <returns>Unique ID</returns>
    /// <code>
    /// SpriteSheetAsset spritePackCharacters = new SpriteSheetAsset();
    /// ShaderAsset shaderEffects = new ShaderAsset();
    ///
    /// const int SHADER_PROP_GLOW = RB.ShaderPropertyID("glow");
    ///
    /// void Initialize() {
    ///     spritePackCharacters.Load("spritesheet/tiles");
    ///     shaderEffects.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     RB.SpriteSheetSet(spritePackCharacters);
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffects);
    ///     shaderEffects.ColorSet(SHADER_PROP_GLOW, Color.yellow);
    ///     RB.DrawSprite("glowing_hero", glowingHeroPos);
    ///
    ///     // Turn off shader and draw a sprite with default RetroBlit shader
    ///     RB.ShaderReset();
    ///     RB.DrawSprite("boring_hero", boringHeroPos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public static int PropertyID(string name)
    {
        return Shader.PropertyToID(name);
    }

    /// <summary>
    /// Load shader asset from the given source
    /// </summary>
    /// <remarks>
    /// Load a shader asset which can be used when drawing sprites or applying screen effects. There are various asset sources supported:
    /// <list type="bullet">
    /// <item><b>Resources</b> - Synchronously loaded shader assets from a <b>Resources</b> folder. This was the only asset source supported in RetroBlit prior to 3.0.</item>
    /// <item><b>ResourcesAsync</b> - Asynchronously loaded shader assets from a <b>Resources</b> folder.</item>
    /// <item><b>AddressableAssets</b> - Asynchronously loaded shader assets from Unity Addressable Assets.</item>
    /// <item><b>Existing Assets</b> - Synchronously loaded shader assets from an existing Unity <b>Shader</b>.</item>
    /// </list>
    ///
    /// If the asset is loaded via a synchronous method then <b>Load</b> will block until the loading is complete.
    /// If the asset is loaded via an asynchronous method then <b>Load</b> will immediately return and the asset loading will
    /// continue in a background thread. The status of an asynchronous loading asset can be check by looking at <see cref="RBAsset.status"/>,
    /// or by using the event system with <see cref="RBAsset.OnLoadComplete"/> to get a callback when the asset is done loading.
    ///
    /// Note that unlike other asset types, <b>WWW</b> does not support loading shaders, this is a limitation of Unity.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// <seedoc>Features:Asynchronous Asset Loading</seedoc>
    /// </remarks>
    /// <code>
    /// ShaderAsset shaderFancy = new ShaderAsset();
    ///
    /// public void Initialize()
    /// {
    ///     // Load asset from Resources asynchronously. This method call will immediately return without blocking.
    ///     shaderFancy.Load("fancy_shader", RB.AssetSource.ResourcesAsync);
    /// }
    ///
    /// public void Render()
    /// {
    ///     if (shaderFancy.status == RB.AssetStatus.Ready)
    ///     {
    ///         RB.ShaderSet(shaderFancy);
    ///     }
    ///
    ///     // Draw a sprite with the shader applied if the shader has finished loading.
    ///     RB.DrawSprite("hero/walk1", playerPos);
    /// }
    /// </code>
    /// <param name="filename">File name to load from</param>
    /// <param name="source">Asset source type</param>
    /// <returns>Load status</returns>
    /// <seealso cref="RB.Result"/>
    /// <seealso cref="RB.AssetStatus"/>
    /// <seealso cref="RB.AssetSource"/>
    public RB.AssetStatus Load(string filename, RB.AssetSource source = RB.AssetSource.Resources)
    {
        Unload();

        if (!RetroBlitInternal.RBAssetManager.CheckSourceSupport(source))
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.NotSupported);
            return status;
        }

        RetroBlitInternal.RBAPI.instance.AssetManager.ShaderLoad(filename, this, source);

        return status;
    }

    /// <summary>
    /// Load a shader asset from an already existing *Shader* object.
    /// </summary>
    /// <param name="existingShader">Existing *Shader* object</param>
    /// <returns>Load status</returns>
    public RB.AssetStatus Load(Shader existingShader)
    {
        Unload();
        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);

        if (existingShader == null)
        {
            InternalSetErrorStatus(RB.AssetStatus.Failed, RB.Result.BadParam);
            return status;
        }

        RetroBlitInternal.RBRenderer.RetroBlitShader newShader = new RetroBlitInternal.RBRenderer.RetroBlitShader(existingShader);
        shader = newShader;
        progress = 1;

        InternalSetErrorStatus(RB.AssetStatus.Ready, RB.Result.Success);

        return status;
    }

    /// <summary>
    /// Unload a previously loaded Shader asset.
    /// </summary>
    public override void Unload()
    {
        ResetInternalState();
        progress = 0;
#if ADDRESSABLES_PACKAGE_AVAILABLE
        if (mAddressableRequest.IsValid())
        {
            Addressables.Release(mAddressableRequest);
        }
#endif
        InternalSetErrorStatus(RB.AssetStatus.Invalid, RB.Result.Undefined);
    }

    /// <summary>
    /// Set a shader color property
    /// </summary>
    /// <remarks>
    /// Set a shader color property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Property name</param>
    /// <param name="color">Color</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.ColorSet("color", Color.yellow);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void ColorSet(string name, Color32 color)
    {
        ColorSet(Shader.PropertyToID(name), color);
    }

    /// <summary>
    /// Set a shader color property
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="color">Color</param>
    public void ColorSet(int propertyID, Color32 color)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetColor(propertyID, color);
    }

    /// <summary>
    /// Set a shader color array property
    /// </summary>
    /// <remarks>
    /// Set a shader color array property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="colorArray">Color array</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     var colors = new List<Color32>();
    ///     colors.Add(Color.red);
    ///     colors.Add(Color.green);
    ///     colors.Add(Color.blue);
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.ColorArraySet("color", colors);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void ColorArraySet(string name, List<Color32> colorArray)
    {
        ColorArraySet(Shader.PropertyToID(name), colorArray.ToArray());
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="colorArray">Color list</param>
    public void ColorArraySet(int propertyID, List<Color32> colorArray)
    {
        ColorArraySet(propertyID, colorArray.ToArray());
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="name">Property name</param>
    /// <param name="colorArray">Color array</param>
    public void ColorArraySet(string name, Color32[] colorArray)
    {
        ColorArraySet(Shader.PropertyToID(name), colorArray);
    }

    /// <summary>
    /// Set a shader color array
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="colorArray">Color array</param>
    public void ColorArraySet(int propertyID, Color32[] colorArray)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        if (colorArray != null)
        {
            Color[] colorArray32 = new Color[colorArray.Length];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray32[i] = colorArray[i];
            }

            RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetColorArray(propertyID, colorArray32);
        }
        else
        {
            RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetColorArray(propertyID, new List<Color>());
        }
    }

    /// <summary>
    /// Set a shader float property
    /// </summary>
    /// <remarks>
    /// Set a shader float property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Property name</param>
    /// <param name="value">Float value</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.FloatSet("intensity", 0.5f);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void FloatSet(string name, float value)
    {
        FloatSet(Shader.PropertyToID(name), value);
    }

    /// <summary>
    /// Set a shader float property
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="value">Float value</param>
    public void FloatSet(int propertyID, float value)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetFloat(propertyID, value);
    }

    /// <summary>
    /// Set a shader float array property
    /// </summary>
    /// <remarks>
    /// Set a shader float array property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="values">Float array</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     var floats = new List<float>();
    ///     floats.Add(0.25f);
    ///     floats.Add(0.5f);
    ///     floats.Add(0.75f);
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.FloatArraySet("levels", floats);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void FloatArraySet(string name, float[] values)
    {
        FloatArraySet(Shader.PropertyToID(name), values);
    }

    /// <summary>
    /// Set a shader float array
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="values">Float array</param>
    public void FloatArraySet(int propertyID, float[] values)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetFloatArray(propertyID, values);
    }

    /// <summary>
    /// Set a shader float array
    /// </summary>
    /// <param name="name">Property name</param>
    /// <param name="values">Float list</param>
    public void FloatArraySet(string name, List<float> values)
    {
        FloatArraySet(Shader.PropertyToID(name), values);
    }

    /// <summary>
    /// Set a shader float array
    /// </summary>
    /// <param name="propertyID">Property name</param>
    /// <param name="values">Float list</param>
    public void FloatArraySet(int propertyID, List<float> values)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetFloatArray(propertyID, values);
    }

    /// <summary>
    /// Set a shader integer property
    /// </summary>
    /// <remarks>
    /// Set a shader integer property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Property name</param>
    /// <param name="value">Integer value</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.InSet("count", 25);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void IntSet(string name, int value)
    {
        IntSet(Shader.PropertyToID(name), value);
    }

    /// <summary>
    /// Set a shader integer property
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="value">Integer value</param>
    public void IntSet(int propertyID, int value)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetInt(propertyID, value);
    }

    /// <summary>
    /// Set a shader matrix property
    /// </summary>
    /// <remarks>
    /// Set a shader matrix property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Property name</param>
    /// <param name="matrix">Matrix</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     var matrix = Matrix4x4.TRS(pos, rot, scale);
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.MatrixSet("mat", matrix);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void MatrixSet(string name, Matrix4x4 matrix)
    {
        MatrixSet(Shader.PropertyToID(name), matrix);
    }

    /// <summary>
    /// Set a shader matrix
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="matrix">Matrix</param>
    public void MatrixSet(int propertyID, Matrix4x4 matrix)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetMatrix(propertyID, matrix);
    }

    /// <summary>
    /// Set a shader matrix array property
    /// </summary>
    /// <remarks>
    /// Set a shader matrix array property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="shaderIndex">Shader index</param>
    /// <param name="name">Property name</param>
    /// <param name="matrices">Matrix array</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     var matrices = new List<Matrix4x4>();
    ///     matrices.Add(Matrix4x4.TRS(new Vector3(0, 0, 0), rot, scale));
    ///     matrices.Add(Matrix4x4.TRS(new Vector3(100, 100, 0), rot, scale));
    ///     matrices.Add(Matrix4x4.TRS(new Vector3(200, 200, 0), rot, scale));
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.MatrixArraySet("mats", matrices);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void MatrixArraySet(string name, Matrix4x4[] matrices)
    {
        MatrixArraySet(Shader.PropertyToID(name), matrices);
    }

    /// <summary>
    /// Set a shader matrix array
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="matrices">Matrix array</param>
    public void MatrixArraySet(int propertyID, Matrix4x4[] matrices)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetMatrixArray(propertyID, matrices);
    }

    /// <summary>
    /// Set a shader matrix array
    /// </summary>
    /// <param name="name">Property name</param>
    /// <param name="matrices">Matrix list</param>
    public void MatrixArraySet(string name, List<Matrix4x4> matrices)
    {
        MatrixArraySet(Shader.PropertyToID(name), matrices);
    }

    /// <summary>
    /// Set a shader matrix array
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="matrices">Matrix list</param>
    public void MatrixArraySet(int propertyID, List<Matrix4x4> matrices)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetMatrixArray(propertyID, matrices);
    }

    /// <summary>
    /// Set a shader vector property
    /// </summary>
    /// <remarks>
    /// Set a shader vector property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Property name</param>
    /// <param name="vector">Vector</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     var matrix = Matrix4x4.TRS(pos, rot, scale);
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.VectorSet("offset", new Vector4(100, 100, 0, 0));
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void VectorSet(string name, Vector4 vector)
    {
        VectorSet(Shader.PropertyToID(name), vector);
    }

    /// <summary>
    /// Set a shader vector property
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="vector">Vector</param>
    public void VectorSet(int propertyID, Vector4 vector)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetVector(propertyID, vector);
    }

    /// <summary>
    /// Set a shader vector array property
    /// </summary>
    /// <remarks>
    /// Set a shader vector array property.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Property name</param>
    /// <param name="vectors">Vector array</param>
    /// <code>
    /// ShaderAsset shaderEffect = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     shaderEffect.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     var vertices = new List<Vector4>();
    ///     vertices.Add(new Vector4(0, 0, 0, 0));
    ///     vertices.Add(new Vector4(100, 0, 0, 0));
    ///     vertices.Add(new Vector4(0, 100, 0, 0));
    ///
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffect);
    ///     shaderEffect.VectorArraySet("verts", vertices);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void VectorArraySet(string name, Vector4[] vectors)
    {
        VectorArraySet(Shader.PropertyToID(name), vectors);
    }

    /// <summary>
    /// Set a shader vector array
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="vectors">Vector array</param>
    public void VectorArraySet(int propertyID, Vector4[] vectors)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetVectorArray(propertyID, vectors);
    }

    /// <summary>
    /// Set a shader vector array
    /// </summary>
    /// <param name="name">Property name</param>
    /// <param name="vectors">Vector list</param>
    public void VectorArraySet(string name, List<Vector4> vectors)
    {
        VectorArraySet(Shader.PropertyToID(name), vectors);
    }

    /// <summary>
    /// Set a shader vector array
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="vectors">Vector list</param>
    public void VectorArraySet(int propertyID, List<Vector4> vectors)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetVectorArray(propertyID, vectors);
    }

    /// <summary>
    /// Set a shader spritesheet texture
    /// </summary>
    /// <remarks>
    /// Set a shader spritesheet texture.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="name">Property name</param>
    /// <param name="spriteSheet">Spritesheet asset</param>
    /// <code>
    /// SpriteSheetAsset offscreenMask = new SpriteSheetAsset();
    /// ShaderAsset rippleShader = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     offscreenMask.Create(RB.DisplaySize);
    ///     rippleShader.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(rippleShader);
    ///     rippleShader.SpriteSheetTextureSet("tex", offscreenMask);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetFilterSet"/>
    public void SpriteSheetTextureSet(string name, SpriteSheetAsset spriteSheet)
    {
        SpriteSheetTextureSet(Shader.PropertyToID(name), spriteSheet);
    }

    /// <summary>
    /// Set a shader offscreen texture
    /// </summary>
    /// <param name="propertyID">Property ID</param>
    /// <param name="spriteSheet">Spritesheet asset</param>
    public void SpriteSheetTextureSet(int propertyID, SpriteSheetAsset spriteSheet)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetTexture(propertyID, spriteSheet.internalState.texture);
    }

    /// <summary>
    /// Set a shader spritesheet texture filter mode
    /// </summary>
    /// <remarks>
    /// Set a shader spritesheet texture filter mode. By default the filter mode is <mref refid="RB.Filter.Nearest">Nearest</mref>, for some effects
    /// it may be desirable to change the filter mode to <mref refid="RB.Filter.Linear">Linear</mref>.
    /// <seedoc>Features:Shaders (Advanced Topic)</seedoc>
    /// </remarks>
    /// <param name="spriteSheet">Spritesheet asset</param>
    /// <param name="filter"><see cref="RB.Filter.Nearest"/> or <see cref="RB.Filter.Linear"/></param>
    /// <code>
    /// SpriteSheetAsset offscreenMask = new SpriteSheetAsset();
    /// ShaderAsset shaderEffects = new ShaderAsset();
    ///
    /// void Initialize() {
    ///     offscreenMask.Create(RB.DisplaySize);
    ///     shaderEffects.Load("shaders/effects");
    /// }
    ///
    /// void Render() {
    ///     // Draw a sprite with a shader effect
    ///     RB.ShaderSet(shaderEffects);
    ///     shaderEffects.SpriteSheetTextureSet("tex", offscreenMask);
    ///     shaderEffects.SpriteSheetFilterSet(offscreenMask, Filter.Linear);
    ///     RB.DrawSprite("hero", pos);
    /// }
    /// </code>
    /// <seealso cref="RB.ShaderSet"/>
    /// <seealso cref="RB.ShaderApplyNow"/>
    /// <seealso cref="RB.ShaderReset"/>
    /// <seealso cref="RB.ShaderPropertyID"/>
    /// <seealso cref="ShaderAsset.ColorSet"/>
    /// <seealso cref="ShaderAsset.ColorArraySet"/>
    /// <seealso cref="ShaderAsset.FloatSet"/>
    /// <seealso cref="ShaderAsset.FloatArrraySet"/>
    /// <seealso cref="ShaderAsset.IntSet"/>
    /// <seealso cref="ShaderAsset.MatrixSet"/>
    /// <seealso cref="ShaderAsset.MatrixArraySet"/>
    /// <seealso cref="ShaderAsset.VectorSet"/>
    /// <seealso cref="ShaderAsset.VectorArraySet"/>
    /// <seealso cref="ShaderAsset.SpriteSheetTextureSet"/>
    public void SpriteSheetFilterSet(SpriteSheetAsset spriteSheet, RB.Filter filter)
    {
        if (status != RB.AssetStatus.Ready)
        {
            return;
        }

        FilterMode filterMode = filter == RB.Filter.Nearest ? FilterMode.Point : FilterMode.Bilinear;
        RetroBlitInternal.RBAPI.instance.Renderer.ShaderParameters(this).SetSpriteSheetFilter(spriteSheet, filterMode);
    }

    private void ResetInternalState()
    {
        shader = null;
    }
}
