using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using db;
using MEC;

/// <summary>
/// 마스터 데이터에서 받아오는 전체 Item Data 가공 
/// </summary>
public class ItemDataManager : Singleton<ItemDataManager>
{
    #region 변수
    #region NomalTypeItemData
    public TableBaseData<Item> NomalTypeItemData { get; private set; } = new TableBaseData<Item>();

    public TableBaseData<Item> ConsumableItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> ProductItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> MaterialItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> ToolItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> VehicleItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> PetItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> OtherItemData { get; private set; } = new TableBaseData<Item>();
    #endregion

    #region InteriorTypeItemData
    public TableBaseData<Item> InteriorTypeItemData { get; private set; } = new TableBaseData<Item>();

    public TableBaseData<Item> FurnitureItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> DecorationItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> SpecialtyItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> FloorItemData { get; private set; } = new TableBaseData<Item>();
    #endregion

    #region CostumeTypeItemData
    public TableBaseData<Item> CostumeTypeItemData { get; private set; } = new TableBaseData<Item>();

    public TableBaseData<Item> HairItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> TopItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> BottomItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> OnepieceItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> ShoesItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> AccItemData { get; private set; } = new TableBaseData<Item>();
    #endregion

    #region NFTTypeItemData
    public TableBaseData<Item> NFTTypeItemData { get; private set; } = new TableBaseData<Item>();

    public TableBaseData<Item> NFTHairItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> NFTTopItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> NFTBottomItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> NFTOnepieceItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> NFTShoesItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> NFTAccItemData { get; private set; } = new TableBaseData<Item>();
    public TableBaseData<Item> NFTSpecialItemData { get; private set; } = new TableBaseData<Item>();
    #endregion
    #endregion

    #region Set Data
    /// <summary>
    /// 리소스 파싱하여 딕셔너리에 데이터 담기
    /// </summary>
    public void LoadResourcesData()
    {
        List<Item> avatarPartsDataList = Single.MasterData.dataItem.GetList();
        int count = avatarPartsDataList.Count;

        #region 아이템 타입 분류
        List<ITEM_TYPE> itemTypeList = Util.Enum2List<ITEM_TYPE>();
        int countItemType = itemTypeList.Count;
        for (int i = 0; i < countItemType; i++)
        {
            List<Item> list = avatarPartsDataList.Where(x => x.itemType == (int)itemTypeList[i]).ToList();

            if (list.Count <= 0) continue;

            switch (itemTypeList[i])
            {
                case ITEM_TYPE.NOMAL: SetDictionary(NomalTypeItemData, list); break;
                case ITEM_TYPE.INTERIOR: SetDictionary(InteriorTypeItemData, list); break;
                case ITEM_TYPE.COSTUME: SetDictionary(CostumeTypeItemData, list); break;
                case ITEM_TYPE.NFT: SetDictionary(NFTTypeItemData, list); break;
                default: break;
            }
        }
        #endregion

        #region 카테고리 타입 분류
        List<CATEGORY_TYPE> categoryTypeList = Util.Enum2List<CATEGORY_TYPE>();
        int countCategoryType = categoryTypeList.Count;
        for (int i = 0; i < countCategoryType; i++)
        {
            List<Item> list = avatarPartsDataList.Where(x => x.categoryType == (int)categoryTypeList[i]).ToList();

            foreach (var item in list)
            {
                AddThumbnail(categoryTypeList[i], item.thumbnail);
            }

            if (list.Count <= 0) continue;

            switch (categoryTypeList[i])
            {
                // NOMAL
                case CATEGORY_TYPE.consumable: SetDictionary(ConsumableItemData, list); break;
                case CATEGORY_TYPE.product: SetDictionary(ProductItemData, list); break;
                case CATEGORY_TYPE.material: SetDictionary(MaterialItemData, list); break;
                case CATEGORY_TYPE.tool: SetDictionary(ToolItemData, list); break;
                case CATEGORY_TYPE.vehicle: SetDictionary(VehicleItemData, list); break;
                case CATEGORY_TYPE.pet: SetDictionary(PetItemData, list); break;
                case CATEGORY_TYPE.other: SetDictionary(OtherItemData, list); break;
                // INTERIOR
                case CATEGORY_TYPE.furniture: SetDictionary(FurnitureItemData, list); break;
                case CATEGORY_TYPE.decoration: SetDictionary(DecorationItemData, list); break;
                case CATEGORY_TYPE.specialty: SetDictionary(SpecialtyItemData, list); break;
                case CATEGORY_TYPE.floor: SetDictionary(FloorItemData, list); break;
                // COSTUME
                case CATEGORY_TYPE.hair: SetDictionary(HairItemData, list); break;
                case CATEGORY_TYPE.top: SetDictionary(TopItemData, list); break;
                case CATEGORY_TYPE.bottom: SetDictionary(BottomItemData, list); break;
                case CATEGORY_TYPE.onepiece: SetDictionary(OnepieceItemData, list); break;
                case CATEGORY_TYPE.shoes: SetDictionary(ShoesItemData, list); break;
                case CATEGORY_TYPE.accessory: SetDictionary(AccItemData, list); break;
                // NFT
                case CATEGORY_TYPE.nft_hair: SetDictionary(NFTHairItemData, list); break;
                case CATEGORY_TYPE.nft_top: SetDictionary(NFTTopItemData, list); break;
                case CATEGORY_TYPE.nft_bottom: SetDictionary(NFTBottomItemData, list); break;
                case CATEGORY_TYPE.nft_onepiece: SetDictionary(NFTOnepieceItemData, list); break;
                case CATEGORY_TYPE.nft_shoes: SetDictionary(NFTShoesItemData, list); break;
                case CATEGORY_TYPE.nft_accessory: SetDictionary(NFTAccItemData, list); break;
                case CATEGORY_TYPE.nft_special: SetDictionary(NFTSpecialItemData, list); break;
                default: break;
            }
        }
        #endregion
    }

