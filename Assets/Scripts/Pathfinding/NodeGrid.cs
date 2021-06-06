using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeGrid : MonoBehaviour
{
    public Terrain terrain;
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

    /**
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
    }*/

    public Node FindNearestAvailableNode(Node node)
    {
        return FindNearestAvailableNode(node, 0);
    }

    public Node FindNearestAvailableNode(Node node, float minRadius)
    {
        int nodeX = node.gridX;
        int nodeY = node.gridY;

        float radius = minRadius;
        Node result = null;

        while (result == null)
        {
            for (int x = 0; x <= radius && result == null; x++)
            {
                int y = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - x * x));

                result = GetCardinalNode(nodeX, nodeY, x, y);
            }

            for (int y = 0; y <= radius && result == null; y++)
            {
                int x = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - y * y));

                result = GetCardinalNode(nodeX, nodeY, x, y);
            }

            radius += 0.5f;
        }

        return result;
    }

    public Node GetCardinalNode(int nodeX, int nodeY, int x, int y)
    {
        try
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
        catch (IndexOutOfRangeException e)
        {
            return null;
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
                worldPoint += Vector3.up * (this.terrain.SampleHeight(worldPoint));

                //Optimize this so that it does not use 2 physics check spheres
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, selectableObjects);
                if (walkable) walkable = !Physics.CheckSphere(worldPoint, nodeRadius, obstacles);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public void UpdateGrid(Vector3 point1, Vector3 point2)
    {
        Node node1 = NodeFromWorldPoint(point1);
        Node node2 = NodeFromWorldPoint(point2);

        for (int x = node1.gridX; x <= node2.gridX; x++)
        {
            for (int y = node1.gridY; y <= node2.gridY; y++)
            {
                Node currentNode = this.grid[x, y];

                bool walkable = !Physics.CheckCapsule(currentNode.worldPos, currentNode.worldPos + new Vector3(1, 10, 1) * nodeDiameter, nodeRadius, selectableObjects);
                if (walkable) walkable = !Physics.CheckCapsule(currentNode.worldPos, currentNode.worldPos + new Vector3(1, 10, 1) * nodeDiameter, nodeRadius, obstacles);

                currentNode.walkable = walkable;
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