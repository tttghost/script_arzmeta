using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad, DefaultExecutionOrder(-1000)]
public static class DefaultSceneLoader
{
    static DefaultSceneLoader(){
        var sceneData = SceneLoaderData.Load();

        if (!sceneData.loadSceneWhenPlay) return;

        EditorSceneManager.playModeStartScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneData.scenePathList[sceneData.defaultSceneIndex]);
        
        EditorApplication.playModeStateChanged += LoadDefaultScene;
    }

    static void LoadDefaultScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
    }
}
