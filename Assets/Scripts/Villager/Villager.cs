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

    private bool foundResource;
    private double nextCollect;
    private double cooldownTime;
    private double lastingTime;
    private string material;
    private double materialPerTime;

    private Animator animator;
    private Economy economyScript;
    //private MapGeneration mapScript;

    private Resource currentResource;


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
            Debug.Log("Working");
            //Debug.Log("Working");
            animator.SetBool("isWorking", true);
            //double startingTime = Time.time;
            //nextCollect = Time.time + cooldownTime;

            //Debug.Log(this.lastingTime + startingTime);
            //Debug.Log(startingTime);
            //Debug.Log(nextCollect);
            StartCoroutine("CollectResource");
            /*
            while (Time.time < this.lastingTime + startingTime)
            {
                Debug.Log("Collecting");
                if (Time.time >= nextCollect)
                {
                    nextCollect = Time.time + cooldownTime;
                    this.economyScript.changeMaterial(material, materialPerTime);
                }
            }
         
            animator.SetBool("isWorking", false);
            this.foundResource = false;
            this.mapScript.destroyResource(currentResource);
            this.currentResource = null;
            this.startWorking = false;
            */
        }
    }

    public void Work(Resource resource)
    {
        Debug.Log("Start Working");
        this.currentResource = resource;
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
                this.economyScript.changeMaterial(material, materialPerTime);
            }
            yield return null;
        }

        animator.SetBool("isWorking", false);
        this.foundResource = false;
        if (this.currentResource != null) Destroy(this.currentResource.gameObject);
        //Probably should also remove it from resource list but for now does not matter
        this.currentResource = null;
        this.startWorking = false;

        yield return null;
    }
}
