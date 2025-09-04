using UnityEngine;
using TMPro;

public class PlayerStatDisplay : MonoBehaviour
{
    [SerializeField]
    public Player playerObject;

    TextMeshProUGUI textUI;

    private void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        textUI.text = "HP:" + playerObject.playerStat.CurrentHp + "   힘:" + playerObject.playerStat.Power + "   지능:" + playerObject.playerStat.Intelligence + "   민첩:" + playerObject.playerStat.Agility;
    }
}
