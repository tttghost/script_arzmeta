
using UnityEngine;
using UnityEditor;

public class CopyPastePositionAndRotation : ScriptableObject
{
    [MenuItem("Tools/Copy Position and Rotation")]
    static void CopyPositionAndRotation()
    {
        // 에디터 모드에서만 작동하기 때문에, Selection.gameObjects로 게임 오브젝트를 선택해줍니다.
        GameObject[] selection = Selection.gameObjects;
        if (selection.Length > 0)
        {
            // 현재 선택한 게임 오브젝트의 포지션과 로테이션 값을 저장합니다.
            Vector3 pos = selection[0].transform.position;
            Quaternion rot = selection[0].transform.rotation;

            // 클립보드에 저장된 값으로 붙여넣기 가능하도록 해줍니다.
            EditorGUIUtility.systemCopyBuffer = string.Format("new Vector3({0}, {1}, {2}), Quaternion.Euler(new Vector3({3}, {4}, {5}))", pos.x, pos.y, pos.z, rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z);
        }
    }

    [MenuItem("Tools/Paste Position and Rotation")]
    static void PastePositionAndRotation()
    {
        // 붙여넣기를 위해 클립보드에 저장된 값을 가져옵니다.
        string buffer = EditorGUIUtility.systemCopyBuffer;

        // "new Vector3(x, y, z), Quaternion.Euler(new Vector3(rx, ry, rz))"
        string pattern = @"new Vector3\(([\-\d\.f]+), ([\-\d\.f]+), ([\-\d\.f]+)\), *Quaternion\.Euler\(new Vector3\(([\-\d\.f]+), ([\-\d\.f]+), ([\-\d\.f]+)\)\)";

        // 가져온 값이 예상과 동일한 패턴인지 확인합니다.
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(buffer, pattern);
        if (match.Success)
        {
            float posX = float.Parse(match.Groups[1].Value);
            float posY = float.Parse(match.Groups[2].Value);
            float posZ = float.Parse(match.Groups[3].Value);
            float rotX = float.Parse(match.Groups[4].Value);
            float rotY = float.Parse(match.Groups[5].Value);
            float rotZ = float.Parse(match.Groups[6].Value);

            // 현재 선택한 게임 오브젝트에 값을 적용합니다.
            GameObject[] selection = Selection.gameObjects;
            foreach (GameObject sel in selection)
            {
                sel.transform.position = new Vector3(posX, posY, posZ);
                sel.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, rotZ));
            }
        }
    }




    [MenuItem("Tools/Copy Positions and Rotations %&c")]
    static void CopyPositionsAndRotations()
    {
        // 에디터 모드에서만 작동하기 때문에, Selection.gameObjects로 게임 오브젝트를 선택해줍니다.
        GameObject[] selection = Selection.gameObjects;
        if (selection.Length > 0)
        {
            // 현재 선택한 게임 오브젝트의 포지션과 로테이션 값을 저장합니다.
            Vector3[] pos = new Vector3[selection.Length];
            Quaternion[] rot = new Quaternion[selection.Length];

            for (int i = 0; i < selection.Length; i++)
            {
                pos[i] = selection[i].transform.position;
                rot[i] = selection[i].transform.rotation;
            }

            // 클립보드에 저장된 값으로 붙여넣기 가능하도록 해줍니다.
            EditorGUIUtility.systemCopyBuffer = "CopyPastePositionAndRotation:\n";

            for (int i = 0; i < selection.Length; i++)
            {
                string txt = string.Format("{0}\tnew Vector3({1}, {2}, {3}), Quaternion.Euler(new Vector3({4}, {5}, {6}))\n", selection[i].name, pos[i].x, pos[i].y, pos[i].z, rot[i].eulerAngles.x, rot[i].eulerAngles.y, rot[i].eulerAngles.z);

                EditorGUIUtility.systemCopyBuffer += txt;
            }
        }
    }

    [MenuItem("Tools/Paste Positions and Rotations %&v")]
    static void PastePositionsAndRotations()
    {
        // 붙여넣기를 위해 클립보드에 저장된 값을 가져옵니다.
        string buffer = EditorGUIUtility.systemCopyBuffer;
        string[] lines = buffer.Split('\n');

        GameObject[] selection = Selection.gameObjects;

        if (lines.Length - 2 != selection.Length)
        {
            DEBUG.LOG("복사길이 다름");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string trim = lines[i].Trim();

            if (trim.Length != 0)
            {
                // "gameObjectName	new Vector3(x, y, z), Quaternion.Euler(new Vector3(rx, ry, rz))"
                string[] tokens = trim.Split('\t');

                // tokens[0]에는 게임 오브젝트 이름, tokens[1]에는 저장된 포지션 및 로테이션 정보가 들어갑니다.
                string pattern = @"new Vector3\(([\-\d\.f]+), ([\-\d\.f]+), ([\-\d\.f]+)\), *Quaternion\.Euler\(new Vector3\(([\-\d\.f]+), ([\-\d\.f]+), ([\-\d\.f]+)\)\)";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(tokens[1], pattern);

                if (match.Success)
                {
                    float posX = float.Parse(match.Groups[1].Value);
                    float posY = float.Parse(match.Groups[2].Value);
                    float posZ = float.Parse(match.Groups[3].Value);
                    float rotX = float.Parse(match.Groups[4].Value);
                    float rotY = float.Parse(match.Groups[5].Value);
                    float rotZ = float.Parse(match.Groups[6].Value);

                    // 이름으로 해당 게임오브젝트를 찾습니다.
                    //GameObject sel = GameObject.Find(tokens[0]);
                    GameObject sel = selection[i - 1];
                    if (sel != null)
                    {
                        sel.transform.position = new Vector3(posX, posY, posZ);
                        sel.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, rotZ));
                    }
                }
            }
        }
    }
}
