using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject barracks;

    void Start()
    {
        CreateBuilding(barracks, Vector3.zero);
        //CreateUnits(barracks, new Vector3(0,0,0));
    }

    private void CreateBuilding(GameObject type, Vector3 position)
    {
        Instantiate(type, position,type.transform.rotation);
    }
}
