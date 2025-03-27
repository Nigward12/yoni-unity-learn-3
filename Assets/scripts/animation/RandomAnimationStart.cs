using UnityEngine;

public class RandomAnimationStart : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        animator.Play(state.fullPathHash, 0, Random.Range(0f, 1f));
    }
}
