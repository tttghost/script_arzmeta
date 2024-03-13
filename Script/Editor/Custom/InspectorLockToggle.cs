using UnityEditor;
using UnityEngine;

public class InspectorLockToggle
{
    static GameObject selectedGameobject;
    [MenuItem("CustomEditor/Toggle Lock &q")]
    static void ToggleInspectorLock()
    {
        if (ActiveEditorTracker.sharedTracker.isLocked == false)
        {
            selectedGameobject = Selection.activeGameObject;
        }
        else
        {
            Selection.activeGameObject = selectedGameobject;
        }
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}
