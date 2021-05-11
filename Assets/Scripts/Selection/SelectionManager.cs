using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private LayerMask clickablesLayer;

    [SerializeField]
    private LayerMask uiLayer;

    [HideInInspector]
    public List<GameObject> selectedObjects;

    [HideInInspector]
    public List<GameObject> selectableObjects;

    private Vector3 mousePos1;
    private Vector3 mousePos2;

    private new Camera camera;
    private GraphicRaycaster graphicRaycaster;
    //private UnitHandler unitScript;

    void Awake()
    {
        selectedObjects = new List<GameObject>();
        selectableObjects = new List<GameObject>();

        camera = Camera.main;
        this.graphicRaycaster = GameObject.FindGameObjectWithTag("UI").GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;

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
                    /*if (rayHit.collider.gameObject.GetComponent<InteractableResource>() != null)
                    {
                        foreach (GameObject obj in selectedObjects)
                        {
                            if (obj.GetComponent<Villager>() != null)
                            {
                                this.unitScript = GameObject.FindGameObjectWithTag("UnitHandler").GetComponent<UnitHandler>();
                                this.unitScript.MoveUnit(obj.GetComponent<Unit>(), new Vector3(rayHit.collider.gameObject.transform.position.x, rayHit.collider.gameObject.transform.position.y, rayHit.collider.gameObject.transform.position.z), rayHit.collider.gameObject);
                            }
                        }
                        ClearSelection();
                    }
                    else
                    {*/
                        ClearSelection();
                        AddToSelection(rayHit.collider.gameObject, clicked);
                    //}
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
            if (IsPointerOverUI()) return;

            mousePos2 = camera.ScreenToViewportPoint(Input.mousePosition);

            if (mousePos1 != mousePos2)
            {
                SelectObjects();
            }
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointer, raycastResults); 

        return raycastResults.Count > 0;
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
                    if (obj.GetComponent<InteractableResource>() == null)
                    {
                        AddToSelection(obj, obj.GetComponent<SelectionHandler>());
                    }
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
                if (obj == null) continue;

                SelectionHandler clicked = obj.GetComponent<SelectionHandler>();
                if (clicked == null) continue;

                clicked.isSelected = false;
                clicked.OnClick();

                ToggleBuildingGui(obj, false);
                ToggleHealthBar(obj, false);
            }

            selectedObjects.Clear();
        }
    }

    private void AddToSelection(GameObject obj, SelectionHandler clicked)
    { 
        if (clicked == null) return;

        selectedObjects.Add(obj);
        clicked.isSelected = true;
        clicked.OnClick();

        ToggleBuildingGui(obj, true);
        ToggleHealthBar(obj, true);
    }

    public void RemoveFromSelection(GameObject obj, SelectionHandler clicked)
    {
        if (clicked == null) return;

        selectedObjects.Remove(obj);

        clicked.isSelected = false;
        clicked.OnClick();

        ToggleBuildingGui(obj, false);
        ToggleHealthBar(obj, false);
    }

    private void ToggleBuildingGui(GameObject obj, bool show)
    {
        Building building = obj.GetComponent<Building>();
        if (building != null)
        {
            if (show) building.showGui();
            else building.hideGui();
        }
    }

    private void ToggleHealthBar(GameObject obj, bool show)
    {
        Unit unit = obj.GetComponent<Unit>();
        if (unit != null)
        {
            unit.ToggleHealthBar(show);
        }
    }
}
