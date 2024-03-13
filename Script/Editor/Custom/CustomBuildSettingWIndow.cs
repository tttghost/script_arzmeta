///****************************************************************************************************************
// * 
// *              BuildSettingWIndow
// * 
///****************************************************************************************************************/
//using System.Text;
//using UnityEngine;
//using UnityEditor;

//public enum COLOR_TYPE
//{
//    Color,
//    BackGround
//};

//public class CustomBuildSettingWIndow : EditorWindow
//{
//    [MenuItem("클라이언트팀/커스텀프로젝트/빌드설정 윈도우", priority = 1)]
//    public static void ShowWindow()
//    {
//        CustomBuildSettingWIndow window = GetWindow<CustomBuildSettingWIndow>(false, "CustomBuildSettingWIndow", true);
//        //DontDestroyOnLoad( window );
//    }

//    private CustomBuildSetting _projSetting;

//    private string _appVersion = "0.0.0";
//    private int _bundleVersion = 1;
//    private string _innerBuildNo = "0";
//    private string _lastBuildVersion = "0.0.0.0";

//    private StringBuilder sb = new StringBuilder();

//    void OnEnable()
//    {
//        if (_projSetting == null)
//        {
//            _projSetting = CustomBuildSetting.Load();
//        }

//        _appVersion = _projSetting._appVersion;
//        _bundleVersion = _projSetting._bundleVersionCode;
//        _innerBuildNo = _projSetting._innerBuildNo;
//        _lastBuildVersion = _projSetting._lastBuildVersion;
//    }

//    void OnGUI()
//    {
//        EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
//        {
//            GUILayout.Space(10);

//            SetGUIColor(COLOR_TYPE.Color, Color.green);
//            SetGUIColor(COLOR_TYPE.BackGround, Color.green);

//            if (_projSetting == null)
//            {
//                _projSetting = CustomBuildSetting.Load();
//                return;
//            }
//#if UNITY_ANDROID
//            GUILayout.Label("--- Android 빌드 설정 ---");

//            GUILayout.Space(5);

//            GUILayout.BeginHorizontal();
//            {
//                EditorGUI.BeginDisabledGroup(true);
//                CommonFunctionEditor.SetStringField("키스토어 이름", PlayerSettings.Android.keystoreName);
//                EditorGUI.EndDisabledGroup();

//                if (GUILayout.Button(new GUIContent("파일 열기", "키스토어 파일을 오픈 합니다")))
//                {
//                    string path = Application.dataPath;
//                    path = path.Replace("Assets", "");

//                    string folderPath = EditorUtility.OpenFilePanel("키스토어 파일 지정", path, "keystore");
//                    if (string.IsNullOrEmpty(folderPath) == false)
//                    {
//                        string[] splits = folderPath.Split('/');
//                        PlayerSettings.Android.keystoreName = splits[splits.Length - 1];
//                        Debug.Log("키스토어 이름 = " + splits[splits.Length - 1]);
//                    }
//                }

//                GUILayout.EndHorizontal();
//            }

//            PlayerSettings.Android.keystorePass = CommonFunctionEditor.SetStringField("키스토어 비밀번호", PlayerSettings.Android.keystorePass);
//            if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass))
//                PlayerSettings.Android.keystorePass = "arzmeta";

//            GUILayout.Space(5);

//            PlayerSettings.Android.keyaliasName = CommonFunctionEditor.SetStringField("키 Alias 이름", PlayerSettings.Android.keyaliasName);
//            if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName))
//                PlayerSettings.Android.keyaliasName = "arzmeta";

//            PlayerSettings.Android.keyaliasPass = CommonFunctionEditor.SetStringField("키 Alias 비밀번호", PlayerSettings.Android.keyaliasPass);
//            if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
//                PlayerSettings.Android.keyaliasPass = "arzmeta";

//            GUILayout.Space(10);

//            GUILayout.Label("--- Android 빌드 버전 설정 ---");
//            GUILayout.Space(5);

//            //////////////////////////////////////////////////////////////////////////////////////////////////
//            EditorGUILayout.BeginHorizontal();
//            {
//                GUILayout.Space(10);

//                // 앱 버전(빌드버전)1
//                _appVersion = CommonFunctionEditor.SetStringField("앱 버전", _appVersion);
//                if (_projSetting != null)
//                {
//                    if (_appVersion.Equals(PlayerSettings.bundleVersion) == false)
//                    {
//                        _projSetting._appVersion = _appVersion;

//                        PlayerSettings.bundleVersion = _projSetting._appVersion;

//                        _projSetting.UpdateVersion();
//                    }
//                }

