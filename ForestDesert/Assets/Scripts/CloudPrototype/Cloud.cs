using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float IdealTemp = 73f;
    public float size = .7f;
    public float idealGrowRate = 1f;
    public Sprite Rain;
    public Sprite Cloud1;
    public Sprite Cloud2;
    public float shrinkRate = .25f;
    public Cloud prefab;
    public bool Suprised = false;

    protected SpriteRenderer sr;
    protected BoxCollider2D bc;
    protected bool bFollowing;
    protected float TimeAlive = 0f;
    protected float TimeSuprised = 0f;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        bFollowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Suprised)
        {
            sr.sprite = Cloud2;
            TimeSuprised += Time.deltaTime;

            if(TimeSuprised > 1f)
            {
                Suprised = false;
                sr.sprite = Cloud1;
            }
        }
        TimeAlive += Time.deltaTime;

        Collider2D[] overlap = Physics2D.OverlapAreaAll(bc.bounds.min, bc.bounds.max);

        float avgTemp = 0f;
        int numWater = 0;
        for(int i = 0; i < overlap.Length; ++i)
        {
            if (overlap[i].tag == "Water")
            {
                Water obj = overlap[i].GetComponent<Water>();
                if (obj)
                {
                    avgTemp += obj.Temperature;
                    numWater++;

                    if (TimeAlive > 12f)
                        obj.Deviate(-2f * Time.deltaTime * Mathf.Clamp(size / 2.3f, 0f, 1f));
                }
            }
        }

        if(numWater > 0)
            avgTemp /= numWater;
        CloudCalculations(avgTemp);

        RainCheck();
    }

    protected virtual void RainCheck()
    {
        if (TimeAlive > 12f)
        {
            sr.sprite = Rain;
            size -= shrinkRate * Time.deltaTime;
            transform.Rotate(new Vector3(0f, 0f, 12f * Time.deltaTime));
        }

        if (size < .25f)
        {
            Destroy(this.gameObject);
        }

        transform.localScale = new Vector3(size, size, size);
    }

    protected virtual void CloudCalculations(float temp)
    {
        float diff = temp - IdealTemp;

        float effRatio = (Mathf.Clamp(diff, -10f, 10f) + 10f)/200f;
        size += idealGrowRate * effRatio * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Cloud")
        {
            Cloud otherCloud = other.GetComponent<Cloud>();
            //float avgLife = (otherCloud.TimeAlive + TimeAlive) / 2f;
            float combinedSize = Mathf.Sqrt(otherCloud.size*otherCloud.size + size*size);

            if(TimeAlive > otherCloud.TimeAlive)
            {
                Suprised = true;
                //transform.position = Vector3.Lerp(transform.position, other.transform.position, 0.5f);
                if (size < otherCloud.size)
                    transform.position = otherCloud.transform.position;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.velocity = (rb.velocity*(size/combinedSize) + other.GetComponent<Rigidbody2D>().velocity*(otherCloud.size/combinedSize)) / 2f;
                size = combinedSize;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
