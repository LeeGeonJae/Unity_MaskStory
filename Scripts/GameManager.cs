using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameStateType
{
    None = 0,
    Title,
    MoveNextStep,
    StoryEvent_TextWrite,
    StoryEvent_CompleteText,
    StoryEvent_SelectChoice
}

public class GameManager : MonoBehaviour
{
    // 매니저
    [Header("Manager")]
    public static GameManager instance;
    public ObjectManager objectManager;
    public StoryManager storyManager;
    public TextManager textManager;

    // 이벤트
    [Header("Event")]
    public UnityEvent<GameStateType> updateGameState;

    // 게임 상태
    [Header("GameState")]
    public GameStateType currentGameState = GameStateType.MoveNextStep;

    // UI 관리
    [Header("UI")]
    public GameObject TitleUI;
    public List<GameObject> InGameUI;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }

        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetGameState(currentGameState == GameStateType.MoveNextStep ? GameStateType.None : GameStateType.MoveNextStep);
        }
    }

    public void SetGameState(GameStateType gameState)
    {
        currentGameState = gameState;
        updateGameState?.Invoke(gameState);
    }
}
