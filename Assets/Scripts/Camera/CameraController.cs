using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 16f;
    public float panBorderThickness = 32f;

    public float minFov = 15f;
    public float maxFov = 90f;
    public float zoomSensitivity = 40f;

    private new Camera camera;

    private void Awake()
    {
        this.camera = Camera.main;
    }

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

        float fov = camera.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        camera.fieldOfView = fov;
    }
}
