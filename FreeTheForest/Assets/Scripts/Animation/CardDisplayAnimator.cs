using UnityEngine;

public class CardDisplayAnimator : MonoBehaviour
{
    [SerializeField] 
    private Animator animator;
    private TargetSlot targetSlot;
    public bool IsDisplayAnimationComplete { get; private set; }

    void Awake()
    {
        this.IsDisplayAnimationComplete = false;
        animator = GetComponent<Animator>();
        if(animator!=null)
        {
            animator.enabled = false;
        }
        targetSlot = GameObject.Find("TargetSlot").GetComponent<TargetSlot>();
    }

    void Update()
    {
    }

    private void ReadyAnimator()
    {
        animator.enabled = true;
        SetIdle(); //set to idle state when its in the target slot.
    }

    private void DisableAnimator()
    {
        animator.enabled = false;
        IsDisplayAnimationComplete = true; //mark the animation complete when it leaves the target slot.
    }

    public void Discard()
    {
        this.animator.enabled = true;
        this.IsDisplayAnimationComplete = false;
        this.animator.SetTrigger("discardTrigger");
    }
    public void SetIdle()
    {
        animator.enabled = true;
        animator.SetTrigger("idleTrigger");
    }

    public void OnDiscardAnimationComplete()
    {
        this.IsDisplayAnimationComplete = false;
        animator.enabled = false;
    }
}