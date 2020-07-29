using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenID : MonoBehaviour
{
    public Sprite greenSprite;

    private Color color = Color.clear;
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
        //sr.sprite = greenSprite;
    }
}
