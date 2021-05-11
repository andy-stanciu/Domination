using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableResource : MonoBehaviour
{
    [HideInInspector]
    private int resourceID;
    private MapGeneration mapGeneration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getID()
    {
        return this.resourceID;
    }

    public void setID(int id)
    {
        this.resourceID = id;
    }

    /*public Resource getResource()
    {
        this.mapGeneration = GameObject.FindGameObjectWithTag("Terrain").GetComponent<MapGeneration>();
        List<Resource> resourceList = this.mapGeneration.getResourceList();

        foreach (Resource resource in resourceList)
        {
            if (resource.getID() == this.resourceID)
            {
                return resource;
            }
        }

        return null;
    }*/
}
