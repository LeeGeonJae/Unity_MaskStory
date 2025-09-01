using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Events;

public enum EEventType
{
    SubStory = 0,
    Monster,
    Basic,
    MainStory
}

public class StoryManager : MonoBehaviour
{
    public UnityEvent<Condition> OnConditionClick;
    public UnityEvent<Reword> OnReword;
    public UnityEvent<Penalty> OnPanalty;

    public List<int> mainStoryChepter = new List<int>();
    public List<GameObject> mainStoryEvent = new List<GameObject>();
    public List<GameObject> subStoreEvent = new List<GameObject>();
    public List<GameObject> MonsterStoryEvent = new List<GameObject>();
    public List<GameObject> BasicStoryEvent = new List<GameObject>();

    public int currentTurn = 0;
    public int currentsubStoryEvent = 0;

    [SerializeField] GameObject storyEventObject;

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

    private bool CheckMainStoryChepter()
    {
        for (int i = 0; i < mainStoryChepter.Count; i++)
        {
            if (mainStoryChepter[i] == currentTurn)
            {
                storyEventObject = Instantiate(mainStoryEvent[i]);
                return true;
            }
        }

        return false;
    }

    private void RandomStoryEvnet()
    {
        EEventType randomValue = (EEventType)(Random.Range(0, (int)EEventType.MainStory - 1));
        switch (randomValue)
        {
            case EEventType.SubStory:
                {
                    if (currentsubStoryEvent >= subStoreEvent.Count)
                    {
                        RandomStoryEvnet();
                        return;
                    }

                    storyEventObject = Instantiate(subStoreEvent[currentsubStoryEvent]);
                    currentsubStoryEvent++;
                }
                break;
            case EEventType.Monster:
                {
                    int randomSpawnMonster = Random.Range(0, MonsterStoryEvent.Count - 1);
                    storyEventObject = Instantiate(MonsterStoryEvent[randomSpawnMonster]);
                }
                break;
            case EEventType.Basic:
                {
                    int randomSpawnMonster = Random.Range(0, BasicStoryEvent.Count - 1);
                    storyEventObject = Instantiate(BasicStoryEvent[randomSpawnMonster]);
                }
                break;
        }
    }
}
