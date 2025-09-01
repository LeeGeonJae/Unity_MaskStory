using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] ObjectManager.EObjectDistanceType backgroundDistanceType;
    [SerializeField] float randomXPosition = 0.1f;
    [SerializeField] Vector2 correctionPosition = new Vector2(0, -1);

    Rigidbody2D rigidbody2;
    float objectSpeed = 1;

    void OnEnable()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ObjectManager objectManager = GameManager.instance.objectManager;
        objectSpeed = objectManager.GetObjectTypeSpeed(backgroundDistanceType);

        GameManager.instance.updateGameState.AddListener(UpdateGameState);
        if (GameManager.instance.currentGameState == EGameState.MoveNextStep)
        {
            rigidbody2.linearVelocity = new Vector3(-objectSpeed, 0, 0);
        }
        else
        {
            rigidbody2.linearVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (transform.position.x < -15)
        {
            gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
    }

    void UpdateGameState(EGameState gameState)
    {
        if (gameState == EGameState.MoveNextStep)
        {
            rigidbody2.linearVelocity = new Vector3(-objectSpeed, 0, 0);
        }
        else
        {
            rigidbody2.linearVelocity = Vector3.zero;
        }
    }

    public void Init()
    {
        transform.position += new Vector3(Random.Range(-randomXPosition, randomXPosition) + correctionPosition.x, correctionPosition.y, 0);

        ObjectManager objectManager = GameManager.instance.objectManager;
        objectSpeed = objectManager.GetObjectTypeSpeed(backgroundDistanceType);
        rigidbody2.linearVelocity = new Vector3(-objectSpeed, 0, 0);
    }
}
