using System.Collections.Generic;
using UnityEngine;

public class ObjectSpriteChange : MonoBehaviour
{
    [SerializeField] List<Sprite> spriteList;
    SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        // 랜덤으로 이미지 변환
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteList.Count > 0)
        {
            int index = Random.Range(0, spriteList.Count);
            spriteRenderer.sprite = spriteList[index];
        }
    }
}
