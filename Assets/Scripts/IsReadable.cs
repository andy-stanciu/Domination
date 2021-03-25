using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IsReadable : MonoBehaviour
{
    [SerializeField]
    public GameObject barracksUI;

    [SerializeField]
    private GameObject barracks;

    //private bool isShowing;

    private Click click;
    private Camera camera;

    void Awake()
    {
        camera = Camera.main;
        click = camera.GetComponent<Click>();
        barracksUI.SetActive(false);
    }
    void Update()
    {
        if(click.selectedObjects.Contains(barracks))
        {
            //isShowing = !isShowing;
            barracksUI.SetActive(true);
        }
        else
        {
            barracksUI.SetActive(false);
        }
    }
}
