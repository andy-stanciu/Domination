using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Animator animator;

    [HideInInspector]
    public bool isStopped;

    void Start()
    {
        isStopped = true;
        animator = GetComponent<Animator>();
    }

    public void StopMoving()
    {
        isStopped = true;
        animator.SetBool("isStopped", true);
    }

    public void StartMoving()
    {
        isStopped = false;
        animator.SetBool("isStopped", false);
    }
}
