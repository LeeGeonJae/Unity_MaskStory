using UnityEngine;

public class ContentResize : MonoBehaviour
{
    public GameObject textObject;
    
    RectTransform rectTransform;

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
    }
}
