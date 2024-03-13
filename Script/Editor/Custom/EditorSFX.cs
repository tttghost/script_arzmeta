using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

/// <summary>
/// 에디터에서 사운드 재생 가능하게 해주는 클래스입니다.
/// 
/// 참고: https://forum.unity.com/threads/way-to-play-audio-in-editor-using-an-editor-script.132042/#post-7015753
/// </summary>
public static class EditorSFX
{
    public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

#if UNITY_2020_1_OR_NEWER
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] {typeof(AudioClip), typeof(int), typeof(bool)},
            null
        );

        Debug.Log(method);
        method.Invoke(
            null,
            new object[] {clip, startSample, loop}
        );
#else
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(AudioClip) },
            null
        );
 
        Debug.Log(method);
        method.Invoke(
            null,
            new object[] { clip }
        );
#endif
    }

    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

#if UNITY_2020_1_OR_NEWER
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { },
            null
        );

        Debug.Log(method);
        method.Invoke(
            null,
            new object[] { }
        );
#else
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllClips",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[]{},
            null
        );
        method.Invoke(
            null,
            new object[] {}
        );
#endif
    }
}
