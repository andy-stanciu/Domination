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
    private int meleeDamage;
    [SerializeField]
    private float meleeRange;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField]
    private LayerMask selectableObjectsLayer;
    [SerializeField]
    private bool attack;
    [SerializeField]
    private bool seek;
    [SerializeField]
    private bool search;
    [SerializeField]
    private bool canPerformTask;

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
    private bool isMelee;
    private bool isProjectileMoving;
    private Vector3 unitCenter;
    private DateTime checkNearbyTime = DateTime.MinValue;

    private Node currentNode;
    private Animator animator;
    private NavMeshAgent unitAgent;
    private SelectionManager selectionManager;
    private SelectionHandler selectionHandler;
    private UnitHandler unitHandler;
    private Villager villager;
    private SkinnedMeshRenderer alphaPrefab;

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

    public void InstantiateUnit(bool isOpponent)
    {
        UpdateColor();
        CreateHealthBar(isOpponent);
        CreateProjectile();
    }

    private void UpdateColor()
    {
        if (this.selectionHandler == null) this.selectionHandler = gameObject.GetComponent<SelectionHandler>();

        Color color = Color.red;
        if (this.gameObject.CompareTag("Player")) color = Color.blue;

        this.selectionHandler.UpdateColor(color);
        SetAlpha(color);
    }

    private void SetAlpha(Color color)
    {
        if (this.alphaPrefab == null)
        {
            if (!this.canPerformTask)
            {
                this.alphaPrefab = this.transform.Find("Erika_Archer_Meshes").transform.Find("Erika_Archer_Clothes_Mesh").GetComponent<SkinnedMeshRenderer>();
                this.alphaPrefab.material.color = color;
            }
            else
            {
                this.alphaPrefab = this.transform.Find("Guard02").GetComponent<SkinnedMeshRenderer>();
                if (color == Color.red) this.alphaPrefab.material.color = Color.red + new Color(0, 0.5f, 0.5f);
                else this.alphaPrefab.material.color = Color.blue + new Color(0.5f, 0.5f, 0);
            }
            
        }
    }

    private void CreateProjectile()
    {
        this.projectile = Instantiate(projectilePrefab, gameObject.transform.position + this.unitCenter, gameObject.transform.rotation);
        this.projectileTransform = this.projectile.transform;

        //Hiding the projectile by default
        this.projectile.SetActive(false);
    }

    private void CreateHealthBar(bool isOpponent)
    {
        this.healthBar = Instantiate(healthBarPrefab, gameObject.transform.position + Vector3.up * unitHeight, healthBarPrefab.transform.rotation);

        //Making it significantly smaller
        this.healthBar.transform.localScale = Vector3.one / 12;

        //Hiding the health bar by default
        this.healthBar.SetActive(false);

        this.healthBarTransform = this.healthBar.GetComponent<RectTransform>();
        this.healthBarTransform.localPosition = gameObject.transform.position + Vector3.up * unitHeight;
        this.healthBarChild = this.healthBar.transform.Find("Bar");
        this.healthBarChild.transform.Find("BarSprite").GetComponent<SpriteRenderer>().color = isOpponent ? Color.red : Color.blue;

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

        if (this.healthBarChild != null)
        {
            this.healthBarChild.localScale = new Vector3(GetHealthPercent(), 1);
        }
    }

    void Start()
    {
        this.isStopped = true;
        this.animator = GetComponent<Animator>();
        this.unitAgent = GetComponent<NavMeshAgent>();
        this.selectionManager = Camera.main.GetComponent<SelectionManager>();
        if (this.selectionHandler == null) this.selectionHandler = gameObject.GetComponent<SelectionHandler>();
        this.unitHandler = GameObject.FindGameObjectWithTag("UnitHandler").GetComponent<UnitHandler>();
        this.unitCenter = Vector3.up * (unitHeight - 0.6f);
        this.villager = gameObject.GetComponent<Villager>();
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
            if (this.attack) Attack();
            if (this.seek) Seek();
            if (this.search) Search();
        }

        //Debug keys
        /*if (Input.GetKeyDown(KeyCode.K))
        {
            if (this.selectionHandler.isSelected) RemoveUnit();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (this.selectionHandler.isSelected) Damage(10);
        }*/
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
            //Should not search for units of their own team
            if (obj.CompareTag(this.gameObject.tag)) continue;

            float distance = hit.distance;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }

        if (nearest != null) SetTarget(nearest);
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
            //this.unitAgent.isStopped = true;
            return;
        }

        if (!IsWithinRange(this.target.transform.position, this.seekDestination))
        {
            this.seekDestination = transform.position + GetDirectionVectorToTarget() * (distance - range + 1);
            Vector3 attemptedDestination = this.unitHandler.MoveUnitToNode(this, this.seekDestination, false);

            //Testing this out
            //While to keep recalculating position to move to if all the positions are occupied
            //This would usually cause the unit to be stuck out of range, but here it is being recalculated so that it can move in range to shoot
            int i = 1;
            while (this.target != null && 
                GetDistanceToTarget(attemptedDestination, this.target.transform.position) > this.range)
            {
                this.seekDestination = transform.position + GetDirectionVectorToTarget() * (distance - range + 1 + i);
                attemptedDestination = this.unitHandler.MoveUnitToNode(this, this.seekDestination, false);
                i++;
            }
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
            if (this.isMelee)
            {
                StopCoroutine("Melee");
                this.isMelee = false;
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
        if (!this.isStopped)
        {
            this.unitAgent.isStopped = true;
            StopMoving();
        }

        this.gameObject.transform.rotation = Quaternion.LookRotation(GetDirectionVectorToTarget());

        if (distance >= this.meleeRange)
        {
            if (!isShooting) StartCoroutine("Shoot");
            if (isMelee)
            {
                StopCoroutine("Melee");
                this.isMelee = false;
            }
        }
        else
        {
            if (!isMelee) StartCoroutine("Melee");
            if (isShooting)
            {
                StopCoroutine("Shoot");
                this.isShooting = false;
            }
        }
    }

    private IEnumerator Melee()
    {
        for (;;)
        {
            this.isMelee = true;
            int meleeType = this.unitHandler.GetMeleeRandom();
            this.animator.SetTrigger("Melee");
            this.animator.SetInteger("MeleeType", meleeType);

            Unit targetUnit = this.target.GetComponent<Unit>();
            if (targetUnit != null) targetUnit.Damage(this.meleeDamage);

            Building targetBuilding = this.target.GetComponent<Building>();
            if (targetBuilding != null) targetBuilding.Damage(this.meleeDamage);

            yield return new WaitForSeconds(6f / this.rateOfFire);
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

        if (this.target != null && this.projectileTransform != null)
        {
            this.projectileTransform.position = gameObject.transform.position + this.unitCenter;
            this.projectileTransform.rotation = gameObject.transform.rotation;
            this.projectile.SetActive(true);


            while (this.target != null && this.projectileTransform != null && 
                GetDistanceToTarget(this.target.transform.position + Vector3.up * (this.unitHeight - 1), this.projectileTransform.position) > 1)
            {
                this.isProjectileMoving = true;

                Vector3 directionVector = GetDirectionVectorToTargetCenter();

                this.projectileTransform.rotation = Quaternion.LookRotation(directionVector);
                this.projectileTransform.position += directionVector * this.projectileSpeed * Time.deltaTime;
                yield return null;
            }
        }

        StopCoroutine("AnimateProjectile");
        
        if (this.projectile != null)
        {
            this.projectile.SetActive(false);
            this.isProjectileMoving = false;
        }

        if (this.target != null)
        {
            Unit targetUnit = this.target.GetComponent<Unit>();
            if (targetUnit != null) targetUnit.Damage(this.rangedDamage);

            Building targetBuilding = this.target.GetComponent<Building>();
            if (targetBuilding != null) targetBuilding.Damage(this.rangedDamage);
        }
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

    public void SetTarget(GameObject obj)
    {
        if (!this.seek || !this.attack) return;
        if (this.gameObject.GetInstanceID() == obj.GetInstanceID()) return;
        if (obj.CompareTag(this.gameObject.tag)) return;

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

    public void SetTask(GameObject obj)
    {
        if (!this.canPerformTask) return;

        Resource resource = obj.GetComponent<Resource>();
        if (resource == null) return;

        float distance = GetDistanceToTarget(obj.transform.position, transform.position);

        if (distance > this.range) this.unitHandler.MoveUnitToNode(this, obj.transform.position, false);
        
        this.villager.Work(resource);
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

    public void Move(Vector3 target)
    {
        //PathRequestManager.RequestPath(transform.position, target, OnPathFound);
        unitAgent.SetDestination(target);
        
        //Clearing target if there is one
        this.target = null;
        //Clearing resource if there is one
        if (this.villager != null) this.villager.currentResource = null;

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
        if (this.villager == null)
        {
            animator.SetBool("isDead", true);

            //Setting unit to be in obstacle layer
            this.gameObject.layer = 9;

            //Setting the tag to dead - not needed anymore as the object is being moved to the obstacle layer
            //this.gameObject.tag = "Dead";

            //De-select the unit if it was selected
            if (this.selectionHandler.isSelected) this.selectionManager.RemoveFromSelection(gameObject, selectionHandler);

            //Making the current node unoccupied
            this.currentNode.isOccupied = false;

            //Stopping corountines if the unit was in the process of attacking
            if (this.state == State.Attacking)
            {
                StopCoroutine("Shoot");
                StopCoroutine("Melee");
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
            unitAgent.SetDestination(gameObject.transform.position + unitAgent.velocity * 0.5f);

            //Destroying the unit after a bit of time
            Destroy(gameObject, 8f);
        }
        else
        {
            //De-select the unit if it was selected
            if (this.selectionHandler.isSelected) this.selectionManager.RemoveFromSelection(gameObject, selectionHandler);

            //Making the current node unoccupied
            this.currentNode.isOccupied = false;

            //Destroying healthbar and projectile for unit as well
            Destroy(this.healthBar);
            Destroy(this.projectile);

            //Destroying the unit immediately
            Destroy(gameObject);
        }
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
