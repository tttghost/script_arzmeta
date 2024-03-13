using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.Google;
using UnityEditor.Localization.Reporting;
using UnityEngine;

public class Localization_GoogleSheets : MonoBehaviour
{
    [MenuItem("클라이언트팀/로컬라이징/구글시트 로컬라이징(arzmeta)", priority = 1)]
    public static void PullAllExtensions()
    {
        Debug.Log("<color=yellow>구글시트 시작</color>");
        // Get every String Table Collection
        var stringTableCollections = LocalizationEditorSettings.GetStringTableCollections();

        foreach (var collection in stringTableCollections)
        {
            // Its possible a String Table Collection may have more than one GoogleSheetsExtension.
            // For example if each Locale we pushed/pulled from a different sheet.
            foreach (var extension in collection.Extensions)
            {
                if (extension is GoogleSheetsExtension googleExtension)
                {
                    PullExtension(googleExtension);
                }
            }
        }
        Debug.Log("<color=green>구글시트 갱신 완료</color>");
    }

    private static void PullExtension(GoogleSheetsExtension googleExtension)
    {
        // Setup the connection to Google
        var googleSheets = new GoogleSheets(googleExtension.SheetsServiceProvider);
        googleSheets.SpreadSheetId = googleExtension.SpreadsheetId;

        // Now update the collection. We can pass in an optional ProgressBarReporter so that we can updates in the Editor.
        googleSheets.PullIntoStringTableCollection(googleExtension.SheetId, googleExtension.TargetCollection as StringTableCollection,
            googleExtension.Columns, reporter: new ProgressBarReporter());
    }
}
