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
    StoryEvent_SelectChoice,
    StoryEvent_CompleteEvent,
    GameOver,
    GameEnding,
    Credit
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
    public UnityEvent<GameButtonType> onGameButton;
    public UnityEvent<ActionType, PlayerStat> OnConditionClick;
    public UnityEvent<bool> OnPlayerAttack;

    // 게임 상태
    [Header("GameState")]
    public GameStateType currentGameState = GameStateType.Title;

    // UI 관리
    [Header("UI")]
    public List<GameObject> titleUI;
    public List<GameObject> inGame_UI;
    public List<GameObject> inGame_SelectButtonUI;
    public List<GameObject> inGame_CompeleteButtonUI;

    [Header("Player")]
    public Player player;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }

        instance = this;
    }

    private void Start()
    {
        SetGameUIActive();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetGameState(currentGameState == GameStateType.MoveNextStep ? GameStateType.None : GameStateType.MoveNextStep);
        }
    }

    // 게임 상태 변경
    public void SetGameState(GameStateType gameState)
    {
        currentGameState = gameState;
        SetGameUIActive();
        updateGameState?.Invoke(gameState);
    }

    // 게임 내 UI 제어
    private void SetGameUIActive()
    {
        switch (currentGameState)
        {
            case GameStateType.Title:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                    {
                        titleUI[i].SetActive(true);
                    }
                    for (int i = 0; i < inGame_UI.Count; i++)
                    {
                        inGame_UI[i].SetActive(false);
                    }
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                    {
                        inGame_SelectButtonUI[i].SetActive(false);
                    }
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                    {
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    }
                }
                break;
            case GameStateType.MoveNextStep:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                    {
                        titleUI[i].SetActive(false);
                    }
                    for (int i = 0; i < inGame_UI.Count; i++)
                    {
                        inGame_UI[i].SetActive(true);
                    }
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                    {
                        inGame_SelectButtonUI[i].SetActive(false);
                    }
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                    {
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    }
                }
                break;
            case GameStateType.StoryEvent_CompleteText:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                    {
                        titleUI[i].SetActive(false);
                    }
                    for (int i = 0; i < inGame_UI.Count; i++)
                    {
                        inGame_UI[i].SetActive(true);
                    }
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                    {
                        inGame_SelectButtonUI[i].SetActive(true);
                    }
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                    {
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    }
                }
                break;
            case GameStateType.StoryEvent_CompleteEvent:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                    {
                        titleUI[i].SetActive(false);
                    }
                    for (int i = 0; i < inGame_UI.Count; i++)
                    {
                        inGame_UI[i].SetActive(true);
                    }
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                    {
                        inGame_SelectButtonUI[i].SetActive(false);
                    }
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                    {
                        inGame_CompeleteButtonUI[i].SetActive(true);
                    }
                }
                break;
        }
    }
}
