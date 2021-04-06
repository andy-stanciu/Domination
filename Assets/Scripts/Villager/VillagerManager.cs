using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    public bool isWorking;
    public bool isStopped;

    private Animator animator;
    private Economy script;

    void Awake()
    {
        //Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        animator = GetComponent<Animator>();
        this.script = GameObject.FindGameObjectWithTag("Economy").GetComponent<Economy>();

        /*
        selectedObject = GameObject.Find("selectedObject")
        material = selectedObject.getMaterial()
        cooldownTime = selectedObject.getCoolDownTime()
        materialPerTime = selectedObject.getMaterialPerTime()
        */
        double nextCollect = 0;
        double cooldownTime = 1;
        double lastingTime = 100;
        string material = "wood";
        double materialPerTime = 1;

        if (animator.GetBool("isWorking") && animator.GetBool("isStopped"))
        {
            double startingTime = Time.time;
            nextCollect = Time.time + cooldownTime;
            while (Time.time < lastingTime + startingTime)
            {
                if (Time.time >= nextCollect)
                {
                    nextCollect = Time.time + cooldownTime;
                    this.script.changeMaterial(material, materialPerTime);
                }
            }
        }
    }
}
