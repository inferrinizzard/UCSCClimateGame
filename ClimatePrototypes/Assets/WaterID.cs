using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterID : MonoBehaviour
{
    private Color color = Color.blue;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        VFXUpdate();
    }

    void VFXUpdate()
    {
        sr.color = color;
    }
}
