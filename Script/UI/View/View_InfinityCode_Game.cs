using FrameWork.UI;
using Gpm.Ui.Sample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class View_InfinityCode_Game : UIBase
{

    private string layer = "UI";

    public int maxCnt = 40; //최대개수

    public GameObject zero;
    public GameObject one;
    public GameObject virus;
    public GameObject psObj;

    private ParticleSystem ps;
    private int curCnt;
    private List<GameObject> codes = new List<GameObject>();
    private GameObject codesParent = null;
    private GameObject zeroOneParent = null;
    private Button btn_whiteHat;

    private float whiteHatWidth;
    private float whiteHatHeight;

    private float moveSpeed_forward = 1f;
    private float fadeTime_forward = 0.5f;

    private float moveSpeed_side = 5f;
    private float fadeTime_side = 0.5f;

    private int minFade = 4;//코드페이드 최소갯수시작
    private int maxFade = 8;//코드페이드 최대갯수끝

    private int minVirus = 1;//바이러스 최소갯수
    private int maxVirus = 5;//바이러스 최대갯수

    private LTDescr lTDescr_Forward; //다가오는 린트윈

    private Panel_InfinityCode panel_InfinityCode;

    private bool isOnce = false;
    private bool isOnce2 = false;
    private Button btn_target_0;
    private Button btn_target_1;
    private Image img_whiteHatBG;
    private List<int> virusList = new List<int>();


    #region test

    public Button btn_Save;
    public Button btn_ResetData;
    public Button btn_Cheat;

    public GameObject go_DummyPanel;
    public Slider s_cnt;
    public Slider s_minVirus;
    public Slider s_maxVirus;
    public Slider s_forwardMove;
    public Slider s_forwardFade;
    public Slider s_sideMove;
    public Slider s_sideFade;
    public Slider s_time;

    public TMP_Text req_cnt;
    public TMP_Text req_minVirus;
    public TMP_Text req_maxVirus;
    public TMP_Text req_forwardMove;
    public TMP_Text req_forwardFade;
    public TMP_Text req_sideMove;
    public TMP_Text req_sideFade;
    public TMP_Text req_time;

    public TMP_Text res_cnt;
    public TMP_Text res_minVirus;
    public TMP_Text res_maxVirus;
    public TMP_Text res_forwardMove;
    public TMP_Text res_forwardFade;
    public TMP_Text res_sideMove;
    public TMP_Text res_sideFade;
    public TMP_Text res_time;



    private void Dummy()
    {
        btn_Save = GetUI_Button("btn_Save", Save);
        btn_ResetData = GetUI_Button("btn_ResetData", ResetData);
        btn_Cheat = GetUI_Button("btn_Cheat", Cheat);

        s_cnt.onValueChanged.AddListener((f) => { req_cnt.text = f.ToString(); });
        s_minVirus.onValueChanged.AddListener((f) => { req_minVirus.text = f.ToString(); });
        s_maxVirus.onValueChanged.AddListener((f) => { req_maxVirus.text = f.ToString(); });
        s_forwardMove.onValueChanged.AddListener((f) => { req_forwardMove.text = f.ToString("###.##"); });
        s_forwardFade.onValueChanged.AddListener((f) => { req_forwardFade.text = f.ToString("###.##"); });
        s_sideMove.onValueChanged.AddListener((f) => { req_sideMove.text = f.ToString("###.##"); });
        s_sideFade.onValueChanged.AddListener((f) => { req_sideFade.text = f.ToString("###.##"); });
        s_time.onValueChanged.AddListener((f) => { req_time.text = f.ToString(); });
    }

    private void InitData()
    {
        s_cnt.onValueChanged.Invoke(40);
        s_minVirus.onValueChanged.Invoke(1);
        s_maxVirus.onValueChanged.Invoke(5);
        s_forwardMove.onValueChanged.Invoke(1f);
        s_forwardFade.onValueChanged.Invoke(0.5f);
        s_sideMove.onValueChanged.Invoke(5f);
        s_sideFade.onValueChanged.Invoke(0.5f);
        s_time.onValueChanged.Invoke(40);
    }

    private void Save()
    {
        res_cnt.text = req_cnt.text;
        maxCnt = int.Parse(res_cnt.text);

        res_minVirus.text = req_minVirus.text;
        minVirus = int.Parse(res_minVirus.text);

        res_maxVirus.text = req_maxVirus.text;
        maxVirus = int.Parse(res_maxVirus.text);

        res_forwardMove.text = req_forwardMove.text;
        moveSpeed_forward = float.Parse(res_forwardMove.text);

        res_forwardFade.text = req_forwardFade.text;
        fadeTime_forward = float.Parse(res_forwardFade.text);

        res_sideMove.text = req_sideMove.text;
        moveSpeed_side = float.Parse(res_sideMove.text);

        res_sideFade.text = req_sideFade.text;
        fadeTime_side = float.Parse(res_forwardFade.text);

        res_time.text = req_time.text;
        maxTime = int.Parse(res_time.text);
        ResetData();
    }
    #endregion

    #region heart관련
    private List<Toggle> heartTogList = new List<Toggle>();
    private int _heart; //목숨
    public int heart
    {
        get
        {
            return _heart;
        }
        set
        {
            _heart = value;
            SetHeart(_heart);
        }
    }
    private void SetHeart(int idx)
    {
        //idx--; //1개 깍아야 함 배열과 인덱스 동기화
        for (int i = heartTogList.Count - 1; i >= 0; i--)
        {
            heartTogList[i].isOn = idx > i ? true : false;
        }
    }

    /// <summary>
    /// 목숨을 사용하기 위한 초기셋팅
    /// </summary>
    private void InitHeart()
    {
        heartTogList.Add(GetUI_Toggle("tog_heart1"));
        heartTogList.Add(GetUI_Toggle("tog_heart2"));
        heartTogList.Add(GetUI_Toggle("tog_heart3"));

        for (int i = 0; i < heartTogList.Count; i++)
        {
            int capture = i;
            heartTogList[capture].onValueChanged.AddListener((b) => { if (!b) heartTogList[capture].GetComponentInChildren<ParticleSystem>().Play(); });
        }
    }
    #endregion

    #region timer관련
    private Dictionary<string, Sprite> numDic = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> numDic2 = new Dictionary<string, Sprite>();
    private List<Image> timerImageList = new List<Image>();
    private List<Image> maxTimeImageList = new List<Image>();
    private float _timer;
    public float timer //시간
    {
        get
        {
            return _timer;
        }
        set
        {
            _timer = value;
            SetTimer(_timer);
        }
    }

    private float _maxTime;
    public float maxTime //시간
    {
        get
        {
            return _maxTime;
        }
        set
        {
            _maxTime = value;
            SetMaxTime(_maxTime);
        }
    }
    private void SetTimer(float curTime)
    {
        if (!isOnce)
        {
            isOnce = true;
            for (int i = 0; i < 10; i++)
            {
                string path = $"{Cons.Path_Image}Img_num_{i}";
                numDic.Add(i.ToString(), Single.Resources.Load<Sprite>(path));
            }
        }
        string timer = Mathf.Floor((curTime * 100f)).ToString("0000");
        for (int i = 0; i < timerImageList.Count; i++)
        {
            timerImageList[i].sprite = numDic[timer[i].ToString()];
        }
    }

    private void SetMaxTime(float time)
    {
        if (!isOnce2)
        {
            isOnce2 = true;
            for (int i = 0; i < 10; i++)
            {
                string path = $"{Cons.Path_Image}img_num2_{i}";
                numDic2.Add(i.ToString(), Single.Resources.Load<Sprite>(path));
            }
        }
        string timer = time.ToString("00");
        for (int i = 0; i < maxTimeImageList.Count; i++)
        {
            maxTimeImageList[i].sprite = numDic2[timer[i].ToString()];
        }
    }

    /// <summary>
    /// 타이머를 작동하기위한 초기 셋팅
    /// </summary>
    private void InitTimer()
    {
        timerImageList.Add(GetUI_Img("img_10"));
        timerImageList.Add(GetUI_Img("img_1"));
        timerImageList.Add(GetUI_Img("img_01"));
        timerImageList.Add(GetUI_Img("img_001"));

        maxTimeImageList.Add(GetUI_Img("img_10_max"));
        maxTimeImageList.Add(GetUI_Img("img_1_max"));
        maxTime = 40f;
    }
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        panel_InfinityCode = GetPanel<Panel_InfinityCode>();

        btn_target_0 = GetUI_Button(nameof(btn_target_0), () => RM("0"), string.Empty);
        btn_target_1 = GetUI_Button(nameof(btn_target_1), () => RM("1"), string.Empty);
        GetUI_Button("btn_guide", Back);

        GetUI_TxtmpMasterLocalizing("txtmp_guide",new MasterLocalData("common_back"));
        btn_whiteHat = GetUI_Button(nameof(btn_whiteHat), () => RM("virus"));
        img_whiteHatBG = GetUI_Img(nameof(img_whiteHatBG));
        whiteHatWidth = btn_whiteHat.GetComponent<RectTransform>().sizeDelta.x / 2f;
        whiteHatHeight = btn_whiteHat.GetComponent<RectTransform>().sizeDelta.y / 2f;
        InitTimer();
        InitHeart();
#if UNITY_EDITOR
        Dummy();
        InitData();
#endif
    }
    protected override void Start()
    {
        base.Start();

        volume = GameObject.Find("Post Process_UI").GetComponent<Volume>();
        if (volume.profile.TryGet(out colorAdjustments))
        {

        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        ResetData();
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        Destroy(codesParent);
        colorAdjustments.colorFilter.value = color_normal;
    }

    /// <summary>
    /// 바이러스 생성
    /// </summary>
    private void CreateVirus()
    {
        isVirusTime = false;
        int virusCnt = Random.Range(minVirus, maxVirus + 1);
        virusList.Clear();
        for (int i = 0; i < virusCnt; i++)
        {
            int virusIdx = Random.Range(1, maxCnt);
            if (virusList.Contains(virusIdx)) //같은게 나오면?
            {
                i--;
                continue;
            }
            virusList.Add(virusIdx);
        }
    }
    private void ResetData()
    {
        LeanTween.reset();
        img_whiteHatBG.gameObject.SetActive(false);
        btn_whiteHat.gameObject.SetActive(false);

        if (codesParent != null)
        {
            Destroy(codesParent);
        }
        codesParent = new GameObject();

        ps = Instantiate(psObj, codesParent.transform).GetComponent<ParticleSystem>();

        zeroOneParent = new GameObject();
        zeroOneParent.transform.SetParent(codesParent.transform);
        zeroOneParent.layer = LayerMask.NameToLayer(layer);

        curCnt = 0;
        timer = 0f;
        heart = 3;

        CreateVirus();
        CreateCode();

        MinShow();
        MinMaxFade();
        MaxHide();
    }

    /// <summary>
    /// 코드 생성
    /// </summary>
    private void CreateCode()
    {
        codes.Clear();
        for (int i = 0; i < maxCnt; i++)
        {
            int ranNum = Random.Range(0, 2);
            if (ranNum == 0)
            {
                GameObject go = Instantiate(zero);
                go.layer = LayerMask.NameToLayer(layer);
                go.name = "0";
                if (Random.Range(0, 6) == 1 || virusList.Contains(i)) //20%확률로 변이됨, 바이러스에 걸리면 무조껀 변이됨
                {
                    go.GetComponent<SpriteRenderer>().color = Cons.Color_HotPink;
                    go.transform.GetChild(0).gameObject.SetActive(true);
                }

                codes.Add(go);
            }
            else
            {
                GameObject go = Instantiate(one);
                go.layer = LayerMask.NameToLayer(layer);
                go.name = "1";
                if (Random.Range(0, 6) == 1 || virusList.Contains(i)) //20%확률로 변이됨, 바이러스에 걸리면 무조껀 변이됨
                {
                    go.GetComponent<SpriteRenderer>().color = Cons.Color_HotPink;
                    go.transform.GetChild(0).gameObject.SetActive(true);
                }
                codes.Add(go);
            }

            codes[i].transform.SetParent(zeroOneParent.transform);
            codes[i].transform.localPosition = Vector3.forward * i * 2f;
        }
    }

    private void GoodCamera()
    {
        CameraShakeManager.Instance.Play(Cons.Path_Prefab_GameObject + "Camera Shakes/InfinityCodeCameraShake_Good");
    }
    private void BadCamera()
    {
        CameraShakeManager.Instance.Play(Cons.Path_Prefab_GameObject + "Camera Shakes/InfinityCodeCameraShake_Bad");
        StartCoroutine(Co_CameraShakePostProcessing(twinkleTime, turmTime, maxTwinkle));
    }

    private void MinShow()
    {
        for (int i = 0; i < minFade; i++)
        {
            SpriteRenderer sr = codes[i].GetComponent<SpriteRenderer>();
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 1f);
        }
    }
    private void MinMaxFade()
    {
        for (int i = curCnt + minFade; i < curCnt + maxFade; i++)
        {
            int capture = i;
            float f = Mathf.InverseLerp(curCnt + maxFade, curCnt + minFade, capture);
            if (capture < codes.Count)
            {
                SpriteRenderer sr = codes[capture].GetComponent<SpriteRenderer>();
                LeanTween.color(codes[capture], new Color(sr.color.r, sr.color.g, sr.color.b, f), fadeTime_forward);
            }
        }
    }
    private void MaxHide()
    {
        for (int i = maxFade; i < codes.Count; i++)
        {
            SpriteRenderer sr = codes[i].GetComponent<SpriteRenderer>();
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 0f);
        }
    }


    /// <summary>
    /// 코드 제거
    /// </summary>
    private void RemoveCode()
    {
        GoodCamera();

        GameObject go = codes[curCnt];

        go.transform.SetParent(codesParent.transform);
        go.GetComponent<Animator>().Play("Ani_hit");
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        LeanTween.color(go, new Color(sr.color.r, sr.color.g, sr.color.b, 0f), fadeTime_forward);
        if (isVirusTime)
        {
            Single.Sound.PlayEffect("effect_virushit_0");
            LeanTween.move(go, btn_whiteHat.transform.position, fadeTime_side).setOnComplete(() => go.SetActive(false)); //옆으로 사라지기
            isVirusTime = false;
        }
        else
        {
            Single.Sound.PlayEffect($"effect_poshit_{Random.Range(0, 4)}");
            //옆으로 숫자 사라지기
            int dir = go.name == "0" ? -1 : 1;
            LeanTween.moveX(go, moveSpeed_side * dir, fadeTime_side).setOnComplete(() => go.SetActive(false)); //옆으로 사라지기
        }

        curCnt++;
        MinMaxFade();
        if (virusList.Contains(curCnt))
        {
            //Debug.Log("virus!!!");
            GameObject go2 = codes[curCnt];
            GameObject virusObj = Instantiate(virus, zeroOneParent.transform);
            virusObj.name = "virus";
            virusObj.transform.localPosition = go2.transform.localPosition;

            codes.Remove(go2);
            codes.Insert(curCnt, virusObj);

            Destroy(go2);

            SetWhiteHat();


            isVirusTime = true;
        }

        if (lTDescr_Forward != null)
        {
            lTDescr_Forward.pause();
        }
        lTDescr_Forward = LeanTween.moveZ(zeroOneParent, -curCnt * 2f, moveSpeed_forward).setEase(LeanTweenType.easeOutExpo); // 앞으로 다가오기

        ps.Play();
    }

    /// <summary>
    /// 화이트햇 랜덤배치
    /// </summary>
    private void SetWhiteHat()
    {
        img_whiteHatBG.gameObject.SetActive(true);
        Debug.Log("SceneLogic.instance.screenWidth: " + SceneLogic.instance.screenWidth);
        float x = Random.Range(SceneLogic.instance.screenWidth / -2f + whiteHatWidth, SceneLogic.instance.screenWidth / 2f - whiteHatWidth);
        float y = Random.Range(SceneLogic.instance.screenHeight / -2f + whiteHatHeight, SceneLogic.instance.screenHeight / 2f - whiteHatHeight);
        btn_whiteHat.transform.localPosition = new Vector2(x, y);
        btn_whiteHat.gameObject.SetActive(true);
    }




    private bool isVirusTime; //바이러스타임

    /// <summary>
    /// 코드일치여부 판단
    /// </summary>
    /// <param name="str"></param>
    private void RM(string str)
    {
        img_whiteHatBG.gameObject.SetActive(false);
        btn_whiteHat.gameObject.SetActive(false);
        if (codes[curCnt].name == str)
        {
            RemoveCode();
        }
        else if (codes[curCnt].name == "virus" && codes[curCnt].name != str)
        {
            GameOver("game_result_infinitecode_infection");
            return;
        }
        else
        {
            BadCamera();
            Single.Sound.PlayEffect("effect_miss_0");
            heart--;
        }
    }
    private void Update()
    {
        return;
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            go_DummyPanel.SetActive(!go_DummyPanel.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            GoodCamera();
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            BadCamera();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            SetWhiteHat();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetData();
        }

        if (heart == 0)
        {
            GameOver("game_result_infinitecode_exhausted");
            return;
        }
        if (timer > maxTime)
        {
            GameOver("game_result_infinitecode_timeover");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Comma)) btn_target_0.onClick.Invoke(); //0
        if (Input.GetKeyDown(KeyCode.Period)) btn_target_1.onClick.Invoke(); ; //1
        if (Input.GetKeyDown(KeyCode.Slash)) Cheat();

        if (curCnt == 0 && heart == 3)
        {
            return;
        }

        if (curCnt == maxCnt)
        {
            GameClear();
            return;
        }

        timer += Time.deltaTime;
    }

    /// <summary>
    /// 치트키
    /// </summary>
    private void Cheat()
    {
        RM(codes[curCnt].name);
    }


    /// <summary>
    /// 게임클리어시
    /// </summary>
    private void GameClear()
    {
        Destroy(codesParent);
        panel_InfinityCode.ChangeView<View_InfinityCode_Success>().SetData(timer);

        Single.Web.Quest_Process_Req(6, Single.Web.Quest_Process_Res); //무한의코드 플레이 퀘스트 프로세스
    }

    /// <summary>
    /// 게임오버시
    /// </summary>
    /// <param name="failIdx"></param>
    private void GameOver(string failIdx)
    {
        Destroy(codesParent);
        panel_InfinityCode.ChangeView<View_InfinityCode_Gameover>().SetData(failIdx);

        Single.Web.Quest_Process_Req(6, Single.Web.Quest_Process_Res); //무한의코드 플레이 퀘스트 프로세스
    }


    [Header("BadTwinkle")]
    private Color color_normal = Color.white;
    public Color color_bad = new Color(1f, .5f, .5f);
    public float twinkleTime = 0.5f;
    public float turmTime = 0.1f;
    public int maxTwinkle = 1;
    private IEnumerator Co_CameraShakePostProcessing(float twinkleTime, float turmTime, int maxTwinkle)
    {
        float halpTime = twinkleTime / 2f;
        int randomTwinkle = Random.Range(0, maxTwinkle) + 1;
        for (int i = 0; i < randomTwinkle; i++)
        {
            yield return Co_CameraShakePostProcessing2(color_normal, color_bad, halpTime);
            yield return Co_CameraShakePostProcessing2(color_bad, color_normal, halpTime);
            yield return new WaitForSeconds(turmTime);
        }
    }
    private Volume volume;
    private ColorAdjustments colorAdjustments;
    private IEnumerator Co_CameraShakePostProcessing2(Color startColor, Color endColor, float durTime)
    {
        ///총시간
        ///트윙클시간
        ///트윙클안하는시간
        ///최대횟수(최소1)
        ///컬러값
		float curTime = 0f;
        while (curTime < 1f)
        {
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, endColor, curTime += Time.deltaTime / durTime);
            yield return null;
        }
    }
}


