using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EndingType
{
    GameOver_Goblin,
    GameOver_Skeleton,
    GameOver_FlyingEye,
    GameOver_Mushroom,
    GameOver_EvilWizard1,
    GameOver_EvilWizard2,
    GameOver_Bandit,
    GameOver_Knight,
    GameEnding_Karen,
    GameEnding_Normal,
}

[System.Serializable]
public class Ending
{
    public EndingType EndingType;
    public Sprite EndingImage;
}

public class EndingManager : MonoBehaviour
{
    [Header("Image")]
    public GameObject ImageBoard;
    public List<Ending> ending;

    [SerializeField]List<Ending> showEndings = new List<Ending>();
    int endingNum = 0;

    GameManager gameManager;

    void Start()
    {
        ImageBoard.SetActive(false);

        gameManager = GameManager.instance;
        gameManager.updateGameState.AddListener(UpdateEnding);
    }

    public void UpdateEnding(GameStateType gameState)
    {
        EndingDisable(gameState);
        if (gameState != GameStateType.GameEnding_TextWrite)
        {
            return;
        }

        if (endingNum == showEndings.Count - 1)
        {
            gameManager.textManager.ShowText(showEndings[endingNum].EndingType.ToString(), GameStateType.GameEnding_CompleteEvent);
        }
        else
        {
            gameManager.textManager.ShowText(showEndings[endingNum].EndingType.ToString(), GameStateType.GameEnding_CompleteText);
            endingNum++;
        }
    }

    private void EndingDisable(GameStateType gameStateType)
    {
        if (gameStateType != GameStateType.GameEnding_CompleteText
            && gameStateType != GameStateType.GameEnding_CompleteEvent
            && gameStateType != GameStateType.GameEnding_TextWrite)
        {
            ImageBoard.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        }
    }

    public void AddEnding(EndingType endingType)
    {
        for (int i = 0; i < ending.Count; i++)
        {
            if (ending[i].EndingType == endingType)
            {
                showEndings.Add(ending[i]);
            }
        }
    }
}
