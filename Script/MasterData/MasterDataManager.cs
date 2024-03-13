/*************************************************************************************************************************************
 * 
 *			Class MasterDataManager
 *			
 *				- MasterData 폴더에 있는 json 파일을 읽어와 가각의 class에 담는다
 * 
 *************************************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using db;
using UnityEngine.Networking;
using System.Text;
using MEC;
public class MasterDataManager : Singleton<MasterDataManager>
{
    #region TableBaseData
    public TableBaseData<AvatarPreset> dataAvatarPreset { get; private set; }
    public TableBaseData<AvatarResetInfo> dataAvatarResetInfo { get; private set; }
    public TableBaseData<BusinessCardTemplate> dataBusinessCardTemplate { get; private set; }
    public TableBaseData<CommerceZoneItem> dataCommerceZoneItem { get; private set; }
    public TableBaseData<CommerceZoneMannequin> dataCommerceZoneMannequin { get; private set; }
    public TableBaseData<CountryCode> dataCountryCode { get; private set; }
    public TableBaseData<Faq> dataFaq { get; private set; }
    public TableBaseData<ForbiddenWords> dataForbiddenWords { get; private set; }
    public TableBaseData<ItemUseEffect> dataItemUseEffect { get; private set; }
    public TableBaseData<JumpingMatchingLevel> dataJumpingMatchingLevel { get; private set; }
    public TableBaseData<LanguageType> dataLanguageType { get; private set; }
    public TableBaseData<OsType> dataOsType { get; private set; }
    public TableBaseData<ProviderType> dataProviderType { get; private set; }
    public TableBaseData<QuizLevel> dataQuizLevel { get; private set; }
    public TableBaseData<QuizQuestionAnswer> dataQuizQuestionAnswer { get; private set; }
    public TableBaseData<QuizRoundTime> dataQuizRoundTime { get; private set; }
    public TableBaseData<ReportType> dataReport { get; private set; }
    public TableBaseData<FunctionTable> dataFunctionTable { get; private set; }
    public TableBaseData<MapExposulInfo> dataMapExposulInfo { get; private set; }
    public TableBaseData<MapExposulBrand> dataMapExposulBrand { get; private set; }
    public TableBaseData<DynamicLinkType> dataDynamicLinkType { get; private set; }
    public TableBaseData<UploadType> dataUploadType { get; private set; }
    public TableBaseData<MoneyType> dataMoneyType { get; private set; }
    public TableBaseData<KtmfSpecialItem> dataKtmfSpecialItem { get; private set; }
    public TableBaseData<FileBoxType> dataFileBoxType { get; private set; }
    public TableBaseData<ItemMaterial> dataItemMaterial { get; private set; }
    public TableBaseData<NoticeType> dataNoticeType { get; private set; }
    public TableBaseData<NoticeExposureType> dataNoticeExposureType { get; private set; }

    #region NPC
    public TableBaseData<NpcCostume> dataNpcCostume { get; private set; }
    public TableBaseData<NpcList> dataNpcList { get; private set; }
    public TableBaseData<NpcLookType> dataNpcLookType { get; private set; }
    public TableBaseData<NpcType> dataNpcType { get; private set; }
    #endregion

    #region 마이룸
    public TableBaseData<CategoryType> dataCategoryType { get; private set; }
    public TableBaseData<GradeType> dataGradeType { get; private set; }
    public TableBaseData<InteriorInstallInfo> dataInteriorInstallInfo { get; private set; }
    public TableBaseData<Item> dataItem { get; private set; }
    public TableBaseData<ItemType> dataItemType { get; private set; }
    public TableBaseData<LayerType> dataLayerType { get; private set; }
    public TableBaseData<Localization> dataLocalization { get; private set; }
    public TableBaseData<PackageType> dataPackageType { get; private set; }
    public TableBaseData<StoreType> dataStoreType { get; private set; }
    #endregion

    #region 회의실
    public TableBaseData<OfficeAlarmType> dataOfficeAlarmType { get; private set; }
    public TableBaseData<db.OfficeAuthority> dataOfficeAuthority { get; private set; }
    public TableBaseData<OfficeBookmark> dataOfficeBookmark { get; private set; }
    public TableBaseData<OfficeDefaultOption> dataOfficeDefaultOption { get; private set; }
    public TableBaseData<OfficeGradeType> dataOfficeGradeType { get; private set; }
    public TableBaseData<OfficeMode> dataOfficeMode { get; private set; }
    public TableBaseData<OfficeGradeAuthority> dataOfficeGradeAuthority { get; private set; }
    public TableBaseData<OfficeModeSlot> dataOfficeModeSlot { get; private set; }
    public TableBaseData<db.OfficeModeType> dataOfficeModeType { get; private set; }
    public TableBaseData<OfficePermissionType> dataOfficePermissionType { get; private set; }
    public TableBaseData<OfficeShowRoomInfo> dataOfficeShowRoomInfo { get; private set; }
    public TableBaseData<OfficeSpaceInfo> dataOfficeSpaceInfo { get; private set; }
    public TableBaseData<OfficeSpawnType> dataOfficeSpawnType { get; private set; }
    public TableBaseData<OfficeTopicType> dataOfficeTopicType { get; private set; }
    public TableBaseData<OnfContentsType> dataOnfContentsType { get; private set; }
    public TableBaseData<OfficeExposure> dataOfficeExposure { get; private set; }
    public TableBaseData<OfficeExposureType> dataOfficeExposureType { get; private set; }
    public TableBaseData<OfficeProductItem> dataOfficeProductItem { get; private set; }
    public TableBaseData<OfficeSeatInfo> dataOfficeSeatInfo { get; private set; }
    public TableBaseData<db.BannerInfo> bannerInfo { get; private set; }
    //public TableBaseData<BannerReservationInfo> bannerReservationInfo { get; private set; }
    #endregion

    #region Postbox
    public TableBaseData<PostalEffectType> dataPostalEffectType { get; private set; }
    public TableBaseData<PostalItemProperty> dataPostalItemProperty { get; private set; }
    public TableBaseData<PostalType> dataPostalType { get; private set; }
    public TableBaseData<PostalMoneyProperty> dataPostalMoneyProperty { get; private set; }
    #endregion Postbox

    #endregion

    private string folderName = "master";
    private string fileName = "master.json";
    public bool masterDataIsDone { get; private set; } = false;


    /// <summary>
    /// 시스템 로컬라이징
    /// </summary>
    public void SystemLocalization()
    {

        TextAsset SystemLocalization = Resources.Load<TextAsset>(Cons.Path_MasterData + nameof(SystemLocalization));

        JObject jobj = (JObject)JsonConvert.DeserializeObject(SystemLocalization.text);
        foreach (var x in jobj)
        {
            switch (x.Key)
            {
                case "SystemLocalization":
                    {
                        dataLocalization = new TableBaseData<Localization>();
                        dataLocalization.SetDictionary(dataLocalization.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                default:
                    break;
            }
        }
    }




    /// <summary>
    /// 마스터데이터 이니셜라이즈
    /// </summary>
    public void Initialize()
    {
        Timing.RunCoroutine(Co_InitMasterData());
    }

    public string forceStorageUrl;

    /// <summary>
    /// 마스터데이터 이니셜라이즈
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> Co_InitMasterData()
    {
        string masterUrl = Path.Combine(forceStorageUrl ?? Single.Web.StorageUrl, folderName, fileName);

        UnityWebRequest www = new UnityWebRequest(masterUrl);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return Timing.WaitUntilDone(www.SendWebRequest());

        switch (www.result)
        {
            case UnityWebRequest.Result.InProgress:
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                {
                    Debug.Log(www.error);
                }
                break;
            case UnityWebRequest.Result.Success:
                {
                    byte[] results = www.downloadHandler.data;

                    string resultStr = ByteToString(results);

                    //SaveMasterData(resultStr); //로컬파일 저장할 필요 없음

                    LoadMasterData(resultStr);
                }
                break;
        }
        www.Dispose();
    }

    /// <summary>
    /// 마스터데이터 로컬저장
    /// </summary>
    private void SaveMasterData(string resultStr)
    {
        //폴더 없으면 생성
        var folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        //파일 저장
        var filePath = Path.Combine(folderPath, fileName);
        File.WriteAllText(filePath, resultStr);
    }

    /// <summary>
    /// 바이트를 스트링으로 변환
    /// </summary>
    /// <param name="strByte"></param>
    /// <returns></returns>
    private string ByteToString(byte[] strByte)
    {
        return Encoding.Default.GetString(strByte);
    }

    /// <summary>
    /// 마스터데이터 불러오기
    /// </summary>
    /// <param name="resultStr"></param>
    private void LoadMasterData(string resultStr)
    {
        JObject jobj = (JObject)JsonConvert.DeserializeObject(resultStr);
        foreach (var x in jobj)
        {
            switch (x.Key)
            {
                case "AvatarPreset":
                    {
                        dataAvatarPreset = new TableBaseData<AvatarPreset>();
                        dataAvatarPreset.SetDictionary(dataAvatarPreset.LoadTable(x.Value.ToString()).ToDictionary(x => (x.presetType, x.partsType), x => x));
                    }
                    break;
                case "AvatarResetInfo":
                    {
                        dataAvatarResetInfo = new TableBaseData<AvatarResetInfo>();
                        dataAvatarResetInfo.SetDictionary(dataAvatarResetInfo.LoadTable(x.Value.ToString()).ToDictionary(x => x.itemId, x => x));
                    }
                    break;
                case "BusinessCardTemplate":
                    {
                        dataBusinessCardTemplate = new TableBaseData<BusinessCardTemplate>();
                        dataBusinessCardTemplate.SetDictionary(dataBusinessCardTemplate.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "CommerceZoneItem":
                    {
                        dataCommerceZoneItem = new TableBaseData<CommerceZoneItem>();
                        dataCommerceZoneItem.SetDictionary(dataCommerceZoneItem.LoadTable(x.Value.ToString()).ToDictionary(x => x.itemId, x => x));
                    }
                    break;
                case "CommerceZoneMannequin":
                    {
                        dataCommerceZoneMannequin = new TableBaseData<CommerceZoneMannequin>();
                        dataCommerceZoneMannequin.SetDictionary(dataCommerceZoneMannequin.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "CountryCode":
                    {
                        dataCountryCode = new TableBaseData<CountryCode>();
                        dataCountryCode.SetDictionary(dataCountryCode.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "Faq":
                    {
                        dataFaq = new TableBaseData<Faq>();
                        dataFaq.SetDictionary(dataFaq.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "ForbiddenWords":
                    {
                        dataForbiddenWords = new TableBaseData<ForbiddenWords>();
                        dataForbiddenWords.SetDictionary(dataForbiddenWords.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "JumpingMatchingLevel":
                    {
                        dataJumpingMatchingLevel = new TableBaseData<JumpingMatchingLevel>();
                        dataJumpingMatchingLevel.SetDictionary(dataJumpingMatchingLevel.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "ItemUseEffect":
                    {
                        dataItemUseEffect = new TableBaseData<ItemUseEffect>();
                        dataItemUseEffect.SetDictionary(dataItemUseEffect.LoadTable(x.Value.ToString()).ToDictionary(x => x.itemId, x => x));
                    }
                    break;
                case "LanguageType":
                    {
                        dataLanguageType = new TableBaseData<LanguageType>();
                        dataLanguageType.SetDictionary(dataLanguageType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OsType":
                    {
                        dataOsType = new TableBaseData<OsType>();
                        dataOsType.SetDictionary(dataOsType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "ProviderType":
                    {
                        dataProviderType = new TableBaseData<ProviderType>();
                        dataProviderType.SetDictionary(dataProviderType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "QuizLevel":
                    {
                        dataQuizLevel = new TableBaseData<QuizLevel>();
                        dataQuizLevel.SetDictionary(dataQuizLevel.LoadTable(x.Value.ToString()).ToDictionary(x => x.level, x => x));
                    }
                    break;
                case "QuizQuestionAnswer":
                    {
                        dataQuizQuestionAnswer = new TableBaseData<QuizQuestionAnswer>();
                        dataQuizQuestionAnswer.SetDictionary(dataQuizQuestionAnswer.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "QuizRoundTime":
                    {
                        dataQuizRoundTime = new TableBaseData<QuizRoundTime>();
                        dataQuizRoundTime.SetDictionary(dataQuizRoundTime.LoadTable(x.Value.ToString()).ToDictionary(x => x.round, x => x));
                    }
                    break;
                case "ReportType":
                    {
                        dataReport = new TableBaseData<ReportType>();
                        dataReport.SetDictionary(dataReport.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "CategoryType":
                    {
                        dataCategoryType = new TableBaseData<CategoryType>();
                        dataCategoryType.SetDictionary(dataCategoryType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "GradeType":
                    {
                        dataGradeType = new TableBaseData<GradeType>();
                        dataGradeType.SetDictionary(dataGradeType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "InteriorInstallInfo":
                    {
                        dataInteriorInstallInfo = new TableBaseData<InteriorInstallInfo>();
                        dataInteriorInstallInfo.SetDictionary(dataInteriorInstallInfo.LoadTable(x.Value.ToString()).ToDictionary(x => x.itemId, x => x));
                    }
                    break;
                case "Item":
                    {
                        dataItem = new TableBaseData<Item>();
                        dataItem.SetDictionary(dataItem.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "ItemType":
                    {
                        dataItemType = new TableBaseData<ItemType>();
                        dataItemType.SetDictionary(dataItemType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "LayerType":
                    {
                        dataLayerType = new TableBaseData<LayerType>();
                        dataLayerType.SetDictionary(dataLayerType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "Localization":
                    {
                        dataLocalization = new TableBaseData<Localization>();
                        dataLocalization.SetDictionary(dataLocalization.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "PackageType":
                    {
                        dataPackageType = new TableBaseData<PackageType>();
                        dataPackageType.SetDictionary(dataPackageType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "StoreType":
                    {
                        dataStoreType = new TableBaseData<StoreType>();
                        dataStoreType.SetDictionary(dataStoreType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeAlarmType": // 회의실 데이터
                    {
                        dataOfficeAlarmType = new TableBaseData<OfficeAlarmType>();
                        dataOfficeAlarmType.SetDictionary(dataOfficeAlarmType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeAuthority": // 회의실 데이터
                    {
                        dataOfficeAuthority = new TableBaseData<db.OfficeAuthority>();
                        dataOfficeAuthority.SetDictionary(dataOfficeAuthority.LoadTable(x.Value.ToString()).ToDictionary(x => (x.modeType, x.permissionType), x => x));
                    }
                    break;
                case "OfficeBookmark":
                    {
                        dataOfficeBookmark = new TableBaseData<OfficeBookmark>();
                        dataOfficeBookmark.SetDictionary(dataOfficeBookmark.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "OfficeDefaultOption":
                    {
                        dataOfficeDefaultOption = new TableBaseData<OfficeDefaultOption>();
                        dataOfficeDefaultOption.SetDictionary(dataOfficeDefaultOption.LoadTable(x.Value.ToString()).ToDictionary(x => x.permissionType, x => x));
                    }
                    break;
                case "OfficeGradeType":
                    {
                        dataOfficeGradeType = new TableBaseData<OfficeGradeType>();
                        dataOfficeGradeType.SetDictionary(dataOfficeGradeType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeMode":
                    {
                        dataOfficeMode = new TableBaseData<OfficeMode>();
                        dataOfficeMode.SetDictionary(dataOfficeMode.LoadTable(x.Value.ToString()).ToDictionary(x => x.modeType, x => x));
                    }
                    break;
                case "OfficeGradeAuthority":
                    {
                        dataOfficeGradeAuthority = new TableBaseData<OfficeGradeAuthority>();
                        dataOfficeGradeAuthority.SetDictionary(dataOfficeGradeAuthority.LoadTable(x.Value.ToString()).ToDictionary(x => x.gradeType, x => x));
                    }
                    break;
                case "OfficeModeSlot":
                    {
                        dataOfficeModeSlot = new TableBaseData<OfficeModeSlot>();
                        dataOfficeModeSlot.SetDictionary(dataOfficeModeSlot.LoadTable(x.Value.ToString()).ToDictionary(x => (x.modeType, x.permissionType), x => x));
                    }
                    break;
                case "OfficeModeType":
                    {
                        dataOfficeModeType = new TableBaseData<db.OfficeModeType>();
                        dataOfficeModeType.SetDictionary(dataOfficeModeType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficePermissionType":
                    {
                        dataOfficePermissionType = new TableBaseData<OfficePermissionType>();
                        dataOfficePermissionType.SetDictionary(dataOfficePermissionType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeSpaceInfo":
                    {
                        dataOfficeSpaceInfo = new TableBaseData<OfficeSpaceInfo>();
                        dataOfficeSpaceInfo.SetDictionary(dataOfficeSpaceInfo.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "OfficeSpawnType":
                    {
                        dataOfficeSpawnType = new TableBaseData<OfficeSpawnType>();
                        dataOfficeSpawnType.SetDictionary(dataOfficeSpawnType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeTopicType":
                    {
                        dataOfficeTopicType = new TableBaseData<OfficeTopicType>();
                        dataOfficeTopicType.SetDictionary(dataOfficeTopicType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OnfContentsType":
                    {
                        dataOnfContentsType = new TableBaseData<OnfContentsType>();
                        dataOnfContentsType.SetDictionary(dataOnfContentsType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeExposure":
                    {
                        dataOfficeExposure = new TableBaseData<OfficeExposure>();
                        dataOfficeExposure.SetDictionary(dataOfficeExposure.LoadTable(x.Value.ToString()).ToDictionary(x => (x.exposureType, x.modeType), x => x));
                    }
                    break;
                case "OfficeExposureType":
                    {
                        dataOfficeExposureType = new TableBaseData<OfficeExposureType>();
                        dataOfficeExposureType.SetDictionary(dataOfficeExposureType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeProductItem":
                    {
                        dataOfficeProductItem = new TableBaseData<OfficeProductItem>();
                        dataOfficeProductItem.SetDictionary(dataOfficeProductItem.LoadTable(x.Value.ToString()).ToDictionary(x => x.productId, x => x));
                    }
                    break;
                case "FunctionTable":
                    {
                        dataFunctionTable = new TableBaseData<FunctionTable>();
                        dataFunctionTable.SetDictionary(dataFunctionTable.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "OfficeShowRoomInfo":
                    {
                        dataOfficeShowRoomInfo = new TableBaseData<OfficeShowRoomInfo>();
                        dataOfficeShowRoomInfo.SetDictionary(dataOfficeShowRoomInfo.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "MapExposulInfo":
                    {
                        dataMapExposulInfo = new TableBaseData<MapExposulInfo>();
                        dataMapExposulInfo.SetDictionary(dataMapExposulInfo.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "MapExposulBrand":
                    {
                        dataMapExposulBrand = new TableBaseData<MapExposulBrand>();
                        dataMapExposulBrand.SetDictionary(dataMapExposulBrand.LoadTable(x.Value.ToString()).ToDictionary(x => (x.mapExposulInfoId, x.brandName), x => x));
                    }
                    break;
                case "NpcCostume":
                    {
                        dataNpcCostume = new TableBaseData<NpcCostume>();
                        dataNpcCostume.SetDictionary(dataNpcCostume.LoadTable(x.Value.ToString()).ToDictionary(x => (x.npcId, x.partsType), x => x)); // ?
                    }
                    break;
                case "NpcList":
                    {
                        dataNpcList = new TableBaseData<NpcList>();
                        dataNpcList.SetDictionary(dataNpcList.LoadTable(x.Value.ToString()).ToDictionary(x => x.id, x => x));
                    }
                    break;
                case "NpcLookType":
                    {
                        dataNpcLookType = new TableBaseData<NpcLookType>();
                        dataNpcLookType.SetDictionary(dataNpcLookType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "NpcType":
                    {
                        dataNpcType = new TableBaseData<NpcType>();
                        dataNpcType.SetDictionary(dataNpcType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "DynamicLinkType":
                    {
                        dataDynamicLinkType = new TableBaseData<DynamicLinkType>();
                        dataDynamicLinkType.SetDictionary(dataDynamicLinkType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "OfficeSeatInfo":
                    {
                        dataOfficeSeatInfo = new TableBaseData<OfficeSeatInfo>();
                        dataOfficeSeatInfo.SetDictionary(dataOfficeSeatInfo.LoadTable(x.Value.ToString()).ToDictionary(x => (x.spaceId, x.num), x => x));
                    }
                    break;
                case "UploadType":
                    {
                        dataUploadType = new TableBaseData<UploadType>();
                        dataUploadType.SetDictionary(dataUploadType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "MoneyType":
                    {
                        dataMoneyType = new TableBaseData<MoneyType>();
                        dataMoneyType.SetDictionary(dataMoneyType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "PostalEffectType":
                    {
                        dataPostalEffectType = new TableBaseData<PostalEffectType>();
                        dataPostalEffectType.SetDictionary(dataPostalEffectType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "PostalItemProperty":
                    {
                        dataPostalItemProperty = new TableBaseData<PostalItemProperty>();
                        dataPostalItemProperty.SetDictionary(dataPostalItemProperty.LoadTable(x.Value.ToString()).ToDictionary(x => (x.itemType, x.categoryType), x => x));
                    }
                    break;
                case "PostalType":
                    {
                        dataPostalType = new TableBaseData<PostalType>();
                        dataPostalType.SetDictionary(dataPostalType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "PostalMoneyProperty":
                    {
                        dataPostalMoneyProperty = new TableBaseData<PostalMoneyProperty>();
                        dataPostalMoneyProperty.SetDictionary(dataPostalMoneyProperty.LoadTable(x.Value.ToString()).ToDictionary(x => x.moneyType, x => x));
                    }
                    break;
                case "KtmfSpecialItem":
                    {
                        dataKtmfSpecialItem = new TableBaseData<KtmfSpecialItem>();
                        dataKtmfSpecialItem.SetDictionary(dataKtmfSpecialItem.LoadTable(x.Value.ToString()).ToDictionary(x => (x.costumeId, x.partsId), x => x));
                    }
                    break;
                case "FileBoxType":
                    {
                        dataFileBoxType = new TableBaseData<FileBoxType>();
                        dataFileBoxType.SetDictionary(dataFileBoxType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "ItemMaterial":
                    {
                        dataItemMaterial = new TableBaseData<ItemMaterial>();
                        dataItemMaterial.SetDictionary(dataItemMaterial.LoadTable(x.Value.ToString()).ToDictionary(x => (x.itemId, x.num), x => x));
                    }
                    break;
                case "NoticeType":
                    {
                        dataNoticeType = new TableBaseData<NoticeType>();
                        dataNoticeType.SetDictionary(dataNoticeType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
                case "NoticeExposureType":
                    {
                        dataNoticeExposureType = new TableBaseData<NoticeExposureType>();
                        dataNoticeExposureType.SetDictionary(dataNoticeExposureType.LoadTable(x.Value.ToString()).ToDictionary(x => x.type, x => x));
                    }
                    break;
            }

            JToken value = x.Value;
        }

        SetDataInit();
        masterDataIsDone = true;
    }

    /// <summary>
    /// 마스터 데이터 파싱 완료 시 데이터 세팅 - 한효주 추가
    /// </summary>
    private void SetDataInit()
    {
        Single.ItemData.LoadResourcesData();
    }

    #region 마이룸 캐싱
    public db.Item GetItem(string prefabName)
    {
        return dataItem.GetList().FirstOrDefault(x => x.prefab == prefabName);
    }
    public db.Item GetItem(int id)
    {
        return dataItem.GetList().FirstOrDefault(x => x.id == id);
    }
    public bool TryGetItem(int id, out Item ret)
    {
        ret = dataItem.GetList().FirstOrDefault(x => x.id == id);

        if (ret == null)
        {
            Debug.LogErrorFormat("cant find item in table : {0}", id);
            return false;
        }
        else
            return true;
    }
    public string GetLayerType(string prefabName)
    {
        db.Item item = GetItem(prefabName);
        int layerType = dataInteriorInstallInfo.GetData(item.id).layerType;
        return dataLayerType.GetData(layerType).name;
    }

    public string GetLayerType(int id)
    {
        int layerType = dataInteriorInstallInfo.GetData(id).layerType;
        return dataLayerType.GetData(layerType).name;
    }

    #endregion

    #region 오피스 데이터 캐싱

    /// <summary>
    /// 오피스 모드 정보 가져옴
    /// </summary>
    public OfficeMode GetOfficeModeData(OfficeModeType type)
    {
        return dataOfficeMode.GetData((int)type);
    }

    /// <summary>
    /// 오피스모드type에 맞는 마스터 데이터 반환 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<OfficeSpaceInfo> GetOfficeSpaceInfoDatas(OfficeModeType type)
    {
        var qry = from data in dataOfficeSpaceInfo.GetList()
                  where data.modeType == (int)type
                  select data;
        var infos = new List<OfficeSpaceInfo>();
        infos.AddRange(qry);

        return infos;
    }



    #endregion

    public Localization GetLocalizeData(string id)
    {
        try
        {
            return dataLocalization.GetData(id);
        }
        catch (System.Exception)
        {
            Debug.Log("bug : " + id);
            throw;
        }
    }
    public Localization GetLocalizationId(string value)
    {
        return Single.MasterData.dataLocalization.GetList().FirstOrDefault(x => x.kor == value || x.eng == value);
    }
}
