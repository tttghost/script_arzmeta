using db;
using FrameWork.UI;
using Lean.Common;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Panel_Preset : PanelBase
{
    #region 변수
    public GameObject go_AvatarView;

    private List<RenderTexture> rtList = new List<RenderTexture>();
    private List<GameObject> avatarViewList = new List<GameObject>();
    private List<Dictionary<string, int>> avatarCostumeList = new List<Dictionary<string, int>>();
    private LoopScrollView_Custom go_ScrollView;
    private Transform AvatarViewparent;
    private AvatarPartsController controller_AvatarView;

    private List<AVATAR_PRESET> avatarPresetList;

    private int curPresetId = 0;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("3065"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sub", new MasterLocalData("3060"));
        #endregion

        #region Button
        GetUI_Button("btn_PreItem", () => go_ScrollView.SelectNextCell());
        GetUI_Button("btn_NextItem", () => go_ScrollView.SelectPrevCell());
        GetUI_Button("btn_Next", OnClick_Next);
        #endregion

        #region etc
        go_ScrollView = GetChildGObject(nameof(go_ScrollView)).GetComponent<LoopScrollView_Custom>();

        GameObject go_RTView = GameObject.Find(nameof(go_RTView));
        controller_AvatarView = go_RTView.GetComponentInChildren<AvatarPartsController>();
        if (controller_AvatarView)
        {
            Animator anim_Avatar = Util.Search<Animator>(go_RTView, "AvatarParts");
            if (anim_Avatar != null)
            {
                controller_AvatarView.SetTarget(anim_Avatar.transform);
            }
        }

        avatarPresetList = Util.Enum2List<AVATAR_PRESET>();
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        Util.RunCoroutine(Co_SetPreset());
    }

    private IEnumerator<float> Co_SetPreset()
    {
        #region Rig 생성
        GameObject go = new GameObject("go_AvatarViewRig");
        AvatarViewparent = go.transform;

        if (AvatarViewparent == null)
        {
            Debug.Log("AvatarViewparent is null !!");
            yield break;
        }
        #endregion

        yield return Timing.WaitUntilDone(Co_SetAvatarPreset());

        ItemSetting();
    }

    /// <summary>
    /// 아바타 생성
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> Co_SetAvatarPreset()
    {
        int count = avatarPresetList.Count;
        for (int i = 0; i < count; i++)
        {
            // 생성 및 위치 지정
            GameObject presetObj = Instantiate(go_AvatarView, new Vector3(0, -(50 * i), 0), Quaternion.identity);
            presetObj.transform.SetParent(AvatarViewparent);

            // 카메라 끄기
            Camera camera = presetObj.GetComponentInChildren<Camera>();
            camera.enabled = false;

            // 아바타 세팅 타깃 설정
            AvatarPartsController avatarPartsController = presetObj.GetComponentInChildren<AvatarPartsController>();
            Transform avatarTarget = presetObj.GetComponentInChildren<Animator>().transform;
            avatarPartsController.SetTarget(avatarTarget);

            // 옷 입히기
            Dictionary<string, int> presetInfoDic = SetAvatarParts(i);
            avatarCostumeList.Add(presetInfoDic);
            avatarPartsController.SetAvatarParts(presetInfoDic);

            yield return Timing.WaitForOneFrame;

            // 랜더 텍스쳐 생성 및 설정
            RenderTexture renderTexture = new RenderTexture(350, 700, 24);
            camera.targetTexture = renderTexture;
            camera.enabled = true; // 카메라 켜기
            rtList.Add(renderTexture);

            avatarViewList.Add(presetObj);
        }
    }

    private Dictionary<string, int> SetAvatarParts(int i)
    {
        return Single.MasterData.dataAvatarPreset.GetList()
            .Where(x => x.presetType == (i + 1))
            .ToDictionary(x => x.partsType.ToString(), x => x.itemId);
    }

    /// <summary>
    /// 아이템 설정
    /// </summary>
    private void ItemSetting()
    {
        List<Item_AvatarPresetData> presetItemList = new List<Item_AvatarPresetData>();
        List<AVATAR_PRESET> avatarPresetList = Util.Enum2List<AVATAR_PRESET>();
        int count = avatarPresetList.Count;
        for (int i = 0; i < count; i++)
        {
            Item_AvatarPresetData item = new Item_AvatarPresetData
            {
                presetId = i + 1,
                texture = rtList[i],
                leanManualRotate = avatarViewList[i].GetComponentInChildren<LeanManualRotate>()
            };

            string localId = string.Format("307{0}", i);
            item.localId = localId;

            presetItemList.Add(item);
        }

        go_ScrollView.UpdateData(presetItemList.ConvertAll(x => x as Item_Data));
        go_ScrollView.JumpTo(0);
    }
    #endregion

    #region Preset
    /// <summary>
    /// 현재 내가 선택한 프리셋 Id
    /// </summary>
    /// <param name="presetId"></param>
    public void SelectPresetId(int presetId) => curPresetId = presetId;

    /// <summary>
    /// 선택 정보 저장 및 다음 단계로 이동
    /// </summary>
    private void OnClick_Next()
    {
        if (curPresetId <= 0)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("common_error_retye")));
            return;
        }

        controller_AvatarView.SetAvatarParts(avatarCostumeList[curPresetId - 1]);

        SceneLogic.instance.isUILock = false;
        PushPanel<Panel_ArzProfile_Create>().curPresetId = curPresetId;
    }
    #endregion
}
