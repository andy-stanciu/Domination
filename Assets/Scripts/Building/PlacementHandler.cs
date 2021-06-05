using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject barracksPrefab;

    [SerializeField]
    private LayerMask groundLayer;

    private new Camera camera;
    private GameObject ghost;
    private Building ghostBuilding;
    private bool isPlacingBuilding;

    private void Start()
    {
        this.ghost = Instantiate(this.barracksPrefab, Vector3.zero, this.barracksPrefab.transform.rotation);

        //Set ghost albedo to some white color, set alpha to be like 50% transparent

        //Hiding it by default
        this.ghost.SetActive(false);
        this.ghostBuilding = this.ghost.GetComponent<Building>();
        this.ghostBuilding.isGhost = true;
        camera = Camera.main;
    }

    private void Update()
    {
        if (this.isPlacingBuilding)
        {
            Vector3 hoverLocation = CastRay();
            this.ghost.transform.position = hoverLocation;
            this.ghost.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                if (this.ghostBuilding.canPlace)
                {
                    Instantiate(this.barracksPrefab, this.ghost.transform.position, this.ghost.transform.rotation);
                    this.isPlacingBuilding = false;
                    this.ghost.SetActive(false);
                }
            }
        }
    }
    private Vector3 CastRay()
    {
        Vector3 location = Vector3.zero;
        RaycastHit rayHit;

        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, groundLayer))
        {
            location = rayHit.point;
        }

        return location;
    }

    public void OnClick()
    {
        this.isPlacingBuilding = true;
    }
}
