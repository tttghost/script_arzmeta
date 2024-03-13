using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using db;
using TMPro;
using Lean.Common;

public class Panel_GiftboxGet : PanelBase
{
    private Image img_GiftboxThumanail;
    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_Count;
    private Button btn_Next;
    private Button btn_Skip;
    private GameObject go_AvatarView;
    private GameObject go_InteriorView;
    private GameObject go_AvatarGroup;
    private GameObject go_InteriorGroup;

    private Queue<View_Giftbox.GiftItemInfo> effectItems = new Queue<View_Giftbox.GiftItemInfo>();    // 획득시 오픈 연출이 존재하는 아이템 컨테이너
    private AvatarPartsController selector;
    private Animator anim_Avatar; // 아바타 애니메이터
    private LeanDragPanel dragPanel;
    private Animation[] panelAnims;
    private InteriorRTView interiorRTView;

    #region override
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        img_GiftboxThumanail = GetUI_Img("img_GiftboxThumanail");
        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
        txtmp_Count = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count));
        btn_Next = GetUI_Button(nameof(btn_Next), OnClickNext);
        btn_Skip = GetUI_Button(nameof(btn_Skip), OnClickSkip);

        #region 아바타 렌더텍스처
        go_AvatarView = GameObject.Find(nameof(go_AvatarView));
        if (go_AvatarView)
        {
            selector = go_AvatarView.GetComponent<AvatarPartsController>();
            anim_Avatar = Util.Search<Animator>(go_AvatarView, "AvatarParts");
            if (anim_Avatar != null)
            {
                selector.SetTarget(anim_Avatar.transform);
            }

            go_AvatarGroup = GetChildGObject(nameof(go_AvatarGroup));
            dragPanel = go_AvatarGroup.GetComponentInChildren<LeanDragPanel>();
            dragPanel.rotate = go_AvatarView.GetComponentInChildren<LeanManualRotate>();
        }

        panelAnims = GetComponentsInChildren<Animation>();
        #endregion

        #region 가구 렌더텍스처
        go_InteriorView = GameObject.Find(nameof(go_InteriorView));
        if (go_InteriorView)
        {
            interiorRTView = go_InteriorView.GetComponentInChildren<InteriorRTView>();
            go_InteriorGroup = GetChildGObject(nameof(go_InteriorGroup));
        }
        #endregion
    }

    public override void Back(int cnt)
    {
        base.Back(cnt);

        effectItems.Clear();
        interiorRTView.Hide();

        // 렌더텍스처 아바타 코스튬 장착중인 코스튬으로 원복 및 방향 리셋
        if (selector != null)
            selector.SetAvatarParts(LocalPlayerData.AvatarInfo);
        if (dragPanel != null)
            dragPanel.ResetRotate();
    }
    #endregion

    private void OnDestroy()
    {
        effectItems.Clear();
        effectItems = null;
    }

    public void SetData(Queue<View_Giftbox.GiftItemInfo> itemQueue)
    {
        effectItems = itemQueue;

        Refresh(itemQueue.Dequeue());
    }

    void Refresh(View_Giftbox.GiftItemInfo itemInfo)
    {
        go_AvatarGroup.SetActive(false);
        go_InteriorGroup.SetActive(false);
        dragPanel.enabled = false;

        img_GiftboxThumanail.sprite = Util.GetItemIconSprite(itemInfo.giftItemId);

        if (Single.MasterData.TryGetItem(itemInfo.giftItemId, out Item itemDb))
        {
            // 효과음 출력
            Single.Sound.PlayEffect("effect_coin_0");

            txtmp_Count.text = string.Format("x{0}", itemInfo.count);
            Util.SetMasterLocalizing(txtmp_Title, new MasterLocalData(itemDb.name));

            if (itemDb.itemType == (int)ITEM_TYPE.COSTUME)
            {
                go_AvatarGroup.SetActive(true);

                var type = Single.ItemData.CategoryToAvatar((CATEGORY_TYPE)itemDb.categoryType);
                selector.WearSelectParts(type, itemDb.prefab, Single.ItemData.GetMaterials(itemDb.id)); // 2. 아이템 착용

                ItemUseEffect itemUseEffect = Single.MasterData.dataItemUseEffect.GetData(itemDb.id);
                if (itemUseEffect != null)
                {
                    PlayAvatarAnimation(itemUseEffect.animationName); // 4. 애니메이션 실행
                }

                dragPanel.enabled = true;
                dragPanel.ResetRotate();
            }
            else if (itemDb.itemType == (int)ITEM_TYPE.INTERIOR)
            {
                go_InteriorGroup.SetActive(true);

                interiorRTView.Show(itemDb);
                interiorRTView.StartRotate();
            }
        }
    }

    #region 아바타 애니메이션 실행
    /// <summary>
    /// 아바타 애니메이션 실행
    /// </summary>
    /// <param name="aniName"></param>
    private void PlayAvatarAnimation(string aniName)
    {
        if (string.IsNullOrEmpty(aniName))
        {
            Debug.Log("해당 애니메이션이 없습니다!");
            return;
        }

        if (anim_Avatar != null)
        {
            if (!anim_Avatar.IsInTransition(0))
            {
                anim_Avatar.CrossFade(aniName, 0.1f);
            }
        }
    }
    #endregion

    void OnClickNext()
    {
        if (effectItems.Count > 0)
        {
            Refresh(effectItems.Dequeue());

            for (int i = 0; i < panelAnims.Length; i++)
            {
                panelAnims[i].Stop();
                panelAnims[i].Rewind();
                panelAnims[i].Play();
            }
        }
        else
        {
            Back();

            interiorRTView.Hide();
        }
    }

    void OnClickSkip()
    {
        Back();
    }
}