//                // 번들 버전 코드
//                _bundleVersion = CommonFunctionEditor.SetIntField("버전코드", _bundleVersion);
//                if (_projSetting != null)
//                {
//                    if (_bundleVersion != _projSetting._bundleVersionCode)
//                    {
//                        _projSetting._bundleVersionCode = _bundleVersion;

//                        PlayerSettings.Android.bundleVersionCode = _projSetting._bundleVersionCode;

//                        _projSetting.UpdateVersion();
//                    }
//                }

//                EditorGUILayout.EndHorizontal();
//            }

//            GUILayout.Space(5);

//            //////////////////////////////////////////////////////////////////////////////////////////////////
//            // 내부 빌드 넘버
//            _innerBuildNo = CommonFunctionEditor.SetStringField("내부 빌드넘버", _innerBuildNo);
//            if (_projSetting != null)
//            {
//                if (_innerBuildNo.Equals(_projSetting._innerBuildNo) == false)
//                {
//                    _projSetting._innerBuildNo = _innerBuildNo;
//                    _projSetting.UpdateVersion();
//                }
//            }

//#elif UNITY_IOS
//            GUILayout.Label( "--- iOS 빌드 설정 ---" );

//            PlayerSettings.iOS.appleDeveloperTeamID = CommonFunctionEditor.SetStringField( "Team ID", PlayerSettings.iOS.appleDeveloperTeamID );

//            EditorGUILayout.BeginHorizontal();
//            {
//                _appVersion = CommonFunctionEditor.SetStringField( "앱 버전", _appVersion );
//                if (_projSetting != null)
//                {
//                    if (_appVersion.Equals(_projSetting._appVersion) == false)
//                    {
//                        _projSetting._appVersion = _appVersion;
//                        PlayerSettings.bundleVersion = _projSetting._appVersion;
//                        _projSetting.UpdateVersion();
//                    }
//                }

//                _bundleVersion = CommonFunctionEditor.SetIntField( "빌드넘버", _bundleVersion );
//                if ( _projSetting != null )
//                {
//                    if ( _bundleVersion != _projSetting._bundleVersionCode )
//                    {
//                        _projSetting._bundleVersionCode = _bundleVersion;
//                        PlayerSettings.iOS.buildNumber = _projSetting._bundleVersionCode.ToString();
//                    }
//                }

//                EditorGUILayout.EndHorizontal();
//            }

//            // 내부 빌드넘버
//            _innerBuildNo = CommonFunctionEditor.SetStringField( "내부 빌드넘버", _innerBuildNo );
//            if ( _projSetting != null )
//            {
//                if ( _innerBuildNo.Equals( _projSetting._innerBuildNo ) == false )
//                {
//                    _projSetting._innerBuildNo = _innerBuildNo;
//                    _projSetting.UpdateVersion();
//                }
//            }
//#endif
//#if UNITY_STANDALONE
//            PlayerSettings.productName = "arzMETA";
//#else
//            PlayerSettings.productName = "아즈메타";
//#endif
//            GUILayout.Space(10);

//            EditorGUI.EndDisabledGroup();

//            // 최종 클라이언트 버전(빌드버전 + 내부 빌드넘버)
//            ClearGUIColor();
//            SetGUIColor(COLOR_TYPE.Color, Color.yellow);
//            SetGUIColor(COLOR_TYPE.BackGround, Color.yellow);

//            EditorGUI.BeginDisabledGroup(true);
//            {
//                if (_projSetting != null)
//                {
//                    _projSetting.UpdateVersion();
//                    _lastBuildVersion = _projSetting._lastBuildVersion;
//                }

//                CommonFunctionEditor.SetStringField("클라이언트 버전", _lastBuildVersion);

//                EditorGUI.EndDisabledGroup();
//            }

//            GUILayout.Space(5);

//            ClearGUIColor();
//            SetGUIColor(COLOR_TYPE.Color, Color.green);
//            SetGUIColor(COLOR_TYPE.BackGround, Color.green);
//            if (GUILayout.Button(new GUIContent("빌드설정 저장", "프로젝트 설정이 저장된 스크립터블 오브젝트를 본다"), GUILayout.Height(25.0f)))
//            {
//                AssetDatabase.SaveAssets();
//                AssetDatabase.Refresh();
//                var obj = AssetDatabase.LoadMainAssetAtPath(Cons.Path_CustomProjectSetting);
//                if (obj != null)
//                {
//                    EditorUtility.SetDirty(obj);
//                    AssetDatabase.SaveAssets();
//                    AssetDatabase.Refresh();
//                    //Selection.activeObject = obj;
//                }
//            }

//        } // EditorGUI.BeginDisabledGroup( EditorApplication.isCompiling );
//    }

//    public static void SetGUIColor(COLOR_TYPE eType, Color color)
//    {
//        switch (eType)
//        {
//            case COLOR_TYPE.Color: GUI.color = color; break;
//            case COLOR_TYPE.BackGround: GUI.backgroundColor = color; break;
//            default:
//                return;
//        }
//    }

