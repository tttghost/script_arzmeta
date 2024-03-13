using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class CustomMenu
{
#if UNITY_EDITOR
	[MenuItem("클라이언트팀/커스텀프로젝트/커스텀 빌드세팅", priority = 1)]

	static void ShowCustomBuildSetting()
	{
		var obj = AssetDatabase.LoadMainAssetAtPath(Cons.Path_CustomProjectSetting);
		if (obj != null)
		{
			Selection.activeObject = obj;
		}
	}

	[MenuItem("기획팀/캡쳐폴더 열기", priority = 1)]
	static void OpenCaptureFolder()
	{
		Process.Start(Util.capturePath);
	}


	//작동안함
	//[MenuItem("클라이언트팀/캐시 삭제/튜토리얼 캐시 삭제", priority = 10)]
	//static void Editor_ResetTutorial()
	//{
	//       var guids = AssetDatabase.FindAssets("t:GameObject");
	//       TutorialManager tutorialManager = null;
	//       foreach (var guid in guids)
	//       {
	//           var path = AssetDatabase.GUIDToAssetPath(guid);
	//           var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
	//           if (asset.TryGetComponent(out tutorialManager))
	//           {
	//               break;
	//           }
	//       }
	//       if (tutorialManager)
	//       {
	//           tutorialManager.DeleteTutorialSaveData();
	//           Debug.Log("튜토리얼 캐시 삭제 성공");
	//       }
	//       else
	//       {
	//           Debug.Log("현재 씬에 튜토리얼매니저가 존재하지 않습니다. 다시 확인해 주세요.");
	//       }
	//   }

#endif
}
