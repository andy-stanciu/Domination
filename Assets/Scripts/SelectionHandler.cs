using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject character;

    private Outline outline;

    [HideInInspector]
    public bool isSelected = false;

    void Start()
    {
        Camera.main.gameObject.GetComponent<Click>().selectableObjects.Add(this.gameObject);

        outline = character.AddComponent(typeof(Outline)) as Outline;
        outline.precomputeOutline = true;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.blue;
        outline.OutlineWidth = 3f;

        outline.enabled = isSelected;
    }

    public void OnClick()
    {
        outline.enabled = isSelected;
    }
}
