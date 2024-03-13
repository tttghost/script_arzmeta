namespace Gpm.Ui.Sample
{
    using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
    using FrameWork.UI;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class Item_GiftboxData : InfiniteScrollData
    {
        public PostboxRes PostboxRes;

        public Item_GiftboxData(PostboxRes PostboxRes)
        {
            this.PostboxRes = PostboxRes;
        }
    }
    public class Item_Giftbox : InfiniteScrollItem
    {
        private Image img_Thumbnail;
        private TMP_Text txtmp_Count;

        private TMP_Text txtmp_Title;
        private TMP_Text txtmp_Summary;
        private TMP_Text txtmp_Desc;


        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);
            InitData();
            SetData();
        }
        private void InitData()
        {
            gameObject.SetActive(true);
            img_Thumbnail   = Util.Search<Image>(gameObject, nameof(img_Thumbnail));
            txtmp_Count     = Util.Search<TMP_Text>(gameObject, nameof(txtmp_Count));

            txtmp_Title     = Util.Search<TMP_Text>(gameObject, nameof(txtmp_Title));
            txtmp_Summary   = Util.Search<TMP_Text>(gameObject, nameof(txtmp_Summary));
            //txtmp_Desc      = Util.Search<TMP_Text>(gameObject, nameof(txtmp_Desc));
        }

        private void SetData()
        {
            Item_GiftboxData Item_GiftboxData = (Item_GiftboxData)scrollData;
            PostboxRes PostboxRes = Item_GiftboxData.PostboxRes;

            if ((WEBERROR)PostboxRes.error != WEBERROR.NET_E_SUCCESS)
            {
                return;
            }
            for (int i = 0; i < PostboxRes.postboxes.Count; i++)
            {
                //기본정보
                Postbox postbox = PostboxRes.postboxes[i];
                
                //썸네일
                int itemId = postbox.item.appendType; //아이템 Id

                db.Item firstItem = Single.MasterData.dataItem.GetData(itemId); //첫번째 아이템 정보
                string categoryFolder = Single.MasterData.dataCategoryType.GetData(firstItem.categoryType).name.Split('_').Last();

                //1번대 일반, 2번대 인테리어, 3번대 코스튬

                //case 썸네일 생성:
                string prefabPath = "";
                string prefabName = firstItem.prefab;
                GameObject itemObj = Single.Resources.Load<GameObject>(prefabPath + categoryFolder + prefabName);
                Texture2D tex = RuntimePreviewGenerator.GenerateModelPreview(itemObj.transform, 512, 512);
                Sprite spr = Util.Tex2Sprite(tex);
                img_Thumbnail.sprite = spr;
                //break;

                //case 썸네일 가져오기:
                string thumbnailPath = "";
                string thumbnailName = firstItem.thumbnail;
                Single.Resources.Load<Sprite>(thumbnailPath + categoryFolder + thumbnailName);
                //break;



                //개수
                txtmp_Count.text = postbox.item.count.ToString(); //or Postbox.items.Count.ToString(); 혹시 items의 개수인가요? 여러개일때는?
                //이름
                txtmp_Title.text = postbox.subject;
                //요약
                txtmp_Desc.text = postbox.summary;

                //for (int j = 0; j < Postbox.items.Count; j++)
                //{

                //}
            }



            //Item_GiftboxData Item_GiftboxData = (Item_GiftboxData)scrollData;
            //PostboxRes PostboxRes = Item_GiftboxData.PostboxRes;

            //img_Thumbnail.sprite = Single.MasterData.GetThumbnail(PostboxRes.itemId);
            //txtmp_Count.text = defaultGiftbox.count > 1 ? defaultGiftbox.count.ToString() : string.Empty;

            //txtmp_Title.text = defaultGiftbox.title;
            //txtmp_Summary.text = defaultGiftbox.summary;
            //txtmp_Desc.text = defaultGiftbox.desc;
        }
    }
}