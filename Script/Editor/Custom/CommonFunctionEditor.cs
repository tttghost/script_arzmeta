/*******************************************************************************************************
 * 
 *		CommonFunctionEditor.cs
 *			- 에디터 확장(에디터 툴기능 추가) 시 변수 타입별 입력 필드 함수 지원
 *			- Toogle, Slider, 구분선, 라인 그려주는 함수 지원
 * 
 *******************************************************************************************************/
using UnityEditor;
using UnityEngine;
using System;
using System.Text;

public class CommonFunctionEditor
{
    private static StringBuilder sb = new StringBuilder();

    public static int SetIntField(string title, int value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(title);

        int index = 0;
        index = EditorGUILayout.IntField(value);

        GUILayout.EndHorizontal();

        return index;
    }

    public static float SetFloatField(string title, float value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(title);

        float index = 0;
        index = EditorGUILayout.FloatField(value);

        GUILayout.EndHorizontal();

        return index;
    }

    public static string SetStringField(string title, string value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(title);

        string temp = value;
        temp = EditorGUILayout.TextField(value);
        GUILayout.EndHorizontal();

        return temp;
    }

    public static Enum SetEnumField(string title, Enum value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(title);

        value = EditorGUILayout.EnumPopup(value);

        GUILayout.EndHorizontal();
        return value;
    }

    public static int SetIntPopupField(string title, string[] displayedOptions, int value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(title);

        value = EditorGUILayout.Popup(value, displayedOptions);

        GUILayout.EndHorizontal();
        return value;
    }

    public static bool SetToggleField(string title, bool value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(title);

        value = EditorGUILayout.Toggle(value);

        GUILayout.EndHorizontal();

        return value;
    }

    public static bool SetToggleTitleField(string title, bool value)
    {
        GUILayout.BeginHorizontal();

        value = EditorGUILayout.Toggle(title, value);

        GUILayout.EndHorizontal();

        return value;
    }

    public static bool DrawHeader(string title, bool value = false)
    {
        GUILayout.Box(title);
        
        return true;
    }

    public static float SetSlider(string title, float value, float min_value, float max_value)
    {
        GUILayout.BeginVertical();

        GUILayout.Label(title);
        value = GUILayout.HorizontalSlider(value, min_value, max_value);

        GUILayout.EndVertical();
        return value;
    }

    public static bool TitleBar(string title, bool is_open, Color color, float width = 100f, float height = 20f)
    {
        if (is_open)
            GUI.backgroundColor = color;
        else
            GUI.backgroundColor = Color.white;

        sb.Length = 0;
        sb.AppendFormat("{0} {1}", is_open == true ? "" : "", title);
        if (GUILayout.Button(sb.ToString(), GUILayout.Width(width), GUILayout.Height(height)))
            is_open ^= true;

        GUI.backgroundColor = Color.white;
        return is_open;
    }

    public static Vector3 SetVector3Field(string title, Vector3 value)
    {
        GUILayout.BeginVertical();
        value = EditorGUILayout.Vector3Field(title, value);
        GUILayout.EndVertical();
        return value;
    }

    public static void DrawSeparator()
    {
        EditorGUILayout.Separator();
    }

    public static void DrawUILine( Color color, int thickness = 2, int padding = 10 )
    {
        Rect r = EditorGUILayout.GetControlRect( GUILayout.Height( padding + thickness ) );
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect( r, color );
    }
}
