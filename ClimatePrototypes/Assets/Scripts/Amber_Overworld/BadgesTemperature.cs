using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadgesTemperature : MonoBehaviour
{
    // Start is called before the first frame update
    Image myImageComponent;
    public Sprite[] badge = new Sprite[3];


    // Start is called before the first frame update
    void Start()
    {
        myImageComponent = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        /* TODO:Temperature Values
        if (World.temp[] > 50.0)
        {
            myImageComponent.sprite = badge[2];
        }
        else if (World.publicOpinion < 0)
        {
            myImageComponent.sprite = badge[1];
        }
        else
        {
            myImageComponent.sprite = badge[0];
        }*/
    }
}
