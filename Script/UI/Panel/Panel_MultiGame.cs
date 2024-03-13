using FrameWork.Network;
using FrameWork.UI;
using MEC;
using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_MultiGame : PanelBase, INetworkRoom
{
	#region Members

	[HideInInspector] public RoomType roomType;
	[HideInInspector] public string roomName = string.Empty;

	Transform go_Content;

	Button btn_QuickStart;
	Button btn_Back;
	Button btn_GameGuide;

	Button btn_Create;
	Button btn_Search;
	Button btn_Refresh;

	TMP_Text txtmp_EnterHidden;
	TMP_Text txtmp_Create;
	TMP_Text txtmp_QuickStart;
	TMP_Text txtmp_NoRoom;

	DisplayUGUI displayUGUI;

	Vector2 displaySize = Vector3.zero;
	string thumbnail = string.Empty;
	string logo = string.Empty;

	bool isDim = false;

	CoroutineHandle handle_Refresh;

	#endregion



	#region Initialize

	protected void OnDestroy()
	{
		RealtimeExceptionHandler.callback_error = null;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		isDim = false;

		RefreshList();

		LocalContentsData.scenePortal = Util.String2Enum<ScenePortal>(roomType.ToString());

		handle_Refresh = Timing.RunCoroutine(Co_RefreshList(10f));
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		ClearContents();

		Timing.KillCoroutines(handle_Refresh);
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		go_Content = GetChildGObject(Cons.go_Content).transform;

		//txtmp_EnterHidden = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EnterHidden),new MasterLocalData("205"));
		txtmp_Create = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Create), new MasterLocalData("game_roomcreate"));
		txtmp_QuickStart = GetUI_TxtmpMasterLocalizing(nameof(txtmp_QuickStart), new MasterLocalData("game_start_quick"));
		txtmp_NoRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_NoRoom), new MasterLocalData("game_error_notcreated_yet"));

		btn_Back = GetUI_Button(nameof(btn_Back), () => SceneLogic.instance.PopPanel());

		BackAction_Custom = () => SceneLogic.instance.PopPanel();

		displayUGUI = this.transform.Search(Cons.go_gamePreview).GetComponent<DisplayUGUI>();
	}

	protected override void Start()
	{
		base.Start();

		btn_GameGuide = GetUI_Button(nameof(btn_GameGuide), ShowGameGuide);
		btn_QuickStart = GetUI_Button(nameof(btn_QuickStart), QuickStart);

		btn_Create = GetUI_Button(nameof(btn_Create), () => ShowPopup(Cons.CREATE));
		btn_Search = GetUI_Button(nameof(btn_Search), () => ShowPopup(Cons.SEARCH));
		btn_Refresh = GetUI_Button(nameof(btn_Refresh), () => { isDim = true; RefreshList(); });

		RealtimeExceptionHandler.callback_error = PacketError;
	}

	public void SetGameLogo(GameData _data)
	{
		switch (_data.gameName)
		{
			case Cons.JUMPINGMATCHING:
				roomType = RoomType.JumpingMatching;
				logo = "img_game_jumping_logo";
				thumbnail = "bg_JumpingFrame";
				displaySize = new Vector2(466, 238);
				break;

			case Cons.OXQUIZ:
				roomType = RoomType.OXQuiz;
				logo = "img_game_oxquiz_logo";
				thumbnail = "bg_OXquizframe";
				displaySize = new Vector2(336, 276);
				break;
		}

		Image img_frame = GetUI_Img("img_previewFrame", thumbnail);

		Image img_logo = GetUI_Img("img_logo", logo);
		img_logo.rectTransform.sizeDelta = displaySize;

		displayUGUI.CurrentMediaPlayer = _data.GetComponent<MediaPlayer>();
	}

	private void ShowPopup(string _type)
	{
		PushPopup<Popup_MultiGame>()
			.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, new MasterLocalData("game_find_room"), new MasterLocalData("game_info_roomcode_search"))
		);

		GetPopup<Popup_MultiGame>().SetData(_type);
	}

	private void ShowGameGuide() => GetView(nameof(View_MultigameGuide)).SetActive(true);


	private void QuickStart()
	{
		GameRoomData data = new GameRoomData();
		data.roomType = roomType.ToString();
		data.roomName = roomName;
		data.isPlaying = false;
		data.maxPlayerNumber = 20;
		data.currentPlayerNumber = 1;

		CreateOrJoin(data);
	}

	public void CreateRoom()
	{
		GameRoomData data = new GameRoomData();
		data.roomType = roomType.ToString();
		data.roomName = GetPopup<Popup_MultiGame>().GetRoomCode();
		data.sceneName = RealtimeUtils.GetSceneName(roomType);
		data.maxPlayerNumber = 20;

		CreateAndJoin(data);
	}

	public void SearchRoom()
	{

		var roomCode = GetPopup<Popup_MultiGame>().GetRoomCode();
		SearchAndJoin(roomCode);
	}

	#endregion



	#region Core Methods


	public void JoinRoom<T>(T _roomInfo)
	{
		var roomInfo = _roomInfo as GameRoomInfoRes;

		Single.RealTime.roomType.target = roomType;

		Single.RealTime.JoinRoom(roomInfo);
	}


	public void CreateOrJoin<T>(T _roomData)
	{
		btn_QuickStart.interactable = false;

		var roomData = _roomData as GameRoomData;

		Single.RealTime.SetRoom(roomData);

		Single.RealTime.EnterRoom(roomType);
	}



	public void CreateAndJoin<T>(T _roomData)
	{
		btn_QuickStart.interactable = false;

		var roomData = _roomData as GameRoomData;

		RealtimeWebManager.CreateRoom(roomData);

		RealtimeWebManager.Run<GameRoomInfoRes>(JoinRoom);
	}



	public void SearchAndJoin(string _roomCode)
	{
		RealtimeWebManager.SetQuery(roomType, RealtimeUtils.GetMemberCode());

		RealtimeWebManager.AddQuery(Query.roomCode, _roomCode);

		RealtimeWebManager.GetRoom();

		RealtimeWebManager.Run<GameRoomInfoRes[]>(JoinSearchRoom);
	}

	public void JoinSearchRoom(GameRoomInfoRes[] _roomInfo)
	{
		var popup = GetPopup<Popup_MultiGame>();

		if (_roomInfo.Length <= 0)
		{
			popup.SetCautionMessage("game_error_room_nonexist");
			return;
		}

		var roomInfo = _roomInfo[0];

		if (roomInfo.isPlaying)
		{
			popup.SetCautionMessage("game_error_already_start");
			return;
		}

		JoinRoom(roomInfo);
	}




	public IEnumerator<float> Co_RefreshList(float _refreshRate)
	{
		while (this.enabled)
		{
			yield return Timing.WaitForSeconds(_refreshRate);

			isDim = false;

			RefreshList();
		}
	}

	public void RefreshList()
	{
		RealtimeWebManager.SetQuery(roomType, RealtimeUtils.GetMemberCode());

		RealtimeWebManager.GetRoom();

		RealtimeWebManager.Run<GameRoomInfoRes[]>(GetRoomList);
	}

	private void GetRoomList(GameRoomInfoRes[] _roomInfos) => Util.RunCoroutine(Co_GetRoomList(_roomInfos));

	private IEnumerator<float> Co_GetRoomList(GameRoomInfoRes[] _roomInfos)
	{
		if (isDim) { Single.Scene.SetDimOn(); yield return Timing.WaitForSeconds(.5f); }

		ClearContents();

		GameObject prefab = Resources.Load<GameObject>("Addressable/Prefab/Item/item_GameRoom");

		int index = 0;

		for (int i = 0; i < _roomInfos.Length; i++)
		{
			if (_roomInfos[i].isPlaying) continue;

			GameObject item = Instantiate(prefab, Vector3.zero, Quaternion.identity, go_Content);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			item.transform.name = _roomInfos[i].roomName;

			item.GetComponent<Item_GameRoom>().RefreshUI(_roomInfos[i]);

			index++;
		}

		txtmp_NoRoom.gameObject.SetActive(index <= 0);

		if (Single.Scene.isDim) { Single.Scene.SetDimOff(); isDim = false; }
	}

	private void ClearContents()
	{
		for (int i = 0; i < go_Content.childCount; i++)
		{
			Destroy(go_Content.GetChild(i).gameObject);
		}
	}

	#endregion



	#region Utils


	private bool IsNoForbiddenWords(string _roomName)
	{
		var count = MasterDataManager.Instance.dataForbiddenWords.GetList().Count;

		for (int i = 0; i < count; ++i)
		{
			var words = MasterDataManager.Instance.dataForbiddenWords.GetList()[i].text;

			if (_roomName.Contains(words))
			{
				var popup = GetPopup<Popup_MultiGame>();

				popup.SetCautionMessage("game_error_include_inapposite");

				Single.Scene.SetDimOff();

				return false;
			}
		}

		if (!Util.RegularRoomNameExpression(_roomName) || _roomName == "")
		{
			Single.Scene.SetDimOff();

			return false;
		}

		return true;
	}

	private void PacketError()
	{
		RefreshList();
	}

	#endregion
}