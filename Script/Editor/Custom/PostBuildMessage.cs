using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PostBuildMessage : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }
    public void OnPreprocessBuild(BuildReport report)
    {
        /*
        if (EditorUtility.DisplayDialog("확인 사항", "다음 사항을 확인하셨습니까?\n\n" +
                                                 "- Addressable 빌드\n" +
                                                 "- Application Version\n" +
                                                 "- (안드로이드 한정) Bundle Version Code\n"+
                                                 "- 내부 빌드 버전 번호\n"+
                                                 "- 모든 Scene 포함 여부\n"+
                                                 "- 튜토리얼 게임오브젝트 활성화(마이룸 / 오피스)\n"
                , "네", "아니요"))
        {
            Debug.Log("빌드를 진행합니다.");
        }
        else
        {
            throw new BuildFailedException("빌드가 취소되었습니다.");
        }
        */
    }
}
