using UnityEngine;

public class ContentResize : MonoBehaviour
{
    public GameObject textObject;
    public GameObject endingObject;
    
    RectTransform rectTransform;
    float previewSize;
    float currentSize;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        if (textObject)
        {
            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
            if (textRectTransform)
            {
                Vector2 textSize = textRectTransform.sizeDelta;
                textSize.y += 10;
                rectTransform.sizeDelta = textSize;
            }
        }

        if (endingObject)
        {
            RectTransform endingRectTransform = endingObject.GetComponent<RectTransform>();
            if (endingRectTransform)
            {
                Vector2 textSize = endingRectTransform.sizeDelta;
                textSize.y += 10;
                rectTransform.sizeDelta = new Vector2(0, textSize.y + rectTransform.sizeDelta.y);
                currentSize = textSize.y + rectTransform.sizeDelta.y;
            }
        }


        if (currentSize > 760 && currentSize > previewSize)
        {
            UpdatePosition(currentSize - 760);
        }

        previewSize = currentSize;
    }

    private void UpdatePosition(float position)
    {
        Vector2 anchoredPos = rectTransform.anchoredPosition;
        anchoredPos.y = position;
        rectTransform.anchoredPosition = anchoredPos;
    }
}
