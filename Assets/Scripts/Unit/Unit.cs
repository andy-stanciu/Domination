using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public bool isStopped;

    [SerializeField]
    private GameObject healthBarPrefab;
    [SerializeField]
    private int hitpoints;
    [SerializeField]
    private float unitHeight;
    [SerializeField]
    private LayerMask obstacleLayer;

    private int health;
    private GameObject healthBar;
    private RectTransform healthBarTransform;

    private Node currentNode;
    private Animator animator;
    private NavMeshAgent unitAgent;
    private SelectionManager selectionManager;

    //private float speed = 5.0f;
    //private Vector3[] path;
    //private int targetIndex;

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

    public void CreateHealthBar()
    {
        this.healthBar = Instantiate(healthBarPrefab, gameObject.transform.position + Vector3.up * unitHeight, healthBarPrefab.transform.rotation);

        //Making it significantly smaller
        this.healthBar.transform.localScale = Vector3.one / 12;

        //Hiding the health bar by default
        this.healthBar.SetActive(false);

        this.healthBarTransform = this.healthBar.GetComponent<RectTransform>();
    }

    public void ToggleHealthBar(bool show)
    {
        this.healthBar.SetActive(show);
    }

    public int GetHealth() { return this.health; }
    public float GetHealthPercent() { return (float) this.health / this.hitpoints; }
    public void Damage(int health)
    {
        if (health <= 0)
        {
            RemoveUnit();
            return;
        }

        this.health -= health;
    }

    void Start()
    {
        this.isStopped = true;
        this.animator = GetComponent<Animator>();
        this.unitAgent = GetComponent<NavMeshAgent>();
        this.selectionManager = Camera.main.GetComponent<SelectionManager>();
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

            if (this.healthBar != null) this.healthBarTransform.localPosition = gameObject.transform.position + Vector3.up * unitHeight;
        }

        if (Input.GetKeyDown(KeyCode.K)) RemoveUnit();
    }

    public void SetTask(GameObject obj)
    {

    }

    public void Move(Vector3 target)
    {
        //PathRequestManager.RequestPath(transform.position, target, OnPathFound);
        unitAgent.SetDestination(target);
        StartMoving();
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

    private void RemoveUnit()
    {
        animator.SetBool("isDead", true);

        //De-select the unit if it was selected
        SelectionHandler selectionHandler = gameObject.GetComponent<SelectionHandler>();
        if (selectionHandler.isSelected) this.selectionManager.RemoveFromSelection(gameObject, selectionHandler);

        //Making the current node unoccupied
        this.currentNode.isOccupied = false;

        //Clearing selection capability to make sure that the unit is not controllable after death
        Destroy(gameObject.GetComponent<SelectionHandler>());

        //Destroying healthbar for unit as well
        Destroy(this.healthBar);

        //By giving the unit velocity in the direction that they are moving, the death looks realistic
        NavMeshAgent unitAgent = gameObject.GetComponent<NavMeshAgent>();
        unitAgent.stoppingDistance = 2;
        unitAgent.SetDestination(gameObject.transform.position + unitAgent.velocity * 2);

        //Destroying the unit after a bit of time
        Destroy(gameObject, 8f);
    }

    /**
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
    }*/
}
