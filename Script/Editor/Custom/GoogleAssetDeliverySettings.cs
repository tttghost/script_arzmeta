using Google.Android.AppBundle.Editor;
using Google.Android.AppBundle.Editor.AssetPacks;
using UnityEditor;

public class GoogleAssetDeliverySettings
{
    [MenuItem("클라이언트팀/GoogleCustom/AssetDeliverySettings")]
    private static void AssetDeliverySettings()
    {
        var assetPackConfig = new AssetPackConfig();
        // AssetBundle 이외의 것을 AssetPack에 포함하고 싶은 경우는 이쪽
        // 하나의 AssetPack에 지정된 폴더 아래의 모든 파일을 포함시킵니다.
        assetPackConfig.AddAssetsFolder("{AssetPack 이름}", "{대상 폴더 상대 경로}", AssetPackDeliveryMode.InstallTime);

        // AssetBundle을 개별적으로 추가하려면 여기
        // AssetPack 이름 = AssetBundle 이름이 됨
        assetPackConfig.AddAssetBundle("{AssetBundle 상대 경로}", AssetPackDeliveryMode.InstallTime);

        // 설정 저장
        AssetPackConfigSerializer.SaveConfig(assetPackConfig);
    }
}
