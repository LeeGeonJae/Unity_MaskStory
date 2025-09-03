using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerStatType
{
    Power,
    Agility,
    Intelligence,
    Hp
}

public enum PlayerStateType
{
    Idle,
    Run,
    Attack,
    Hit,
    Death
}

[System.Serializable]
public class PlayerStat
{
    public int Power = 10;
    public int Agility = 10;
    public int Intelligence = 10;
    public int MaxHp = 100;
    public int CurrentHp = 100;
    public double Damage = 10;
}

public class Player : MonoBehaviour
{
    // 애니메이터
    [Header("Animation")]
    Animator animator;
    
    // 애니메이션 클립
    public AnimationClip attackAnim;
    public AnimationClip hitAnim;

    // 플레이어 스텟
    [Header("PlayerStat")]
    public PlayerStat playerStat;
    public Slider hpBar;

    // 플레이어 상태
    PlayerStateType playerStateType;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameManager.instance.updateGameState.AddListener(CheckInGameState);
        GameManager.instance.onGameButton.AddListener(PlayerButtonClick);
        GameManager.instance.OnPlayerAttack.AddListener(Attack);
        GameManager.instance.storyManager.OnReword.AddListener(GetRewrod);
        GameManager.instance.storyManager.OnPenalty.AddListener(GetPanalty);

        UpdatePlayerStat();
        playerStat.CurrentHp = playerStat.MaxHp;
        hpBar.value = 1;
    }

    private void OnDestroy()
    {
        GameManager.instance.updateGameState.RemoveListener(CheckInGameState);
        GameManager.instance.onGameButton.RemoveListener(PlayerButtonClick);
        GameManager.instance.OnPlayerAttack.RemoveListener(Attack);
        GameManager.instance.storyManager.OnReword.RemoveListener(GetRewrod);
        GameManager.instance.storyManager.OnPenalty.RemoveListener(GetPanalty);
    }

    public void Attack(bool check)
    {
        ChangePlayerState(PlayerStateType.Attack);
    }

    public void Damaged(int damage)
    {
        playerStat.CurrentHp -= damage;
        if (playerStat.CurrentHp > 0)
        {
            hpBar.value = (float)playerStat.CurrentHp / (float)playerStat.MaxHp;
            ChangePlayerState(PlayerStateType.Hit);
        }
        else
        {
            hpBar.value = 0;
            ChangePlayerState(PlayerStateType.Death);
        }
    }

    private void GetRewrod(Reword reword)
    {
        switch (reword.StatType)
        {
            case PlayerStatType.Power:
                playerStat.Power += reword.RewordValue;
                break;
            case PlayerStatType.Agility:
                playerStat.Agility += reword.RewordValue;
                break;
            case PlayerStatType.Intelligence:
                playerStat.Intelligence += reword.RewordValue;
                break;
            case PlayerStatType.Hp:
                playerStat.CurrentHp += reword.RewordValue;
                break;
        }

        UpdatePlayerStat();
    }

    private void GetPanalty(Penalty penalty)
    {
        switch (penalty.StatType)
        {
            case PlayerStatType.Power:
                playerStat.Power -= penalty.PenaltydValue;
                break;
            case PlayerStatType.Agility:
                playerStat.Agility -= penalty.PenaltydValue;
                break;
            case PlayerStatType.Intelligence:
                playerStat.Intelligence -= penalty.PenaltydValue;
                break;
            case PlayerStatType.Hp:
                Damaged(penalty.PenaltydValue);
                break;
        }

        UpdatePlayerStat();
    }

    private void CheckInGameState(GameStateType gameState)
    {
        if (gameState == GameStateType.MoveNextStep)
        {
            ChangePlayerState(PlayerStateType.Run);
        }
        else if (gameState != GameStateType.StoryEvent_SelectChoice)
        {
            ChangePlayerState(PlayerStateType.Idle);
        }
    }

    private void ChangePlayerState(PlayerStateType stateType)
    {
        Debug.Log("ChangePlayerState 함수 실행");

        playerStateType = stateType;
        if (animator)
        {
            switch (stateType)
            {
                case PlayerStateType.Idle:
                    {
                        animator.SetBool("IsPlayerRun", false);
                        animator.SetBool("IsPlayerAttack", false);
                        animator.SetBool("IsPlayerHit", false);
                    }
                    break;
                case PlayerStateType.Run:
                    {
                        animator.SetBool("IsPlayerRun", true);
                    }
                    break;
                case PlayerStateType.Attack:
                    {
                        animator.SetBool("IsPlayerAttack", true);
                        StartCoroutine(AttackResetBool());
                    }
                    break;
                case PlayerStateType.Hit:
                    {
                        animator.SetBool("IsPlayerHit", true);
                        StartCoroutine(HitResetBool());
                    }
                    break;
                case PlayerStateType.Death:
                    {
                        animator.SetBool("IsPlayerDeath", true);
                    }
                    break;
            }
        }
    }

    IEnumerator AttackResetBool()
    {
        yield return new WaitForSeconds(attackAnim.length);
        ChangePlayerState(PlayerStateType.Idle);
    }
    IEnumerator HitResetBool()
    {
        yield return new WaitForSeconds(hitAnim.length);
        ChangePlayerState(PlayerStateType.Idle);
    }

    private void PlayerButtonClick(GameButtonType type)
    {
        switch (type)
        {
            case GameButtonType.Act:
                GameManager.instance.OnConditionClick?.Invoke(ActionType.Act, playerStat);
                break;
            case GameButtonType.Observe:
                GameManager.instance.OnConditionClick?.Invoke(ActionType.Observe, playerStat);
                break;
            case GameButtonType.Pass:
                GameManager.instance.OnConditionClick?.Invoke(ActionType.Pass, playerStat);
                break;
        }
    }

    // 플레이어 스텟 갱신
    private void UpdatePlayerStat()
    {
        playerStat.Damage = (double)playerStat.Power * 1.5 + (double)playerStat.Agility * 0.8 + (double)playerStat.Intelligence * 0.5;
        playerStat.MaxHp = 100 + playerStat.Power * 2;

        if (playerStat.CurrentHp > playerStat.MaxHp)
        {
            playerStat.CurrentHp = playerStat.MaxHp;
        }
        hpBar.value = (float)playerStat.CurrentHp / (float)playerStat.MaxHp;
    }
}
