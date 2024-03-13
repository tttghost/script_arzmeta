using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class View_PcManual : UIBase
{
	TMP_Text txtmp_esc;
	TMP_Text txtmp_q;
	TMP_Text txtmp_p;
	TMP_Text txtmp_m;
	TMP_Text txtmp_enter;
	TMP_Text txtmp_shift;
	TMP_Text txtmp_spacebar;
	TMP_Text txtmp_move_1;
	TMP_Text txtmp_move_2;
	TMP_Text txtmp_click;
	TMP_Text txtmp_zoom;

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		txtmp_esc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_esc), new MasterLocalData("arzphone_setting_shortcutkey_esc"));
		txtmp_q = GetUI_TxtmpMasterLocalizing(nameof(txtmp_q), new MasterLocalData("arzphone_setting_shortcutkey_arzphone"));
		txtmp_p = GetUI_TxtmpMasterLocalizing(nameof(txtmp_p), new MasterLocalData("arzphone_setting_shortcutkey_camera"));
		txtmp_m = GetUI_TxtmpMasterLocalizing(nameof(txtmp_m), new MasterLocalData("arzphone_setting_shortcutkey_map"));
		txtmp_enter = GetUI_TxtmpMasterLocalizing(nameof(txtmp_enter), new MasterLocalData("arzphone_setting_shortcutkey_chat"));
		txtmp_shift = GetUI_TxtmpMasterLocalizing(nameof(txtmp_shift), new MasterLocalData("arzphone_setting_shortcutkey_dash"));
		txtmp_spacebar = GetUI_TxtmpMasterLocalizing(nameof(txtmp_spacebar), new MasterLocalData("arzphone_setting_shortcutkey_jump"));
		txtmp_move_1 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_move_1), new MasterLocalData("arzphone_setting_shortcutkey_avatar"));
		txtmp_move_2 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_move_2), new MasterLocalData("arzphone_setting_shortcutkey_avatar"));

		txtmp_click = GetUI_TxtmpMasterLocalizing(nameof(txtmp_click), new MasterLocalData("arzphone_setting_shortcutkey_rightmouse"));
		txtmp_zoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_zoom), new MasterLocalData("arzphone_setting_shortcutkey_mousewheel"));
	}
}
