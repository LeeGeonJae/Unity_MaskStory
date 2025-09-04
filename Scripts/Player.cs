using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    public int Power = 15;
    public int Agility = 15;
    public int Intelligence = 15;
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
    public UnityEngine.UI.Slider hpBar;

    [Header("Audio")]
    AudioSource audioSource;
    public List<AudioClip> attackSound;
    public AudioClip hitSound;
    public AudioClip DeathSound;
    public AudioClip runSound;

    // 플레이어 상태
    PlayerStateType playerStateType;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = GameManager.instance.soundManager.sfxGroup; 

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
            hpBar.gameObject.SetActive(false);
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
            playerStat.CurrentHp += 5;
            if (playerStat.CurrentHp > playerStat.MaxHp)
                playerStat.CurrentHp = playerStat.MaxHp;

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
                        audioSource.Stop();
                    }
                    break;
                case PlayerStateType.Run:
                    {
                        animator.SetBool("IsPlayerRun", true);
                        playSound(runSound, true);
                    }
                    break;
                case PlayerStateType.Attack:
                    {
                        animator.SetBool("IsPlayerAttack", true);
                        StartCoroutine(AttackResetBool());
                        for (int i = 0; i < attackSound.Count; i++)
                        {
                            playSound(attackSound[i], false);
                        }
                    }
                    break;
                case PlayerStateType.Hit:
                    {
                        animator.SetBool("IsPlayerHit", true);
                        StartCoroutine(HitResetBool());
                        playSound(hitSound, false);
                    }
                    break;
                case PlayerStateType.Death:
                    {
                        animator.SetBool("IsPlayerDeath", true);
                        playSound(DeathSound, false);

                        GameManager.instance.GameEndingStart(true);
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

    // 플레이어 버튼 클릭 타입에 따른 스토리 이벤트 이벤트 실행
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
        playerStat.Damage = (double)playerStat.Power * 1.5 + (double)playerStat.Agility * 0.8 + (double)playerStat.Intelligence;
        playerStat.MaxHp = 150 + playerStat.Power * 5;

        if (playerStat.CurrentHp > playerStat.MaxHp)
        {
            playerStat.CurrentHp = playerStat.MaxHp;
        }
        hpBar.value = (float)playerStat.CurrentHp / (float)playerStat.MaxHp;
    }

    private void playSound(AudioClip clip, bool isLoop)
    {
        audioSource.resource = clip;
        audioSource.loop = isLoop;
        audioSource.Play();
    }
}
