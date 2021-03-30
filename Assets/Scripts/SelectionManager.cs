using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private LayerMask clickablesLayer;

    [HideInInspector]
    public List<GameObject> selectedObjects;

    [HideInInspector]
    public List<GameObject> selectableObjects;

    private Vector3 mousePos1;
    private Vector3 mousePos2;

    private Camera camera;

    void Awake()
    {
        selectedObjects = new List<GameObject>();
        selectableObjects = new List<GameObject>();

        camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos1 = camera.ScreenToViewportPoint(Input.mousePosition);

            RaycastHit rayHit;

            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, clickablesLayer))
            {
                SelectionHandler clicked = rayHit.collider.GetComponent<SelectionHandler>();

                if (Input.GetKey("left shift"))
                {
                    if (!clicked.isSelected)
                    {
                        AddToSelection(rayHit.collider.gameObject, clicked);
                    }
                    else
                    {
                        RemoveFromSelection(rayHit.collider.gameObject, clicked);
                    }
                }
                else
                {
                    ClearSelection();
                    AddToSelection(rayHit.collider.gameObject, clicked);
                }
            }
            else
            {
                if (!Input.GetKey("left shift"))
                {
                    ClearSelection();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            mousePos2 = camera.ScreenToViewportPoint(Input.mousePosition);

            if (mousePos1 != mousePos2)
            {
                SelectObjects();
            }
        }
    }

    private void SelectObjects()
    {
        List<GameObject> toRemove = new List<GameObject>();

        if (!Input.GetKey("left shift"))
        {
            ClearSelection();
        }

        Rect selectionBox = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);

        foreach (GameObject obj in selectableObjects)
        {
            if (obj != null)
            {
                if (selectionBox.Contains(camera.WorldToViewportPoint(obj.transform.position), true))
                {
                    AddToSelection(obj, obj.GetComponent<SelectionHandler>());
                }
            }
            else
            {
                toRemove.Add(obj);
            }
        }

        if (toRemove.Count > 0)
        {
            foreach (GameObject obj in toRemove)
            {
                RemoveFromSelection(obj, null);
            }

            toRemove.Clear();
        }
    }

    private void ClearSelection()
    {
        if (selectedObjects.Count > 0)
        {
            foreach (GameObject obj in selectedObjects)
            {
                if (obj != null)
                {
                    obj.GetComponent<SelectionHandler>().isSelected = false;
                    obj.GetComponent<SelectionHandler>().OnClick();
                }
            }

            selectedObjects.Clear();
        }
    }

    private void AddToSelection(GameObject obj, SelectionHandler clicked)
    {
        selectedObjects.Add(obj);
        clicked.isSelected = true;
        clicked.OnClick();
    }

    private void RemoveFromSelection(GameObject obj, SelectionHandler clicked)
    {
        selectedObjects.Remove(obj);

        if (clicked != null)
        {
            clicked.isSelected = false;
            clicked.OnClick();
        }
    }
}
