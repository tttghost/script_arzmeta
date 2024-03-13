using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class FrontisRenderEditor : EditorWindow
{
    private static readonly Vector2 _minSize = new Vector2(300, 500);

    private static float smallestOccluder = 5;
    private static float smallestHole = 0.25f;
    private static float backfaceThreshold = 100;
    
    private void OnEnable()
    {
        smallestOccluder = EditorPrefs.GetFloat("frontis_smallestOccluder", smallestOccluder);
        smallestHole = EditorPrefs.GetFloat("frontis_smallestHole", smallestHole);
        backfaceThreshold = EditorPrefs.GetFloat("frontis_backfaceThreshold", backfaceThreshold);
    }

    private void OnDisable()
    {
        EditorPrefs.SetFloat("frontis_smallestOccluder", smallestOccluder);
        EditorPrefs.SetFloat("frontis_smallestHole", smallestHole);
        EditorPrefs.SetFloat("frontis_backfaceThreshold", backfaceThreshold);
    }

    private void OnGUI()
    {
        Draw();
    }

    private void Draw()
    {
        minSize = _minSize;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Bake Light", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        if(GUILayout.Button("☆Bakery Render Lightmap☆"))
        {
            BakeryRenderLightmapMenu();
        }
        
        GUILayout.Space(10);
        
        if(GUILayout.Button("라이트맵 베이크"))
        {
            RenderLightmap();
        }

        if(GUILayout.Button("리플렉션 프로브 베이크"))
        {
            RenderReflectionProbe();
        }

        GUILayout.Space(15);
        EditorGUILayout.LabelField("Occlusion Culling", EditorStyles.boldLabel);
        GUILayout.Space(5);

        smallestOccluder = EditorGUILayout.FloatField("Smallest Occluder", smallestOccluder);
        smallestHole = EditorGUILayout.FloatField("Smallest Hole", smallestHole);
        backfaceThreshold =
            EditorGUILayout.Slider("Backface Threshold", backfaceThreshold, 5.0f, 100.0f);

        if(GUILayout.Button("오클루젼 데이터 삭제"))
        {
            ClearOcclusionCulling();
        }
        
        if(GUILayout.Button("폴더 캐시 삭제"))
        {
            RemoveOcclusionCullingCache();
        }
        
        GUILayout.Space(10);
        
        if(GUILayout.Button("☆베이크☆"))
        {
            BakeOcclusionCulling();
        }

        GUILayout.Space(15);
        EditorGUILayout.LabelField("LOD", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        
        if(GUILayout.Button("전체 켜기"))
        {
            EnableLOD(true);
        }
        
        if(GUILayout.Button("전체 끄기"))
        {
            EnableLOD(false);
        }
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// 게임 이벤트 매니저 메뉴를 표시합니다.
    /// </summary>
    [MenuItem("클라이언트팀/렌더링 원 툴", priority = 0)]
    public static void ShowWindow()
    {
        FrontisRenderEditor window = GetWindow<FrontisRenderEditor>( false, "렌더링 원 툴", true );
        window.titleContent = new GUIContent( "렌더링 원 툴" );
    }
    
    public void ClearOcclusionCulling()
    {
        StaticOcclusionCulling.Clear();
    }

    public void RemoveOcclusionCullingCache()
    {
        StaticOcclusionCulling.RemoveCacheFolder();
    }
    
    public void BakeOcclusionCulling()
    {
        StaticOcclusionCulling.smallestOccluder = smallestOccluder;
        StaticOcclusionCulling.smallestHole = smallestHole;
        StaticOcclusionCulling.backfaceThreshold = backfaceThreshold;
        StaticOcclusionCulling.GenerateInBackground();
    }

    public void EnableLOD(bool enable)
    {
        var lods = FindObjectsOfType<LODGroup>();

        foreach (var lod in lods)
        {
            lod.enabled = enable;
        }
        
        EditorUtility.SetDirty(this);
    }

    public void RenderLightmap()
    {
        Lightmapping.Bake();
    }

    public void RenderReflectionProbe()
    {
        var reflectionProbes = FindObjectsOfType<ReflectionProbe>();
        var path = EditorSceneManager.GetActiveScene().path;
        var sceneName = EditorSceneManager.GetActiveScene().name;
        var actualPath = path.Remove(path.Length - 6);

        for (var index = 0; index < reflectionProbes.Length; index++)
        {
            var reflectionProbe = reflectionProbes[index];
            
            Lightmapping.BakeReflectionProbe(reflectionProbe, path: actualPath + $"/ReflectionProbe-{index}.exr");
        }
    }
    
    public void BakeryRenderLightmapMenu()
    {
        ftRenderLightmap.RenderLightmap();
    }
}
