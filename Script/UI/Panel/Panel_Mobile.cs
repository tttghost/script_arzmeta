using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Mobile : PanelBase
{
	[SerializeField] Button btn_back;
	[SerializeField] Button btn_cameraApp;
	[SerializeField] Button btn_mapApp;

	[SerializeField] Transform content;

	protected override void OnEnable()
	{
		base.OnEnable();

		Util.RunCoroutine(Initialization());

		BackAction_Custom += OnClick_Back;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		BackAction_Custom -= OnClick_Back;
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		this.GetComponent<CanvasGroup>().alpha = 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;

		content = this.transform.Search(Define.Content);
		btn_back = GetUI_Button(nameof(btn_back), OnClick_Back);
	}

	IEnumerator<float> Initialization()
	{
		yield return Timing.WaitUntilTrue(() => content.childCount > 1);

		btn_cameraApp = this.transform.Search("go_app_Camera").GetComponentInChildren<Button>();
		btn_cameraApp.onClick.RemoveAllListeners();
		btn_cameraApp.onClick.AddListener(CameraAppEvent);

		btn_mapApp = this.transform.Search("go_app_Map").GetComponentInChildren<Button>();

		SetType();

		GetPanel<Panel_HUD>().Joystick.AddItem();
	}

	public bool isSit = false;

	public void SetType()
	{
		if(isSit)
		{
			btn_mapApp.transform.parent.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.39f);
			btn_mapApp.GetComponent<Image>().color = new Color(.5f, .5f, .5f, 0.39f);
			btn_mapApp.transform.parent.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(0f, 0f, 0f, .65f);

			btn_mapApp.onClick.RemoveAllListeners();
			btn_mapApp.onClick.AddListener(
			() =>
				PushPopup<Popup_Basic>()
				.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("common_error_map"))
			));
		}
		else
		{
			btn_mapApp.transform.parent.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			btn_mapApp.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			btn_mapApp.transform.parent.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(0f, 0f, 0f, 0.85f);

			btn_mapApp.onClick.RemoveAllListeners();
			btn_mapApp.onClick.AddListener(
			() =>
			{
				Single.Sound.Click();

				ArzMetaManager.Instance.PhoneController.Phone.Minimap.OpenMap();
			}
			);
		}
	}

	protected override IEnumerator<float> Co_OpenStartAct()
	{
		yield return Timing.WaitForOneFrame;

		Show(true);

		GetPanel<Panel_HUD>().Joystick.ShowItem(false);
	}

	protected override IEnumerator<float> Co_SetCloseStartAct()
	{
		Show(false);

		yield return Timing.WaitUntilTrue(() => this.GetComponent<CanvasGroup>().alpha <= 0);
	}

	private void OnClick_Back()
	{
		GetPanel<Panel_HUD>().Show(true);

		PopPanel();

		GetPanel<Panel_HUD>().Joystick.ShowItem(true);
	}

	private void CameraAppEvent()
	{
		Show(false);

		Util.RunCoroutine(CameraAppEventSequance());
	}

	IEnumerator<float> CameraAppEventSequance()
	{
		yield return Timing.WaitUntilTrue(() => this.GetComponent<CanvasGroup>().alpha <= 0);

		GetPanel<Panel_HUD>().Joystick.left.SetActive(false);
		GetPanel<Panel_HUD>().Joystick.right.SetActive(false);

		PushPanel<Panel_Photo>(false);
		GetPanel<Panel_Photo>().Show(true);
		GetPanel<Panel_Photo>().SetBackEvent();
	}
}
