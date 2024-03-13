using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FrameWork.UI;

public class Item_GameRoom : MonoBehaviour
{
	#region Members

	GameRoomInfoRes roomInfo;

    TMP_Text txtmp_playerNumber;
	TMP_Text txtmp_roomName;

	Button btn_GameEnter;

	public GameRoomInfoRes RoomInfo { get => roomInfo; }

	#endregion



	#region Initialize

	private void Awake()
	{
		txtmp_roomName = this.transform.Search(nameof(txtmp_roomName)).GetComponent<TMP_Text>();
		txtmp_playerNumber = this.transform.Search(nameof(txtmp_playerNumber)).GetComponent<TMP_Text>();

		btn_GameEnter = this.transform.Search(nameof(btn_GameEnter)).GetComponent<Button>();
		btn_GameEnter.onClick.AddListener(() => SceneLogic.instance.GetPanel<Panel_MultiGame>().JoinRoom<GameRoomInfoRes>(roomInfo));
	}

	#endregion



	#region Core Methods

	public void RefreshUI(GameRoomInfoRes data)
	{
		roomInfo = data;

		txtmp_roomName.text = data.roomName;
		txtmp_playerNumber.text = data.currentPlayerNumber + "/" + data.maxPlayerNumber;
	}

	#endregion
}