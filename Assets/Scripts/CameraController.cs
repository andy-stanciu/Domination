using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 16f;
    public float panBorderThickness = 32f;

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.mousePosition.y >= Screen.height - panBorderThickness || Input.mousePosition.y <= panBorderThickness || Input.mousePosition.x >= Screen.width - panBorderThickness || Input.mousePosition.x <= panBorderThickness)
        {
            float yDist = Screen.height / 2;
            float xDist = Screen.width / 2;

            pos.z += Mathf.Pow((Input.mousePosition.y - yDist) / yDist, 3) * panSpeed * Time.deltaTime;
            pos.x += Mathf.Pow((Input.mousePosition.x - xDist) / xDist, 3) * panSpeed * Time.deltaTime;
        }

        transform.position = pos;
    }
}
