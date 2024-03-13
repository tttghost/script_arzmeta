using FrameWork.UI;

public class View_OxQuizGuide : UIBase
{
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        GetUI_Button("btn_back", () => { gameObject.SetActive(false); });
    }

    protected override void Start()
    {

    }

    private void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
