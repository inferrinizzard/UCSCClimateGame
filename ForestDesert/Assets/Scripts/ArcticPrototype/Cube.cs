using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool selected;

    int step;

    void Start() { }

    void Update()
    {
        step++;
        if (selected)		//if hit by ray
        {
            if (step % 5 == 0)		//melt speed
                base.transform.localScale *= 0.99f;
            GetComponent<MeshRenderer>().material.color = Color.blue;		//show melting state
            if (base.transform.lossyScale.sqrMagnitude < 0.85f)		//destroy below threshold
                UnityEngine.Object.Destroy(base.gameObject);
        }
    }
}
