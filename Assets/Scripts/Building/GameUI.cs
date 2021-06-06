using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private GameObject ui;
    public List<Button> buttons = new List<Button>();
    private float buttonPadding = 10;

    private void Awake()
    {
        this.ui = GameObject.FindGameObjectWithTag("UI");
        for (int i = 0; i < this.buttons.Count; i++)
        {
            Button button = this.buttons[i];

            RectTransform rectTransform = button.gameObject.GetComponent<RectTransform>();

            float buttonSizeX = rectTransform.rect.width * rectTransform.localScale.x;
            float buttonSizeY = rectTransform.rect.height * rectTransform.localScale.y;
            rectTransform.anchoredPosition = new Vector2(-1*(Screen.width / 2 - buttonSizeX / 2 - (i * buttonSizeX + buttonPadding)), -1 * (Screen.height / 2 - buttonSizeY / 2 - buttonPadding));
            /*
            ActionButton actionButton = button.gameObject.GetComponent<ActionButton>();
            actionButton.building = this;
            actionButton.action = button.gameObject.name;
            */
            button.gameObject.SetActive(true);
        }
    }
}
