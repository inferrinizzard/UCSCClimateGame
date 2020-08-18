using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireID : MonoBehaviour
{
    public Sprite fireGreenSprite;
    public Sprite fireTree1Sprite;
    public Sprite fireTree2Sprite;
    private Sprite myFireSprite;

    private Color color = Color.white;
    private SpriteRenderer sr;

    private bool growing;
    
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        //VFXUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (!growing)
        {
            growing = true;
            StartCoroutine(WaitForFire(4f));
            
        }
            
        //FireGrowth();
        VFXUpdate();
    }

    void VFXUpdate()
    {
        sr.color = color;
        if (GetComponent<IdentityManager>().GetFireVariance() == 0)
        {
            sr.sprite = fireGreenSprite;
        }
        else
        {
            sr.sprite = GetComponent<TreeID>().treeVariance == 1 ? fireTree1Sprite : fireTree2Sprite;
        }
       
    }

    void FireGrowth()
    {
        // if I am fire, I ignite my non-fire neighbors coroutine
        GameObject[] myNeighbors = PopulateWorld.Instance.GetNeighbors(gameObject);

        foreach (var neighbor in myNeighbors)
        {
            if (neighbor != null)
            {
                IdentityManager.Identity neighborID = neighbor.GetComponent<IdentityManager>().id;
                IdentityManager.Moisture neighborMoisture = neighbor.GetComponent<IdentityManager>().moisture;
                if (neighborID == IdentityManager.Identity.Green && neighborMoisture != IdentityManager.Moisture.Moist)  // if it is not already fire, or is water
                    PopulateWorld.Instance.MutateCell(neighbor, IdentityManager.Identity.Fire);
            }
            
        }


    }
    IEnumerator WaitForFire(float seconds)
    {
        
        yield return new WaitForSeconds(seconds);
        FireGrowth();
        growing = false;
    }
    
    
}
