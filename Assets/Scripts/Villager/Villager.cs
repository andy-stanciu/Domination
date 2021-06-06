using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    [HideInInspector]
    public bool isWorking;
    [HideInInspector]
    public bool startWorking;
    [HideInInspector]
    public bool isStopped;

    public bool foundResource;
    private double nextCollect;
    private double cooldownTime;
    private double lastingTime;
    private string material;
    private double materialPerTime;

    private Animator animator;
    private Economy economyScript;
    //private MapGeneration mapScript;

    public Resource currentResource;
    private bool isOpponent;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        this.economyScript = GameObject.FindGameObjectWithTag("Economy").GetComponent<Economy>();
        //this.mapScript = GameObject.FindGameObjectWithTag("Terrain").GetComponent<MapGeneration>();
        this.animator = GetComponent<Animator>();
    }

    public void SetTeam(bool isOpponent)
    {
        this.isOpponent = isOpponent;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentResource == null) return;

        if (!this.foundResource && this.startWorking)
        {
            this.material = this.currentResource.getMaterial();
            this.cooldownTime = this.currentResource.getCoolDownTime();
            this.materialPerTime = this.currentResource.getMaterialPerTime();
            this.lastingTime = this.currentResource.getLastingTime();
            //Debug.Log(material);
            //Debug.Log(cooldownTime);
            //Debug.Log(materialPerTime);
            //Debug.Log(lastingTime);
    
            this.nextCollect = 0;

            this.foundResource = true;
        }

        if (!animator.GetBool("isWorking") && animator.GetBool("isStopped"))
        {
            //Debug.Log("Working");
            //Debug.Log("Working");
            animator.SetBool("isWorking", true);
            //double startingTime = Time.time;
            //nextCollect = Time.time + cooldownTime;

            //Debug.Log(this.lastingTime + startingTime);
            //Debug.Log(startingTime);
            //Debug.Log(nextCollect);
            StartCoroutine("CollectResource");
        }
    }

    public void Work(Resource resource)
    {
        StopCoroutine("CollectResource");
        ResetResource();

        this.currentResource = resource;
        string resourceName = this.currentResource.getName();
        //Debug.Log(resourceName);

        if (resourceName == "TreasureBarrel" || resourceName == "BerryBush" || resourceName == "Pond")
        {
            animator.SetBool("isCollectingFruit", true);
        }
        else if (resourceName == "GoldPaddy" || resourceName == "FoodPaddy")
        {
            animator.SetBool("isFarming", true);
        }
        else
        {
            animator.SetBool("isCollectingTree", true);
        }
        this.startWorking = true; 
    }
    
    private IEnumerator CollectResource()
    {
        double startingTime = Time.time;
        nextCollect = Time.time + cooldownTime;

        while (Time.time < this.lastingTime + startingTime)
        {
            if (Time.time >= nextCollect)
            {
                nextCollect = Time.time + cooldownTime;
                this.economyScript.ChangeMaterial(material, materialPerTime, isOpponent);
            }
            yield return null;
        }

        ResetResource();

        if (this.currentResource != null) Destroy(this.currentResource.gameObject);
        //Probably should also remove it from resource list but for now does not matter
        this.currentResource = null;
        this.startWorking = false;

        yield return null;
    }

    private void ResetResource()
    {
        animator.SetBool("isWorking", false);
        animator.SetBool("isCollectingTree", false);
        animator.SetBool("isCollectingFruit", false);
        animator.SetBool("isFarming", false);
        this.foundResource = false;
    }
}
