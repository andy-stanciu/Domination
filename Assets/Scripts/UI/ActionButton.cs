using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    [HideInInspector]
    public Building building;

    [HideInInspector]
    public string action;

    public void OnClick()
    {
        if (building != null)
        {
            building.ExecuteAction(action, false);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}
