using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomContextMenu
{
    [UnityEditor.MenuItem("GameObject/Copy Hierarchy Path", false, 0)]
    private static void CopyHierarchyPath(UnityEditor.MenuCommand menuCommand)
    {
        var go = menuCommand.context as GameObject;
        var path = go.transform.GetPath();
        GUIUtility.systemCopyBuffer = path;
        Debug.Log(path);
    }
}
