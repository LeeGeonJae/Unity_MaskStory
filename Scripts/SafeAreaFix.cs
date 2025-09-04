using UnityEngine;

public class SafeAreaFix : MonoBehaviour
{
    void Start()
    {
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Safe Area 무시하고 꽉 채우고 싶다면 이렇게 고정
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);

        // 만약 Safe Area 적용하려면 아래 코드 사용
        // rectTransform.anchorMin = anchorMin;
        // rectTransform.anchorMax = anchorMax;
    }
}
