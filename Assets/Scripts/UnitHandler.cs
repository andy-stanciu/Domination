using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitHandler : MonoBehaviour
{
    private Camera camera;
    public Plane plane;
    private Click click;
    private Vector3 zero;

    public LayerMask groundLayer;

    public GameObject archer;
    public GameObject longbowman;

    public GameObject barracks;

    void Awake()
    {
        camera = Camera.main;
        click = camera.GetComponent<Click>();
        zero = Vector3.zero;
    }

    void Start()
    {
        //CreateUnits(longbowman, 5, 4);
    }

    void Update()
    {
        foreach (GameObject obj in click.selectableObjects)
        {
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null)
            {
                if (!unit.isStopped)
                {
                    if (obj.GetComponent<NavMeshAgent>().remainingDistance == 0)
                    {
                        unit.StopMoving();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach(GameObject obj in click.selectedObjects)
            {
                MoveUnit(obj);
            }
        }
    }

    public void CreateUnits(GameObject type, int width, int length)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                Instantiate(type, new Vector3(i, type.transform.position.y, type.transform.position.z - j), type.transform.rotation);
            }
        }
    }

    private void MoveUnit(GameObject obj)
    {
        Unit unit = obj.GetComponent<Unit>();
        NavMeshAgent unitAgent = obj.GetComponent<NavMeshAgent>();
        Vector3 destination = GetPointUnderCursor();

        if (!destination.Equals(zero))
        {
            unitAgent.SetDestination(GetPointUnderCursor());
            unit.StartMoving();
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
}
