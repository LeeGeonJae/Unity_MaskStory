using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.Audio;

[System.Serializable]
public class Condition
{
    public PlayerStatType StatType;
    public int RequiredValue;
}

[System.Serializable]
public class Reword
{
    public PlayerStatType StatType;
    public int RewordValue;
}

[System.Serializable]
public class Penalty
{
    public PlayerStatType StatType;
    public int PenaltydValue;
}

public class StoryEvent : MonoBehaviour
{
    [Header ("Event Succed Condition Value")]
    [Tooltip("성공 조건 값(0이면 성공) : 이벤트가 만약 몬스터라면 HP를 말합니다.")]
    public int succedConditionValue = 1;

    [Header ("Event Setting")]
    [SerializeField]StoryEventType storyEventType;
    public string eventName;

    public List<Condition> condition;
    public List<Reword> reword;
    public List<Penalty> penalty;
    public List<string> ChoicesText;

    [Header ("Object Setting")]
    public float objectSpeed = 3;
    public float objectDeletePositionX = -15;
    public float objectConnectedPlayerPositionX = 4;

    Rigidbody2D rigidbody2;
    GameManager gameManager;
    StoryManager storyManager;
    TextManager textManager;

    [Header("Audio")]
    public int audioNumber = -1;
    AudioSource audioSource;
    public AudioClip succedAudioClip;
    public AudioClip failedAudioClip;

    [Header("Ending")]
    public bool isNormalEnding = false;
    public EndingType endingType;

    bool IsActiveEvent = false;

    void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        gameManager = GameManager.instance;
        gameManager.updateGameState.AddListener(UpdateGameState);
        gameManager.OnConditionClick.AddListener(SelectChoice);

        storyManager = gameManager.storyManager;
        textManager = gameManager.textManager;
        audioSource.outputAudioMixerGroup = gameManager.soundManager.sfxGroup;

