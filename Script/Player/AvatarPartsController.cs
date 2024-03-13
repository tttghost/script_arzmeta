using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using db;
using MEC;
using System;

public class AvatarPartsController : BasePartsSelector
{
    #region Members

    private List<AVATAR_PARTS_TYPE> avatarPartsType = new List<AVATAR_PARTS_TYPE>();
    private Dictionary<string, int> avatarItemDic = new Dictionary<string, int>();

    private SkinParts skinParts;
    //private SkinPart hair;
    //private SkinPart top;
    //private SkinPart bottom;
    //private SkinPart costume;
    //private SkinPart shoes;
    //private SkinPart acc;

    private Item item = new Item();

    #endregion



    #region Initialize

    private void Awake()
    {
        avatarPartsType = Util.Enum2List<AVATAR_PARTS_TYPE>();

        Util.RunCoroutine(Initialize());
    }

    IEnumerator<float> Initialize()
    {
        yield return Timing.WaitUntilTrue(() => MyPlayer.instance);

        var skinnedMeshRenderers = MyPlayer.instance.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            skinnedMeshRenderers[i].updateWhenOffscreen = true;
        }
    }

    #endregion



    #region public

    public void SetAvatarParts(Dictionary<string, int> dic, Action initEnd = null, bool log = true)
    {
        if (dic != null) SetAvatarInit(dic, initEnd, log);
    }

    public void SetAvatarParts(string json, Action initEnd = null, bool log = true)
    {
        Dictionary<string, int> dic = Single.ItemData.ConvertJsonToDic(json);

        if (dic != null) SetAvatarInit(dic, initEnd, log);
    }

    /// <summary>
    /// 리셋 포함된 bool값
    /// </summary>
    public bool IsResetCloth(AVATAR_PARTS_TYPE type)
    {
        switch (type)
        {
            case AVATAR_PARTS_TYPE.shoes:
            case AVATAR_PARTS_TYPE.accessory:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 선택한 파츠 바로 입기
    /// </summary>
    public void WearSelectParts(AVATAR_PARTS_TYPE type, string prefabName, List<string> materialName)
    {
        switch (type)
        {
            case AVATAR_PARTS_TYPE.hair: ChangeParts(PARTS_TYPE.hair, prefabName, materialName); break;
            case AVATAR_PARTS_TYPE.top: ChangeParts(PARTS_TYPE.top, prefabName, materialName, _resetOnepiece: true); break;
            case AVATAR_PARTS_TYPE.bottom: ChangeParts(PARTS_TYPE.bottom, prefabName, materialName, _resetOnepiece: true); break;
            case AVATAR_PARTS_TYPE.onepiece: ChangeParts(PARTS_TYPE.onepiece, prefabName, materialName); break;
            case AVATAR_PARTS_TYPE.shoes: ChangeParts(PARTS_TYPE.shoes, prefabName, materialName, _resetOnepiece: true); break;
            case AVATAR_PARTS_TYPE.accessory: ChangeParts(PARTS_TYPE.accessory, prefabName, materialName); break;
            default: break;
        }
    }

    /// <summary>
    /// 리셋 바로 입기
    /// </summary>
    public void ChangeResetCloth(AVATAR_PARTS_TYPE type)
    {
        switch (type)
        {
            case AVATAR_PARTS_TYPE.shoes: ChangeParts(PARTS_TYPE.shoes, _resetOnepiece: true); break;
            case AVATAR_PARTS_TYPE.accessory: ChangeParts(PARTS_TYPE.accessory); break;
            default: break;
        }
    }
    #endregion



    #region private
    /// <summary>
    /// 아바타 프리팹 세팅 본 메소드
    /// </summary>
    private void SetAvatarInit(Dictionary<string, int> dic, Action initEnd = null, bool _log = true)
    {
        avatarItemDic = Single.ItemData.ConvertNFTCostumData(dic.ToDictionary(x => x.Key, x => x.Value));
        if (avatarItemDic == null)
        {
            avatarItemDic = Single.ItemData.GetAvatarResetInfo().ToDictionary(x => x.Key, x => x.Value);
        }

        skinParts = new SkinParts();

        foreach (AVATAR_PARTS_TYPE type in avatarPartsType)
        {
            string key = Util.EnumInt2String(type);

            if (!avatarItemDic.TryGetValue(key, out int value)) continue;

            if (value <= 0)
            {
                ResetCloth(type);
                continue;
            }

            item = Single.MasterData.dataItem.GetData(value);
            if (item == null)
            {
                Debug.Log("인덱스에 없는 아이템이 있어요!");
                Single.ItemData.AvatarSettingInit();
                SetAvatarInit(LocalPlayerData.AvatarInfo); // 재귀함수
                break;
            }

            ChangeCloth(type);
        }

        SetAvatar(skinParts, _log);
        initEnd?.Invoke();
    }

    /// <summary>
    /// 입을 파츠 저장
    /// </summary>
    private void ChangeCloth(AVATAR_PARTS_TYPE type)
    {
        var skinPart = new SkinPart(item.prefab, Single.ItemData.GetMaterials(item.id));

        switch (type)
        {
            case AVATAR_PARTS_TYPE.hair: skinParts.hair = skinPart; break;
            case AVATAR_PARTS_TYPE.top: skinParts.top = skinPart; break;
            case AVATAR_PARTS_TYPE.bottom: skinParts.bottom = skinPart; break;
            case AVATAR_PARTS_TYPE.onepiece: skinParts.onepiece = skinPart; break;
            case AVATAR_PARTS_TYPE.shoes: skinParts.shoes = skinPart; break;
            case AVATAR_PARTS_TYPE.accessory: skinParts.accessory = skinPart; break;
        }
    }

    /// <summary>
    /// 리셋 파츠 저장
    /// </summary>
    private void ResetCloth(AVATAR_PARTS_TYPE type)
    {
        switch (type)
        {
            case AVATAR_PARTS_TYPE.hair:
                skinParts.hair = new SkinPart(Define.DEFALUT_PREFAB_HAIR, Define.DEFALUT_MAT_HAIR); break;
            case AVATAR_PARTS_TYPE.top:
                if (IsContainsCostumeKey()) return;
                skinParts.top = new SkinPart(Define.DEFALUT_PREFAB_TOP, Define.DEFALUT_MAT_TOP); break;
            case AVATAR_PARTS_TYPE.bottom:
                if (IsContainsCostumeKey()) return;
                skinParts.bottom = new SkinPart(Define.DEFALUT_PREFAB_BOTTOM, Define.DEFALUT_MAT_BOTTOM); break;
            case AVATAR_PARTS_TYPE.shoes:
                if (IsContainsCostumeKey()) return;
                skinParts.shoes = new SkinPart(Define.DEFALUT_PREFAB_SHOES, Define.DEFALUT_MAT_SHOES); break;
            case AVATAR_PARTS_TYPE.accessory:
                skinParts.accessory = new SkinPart(Define.DEFALUT_PREFAB_ACC, Define.DEFALUT_MAT_ACC); break;
            default: break;
        }
    }

    /// <summary>
    /// 원피스를 입고 있는지 아닌지
    /// </summary>
    /// <returns></returns>
    private bool IsContainsCostumeKey()
    {
        string key = Util.EnumInt2String(AVATAR_PARTS_TYPE.onepiece);
        return avatarItemDic.ContainsKey(key) && avatarItemDic[key] > 0;
    }

    #endregion
}