//    public static void ClearGUIColor()
//    {
//        GUI.color = Color.white;
//        GUI.backgroundColor = Color.white;
//    }
//}


/****************************************************************************************************************
 * 
 *              BuildSettingWIndow
 * 
/****************************************************************************************************************/
using System.Text;
using UnityEngine;
using UnityEditor;

public enum COLOR_TYPE
{
    Color,
    BackGround
};

public class CustomBuildSettingWIndow : EditorWindow
{
    [MenuItem("클라이언트팀/커스텀프로젝트/빌드설정 윈도우", priority = 1)]
    public static void ShowWindow()
    {
        CustomBuildSettingWIndow window = GetWindow<CustomBuildSettingWIndow>(false, "CustomBuildSettingWIndow", true);
        //DontDestroyOnLoad( window );
    }

    private CustomBuildSetting _projSetting;

    private string _appVersion = "0.0.0";
    private int _bundleVersion = 1;
    private string _innerBuildNo = "0";
    private string _lastBuildVersion = "0.0.0.0";

    private StringBuilder sb = new StringBuilder();

    void OnEnable()
    {
        if (_projSetting == null)
        {
            _projSetting = CustomBuildSetting.Load();
        }

        _appVersion = _projSetting._appVersion;
        _bundleVersion = _projSetting._bundleVersionCode;
        _innerBuildNo = _projSetting._innerBuildNo;
        _lastBuildVersion = _projSetting._lastBuildVersion;
    }

    void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
        {
            GUILayout.Space(10);

            SetGUIColor(COLOR_TYPE.Color, Color.green);
            SetGUIColor(COLOR_TYPE.BackGround, Color.green);

            if (_projSetting == null)
            {
                _projSetting = CustomBuildSetting.Load();
                return;
            }
#if UNITY_ANDROID
            GUILayout.Label("--- Android 빌드 설정 ---");

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(true);
                CommonFunctionEditor.SetStringField("키스토어 이름", PlayerSettings.Android.keystoreName);
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button(new GUIContent("파일 열기", "키스토어 파일을 오픈 합니다")))
                {
                    string path = Application.dataPath;
                    path = path.Replace("Assets", "");

                    string folderPath = EditorUtility.OpenFilePanel("키스토어 파일 지정", path, "keystore");
                    if (string.IsNullOrEmpty(folderPath) == false)
                    {
                        string[] splits = folderPath.Split('/');
                        PlayerSettings.Android.keystoreName = splits[splits.Length - 1];
                        Debug.Log("키스토어 이름 = " + splits[splits.Length - 1]);
                    }
                }

