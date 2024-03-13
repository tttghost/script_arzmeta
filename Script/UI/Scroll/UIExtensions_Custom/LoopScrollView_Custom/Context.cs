using System;

namespace UnityEngine.UI.Extensions.LoopScrollView_Custom
{
    class Context
    {
        public int SelectedIndex = -1;
        public Action<int> OnCellClicked;
    }
}
