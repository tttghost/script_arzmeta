using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameWork.UI;
using FrameWork.Utils;
using MEC;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Unity.Linq;
using System.Threading.Tasks;
using FrameWork.Network;
using Google.Protobuf;
using Protocol;
using Cysharp.Threading.Tasks;

public class InteractionUniqueObj : MonoBehaviour
{
    private GameObject go_Parent;

    private InteractionUnique interactionUnique;

    private Transform go_Interaction;

    private TouchInteractable touchInteractableUI;

    private Outline outline;

    private OutlineFeedback outlineFeedback;

    private InteractionArea interactionArea;

    [FormerlySerializedAs("dist")] [SerializeField] private float activeDistance = 4f;
    //private Transform targetTransform;

    private TouchInteractable touchInteractableObj;

    /// <summary>
    /// 데이터 셋팅
    /// </summary>
    /// <param name="interactionUnique"></param>
    public void SetData(InteractionUnique interactionUnique)
    {
        this.interactionUnique = interactionUnique;

        InitBaseData();

        InitActionData();

        AddHandler_Interaction();
        AddHandler_Realtime();

        OnInteraction(false);
    }

    /// <summary>
    /// 기본데이터 셋팅
    /// </summary>
    private void InitBaseData()
    {
        //제어를 위한 부모오브젝트
        go_Parent = Util.Search(transform, "go_Parent").gameObject;

        //터치UI 오브젝트
        go_Interaction = Util.Search(transform, "go_Interaction");
        go_Interaction.localPosition = interactionUnique.localPosition_Clicker;

        //터치UI 인터렉션
        touchInteractableUI = go_Interaction.GetComponent<TouchInteractable>();
        touchInteractableUI.interactDistance = activeDistance;

        //해당콜라이더에 인터렉션 추가
        touchInteractableObj = transform.parent.gameObject.AddComponent<TouchInteractable>();
        touchInteractableObj.interactDistance = activeDistance;

        //아웃라인
        //outlineFeedback = transform.parent.gameObject.GetComponent<OutlineFeedback>();
        //if (outlineFeedback != null)
        //{
        //    outlineFeedback.Initialize(touchInteractableUI);
        //    outline = transform.parent.gameObject.GetComponent<Outline>();
        //}

        //인터렉션에어리어
        interactionArea = Util.Search<InteractionArea>(go_Parent, "InteractionArea");
        interactionArea.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 기본 이외의 액션데이터 셋팅
    /// </summary>
    private void InitActionData()
    {
        UnityAction action = default;
        switch (interactionUnique.iNTERACTIONUNIQUE_TYPE)
        {
            case eINTERACTIONUNIQUE_TYPE.MIRROR:
                action = SetAction_Mirror;
                break;
            case eINTERACTIONUNIQUE_TYPE.GAME:
            case eINTERACTIONUNIQUE_TYPE.DOOR:
            case eINTERACTIONUNIQUE_TYPE.MAGAZINE:
                action = SetAction_SceneLoad;
                break;
            case eINTERACTIONUNIQUE_TYPE.MAP:
                action = SetAction_OpenMap;
                break;
            case eINTERACTIONUNIQUE_TYPE.MAILBOX:
                action = () =>
                {
                    // 우편함 목록 조회 후 메일함 열기
                    Single.Web.webPostbox.PostboxReq((res) => SceneLogic.instance.PushPanel<Panel_Mailbox>());
                };
                break;
            case eINTERACTIONUNIQUE_TYPE.FRAME:
                action = SetAction_OpenFrame;
                break;
        }

        touchInteractableUI.AddEvent(action);
        touchInteractableObj.AddEvent(action);
    }

    /// <summary>
    /// 앨범 열기 액션
    /// </summary>
    private void SetAction_OpenFrame()
    {
        Panel_MyRoomFrame Panel_MyRoomFrame = SceneLogic.instance.GetPanel<Panel_MyRoomFrame>();
        //FRAME_KIND FRAME_KIND = Util.String2Enum<FRAME_KIND>(gameObject.transform.parent.name);
        //Panel_MyRoomFrame.SetFrame(FRAME_KIND);
        Panel_MyRoomFrame.SetSprite(sprite);
        Panel_MyRoomFrame.SetData(gridItemData);
        SceneLogic.instance.PushPanel<Panel_MyRoomFrame>();
    }



    #region 
    private async void OnEnable()
    {
        await Task.Delay(1);

        if (this == null)
        {
            return;
        }

        switch (interactionUnique?.iNTERACTIONUNIQUE_TYPE)
        {
            case eINTERACTIONUNIQUE_TYPE.MIRROR:
                break;
            case eINTERACTIONUNIQUE_TYPE.GAME:
                break;
            case eINTERACTIONUNIQUE_TYPE.MAP:
                break;
            case eINTERACTIONUNIQUE_TYPE.DOOR:
                break;
            case eINTERACTIONUNIQUE_TYPE.MAGAZINE:
                break;
            case eINTERACTIONUNIQUE_TYPE.MAILBOX:
                LocalPlayerData.Method.Handler_CheckTime += Callback_CheckTime; //메일박스 콜백 - 열고 닫는 애니메이션
                Callback_CheckTime(LocalPlayerData.Method.GetCheckTime());
                break;
            case eINTERACTIONUNIQUE_TYPE.FRAME:
                try //null되는상황 예외처리
                {
                    await UniTask.WaitUntil(() => gridItemData = gameObject.transform.parent.parent.parent.GetComponent<GridItemData>());

                    Util.ProcessQueue("frame", () => OnFrameImage(gridItemData));
                    //OnFrameImage(gridItemData);
                }
                catch
                {

                }
                break;
            default:
                break;
        }
    }
    private void OnDisable()
    {
        switch (interactionUnique?.iNTERACTIONUNIQUE_TYPE)
        {
            case eINTERACTIONUNIQUE_TYPE.MAILBOX:
                LocalPlayerData.Method.Handler_CheckTime -= Callback_CheckTime;
                break;
        }
    }
    private void Callback_CheckTime(bool b)
    {
        try
        {
            gameObject?.Ancestors().OfComponent<Animator>().SingleOrDefault().Play("Animation_Mailbox_" + (b ? "Close" : "Open"));
        }
        catch
        {

        }
    }
    #endregion




    #region 지도
    private string[] mapBtns = new string[] { "40000", "40000", "8009", "40000" };
    private void SetAction_OpenMap()
    {
        Single.Scene.FadeOut(2f, () =>
        {
            Single.Scene.FadeIn(2f);

            SceneLogic.instance.ClosePanel<Panel_HUD>();
            SceneLogic.instance.ClosePanel<Panel_MyRoomPlaymode>();

            GameObject go = Single.Resources.Instantiate<GameObject>(Cons.Path_Interaction + "InteractionUnique_Map");
            Panel_Empty Panel_Empty = SceneLogic.instance.GetPanel<Panel_Empty>();
            Panel_Empty.BackAction_Custom = () => CloseMap(go);
            SceneLogic.instance.PushPanel<Panel_Empty>(false);

            MyPlayer.instance.TPSController.Camera.SetEaseSpeed(0f);
            StartMap(go);
        });
    }
    private void CloseMap(GameObject go)
    {
        Single.Scene.FadeOut(2f, () =>
        {
            Single.Scene.FadeIn(2f);

            SceneLogic.instance.OpenPanel<Panel_HUD>();
            SceneLogic.instance.OpenPanel<Panel_MyRoomPlaymode>();

            Panel_Empty Panel_Empty = SceneLogic.instance.GetPanel<Panel_Empty>();
            Panel_Empty.BackAction_Custom = null;
            SceneLogic.instance.Back();

            MyPlayer.instance.TPSController.Camera.SetEaseSpeed(2f);
            Destroy(go);
        });
    }

    private List<GameObject> img_eff_01List = new List<GameObject>();
    private void StartMap(GameObject go)
    {
        img_eff_01List.Clear();
        Util.Search<Button>(go, "btn_Back").onClick.AddListener(SceneLogic.instance.Back);
        Util.Search<TouchInteractable>(go, "TouchInteractable_Exit").AddEvent(SceneLogic.instance.Back);


        Button[] btns = go.Descendants().Single(x => x.name == "go_Btns").GetComponentsInChildren<Button>();

        for (int i = 0; i < btns.Length; i++)
        {
            int capture = i;
            Button btn = btns[capture];

            //이펙트리스트업
            GameObject img_eff_01 = Util.Search(btn.gameObject, "img_eff_01").gameObject;
            img_eff_01List.Add(img_eff_01);

            //버튼이벤트
            btn.onClick.AddListener(() =>
            {
                string entry = mapBtns[capture];
                BTN_TYPE bTN_TYPE = default;
                PopupAction popupAction = default;
                switch (entry)
                {
                    case "40000":
                        bTN_TYPE = BTN_TYPE.Confirm;
                        popupAction = new PopupAction(
                            null
                            //,() => SetMapEff(-1)
                            );
                        break;
                    case "8009":
                        bTN_TYPE = BTN_TYPE.ConfirmCancel;
                        popupAction = new PopupAction(
                            () => MyRoomManager.Instance.OnChangeWorld(GetInteractionType2RoomType())
                            //() => SetMapEff(-1)
                            );
                        break;
                }

                //팝업
                SceneLogic.instance.PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, bTN_TYPE, new MasterLocalData("common_popup_notice"), new MasterLocalData(entry)))
                 .ChainPopupAction(popupAction);

                //이펙트
                //SetMapEff(capture);
            });
        }

