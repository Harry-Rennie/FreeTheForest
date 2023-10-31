using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAnimator : MonoBehaviour
{
    private Queue<string> animationQueue = new Queue<string>();
    private bool isAnimating = false;
    public Animator animator;
    public Image cardSprite;

    public bool IsAnimationComplete;
    void Awake()
    {
        animator = GetComponent<Animator>();
        this.IsAnimationComplete = false;
        //stop animator
        StopAnimator();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
void Update()
{
    if (!animator.enabled) return;

    if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
    {
        this.IsAnimationComplete = true;
        StopAnimator();
    }
}

    public void Draw()
    {
        Debug.Log("Draw called");
        animationQueue.Enqueue("drawTrigger");
        
        if (!isAnimating) StartCoroutine(PlayAnimations());
    }

    IEnumerator PlayAnimations()
    {
        isAnimating = true;
        
        while(animationQueue.Count > 0)
        {
            string trigger = animationQueue.Dequeue();
            animator.enabled = true;
            this.IsAnimationComplete = false;
            cardSprite.enabled = true;
            animator.SetTrigger(trigger);
            
            // Wait for animation to complete
            yield return new WaitUntil(() => IsAnimationComplete);
        }
        
        isAnimating = false;
    }

    public void SetIdle()
    {
        //start animator
        animator.enabled = true;
        animator.SetTrigger("idleTrigger");
    }
    public void StopAnimator()
    {
        //stop animator
        this.IsAnimationComplete = true;
        SetIdle();
    }
}