    private void SetDictionary(TableBaseData<Item> table, List<Item> items)
    {
        table.SetDictionary(table.LoadTable(items).ToDictionary(x => x.id, x => x));
    }
    #endregion

    #region Get Data
    /// <summary>
    /// 카테고리 내의 전체 아이템 정보 가져오기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TableBaseData<Item> GetCategoryData(ITEM_TYPE type)
    {
        TableBaseData<Item> itemData = null;
        switch (type)
        {
            case ITEM_TYPE.NOMAL: itemData = NomalTypeItemData; break;
            case ITEM_TYPE.INTERIOR: itemData = InteriorTypeItemData; break;
            case ITEM_TYPE.COSTUME: itemData = CostumeTypeItemData; break;
            case ITEM_TYPE.NFT: itemData = NFTTypeItemData; break;
            default: break;
        }
        return itemData;
    }

    /// <summary>
    /// 아이템 정보 가져오기 (AVATAR_PARTS_TYPE)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TableBaseData<Item> GetItemData(AVATAR_PARTS_TYPE type)
    {
        return GetItemData(AvatarToCategory(type));
    }

    /// <summary>
    /// 아이템 정보 가져오기 (CATEGORY_TYPE)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TableBaseData<Item> GetItemData(CATEGORY_TYPE type)
    {
        TableBaseData<Item> itemData = null;

        switch (type)
        {
            case CATEGORY_TYPE.consumable: itemData = ConsumableItemData; break;
            case CATEGORY_TYPE.product: itemData = ProductItemData; break;
            case CATEGORY_TYPE.material: itemData = MaterialItemData; break;
            case CATEGORY_TYPE.tool: itemData = ToolItemData; break;
            case CATEGORY_TYPE.vehicle: itemData = VehicleItemData; break;
            case CATEGORY_TYPE.pet: itemData = PetItemData; break;
            case CATEGORY_TYPE.other: itemData = OtherItemData; break;

            case CATEGORY_TYPE.furniture: itemData = FurnitureItemData; break;
            case CATEGORY_TYPE.decoration: itemData = DecorationItemData; break;
            case CATEGORY_TYPE.specialty: itemData = SpecialtyItemData; break;
            case CATEGORY_TYPE.floor: itemData = FloorItemData; break;

            case CATEGORY_TYPE.hair: itemData = HairItemData; break;
            case CATEGORY_TYPE.top: itemData = TopItemData; break;
            case CATEGORY_TYPE.bottom: itemData = BottomItemData; break;
            case CATEGORY_TYPE.onepiece: itemData = OnepieceItemData; break;
            case CATEGORY_TYPE.shoes: itemData = ShoesItemData; break;
            case CATEGORY_TYPE.accessory: itemData = AccItemData; break;

            case CATEGORY_TYPE.nft_hair: itemData = NFTHairItemData; break;
            case CATEGORY_TYPE.nft_top: itemData = NFTTopItemData; break;
            case CATEGORY_TYPE.nft_bottom: itemData = NFTBottomItemData; break;
            case CATEGORY_TYPE.nft_onepiece: itemData = NFTOnepieceItemData; break;
            case CATEGORY_TYPE.nft_shoes: itemData = NFTShoesItemData; break;
            case CATEGORY_TYPE.nft_accessory: itemData = NFTAccItemData; break;
            case CATEGORY_TYPE.nft_special: itemData = NFTSpecialItemData; break;
            default: break;
        }
        return itemData;
    }

