using MEC;
using Protocol;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
using FrameWork.Network;

public enum CameraView
{
	None,
	Front,
	Back,
}

public class PlayerData
{
	public Vector3 position;
	public Vector3 eulerAngle;

	public bool isRandomSpawn = false;

	public string prefabName = Cons.Player_Realtime;

	public CameraView cameraView = CameraView.Back;

	public void RandomSpawn()
	{
		position += PositionOffset();
		eulerAngle += EulerAngleOffset();
	}

	public Vector3 PositionOffset()
	{
		return Vector3.right * Random.Range(-2f, 2f) + Vector3.forward * Random.Range(-2f, 2f);
	}

	public Vector3 EulerAngleOffset()
	{
		return Vector3.up * Random.Range(-360f, 360f);
	}
}

public class NetworkPlayer
{
	/// <summary>
	/// 객체 생성 시 바로 네트워크 플레이어 생성
	/// </summary>
	/// <param name="data">서버타입, 위치, 씬 이름, 프리팹 지정 가능</param>
	///
	public NetworkPlayer(PlayerData data)
	{
		PKT_C_BASE_INSTANTIATE_OBJECT(data);

		Timing.RunCoroutine(Co_SetPlayer(data));
	}

	private void PKT_C_BASE_INSTANTIATE_OBJECT(PlayerData _data)
	{
		C_BASE_INSTANTIATE_OBJECT packet = new C_BASE_INSTANTIATE_OBJECT();

		if (_data.isRandomSpawn) _data.RandomSpawn();

		packet.PrefabName = _data.prefabName;
		packet.Position = NetworkUtils.UnityVector3ToProtocolVector3(_data.position);
		packet.Rotation = NetworkUtils.UnityVector3ToProtocolVector3(_data.eulerAngle);
		packet.ObjectData = ItemDataManager.Instance.GetAvatarInfoJson();

		Single.RealTime.Send(packet);

		DEBUG.LOG("INSTANTIATE_OBJECT 요청", eColorManager.REALTIME);
	}

	public void SetNickname() => PKT_C_SET_NICKNAME();

	private void PKT_C_SET_NICKNAME()
	{
		C_SET_NICKNAME packet = new C_SET_NICKNAME();
		packet.Nickname = LocalPlayerData.NickName;

		Single.RealTime.Send(packet);
	}

	IEnumerator<float> Co_SetPlayer(PlayerData _data)
	{
		yield return Timing.WaitUntilTrue(() => Object.FindObjectOfType<NetworkHandler>().isInstantiated);

		yield return Timing.WaitUntilTrue(() => MyPlayer.instance != null);

		switch (_data.cameraView)
		{
			case CameraView.Front:
				MyPlayer.instance.TPSController.Camera.ResetCameraFront();
				break;
			case CameraView.Back:
				MyPlayer.instance.TPSController.Camera.ResetCameraBack();
				break;
			case CameraView.None:
				break;
		}
	}
}