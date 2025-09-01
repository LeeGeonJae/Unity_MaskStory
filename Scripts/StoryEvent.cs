using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Condition
{
    public EPlayerStatType StatType;
    public int RequiredValue;
}

[System.Serializable]
public class Reword
{
    public EPlayerStatType StatType;
    public int RewordValue;
}

[System.Serializable]
public class Penalty
{
    public EPlayerStatType StatType;
    public int PenaltydValue;
}

public class StoryEvent : MonoBehaviour
{
    public string EventName;
    public Condition condition;
    public Reword reword;
    public Penalty penalty;
    public float objectSpeed = -3;

    Rigidbody2D rigidbody2;
    StoryManager storyManager;

    void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();

        GameManager gameManager = GameManager.instance;
        GameManager.instance.updateGameState.AddListener(UpdateGameState);

        storyManager = GameManager.instance.storyManager;
    }

    void FixedUpdate()
    {
        if (transform.position.x < -15)
        {
            Destroy(gameObject);
            storyManager.SpawnStoryEvent();
        }
    }

    private void OnDestroy()
    {
        GameManager gameManager = GameManager.instance;
        GameManager.instance.updateGameState.RemoveListener(UpdateGameState);
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
}