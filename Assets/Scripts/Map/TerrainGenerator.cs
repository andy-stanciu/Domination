using UnityEngine;
using UnityEditor.AI;

public class TerrainGenerator : MonoBehaviour
{
    public int sizeY = 5;

    private int sizeX;
    private int sizeZ;

    public float scale = 20f;

    private float offsetX;
    private float offsetZ;

    private void Awake()
    {
        this.offsetX = Random.Range(0f, 9999f);
        this.offsetZ = Random.Range(0f, 9999f);

        Terrain terrain = GetComponent<Terrain>();

        this.sizeX = this.sizeZ = terrain.terrainData.heightmapResolution;

        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        NavMeshBuilder.BuildNavMesh();
    }

    private TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = this.sizeX;
        terrainData.size = new Vector3(sizeX, sizeY, sizeZ);
        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    private float[,] GenerateHeights()
    {
        float[,] heights = new float[sizeX, sizeZ];
        
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                heights[x, z] = CalculateHeight(x, z);
            }
        }

        return heights;
    }

    private float CalculateHeight(float x, float z)
    {
        float xCoord = x / this.sizeX * scale + offsetX;
        float zCoord = z / this.sizeZ * scale + offsetZ;

        return 0;
        //return Mathf.PerlinNoise(xCoord, zCoord);
    }
}
