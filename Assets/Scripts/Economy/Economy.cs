using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
    private double wood;
    private double food;
    private double gold;

    private double woodOpponent;
    private double foodOpponent;
    private double goldOpponent;

    private GameObject economyUI;
    public Text foodText;
    public Text woodText;
    public Text goldText;

    // Start is called before the first frame update
    void Start()
    {
        this.wood = 0.0;
        this.food = 0.0;
        this.gold = 0.0;
        this.woodOpponent = 0.0;
        this.foodOpponent = 0.0;
        this.goldOpponent = 0.0;

        this.economyUI = GameObject.FindGameObjectWithTag("EconomyUI");

        this.foodText = this.economyUI.transform.Find("FoodCount").GetComponent<Text>();
        this.woodText = this.economyUI.transform.Find("WoodCount").GetComponent<Text>();
        this.goldText = this.economyUI.transform.Find("GoldCount").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        this.foodText.text = "" + Convert.ToInt32(this.food) /*+ " : " + Convert.ToInt32(this.foodOpponent)*/;
        this.woodText.text = "" + Convert.ToInt32(this.wood) /*+ " : " + Convert.ToInt32(this.woodOpponent)*/;
        this.goldText.text = "" + Convert.ToInt32(this.gold) /*+ " : " + Convert.ToInt32(this.goldOpponent)*/;

        if (this.foodText.color == Color.red || this.woodText.color == Color.red || this.goldText.color == Color.red)
        {
            StartCoroutine("ClearResourceFlash");
        }
    }

    private IEnumerator ClearResourceFlash()
    {
        yield return new WaitForSeconds(0.5f);

        this.foodText.color = Color.white;
        this.woodText.color = Color.white;
        this.goldText.color = Color.white;

        StopCoroutine("ClearResourceFlash");
    }

    public void ChangeMaterial(string material, double number, bool isOpponent)
    {
        if (material == "wood")
        {
            if (!isOpponent) this.wood += number;
            else this.woodOpponent += number;
        }
        else if (material == "food")
        {
            if (!isOpponent) this.food += number;
            else this.foodOpponent += number;
        }
        else if (material == "gold")
        {
            if (!isOpponent) this.gold += number;
            else this.goldOpponent += number;
        }
        else
        {
            //Debug.Log("All");
            if (!isOpponent)
            {
                this.wood += number;
                this.food += number;
                this.gold += number;
            }
            else
            {
                this.woodOpponent += number;
                this.foodOpponent += number;
                this.goldOpponent += number;
            }
            
        }
    }

    public void setWood(int number, bool isOpponent)
    {
        if (!isOpponent) this.wood = number;
        else this.woodOpponent = number;
    }

    public void setFood(int number, bool isOpponent)
    {
        if (!isOpponent) this.food = number;
        else this.foodOpponent = number;
    }

    public void setGold(int number, bool isOpponent)
    {
        if (!isOpponent) this.gold = number;
        else this.goldOpponent = number;
    }

    public int getWood(bool isOpponent)
    {
        return !isOpponent ? Convert.ToInt32(this.wood) : Convert.ToInt32(this.woodOpponent);
    }

    public int getFood(bool isOpponent)
    {
        return !isOpponent ? Convert.ToInt32(this.food) : Convert.ToInt32(this.foodOpponent);
    }

    public int getGold(bool isOpponent)
    {
        return !isOpponent ? Convert.ToInt32(this.gold) : Convert.ToInt32(this.goldOpponent);
    }
}
