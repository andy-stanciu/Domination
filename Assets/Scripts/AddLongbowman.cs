using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AddLongbowman : MonoBehaviour
{
    private UnitHandler unitHandler;

    public GameObject longbowman;
    // Start is called before the first frame update
    void Awake()
    {
        unitHandler = GameObject.FindGameObjectWithTag("UnitHandler").GetComponent<UnitHandler>();
    }
    public void SpawnLongbowman()
    {
        unitHandler.CreateUnits(longbowman, 1, 1);
    }

    // Update is called once per frame
}
