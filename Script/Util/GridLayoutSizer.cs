//using UnityEngine;
//using UnityEngine.UI;

//public class GridLayoutSizer : MonoBehaviour
//{
//    // GridLayoutGroup 컴포넌트
//    private GridLayoutGroup gridLayoutGroup;

//    // 부모 RectTransform 컴포넌트
//    public RectTransform parentRectTransform;

//    // 그리드 가로 자식 오브젝트 개수
//    public int gridColumns;

//    // 스페이싱 값
//    public float spacing;

//    // 패딩 값
//    public float padding;

//    // Start 함수에서 GridLayoutGroup 컴포넌트를 찾아냅니다.
//    void Start()
//    {
//        gridLayoutGroup = GetComponent<GridLayoutGroup>();
//    }

//    // Update 함수에서 그리드 사이즈를 조정합니다.
//    void Update()
//    {
//        // 아이템 개수를 가져옵니다.
//        int itemCount = transform.childCount;

//        // 그리드 가로 자식 오브젝트 개수를 기반으로 셀의 개수를 계산합니다.
//        int columnCount = Mathf.Max(1, gridColumns);
//        int rowCount = Mathf.CeilToInt((float)itemCount / columnCount);

//        // 부모 rectTransform의 사이즈를 기반으로 셀 사이즈를 계산합니다.
//        float cellSize = Mathf.Floor((parentRectTransform.rect.width - padding * 2f - spacing * (columnCount - 1)) / columnCount);

//        // 스페이싱 값을 적용하여 간격을 조정합니다.
//        float horizontalSpacing = spacing * (columnCount - 1);
//        float verticalSpacing = spacing * (rowCount - 1);
//        float totalPaddingX = (parentRectTransform.rect.width - cellSize * columnCount - horizontalSpacing) * 0.5f;
//        float totalPaddingY = (parentRectTransform.rect.height - cellSize * rowCount - verticalSpacing) * 0.5f;

//        // 셀 사이즈와 스페이싱을 변경합니다.
//        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
//        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
//        gridLayoutGroup.padding = new RectOffset((int)totalPaddingX, (int)totalPaddingX, (int)totalPaddingY, (int)totalPaddingY);
//    }
//}

//using UnityEngine;
//using UnityEngine.UI;

//public class GridLayoutSizer : MonoBehaviour
//{
//    // GridLayoutGroup 컴포넌트
//    private GridLayoutGroup gridLayoutGroup;

//    // 부모 RectTransform 컴포넌트
//    public RectTransform parentRectTransform;

//    // 그리드 가로 자식 오브젝트 개수
//    public int gridColumns;

//    // 스페이싱 값
//    public float spacing;

//    // 패딩 값
//    public float padding;

//    // 그리드 아이템이 1개일 때 적용할 비율 값
//    public float singleItemRatio = 0.5f;

//    // Start 함수에서 GridLayoutGroup 컴포넌트를 찾아냅니다.
//    void Start()
//    {
//        gridLayoutGroup = GetComponent<GridLayoutGroup>();
//    }

//    // Update 함수에서 그리드 사이즈를 조정합니다.
//    void Update()
//    {
//        // 아이템 개수를 가져옵니다.
//        int itemCount = transform.childCount;

//        // 그리드 가로 자식 오브젝트 개수를 기반으로 셀의 개수를 계산합니다.
//        int columnCount = Mathf.Max(1, gridColumns);
//        int rowCount = Mathf.CeilToInt((float)itemCount / columnCount);
//        float cellSize;
//        // 그리드 아이템이 1개일 때 처리합니다.
//        if (itemCount == 1)
//        {
//            float shortLength = Mathf.Min(parentRectTransform.rect.width, parentRectTransform.rect.height);
//            cellSize = shortLength * singleItemRatio;
//            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
//            gridLayoutGroup.spacing = Vector2.zero;
//            gridLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
//            return;
//        }

