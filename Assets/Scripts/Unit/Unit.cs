using System;
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
    private GameObject projectilePrefab;
    [SerializeField]
    private int hitpoints;
    [SerializeField]
    private float unitHeight;
    [SerializeField]
    private float range;
    [SerializeField]
    private float lineOfSight;
    [SerializeField]
    private float rateOfFire;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private int rangedDamage;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField]
    private LayerMask selectableObjectsLayer;
    [SerializeField]
    private bool attack;
    [SerializeField]
    private bool search;

    private int health;
    private State state;
    private GameObject target;
    private Vector3 seekDestination;
    private GameObject healthBar;
    private GameObject projectile;
    private RectTransform healthBarTransform;
    private Transform projectileTransform;
    private Transform healthBarChild;
    private bool isShooting;
    private bool isProjectileMoving;
    private Vector3 unitCenter;
    private DateTime checkNearbyTime = DateTime.MinValue;

    private Node currentNode;
    private Animator animator;
    private NavMeshAgent unitAgent;
    private SelectionManager selectionManager;
    private SelectionHandler selectionHandler;
    private UnitHandler unitHandler;
    private VillagerManager villageManager;

    //private float speed = 5.0f;
    //private Vector3[] path;
    //private int targetIndex;

    public enum State
    {
        Unassigned,
        Seeking,
        Attacking
    }

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

    public void InstantiateUnit()
    {
        CreateHealthBar();
        CreateProjectile();
    }

    private void CreateProjectile()
    {
        this.projectile = Instantiate(projectilePrefab, gameObject.transform.position + this.unitCenter, gameObject.transform.rotation);
        this.projectileTransform = this.projectile.transform;

        //Hiding the projectile by default
        this.projectile.SetActive(false);
    }

    private void CreateHealthBar()
    {
        this.healthBar = Instantiate(healthBarPrefab, gameObject.transform.position + Vector3.up * unitHeight, healthBarPrefab.transform.rotation);

        //Making it significantly smaller
        this.healthBar.transform.localScale = Vector3.one / 12;

        //Hiding the health bar by default
        this.healthBar.SetActive(false);

        this.healthBarTransform = this.healthBar.GetComponent<RectTransform>();
        this.healthBarTransform.localPosition = gameObject.transform.position + Vector3.up * unitHeight;
        this.healthBarChild = this.healthBar.transform.Find("Bar");

        //Initializing health to hitpoints
        this.health = this.hitpoints;
    }

    public void ToggleHealthBar(bool show)
    {
        this.healthBar.SetActive(show);
    }

    public State GetState() { return this.state; }
    public void SetState(State state) { this.state = state; }

    public int GetHealth() { return this.health; }
    public float GetHealthPercent() { return ((float)this.health) / this.hitpoints; }
    public void Damage(int health)
    {
        this.health -= health;

        if (this.health <= 0)
        {
            RemoveUnit();
            return;
        }

        this.healthBarChild.localScale = new Vector3(GetHealthPercent(), 1);
    }

    void Start()
    {
        this.isStopped = true;
        this.animator = GetComponent<Animator>();
        this.unitAgent = GetComponent<NavMeshAgent>();
        this.selectionManager = Camera.main.GetComponent<SelectionManager>();
        this.selectionHandler = gameObject.GetComponent<SelectionHandler>();
        this.unitHandler = GameObject.FindGameObjectWithTag("UnitHandler").GetComponent<UnitHandler>();
        this.unitCenter = Vector3.up * (unitHeight - 0.6f);
    }

    private void Update()
    {
        if (!this.isStopped)
        {
            float remainingDistance = unitAgent.remainingDistance;
            if (remainingDistance > 0 && remainingDistance < 0.1 && !unitAgent.pathPending)
            {
                StopMoving();
            }

            if (this.healthBar != null) this.healthBarTransform.localPosition = gameObject.transform.position + Vector3.up * unitHeight;
        }

        //Unit functions only when they are alive
        if (this.gameObject.layer != 9)
        {
            if (this.attack)
            {
                Seek();
                Attack();
            }
            if (this.search)
            {
                Search();
            }
        }

        //Debug keys
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (this.selectionHandler.isSelected) RemoveUnit();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (this.selectionHandler.isSelected) Damage(10);
        }
    }

    private void Search()
    {
        //If currently seeking, attacking, moving, or is dead (obstacle layer), then do not attempt to search for a target
        //In other words, only if the unit is idle it will try to search for nearby units to attack
        if (this.state == State.Seeking || this.state == State.Attacking || !this.isStopped || this.gameObject.layer == 9) return;

        DateTime now = DateTime.Now;
        if (now < this.checkNearbyTime)
        {
            return;
        }

        //This method will only run every 1 second rather than every frame update as it is pretty expensive
        this.checkNearbyTime = now.AddSeconds(1);

        RaycastHit[] nearbyObjects = Physics.SphereCastAll(this.transform.position, this.lineOfSight, this.transform.forward, this.lineOfSight, this.selectableObjectsLayer);
        if (nearbyObjects.Length == 0) return;

        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (RaycastHit hit in nearbyObjects)
        {
            GameObject obj = hit.transform.gameObject;
            if (obj.GetInstanceID() == this.gameObject.GetInstanceID()) continue;

            float distance = hit.distance;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }

        if (nearest != null) SetTask(nearest);
    }

    private void Seek()
    {
        if (this.state != State.Seeking) return;

        if (this.target == null)
        {
            this.state = State.Unassigned;
            return;
        }

        //Checking if target is dead
        if (this.target.layer == 9)
        {
            this.state = State.Unassigned;
            return;
        }

        float distance = GetDistanceToTarget();

        if (distance <= this.range)
        {
            this.state = State.Attacking;
            return;
        }

        if (!IsWithinRange(this.target.transform.position, this.seekDestination))
        {
            //fix this
            //adding + 1 for now
            this.seekDestination = transform.position + GetDirectionVectorToTarget() * (distance - range + 1);
            this.unitHandler.MoveUnitToNode(this, this.seekDestination, false);
            //this.seekDestination = this.unitHandler.MoveUnitToNode(this, this.target.transform.position, false, this.range);
        }
    }

    private void Attack()
    {
        if (this.state != State.Attacking)
        {
            if (this.isShooting)
            {
                StopCoroutine("Shoot");
                this.isShooting = false;
            }
            return;
        }

        if (this.target == null)
        {
            this.state = State.Unassigned;
            return;
        }

        //Checking if target is dead
        if (this.target.layer == 9)
        {
            this.state = State.Unassigned;
            return;
        }

        float distance = GetDistanceToTarget();

        if (distance > this.range)
        {
            this.state = State.Seeking;
            return;
        }

        //Stop movement before unit can shoot
        /*if (!this.isStopped)
        {
            this.unitAgent.isStopped = true;
            StopMoving();
        }*/

        this.gameObject.transform.rotation = Quaternion.LookRotation(GetDirectionVectorToTarget());

        if (!isShooting)
        {
            StartCoroutine("Shoot");
        }
    }

    private IEnumerator Shoot()
    {
        for (;;)
        {
            this.isShooting = true;
            this.animator.SetTrigger("Attack");
            if (!isProjectileMoving) StartCoroutine("AnimateProjectile");
            yield return new WaitForSeconds(10f / this.rateOfFire);
        }
    }

    private IEnumerator AnimateProjectile()
    {
        //Delaying the release of the arrow so that it is in sync with the bow animation
        yield return new WaitForSeconds(0.45f);

        this.projectileTransform.position = gameObject.transform.position + this.unitCenter;
        this.projectileTransform.rotation = gameObject.transform.rotation;
        this.projectile.SetActive(true);

        while (GetDistanceToTarget(this.target.transform.position + Vector3.up * (this.unitHeight - 1), this.projectileTransform.position) > 1)
        {
            this.isProjectileMoving = true;

            Vector3 directionVector = GetDirectionVectorToTargetCenter();

            this.projectileTransform.rotation = Quaternion.LookRotation(directionVector);
            this.projectileTransform.position += directionVector * this.projectileSpeed * Time.deltaTime;
            yield return null;
        }

        StopCoroutine("AnimateProjectile");
        this.projectile.SetActive(false);
        this.isProjectileMoving = false;

        Unit targetUnit = this.target.GetComponent<Unit>();
        if (targetUnit != null) targetUnit.Damage(this.rangedDamage);
    }

    private float GetDistanceToTarget(Vector3 target, Vector3 from)
    {
        return Mathf.Abs(Vector3.Distance(target, from));
    }

    private float GetDistanceToTarget()
    {
        if (this.target == null) return -1;
        return GetDistanceToTarget(transform.position);
    }

    public void SetTask(GameObject obj)
    {
        if (this.gameObject.GetInstanceID() == obj.GetInstanceID()) return;

        this.target = obj;
        float distance = GetDistanceToTarget();

        if (distance > this.range)
        {
            this.state = State.Seeking;
        }
        else
        {
            this.state = State.Attacking;
        }
    }

    private Vector3 GetDirectionVectorToTarget()
    {
        if (this.target == null) return Vector3.zero;
        return (this.target.transform.position - transform.position).normalized;
    }

    private Vector3 GetDirectionVectorToTargetCenter()
    {
        if (this.target == null) return Vector3.zero;
        return ((this.target.transform.position + this.unitCenter) - this.projectileTransform.position).normalized;
    }

    private float GetDistanceToTarget(Vector3 from)
    {
        return GetDistanceToTarget(this.target.transform.position, from);
    }

    private bool IsWithinRange(Vector3 target, Vector3 position)
    {
        return Mathf.Abs(Vector3.Distance(target, position)) <= this.range;
    }

    public void Move(Vector3 target, GameObject resource)
    {
        //PathRequestManager.RequestPath(transform.position, target, OnPathFound);
        unitAgent.SetDestination(target);
        StartMoving(resource);
    }

    public void StopMoving()
    {
        animator.SetBool("isStopped", true);
        isStopped = true;
    }

    public void StartMoving(GameObject obj)
    {
        animator.SetBool("isStopped", false);
        isStopped = false;
        if (obj != null)
        {
            this.villageManager = gameObject.GetComponent<VillagerManager>();
            Resource resource = obj.GetComponent<InteractableResource>().getResource();
            this.villageManager.work(resource);
        }
    }

    private void RemoveUnit()
    {
        animator.SetBool("isDead", true);

        //Setting unit to be in obstacle layer
        this.gameObject.layer = 9;

        //Setting the tag to dead - not needed anymore as the object is beibng moved to the obstacle layer
        //this.gameObject.tag = "Dead";

        //De-select the unit if it was selected
        if (this.selectionHandler.isSelected) this.selectionManager.RemoveFromSelection(gameObject, selectionHandler);

        //Making the current node unoccupied
        this.currentNode.isOccupied = false;

        //Stopping corountines if the unit was in the process of attacking
        if (this.state == State.Attacking)
        {
            StopCoroutine("Shoot");
            StopCoroutine("AnimateProjectile");
        }

        //Clearing selection capability to make sure that the unit is not controllable after death
        Destroy(gameObject.GetComponent<SelectionHandler>());

        //Destroying healthbar and projectile for unit as well
        Destroy(this.healthBar);
        Destroy(this.projectile);

        //By giving the unit velocity in the direction that they are moving, the death looks realistic
        NavMeshAgent unitAgent = gameObject.GetComponent<NavMeshAgent>();
        unitAgent.stoppingDistance = 2;
        unitAgent.SetDestination(gameObject.transform.position + unitAgent.velocity * 1.5f);

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