        // 버튼 텍스트 수정
        for (int i = 0; i < gameManager.inGame_SelectButtonUI.Count; i++)
        {
            double probability = (int)(CheckSuccedProbability((ActionType)i, gameManager.player.playerStat) * 100);
            gameManager.inGame_SelectButtonUI[i].GetComponentInChildren<TextMeshProUGUI>().text = ChoicesText[i] + "\n(" + probability + "%)";
        }
    }

    void FixedUpdate()
    {
        if (gameManager.currentGameState == GameStateType.MoveNextStep)
        {
            rigidbody2.linearVelocity = new Vector3(-objectSpeed, 0, 0);
        }

        if (transform.position.x < objectDeletePositionX)
        {
            storyManager.SpawnStoryEvent();
            Destroy(gameObject);
        }
        else if (objectConnectedPlayerPositionX > transform.position.x && !IsActiveEvent)
        {
            IsActiveEvent = true;
            gameManager.SetGameState(GameStateType.StoryEvent_TextWrite);

            // 배경음 재생
            if (audioNumber >= 0)
            {
                gameManager.OnBGMPlay?.Invoke(audioNumber);
            }
        }
    }

    private void OnDestroy()
    {
        GameManager gameManager = GameManager.instance;
        GameManager.instance.updateGameState.RemoveListener(UpdateGameState);
    }

    // 이벤트 타입
    public void SetEventType(StoryEventType eventType)
    {
        storyEventType = eventType;
    }

    // 게임 상태 체크
    void UpdateGameState(GameStateType gameState)
    {
        if (gameState == GameStateType.MoveNextStep)
        {
            rigidbody2.linearVelocity = new Vector3(-objectSpeed, 0, 0);
        }
        else if (gameState == GameStateType.StoryEvent_TextWrite)
        {
            TextWrite(eventName, GameStateType.StoryEvent_CompleteText);
            rigidbody2.linearVelocity = Vector3.zero;
        }
    }

    // 확률 계산 (UI용)
    public double CheckSuccedProbability(ActionType actionType, PlayerStat playerStat)
    {
        int actionNum = (int)actionType;
        if (condition[actionNum].RequiredValue == 0)        // 조건값이 0이면 성공확률 100퍼
        {
            return 1;
        }

        double probability = 0;
        switch (condition[actionNum].StatType)
        {
            case PlayerStatType.Power:
                probability = (double)playerStat.Power / (double)condition[actionNum].RequiredValue;
                break;
            case PlayerStatType.Agility:
                probability = (double)playerStat.Agility / (double)condition[actionNum].RequiredValue;
                break;
            case PlayerStatType.Intelligence:
                probability = (double)playerStat.Intelligence / (double)condition[actionNum].RequiredValue;
                break;
            case PlayerStatType.Hp:
                probability = (double)playerStat.CurrentHp / (double)condition[actionNum].RequiredValue;
                break;
        }

        if (probability >= 1)
        {
            return 1;
        }
        return probability;
    }

    // 선택 후 성공 여부 확인 (보상, 패널티 부여)
    public void SelectChoice(ActionType actionType, PlayerStat playerStat)
    {
        int actionNum = (int)actionType;
        bool eventSucced = false;

        if ((double)condition[actionNum].RequiredValue == 0)        // 조건값이 0이면 성공확률 100퍼
        {
            eventSucced = true;
        }
        else
        {
            switch (condition[actionNum].StatType)
            {
                case PlayerStatType.Power:
                    eventSucced = CheckSuccedEvent(playerStat.Power, condition[actionNum].RequiredValue);
                    break;
                case PlayerStatType.Agility:
                    eventSucced = CheckSuccedEvent(playerStat.Agility, condition[actionNum].RequiredValue);
                    break;
                case PlayerStatType.Intelligence:
                    eventSucced = CheckSuccedEvent(playerStat.Intelligence, condition[actionNum].RequiredValue);
                    break;
                case PlayerStatType.Hp:
                    eventSucced = CheckSuccedEvent(playerStat.CurrentHp, condition[actionNum].RequiredValue);
                    break;
            }
        }

        if (eventSucced)
        {
            if (GetComponentInChildren<Monster>() && actionType == ActionType.Pass)                     // 몬스터 지나치기
            {
                EventSucced(succedConditionValue, actionNum);
            }
            else if (GetComponentInChildren<Monster>() && actionType == ActionType.Act)                 // 몬스터면 데미지
            {
                EventSucced((int)playerStat.Damage, actionNum);
                GetComponentInChildren<Monster>().Damaged();
            }
            else if (GetComponentInChildren<Monster>() && actionType == ActionType.Observe)             // 몬스터 관찰
            {
                EventSucced(0, actionNum);
                GetComponentInChildren<Monster>().Tack();
            }
            else if (storyEventType == StoryEventType.SubStory && actionType == ActionType.Pass)        // 서브 퀘스트 지나칠 시 실패
            {
                EventSucced(1, actionNum);
                storyManager.SubStoryFailed();
            }
            else
            {
                EventSucced(1, actionNum);
            }
        }
        else
        {
            storyManager.OnPenalty?.Invoke(penalty[actionNum]);
            if (GetComponentInChildren<Monster>())
            {
                TextWrite(eventName + "-" + ((int)actionType + 1) + "-2", GameStateType.StoryEvent_CompleteText);
                GetComponentInChildren<Monster>().Attack();
            }
            else
            {
                if (failedAudioClip)
                {
                    PlaySound(failedAudioClip);
                }
                TextWrite(eventName + "-" + ((int)actionType + 1) + "-2", GameStateType.StoryEvent_CompleteEvent);
            }
        }
    }

    // 이벤트가 성공했는지 판단
    private bool CheckSuccedEvent(int playerStatValue, int conditionValue)
    {
        double probability = (double)playerStatValue / (double)conditionValue;

        if (probability >= 1)
        {
            return true;
        }
        else
        {
            System.Random random = new System.Random();
            double value = random.NextDouble();

            if (value < probability)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // 성공했는지 확인하고 성공했으면 보상
    private void EventSucced(int value, int actionNum)
    {
        Debug.Log("선택 성공 하였습니다 : " + value);

        succedConditionValue -= value;
        if (succedConditionValue <= 0 && isNormalEnding)
        {
            Debug.Log("노말 게임 엔딩 로그" + value);
            GameManager.instance.GameEndingStart(false);
        }
        else if (succedConditionValue <= 0 && !isNormalEnding)
        {
            storyManager.OnReword?.Invoke(reword[actionNum]);
            TextWrite(eventName + "-" + (actionNum + 1) + "-1", GameStateType.StoryEvent_CompleteEvent);

            if (audioNumber >= 0)
            {
                gameManager.OnBGMPlay?.Invoke(0);
            }

            if (succedAudioClip)
            {
                PlaySound(succedAudioClip);
            }
        }
        else
        {
            TextWrite(eventName + "-" + (actionNum + 1) + "-1", GameStateType.StoryEvent_CompleteText);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.resource = clip;
        audioSource.loop = false;
        audioSource.Play();
    }

    private void TextWrite(string id, GameStateType gameState)
    {
        if (gameManager.currentGameState < GameStateType.GameOver)
        {
            textManager.ShowText(id, gameState);
        }
    }
}