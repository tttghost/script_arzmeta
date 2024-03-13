using FrameWork.UI;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 멀티게임(점핑매칭, OX 퀴즈) 가이드 공통으로 사용
/// </summary>
public class View_MultigameGuide : UIBase
{
    private TMP_Text txtmp_content;
    private Image image_thumbnail;
    private Panel_MultiGame Panel_MultiGame;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        GetUI_TxtmpMasterLocalizing("txtmp_title", new MasterLocalData("game_guide_eng")); //타이틀
        txtmp_content = GetUI_TxtmpMasterLocalizing(nameof(txtmp_content)); //content
        GetUI_TxtmpMasterLocalizing("txtmp_back", new MasterLocalData("common_close")); 
    
        GetUI_Button("btn_back", () => { gameObject.SetActive(false); });
        
    }

    protected override void Awake()
    {
        base.Awake();

        Panel_MultiGame = GetPanel<Panel_MultiGame>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        switch(Panel_MultiGame.roomType)
        {
			case RoomType.JumpingMatching:
                Util.SetMasterLocalizing(txtmp_content, new MasterLocalData("game_jumpingmatching_desc"));
				GetUI_Img("img_thumbnail", "img_guide_matching");
				break;

			case RoomType.OXQuiz:
                Util.SetMasterLocalizing(txtmp_content, new MasterLocalData("game_oxquiz_desc"));
				GetUI_Img("img_thumbnail", "img_guide_oxQuiz");
				break;
		}
	}

	protected override void OnDisable()
    {
        base.OnDisable();

        gameObject.SetActive(false);
    }
}
