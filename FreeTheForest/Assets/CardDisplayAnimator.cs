using UnityEngine;

public class CardDisplayAnimator : MonoBehaviour
{
    [SerializeField] 
    private Animator animator;

    [SerializeField] 
    private TargetSlot targetSlot;
    public bool IsDisplayAnimationComplete { get; set; }

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger("idleTrigger");
            if (animator == null)
            {
                Debug.LogError("Animator component is not found on the GameObject.");
            }
        }

        // Initialize your variables.
        IsDisplayAnimationComplete = false;
    }

    void Update()
    {
        if (animator != null && !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            IsDisplayAnimationComplete = true;
        }
    }

    public void Discard()
    {
        Debug.Log("Discard");
        Debug.Log("Animator: " + animator);
        animator.enabled = true;
        IsDisplayAnimationComplete = false;
        animator.SetTrigger("discardTrigger");
    }

    public void SetIdle()
    {
        if (animator == null) return;
        animator.SetTrigger("idleTrigger");
    }
    public void OnDiscardAnimationComplete()
    {
        IsDisplayAnimationComplete = true;
    }
}