    /// <summary>
    /// 다중 메테리얼 데이터 가져오기
    /// </summary>
    /// <param name="itemid"></param>
    /// <returns></returns>
    public List<string> GetMaterials(int itemid)
    {
        return Single.MasterData.dataItemMaterial.GetList()
            .Where(x => x.itemId == itemid)
            .Select(x => x.material)
            .ToList();
    }
    #endregion

    #region Convert
    /// <summary>
    /// 현재 저장된 아바타 세팅 정보 Json으로 변환
    /// </summary>
    /// <returns></returns>
    public string GetAvatarInfoJson()
    {
        return JsonConvert.SerializeObject(LocalPlayerData.AvatarInfo);
    }

    /// <summary>
    /// Json 데이터를 딕셔너리 형태로 변환
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public Dictionary<string, int> ConvertJsonToDic(string json)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, int>>(json).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// AVATAR_PARTS_TYPE을 CATEGORY_TYPE으로 변경. 해당 이름이 같아야 한다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public AVATAR_PARTS_TYPE CategoryToAvatar(CATEGORY_TYPE type)
    {
        return Util.String2Enum<AVATAR_PARTS_TYPE>(type.ToString());
    }

    /// <summary>
    /// CATEGORY_TYPE을 AVATAR_PARTS_TYPE으로 변경. 해당 이름이 같아야 한다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public CATEGORY_TYPE AvatarToCategory(AVATAR_PARTS_TYPE type)
    {
        return Util.String2Enum<CATEGORY_TYPE>(type.ToString());
    }

    /// <summary>
    /// NFT 코스튬 Id의 데이터를 조회하여 딕셔너리 데이터로 변환
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Dictionary<string, int> ConvertNFTCostumData(int id)
    {
        var data = new Dictionary<string, int> { { Util.EnumInt2String(AVATAR_PARTS_TYPE.nft_special), id } };
        return ConvertNFTCostumData(data);
    }

    /// <summary>
    /// NFT 코스튬 Id의 여부를 확인하고 해당 데이터를 조회하여 딕셔너리 데이터로 변환
    /// Id가 없을 시 데이터 그대로 반환
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Dictionary<string, int> ConvertNFTCostumData(Dictionary<string, int> data)
    {
        if (data.TryGetValue(Util.EnumInt2String(AVATAR_PARTS_TYPE.nft_special), out int value))
        {
            if (value > 0)
            {
                return Single.MasterData.dataKtmfSpecialItem.GetDictionary_intint()
                    .Where(x => x.Key.Item1 == value)
                    .ToDictionary(x => ConvertEnumStr(x.Value.partsId), x => x.Value.partsId);
            }
        }
        return data;
    }

    /// <summary>
    /// NFT 코스튬 카테고리 타입 마지막 숫자만 가져와 AVATAR_PARTS_TYPE의 int값 string으로 사용
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private string ConvertEnumStr(int id)
    {
        var str = Single.MasterData.dataItem.GetData(id).categoryType.ToString();
        return str[str.Length - 1].ToString();
    }
    #endregion

    #region Avatar Data 관련
    /// <summary>
    /// 아바타 세팅 리셋
    /// </summary>
    public void AvatarSettingInit()
    {
        LocalPlayerData.AvatarInfo = Single.MasterData.dataAvatarResetInfo.GetList().ToDictionary(x => x.partsType.ToString(), x => x.itemId);
    }

    /// <summary>
    /// 아바타 리셋 옷 데이터 딕셔너리 반환
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, int> GetAvatarResetInfo()
    {
        if (Single.MasterData.dataAvatarResetInfo != null)
        {
            return Single.MasterData.dataAvatarResetInfo.GetList().ToDictionary(x => x.partsType.ToString(), x => x.itemId);
        }
        return null;
    }

