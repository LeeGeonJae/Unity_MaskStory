using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Monster : MonoBehaviour
{
    [Header ("Monster Setting")]
    public UnityEngine.UI.Slider slider;
    public AnimationClip hitAnim;
    public AnimationClip AttackAnim;

    [Header("Audio")]
    AudioSource audioSource1;
    AudioSource audioSource2;
    public List<AudioClip> attackSound;
    public AudioClip tackSound;
    public AudioClip hitSound;
    public AudioClip DeathSound;

    StoryEvent storyEvent;
    Animator animator;

    int defaultHp;

    void Start()
    {
        slider.value = 1;
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource1.outputAudioMixerGroup = GameManager.instance.soundManager.sfxGroup;
        audioSource2.outputAudioMixerGroup = GameManager.instance.soundManager.sfxGroup;
        storyEvent = GetComponentInParent<StoryEvent>();
        animator = GetComponent<Animator>();

        defaultHp = storyEvent.succedConditionValue;
    }

    void Update()
    {
        slider.value = (float)storyEvent.succedConditionValue / (float)defaultHp;
    }

    public void Tack()
    {
        if (tackSound)
        {
            PlaySound(tackSound);
        }
    }

    public void Damaged()
    {
        GameManager.instance.OnPlayerAttack?.Invoke(true);
        if (storyEvent.succedConditionValue > 0)
        {
            PlaySound(hitSound);
            animator.SetBool("IsHit", true);
            StartCoroutine(HitResetBool());
        }
        else
        {
            slider.gameObject.SetActive(false);
            PlaySound(DeathSound);
            animator.SetBool("IsDeath", true);
        }
    }

    public void Attack()
    {
        for (int i = 0; i < attackSound.Count; i++)
        {
            PlaySound(attackSound[i], i + 1);
        }
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

    private void PlaySound(AudioClip audio, int num = 1)
    {
        if (num == 1)
        {
            audioSource1.resource = audio;
            audioSource1.loop = false;
            audioSource1.Play();
        }
        else
        {
            audioSource2.resource = audio;
            audioSource2.loop = false;
            audioSource2.Play();
        }

    }
}
