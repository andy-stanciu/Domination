using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    [SerializeField]
    private LayerMask obstaclesLayer;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private GameObject barracksPrefab;
    [SerializeField]
    private Terrain terrain;

    private Economy economy;
    private List<Villager> villagers = new List<Villager>();
    private List<Unit> units = new List<Unit>();

    private int villagersOnFood;
    private int villagersOnWood;
    private int villagersOnGold;

    private GameObject barracks;
    private Building barracksBuilding;
    private bool hasBarracks;

    private DateTime updateTime = DateTime.MinValue;
    private DateTime shortUpdateTime = DateTime.MinValue;

    void Start()
    {
        this.economy = GameObject.FindGameObjectWithTag("Economy").GetComponent<Economy>();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Opponent"))
        {
            Villager vil = obj.GetComponent<Villager>();
            if (vil != null) this.villagers.Add(vil);
        }
    }

    void Update()
    {
        LongIntervalExecute();
        ShortIntervalExecute();
    }

    private void LongIntervalExecute()
    {
        DateTime now = DateTime.Now;
        if (now < this.updateTime)
        {
            return;
        }
        this.updateTime = now.AddSeconds(8);

        TaskVillagers();
        CreateBarracks();
    }

    private void ShortIntervalExecute()
    {
        DateTime now = DateTime.Now;
        if (now < this.shortUpdateTime)
        {
            return;
        }
        this.shortUpdateTime = now.AddSeconds(1);

        SpawnUnits();
    }

    private void SpawnUnits()
    {
        if (this.barracks != null)
        {
            if (this.barracksBuilding == null) this.barracksBuilding = this.barracks.GetComponent<Building>();
            this.barracksBuilding.ExecuteAction("SpawnLongbowman", true);
        }
    }

    private void CreateBarracks()
    {
        if (this.barracks == null) this.hasBarracks = false;

        if (!hasBarracks)
        {
            if (economy.getWood(true) >= 200)
            {
                economy.setWood(economy.getWood(true) - 200, true);
                PlaceBarracks();
            }
        }
    }

    private void PlaceBarracks()
    {
        this.barracks = Instantiate(this.barracksPrefab, new Vector3(-180, this.terrain.SampleHeight(new Vector3(-180, 0, 180)), 180), this.barracksPrefab.transform.rotation);
        this.barracks.GetComponent<Building>().InstantiateBuilding(true);
        this.hasBarracks = true;
    }

    private void TaskVillagers()
    {
        this.villagers.RemoveAll(vil => vil == null);
        this.villagersOnFood = 0;
        this.villagersOnWood = 0;
        this.villagersOnGold = 0;

        foreach (Villager vil in this.villagers)
        {
            Resource current = vil.currentResource;
            if (current != null)
            {
                string material = current.GetComponent<Resource>().material;
                ChangeVillagerBalance(material, 1);
            }
        }

        foreach (Villager vil in this.villagers)
        {
            if ((!vil.isWorking && !vil.foundResource) || !HasVillagerBalance())
            {
                string preference = GetResourcePreference();
                string previousResource = TaskToNearbyResource(vil, preference);
                if (previousResource != null) ChangeVillagerBalance(previousResource, -1);
            }
        }
    }

    private void ChangeVillagerBalance(string material, int change)
    {
        if (material == "food") this.villagersOnFood += change;
        else if (material == "wood") this.villagersOnWood += change;
        else this.villagersOnGold += change;
    }

    private string GetResourcePreference()
    {
        string preference = "food";

        int min = Math.Min(this.villagersOnWood, this.villagersOnGold);

        if (min < this.villagersOnFood)
        {
            preference = (this.villagersOnWood == min) ? "wood" : "gold";
        }

        return preference;
    }

    private bool HasVillagerBalance()
    {
        return Math.Max(Math.Abs(this.villagersOnFood - this.villagersOnWood), Math.Abs(this.villagersOnFood - this.villagersOnGold)) <= 2;
    }

    /// <summary>
    /// Tasks villager to nearby resource with a preference
    /// </summary>
    /// <param name="vil"></param>
    /// <param name="preference"></param>
    /// <returns>Previous resource the villager was working on</returns>
    private string TaskToNearbyResource(Villager vil, String preference)
    {
        string ret = null;

        Resource currentResource = vil.currentResource;
        if (currentResource != null) ret = currentResource.material;

        RaycastHit[] nearbyResources = Physics.SphereCastAll(vil.transform.position, 30, vil.transform.forward, 30, this.obstaclesLayer);
        if (nearbyResources.Length == 0) return ret;

        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (RaycastHit hit in nearbyResources)
        {
            GameObject obj = hit.transform.gameObject;
            if (obj.GetInstanceID() == this.gameObject.GetInstanceID()) continue;

            //Should not consider objects that are not resources as there are some obstacles that are not harvestable
            //Finds preferred resource only
            Resource resource = obj.GetComponent<Resource>();
            if (resource == null) continue;
            if (resource.material != preference) continue;

            float distance = hit.distance;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }

        if (nearest != null)
        {
            vil.gameObject.GetComponent<Unit>().SetTask(nearest);
        }

        return ret;
    }
}
