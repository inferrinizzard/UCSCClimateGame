using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Child of cloud for the Horizontal prototype.
// Evaluates whether or not it will start raining based on how high it is, not on how long it's been alive
public class HorizCloud : Cloud
{
    public AnimationCurve HeightRainChance;
    protected override void RainCheck()
    {
        float chance = HeightRainChance.Evaluate( (transform.position.y + 5) / 10f);

        if (size > 3f || Random.Range(0f, 1f) < chance*Time.deltaTime)
        {
            sr.sprite = Rain;
            size -= shrinkRate * Time.deltaTime;
        }

        if (size < .25f)
        {
            Destroy(this.gameObject);
        }

        transform.localScale = new Vector3(size, size, size);
    }
}
