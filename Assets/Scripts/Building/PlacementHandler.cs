using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlacementHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject barracksPrefab;
    [SerializeField]
    private GameObject barracksGhost;
    [SerializeField]
    private Cost barracksCost;

    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask obstaclesLayer;
    [SerializeField]
    private LayerMask selectableObjects;

    private new Camera camera;
    private GameObject ghost;
    private Building ghostBuilding;
    private bool isPlacingBuilding;

    private void Start()
    {
        this.ghost = Instantiate(this.barracksGhost, Vector3.zero, this.barracksGhost.transform.rotation);

        //Hiding it by default
        this.ghost.SetActive(false);

        this.ghostBuilding = this.ghost.GetComponent<Building>();
        this.ghostBuilding.isGhost = true;
        this.camera = Camera.main;
    }

    private void Update()
    {
        if (this.isPlacingBuilding)
        {
            bool canPlace = !Physics.CheckBox(this.ghost.transform.position, new Vector3(3.5f, 3, 7), Quaternion.identity, obstaclesLayer) && !Physics.CheckBox(this.ghost.transform.position, new Vector3(3.5f, 3, 7), Quaternion.identity, selectableObjects);
            this.ghostBuilding.canPlace = canPlace;
            this.ghostBuilding.SetAlbedo();

            Vector3 hoverLocation = CastRay();
            if (hoverLocation != Vector3.zero) this.ghost.transform.position = hoverLocation;
            this.ghost.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                if (this.ghostBuilding.canPlace)
                {
                    GameObject spawn = Instantiate(this.barracksPrefab, this.ghost.transform.position, this.ghost.transform.rotation);
                    spawn.GetComponent<Building>().InstantiateBuilding(false);

                    this.isPlacingBuilding = false;
                    this.ghost.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.isPlacingBuilding = false;
                this.ghost.SetActive(false);
            }
        }
    }
    private Vector3 CastRay()
    {
        Vector3 location = Vector3.zero;
        RaycastHit rayHit;

        Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, groundLayer))
        {
            location = rayHit.point;
        }

        return location;
    }

    public void OnClick()
    {
        if (this.barracksCost.CanAfford(false))
        {
            this.barracksCost.SubtractCost(false);
            this.isPlacingBuilding = true;
        }
    }
}
