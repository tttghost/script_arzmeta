using System;

namespace UnityEngine.UI.Extensions.ScrollView_Custom
{
    public class Context : FancyScrollRectContext
    {
        public int SelectedIndex = -1;
        public Action<int> OnCellClicked;
    }
}
