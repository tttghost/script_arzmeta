using Newtonsoft.Json;
using Program;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using db;
public class DataManager : Singleton<DataManager>
{
    #region 마스터 데이터 매니저 코드

    //private MasterData masterData = new MasterData();

    //private bool isLoadData = false;

    //#region 초기화
    //private void Awake()
    //{
    //    if (!isLoadData) LoadMasterData();
    //}

    ///// <summary>
    ///// 마스터 데이터 로드
    ///// </summary>
    //public void LoadMasterData()
    //{
    //    isLoadData = true;

    //    masterData = null;

    //    TextAsset asset = Resources.Load<TextAsset>(Cons.Path_MasterData + "master");
    //    if (asset != null) masterData = JsonConvert.DeserializeObject<MasterData>(asset.ToString());

    //    SetDatas();
    //}

    ///// <summary>
    ///// 초기 데이터 세팅
    ///// 다른 스크립트에서 초기에 세팅해야 할 데이터가 있다면 여기에 해당 데이터를 보내주는 코드 작성 
    ///// </summary>
    //private void SetDatas()
    //{
    //    if (masterData == null) return;

    //    Single.AvatarData.LoadResourcesData(GetAvatarPartsArray());
    //}
    //#endregion

    //#region 마스터 데이터 전체 가져오기
    ///// <summary>
    ///// 마스터 데이터 전체 가져오기
    ///// </summary>
    ///// <returns></returns>
    //public MasterData GetSetMasterData()
    //{
    //    if (masterData != null)
    //    {
    //        return masterData;
    //    }
    //    return null;
    //}
    //#endregion

    //#region 데이터 겟셋 Dictionary
    ///// <summary>
    ///// 아바타 파츠 데이터 겟셋 Dictionary
    ///// </summary>
    //public Dictionary<int, AvatarParts> GetAvatarPartsDic()
    //{
    //    if (masterData.avatarParts != null)
    //    {
    //        Dictionary<int, AvatarParts> valuePairs = masterData.avatarParts.ToDictionary(x => x.id, x => x);
    //        return valuePairs;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// FAQ 데이터 겟셋 Dictionary
    ///// </summary>
    ///// <returns></returns>
    //public Dictionary<int, Faq> GetFAQDic()
    //{
    //    if (masterData.FAQ != null)
    //    {
    //        Dictionary<int, Faq> valuePairs = masterData.FAQ.ToDictionary(x => x.id, x => x);
    //        return valuePairs;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 금칙어 데이터 겟셋 Dictionary
    ///// </summary>
    ///// <returns></returns>
    //public Dictionary<int, ForbiddenWords> GetForbiddenWordsDic()
    //{
    //    if (masterData.forbiddenWords != null)
    //    {
    //        Dictionary<int, ForbiddenWords> valuePairs = masterData.forbiddenWords.ToDictionary(x => x.id, x => x);
    //        return valuePairs;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 퀘스트 데이터 겟셋 Dictionary
    ///// </summary>
    ///// <returns></returns>
    //public Dictionary<int, Quest> GetQuestDic()
    //{
    //    if (masterData.quest != null)
    //    {
    //        Dictionary<int, Quest> valuePairs = masterData.quest.ToDictionary(x => x.id, x => x);
    //        return valuePairs;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 퀘스트 리워드 데이터 겟셋 Dictionary
    ///// </summary>
    ///// <returns></returns>
    //public Dictionary<int, QuestReward> GetQuestRewardDic()
    //{
    //    if (masterData.questReward != null)
    //    {
    //        Dictionary<int, QuestReward> valuePairs = masterData.questReward.ToDictionary(x => x.id, x => x);
    //        return valuePairs;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 기기 종류에 따른 스토어 주소 Dictionary
    ///// </summary>
    ///// <returns></returns>
    //public Dictionary<int, OsType> GetOsTypeDic()
    //{
    //    if (masterData.osType != null)
    //    {
    //        Dictionary<int, OsType> valuePairs = masterData.osType.ToDictionary(x => x.type, x => x);
    //        return valuePairs;
    //    }
    //    return null;
    //}
    //#endregion

