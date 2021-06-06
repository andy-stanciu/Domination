using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [HideInInspector]
    public bool isGuiOpen;

    [SerializeField]
    private GameObject healthBarPrefab;
    [SerializeField]
    private List<Material> ghostMaterials = new List<Material>();
    [SerializeField]
    private int hitpoints;
    [SerializeField]
    private Cost longbowmanCost;

    public List<string> buttonTags = new List<string>();

    private int health;
    private GameObject ui;
    private List<Button> buttons = new List<Button>();
    private Node center;
    private GameObject healthBar;
    private RectTransform healthBarTransform;
    private Transform healthBarChild;

    private UnitHandler unitHandler;
    private NodeGrid grid;
    private SelectionHandler selectionHandler;
    private SelectionManager selectionManager;

    private float buttonPadding = 10;

    public bool isGhost;
    public bool canPlace;

    public void InstantiateBuilding(bool isOpponent)
    {
        this.ui = GameObject.FindGameObjectWithTag("UI");

        GameObject unitHandlerObj = GameObject.FindGameObjectWithTag("UnitHandler");
        this.unitHandler = unitHandlerObj.GetComponent<UnitHandler>();
        this.grid = unitHandlerObj.GetComponent<NodeGrid>();
        this.selectionHandler = this.gameObject.GetComponent<SelectionHandler>();
        this.selectionManager = Camera.main.GetComponent<SelectionManager>();

        UpdateNodes();

        Component[] children = this.ui.GetComponentsInChildren(typeof(Button), true);
        foreach (Component child in children)
        {
            if (buttonTags.Contains(child.gameObject.name)) this.buttons.Add((Button) child);
        }

        if (!isOpponent) this.gameObject.tag = "Player";
        else this.gameObject.tag = "Opponent";

        if (this.selectionHandler != null)
        {
            Color color = Color.red;
            if (!isOpponent) color = Color.blue;

            this.selectionHandler.UpdateColor(color);
        }

        CreateHealthBar(isOpponent);
    }

    private void CreateHealthBar(bool isOpponent)
    {
        this.healthBar = Instantiate(healthBarPrefab, gameObject.transform.position + Vector3.up * 8, healthBarPrefab.transform.rotation);

        //Making it a bit smaller
        this.healthBar.transform.localScale = Vector3.one / 4;

        //Hiding the health bar by default
        this.healthBar.SetActive(false);

        this.healthBarTransform = this.healthBar.GetComponent<RectTransform>();
        this.healthBarTransform.localPosition = gameObject.transform.position + Vector3.up * 8;
        this.healthBarChild = this.healthBar.transform.Find("Bar");
        this.healthBarChild.transform.Find("BarSprite").GetComponent<SpriteRenderer>().color = isOpponent ? Color.red : Color.blue;

        //Initializing health to hitpoints
        this.health = this.hitpoints;
    }

    public void ToggleHealthBar(bool show)
    {
        this.healthBar.SetActive(show);
    }

    public int GetHealth() { return this.health; }
    public float GetHealthPercent() { return ((float)this.health) / this.hitpoints; }
    public void Damage(int health)
    {
        this.health -= health;

        if (this.health <= 0)
        {
            RemoveBuilding();
            return;
        }

        if (this.healthBarChild != null)
        {
            this.healthBarChild.localScale = new Vector3(GetHealthPercent(), 1);
        }
    }

    private void RemoveBuilding()
    {
        //Setting unit to be in obstacle layer
        this.gameObject.layer = 9;

        //De-select the building if it was selected
        if (this.selectionHandler.isSelected) this.selectionManager.RemoveFromSelection(gameObject, selectionHandler);

        Destroy(this.healthBar);
        DestroyImmediate(this.gameObject.GetComponent<BoxCollider>());
        Destroy(this.gameObject);

        //Unoccupying the building's nodes
        UpdateNodes();
    }

    private void UpdateNodes()
    {
        //Hard coded for barracks as of now
        this.grid.UpdateGrid(transform.position + new Vector3(-5, 10, -8), transform.position + new Vector3(5, 10, 8));
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

    public void ExecuteAction(string action, bool isOpponent)
    {
        switch (action)
        {
            case "SpawnLongbowman":
                if (center == null)
                {
                    this.center = this.grid.NodeFromWorldPoint(transform.position);
                }

                if (this.longbowmanCost.CanAfford(isOpponent))
                {
                    this.longbowmanCost.SubtractCost(isOpponent);
                    unitHandler.CreateUnits(unitHandler.longbowman, 1, 1, this.grid.FindNearestAvailableNode(center).worldPos, isOpponent);
                }
                
                break;
        }
    }

    public void hideGui()
    {
        foreach (Button button in this.buttons) button.gameObject.SetActive(false);
        this.isGuiOpen = false;
    }
    /*private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision enter");
        if (isGhost)
        {
            if (this.canPlace)
            {
                this.canPlace = false;

                //Set albedo to be red so that the building can show that it is unable to be placed there
                SetAlbedo(Color.red);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log("Collision exit");
        if (isGhost)
        {
            if (!this.canPlace)
            {
                this.canPlace = true;

                //Set albedo back to normal white
                SetAlbedo(Color.white);
            }
        }
    }*/

    public void SetAlbedo()
    {
        if (!this.isGhost) return;

        Color color;
        if (this.canPlace) color = Color.white;
        else color = Color.red;

        color.a = 0.2f;

        foreach (Material mat in this.ghostMaterials)
        {
            mat.color = color;
        }
    }
}
