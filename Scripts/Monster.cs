using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [Header ("Monster Setting")]
    public Slider slider;
    public AnimationClip hitAnim;
    public AnimationClip AttackAnim;

    StoryEvent storyEvent;
    Animator animator;

    int defaultHp;

    void Start()
    {
        slider.value = 1;

        storyEvent = GetComponentInParent<StoryEvent>();
        animator = GetComponent<Animator>();

        defaultHp = storyEvent.succedConditionValue;
    }

    void Update()
    {
        slider.value = (float)storyEvent.succedConditionValue / (float)defaultHp;
    }

    public void Damaged()
    {
        GameManager.instance.OnPlayerAttack?.Invoke(true);
        if (storyEvent.succedConditionValue > 0)
        {
            animator.SetBool("IsHit", true);
            StartCoroutine(HitResetBool());
        }
        else
        {
            animator.SetBool("IsDeath", true);
        }
    }

    public void Attack()
    {
        animator.speed = 2f;
        animator.SetBool("IsAttack", true);
        StartCoroutine(AttackResetBool());
    }

    IEnumerator HitResetBool()
    {
        yield return new WaitForSeconds(hitAnim.length);
        animator.SetBool("IsHit", false);
    }
    IEnumerator AttackResetBool()
    {
        yield return new WaitForSeconds(AttackAnim.length / 2);
        animator.SetBool("IsAttack", false);
        animator.speed = 1f;
    }
}
