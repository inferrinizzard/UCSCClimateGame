using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
