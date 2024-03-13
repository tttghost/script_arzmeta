﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEditor.Callbacks;
using UnityEngine;


namespace FrameWork.Build
{
	public class CustomBuildProcessor
	{
#if UNITY_IOS
#pragma warning disable 0162
        [PostProcessBuild(999)]
        public static void OnPostprocessBuild( BuildTarget target, string buildPath )
        {
            if ( target == BuildTarget.iOS )
            {
                string pbxProjectPath = PBXProject.GetPBXProjectPath( buildPath );
                string plistPath = Path.Combine( buildPath, "Info.plist" );
                PBXProject pbxProject = new PBXProject();
                pbxProject.ReadFromFile( pbxProjectPath );

#if UNITY_2019_3_OR_NEWER
                string targetGUID = pbxProject.GetUnityFrameworkTargetGuid();

#else // Unity 2019.3 이전 버전일 경우 GUID 얻어오는 방식
                
                string targetGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());

#endif

                // 필요한 라이브러리 추가, 삭제 참고 함수
                //pbxProject.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-weak_framework PhotosUI");
                //pbxProject.RemoveFrameworkFromProject(targetGUID, "Photos.framework");
                //File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());

                //---------------------------------------------------------------------------------------
                // plist 컨트롤
                //---------------------------------------------------------------------------------------
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString( File.ReadAllText( plistPath ) );
                PlistElementDict rootDict = plist.root;
                
                // 수출 규정 물어보지 않게 하기 위한 옵션.
                rootDict.SetBoolean( "ITSAppUsesNonExemptEncryption", false );

                // 백그라운드 상태일 때, 앱 재시작 시키지 않게 하기
                string exitsOnSuspend = "UIApplicationExitsOnSuspend";
                if ( rootDict.values.ContainsKey( exitsOnSuspend ) )
                {
                    rootDict.values.Remove( exitsOnSuspend );
                }

                //// 사진첩 사용권한 설명.
                //rootDict.SetString( "NSPhotoLibraryUsageDescription", PHOTO_LIBRARY_USAGE_DESCRIPTION );
                //// 사진추가 사용권한 설명.
                //rootDict.SetString( "NSPhotoLibraryAddUsageDescription", PHOTO_LIBRARY_ADDITIONS_USAGE_DESCRIPTION );
                //// 마이크 사용권한 설명.
                //rootDict.SetString( "NSMicrophoneUsageDescription", MICROPHONE_USAGE_DESCRIPTION );

                //if (DONT_ASK_LIMITED_PHOTOS_PERMISSION_AUTOMATICALLY_ON_IOS14)
                //    rootDict.SetBoolean("PHPhotoLibraryPreventAutomaticLimitedAccessAlert", true);

                File.WriteAllText( plistPath, plist.WriteToString() );
            }
        }
#pragma warning restore 0162
#endif
	}
}

/*
		//---------------------------------------------------------------------------
		//
		//  커스텀 빌드 참고 코드
		//      - 자동화 빌드 할 때 사용할 필요가 있음
		//
		//---------------------------------------------------------------------------
		// App Info
		private const string APP_NAME = "APPNAME"; //APK 명칭
		protected const string KEYSTORE_PASSWORD = "********";
		private const string BUILD_BASIC_PATH = "../../build/";
		private const string BUILD_ANDROID_PATH = BUILD_BASIC_PATH + "Android/";
		private const string BUILD_IOS_PATH = BUILD_BASIC_PATH + "Ios/";

		// IOS 권한 메세지 정보
		private const string PHOTO_LIBRARY_USAGE_DESCRIPTION = "앱과 상호 작용하려면 사진 액세스 권한이 필요합니다.";
		private const string PHOTO_LIBRARY_ADDITIONS_USAGE_DESCRIPTION = "이 앱에 미디어를 저장하려면 사진에 액세스할 수 있어야 합니다.";
		private const string MICROPHONE_USAGE_DESCRIPTION = "앱 내 음성 확인 콘텐츠를 활용하려면 마이크 권한이 필요합니다.";
		private const bool DONT_ASK_LIMITED_PHOTOS_PERMISSION_AUTOMATICALLY_ON_IOS14 = true;

		[MenuItem( "Builder/Build/BuildForAndroid" )]
		public static void BuildForAndroid()
		{
			string fileName = SetPlayerSettingsForAndroid();
			BuildPlayerOptions buildOption = new BuildPlayerOptions();
			buildOption.locationPathName = BUILD_ANDROID_PATH + fileName;
			buildOption.scenes = GetBuildSceneList();
			buildOption.target = BuildTarget.Android;
			buildOption.options = BuildOptions.AutoRunPlayer;
			BuildPipeline.BuildPlayer( buildOption );
		}

		[MenuItem( "Builder/Build/BuildForIOS" )]
		public static void BuildForIOS()
		{
			BuildPlayerOptions buildOption = new BuildPlayerOptions();
			buildOption.target = BuildTarget.iOS;
			buildOption.scenes = GetBuildSceneList();
			buildOption.locationPathName = BUILD_IOS_PATH;
			BuildPipeline.BuildPlayer( buildOption );
		}

		[MenuItem( "Builder/OpenBuildDirectory" )]
		public static void OpenBuildDirectory()
		{
			OpenFileBrowser( Path.GetFullPath( BUILD_BASIC_PATH ) );
		}

		public static void OpenFileBrowser( string path )
		{
			bool openInsidesOfFolder = false;
			if ( Directory.Exists( path ) )
			{
				openInsidesOfFolder = true;
			}

			string arguments = ( openInsidesOfFolder ? "" : "-R " ) + path;
			try
			{
				System.Diagnostics.Process.Start( "open", arguments );
			}
			catch ( Exception e )
			{
				Debug.Log( "Failed to open path : " + e.ToString() );
			}
		}

		/// <summary> 현재 빌드세팅의 Scene리스트를 받아옴.
		/// Enable이 True인 것만 받아옴.
		/// </summary> <returns></returns>
		protected static string[] GetBuildSceneList()
		{
			EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;
			List<string> listScenePath = new List<string>();
			for ( int i = 0; i < scenes.Length; i++ )
			{
				if ( scenes[i].enabled )
					listScenePath.Add( scenes[i].path );
			}

			return listScenePath.ToArray();
		}

		protected static string SetPlayerSettingsForAndroid()
		{
			PlayerSettings.Android.keystorePass = KEYSTORE_PASSWORD;
			PlayerSettings.Android.keyaliasPass = KEYSTORE_PASSWORD;
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
			string fileName = string.Format( "{0}_{1}.apk", APP_NAME, PlayerSettings.bundleVersion );
			return fileName;
		}
	}
*/
