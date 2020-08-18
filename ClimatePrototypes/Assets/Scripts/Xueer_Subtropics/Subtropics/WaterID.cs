using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterID : MonoBehaviour
{
    private Color color = Color.clear;  // color to debug cell id
    private SpriteRenderer sr;

    public Sprite waterSprite;
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
        //sr.sprite = waterSprite;
    }
}
