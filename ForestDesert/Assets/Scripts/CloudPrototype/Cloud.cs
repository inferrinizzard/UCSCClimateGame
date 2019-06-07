using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    // The temperature at which the growth rate is 1. 
    // A temperature higher than this grows the cloud faster, lower grows it slower
    public float IdealTemp = 73f;
    // Current scale of cloud
    public float size = .7f;
    // The rate at which the cloud grows @ IdealTemp
    public float idealGrowRate = 1f;
    // Sprite for when the cloud is raining
    public Sprite Rain;
    // Sprite for normal cloud
    public Sprite Cloud1;
    // Sprite for suprised cloud
    public Sprite Cloud2;
    // The rate at which a raining cloud will shrink
    public float shrinkRate = .25f;

    // Reference to the cloud prefab
    public Cloud prefab;
    // Whether or not the cloud has a suprised face
    public bool Suprised = false;

    protected SpriteRenderer sr;
    protected BoxCollider2D bc;
    protected bool bFollowing; // I don't think this value is used
    protected float TimeAlive = 0f; // Amount of time the cloud has been alive
    protected float TimeSuprised = 0f; // Amount of time the cloud has been suprised
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
        // If we are suprised, update TimeSuprised and change our sprite
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

        // Get everything we are currently overlapping with
        Collider2D[] overlap = Physics2D.OverlapAreaAll(bc.bounds.min, bc.bounds.max);

        // Compute the average temperature of all the water objects we are overlapping with
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

                    // If we are raining, we want to make the water under us get colder, as clouds suck up hot water
                    // This is a real life thing reccommended by Nicole
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

    // Check if we should rain, and if we are raining change our size and sprite
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

    // Calculate what to set size based on the given temperature
    protected virtual void CloudCalculations(float temp)
    {
        float diff = temp - IdealTemp;

        float effRatio = (Mathf.Clamp(diff, -10f, 10f) + 10f)/200f;
        size += idealGrowRate * effRatio * Time.deltaTime;
    }

    // When we collide with another cloud, combine it and us
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Cloud")
        {
            Cloud otherCloud = other.GetComponent<Cloud>();

            // Get the size of our cloud + their cloud (pythagorean theorem)
            float combinedSize = Mathf.Sqrt(otherCloud.size*otherCloud.size + size*size);

            // If I'm older than the other cloud, I stay alive
            if(TimeAlive > otherCloud.TimeAlive)
            {
                Suprised = true;
                // Move me to the position of the biggest cloud
                if (size < otherCloud.size)
                    transform.position = otherCloud.transform.position;

                // Set my velocity to the average velocity of each cloud, weighted based on size
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.velocity = (rb.velocity*(size/combinedSize) + other.GetComponent<Rigidbody2D>().velocity*(otherCloud.size/combinedSize)) / 2f;
                size = combinedSize;
            }
            else // Otherwise I die and they stay alive
            {
                Destroy(this.gameObject);
            }
        }
    }
}
