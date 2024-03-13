using FrameWork.UI;

public class Panel_Game : PanelBase
{
	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		switch(Single.RealTime.roomType.current)
		{
			case RoomType.JumpingMatching:
				ChangeView(nameof(View_GameRoom_JumpingMatching));
				DEBUG.LOG("Open Jumping Matching");
				break;

			case RoomType.OXQuiz:
				ChangeView(nameof(View_GameRoom_OXQuiz));
				DEBUG.LOG("Open OX Quiz");
				break;
		}
	}

	protected override void Start()
	{
		base.Start();
	}
}
