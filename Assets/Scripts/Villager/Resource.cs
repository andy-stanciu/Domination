using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Resource : MonoBehaviour
{
    [SerializeField]
    public float xSize;
    [SerializeField]
    public float zSize;

    [SerializeField]
    private string material;
    [SerializeField]
    private double coolDownTime;
    [SerializeField]
    private double materialPerTime;
    [SerializeField]
    private double lastingTime;
    [SerializeField]
    private bool interactable;
    
    //private int id;

    /*public Resource(string material, double coolDownTime, double materialPerTime, double lastingTime, float xSize, float zSize, bool interactable)
    {
        this.material = material;
        this.coolDownTime = coolDownTime;
        this.materialPerTime = materialPerTime;
        this.lastingTime = lastingTime;
        this.xSize = xSize;
        this.zSize = zSize;
        this.interactable = interactable;
    }*/

    /*public void setID(int id)
    {
        this.id = id;
    }

    public int getID()
    {
        return this.id;
    }*/

    public String getMaterial()
    {
        return this.material;
    }

    public double getCoolDownTime()
    {
        return this.coolDownTime;
    }

    public double getMaterialPerTime()
    {
        return this.materialPerTime;
    }

    public void changeLastingTime(double time)
    {
        this.lastingTime -= time;
    }

    public double getLastingTime()
    {
        return this.lastingTime;
    }

    public bool isOverlapping(float x, float z)
    {
        if (x > this.gameObject.transform.position.x - this.xSize/2 && x < this.gameObject.transform.position.x + this.xSize/2)
        {
            if (z > this.gameObject.transform.position.z - this.zSize/2 && z < this.gameObject.transform.position.z + this.zSize/2)
            {
                return true;
            }
        }
        return false;
    }

    public bool isInteractable()
    {
        return this.interactable;
    }
}

//Changing this to parameter-based instead of classes (significantly cleaner in Unity)
/*public class Sheep: Resource
{
    public Sheep() : base("food", 1, 2, 10, 3, 3, true) {}
}

public class Woodmill: Resource
{
    public Woodmill() : base("wood", 1, 1, Int64.MaxValue, 10, 10, true) {}
}

public class Plantation: Resource
{
    public Plantation() : base("gold", 1, 0.5, Int64.MaxValue, 20, 10, true) {}
}

public class Mine: Resource
{
    public Mine() : base("gold", 1, 0.6, Int64.MaxValue, 10, 10, true) {}
}

public class GreenTree: Resource
{
    public GreenTree() : base("wood", 1, 0.5, 15, 8, 8, true) {}
}

public class GoldPaddy: Resource
{
    public GoldPaddy() : base("gold", 1, 0.34, 30, 2, 2, true) {}
}

public class FoodPaddy: Resource
{
    public FoodPaddy() : base("food", 1, 0.34, 30, 2, 2, true) {}
}

public class Fish: Resource
{
    public Fish() : base("food", 1, 1, 5, 1, 1, true) {}
}

public class Farm: Resource
{
    public Farm() : base("food", 1, 0.5, Int64.MaxValue, 20, 20, true) {}
}

public class Factory: Resource
{
    public Factory() : base("food", 1, 0.5, Int64.MaxValue, 20, 10, true) {}
}

public class BerryBush: Resource
{
    public BerryBush() : base("food", 1, 0.67, 223.880597, 2, 2, true) {}
}

public class Village: Resource
{
    public Village() : base(null, 0, 0, 0, 20, 10, false) {}
}

public class BigRock: Resource
{
    public BigRock() : base(null, 0, 0, 0, 5, 5, false) {}
}

public class SmallRock: Resource
{
    public SmallRock() : base(null, 0, 0, 0, 5, 5, false) {}
}*/