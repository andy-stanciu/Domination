using UnityEngine;
using UnityEditor.AI;

public class TerrainGenerator : MonoBehaviour
{
    public int x = 500;
    public int y = 20;
    public int z = 500;

    public float scale = 20f;

    private float offsetX;
    private float offsetZ;

    private void Awake()
    {
        this.offsetX = Random.Range(0f, 9999f);
        this.offsetZ = Random.Range(0f, 9999f);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        NavMeshBuilder.BuildNavMesh();
    }

    private TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = this.x + 1;
        terrainData.size = new Vector3(x, y, z);
        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    private float[,] GenerateHeights()
    {
        float[,] heights = new float[x, z];
        
        for (int j = 0; j < x; j++)
        {
            for (int k = 0; k < z; k++)
            {
                heights[j, k] = CalculateHeight(j, k);
            }
        }

        return heights;
    }

    private float CalculateHeight(float x, float z)
    {
        float xCoord = x / this.x * scale + offsetX;
        float zCoord = z / this.z * scale + offsetZ;

        return Mathf.PerlinNoise(xCoord, zCoord);
    }
}
