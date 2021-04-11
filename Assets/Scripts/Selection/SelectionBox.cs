using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    [SerializeField]
    private RectTransform selectionBoxImage;

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 center;

    void Start()
    {
        selectionBoxImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectionBoxImage.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            endPos = Input.mousePosition;

            center = (startPos + endPos) / 2f;
            selectionBoxImage.position = center;

            selectionBoxImage.sizeDelta = new Vector2(Mathf.Abs(startPos.x - endPos.x), Mathf.Abs(startPos.y - endPos.y));

            if (!selectionBoxImage.gameObject.activeInHierarchy)
            {
                selectionBoxImage.gameObject.SetActive(true);
            }
        }
    }
}
