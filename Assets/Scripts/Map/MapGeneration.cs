using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

//BerryBush, Factory, Farm, Fish, FishPaddy, GoldPaddy, Mine, Plantation, Sheep, Tree, Woodmill

public class MapGeneration : MonoBehaviour
{
    public Terrain terrain;

    // Implemented Prefabs
    public GameObject Factory;
    public GameObject GreenTree;

    //Need to implement prefabs
    public GameObject Sheep;
    public GameObject Woodmill;
    public GameObject Plantation;
    public GameObject Mine;
    public GameObject GoldPaddy;
    public GameObject FoodPaddy;
    public GameObject Fish;
    public GameObject Farm;
    public GameObject BerryBush;


    // Not resource
    public GameObject village;
    public GameObject BigRock;
    public GameObject SmallRock;

    private float xPosBoundary;
    private float xNegBoundary;
    private float zPosBoundary;
    private float zNegBoundary;

    private float playerXPos;
    private float playerXNeg;
    private float playerZPos;
    private float playerZNeg;

    private List<Resource> resources;

    //private InteractableResource resourceScript;

    void Awake()
    {
        this.xPosBoundary = 250;
        this.xNegBoundary = -250;
        this.zPosBoundary = 250;
        this.zNegBoundary = -250;

        this.resources = new List<Resource>();

        generatePlayerStart();
        generateNature();
        generateBushes();
        generateResourceBuildings();
        generateAnimals();
    }


    public void generateNature()
    {
        int i = 0;
        while (i < 1500)
        {
            createResource(GreenTree, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary, true);
            //createResource(BigRock, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary, new BigRock(), false);
            //createResource(SmallRock, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary, new SmallRock(), false);
            //createResource(SmallRock, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary, new SmallRock(), false);
            i++;
        }
    }

    public void generateBushes()
    {

    }

    public void generateResourceBuildings()
    {

    }

    public void generateAnimals()
    {

    }

    public void generatePlayerStart()
    {
        this.playerXNeg = Random.Range(xNegBoundary, xPosBoundary - 10);
        this.playerZNeg = Random.Range(zNegBoundary, zPosBoundary - 10);
        this.playerXPos = this.playerXNeg + 10;
        this.playerZPos = this.playerZPos + 10;

        //createResource(village, this.playerXNeg, this.playerXPos, this.playerZNeg, this.playerZPos, new Village(), false);
    }

    public void createResource(GameObject type, float xNegLimit, float xPosLimit, float zNegLimit, float zPosLimit, bool collectable)
    {
        float x = 0;
        float z = 0;
        bool randomizing = true;

        while(randomizing)
        {
            bool overlapped = false;

            x = Random.Range(xNegLimit, xPosLimit);
            z = Random.Range(zNegLimit, zPosLimit);

            foreach (Resource r in this.resources)
            {
                if (r.isOverlapping(x, z))
                {
                    overlapped = true;
                    continue;
                }
            }
            if (overlapped)
            {
                continue;
            }
            randomizing = false;
        }

        float y = this.terrain.SampleHeight(new Vector3(x, 0, z));

        GameObject gameObject = Instantiate(type, new Vector3(x, type.transform.position.y + y, z), Quaternion.Euler(type.transform.rotation.x, Random.Range(0, 360), type.transform.rotation.z));
        this.resources.Add(gameObject.GetComponent<Resource>());

        //resource.setGameObject(gameObject);
        //resource.setID(this.resources.Count);

        //Debug.Log(resource.getMaterial());

        /*if (collectable)
        {
            this.resourceScript = gameObject.GetComponent<InteractableResource>();
            this.resourceScript.setID(this.resources.Count);
        }*/
    }

    /*public void destroyResource(Resource resource)
    {
        int resourceID = resource.getID();

        for (int i = 0; i < this.resources.Count; i++)
        {
            if (this.resources[i].getID() == resourceID)
            {
                Destroy(resource.getGameObject());
                this.resources.RemoveAt(i);
            }
        }
    }

    public List<Resource> getResourceList()
    {
        return this.resources;
    }*/

    public float[] getPlayerStart()
    {
        return new float[] {this.playerXNeg, this.playerXPos, this.playerZNeg, this.playerZPos};
    }
}
