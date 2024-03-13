using db;
using FrameWork.UI;
using Office;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Office
{
    /// <summary>
    /// 오피스공간 토글들, 선택은 안됨
    /// </summary>
    public class Item_OfficeSpace : MonoBehaviour
    {
        [HideInInspector] public TogglePlus togglePlus;
        [HideInInspector] public Button btn_OfficeSpace;
        
        /// <summary>
        /// 데이터 셋업
        /// </summary>
        /// <param name="togg"></param>
        /// <param name="officeSpaceInfo"></param>
        public void SetData(ToggleGroup togg, OfficeSpaceInfo officeSpaceInfo)
        {
            //토글 그룹, 기능 셋업
            togglePlus = GetComponent<TogglePlus>();
            togglePlus.tog.interactable = false;
            togglePlus.tog.group = togg;
            togglePlus.isSound = false;

            //이미지 셋업
            SetImage(officeSpaceInfo.thumbnail, "img_ThumbnailOn");
            SetImage(officeSpaceInfo.thumbnail, "img_ThumbnailOff");

            btn_OfficeSpace = Util.Search<Button>(gameObject, nameof(btn_OfficeSpace));
        }

        /// <summary>
        /// 썸네일 셋업
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <param name="prefabName"></param>
        private void SetImage(string thumbnail, string prefabName)
        {
            Image img = Util.Search<Image>(gameObject, prefabName);
            img.sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + "OfficeThumbnail/" + thumbnail);
        }
    }

}