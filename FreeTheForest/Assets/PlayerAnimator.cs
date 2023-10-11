using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        //stop animator
        StopAnimator();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        //start animator
        animator.enabled = true;
        animator.SetTrigger("attackTrigger");
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
        animator.enabled = false;
    }
}
