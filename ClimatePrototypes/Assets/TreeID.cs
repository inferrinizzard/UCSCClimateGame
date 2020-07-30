using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeID : MonoBehaviour
{
    [Header("References")]
    public Sprite tree1Sprite;
    public Sprite tree2Sprite;
    public Sprite tree1BurntSprite;
    public Sprite tree2BurntSprite;

    public bool burnt;
    
    public int treeVariance;
    
    private Sprite myTreeSprite;
    private Sprite myTreeBurntSprite;
    private Color color = Color.clear;
    private SpriteRenderer sr;
    
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        
        // determine tree variance for this cell
        int seed = Random.Range(0, 2);
        myTreeSprite =  seed == 1? tree1Sprite: tree2Sprite;
        myTreeBurntSprite =  seed == 1? tree1BurntSprite: tree2BurntSprite;
        treeVariance = seed == 1 ? 1 : 2;
    }

    // Update is called once per frame
    void Update()
    {
        VFXUpdate();
    }

    void VFXUpdate()
    {
        //sr.color = color;
        sr.sprite = burnt? myTreeBurntSprite : myTreeSprite;
    }
}
