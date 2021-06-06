using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    public Terrain terrain;

    [SerializeField]
    private GameObject barracks;

    void Awake()
    {
        //CreateBuilding(barracks, new Vector3(20, this.terrain.SampleHeight(new Vector3(20, 0, 20)), 20));
        //CreateUnits(barracks, new Vector3(0,0,0));
    }

    private void CreateBuilding(GameObject type, Vector3 position)
    {
        Instantiate(type, position, type.transform.rotation);
    }
}
