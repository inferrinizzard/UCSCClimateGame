using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineScript : MonoBehaviour
{
    private LineRenderer line;

    private float[] values;

    public void Start()
    {
        line = GetComponent<LineRenderer>();
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
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        Vector3[] pos = new Vector3[line.numPositions];
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
        Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseLocation.z = 0f;

        Debug.Log(mouseLocation);
    }

    private void OnMouseEnter()
    {
        line.SetWidth(.2f, .2f);
    }

    private void OnMouseExit()
    {
        line.SetWidth(.1f, .1f);
    }
}
