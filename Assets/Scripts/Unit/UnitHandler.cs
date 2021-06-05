using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitHandler : MonoBehaviour
{
    public Terrain terrain; 

    private new Camera camera;
    private NodeGrid grid;
    private SelectionManager selectionManager;
    private Vector3 zero;
    private Vector3 negInf;
    private Villager script;
    private System.Random random;

    public LayerMask groundLayer;
    public LayerMask selectableLayer;
    public LayerMask obstacleLayer;

    public GameObject archer;
    public GameObject longbowman;
    public GameObject villager;

    public GameObject barracks;

    void Awake()
    {
        camera = Camera.main;
        grid = GetComponent<NodeGrid>();
        selectionManager = camera.GetComponent<SelectionManager>();
        zero = Vector3.zero;
        negInf = Vector3.negativeInfinity;
        this.random = new System.Random();
    }

    void Start()
    {
        CreateUnits(longbowman, 5, 4, zero, false);
        CreateUnits(longbowman, 5, 4, new Vector3(0, 0, -20), true);
        //CreateUnits(longbowman, 4, 5, zero, false);
        //CreateUnits(longbowman, 4, 5, new Vector3(0, 0, -30), true);
        //CreateUnits(longbowman, 10, 10, zero);
        //CreateUnits(longbowman, 4, 5, zero);
        CreateUnits(villager, 5, 4, zero, false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RightClickNode();
        }
    }

    public int GetMeleeRandom()
    {
        return this.random.Next(1, 6);
    }

    private void RightClickNode()
    {
        if (selectionManager.selectedObjects.Count > 0)
        {
            foreach (GameObject obj in selectionManager.selectedObjects)
            {
                Unit unit = obj.GetComponent<Unit>();
                if (unit != null)
                {
                    //Only moving/tasking the unit if it is the player's unit
                    if (unit.CompareTag("Player")) MoveUnit(unit);
                }
            }
        }
    }

    public void MoveUnit(Unit unit)
    {
        MoveUnit(unit, Vector3.zero);
    }

    public void MoveUnit(Unit unit, Vector3 vector)
    {
        Vector3 destination = zero;

        if (vector != zero)
        {
            destination = vector;
        }
        else
        {
            RaycastHit rayHit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, groundLayer))
            {
                destination = rayHit.point;
            }
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, selectableLayer))
            {
                unit.SetTarget(rayHit.transform.root.gameObject);
                return;
            }

            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, obstacleLayer))
            {
                unit.SetTask(rayHit.transform.root.gameObject);
                return;
            }

            if (destination == zero) return;
        }

        MoveUnitToNode(unit, destination, true, 0);
    }

    public Vector3 MoveUnitToNode(Unit unit, Vector3 destination, bool unassigned, float minRadius)
    {
        Node node = grid.NodeFromWorldPoint(destination);
        if (node.isOccupied)
        {
            Node nearestNode = grid.FindNearestAvailableNode(node, minRadius);
            if (nearestNode == null) return destination;

            destination = nearestNode.worldPos;

            if (destination == null) return destination;

            unit.CurrentNode = nearestNode;
        }
        else
        {
            unit.CurrentNode = node;
        }

        if (unassigned) unit.SetState(Unit.State.Unassigned);
        unit.Move(destination);
        return destination;
    }

    public Vector3 MoveUnitToNode(Unit unit, Vector3 destination, bool unassigned)
    {
        return MoveUnitToNode(unit, destination, unassigned, 0);
    }

    public void CreateUnits(GameObject type, int width, int length, Vector3 position, bool isOpponent)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                float currentX = i + position.x;
                float currentZ = type.transform.position.z -  j + position.z;

                Vector3 loc = new Vector3(currentX, type.transform.position.y + position.y, currentZ);
                loc.y += this.terrain.SampleHeight(loc);

                GameObject unitObj = Instantiate(type, loc, type.transform.rotation);

                if (isOpponent) unitObj.tag = "Opponent";
                else unitObj.tag = "Player";

                Unit unit = unitObj.GetComponent<Unit>();
                unit.CurrentNode = this.grid.NodeFromWorldPoint(loc);
                unit.InstantiateUnit(isOpponent);

                //Debug.Log("Spawned unit at " + loc);
            }
        }
    }
}
