using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class LineScript : MonoBehaviour
{
    public Text text;
    public CanvasGroup GraphGroup;
    public string unit = "f";

    private LineRenderer line;

    private float[] values;
    private int[] years;

    public void Start()
    {
        ChangeValues(new float[]{65f, 73f, 76f, 70f}, new int[] { 2001, 2002, 2003, 2004 });
    }

    public void ChangeValues(float[] n, int[] newYears)
    {
        values = new float[n.Length];
        years = new int[newYears.Length];

        for (int i = 0; i < n.Length; ++i)
        {
            values[i] = n[i];
            years[i] = newYears[i];
        }
    }

    public void BuildMesh()
    {
        RectTransform r = GraphGroup.GetComponent<RectTransform>();
        line = GetComponent<LineRenderer>();
        if (line)
        {
            MeshCollider meshCollider = GetComponent<MeshCollider>();

            Mesh mesh = new Mesh();
            line.BakeMesh(mesh, true);

            Vector3[] verts = mesh.vertices;
            Vector3[] newVerts = new Vector3[verts.Length];

            for (int i = 0; i < verts.Length; ++i)
            {
                Vector3 curr = verts[i];
                curr.x = curr.x * 54.7f / r.localScale.x;
                curr.y = curr.y * 54.7f / r.localScale.x;
                curr.z = curr.z * 54.7f / r.localScale.x;

                newVerts[i] = curr;
            }

            mesh.vertices = newVerts;

            meshCollider.sharedMesh = mesh;
        }
    }

    private int FindClosestValue(Vector3 v)
    {
        RectTransform r = GetComponent<RectTransform>();
        v = v - Camera.main.WorldToScreenPoint(r.position);
        v.z = 0;
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        Vector3[] pos = new Vector3[line.positionCount];
        line.GetPositions(pos);

        for (int i = 0; i < values.Length; ++i)
        {
            float dist = Vector3.Distance(v, pos[i]*0.8f);

            /*if (i == values.Length - 1)
                Debug.Log("v: " + v + "\npos: " + pos[i]);*/

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        return closestIndex;       
    }

    private void OnMouseOver()
    {
        int index = FindClosestValue(Input.mousePosition);

        text.text = years[index] + ": " + values[index] + unit;
        //text.rectTransform.rect.x = val.x;
        Vector3[] pos = new Vector3[line.positionCount];
        line.GetPositions(pos);
        text.rectTransform.anchoredPosition = pos[index];
    }

    private void OnMouseEnter()
    {
        text.gameObject.SetActive(true);
        line.startWidth = .23f;
        line.endWidth = .23f;
        BuildMesh();
    }

    private void OnMouseExit()
    {
        text.gameObject.SetActive(false);
        line.startWidth = .1f;
        line.endWidth = .1f;
        BuildMesh();
    }
}
