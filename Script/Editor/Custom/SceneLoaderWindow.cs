/*******************************************************************************************************
 * 
 *		SceneLoaderWindow.cs
 *			- 특정 폴더안의 scene 을 오픈시키기 위한 EditorWindow
 * 
 *******************************************************************************************************/

using System;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneLoaderWindow : EditorWindow
{
	private SceneLoaderData sceneLoaderDataInstance = default;
	public SceneLoaderData sceneLoaderData
	{
		get
		{
			if ( sceneLoaderDataInstance == null )
				sceneLoaderDataInstance = SceneLoaderData.Load();

			return sceneLoaderDataInstance;
		}
	}

	//private string currentPath;
	[SerializeField] private Vector2 scrollPos = Vector2.zero;

	void OnGUI()
    {
        if (Application.isPlaying) GUI.enabled = false;
		
		CommonFunctionEditor.DrawUILine( Color.gray );

		sceneLoaderData.loadSceneWhenPlay = GUILayout.Toggle(sceneLoaderData.loadSceneWhenPlay, "플레이시 로드할 Default Scene 지정");
		if (sceneLoaderData.loadSceneWhenPlay)
		{
			sceneLoaderData.defaultSceneIndex = EditorGUILayout.Popup("Scene", sceneLoaderData.defaultSceneIndex, sceneLoaderData.sceneNameList.ToArray());

			//sceneLoaderData.mode = (LoadSceneMode)EditorGUILayout.Popup("Load Mode", (int)sceneLoaderData.mode, Enum.GetNames(typeof(LoadSceneMode)));
		}

		CommonFunctionEditor.DrawUILine(Color.gray);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        //--------------------------------------------------------------------------
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scene 폴더 지정");
        if (GUILayout.Button(new GUIContent("폴더 지정", "사용 하려고 하는 폴더의 Scene 을 지정합니다")))
        {
            if (sceneLoaderData != null)
            {
                sceneLoaderData.ClearList();

                // 폴더지정 파일 다이알로그 띄우기
                string folderPath = EditorUtility.SaveFolderPanel("Scene 폴더 지정", Application.dataPath, "");
                string assetPath = folderPath.Substring(folderPath.IndexOf("Assets"));
                
                if (!string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(sceneLoaderData.currentPath))
                {
	                sceneLoaderData.currentPath = assetPath;
                }

                // 지정된 폴더경로에서 scene 파일만 찾아오기
                GetList(sceneLoaderData.currentPath);

                if (sceneLoaderData.GetSceneNameListCount() == 0)
                    EditorUtility.DisplayDialog("안내 팝업", "폴더 내에 scene 파일이 없습니다.", "확인");
            }
        }
        GUILayout.EndHorizontal();

        if ( sceneLoaderData != null )
			sceneLoaderData.DrawSceneList();

        EditorGUILayout.EndScrollView();
		CommonFunctionEditor.DrawUILine( Color.gray );

		if(Application.isPlaying) GUI.enabled = true;
    }

	private void OnProjectChange()
	{
		sceneLoaderData.ClearList();
		GetList(sceneLoaderData.currentPath);
	}

	private void GetList(string path)
	{
		StringBuilder sb = new StringBuilder();
		System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
		foreach (System.IO.FileInfo file in di.GetFiles())
		{
			string extension = file.Name.Substring(file.Name.Length - 5, 5);
			if (extension.Equals("unity") == false)
				continue;

			// 지정한 폴더경로 + scene 파일명 조합하기
			sb.Length = 0;

			path = path.Substring(path.IndexOf("Assets"));

			sb.Append(path);
			sb.Append("/");
			sb.Append(file.Name);

			sceneLoaderData.AddSceneName(file.Name);
			sceneLoaderData.AddScenePath(sb.ToString());
		}
	}

	/// <summary>
	/// 유니티 에디터에 메뉴 추가하기
	/// </summary>
	[MenuItem("클라이언트팀/Scene 관련 툴/Scene Loader")]

	public static void ShowWindow()
	{
		SceneLoaderWindow window = EditorWindow.GetWindow<SceneLoaderWindow>( false, " Scene Loader ", true );
		window.titleContent = new GUIContent( "Scene 로더" );
	}
}

