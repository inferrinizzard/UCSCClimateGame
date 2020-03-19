using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTracker : MonoBehaviour
{
    [SerializeField] Slider damageFire = default;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        damageFire.value = WaterSpraying.damage;
    }
}
