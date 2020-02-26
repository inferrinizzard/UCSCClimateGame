using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBackgroundDay : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField]private bool faded = false;

    private float dayTime = 5.0f;

    private float currentVelocity = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        yield return new WaitForSeconds(5f);
        faded = true;
    }
    
    IEnumerator FadeIn()
    {
        _spriteRenderer.color = new Color(255f,255f,255f, 255f);
        
        yield return new WaitForSeconds(5f);
        faded = false;
    }
}
