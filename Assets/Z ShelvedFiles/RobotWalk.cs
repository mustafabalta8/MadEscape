using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWalk : MonoBehaviour
{
    Animator animator;

    [SerializeField] bool isWalking;
    [SerializeField] bool isTurning;
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        animator.SetFloat("direction", h);
        float v = Input.GetAxis("Vertical");
        animator.SetFloat("speed", v);

        animator.SetBool("walk", isWalking);
        animator.SetBool("turn", isTurning);
    }
}
