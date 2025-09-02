using UnityEngine;
using System.Collections.Generic;
using System;

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
    [Header ("Event Setting")]
    public string eventName;
    public List<Condition> condition;
    public List<Reword> reword;
    public List<Penalty> penalty;

    [Tooltip("성공 조건 값(0이면 성공) : 이벤트가 만약 몬스터라면 HP를 말합니다.")]
    public int succedConditionValue = 1;

    [Header ("Object Setting")]
    public float objectSpeed = -3;
    public float objectDeletePositionX = -15;
    public float objectConnectedPlayerPositionX = 4;

    Rigidbody2D rigidbody2;
    GameManager gameManager;
    StoryManager storyManager;
    TextManager textManager;

    StoryEventType storyEventType;
    bool IsActiveEvent = false;

    void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();

        gameManager = GameManager.instance;
        gameManager.updateGameState.AddListener(UpdateGameState);

        storyManager = gameManager.storyManager;
        storyManager.OnConditionClick.AddListener(SelectChoice);

        textManager = gameManager.textManager;
    }

    void FixedUpdate()
    {
        if (transform.position.x < objectDeletePositionX)
        {
            Destroy(gameObject);
        }
        else if (objectConnectedPlayerPositionX > transform.position.x && !IsActiveEvent)
        {
            IsActiveEvent = true;
            gameManager.SetGameState(GameStateType.StoryEvent_TextWrite);
        }
    }

    private void OnDestroy()
    {
        storyManager.SpawnStoryEvent();

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
            textManager.ShowText(eventName);
            rigidbody2.linearVelocity = Vector3.zero;
        }
    }

    // 확률 계산 (UI용)
    public double CheckSuccedProbability(ActionType actionType, PlayerStat playerStat)
    {
        int actionNum = (int)actionType;
        double probability = 0;
        switch (condition[actionNum].StatType)
        {
            case PlayerStatType.Power:
                probability = (double)condition[actionNum].RequiredValue / (double)playerStat.Power;
                break;
            case PlayerStatType.Agility:
                probability = (double)condition[actionNum].RequiredValue / (double)playerStat.Agility;
                break;
            case PlayerStatType.Intelligence:
                probability = (double)condition[actionNum].RequiredValue / (double)playerStat.Intelligence;
                break;
            case PlayerStatType.Hp:
                probability = (double)condition[actionNum].RequiredValue / (double)playerStat.CurrentHp;
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

        if (eventSucced)
        {
            if (storyEventType == StoryEventType.Monster && actionType == ActionType.Pass)
            {
                EventSucced(succedConditionValue, actionNum);
            }
            else if (storyEventType == StoryEventType.Monster)
            {
                EventSucced((int)playerStat.Damage, actionNum);
            }
            else
            {
                EventSucced(1, actionNum);
            }
        }
        else
        {
            storyManager.OnPenalty?.Invoke(penalty[actionNum]);
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
        succedConditionValue -= value;
        if (succedConditionValue <= 0)
        {
            storyManager.OnReword?.Invoke(reword[actionNum]);
            EventExit();
        }
    }

    private void EventExit()
    {

    }
}