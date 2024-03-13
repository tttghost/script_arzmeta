using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;
using Assets.SimpleLocalization;
using System.Collections.Generic;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;



public class CSVDownloader
{
	/// <summary>
	/// Table id on Google Spreadsheet.
	/// Let's say your table has the following url https://docs.google.com/spreadsheets/d/1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4/edit#gid=331980525
	/// So your table id will be "1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4" and sheet id will be "331980525" (gid parameter)
	/// </summary>
	public static string TableId = "1XczhKhZsodrMp4tqUJmIWwsYKVkMGaHuqU1P8DddJ2g";

	/// <summary>
	/// Table sheet contains sheet name and id. First sheet has always zero id. Sheet name is used when saving.
	/// </summary>
	public static Sheet[] Sheets = new Sheet[] {
		new Sheet("Codegate", 921753822),
        new Sheet("NPC", 1305978018),
        new Sheet("AvatarParts", 1603349199),
        new Sheet("InfinityCodes", 406456216),
        new Sheet("Game", 686329362),
    };

	/// <summary>
	/// Folder to save spreadsheets. Must be inside Resources folder.
	/// </summary>
	//public static UnityEngine.Object SaveFolder;
	public static string FolderPath = "Assets/_CSV_DATA_";

	private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

	static EditorCoroutine coroutine = null;
	[MenuItem("클라이언트팀/로컬라이징/구글시트 CSV 다운로드(현재사용x)", priority = 2)]
	static void Sync()
	{
		if(coroutine!=null)
        {
            EditorCoroutineUtility.StopCoroutine(coroutine);
        }
		coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(SyncCoroutine());
	}

	static IEnumerator SyncCoroutine()
	{
        //var FolderPath = AssetDatabase.GetAssetPath(SaveFolder);
		Debug.Log("<color=yellow>Sync started, please wait for confirmation message...</color>");
		var dict = new Dictionary<string, UnityWebRequest>();
		if(!System.IO.Directory.Exists(FolderPath))
        {
			System.IO.Directory.CreateDirectory(FolderPath);
		}
        foreach (var sheet in Sheets)
		{
			var url = string.Format(UrlPattern, TableId, sheet.Id);

			Debug.LogFormat("Downloading: {0}...", url);

			dict.Add(url, UnityWebRequest.Get(url));
		}
        foreach (var entry in dict)
		{
			var url = entry.Key;
			var request = entry.Value;

			if (!request.isDone)
			{
				yield return request.SendWebRequest();
			}

			if (request.error == null)
			{
				var sheet = Sheets.Single(i => url == string.Format(UrlPattern, TableId, i.Id));
				var path = System.IO.Path.Combine(FolderPath, sheet.Name + ".csv");

				System.IO.File.WriteAllBytes(path, request.downloadHandler.data);
				Debug.LogFormat("Sheet {0} downloaded to {1}", sheet.Id, path);
			}
			else
			{
				throw new Exception(request.error);
			}
		}

		Debug.Log("<color=green>Localization successfully synced!</color>");
	}

}
