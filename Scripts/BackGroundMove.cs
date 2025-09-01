using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    [Header("Setting")]
    [Tooltip("텍스쳐가 얼마나 빨라야 하는가.")]
    public float scrollSpeed;

    MeshRenderer MeshRenderer;

    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        MeshRenderer.material.mainTextureOffset += new Vector2(scrollSpeed * Time.deltaTime, 0);
    }
}
