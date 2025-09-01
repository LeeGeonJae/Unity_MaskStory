using UnityEngine;
using UnityEngine.Events;

public enum EGameState
{
    None = 0,
    MoveNextStep,
    TextWrite,
    ChoiceSelect
}

public class GameManager : MonoBehaviour
{
    // 매니저
    [Header("Manager")]
    public static GameManager instance;
    public ObjectManager objectManager;
    public StoryManager storyManager;
    public GameObject textManager;

    // 이벤트
    [Header("Event")]
    public UnityEvent<EGameState> updateGameState;
    public UnityEvent<EPlayerStateType> updatePlayerState;

    // 게임 상태
    [Header("GameState")]
    public EGameState currentGameState = EGameState.MoveNextStep;

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
            SetGameState(currentGameState == EGameState.MoveNextStep ? EGameState.None : EGameState.MoveNextStep);

            if (currentGameState == EGameState.MoveNextStep)
            {
                updatePlayerState?.Invoke(EPlayerStateType.Run);
            }
            else if (currentGameState == EGameState.None)
            {
                updatePlayerState?.Invoke(EPlayerStateType.Idle);
            }
        }
    }

    public void SetGameState(EGameState gameState)
    {
        currentGameState = gameState;
        updateGameState?.Invoke(gameState);
    }
}
