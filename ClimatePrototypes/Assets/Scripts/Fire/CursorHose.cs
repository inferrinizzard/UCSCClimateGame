using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHose : MonoBehaviour
{
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
        Cursor.visible = false;
    }
    void Update()
    {
        Vector2 cursorPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x-200f, Input.mousePosition.y, cam.nearClipPlane));
        transform.position = cursorPos;
    }
}
