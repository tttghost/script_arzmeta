using System.Linq;
using UnityEngine;
using UnityEditor;

public class FrontisPackageImporter : AssetPostprocessor
{
    //private static int maxTag = 10000;
    private static int maxLayers = 31;

    private const string packageName = "Frontis";
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        var inPackages = importedAssets.Any(path => path.Contains(packageName));
        
        //var outPackage = deletedAssets.Any(path => path.Contains(packageName));
        
        // if (inPackages) InitializeOnLoad();
        if (inPackages)
        {
            
            
        }
    }

    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        if (EditorPrefs.GetBool($"FrontisPackageImporter_{PlayerSettings.productName}", false)) return;
        
        Setup();
    }

    [MenuItem("클라이언트팀/Danger Zone/프로젝트 세팅 초기화",false, 10000)]
    public static void SetupMenu()
    {
        var question = EditorUtility.DisplayDialog("경고!!!!", "프로젝트 세팅이 기본 세팅으로 초기화 됩니다. 초기화 하시겠습니까?", "예", "아니오");

        if (question)
        {
            Setup();
        }
    }
    
    public static void Setup()
    {
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.gcIncremental = true;
        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.companyName = "hancomfrontis";
        
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");
        SerializedProperty layers = tagManager.FindProperty("layers");
        SerializedProperty tags = tagManager.FindProperty("tags");
        
        layers.GetArrayElementAtIndex(3).stringValue = "Post Processing";
        layers.GetArrayElementAtIndex(6).stringValue = "Ignore";
        layers.GetArrayElementAtIndex(7).stringValue = "Player";
        layers.GetArrayElementAtIndex(8).stringValue = "NPC";
        layers.GetArrayElementAtIndex(9).stringValue = "OtherPlayer";
        //layers.GetArrayElementAtIndex(10).stringValue = "TouchZone";
        layers.GetArrayElementAtIndex(11).stringValue = "Invisible";
        layers.GetArrayElementAtIndex(18).stringValue = "OutLine";
        layers.GetArrayElementAtIndex(19).stringValue = "InteractionArea";
        //layers.GetArrayElementAtIndex(28).stringValue = "Book";
        //layers.GetArrayElementAtIndex(29).stringValue = "NoneInteractable";
        //layers.GetArrayElementAtIndex(30).stringValue = "NoneCollideable";
        //layers.GetArrayElementAtIndex(31).stringValue = "Code";
        
        CreateTag(tags, 0, "Media");
        CreateTag(tags, 1, "CinemachineTarget");
        CreateTag(tags, 2, "Avatar_Hair");
        CreateTag(tags, 3, "Hint");
        CreateTag(tags, 4, "OtherPlayer");
        CreateTag(tags, 5, "SpectatorCamera");

        tagManager.ApplyModifiedProperties();

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, (int) Architecture.ARM64);
#elif UNITY_ANDROID
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.Android, (int) Architecture.ARM64);
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
#elif UNITY_IOS
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, (int) Architecture.ARM64);
#endif
        
        EditorPrefs.SetBool($"FrontisPackageImporter_{PlayerSettings.productName}", true);
    }

    public static bool CreateTag(SerializedProperty tags, int index, string name)
    {
        if (TagExist(tags, name)) return false;

        tags.InsertArrayElementAtIndex(index);
        tags.GetArrayElementAtIndex(index).stringValue = name;
        return true;
    }

    private static bool TagExist(SerializedProperty tags, string name)
    {
        for (var i = 0; i < tags.arraySize; i++)
        {
            if(tags.GetArrayElementAtIndex(i).stringValue == name) return true;
        }

        return false;
    }
    
    public static bool CreateLayer(string layerName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        if (!PropertyExists(layersProp, 0, maxLayers, layerName))
        {
            SerializedProperty sp;
            // Start at layer 9th index -> 8 (zero based) => first 8 reserved for unity / greyed out
            for (int i = 8, j = maxLayers; i < j; i++)
            {
                sp = layersProp.GetArrayElementAtIndex(i);
                if (sp.stringValue == "")
                {
                    // Assign string value to layer
                    sp.stringValue = layerName;
                    Debug.Log("Layer: " + layerName + " has been added");
                    // Save settings
                    tagManager.ApplyModifiedProperties();
                    return true;
                }
                if (i == j)
                    Debug.Log("All allowed layers have been filled");
            }
        }
        else
        {
            Debug.Log ("Layer: " + layerName + " already exists");
        }
        return false;
    }
    
    public static bool LayerExists(string layerName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        return PropertyExists(layersProp, 0, maxLayers, layerName);
    }
    
    private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
    {
        for (int i = start; i < end; i++)
        {
            SerializedProperty t = property.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(value))
            {
                return true;
            }
        }
        return false;
    }
    
    
}

public enum Architecture
{
    None = 0,
    ARM64 = 1,
    Universal = 2
}
