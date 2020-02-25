using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBackgroundDay : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _spriteRenderer.color = new Color(1f,1f,1f, Mathf.Lerp(255, 0, 1));
    }
}
