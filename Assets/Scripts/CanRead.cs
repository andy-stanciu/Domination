using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CanRead : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    // Update is called once per frame
}
