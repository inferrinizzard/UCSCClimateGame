using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFade : MonoBehaviour
{
    CanvasGroup cg;
    public AnimationCurve AlphaCurve;

    private float CurrLife = 0f;
    // Start is called before the first frame update
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrLife += Time.deltaTime;

        cg.alpha = AlphaCurve.Evaluate(CurrLife / 4.5f);

        if (CurrLife > 4.5f)
            this.gameObject.SetActive(false);
    }
}
