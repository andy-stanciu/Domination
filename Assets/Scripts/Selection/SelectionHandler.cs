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

    public void UpdateColor()
    {
        Camera.main.gameObject.GetComponent<SelectionManager>().selectableObjects.Add(this.gameObject);

        outline = character.AddComponent(typeof(Outline)) as Outline;
        outline.precomputeOutline = true;
        outline.OutlineMode = Outline.Mode.OutlineVisible;

        if (this.gameObject.CompareTag("Player")) outline.OutlineColor = Color.blue;
        else outline.OutlineColor = Color.red;

        outline.OutlineWidth = 3f;
        outline.enabled = isSelected;
    }

    public void OnClick()
    {
        outline.enabled = isSelected;
    }
}
