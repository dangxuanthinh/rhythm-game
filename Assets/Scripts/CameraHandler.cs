using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public float targetSceneWidth = 4.2f;
    public float laneYPosition = -2.5f;
    private Camera cam;

    private void Awake()
    {
        ResizeCamera();
    }

    // Resize camera so that the 4 lanes always fit entire width the screen regardless of screen resolution
    private void ResizeCamera()
    {
        cam = Camera.main;

        // Resize camera to fit the target scene width
        float aspect = (float)Screen.width / Screen.height;
        cam.orthographicSize = targetSceneWidth / (2f * aspect);

        // Reposition camera so the lanes are always at the bottom half of the screen
        float newY = laneYPosition + cam.orthographicSize / 2f;
        cam.transform.position = new Vector3(cam.transform.position.x, newY, cam.transform.position.z);
    }
}
