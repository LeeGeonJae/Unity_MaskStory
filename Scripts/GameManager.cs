using UnityEngine;

public enum EStageType
{
    Stage_1 = 0,
    Stage_2,
    Stage_3,
    Stage_4
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ObjectManager objectManager;

    EStageType stageType = EStageType.Stage_1;
    bool isPlayerMove = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }

        instance = this;
    }

    public EStageType GetCurrentState()
    {
        return stageType;
    }

    public bool GetIsPlayerMove()
    {
        return isPlayerMove;
    }
}
