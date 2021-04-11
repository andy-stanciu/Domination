using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitHandler : MonoBehaviour
{
    public Plane plane;

    private Camera camera;
    private Grid grid;
    private SelectionManager selectionManager;
    private Vector3 zero;

    public LayerMask groundLayer;

    public GameObject archer;
    public GameObject longbowman;
    public GameObject villager;

    public GameObject barracks;

    void Awake()
    {
        camera = Camera.main;
        grid = GetComponent<Grid>();
        selectionManager = camera.GetComponent<SelectionManager>();
        zero = Vector3.zero;
    }

    void Start()
    {
        //CreateUnits(longbowman, 10, 10);
        CreateUnits(longbowman, 4, 5, Vector3.zero);
        //CreateUnits(villager, 5, 4);
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
                    MoveUnit(obj, unit);
                }
            }
        }
    }

    private void MoveUnit(GameObject obj, Unit unit)
    {
        Vector3 destination = GetPointUnderCursor();

        if (!destination.Equals(zero))
        {
            Node node = grid.NodeFromWorldPoint(destination);
            if (node.isOccupied)
            {
                Node nearestNode = grid.FindNearestAvailableNode(node);
                if (nearestNode == null) return;

                destination = nearestNode.worldPos;

                if (destination == null) return;

                unit.CurrentNode = nearestNode;
            }
            else
            {
                unit.CurrentNode = node;
            }

            unit.Move(destination);
        }
    }

    private Vector3 GetPointUnderCursor()
    {
        RaycastHit rayHit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out rayHit, Mathf.Infinity, groundLayer))
        {
            return rayHit.point;
        }
        return Vector3.zero;
    }

    public void CreateUnits(GameObject type, int width, int length, Vector3 position)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                Instantiate(type, new Vector3(i + position.x, type.transform.position.y + position.y, type.transform.position.z - j + position.z), type.transform.rotation);
            }
        }
    }
}
