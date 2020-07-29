using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeID : MonoBehaviour
{
    public Sprite tree1Sprite;
    public Sprite tree2Sprite;
    private Sprite myTreeSprite;

    private Color color = Color.clear;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        myTreeSprite = Random.Range(0, 2) == 1? tree1Sprite: tree2Sprite;
    }

    // Update is called once per frame
    void Update()
    {
        VFXUpdate();
    }

    void VFXUpdate()
    {
        //sr.color = color;
        sr.sprite = myTreeSprite;
    }
}
