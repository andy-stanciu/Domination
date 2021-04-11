﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask selectableObjects;
    public LayerMask obstacles;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node node in grid)
            {
                if (!node.walkable)
                {
                    Gizmos.color = (node.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiameter - 0.1f));
                }
                if (node.isOccupied)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public List<Node> GetNeighboringNodes(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <=1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    public Node FindNearestAvailableNode(Node node)
    {
        int nodeX = node.gridX;
        int nodeY = node.gridY;

        //Make the radius check infinitely, leaving as a large number for now
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Node nearby = grid[nodeX + x, nodeY + y];
                if (!nearby.isOccupied && nearby.walkable) return nearby;

                nearby = grid[nodeX + x, nodeY - y];
                if (!nearby.isOccupied && nearby.walkable) return nearby;

                nearby = grid[nodeX - x, nodeY + y];
                if (!nearby.isOccupied && nearby.walkable) return nearby;

                nearby = grid[nodeX - x, nodeY - y];
                if (!nearby.isOccupied && nearby.walkable) return nearby;
            }
        }
        return null;
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                //Optimize this so that it does not use 2 physics check spheres
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, selectableObjects);
                if (walkable) walkable = !Physics.CheckSphere(worldPoint, nodeRadius, obstacles);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
}