                GUILayout.EndHorizontal();
            }

            PlayerSettings.Android.keystorePass = CommonFunctionEditor.SetStringField("키스토어 비밀번호", PlayerSettings.Android.keystorePass);
            if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass))
                PlayerSettings.Android.keystorePass = "arzmeta";

            GUILayout.Space(5);

            PlayerSettings.Android.keyaliasName = CommonFunctionEditor.SetStringField("키 Alias 이름", PlayerSettings.Android.keyaliasName);
            if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName))
                PlayerSettings.Android.keyaliasName = "arzmeta";

            PlayerSettings.Android.keyaliasPass = CommonFunctionEditor.SetStringField("키 Alias 비밀번호", PlayerSettings.Android.keyaliasPass);
            if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
                PlayerSettings.Android.keyaliasPass = "arzmeta";

            GUILayout.Space(10);

            GUILayout.Label("--- Android 빌드 버전 설정 ---");
            GUILayout.Space(5);

            //////////////////////////////////////////////////////////////////////////////////////////////////
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);

                // 앱 버전(빌드버전)1
                _appVersion = CommonFunctionEditor.SetStringField("앱 버전", _appVersion);
                if (_projSetting != null)
                {
                    if (_appVersion.Equals(PlayerSettings.bundleVersion) == false)
                    {
                        _projSetting._appVersion = _appVersion;

                        PlayerSettings.bundleVersion = _projSetting._appVersion;

                        _projSetting.UpdateVersion();
                    }
                }

                // 번들 버전 코드
                _bundleVersion = CommonFunctionEditor.SetIntField("버전코드", _bundleVersion);
                if (_projSetting != null)
                {
                    if (_bundleVersion != _projSetting._bundleVersionCode)
                    {
                        _projSetting._bundleVersionCode = _bundleVersion;

                        PlayerSettings.Android.bundleVersionCode = _projSetting._bundleVersionCode;

                        _projSetting.UpdateVersion();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // 내부 빌드 넘버
            _innerBuildNo = CommonFunctionEditor.SetStringField("내부 빌드넘버", _innerBuildNo);
            if (_projSetting != null)
            {
                if (_innerBuildNo.Equals(_projSetting._innerBuildNo) == false)
                {
                    _projSetting._innerBuildNo = _innerBuildNo;
                    _projSetting.UpdateVersion();
                }
            }

#elif UNITY_IOS
            GUILayout.Label( "--- iOS 빌드 설정 ---" );

            PlayerSettings.iOS.appleDeveloperTeamID = CommonFunctionEditor.SetStringField( "Team ID", PlayerSettings.iOS.appleDeveloperTeamID );

            EditorGUILayout.BeginHorizontal();
            {
                _appVersion = CommonFunctionEditor.SetStringField( "앱 버전", _appVersion );
                if (_projSetting != null)
                {
                    if (_appVersion.Equals(_projSetting._appVersion) == false)
                    {
                        _projSetting._appVersion = _appVersion;
                        PlayerSettings.bundleVersion = _projSetting._appVersion;
                        _projSetting.UpdateVersion();
                    }
                }

                _bundleVersion = CommonFunctionEditor.SetIntField( "빌드넘버", _bundleVersion );
                if ( _projSetting != null )
                {
                    if ( _bundleVersion != _projSetting._bundleVersionCode )
                    {
                        _projSetting._bundleVersionCode = _bundleVersion;
                        PlayerSettings.iOS.buildNumber = _projSetting._bundleVersionCode.ToString();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // 내부 빌드넘버
            _innerBuildNo = CommonFunctionEditor.SetStringField( "내부 빌드넘버", _innerBuildNo );
            if ( _projSetting != null )
            {
                if ( _innerBuildNo.Equals( _projSetting._innerBuildNo ) == false )
                {
                    _projSetting._innerBuildNo = _innerBuildNo;
                    _projSetting.UpdateVersion();
                }
            }
#endif
#if UNITY_STANDALONE

            GUILayout.Label("--- PC 빌드 설정 ---");
            EditorGUILayout.BeginHorizontal();
            PlayerSettings.productName = "arzMETA";

            _appVersion = CommonFunctionEditor.SetStringField("PC버전", _appVersion);
            if (_projSetting != null)
            {
                if (_appVersion.Equals(_projSetting._appVersion) == false)
                {
                    _projSetting._appVersion = _appVersion;
                    PlayerSettings.bundleVersion = _projSetting._appVersion;
                    _projSetting.UpdateVersion();
                }
            }

            _bundleVersion = CommonFunctionEditor.SetIntField("빌드넘버", _bundleVersion);
            if (_projSetting != null)
            {
                if (_bundleVersion != _projSetting._bundleVersionCode)
                {
                    _projSetting._bundleVersionCode = _bundleVersion;
                    PlayerSettings.iOS.buildNumber = _projSetting._bundleVersionCode.ToString();
                }
            }
            EditorGUILayout.EndHorizontal();


#else
            PlayerSettings.productName = "아즈메타";
#endif
            GUILayout.Space(10);

            EditorGUI.EndDisabledGroup();

            // 최종 클라이언트 버전(빌드버전 + 내부 빌드넘버)
            ClearGUIColor();
            SetGUIColor(COLOR_TYPE.Color, Color.green);
            SetGUIColor(COLOR_TYPE.BackGround, Color.white);

            EditorGUI.BeginDisabledGroup(true);
            {
                if (_projSetting != null)
                {
                    _projSetting.UpdateVersion();
                    _lastBuildVersion = _projSetting._lastBuildVersion;
                }

                CommonFunctionEditor.SetStringField("클라이언트 버전", _lastBuildVersion);

                EditorGUI.EndDisabledGroup();
            }



            GUILayout.Space(5);

            ClearGUIColor();
            SetGUIColor(COLOR_TYPE.Color, Color.green);
            SetGUIColor(COLOR_TYPE.BackGround, Color.green);
            if (GUILayout.Button(new GUIContent("빌드설정 저장", "프로젝트 설정이 저장된 스크립터블 오브젝트를 본다"), GUILayout.Height(25.0f)))
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                var obj = AssetDatabase.LoadMainAssetAtPath(Cons.Path_CustomProjectSetting);
                if (obj != null)
                {
                    EditorUtility.SetDirty(obj);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    //Selection.activeObject = obj;
                }
            }

        } // EditorGUI.BeginDisabledGroup( EditorApplication.isCompiling );
    }

    public static void SetGUIColor(COLOR_TYPE eType, Color color)
    {
        switch (eType)
        {
            case COLOR_TYPE.Color: GUI.color = color; break;
            case COLOR_TYPE.BackGround: GUI.backgroundColor = color; break;
            default:
                return;
        }
    }

    public static void ClearGUIColor()
    {
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
    }
}
