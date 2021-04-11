using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public bool isStopped;

    private Node currentNode;

    private float speed = 5.0f;
    private Vector3[] path;
    private int targetIndex;

    private Animator animator;
    private NavMeshAgent unitAgent;

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
        unitAgent = GetComponent<NavMeshAgent>();
    }

    public void Move(Vector3 target)
    {
        //PathRequestManager.RequestPath(transform.position, target, OnPathFound);
        unitAgent.SetDestination(target);
        StartMoving();
    }

    private void Update()
    {
        if (!this.isStopped)
        {
            float remainingDistance = unitAgent.remainingDistance;
            if (remainingDistance > 0 && remainingDistance < 0.1)
            {
                StopMoving();
            }
        }
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
        animator.SetBool("isStopped", true);
        isStopped = true;
    }

    public void StartMoving()
    {
        animator.SetBool("isStopped", false);
        isStopped = false;
    }
}
