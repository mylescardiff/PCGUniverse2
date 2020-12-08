using UnityEngine;
using System.Collections;

public class GUIControls : MonoBehaviour
{
    public new Transform camera;

    Vector3 originalPosition;
    Vector3 editedPosition;

    float moveSpeed;

    void Start()
    {
       originalPosition = camera.position;
    }

    void OnGUI()
    {
        //GUILayout.Label("Camera X");
        //editedPosition.x = GUILayout.HorizontalSlider(editedPosition.x, -0.25f, 0.25f);

        //GUILayout.Label("Camera Y");
        //editedPosition.y = GUILayout.HorizontalSlider(editedPosition.y, -0.5f, 0.5f);

        GUILayout.Label("Camera Zoom");
        editedPosition.z = GUILayout.HorizontalSlider(editedPosition.z, 0, 1);

        GUILayout.Label("Move Speed");
        moveSpeed = GUILayout.HorizontalSlider(moveSpeed, 1, 1.5f);
    }

    void Update()
    {
        camera.position = originalPosition + editedPosition;

        Time.timeScale = moveSpeed;
    }
}
