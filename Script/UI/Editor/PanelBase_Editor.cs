using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FrameWork.UI
{
    [CustomEditor(typeof(PanelBase), true)]
    public class PanelBase_Editor : Editor
    {
        private PanelBase panel;

        private bool showAnimation = true;

        private SerializedProperty _dontSetActiveFalse;
        private SerializedProperty _Script;

        private GUIContent label_dontSetActiveFalse;

        private static bool fold_base = false;
        private static bool fold_mine = true;
        
        
        private void OnEnable()
        {
            _dontSetActiveFalse = serializedObject.FindProperty("dontSetActiveFalse");
            _Script = serializedObject.FindProperty("m_Script");
            label_dontSetActiveFalse = new GUIContent("Set Active 불가 옵션");
            fold_base = EditorPrefs.GetBool("PanelBase_FoldPanelMenu", false);
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool("PanelBase_FoldPanelMenu", fold_base);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_Script);
            GUI.enabled = true;
            
            if (!panel) panel = (PanelBase) target;

            fold_base = EditorGUILayout.Foldout(fold_base, "베이스");

            if (fold_base)
            {
                EditorGUILayout.PropertyField(_dontSetActiveFalse, label_dontSetActiveFalse);

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();

                if (GUILayout.Button("패널 열기", GUILayout.Width(120), GUILayout.Height(25)))
                {
                    if (Application.isPlaying)
                    {
                        Type panelType = panel.GetType();
                        if (typeof(PanelBase).IsAssignableFrom(panelType))
                        {
                            MethodInfo pushPanelMethod = typeof(SceneLogic).GetMethod("PushPanel").MakeGenericMethod(panelType);
                            pushPanelMethod.Invoke(SceneLogic.instance, new object[] { true });
                        }
                        //panel.PushPanel(showAnimation);
                    }
                    else
                    {
                        //Debug.LogWarning("패널 열기는 플레이 중에만 작동합니다.");
                        panel.gameObject.SetActive(true);
                    }
                }

                if (GUILayout.Button("패널 닫기", GUILayout.Width(120), GUILayout.Height(25)))
                {
                    if (Application.isPlaying)
                    {
                        if (SceneLogic.instance.PeekPanel() == this) SceneLogic.instance.PopPanel();
                    }
                    else
                    {
                        //Debug.LogWarning("패널 닫기는 플레이 중에만 작동합니다.");
                        if (!panel.dontSetActiveFalse) panel.gameObject.SetActive(false);
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();

                showAnimation = EditorGUILayout.Toggle("(에디터 전용) 애니메이션 재생 여부", showAnimation);

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            
            if (panel.dontSetActiveFalse) panel.gameObject.SetActive(true); // 끄면 안되서 강제로 킵니다.

            GUILayout.Space(5);
            
            fold_mine = EditorGUILayout.Foldout(fold_mine, "본 컴포넌트 변수");
            
            if (fold_mine)
            {
                DrawPropertiesExcluding(serializedObject, "m_Script", "dontSetActiveFalse");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
