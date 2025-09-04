using UnityEngine;
using UnityEngine.UIElements;

public enum GameButtonType
{ 
    GameStart = 0,
    GameExit,
    Act,
    Observe,
    Pass,
    EventCompelete,
    EndingNext,
    EndingComplete
}


public class GameButton : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] GameButtonType type;

    void OnEnable()
    {
    }

    public void OnButtonClick()
    {
        Debug.Log("OnButtonClick 함수 실행");

        if (type == GameButtonType.GameStart)
        {
            GameManager.instance.SetGameState(GameStateType.MoveNextStep);
        }
        else if(type == GameButtonType.GameExit)
        {
            Application.Quit();
        }
        else if(type == GameButtonType.EventCompelete)
        {
            GameManager.instance.SetGameState(GameStateType.MoveNextStep);
        }
        else if(type == GameButtonType.EndingNext)
        {
            GameManager.instance.SetGameState(GameStateType.GameEnding_TextWrite);
        }
        else if(type == GameButtonType.EndingComplete)
        {
            GameManager.instance.SetGameState(GameStateType.GameExit);
        }
        else
        {
            GameManager manager = GameManager.instance;
            for (int i = 0; i < manager.inGame_SelectButtonUI.Count; i++)
            {
                manager.inGame_SelectButtonUI[i].SetActive(false);
            }
        }

        GameManager.instance.onGameButton?.Invoke(type);
    }
}
