using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField]private bool faded = false;
    
    private float dayTime = 10f;

    private float currentVelocity = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool isDayTime
    {
        get { return faded; }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (faded)
        {
            
            StartCoroutine(FadeIn());
        }
        else
        {
            
            StartCoroutine(FadeOut());
        }
        
    }

    IEnumerator FadeOut()
    {
        _spriteRenderer.color = new Color(255f,255f,255f, 0f);
        yield return new WaitForSeconds(dayTime);
        faded = true;
    }
    
    IEnumerator FadeIn()
    {
        _spriteRenderer.color = new Color(255f,255f,255f, 255f);
        
        yield return new WaitForSeconds(dayTime);
        faded = false;
    }
}
