using FrameWork.UI;
using Lean.Common;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.LoopScrollView_Custom;

class Item_AvatarPreset : FancyCell_Custom
{
    #region 변수
    private Item_AvatarPresetData data;

    private TMP_Text txtmp_PresetName;
    private RawImage go_RawImage;

    private Panel_Preset panel_Preset;
    private LeanDragPanel leanDragPanel;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_PresetName = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_PresetName));
        #endregion

        #region etc
        panel_Preset = SceneLogic.instance.GetPanel<Panel_Preset>();

        go_RawImage = uIBase.GetChildGObject(nameof(go_RawImage)).GetComponent<RawImage>();
        if (go_RawImage != null)
        {
            leanDragPanel = go_RawImage.GetComponent<LeanDragPanel>();
        }
        #endregion
    }

    #region AvatarPreset

    public override void UpdateContent(Item_Data itemData)
    {
        if(itemData is Item_AvatarPresetData _data)
        {
            data = _data;

            base.UpdateContent(itemData);
        }
    }

    protected override void SetContent()
    {
        if (go_RawImage != null)
        {
            go_RawImage.texture = data.texture;
        }

        if (leanDragPanel != null)
        {
            leanDragPanel.rotate = data.leanManualRotate as LeanManualRotate;
        }

        if (txtmp_PresetName != null)
        {
            Util.SetMasterLocalizing(txtmp_PresetName, new MasterLocalData(data.localId));
        }
    }

    protected override void SetAnimatorHash() => animatorHashName = Cons.Ani_PresetScroll;

    public override void UpdatePosition(float position)
    {
        base.UpdatePosition(position);

        if (Context == null) return;

        bool isSelect = Context.SelectedIndex == Index;
        go_RawImage.raycastTarget = isSelect;

        if (isSelect)
        {
            panel_Preset.SelectPresetId(data.presetId);
        }
        else if (isDone)
        {
            Timing.RunCoroutine(Co_ResetCamera());
        }
    }

    private bool isDone = true;
    /// <summary>
    /// 선택된 아이템이 아닐 시 아바타 회전 리셋
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> Co_ResetCamera()
    {
        isDone = false;

        yield return Timing.WaitUntilTrue(() => leanDragPanel.rotate != null);

        Quaternion start = leanDragPanel.rotate.transform.localRotation;
        if (start == Quaternion.identity)
        {
            isDone = true;
            yield break;
        }

        float curTime = 0f;
        float durTime = 1f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            leanDragPanel.rotate.transform.localRotation = Quaternion.Lerp(start, Quaternion.identity, curTime);
        }

        leanDragPanel.ResetRotate();

        isDone = true;
    }

    #endregion
}