    public bool IsWearNFTCostume(Dictionary<string, int> data)
    {
        if (data.TryGetValue(Util.EnumInt2String(AVATAR_PARTS_TYPE.nft_special), out int value))
        {
            return value > 0;
        }
        return false;
    }
    #endregion

    #region 코스튬 썸네일 관리
    private Dictionary<string, Sprite> ThumbnailDic = new Dictionary<string, Sprite>();

    /// <summary>
    /// 썸네일 로드하기
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private void AddThumbnail(CATEGORY_TYPE type, string name)
    {
        string path = null;
        switch (type)
        {
            case CATEGORY_TYPE.hair: path = Cons.Path_Thumbnail_hair; break;
            case CATEGORY_TYPE.top: path = Cons.Path_Thumbnail_top; break;
            case CATEGORY_TYPE.bottom: path = Cons.Path_Thumbnail_bottom; break;
            case CATEGORY_TYPE.onepiece: path = Cons.Path_Thumbnail_onepiece; break;
            case CATEGORY_TYPE.shoes: path = Cons.Path_Thumbnail_shoes; break;
            case CATEGORY_TYPE.accessory: path = Cons.Path_Thumbnail_accessory; break;
            default: return;
        }

        if (string.IsNullOrEmpty(path)) return;

        string itemPath = path + name;
        Sprite sprite = Resources.Load<Sprite>(itemPath);
        if (sprite == null)
        {
            sprite = Util.GetDummySprite();
            Debug.LogWarning(itemPath + " 의 썸네일이 존재하지 않습니다!");

            if (sprite == null) return;
        }

        if (!ThumbnailDic.ContainsKey(name))
        {
            ThumbnailDic.Add(name, sprite);
        }
    }

    /// <summary>
    /// 딕셔너리에서 썸네일 가져오기
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Sprite GetThumbnail(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return ThumbnailDic[name];
    }

    public Sprite GetItemIconSprite(int id)
    {
        Sprite spr = null;

        Item itemDb = Single.MasterData.GetItem(id);
        switch ((ITEM_TYPE)itemDb.itemType)
        {
            case ITEM_TYPE.NOMAL:
                break;

            case ITEM_TYPE.INTERIOR:
                if (InteriorThumbnailDic.TryGetValue(itemDb.prefab, out Texture2D tex))
                    spr = Util.Tex2Sprite(tex);
                break;

            case ITEM_TYPE.COSTUME:
                if (!string.IsNullOrEmpty(itemDb.thumbnail))
                    spr = GetThumbnail(itemDb.thumbnail);
                break;
        }

        return spr;
    }
    #endregion

    #region 가구 썸네일 관리
    Dictionary<string, Texture2D> InteriorThumbnailDic = new Dictionary<string, Texture2D>();

    /// <summary> 가구 썸네일 아이콘 생성 후 캐싱 </summary>
    public void CreateInteriorThumbnail()
    {
        if (InteriorThumbnailDic.Count > 0)
            return;

        RuntimePreviewGenerator.BackgroundColor = Color.clear;
        List<Item> itemList = Single.MasterData.dataItem.GetList();
        for (int i = 0; i < itemList.Count; i++)
        {
            Item item = itemList[i];
            if (Single.MasterData.dataItemType.GetData(item.itemType).name != "인테리어")
            {
                continue;
            }
            GameObject prefab = LoadPrefab(item.id); //이제는 썸네일 생성하지 않아야할때가 오지않았나..230512
            if (prefab == null)
            {
                continue;
            }
            InteriorThumbnailDic.Add(prefab.name, RuntimePreviewGenerator.GenerateModelPreview(prefab.transform, 512, 512));
        }
    }

    GameObject LoadPrefab(int itemId)
    {
        try
        {
            Item item = Single.MasterData.GetItem(itemId);

            //기존폴더
            int layerType = Single.MasterData.dataInteriorInstallInfo.GetData(item.id).layerType;
            string layerName = Single.MasterData.dataLayerType.GetData(layerType).name;
            string path = Path.Combine(Cons.Path_MyRoom, layerName, item.prefab);

            //새로운폴더
            //string categoryType = MasterDataManager.Instance.dataCategoryType.GetData(item.categoryType).name;
            //categoryType  = categoryType.Split('_').Last();
            //string path = Path.Combine(Cons.Path_MyRoom, categoryType, item.prefab);

            return Resources.Load<GameObject>(path);
        }
        catch
        {
            return null;
        }
    }
    #endregion
}