//        // 부모 rectTransform의 사이즈를 기반으로 셀 사이즈를 계산합니다.
//        cellSize = Mathf.Floor((parentRectTransform.rect.width - padding * 2f - spacing * (columnCount - 1)) / columnCount);

//        // 스페이싱 값을 적용하여 간격을 조정합니다.
//        float horizontalSpacing = spacing * (columnCount - 1);
//        float verticalSpacing = spacing * (rowCount - 1);
//        float totalPaddingX = (parentRectTransform.rect.width - cellSize * columnCount - horizontalSpacing) * 0.5f;
//        float totalPaddingY = (parentRectTransform.rect.height - cellSize * rowCount - verticalSpacing) * 0.5f;

//        // 셀 사이즈와 스페이싱을 변경합니다.
//        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
//        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
//        gridLayoutGroup.padding = new RectOffset((int)totalPaddingX, (int)totalPaddingX, (int)totalPaddingY, (int)totalPaddingY);
//    }
//}
//using UnityEngine;
//using UnityEngine.UI;

//public class GridLayoutSizer : MonoBehaviour
//{
//    // GridLayoutGroup 컴포넌트
//    private GridLayoutGroup gridLayoutGroup;

//    // 부모 RectTransform 컴포넌트
//    public RectTransform parentRectTransform;

//    // 그리드 가로 자식 오브젝트 개수
//    public int gridColumns;

//    // 스페이싱 비율 값
//    public float spacingRatio = 0.1f;

//    // 패딩 비율 값
//    public float paddingRatio = 0.1f;

//    // 그리드 아이템이 1개일 때 적용할 비율 값
//    public float singleItemRatio = 0.5f;

//    // Start 함수에서 GridLayoutGroup 컴포넌트를 찾아냅니다.
//    void Start()
//    {
//        gridLayoutGroup = GetComponent<GridLayoutGroup>();
//    }

//    // Update 함수에서 그리드 사이즈를 조정합니다.
//    void Update()
//    {
//        // 아이템 개수를 가져옵니다.
//        int itemCount = transform.childCount;

//        // 그리드 가로 자식 오브젝트 개수를 기반으로 셀의 개수를 계산합니다.
//        int columnCount = Mathf.Max(1, gridColumns);
//        int rowCount = Mathf.CeilToInt((float)itemCount / columnCount);
//        float cellSize;
//        // 그리드 아이템이 1개일 때 처리합니다.
//        if (itemCount == 1)
//        {
//            float shortLength = Mathf.Min(parentRectTransform.rect.width, parentRectTransform.rect.height);
//             cellSize = shortLength * singleItemRatio;
//            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
//            gridLayoutGroup.spacing = Vector2.zero;
//            gridLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
//            return;
//        }

//        // 부모 rectTransform의 사이즈를 기반으로 셀 사이즈를 계산합니다.
//         cellSize = Mathf.Floor((parentRectTransform.rect.width - parentRectTransform.rect.width * paddingRatio * 2f - (columnCount - 1) * parentRectTransform.rect.width * spacingRatio) / columnCount);

//        // 스페이싱 값을 적용하여 간격을 조정합니다.
//        float totalPaddingX = parentRectTransform.rect.width * paddingRatio;
//        float totalPaddingY = parentRectTransform.rect.height * paddingRatio;

//        // 셀 사이즈와 스페이싱을 변경합니다.
//        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
//        gridLayoutGroup.spacing = new Vector2(parentRectTransform.rect.width * spacingRatio, parentRectTransform.rect.width * spacingRatio);
//        gridLayoutGroup.padding = new RectOffset((int)totalPaddingX, (int)totalPaddingX, (int)totalPaddingY, (int)totalPaddingY);
//    }
//}
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 개수에 맞춰 그리드사이즈 조절하는 클래스
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(ContentSizeFitter))]
public class GridLayoutSizer : MonoBehaviour
{
    // GridLayoutGroup 컴포넌트
    private GridLayoutGroup gridLayoutGroup;

    // 부모 RectTransform 컴포넌트
    public RectTransform parentRectTransform;

