using UnityEngine;

public class Box : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameManager.instance.storyManager.OnReword.AddListener(BoxOpen);
    }

    private void OnDestroy()
    {
        GameManager.instance.storyManager.OnReword.RemoveListener(BoxOpen);
    }

    private void BoxOpen(Reword reword)
    {
        if (reword.RewordValue != 0)
        {
            animator.SetBool("IsOpened", true);
        }
    }
}
