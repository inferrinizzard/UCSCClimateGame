using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class LineScript : MonoBehaviour
{
    // Reference to text object that displays the hover value
    public Text text;
    // Reference to the info group
    public CanvasGroup GraphGroup;
    // The unit to display when hovering
    public string unit = "f";

    // Reference to the linerenderer
    private LineRenderer line;

    // The values to display at each index
    private float[] values;
    // The year to display at each index
    private int[] years;

    public void Start()
    {
        // Set up some default values if changevalues is never called
        ChangeValues(new float[]{65f, 73f, 76f, 70f}, new int[] { 2001, 2002, 2003, 2004 });
    }

    // Changes values and years to the input parameters
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

    // Builds the collision mesh for this line
    // Collision mesh is needed to track the mouse for hovering
    public void BuildMesh()
    {
        // Get the rectangle of the info group
        RectTransform r = GraphGroup.GetComponent<RectTransform>();
        line = GetComponent<LineRenderer>();
        if (line)
        {
            MeshCollider meshCollider = GetComponent<MeshCollider>();

            // Create a new mesh and bake the linerenderer's mesh into it
            Mesh mesh = new Mesh();
            line.BakeMesh(mesh, true);

            // Get every vertex for the baked mesh, and make an empty matching array
            Vector3[] verts = mesh.vertices;
            Vector3[] newVerts = new Vector3[verts.Length];

            // For each vertex, scale it by 54.7x
            // If you don't do this, the mesh collider will be extremely small (as it will be in world units)
            // This number is somehow tied to the aspect ratio of the screen, and I couldn't figure out how
            // It will need to be changed if screen size is not 1920x1080
            for (int i = 0; i < verts.Length; ++i)
            {
                Vector3 curr = verts[i];
                curr.x = curr.x * 54.7f / r.localScale.x;
                curr.y = curr.y * 54.7f / r.localScale.x;
                curr.z = curr.z * 54.7f / r.localScale.x;

                newVerts[i] = curr;
            }

            mesh.vertices = newVerts;

            // Set the mesh of our collider to this new mesh we just made
            meshCollider.sharedMesh = mesh;
        }
    }

    // Finds the closest value to a given position v
    // v should be a world position (NOT a screen position)
    private int FindClosestValue(Vector3 v)
    {
        RectTransform r = GetComponent<RectTransform>();
        v.z = 0;

        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        // Get all the positions of the line
        Vector3[] pos = new Vector3[line.positionCount];
        line.GetPositions(pos);

        // Go through each position in the line and find the closest position to v
        for (int i = 0; i < values.Length; ++i)
        {
            // Find the world position of the current line position by adding it and it's rectangle's screen positions, then converting to world position
            Vector3 newPos = Camera.main.ScreenToWorldPoint(pos[i] + Camera.main.WorldToScreenPoint(r.position));
            newPos.z = 0;

            // Find the distance to v and new current positions world position
            float dist = Vector3.Distance(v, newPos);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        return closestIndex;       
    }

    // Handles when the mouse is over the collider
    private void OnMouseOver()
    {
        // Find the index of the closest line position
        int index = FindClosestValue(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // Display its year and value
        text.text = years[index] + ": " + values[index] + unit;

        // Move the text object to display over the closest position
        Vector3[] pos = new Vector3[line.positionCount];
        line.GetPositions(pos);
        text.rectTransform.anchoredPosition = pos[index];
    }

    // Make the line thicker on mouse over
    private void OnMouseEnter()
    {
        text.gameObject.SetActive(true);
        line.startWidth = .23f;
        line.endWidth = .23f;
        BuildMesh();
    }

    // Make the line thinner when the mouse exits
    private void OnMouseExit()
    {
        text.gameObject.SetActive(false);
        line.startWidth = .1f;
        line.endWidth = .1f;
        BuildMesh();
    }
}
