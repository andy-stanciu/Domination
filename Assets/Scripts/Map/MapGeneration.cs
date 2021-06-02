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
    public GameObject TreasureBarrel;

    //Need to implement prefabs
    public GameObject Mine;
    public GameObject GoldPaddy;
    public GameObject FoodPaddy;
    public GameObject BerryBush;
    public GameObject Pond;


    // Not resource
    public GameObject Village;
    public GameObject BigRock;
    public GameObject SmallRock;
    public GameObject GrassFood;
    public GameObject GrassGold;
    public GameObject PondGrass;

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
        this.xPosBoundary = 230;
        this.xNegBoundary = -230;
        this.zPosBoundary = 230;
        this.zNegBoundary = -230;

        this.resources = new List<Resource>();

        generatePlayerStart();
        generateMap();
    }


    public void generateMap()
    {
        int i = 0;
        while (i < 1500)
        {
            createResource(GreenTree, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
            //createResource(BigRock, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
            //createResource(SmallRock, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
            //createResource(SmallRock, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);

            if (i % 5 == 0)
            {
                createResource(BerryBush, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
            }

            if (i % 10 == 0)
            {
                generateResourceBuildings();
                createResource(TreasureBarrel, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
            }

            if (i % 50 == 0)
            {
                createResource(Pond, this.xNegBoundary, this.xPosBoundary, this.xNegBoundary, this.zPosBoundary);
            }

            i++;
        }
    }

    public void generateResourceBuildings()
    {
       int paddyType = Random.Range(0, 2);

        if (paddyType == 0)
        {
            createResource(GoldPaddy, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
        }
        else
        {
            createResource(FoodPaddy, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
        }

        int spawnMine = Random.Range(0, 3);
        if (spawnMine == 0)
        {
            //createResource(Mine, this.xNegBoundary, this.xPosBoundary, this.zNegBoundary, this.zPosBoundary);
        }
    }

    public void generatePlayerStart()
    {
        this.playerXNeg = Random.Range(xNegBoundary, xPosBoundary - 10);
        this.playerZNeg = Random.Range(zNegBoundary, zPosBoundary - 10);
        this.playerXPos = this.playerXNeg + 10;
        this.playerZPos = this.playerZPos + 10;

        //createResource(Village, this.playerXNeg, this.playerXPos, this.playerZNeg, this.playerZPos);
    }

    public void createResource(GameObject type, float xNegLimit, float xPosLimit, float zNegLimit, float zPosLimit)
    {
        float x = 0;
        float z = 0;
        float xSize = 0;
        float zSize = 0;
        float xCenter = 0;
        float zCenter = 0;
        bool randomizing = true;
        GameObject grassType = null;

        while (randomizing)
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

        if (type == GoldPaddy || type == FoodPaddy)
        {
            if (type == GoldPaddy)
            {
                grassType = GrassGold;
            }
            else
            {
                grassType = GrassFood;
            }

            xCenter = gameObject.transform.position.x;
            zCenter = gameObject.transform.position.z;

            generateGrass(grassType, xCenter, zCenter, 6, 13);
        }
    }
    
    public void generateGrass(GameObject grass, float xCenter, float zCenter, int xSize, int zSize)
    {
        float xStart = xCenter - xSize / 2;
        float zStart = zCenter - zSize / 2;
        for (int i = 0; i < xSize; i++)
        {
            for (int ii = 0; ii < zSize; ii++)
            {
                float y = this.terrain.SampleHeight(new Vector3(xStart + i, 0, zStart + ii));
                Instantiate(grass, new Vector3(xStart + i, grass.transform.position.y + y, zStart + ii), Quaternion.Euler(grass.transform.rotation.x, Random.Range(0, 360), grass.transform.rotation.z));

                /*
                float negative = Mathf.Pow(-1, ii);
                float y = this.terrain.SampleHeight(new Vector3(xCenter + i * negative, 0, zCenter + ii * negative));
                Instantiate(grass, new Vector3(xCenter+i*negative, grass.transform.position.y + y, zCenter+ii*negative), Quaternion.Euler(grass.transform.rotation.x, Random.Range(0, 360), grass.transform.rotation.z));
                */
            }
        }
    }

    public float[] getPlayerStart()
    {
        return new float[] {this.playerXNeg, this.playerXPos, this.playerZNeg, this.playerZPos};
    }
}
