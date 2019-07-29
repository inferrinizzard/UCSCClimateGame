using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VertMoveText : MonoBehaviour
{
    public Text t;
    public Vector2 StartPos;
    public Vector2 EndPos;
    public AnimationCurve MoveCurve;

    private float CurrTime = 0f;
    private float LifeTime = 1.2f;
    private string StringVal = "+XX";
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Text>();
        t.text = StringVal;
    }

    public void Initialize(Vector2 start, Vector2 end, float val)
    {
        StartPos = start;
        EndPos = end;

        if (val < 0)
        {
            StringVal = "- " + string.Format("{0:0,0}", Mathf.Abs(val));
        }
        else
        {
            StringVal = "+ " + string.Format("{0:0,0}", val);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //float lerpY = Mathf.Lerp(160f, 200f, MoveCurve.Evaluate(CurrTime / LifeTime));
        t.rectTransform.anchoredPosition = Vector2.Lerp(StartPos, EndPos, MoveCurve.Evaluate(CurrTime / LifeTime));

        Color newC = t.color;
        newC.a = 1f - MoveCurve.Evaluate(CurrTime / LifeTime);
        t.color = newC;

        CurrTime += Time.deltaTime;

        if (CurrTime >= LifeTime)
            Destroy(this.gameObject);
    }
}
