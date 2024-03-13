using UnityEngine.UI;
using UnityEngine.EventSystems;
using PullToRefresh;
/// <summary>
/// 설명 : 당겼다 놓으면 갱신되는 스크롤뷰
/// 사용 아이템 스크립트 : 없음
/// 사용 방법 : 스크롤뷰 오브젝트에 해당 스크립트와 UIRefreshControl 스크립트 추가
/// 원본 예제(프로젝트) : Assets\_DEV\Script\UI\PullToRefresh\Examples\Scenes\ExampleScene
/// 원본 예제(웹사이트) : https://github.com/kiepng/Unity-PullToRefresh
/// </summary>
public class ScrollRect_Custom : ScrollRect, IScrollable
{
    private bool _Dragging;
    public bool Dragging
    {
        get { return _Dragging; }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        _Dragging = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        _Dragging = false;
    }
}
