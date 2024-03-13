using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialManager))]
public class TutorialManagerEditor : Editor
{
    private TutorialManager tutorialManager;

    private void OnEnable()
    {
        tutorialManager = (TutorialManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("튜토리얼 매니저 작업 흐름", new GUIStyle
        {
            fontSize = 20,
            normal =
            {
                textColor = Color.white
            }
        });
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Step: 튜토리얼 단계 / Point: 마스크로 강조할 부분");
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("1. TutorialBase 프리팹을 UnPack합니다.");
        EditorGUILayout.LabelField("2. 원하는 수만큼 Step Preset을 추가합니다.");
        EditorGUILayout.LabelField("3. Step Preset에 원하는 수만큼 Point Preset을 추가합니다.");
        EditorGUILayout.LabelField("4. 각 Point Preset에 강조되길 원하는 UI 이미지를 Target에 드래그하고 해당 이미지의 설명을 적습니다.");
        EditorGUILayout.LabelField("5. Step Preset 세팅이 완료되면 '세팅 시작'을 클릭합니다./");
        EditorGUILayout.LabelField("6. 필요한 경우 추가 수정합니다.");
        EditorGUILayout.LabelField("7. 수정이 완료되었으면 TutorialBase를 원하는 이름으로 변경하고 프리팹화합니다.");

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("<안내 사항>");
        EditorGUILayout.LabelField("※ TutorialManager는 PanelBase를 상속받지 않기때문에 독립적으로 작동합니다.");
        EditorGUILayout.LabelField("※ 작업하다 마스크가 보이지 않는다면 DimPanel 게임오브젝트를 껏다 켜주세요");
        EditorGUILayout.LabelField("※ 리스트 0번째 열이 이상하게 보이는건 에디터 2020.3.35f1의 버그입니다.");
        EditorGUILayout.LabelField("  0번째를 1번째로 옮겨서 수정하시면 됩니다.");

        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();
        base.OnInspectorGUI();

        if (GUILayout.Button("세팅 시작"))
        {
            tutorialManager.SetData();
            Undo.RecordObject(tutorialManager.gameObject, $"{tutorialManager.gameObject.name}: 튜토리얼 세팅 시작");
            EditorUtility.SetDirty(tutorialManager);
        }

        if (GUILayout.Button("다음 스텝"))
        {
            if (Application.isPlaying)
            {
                tutorialManager.NextStep();
            }
            else
            {
                Debug.Log("플레이 중에만 호출 가능합니다.");
            }
        }

        if (GUILayout.Button("튜토리얼 PlayerPrefs 삭제"))
        {
            tutorialManager.DeleteTutorialSaveData();
        }
    }

    [MenuItem("클라이언트팀/캐시 삭제/튜토리얼 진행 여부")]
    public static void ClearTutorialCache()
    {
        if (Application.isPlaying && LocalPlayerData.MemberCode != null)
        {
            PlayerPrefs.DeleteKey($"Sav_Tutorial_MyRoom_{LocalPlayerData.MemberCode}");
            PlayerPrefs.DeleteKey($"Sav_Tutorial_OfficeRoomCreate_{LocalPlayerData.MemberCode}");
            PlayerPrefs.DeleteKey($"Sav_Tutorial_OfficeRoomEnter_{LocalPlayerData.MemberCode}");
            PlayerPrefs.DeleteKey($"Sav_Tutorial_OfficeRoomHostFirstTime_{LocalPlayerData.MemberCode}");
            PlayerPrefs.DeleteKey($"Sav_Tutorial_OfficeRoomInfo_{LocalPlayerData.MemberCode}");
            PlayerPrefs.DeleteKey($"Sav_Tutorial_OfficeRoomUserInfo_{LocalPlayerData.MemberCode}");

            PlayerPrefs.Save();

            Debug.Log("튜토리얼 캐시 삭제 완료");
        }
        else
        {
            Debug.Log("플레이 상태에서 로그인을 해야 캐시 삭제가 가능합니다");
        }
    }
}
