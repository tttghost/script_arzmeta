using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class BuildCompletedAlert
{
    private const string clipPath = "alarm";
    
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) 
    {
        Debug.Log( pathToBuiltProject );
        BuildCompleteSound();
    }

    private static async void BuildCompleteSound()
    {
        await Task.Delay(2000);

        EditorUtility.audioMasterMute = false;

        EditorSFX.PlayClip(Resources.Load<AudioClip>(clipPath));
    }
}
