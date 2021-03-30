using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public bool isStopped;

    private Node currentNode;

    private float speed = 5.0f;
    private Vector3[] path;
    private int targetIndex;

    private Animator animator;

    public Node CurrentNode
    {
        get
        {
            return this.currentNode;
        }
        set
        {
            if (this.currentNode != null)
            {
                this.currentNode.isOccupied = false;
            }
            this.currentNode = value;
            this.currentNode.isOccupied = true;
        }
    }

    void Start()
    {
        isStopped = true;
        animator = GetComponent<Animator>();
    }

    public void Move(Vector3 target)
    {
        PathRequestManager.RequestPath(transform.position, target, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    StopMoving();
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
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
