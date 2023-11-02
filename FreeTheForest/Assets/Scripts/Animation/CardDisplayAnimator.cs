using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplayAnimator : MonoBehaviour
{
    private Animator animator;
    private bool isAnimating = false;
    private Queue<string> animationQueue = new Queue<string>();
    public bool discarded;

    public bool IsDisplayAnimationComplete { get; set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        IsDisplayAnimationComplete = false;
        discarded = false;
        StopAnimator();
    }

    void Update()
    {
        if (!animator.enabled) return;

        if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            IsDisplayAnimationComplete = true;
            StopAnimator();
        }
    }

    public void EnqueueAnimation(string triggerName)
    {
        animationQueue.Enqueue(triggerName);

        if (!isAnimating) StartCoroutine(PlayAnimations());
    }

    IEnumerator PlayAnimations()
    {
        isAnimating = true;

        while (animationQueue.Count > 0)
        {
            string trigger = animationQueue.Dequeue();
            StartAnimator();
            IsDisplayAnimationComplete = false;
            animator.SetTrigger(trigger);

            // Wait for animation to complete
            yield return new WaitUntil(() => IsDisplayAnimationComplete);
        }

        isAnimating = false;
        discarded = true;
    }

    public void Discard()
    {
        EnqueueAnimation("discardTrigger");
    }

    public void SetIdle()
    {
        StartAnimator();
        animator.SetTrigger("idleTrigger");
    }

    private void StartAnimator()
    {
        animator.enabled = true;
    }

    private void StopAnimator()
    {
        IsDisplayAnimationComplete = true;
        animator.enabled = false;
    }

    public void OnDiscardAnimationComplete()
    {
        IsDisplayAnimationComplete = true;
        StopAnimator();
    }
}