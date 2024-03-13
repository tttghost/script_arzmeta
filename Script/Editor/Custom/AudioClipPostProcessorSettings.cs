using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AudioClipPostProcessorSettings : ScriptableObject
{
    private const string settingsAssetPath = "Assets/Editor/AudioClipPostProcessorSettings.asset";

    public AudioImportPath pathType = AudioImportPath.All;
    
    [Header("Custom Path")]
    public string bgmPath = "Assets/AudioClip/BGM";
    public string sfxPath = "Assets/AudioClip/SFX";
    
    [InitializeOnLoadMethod]
    private static void Init()
    {
        var asset = AssetDatabase.LoadAssetAtPath<AudioClipPostProcessorSettings>(settingsAssetPath);
        if (asset == null)
        {
            AudioClipPostProcessorSettings settingsAsset = CreateInstance<AudioClipPostProcessorSettings>();
            AssetDatabase.CreateAsset(settingsAsset, settingsAssetPath);
        }
    }
}

public enum AudioImportPath
{
    All,
    Custom,
}




