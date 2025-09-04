using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    GameEnding_TextWrite,
    GameEnding_CompleteText,
    GameEnding_CompleteEvent,
    GameExit
}

public class GameManager : MonoBehaviour
{
    // 매니저
    [Header("Manager")]
    public static GameManager instance;
    public ObjectManager objectManager;
    public StoryManager storyManager;
    public TextManager textManager;
    public EndingManager endingManager;
    public SoundManager soundManager;

    // 이벤트
    [Header("Event")]
    public UnityEvent<GameStateType> updateGameState;
    public UnityEvent<GameButtonType> onGameButton;
    public UnityEvent<ActionType, PlayerStat> OnConditionClick;
    public UnityEvent<bool> OnPlayerAttack;
    public UnityEvent<int> OnBGMPlay;

    // 게임 상태
    [Header("GameState")]
    public GameStateType currentGameState = GameStateType.Title;

    // UI 관리
    [Header("UI")]
    public List<GameObject> titleUI;
    public List<GameObject> inGame_UI;
    public List<GameObject> inGame_SelectButtonUI;
    public List<GameObject> inGame_CompeleteButtonUI;
    public List<GameObject> ending_CompeleteButtonUI;
    public List<GameObject> ending_ExitButtonUI;

    // 오디오
    [Header("Audio")]
    AudioSource audioSource_BGM;
    [SerializeField] List<AudioClip> audioClip_BGM;

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
        audioSource_BGM = GetComponent<AudioSource>();
        audioSource_BGM.outputAudioMixerGroup = soundManager.bgmGroup;
        PlayBGM(0);

        OnBGMPlay.AddListener(PlayBGM);

        SetGameUIActive();
    }

    private void Update()
    {
    }

    public void GameEndingStart(bool isDeath)
    {
        if (isDeath)
        {
            endingManager.AddEnding(storyManager.storyEventObject.GetComponent<StoryEvent>().endingType);
        }
        else
        {
            endingManager.AddEnding(EndingType.GameEnding_Normal);
        }

        // 게임 쓰기
        SetGameState(GameStateType.GameEnding_TextWrite);
    }

    public void SubStoryClear()
    {
        endingManager.AddEnding(EndingType.GameEnding_Karen);
    }

    private void PlayBGM(int num)
    {
        audioSource_BGM.resource = audioClip_BGM[num];
        audioSource_BGM.loop = true;
        audioSource_BGM.Play();
    }

    // 게임 상태 변경
    public void SetGameState(GameStateType gameState)
    {
        Debug.Log("SetGameState 함수 실행 : " + gameState);

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
                        titleUI[i].SetActive(true);
                    for (int i = 0; i < inGame_UI.Count; i++)
                        inGame_UI[i].SetActive(false);
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                        inGame_SelectButtonUI[i].SetActive(false);
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_CompeleteButtonUI.Count; i++)
                        ending_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_ExitButtonUI.Count; i++)
                        ending_ExitButtonUI[i].SetActive(false);
                }
                break;
            case GameStateType.MoveNextStep:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                        titleUI[i].SetActive(false);
                    for (int i = 0; i < inGame_UI.Count; i++)
                        inGame_UI[i].SetActive(true);
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                        inGame_SelectButtonUI[i].SetActive(false);
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_CompeleteButtonUI.Count; i++)
                        ending_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_ExitButtonUI.Count; i++)
                        ending_ExitButtonUI[i].SetActive(false);
                }
                break;
            case GameStateType.StoryEvent_CompleteText:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                        titleUI[i].SetActive(false);
                    for (int i = 0; i < inGame_UI.Count; i++)
                        inGame_UI[i].SetActive(true);
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                        inGame_SelectButtonUI[i].SetActive(true);
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_CompeleteButtonUI.Count; i++)
                        ending_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_ExitButtonUI.Count; i++)
                        ending_ExitButtonUI[i].SetActive(false);
                }
                break;
            case GameStateType.StoryEvent_CompleteEvent:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                        titleUI[i].SetActive(false);
                    for (int i = 0; i < inGame_UI.Count; i++)
                        inGame_UI[i].SetActive(true);
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                        inGame_SelectButtonUI[i].SetActive(false);
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                        inGame_CompeleteButtonUI[i].SetActive(true);
                    for (int i = 0; i < ending_CompeleteButtonUI.Count; i++)
                        ending_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_ExitButtonUI.Count; i++)
                        ending_ExitButtonUI[i].SetActive(false);
                }
                break;
            case GameStateType.GameEnding_CompleteText:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                        titleUI[i].SetActive(false);
                    for (int i = 0; i < inGame_UI.Count; i++)
                        inGame_UI[i].SetActive(true);
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                        inGame_SelectButtonUI[i].SetActive(false);
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_CompeleteButtonUI.Count; i++)
                        ending_CompeleteButtonUI[i].SetActive(true);
                    for (int i = 0; i < ending_ExitButtonUI.Count; i++)
                        ending_ExitButtonUI[i].SetActive(false);
                }
                break;
            case GameStateType.GameEnding_TextWrite:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                        titleUI[i].SetActive(false);
                    for (int i = 0; i < inGame_UI.Count; i++)
                        inGame_UI[i].SetActive(true);
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                        inGame_SelectButtonUI[i].SetActive(false);
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_CompeleteButtonUI.Count; i++)
                        ending_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_ExitButtonUI.Count; i++)
                        ending_ExitButtonUI[i].SetActive(false);
                }
                break;
            case GameStateType.GameEnding_CompleteEvent:
                {
                    for (int i = 0; i < titleUI.Count; i++)
                        titleUI[i].SetActive(false);
                    for (int i = 0; i < inGame_UI.Count; i++)
                        inGame_UI[i].SetActive(true);
                    for (int i = 0; i < inGame_SelectButtonUI.Count; i++)
                        inGame_SelectButtonUI[i].SetActive(false);
                    for (int i = 0; i < inGame_CompeleteButtonUI.Count; i++)
                        inGame_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_CompeleteButtonUI.Count; i++)
                        ending_CompeleteButtonUI[i].SetActive(false);
                    for (int i = 0; i < ending_ExitButtonUI.Count; i++)
                        ending_ExitButtonUI[i].SetActive(true);
                }
                break;
            case GameStateType.GameExit:
                {
                    string currentSceneName = SceneManager.GetActiveScene().name;
                    SceneManager.LoadScene(currentSceneName);
                }
                break;
        }
    }
}