    // 그리드 가로 자식 오브젝트 개수
    public int gridColumns = 5;

    // 스페이싱과 패딩 비율 값
    private float spacingAndPaddingRatio;

    // 그리드 아이템이 1개일 때 적용할 비율 값
    public float singleItemRatio = 0.75f;

    // Start 함수에서 GridLayoutGroup 컴포넌트를 찾아냅니다.
    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
    }

    // Update 함수에서 그리드 사이즈를 조정합니다.
    private void Update()
    {
        // 아이템 개수를 가져옵니다.
        int itemCount = transform.childCount;

        // 그리드 가로 자식 오브젝트 개수를 기반으로 셀의 개수를 계산합니다.
        int columnCount = Mathf.Max(1, gridColumns);
        int rowCount = Mathf.CeilToInt((float)itemCount / columnCount);

        // 그리드 아이템이 1개일 때 처리합니다.
        if (itemCount == 1)
        {
            float shortLength = Mathf.Min(parentRectTransform.rect.width, parentRectTransform.rect.height);
            float cellSize = shortLength * singleItemRatio;
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
            gridLayoutGroup.spacing = Vector2.zero;
            gridLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
            return;
        }

        spacingAndPaddingRatio = itemCount > 5 ? 0.2f : 0.1f;

        // 부모 rectTransform의 사이즈를 기반으로 셀 너비를 계산합니다.
        float gridWidth = parentRectTransform.rect.width;
        float gridHeight = parentRectTransform.rect.height;

        float spacingAndPaddingValue = spacingAndPaddingRatio * (gridWidth + gridHeight) / (columnCount * 2f + 1f);
        float cellWidth = (gridWidth - spacingAndPaddingValue * (columnCount - 1) - spacingAndPaddingValue * 2f) / columnCount;

        // 셀 높이는 셀 너비를 기반으로 계산합니다.
        float cellHeight = cellWidth;

        // 그리드의 가로 길이를 구합니다.
        float gridHorizontalLength = cellWidth * columnCount + spacingAndPaddingValue * (columnCount - 1) + spacingAndPaddingValue * 2f;

        // 그리드 아이템이 가로로 모두 들어갈 수 있는 경우 스페이싱과 패딩을 구합니다.
        if (gridHorizontalLength <= gridWidth)
        {
            float paddingValue = (gridWidth - gridHorizontalLength) / 2f;
            float spacingValue = spacingAndPaddingValue;

            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayoutGroup.spacing = new Vector2(spacingValue, spacingValue);
            gridLayoutGroup.padding = new RectOffset((int)paddingValue, (int)paddingValue, (int)spacingValue, (int)spacingValue);
        }
        // 그리드 아이템이 가로로 다 들어가지 않는 경우 그리드 사이즈를 조정합니다.
        else
        {
            // 그리드의 세로 길이를 계산합니다.
            float gridVerticalLength = cellHeight * rowCount + spacingAndPaddingValue * (rowCount - 1) + spacingAndPaddingValue * 2f;

            // 그리드의 세로 길이가 부모 rectTransform의 높이보다 큰 경우 스페이싱과 패딩을 0으로 설정합니다.
            if (gridVerticalLength > gridHeight)
            {
                float cellSize = (gridWidth - spacingAndPaddingValue * (columnCount - 1) - spacingAndPaddingValue * 2f) / columnCount;

                gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
                gridLayoutGroup.spacing = Vector2.zero;
                gridLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
            }
            // 그리드의 세로 길이가 부모 rectTransform의 높이보다 작거나 같은 경우 스페이싱과 패딩을 구합니다.
            else
            {
                float paddingValue = (gridWidth - gridHorizontalLength) / 2f;
                float spacingValue = spacingAndPaddingValue;

                gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
                gridLayoutGroup.spacing = new Vector2(spacingValue, spacingValue);
                gridLayoutGroup.padding = new RectOffset((int)paddingValue, (int)paddingValue, (int)spacingValue, (int)spacingValue);
            }
        }
    }
}

