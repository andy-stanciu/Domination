using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Economy : MonoBehaviour
{
    private double wood;
    private double food;
    private double gold;

    // Start is called before the first frame update
    void Start()
    {
      this.wood = 0.0;
      this.food = 0.0;
      this.gold = 0.0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(this.wood);
    }

    public void changeMaterial(string material, double number)
    {
        if (material == "wood")
        {
            this.wood += number;
        }
        else if (material == "food")
        {
            this.food += number;
        }
        else if (material == "gold")
        {
            this.gold += number;
        }
        else
        {
            Debug.Log("All");
            this.wood += number;
            this.food += number;
            this.gold += number;
        }
    }

    public void setWood(int number)
    {
      this.wood = number;
    }

    public void setFood(int number)
    {
      this.food = number;
    }

    public void setGold(int number)
    {
      this.gold = number;
    }

    public int getWood()
    {
      return (int)Math.Floor(this.wood);
    }

    public int getFood()
    {
      return (int)Math.Floor(this.food);
    }

    public int getGold()
    {
      return (int)Math.Floor(this.gold);
    }
}
