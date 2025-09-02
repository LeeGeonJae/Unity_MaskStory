using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Events;

public enum StoryEventType
{
    SubStory = 0,   // 서브 스토리
    Monster,        // 몬스터
    Basic,          // 기본
    MainStory       // 메인 스토리
}

public enum ActionType
{
    Act = 0,   // 행동하기
    Observe,   // 살펴보기
    Pass       // 지나가기
}

public class StoryManager : MonoBehaviour
{
    [Header("Event")]
    public UnityEvent<Reword> OnReword;
    public UnityEvent<Penalty> OnPenalty;

    [Header("Story")]
    [SerializeField] List<int> mainStoryChepter = new List<int>();
    [SerializeField] List<GameObject> mainStoryEvent = new List<GameObject>();
    [SerializeField] List<GameObject> subStoreEvent = new List<GameObject>();
    [SerializeField] List<GameObject> MonsterStoryEvent = new List<GameObject>();
    [SerializeField] List<GameObject> BasicStoryEvent = new List<GameObject>();

    [Header("Chepter")]
    [SerializeField] int currentTurn = 0;
    [SerializeField] int currentsubStoryEvent = 0;

    [SerializeField] GameObject storyEventObject;

    // 이벤트 생성
    public void SpawnStoryEvent()
    {
        currentTurn++;

        // 메인 스토리 체크 생성
        if (!CheckMainStoryChepter())
        {
            // 일반 스토리 이벤트 생성
            RandomStoryEvnet();
        }
    }

    // 메인 스토리 챕터인지 확인
    private bool CheckMainStoryChepter()
    {
        for (int i = 0; i < mainStoryChepter.Count; i++)
        {
            if (mainStoryChepter[i] == currentTurn)
            {
                storyEventObject = Instantiate(mainStoryEvent[i]);
                storyEventObject.GetComponent<StoryEvent>().SetEventType(StoryEventType.MainStory);
                return true;
            }
        }

        return false;
    }

    // 랜덤 이벤트 생성
    private void RandomStoryEvnet()
    {
        StoryEventType randomValue = (StoryEventType)Random.Range(0, (int)StoryEventType.Basic + 1);
        Debug.Log(randomValue + "이벤트 생성");
        switch (randomValue)
        {
            // 서브 퀘스트 (챕터별 생성)
            case StoryEventType.SubStory:
                {
                    if (currentsubStoryEvent >= subStoreEvent.Count)
                    {
                        RandomStoryEvnet();
                        return;
                    }

                    storyEventObject = Instantiate(subStoreEvent[currentsubStoryEvent]);
                    storyEventObject.GetComponent<StoryEvent>().SetEventType(StoryEventType.SubStory);
                    StoryEventSetting();
                    currentsubStoryEvent++;
                }
                break;
                // 몬스터 (랜덤 생성)
            case StoryEventType.Monster:
                {
                    int randomSpawnMonster = Random.Range(0, MonsterStoryEvent.Count);
                    storyEventObject = Instantiate(MonsterStoryEvent[randomSpawnMonster]);
                    storyEventObject.GetComponent<StoryEvent>().SetEventType(StoryEventType.Monster);
                    StoryEventSetting();
                }
                break;
                // 기본 이벤트 (랜덤 생성)
            case StoryEventType.Basic:
                {
                    int randomSpawnBasic = Random.Range(0, BasicStoryEvent.Count);
                    storyEventObject = Instantiate(BasicStoryEvent[randomSpawnBasic]);
                    storyEventObject.GetComponent<StoryEvent>().SetEventType(StoryEventType.Basic);
                    StoryEventSetting();
                }
                break;
        }
    }

    private void StoryEventSetting()
    {
        storyEventObject.transform.SetParent(transform);
        storyEventObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    public void SubStoryFailed()
    {
        currentsubStoryEvent = subStoreEvent.Count + 1;
    }
}