        //이펙초기화
        //SetMapEff(-1);
    }


    /// <summary>
    /// 맵 이펙트 제어
    /// </summary>
    /// <param name="idx"></param>
    private void SetMapEff(int idx)
    {
        for (int i = 0; i < img_eff_01List.Count; i++)
        {
            img_eff_01List[i].SetActive(false);
        }
        if (idx == -1)
        {
            return;
        }
        img_eff_01List[idx].SetActive(true);
    }

    #endregion


    //private bool initHandler = false;
    private void AddHandler_Interaction()
    {
        //initHandler = true;
        //if (!InteractionCustomManager.Instance)
        //{
        //    return;
        //}

        interactionArea._ontriggerEnter.AddListener((col) => OnInteractableArea(true));
        interactionArea._ontriggerExit.AddListener((col) => OnInteractableArea(false));

        InteractionCustomManager.Instance.handlerInteraction += OnInteraction;

        switch (interactionUnique.iNTERACTIONUNIQUE_TYPE)
        {
            case eINTERACTIONUNIQUE_TYPE.MIRROR:
                InteractionCustomManager.Instance.handlerMyRoomModeChange += OnMirror;
                break;
            case eINTERACTIONUNIQUE_TYPE.GAME:
            case eINTERACTIONUNIQUE_TYPE.MAP:
            case eINTERACTIONUNIQUE_TYPE.DOOR:
            case eINTERACTIONUNIQUE_TYPE.MAGAZINE:
            case eINTERACTIONUNIQUE_TYPE.MAILBOX:
            case eINTERACTIONUNIQUE_TYPE.FRAME:
                break;
        }
        InteractionCustomManager.Instance.handlerMyRoomModeChange += OnMyRoomModeChange;
    }


    /// <summary>
    /// 실시간 핸들러 등록
    /// </summary>
    private void AddHandler_Realtime()
    {
        //RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM, this, S_INTERACTION_SET_ITEM);
        //RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM_NOTICE, this, S_INTERACTION_SET_ITEM_NOTICE);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_WILDCARD, this, S_WILDCARD);
    }

    private void S_WILDCARD(PacketSession arg1, IMessage arg2)
    {
        S_WILDCARD packet = arg2 as S_WILDCARD;
        switch ((WILDCARD_TYPE)packet.Code)
        {
            case WILDCARD_TYPE.MYROOM_FRAME:
                {
                    if (interactionUnique.iNTERACTIONUNIQUE_TYPE != eINTERACTIONUNIQUE_TYPE.FRAME)
                    {
                        return;
                    }

                    //프레임 데이터라면 해당 num 인지
                    if (gridItemData == null || int.Parse(packet.Data) != gridItemData.num)
                    {
                        return;
                    }
                    Util.ProcessQueue("frame", () => OnFrameImage(gridItemData));
                    //OnFrameImage(gridItemData);
                }
                break;
            default:
                break;
        }
    }



    //private void S_INTERACTION_SET_ITEM(PacketSession arg1, IMessage arg2)
    //{
    //OnFrameImage(gridItemData);
    //}

    /// <summary>
    /// 인터렉션아이템 노티스 -> 임시로 프레임 이미지용으로 사용
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    //private void S_INTERACTION_SET_ITEM_NOTICE(PacketSession arg1, IMessage arg2)
    //{
    //    S_INTERACTION_SET_ITEM_NOTICE packet = arg2 as S_INTERACTION_SET_ITEM_NOTICE;
    //    S_FrameImage(packet);
    //}


    private void OnDestroy()
    {
        RemoveHandler_Interaction();
        RemoveHandler_Realtime();
    }
    private void RemoveHandler_Interaction()
    {
        //if (!initHandler)
        //{
        //    return;
        //}
        if (!InteractionCustomManager.Instance)
        {
            return;
        }

        interactionArea._ontriggerEnter.RemoveListener((col) => OnInteractableArea(true));
        interactionArea._ontriggerExit.RemoveListener((col) => OnInteractableArea(false));

        InteractionCustomManager.Instance.handlerInteraction -= OnInteraction;

        switch (interactionUnique.iNTERACTIONUNIQUE_TYPE)
        {
            case eINTERACTIONUNIQUE_TYPE.MIRROR:
                InteractionCustomManager.Instance.handlerMyRoomModeChange -= OnMirror;
                break;
            case eINTERACTIONUNIQUE_TYPE.GAME:
            case eINTERACTIONUNIQUE_TYPE.MAP:
            case eINTERACTIONUNIQUE_TYPE.DOOR:
            case eINTERACTIONUNIQUE_TYPE.MAGAZINE:
            case eINTERACTIONUNIQUE_TYPE.MAILBOX:
            case eINTERACTIONUNIQUE_TYPE.FRAME:
                break;
        }
        InteractionCustomManager.Instance.handlerMyRoomModeChange -= OnMyRoomModeChange;
    }
    private void RemoveHandler_Realtime()
    {
        //RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM, this);
        //RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM_NOTICE, this); 
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_WILDCARD, this);
    }





    #region 액자

    private GridItemData gridItemData;
    private Sprite sprite;

    /// <summary>
    /// 룸리스트 안에있는 프레임이미지 가져옴, Callback함수
    /// </summary>
    /// <param name="frameNum"></param>
    private void OnFrameImage(GridItemData gridItemData)
    {
        Single.Web.myRoom.MyRoomOthersRoomList_Req(LocalPlayerData.Method.roomCode,
            async (res) =>
            {
                MyRoomFrameImage[] myRoomFrameImages = res.othersMyRoomFrameImages;
                if (myRoomFrameImages == null || myRoomFrameImages.Length < 1)
                {
                    sprite = null;
                    SetSprite(sprite);
                    return;
                }
                MyRoomFrameImage myRoomFrameImage = myRoomFrameImages.SingleOrDefault(x => x.num == gridItemData.num);

                Texture2D tex_FrameImage = null;
                if (myRoomFrameImage != null)
                {
                    tex_FrameImage = await Util.Co_LoadMyRoomFrame( myRoomFrameImage, true); //콜백할수로 이미지 가져옴
                }

                sprite = Util.Tex2Sprite(tex_FrameImage);
                SetSprite(sprite);
            },
            false);
    }
    private void SetSprite(Sprite sprite)
    {
        InteractionUniqueComponent interactionUniqueComponent = transform.parent.GetComponent<InteractionUniqueComponent>(); //게임오브젝트의 조상중에 InteractionUniqueComponent를 찾고
        if (interactionUniqueComponent == null)
        {
            return;
        }
        Image img_FrameImage = Util.Search<Image>(interactionUniqueComponent.transform, "img_FrameImage");
        if (img_FrameImage == null)
        {
            return;
        }
        img_FrameImage.sprite = sprite;
        Util.ZoomImage_Crop(img_FrameImage);
    }

    #endregion





    [InspectorButton("시작")]
    public void Play()
    {
        InteractionCustomManager.Instance.handlerInteraction?.Invoke(false);
    }

    [InspectorButton("종료")]
    public void Stop()
    {
        InteractionCustomManager.Instance.handlerInteraction?.Invoke(true);
    }


    private void SetAction_Mirror()
    {
        Play();
        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, new MasterLocalData("common_popup_notice"), new MasterLocalData("8012")))
            .ChainPopupAction(new PopupAction(() =>
            {
                Stop();
                SceneLogic.instance.isUILock = false;
                SceneLogic.instance.PushPanel<Panel_CostumeInven>();
            },
            Stop));
    }

    /// <summary>
    /// 인터렉션타입으로 씬타입 추출
    /// </summary>
    /// <returns></returns>
    private RoomType GetInteractionType2RoomType()
    {
        eINTERACTIONUNIQUE_TYPE iNTERACTIONUNIQUE_TYPE = interactionUnique.iNTERACTIONUNIQUE_TYPE;
        RoomType roomType = RoomType.Arz;
        //인터렉션타입으로 씬타입 추출
        switch (iNTERACTIONUNIQUE_TYPE)
        {
            case eINTERACTIONUNIQUE_TYPE.GAME: roomType = RoomType.Game; break;
            case eINTERACTIONUNIQUE_TYPE.MAP: roomType = RoomType.Busan; break;
            case eINTERACTIONUNIQUE_TYPE.MAGAZINE: roomType = RoomType.Store; break;
        }
        return roomType;
    }



    /// <summary>
    /// 씬타입으로 로컬라이징용 엔트리 추출
    /// </summary>
    /// <param name="roomType"></param>
    /// <returns></returns>
    private string GetSceneType2Entry(RoomType roomType)
    {
        //씬타입으로 내용 추출
        string entry = string.Empty;
        switch (roomType)
        {
            case RoomType.Store: entry = "8003"; break;
            case RoomType.Busan: entry = "8009"; break;
            case RoomType.Arz: entry = "8010"; break;
            case RoomType.Game: entry = "8011"; break;
            case RoomType.Festival: entry = "common_notice_move_festivalzone"; break;
        }
        return entry;
    }

    /// <summary>
    /// 씬로더
    /// </summary>
    private void SetAction_SceneLoad()
    {
        RoomType roomType = GetInteractionType2RoomType();
        string entry = GetSceneType2Entry(roomType);

        Play();
        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, new MasterLocalData("common_popup_notice"), new MasterLocalData(entry)))
            .ChainPopupAction(new PopupAction(() => MyRoomManager.Instance.OnChangeWorld(roomType), Stop));
    }



    #region 콜백위주
    /// <summary>
    /// 인터렉터블 영역 enter/exit 콜백 -> 인터렉션UI 활성/비활성
    /// </summary>
    /// <param name="enable"></param>
    private void OnInteractableArea(bool enable)
    {
        go_Interaction.gameObject.SetActive(enable);
    }

    /// <summary>
    /// 인터렉션버튼 클릭시 콜백 -> 인터렉션요소 활성/비활성 (재사용)
    /// </summary>
    /// <param name="enable"></param>
    public void OnInteraction(bool enable)
    {
        OnMyRoomModeChange(enable ? eMyRoomMode.PLAYMODE : eMyRoomMode.EDITMODE);
    }

    /// <summary>
    /// 마이룸모드 체인지 콜백 -> 인터렉션요소 활성/비활성 
    /// </summary>
    /// <param name="enable"></param>
    private void OnMyRoomModeChange(eMyRoomMode myRoomMode)
    {
        switch (myRoomMode)
        {
            case eMyRoomMode.PLAYMODE:
                go_Parent.SetActive(true);
                touchInteractableObj.enabled = true;
                //outline.enabled = true;
                break;
            case eMyRoomMode.EDITMODE:
                go_Parent.SetActive(false);
                touchInteractableObj.enabled = false;
                //outline.enabled = false;
                OnInteractableArea(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 거울 활성/비활성 콜백
    /// </summary>
    /// <param name="myRoomMode"></param>
    private void OnMirror(eMyRoomMode myRoomMode)
    {
        bool enable = false;
        switch (myRoomMode)
        {
            case eMyRoomMode.PLAYMODE: enable = true; break;
            case eMyRoomMode.EDITMODE: enable = false; break;
        }
        Util.Search(transform.parent.parent, "MirrorParent").gameObject.SetActive(enable);
    }
    #endregion

}
