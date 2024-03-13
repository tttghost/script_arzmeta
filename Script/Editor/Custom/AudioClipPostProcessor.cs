using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AudioClipPostProcessor : AssetPostprocessor
{
    private const string settingsAssetPath = "Assets/Editor/AudioClipPostProcessorSettings.asset";

    private AudioClipPostProcessorSettings asset;
    
    private void OnPreprocessAudio()
    {
        asset = AssetDatabase.LoadAssetAtPath<AudioClipPostProcessorSettings>(settingsAssetPath);

        switch (asset.pathType)
        {
            case AudioImportPath.All:
                ImportAudioClipAll();
                break;
            case AudioImportPath.Custom:
                ImportCustomPathAudioClips();
                break;
        }
    }
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string movedAsset in movedAssets)
        {
            if (movedAsset.Contains(".wav") || movedAsset.Contains(".mp3"))
            {
                AssetDatabase.ImportAsset(movedAsset);
            }
        }
    }

    private void ImportCustomPathAudioClips()
    {
        AudioImporter audioImporter = assetImporter as AudioImporter;
        AudioImporterSampleSettings defaultSampleSettings = audioImporter.defaultSampleSettings;
        AudioImporterSampleSettings androidSampleSettings = audioImporter.GetOverrideSampleSettings("Android");
        AudioImporterSampleSettings iosSampleSettings = audioImporter.GetOverrideSampleSettings("iOS");

        bool settingChanged = false;
        string path = audioImporter.assetPath;

        if (path.Contains(asset.bgmPath))
        {
            audioImporter.forceToMono = false;
            audioImporter.loadInBackground = true;
        }

        if (path.Contains(asset.sfxPath))
        {
            audioImporter.forceToMono = true;
            
            if (path.Contains("Game"))
            {
                // 인게임 효과음의 공통 설정
            }
            else if (path.Contains("UI"))
            {
                // UI 효과음의 공통 설정
            }
        }
        
        defaultSampleSettings.loadType = AudioClipLoadType.Streaming;
        defaultSampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;

        androidSampleSettings.loadType = AudioClipLoadType.CompressedInMemory;
        androidSampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;
        
        iosSampleSettings.loadType = AudioClipLoadType.Streaming;
        iosSampleSettings.compressionFormat = AudioCompressionFormat.MP3;

        audioImporter.defaultSampleSettings = defaultSampleSettings;
        audioImporter.SetOverrideSampleSettings("Android", androidSampleSettings);
        audioImporter.SetOverrideSampleSettings("iOS", iosSampleSettings);
        
        Debug.LogFormat("{0} has been imported to {1}.\nforce to mono - {2}\nload type - {3}\ncompression format - default: {4}, iOS: {5}, Android: {6}",
            Path.GetFileName(path), Path.GetDirectoryName(path), audioImporter.forceToMono.ToString(), defaultSampleSettings.loadType.ToString(),
            defaultSampleSettings.compressionFormat.ToString(), iosSampleSettings.compressionFormat.ToString(), androidSampleSettings.compressionFormat.ToString());
    }

    private void ImportAudioClipAll()
    {
        AudioImporter audioImporter = assetImporter as AudioImporter;
        AudioImporterSampleSettings defaultSampleSettings = audioImporter.defaultSampleSettings;
        AudioImporterSampleSettings androidSampleSettings = audioImporter.GetOverrideSampleSettings("Android");
        AudioImporterSampleSettings iosSampleSettings = audioImporter.GetOverrideSampleSettings("iOS");

        string path = audioImporter.assetPath;

        audioImporter.forceToMono = true;
        audioImporter.loadInBackground = true;

        defaultSampleSettings.loadType = AudioClipLoadType.Streaming;
        defaultSampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;

        androidSampleSettings.loadType = AudioClipLoadType.CompressedInMemory;
        androidSampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;
        
        iosSampleSettings.loadType = AudioClipLoadType.Streaming;
        iosSampleSettings.compressionFormat = AudioCompressionFormat.MP3;
        
        audioImporter.defaultSampleSettings = defaultSampleSettings;
        audioImporter.SetOverrideSampleSettings("Android", androidSampleSettings);
        audioImporter.SetOverrideSampleSettings("iOS", iosSampleSettings);
        
        Debug.LogFormat("{0} has been imported to {1}.\nforce to mono - {2}\nload type - {3}\ncompression format - default: {4}, iOS: {5}, Android: {6}",
            Path.GetFileName(path), Path.GetDirectoryName(path), audioImporter.forceToMono.ToString(), defaultSampleSettings.loadType.ToString(),
            defaultSampleSettings.compressionFormat.ToString(), iosSampleSettings.compressionFormat.ToString(), androidSampleSettings.compressionFormat.ToString());
    }
}
