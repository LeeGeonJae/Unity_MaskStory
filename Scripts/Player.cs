using System.Collections;
using UnityEngine;

public enum EPlayerStatType
{
    Poawe,
    Agility,
    Intelligence,
    Hp
}

public enum EPlayerStateType
{
    Idle,
    Run,
    Attack,
    Hit,
    Death
}

public class Player : MonoBehaviour
{
    // 애니메이터
    Animator animator;
    
    // 애니메이션 클립
    public AnimationClip attackAnim;
    public AnimationClip hitAnim;

    // 플레이어 상태
    EPlayerStateType playerStateType;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameManager.instance.updatePlayerState.AddListener(ChangePlayerState);
    }

    private void OnDestroy()
    {
        GameManager.instance.updatePlayerState.RemoveListener(ChangePlayerState);
    }

    public void ChangePlayerState(EPlayerStateType stateType)
    {
        Debug.Log("ChangePlayerState 함수 실행");

        playerStateType = stateType;
        if (animator)
        {
            switch (stateType)
            {
                case EPlayerStateType.Idle:
                    {
                        animator.SetBool("IsPlayerRun", false);
                        animator.SetBool("IsPlayerAttack", false);
                        animator.SetBool("IsPlayerHit", false);
                    }
                    break;
                case EPlayerStateType.Run:
                    {
                        animator.SetBool("IsPlayerRun", true);
                    }
                    break;
                case EPlayerStateType.Attack:
                    {
                        animator.SetBool("IsPlayerAttack", true);
                        StartCoroutine(AttackResetBool());
                    }
                    break;
                case EPlayerStateType.Hit:
                    {
                        animator.SetBool("IsPlayerHit", true);
                        StartCoroutine(HitResetBool());
                    }
                    break;
                case EPlayerStateType.Death:
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
        ChangePlayerState(EPlayerStateType.Idle);
    }
    IEnumerator HitResetBool()
    {
        yield return new WaitForSeconds(hitAnim.length);
        ChangePlayerState(EPlayerStateType.Idle);
    }
}
