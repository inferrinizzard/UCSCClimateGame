using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public float GrowthRate = 8f;
    private bool bGrown = false;
    private float GrowthTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!bGrown)
        {
            GrowthTime = Mathf.Clamp(GrowthTime + Time.deltaTime, 0f, GrowthRate);
            float currScale = transform.localScale.x;

            currScale = Mathf.Lerp(.3f, .6f, GrowthTime / GrowthRate);
            transform.localScale = new Vector3(currScale, currScale, currScale);

            if(GrowthTime >= GrowthRate)
            {
                bGrown = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Cloud")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && bGrown)
        {
            Destroy(this.gameObject);
        }
    }
}
