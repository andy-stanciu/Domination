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
    private VillagerManager script;

    public LayerMask groundLayer;
    public LayerMask selectableLayer;

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
    }

    void Start()
    {
        //CreateUnits(longbowman, 10, 10, zero);
        //CreateUnits(longbowman, 4, 5, zero);
        CreateUnits(villager, 5, 4, zero);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RightClickNode();
        }
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
                    MoveUnit(unit, zero, null);
                }
            }
        }
    }

    public void MoveUnit(Unit unit, Vector3 vector, GameObject resource)
    {
        Vector3 destination = zero;

        if (vector != zero)
        {
            destination = vector;
        }
        else
        {
            bool hasTask = false;

            RaycastHit rayHit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, groundLayer))
            {
                destination = rayHit.point;
            }
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, selectableLayer)) hasTask = true;

            if (destination == zero) return;

            if (hasTask)
            {
                unit.SetTask(rayHit.transform.root.gameObject);
                return;
            }
        }

        MoveUnitToNode(unit, destination, true, 0, resource);
    }

    public Vector3 MoveUnitToNode(Unit unit, Vector3 destination, bool unassigned, float minRadius, GameObject resource)
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
        unit.Move(destination, resource);
        return destination;
    }

    public void MoveUnitToNode(Unit unit, Vector3 destination, bool unassigned)
    {
        MoveUnitToNode(unit, destination, unassigned, 0, null);
    }

    public void CreateUnits(GameObject type, int width, int length, Vector3 position)
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

                Unit unit = unitObj.GetComponent<Unit>();
                unit.CurrentNode = this.grid.NodeFromWorldPoint(loc);
                unit.InstantiateUnit();

                //Debug.Log("Spawned unit at " + loc);
            }
        }
    }
}
