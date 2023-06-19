using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom default settings for audio imports
/// </summary>
public class RetroBlitAudioPostProcessor : AssetPostprocessor
{
    /// <summary>
    /// Called when audio clip is processed by the asset importer, this gives us a chance to adjust the defaults
    /// </summary>
    /// <param name="audioClip">Audio clip</param>
    public void OnPostprocessAudio(AudioClip audioClip)
    {
        if (assetPath.Contains("RetroBlit-ignore"))
        {
            return;
        }

        AudioImporter importer = assetImporter as AudioImporter;
        AudioImporterSampleSettings iss = new AudioImporterSampleSettings();
        iss.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
#if UNITY_2022_2_OR_NEWER
        iss.preloadAudioData = true;
#else
        importer.preloadAudioData = true;
#endif

        bool normalize;

        // Assume clips longer than 10 seconds should stream (they are probably music)
        if (audioClip.length > 10)
        {
            importer.forceToMono = false;
#if UNITY_2022_2_OR_NEWER
            iss.preloadAudioData = false;
#else
            importer.preloadAudioData = false;
#endif
            iss.loadType = AudioClipLoadType.Streaming;
            iss.compressionFormat = AudioCompressionFormat.Vorbis;
            iss.quality = 0.7f;
            iss.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            normalize = true;
        }
        else
        {
            importer.forceToMono = false;
#if UNITY_2022_2_OR_NEWER
            iss.preloadAudioData = true;
#else
            importer.preloadAudioData = true;
#endif
            iss.loadType = AudioClipLoadType.DecompressOnLoad;
            iss.compressionFormat = AudioCompressionFormat.Vorbis;
            iss.quality = 0.7f;
            iss.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            normalize = true;
        }

        importer.defaultSampleSettings = iss;

        // Have to use serialized object to get at Normalize
        SerializedObject so = new SerializedObject(importer);
        var normalizeProp = so.FindProperty("m_Normalize");
        if (normalizeProp != null)
        {
            normalizeProp.boolValue = normalize;
            so.ApplyModifiedProperties();
        }
    }
}
