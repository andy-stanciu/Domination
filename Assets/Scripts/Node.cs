using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapObject<Node>
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;

    public int GCost;
    public int HCost;
    public Node parent;

    public bool isOccupied;

    private int heapIndex;

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int FCost { get { return GCost + HCost; } }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node node)
    {
        int compare = FCost.CompareTo(node.FCost);

        //Using h cost as a tiebreaker if the f costs are equal
        if (compare == 0)
        {
            compare = HCost.CompareTo(node.HCost);
        }

        //Prefering traveling to a node that has a lower cost rather than a higher cost
        return -compare;
    }
}
