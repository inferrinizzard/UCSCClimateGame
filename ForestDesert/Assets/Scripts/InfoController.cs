using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InfoController : MonoBehaviour
{
    float[] tempPred = { 70f, 100f, 135f, 145f, 160f, 165f };
    public LineRenderer tempLine;
    // Start is called before the first frame update
    void Start()
    {
        RenderAllLines();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RenderAllLines()
    {
        RenderLine(tempLine, tempPred);
    }

    void RenderLine(LineRenderer line, float[] vals)
    {
        float start = line.GetComponent<RectTransform>().anchoredPosition.x;
        float width = GetComponent<RectTransform>().rect.width;
        
        line.positionCount = vals.Length;
        Vector3[] positions = new Vector3[vals.Length];

        for(int i = 0; i < vals.Length; ++i)
        {
            positions[i].x = (width - start) * i / (vals.Length - 1);
            positions[i].y = vals[i];
            positions[i].z = 1;
        }

        positions[0].z = 0;
        line.SetPositions(positions);

        LineScript ls = line.GetComponent<LineScript>();
        if (ls)
        {
            ls.BuildMesh();
            ls.ChangeValues(vals);
        }
    }
}
