﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gather_Materials : MonoBehaviour
{
    private string material;
    private double cooldownTime;
    private double nextCollect;
    private double materialPerTime;
    private Animator animator;
    public bool isWorking;
    public bool isStopped;
    PlayerScript script;

    void Awake()
    {
      //Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    // Start is called before the first frame update
    void Start()
    {
      this.nextCollect = 0;
      this.cooldownTime = 1;
    }

    // Update is called once per frame
    void Update()
    {
      animator = GetComponent<Animator>();
      this.script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

      /*
      selectedObject = GameObject.Find("selectedObject")
      material = selectedObject.getMaterial()
      cooldownTime = selectedObject.getCoolDownTime()
      materialPerTime = selectedObject.getMaterialPerTime()
      */

      this.material = "wood";
      this.materialPerTime = 1;

      if (animator.GetBool("isWorking") && animator.GetBool("isStopped"))
      {
        if (Time.time >= this.nextCollect)
        {
          this.nextCollect = Time.time + this.cooldownTime;
          this.script.changeMaterial(this.material, this.materialPerTime);
        }
      }
    }
}