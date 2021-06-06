using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Cost
{
    private Economy economy;

    [SerializeField]
    private int food;
    [SerializeField]
    private int wood;
    [SerializeField]
    private int gold;

    public Cost(int food, int wood, int gold)
    {
        this.food = food;
        this.wood = wood;
        this.gold = gold;
    }

    public bool CanAfford(bool isOpponent)
    {
        if (this.economy == null) this.economy = GameObject.FindGameObjectWithTag("Economy").GetComponent<Economy>();

        if (economy.getFood(isOpponent) >= this.food && economy.getWood(isOpponent) >= this.wood && economy.getGold(isOpponent) >= this.gold)
        {
            return true;
        }
        else
        {
            if (!isOpponent)
            {
                if (economy.getFood(isOpponent) < this.food) this.economy.foodText.color = Color.red;
                if (economy.getWood(isOpponent) < this.wood) this.economy.woodText.color = Color.red;
                if (economy.getGold(isOpponent) < this.gold) this.economy.goldText.color = Color.red;
            }
            return false;
        }
        
    }

    public void SubtractCost(bool isOpponent)
    {
        economy.setFood(economy.getFood(isOpponent) - this.food, isOpponent);
        economy.setWood(economy.getWood(isOpponent) - this.wood, isOpponent);
        economy.setGold(economy.getGold(isOpponent) - this.gold, isOpponent);
    }
}
