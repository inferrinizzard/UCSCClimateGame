using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InfoController : MonoBehaviour
{
    // Placeholder values for temperature
    float[] tempHist = { 65f, 73f, 76f, 70f };
    int[] tempHistYears = { 1990, 1995, 2000, 2005 };
    float[] tempPred = { 70f, 100f, 135f, 145f };
    int[] tempPredYears = { 2005, 2010, 2015, 2020 };
    // Reference to the temperature prediction and history line renderers
    public LineRenderer tempLine;
    public LineRenderer tempHistory;

    // Placeholder values for albedo
    float[] albHist = { 22.5f, 22.3f, 22.4f, 22.6f };
    float[] albPred = { 22.6f, 20f, 18f, 16f };
    // Reference to the albedo prediction and history line renderers
    public LineRenderer albPredLine;
    public LineRenderer albHistLine;

    // Placeholder values for co2
    float[] coHist = { 378f, 380f, 390f, 405f };
    float[] coPred = { 405f, 421f, 436f, 450f };
    // Reference to the co2 prediction and history line renderers
    public LineRenderer coPredLine;
    public LineRenderer coHistLine;

    // When this is true, it will get consumed and render all lines on the next frame
    // If you try to render lines before they've been instantiated correctly (like in Start) it wont render them right
    public bool bRenderOnNextFrame = false;
    // Start is called before the first frame update
    void Start()
    {
        //RenderAllLines();
    }

    // Update is called once per frame
    void Update()
    {
        if (bRenderOnNextFrame)
        {
            RenderAllLines();
            bRenderOnNextFrame = false;
        }
    }

    // Calls RenderLine on each line
    private void RenderAllLines()
    {
        RenderLine(tempHistory, tempLine, tempHist, tempPred);
        RenderLine(albHistLine, albPredLine, albHist, albPred);
        RenderLine(coHistLine, coPredLine, coHist, coPred);
    }

    /*
     * Renders a lines history and prediction section based off array parameters
     * Histline: Reference to line history segment
     * Predline: Reference to prediction line segment
     * hist: stores the history values at each index
     * pred: stores the prediction values at each index
    */
    void RenderLine(LineRenderer HistLine, LineRenderer PredLine, float[] hist, float[] pred)
    {
        // First, we need to figure out the scale we should set our lines to, so it doesn't go outside the frame
        float scale, max = -Mathf.Infinity;

        // Find the maximum value in either the history or prediction array
        for(int i = 0; i < hist.Length || i < pred.Length; ++i)
        {
            if (i < hist.Length && hist[i] > max)
                max = hist[i];

            if (i < pred.Length && pred[i] > max)
                max = pred[i];
        }

        // Then set the scale to 80% of the info group's height divided by the max value
        Rect r = GetComponent<RectTransform>().rect;
        scale = (0.8f*r.height) / max;

        // Render each individual part of the line
        RenderPart(HistLine, hist, tempHistYears, 0f, r.width/2f, scale);
        RenderPart(PredLine, pred, tempPredYears, r.width/2f, r.width, scale);
    }

    /*
     * Renders a part of a line given input values(heights), year values, start and end locations, and scale
    */
    void RenderPart(LineRenderer line, float[] vals, int[] years, float start, float end, float scale)
    {
        // Create an array of Vector3's to store the line's positions
        line.positionCount = vals.Length;
        Vector3[] positions = new Vector3[vals.Length];

        // Go through each elemeny in vals and calculate it's location in pixels
        for (int i = 0; i < vals.Length; ++i)
        {
            // X position is calculated by finding the distance and multiplying it by i / length 
            positions[i].x = (end - start) * i / (vals.Length - 1);
            // Y position is scale * the value
            positions[i].y = scale * vals[i];
            positions[i].z = 1;
        }

        // Not sure if this is needed, but every default line always has a z value of 0, and 1s everywhere else
        positions[0].z = 0;
        line.SetPositions(positions);

        // See if the line is a linescript object
        LineScript ls = line.GetComponent<LineScript>();
        if (ls)
        {
            // If it is, pass in the values and years and calculate the collision mesh
            ls.ChangeValues(vals, years);
            ls.BuildMesh();
        }
    }
}
