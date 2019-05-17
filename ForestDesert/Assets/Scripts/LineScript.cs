using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class LineScript : MonoBehaviour
{
    public Text text;

    private LineRenderer line;

    private float[] values;

    public void Start()
    {
        line = GetComponent<LineRenderer>();

        ChangeValues(new float[]{65f, 73f, 76f, 70f});

        BuildMesh();
    }

    public void ChangeValues(float[] n)
    {
        values = new float[n.Length];

        for (int i = 0; i < n.Length; ++i)
            values[i] = n[i];
    }

    public void BuildMesh()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        line.BakeMesh(mesh, true);

        Vector3[] verts = mesh.vertices;
        Vector3[] newVerts = new Vector3[verts.Length];

        for(int i = 0; i < verts.Length; ++i)
        {
            Vector3 curr = verts[i];
            curr.x = curr.x * 53.5f;
            curr.y = curr.y * 53.5f;
            curr.z = curr.z * 53.5f;

            newVerts[i] = curr;
        }

        mesh.vertices = newVerts;

        meshCollider.sharedMesh = mesh;
    }

    private Vector3 FindClosestValue(Vector3 v)
    {
        RectTransform r = GetComponent<RectTransform>();
        v = v - Camera.main.WorldToScreenPoint(r.position);
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        Vector3[] pos = new Vector3[line.positionCount];
        line.GetPositions(pos);

        for (int i = 0; i < values.Length; ++i)
        {
            float dist = Vector3.Distance(v, pos[i]);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        if (closestIndex != -1)
            return pos[closestIndex];
        else
            return new Vector3(0, 0, 0);
    }

    private void OnMouseOver()
    {
        //Debug.Log(FindClosestValue(Input.mousePosition));
        Vector3 val = FindClosestValue(Input.mousePosition);

        text.text = "2005: " + val.y;
        //text.rectTransform.rect.x = val.x;
        text.rectTransform.anchoredPosition = val;
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
