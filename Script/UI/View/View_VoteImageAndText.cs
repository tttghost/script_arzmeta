using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View_VoteImageAndText : View_VoteTextOnly
{
    #region 변수
    Image img_Example;
    Sprite sprite;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Image
        img_Example = GetUI_Img("img_Example");
        #endregion
    }

    #region VoteImageAndText
    protected override void InitData(GetVoteInfoPacketRes voteData)
    {
        if (img_Example != null)
        {
            // 이미지 넣기
            string url = Single.Web.StorageUrl + "/vote/" + voteData.voteInfo.id + "/" + voteData.voteInfo.imageName;
            StartCoroutine(ImageDownloadUtility.DownloadTexture(url, (texture) =>
             {
                 // 현재 이미지 기준으로 높이 고정
                 // 가로는 이미지 해상도에 따라 변경
                 Debug.Log("texture.width" + texture.width);
                 Debug.Log("texture.height" + texture.height);
                 // 되는지 확인
                 if (texture != null)
                 {
                     RectTransform rect = img_Example.GetComponent<RectTransform>();
                     float width = texture.width * (rect.rect.height / texture.height);
                     rect.sizeDelta = new Vector2(width, rect.rect.height);
                     img_Example.sprite = sprite = Util.Tex2Sprite((Texture2D)texture);
                 }
                 else
                 {
                     Debug.Log("[View_VoteImageAndText] 이미지를 가져오지 못했습니다! : " + url);
                 }
             }));
        }
        base.InitData(voteData);
    }

    private void OnDestroy()
    {
        if (sprite)
            Destroy(sprite);
    }
    #endregion
}
