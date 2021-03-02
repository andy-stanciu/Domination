using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isIdle = animator.GetBool("isIdle");
        bool isPunching = animator.GetBool("isPunching");
        bool isKicking = animator.GetBool("isKicking");
        bool walkingBack = animator.GetBool("walkingBack");
        bool dyingForward = animator.GetBool("dyingForward");
        bool dyingLeft = animator.GetBool("dyingLeft");
        bool dead = animator.GetBool("dead");
        bool disarmingBow = animator.GetBool("disarmingBow");
        bool drawingArrow = animator.GetBool("drawingArrow");

        if (!isPunching && !dead && Input.GetKey("p"))
        {
          animator.SetBool("isPunching", true);
          animator.SetBool("isIdle", false);
        }

        if (isPunching && !dead && !Input.GetKey("p"))
        {
          animator.SetBool("isPunching", false);
        }

        if (!isKicking && !dead && Input.GetKey("k"))
        {
          animator.SetBool("isKicking", true);
          animator.SetBool("isIdle", false);
        }

        if (isKicking && !dead && !Input.GetKey("k"))
        {
          animator.SetBool("isKicking", false);
        }

        if (!walkingBack && !dead && Input.GetKey("s"))
        {
          animator.SetBool("walkingBack", true);
          animator.SetBool("isIdle", false);
        }

        if (walkingBack && !dead && !Input.GetKey("s"))
        {
          animator.SetBool("walkingBack", false);
        }

        if (!dyingForward && !dead && Input.GetKey("q"))
        {
          animator.SetBool("dyingForward", true);
          animator.SetBool("dead", true);
        }

        if (dyingForward && !Input.GetKey("q"))
        {
          animator.SetBool("dyingForward", false);
        }

        if (!dyingLeft && !dead && Input.GetKey("e"))
        {
          animator.SetBool("dyingLeft", true); 
          animator.SetBool("dead", true);
        }

        if (dyingLeft && !Input.GetKey("e"))
        {
          animator.SetBool("dyingLeft", false);
        }

        if (!disarmingBow && !dead && Input.GetKey("d"))
        {
          animator.SetBool("disarmingBow", true);
          animator.SetBool("isIdle", false);
        }

        if (disarmingBow && !dead && !Input.GetKey("d"))
        {
          animator.SetBool("disarmingBow", false);
        }

        if (!drawingArrow && !dead && Input.GetKey("a"))
        {
          animator.SetBool("drawingArrow", true);
          animator.SetBool("isIdle", false);
        }

        if (drawingArrow && !dead && !Input.GetKey("a"))
        {
          animator.SetBool("drawingArrow", false);
        }
        /*
        if (!dead && !isPunching && !isKicking && !drawingArrow && !disarmingBow && !walkingBack)
        {
          animator.SetBool("isIdle", true);
        }
        */
    }
}
