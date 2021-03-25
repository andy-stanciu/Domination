using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject barracks;

    void Start()
    {
        CreateUnits(barracks, new Vector3(0,0,0));
    }
    private void CreateUnits(GameObject type, Vector3 position)
    {
        Instantiate(type, position,type.transform.rotation);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
