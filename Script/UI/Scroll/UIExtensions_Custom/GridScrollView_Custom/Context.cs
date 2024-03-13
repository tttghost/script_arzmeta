using System;

namespace UnityEngine.UI.Extensions.GridScrollView_Custom
{
    class Context : FancyGridViewContext
    {
        public int SelectedIndex = -1;
        public int PreSelectIdx = -1; // 한효주
        public Action<int> OnCellClicked;
    }
}