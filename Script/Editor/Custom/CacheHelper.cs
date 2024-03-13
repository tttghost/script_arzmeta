using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Vuplex.WebView;

public static class CacheHelper
{
    [MenuItem("클라이언트팀/캐시 삭제/웹뷰 캐시 삭제", priority = 0)]
    public static void ClearWebviewCache()
    {
        var path = $"{Application.persistentDataPath}/Vuplex.WebView/";
        
        Web.ClearAllData();
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
    
    [MenuItem("클라이언트팀/캐시 삭제/북마크 데이터 삭제", priority = 0)]
    public static void ClearBookmarkData()
    {
        var imagePath = $"{Application.persistentDataPath}/Bookmark/";
        var jsonPath = $"{Application.persistentDataPath}/browserData.bd";
        
        if (Directory.Exists(imagePath))
        {
            Directory.Delete(imagePath, true);
        }
        
        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
        }
    }

    [MenuItem("클라이언트팀/캐시 삭제/자동로그인 해제")]
    public static void DeleteAccount()
    {
        LocalPlayerData.ResetData();
    }

    [MenuItem("클라이언트팀/캐시 삭제/고도원 아침편지 읽은이력 삭제")]
    public static void DeleteGodowon()
    {
        LocalPlayerData.Method.lastCheckTime = string.Empty;
    }
}
