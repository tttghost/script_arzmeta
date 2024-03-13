using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Messanger : PanelBase
{
	Transform content_friend;
	Transform content_chat;

	ScrollRect scrollRect_friendList;
	ScrollRect scrollRect_chatList;

	Button btn_frinedList;
	Button btn_chatList;
	Button btn_banned;
	Button btn_addFriend;
	Button btn_request;

	Button btn_back;
	GameObject block;

	[HideInInspector] public bool initialized;

	[Space(5), Header("List")]
	public List<string> friendNameList;
	public List<GameObject> friendList = new List<GameObject>();

	[Space(5), Header("Button Color")]
	public Color defaultColor;
	public Color pressedColor;

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		this.GetComponent<CanvasGroup>().alpha = 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	private void OnDestroy()
	{
		RemoveListeners();
	}

	private void OnEnable()
	{
		if (initialized) return;

		Util.RunCoroutine(Init());
	}

	private void OnDisable()
	{
		if (block != null)
		{
			ChangeView(scrollRect_friendList.gameObject.name);
			btn_frinedList.GetComponent<Image>().color = pressedColor;
			btn_chatList.GetComponent<Image>().color = defaultColor;

			block.SetActive(false);

			SceneLogic.instance.OpenPanel<Panel_HUD>().canvasGroup.interactable = true;
		}
	}

	IEnumerator<float> Init()
	{
		yield return Timing.WaitUntilTrue(() => ArzMetaManager.Instance);
		
		btn_back = Util.Search<Button>(this.gameObject, Define.Btn_Back);
		block = Util.Search(this.gameObject, Define.Img_Block).gameObject;

		scrollRect_friendList = Util.Search<ScrollRect>(this.gameObject, "View_FriendList");
		content_friend = Util.Search(scrollRect_friendList.gameObject, "Content");

		scrollRect_chatList = Util.Search<ScrollRect>(this.gameObject, "View_ChatList");
		content_chat = Util.Search(scrollRect_chatList.gameObject, "Content");

		btn_frinedList = Util.Search<Button>(this.gameObject, "Btn_FriendList");
		btn_chatList = Util.Search<Button>(this.gameObject, "Btn_ChatList");
		btn_banned = Util.Search<Button>(this.gameObject, "Btn_Banned");
		btn_addFriend = Util.Search<Button>(this.gameObject, "Btn_Add");
		btn_request = Util.Search<Button>(this.gameObject, "Btn_Request");

		CacheListeners();

		btn_frinedList.GetComponent<Image>().color = pressedColor;
		btn_chatList.GetComponent<Image>().color = defaultColor;
		block.SetActive(false);

		initialized = true;
	}

	private void CacheListeners()
	{
		btn_back.onClick.AddListener(BackButtonEvent);
		btn_frinedList.onClick.AddListener(FriendListButtonEvent);
		btn_chatList.onClick.AddListener(ChatListButtonEvent);
		btn_banned.onClick.AddListener(()=>OpenPopUp(Define.Popup_FriendBanned));
		btn_addFriend.onClick.AddListener(() => OpenPopUp(Define.Popup_FriendAdd));
		btn_request.onClick.AddListener(() => OpenPopUp(Define.Popup_FriendRequest));
	}

	private void RemoveListeners()
	{
		if (!initialized) return;

		btn_back.onClick.RemoveAllListeners();
		btn_frinedList.onClick.RemoveAllListeners();
		btn_chatList.onClick.RemoveAllListeners();
		btn_banned.onClick.RemoveAllListeners();
		btn_addFriend.onClick.RemoveAllListeners();
		btn_request.onClick.RemoveAllListeners();
	}

	private void BackButtonEvent()
	{
		Show(false);

		SceneLogic.instance.PushPanel<Panel_Phone>();
	}

	private void FriendListButtonEvent()
	{
		ChangeView(scrollRect_friendList.gameObject.name);

		Util.RunCoroutine(
			FadeUtils.SetColorSequance(btn_frinedList.GetComponent<Image>(), pressedColor, 2f), 
			Define.ChangeColor + "btn_frinedList" + this.GetHashCode(),
			Util.SceneCoroutine.World
		);

		Util.RunCoroutine(
			FadeUtils.SetColorSequance(btn_chatList.GetComponent<Image>(), defaultColor, 2f), 
			Define.ChangeColor + "btn_chatList" + this.GetHashCode(), 
			Util.SceneCoroutine.World
		);
	}

	private void ChatListButtonEvent()
	{
		ChangeView(scrollRect_chatList.gameObject.name);

		Util.RunCoroutine(FadeUtils.SetColorSequance(btn_frinedList.GetComponent<Image>(), defaultColor, 2f), Define.ChangeColor + "btn_frinedList" + this.GetHashCode(), Util.SceneCoroutine.World);
		Util.RunCoroutine(FadeUtils.SetColorSequance(btn_chatList.GetComponent<Image>(), pressedColor, 2f), Define.ChangeColor + "btn_chatList" + this.GetHashCode(), Util.SceneCoroutine.World);
	}

	public void RefreshFriendList()
	{
		for (int i = 0; i < friendNameList.Count; i++)
		{
			CreateFriend(friendNameList[i]);
		}
	}

	public void CreateFriend(string _nickname)
	{
		GameObject friend = ObjectPooler.instance.GetPooledObject(Define.Element_Profile);

		friend.transform.SetParent(content_friend);
		friend.transform.localPosition = Vector3.zero;
		friend.transform.localRotation = Quaternion.identity;
		friend.transform.localScale = Vector3.one;
		friend.name = "FriendProfile_" + _nickname;
		friend.SetActive(true);

		friendList.Add(friend);
	}

	public void OpenPopUp(string _popupName)
	{
		PushPopup<Popup_Basic>()
			.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm));
	}

	public void Show(bool _show, float _delay = 0f, bool _instant = false)
	{
		float target = _show ? 1f : 0f;

		if (_instant)
		{
			this.GetComponent<CanvasGroup>().alpha = _show ? 1f : 0f;
			this.GetComponent<CanvasGroup>().blocksRaycasts = _show;

			SceneLogic.instance.ClosePanel<Panel_Friend>();

			return;
		}

		FadeUtils.FadeCanvasGroup(this.GetComponent<CanvasGroup>(), target, 1f, _delay, null, 
			() => {
				if (!_show) SceneLogic.instance.ClosePanel<Panel_Friend>();
		});
	}

	int startIndex;

	public void ReorderFriendList(Friend_Profile profile, bool _isBookMarked)
	{
		if(_isBookMarked)
		{
			profile.transform.SetSiblingIndex(startIndex);
			startIndex++;
		}
		else
		{
			startIndex--;
			profile.transform.SetSiblingIndex(startIndex);
		}
	}
}
