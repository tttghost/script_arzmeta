using FrameWork.UI;
using FrameWork.Utils;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCustomManager : MonoBehaviour
{
	#region Singleton

	public static InteractionCustomManager Instance
	{
		get
		{
			if (instance != null) return instance;
			instance = FindObjectOfType<InteractionCustomManager>();
			return instance;
		}
	}
	private static InteractionCustomManager instance;

	#endregion



	[HideInInspector] public InteractionCustom interaction;
	[HideInInspector] public Transform target;

	private CanvasGroup playmode;

	public delegate void BoolParam(bool b);
	public BoolParam handlerInteraction; //인터렉션요소 활성화/비활성화

	public delegate void MyRoomModeParam(eMyRoomMode myRoomMode);
	public MyRoomModeParam handlerMyRoomModeChange; //인터렉션요소 활성화/비활성화

	private void OnDestroy()
	{
		handlerInteraction -= ShowHUDElements;
	}

	private void Start()
	{
		playmode = SceneLogic.instance.GetPanel<Panel_MyRoomPlaymode>().btn_EnterEditmode.GetComponent<CanvasGroup>();

		handlerInteraction += ShowHUDElements;
	}

	private void Update()
	{
		if (!MyPlayer.instance || interaction == null) return;

		if (MyPlayer.instance.TPSController.StarterInputs.move != Vector2.zero
			|| MyPlayer.instance.TPSController.StarterInputs.jump
			|| MyPlayer.instance.TPSController.StarterInputs.sprint)
		{
			EndInteraction();
		}
	}


	#region Core Methods

	public void StartInteraction()
	{
		handlerInteraction?.Invoke(false);

		Timing.RunCoroutine(Co_StartInteraction());

		ArzMetaManager.Instance.PhoneController.StartInteraction();
	}

	private IEnumerator<float> Co_StartInteraction()
	{
		string _parameter = string.Empty;

		if (interaction.parameter == "interaction_sleep")
		{
			_parameter = "action_laydown";
		}

		else if (interaction.parameter == "interaction_sit")
		{
			_parameter = "action_sit";
		}

		MyPlayer.instance.Teleport(
			target.position + (target.right * interaction.localPosition_Teleport.x) +
			(target.up * interaction.localPosition_Teleport.y) +
			(target.forward * interaction.localPosition_Teleport.z),
			target.eulerAngles, _useEffect: false
		);

		MyPlayer.instance.TPSController.MotionController.PlayMotion(_parameter, false, 0f);

		MyPlayer.instance.TPSController.Camera.VirtualCam.m_Lens.NearClipPlane = 1f;

		if (interaction.parameter == "interaction_sit")
		{
			var emotionController = FindObjectOfType<EmotionController>(true);

			emotionController.Switch2Stand(false);
		}

		yield return Timing.WaitForOneFrame;
	}



	public void EndInteraction()
	{
		handlerInteraction?.Invoke(true);

		Timing.RunCoroutine(Co_EndInteraction());

		ArzMetaManager.Instance.PhoneController.EndInteraction();
	}

	public IEnumerator<float> Co_EndInteraction()
	{
		string _parameter = null;
		if(interaction == null)
        {
			yield break;
        }
		if (interaction.parameter == "interaction_sleep")
		{
			_parameter = "action_sleep_stand_up";
		}

		else if (interaction.parameter == "interaction_sit")
		{
			_parameter = "action_stand";

			var emotionController = FindObjectOfType<EmotionController>(true);
			emotionController.Switch2Stand(true);
		}

		MyPlayer.instance.TPSController.MotionController.BlendMotion(_parameter, false, 0f);
		MyPlayer.instance.TPSController.Camera.VirtualCam.m_Lens.NearClipPlane = 0.3f;

		interaction = null;

		yield return Timing.WaitForOneFrame;
	}

	#endregion

	private void ShowHUDElements(bool _show)
	{
		var target = _show ? 1f : 0f;

		FadeUtils.FadeCanvasGroup(playmode, target, 1f);
	}



	public void InteractionFrame()
    {

    }
}