    //#region 데이터 겟셋 Array
    ///// <summary>
    ///// 아바타 파츠 데이터 겟셋 Array
    ///// </summary>
    //public AvatarParts[] GetAvatarPartsArray()
    //{
    //    if (masterData.avatarParts != null)
    //    {
    //        return masterData.avatarParts;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// FAQ 데이터 겟셋 Array
    ///// </summary>
    ///// <returns></returns>
    //public Faq[] GetFAQArray()
    //{
    //    if (masterData.FAQ != null)
    //    {
    //        return masterData.FAQ;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 금칙어 데이터 겟셋 Array
    ///// </summary>
    ///// <returns></returns>
    //public ForbiddenWords[] GetForbiddenWordsArray()
    //{
    //    if (masterData.forbiddenWords != null)
    //    {
    //        return masterData.forbiddenWords;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 퀘스트 데이터 겟셋 Array
    ///// </summary>
    ///// <returns></returns>
    //public Quest[] GetQuestArray()
    //{
    //    if (masterData.quest != null)
    //    {
    //        return masterData.quest;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 퀘스트 리워드 데이터 겟셋 Array
    ///// </summary>
    ///// <returns></returns>
    //public QuestReward[] GetQuestRewardArray()
    //{
    //    if (masterData.questReward != null)
    //    {
    //        return masterData.questReward;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 기기 종류에 따른 스토어 주소 Array
    ///// </summary>
    ///// <returns></returns>
    //public OsType[] GetOsTypeArray()
    //{
    //    if (masterData.osType != null)
    //    {
    //        return masterData.osType;
    //    }
    //    return null;
    //}
    //#endregion

    #endregion

    #region 구 데이터 매니저 코드
    #region 로드만 하는 데이터 딕셔너리

    private AppSettingData appSettingData;
    private const string emojiDataPath = "";
    #endregion

    // TODO : Cons로 이동
    private const string characterDataJsonFileName = "CharacterData.json";
    private const string AppSettingDataJsonFileName = "AppSettingData.json";
    private const string localDataFolderName = "Datas";
    private const string Extension = ".json";

    // 캐릭터 데이터 json 파일


    private string localDataFolderPath = null;
    private string characterDataJsonFilePath = null;
    private string appSettingDataJsonFilePath = null;

    protected override void AWAKE()
    {
        base.AWAKE();

        localDataFolderPath = Path.Combine(Application.persistentDataPath, localDataFolderName);
        characterDataJsonFilePath = Path.Combine(localDataFolderPath, characterDataJsonFileName);
        appSettingDataJsonFilePath = Path.Combine(localDataFolderPath, AppSettingDataJsonFileName);

        DEBUG.LOG($"localDataFolderPath : {localDataFolderPath}", eColorManager.DATA);
    }

    protected override void START()
    {
        base.START();
        //LoadResourcesDatas();
        //LoadAppSettingData();
    }
    public void LoadResourcesDatas()
    {
        TextAsset txtEmojiData = Resources.Load(emojiDataPath) as TextAsset;
    }


    private bool IsTextAssetNullOrEmpty(TextAsset txtAppSettingData, TextAsset txtCharacterData)
    {
        // 저장하는 데이터 null 체크 로직
        if (string.IsNullOrEmpty(txtAppSettingData.text))
        {
            Debug.Log("txtAppSettingData is null");
            return false;
        }

        if (string.IsNullOrEmpty(txtCharacterData.text))
        {
            Debug.Log("txtCharacterData is null");
            return false;
        }
        return true;
    }

    #region 딕셔너리에서 데이터 id로 가져오는 함수




    #endregion



    // 데이터 저장
    public void SaveLocalData(string jsonName, string jsonData)
    {
        string path = Path.Combine(localDataFolderPath, jsonName + Extension);
        // 로컬에 파일이 있는지 체크
        if (File.Exists(path))
        {
            Debug.Log("oldUser Folder path : " + localDataFolderPath);
            File.WriteAllText(path, jsonData);
        }
        else
        {
            // 없는 경우 새로운 유저

            // 디렉토리가 없는 경우 디렉토리 생성
            if (!Directory.Exists(localDataFolderPath))
            {
                Directory.CreateDirectory(localDataFolderPath);
            }
            File.WriteAllText(path, jsonData);
        }
    }


    public bool IsExistsJsonData(string jsonFile)
    {
        string path = Path.Combine(localDataFolderPath, jsonFile);
        print("localDataPath + jsonFile: " + path);
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LoadAppSettingData()
    {
        if (!File.Exists(appSettingDataJsonFilePath))
        {
            //Debug.Log(localDataFolderPath + "에 AppDataJson 파일이 존재하지 않습니다.");

            // AppSettingData가 존재하지 않는경우 생성
            AppSettingData appSettingData = new AppSettingData(2000, 0, 0);
            string json = JsonConvert.SerializeObject(appSettingData);
            SaveLocalData("AppSettingData", json);
        }

        string txtAppSettingData = File.ReadAllText(appSettingDataJsonFilePath);

        appSettingData = JsonConvert.DeserializeObject<AppSettingData>(txtAppSettingData);
    }
    #endregion
}

namespace Program
{
    public class AppSettingData
    {

        public int id;

        public int bgm_on;

        public int ui_guid_on;

        public AppSettingData(int id, int bgm_on, int ui_guid_on)
        {
            this.id = id;
            this.bgm_on = bgm_on;
            this.ui_guid_on = bgm_on;
        }
    }
}
