using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float IdealTemp = 73f;
    public float size = 2f;
    public float idealGrowRate = 1f;
    public Sprite Rain;
    public Sprite Cloud1;
    public Sprite Cloud2;
    public float shrinkRate = .25f;
    public Cloud prefab;
    public bool Suprised = false;

    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private bool bFollowing;
    private float TimeAlive = 0f;
    private float TimeSuprised = 0f;
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
        Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseLocation.z = 0;

        if (false && sr.bounds.Contains(mouseLocation) && Input.GetMouseButtonDown(0))
            bFollowing = true;

        if (Input.GetMouseButtonUp(0))
            bFollowing = false;

        if (bFollowing)
            transform.position = Vector3.Lerp(transform.position, mouseLocation, .3f);

        Collider2D[] overlap = Physics2D.OverlapAreaAll(bc.bounds.min, bc.bounds.max);

        float avgTemp = 0f;
        int numWater = 0;
        for(int i = 0; i < overlap.Length; ++i)
        {
            if (overlap[i].tag == "Water")
            {
                avgTemp += overlap[i].GetComponent<Water>().Temperature;
                numWater++;
            }
        }

        avgTemp /= numWater;
        CloudCalculations(avgTemp);

        if (TimeAlive > 12f)
        {
            sr.sprite = Rain;
            size -= shrinkRate*Time.deltaTime;
        }

        if (size < .5f)
        {
            Destroy(this.gameObject);
        }

        transform.localScale = new Vector3(size, size);
    }

    void CloudCalculations(float temp)
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

            if (TimeAlive > otherCloud.TimeAlive)
            {
                Cloud newCloud = Instantiate(prefab, transform.position, transform.rotation) as Cloud;
                newCloud.size = combinedSize;
                newCloud.TimeAlive = TimeAlive;

                newCloud.Suprised = true;
            }

            Destroy(this.gameObject);
        }
    }
}
