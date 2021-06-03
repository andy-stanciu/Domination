using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [HideInInspector]
    public bool isGuiOpen;

    public List<string> buttonTags = new List<string>();

    private GameObject ui;
    private List<Button> buttons = new List<Button>();
    private Node center;

    private UnitHandler unitHandler;
    private NodeGrid grid;

    private float buttonPadding = 10;

    private GameUI buildingScript;

    public bool isGhost;
    public bool canPlace;

    void Awake()
    {
        this.ui = GameObject.FindGameObjectWithTag("UI");

        GameObject unitHandlerObj = GameObject.FindGameObjectWithTag("UnitHandler");
        this.unitHandler = unitHandlerObj.GetComponent<UnitHandler>();
        this.grid = unitHandlerObj.GetComponent<NodeGrid>();

        foreach (Button child in this.ui.GetComponentsInChildren<Button>())
        {
            if (buttonTags.Contains(child.gameObject.name)) this.buttons.Add(child);
        }

        //Make this check teams once building placement is implemented
        this.gameObject.tag = "Player";
        this.gameObject.GetComponent<SelectionHandler>().UpdateColor();
    }

    public void showGui()
    {
        for (int i = 0; i < this.buttons.Count; i++)
        {
            Button button = this.buttons[i];

            RectTransform rectTransform = button.gameObject.GetComponent<RectTransform>();

            float buttonSizeX = rectTransform.rect.width * rectTransform.localScale.x;
            float buttonSizeY = rectTransform.rect.height * rectTransform.localScale.y;

            rectTransform.anchoredPosition = new Vector2(Screen.width / 2 - buttonSizeX / 2 - (i * buttonSizeX + buttonPadding), -1 * (Screen.height / 2 - buttonSizeY / 2 - buttonPadding));

            ActionButton actionButton = button.gameObject.GetComponent<ActionButton>();
            actionButton.building = this;
            actionButton.action = button.gameObject.name;

            button.gameObject.SetActive(true);
        }
        this.isGuiOpen = true;
    }

    public void ExecuteAction(string action)
    {
        switch (action)
        {
            case "SpawnLongbowman":
                if (center == null)
                {
                    this.center = this.grid.NodeFromWorldPoint(transform.position);
                }
                //When the AI can spawn units, make this set to false or true
                unitHandler.CreateUnits(unitHandler.longbowman, 1, 1, this.grid.FindNearestAvailableNode(center).worldPos, false);
                break;
            /*
            case "SpawnBuilding":
                this.buildingScript = GameObject.FindGameObjectWithTag("Tag").GetComponent<Building>();
                break;
            */
        }
    }

    public void hideGui()
    {
        foreach (Button button in this.buttons) button.gameObject.SetActive(false);
        this.isGuiOpen = false;
    }
    private void OnCollisionEnter(Collider other)
    {
        if (isGhost)
        {
            if (this.canPlace) this.canPlace = false;
            //Set albedo to be red so that the building can show that it is unable to be placed there
        }
    }

    private void OnCollisionExit(Collider other)
    {
        if (isGhost)
        {
            if (!this.canPlace) this.canPlace = true;
            //Set albedo back to normal white
        }
    }
}
