using Cysharp.Threading.Tasks;
using FrameWork.UI;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Popup_FileBrowser : PopupBase
{
    public async UniTask<string> OpenFileBrowser_ShowLoadDialog(FILEBROWSER_SETFILTER fILEBROWSER_FILTER = FILEBROWSER_SETFILTER.IMAGE)
    {
        return await Util.Co_ShowLoadDialog((paths) => PopPopup(), () => PopPopup(), fILEBROWSER_FILTER);
    }
    public async UniTask<string> OpenFileBrowser_ShowSaveDialog(FILEBROWSER_SETFILTER fILEBROWSER_FILTER = FILEBROWSER_SETFILTER.IMAGE)
	{
        return await Util.Co_ShowSaveDialog((paths) => PopPopup(), () => PopPopup(), fILEBROWSER_FILTER);
    }
    public override void Back(int cnt = 1)
    {
        base.Back(cnt);
        FileBrowser.HideDialog();
    }

}
