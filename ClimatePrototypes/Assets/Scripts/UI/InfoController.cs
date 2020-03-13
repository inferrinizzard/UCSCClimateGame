using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InfoController : MonoBehaviour {
	//temperature
	float[] tempHist = { 65f, 73f, 76f, 70f };
	int[] tempHistYears = { 1990, 1995, 2000, 2005 };
	float[] tempPred = { 70f, 100f, 135f, 145f };
	int[] tempPredYears = { 2005, 2010, 2015, 2020 };
	public LineRenderer tempLine;
	public LineRenderer tempHistory;

	//albedo
	float[] albHist = { 22.5f, 22.3f, 22.4f, 22.6f };
	float[] albPred = { 22.6f, 20f, 18f, 16f };
	public LineRenderer albPredLine;
	public LineRenderer albHistLine;

	//co2 density
	float[] coHist = { 378f, 380f, 390f, 405f };
	float[] coPred = { 405f, 421f, 436f, 450f };
	public LineRenderer coPredLine;
	public LineRenderer coHistLine;

	public bool bRenderOnNextFrame = false;
	// Start is called before the first frame update
	void Start() {
		//RenderAllLines();
	}

	// Update is called once per frame
	void Update() {
		if (bRenderOnNextFrame) {
			RenderAllLines();
			bRenderOnNextFrame = false;
		}
	}

	private void RenderAllLines() {
		RenderLine(tempHistory, tempLine, tempHist, tempPred);
		RenderLine(albHistLine, albPredLine, albHist, albPred);
		RenderLine(coHistLine, coPredLine, coHist, coPred);
	}

	void RenderLine(LineRenderer HistLine, LineRenderer PredLine, float[] hist, float[] pred) {
		float scale, max = -Mathf.Infinity;

		for (int i = 0; i < hist.Length || i < pred.Length; ++i) {
			if (i < hist.Length && hist[i] > max)
				max = hist[i];

			if (i < pred.Length && pred[i] > max)
				max = pred[i];
		}

		Rect r = GetComponent<RectTransform>().rect;
		scale = (0.8f * r.height) / max;

		RenderPart(HistLine, hist, tempHistYears, 0f, r.width / 2f, scale);
		RenderPart(PredLine, pred, tempPredYears, r.width / 2f, r.width, scale);
	}

	void RenderPart(LineRenderer line, float[] vals, int[] years, float start, float end, float scale) {
		line.positionCount = vals.Length;
		Vector3[] positions = new Vector3[vals.Length];

		for (int i = 0; i < vals.Length; ++i) {
			positions[i].x = (end - start) * i / (vals.Length - 1);
			positions[i].y = scale * vals[i];
			positions[i].z = 1;
		}

		positions[0].z = 0;
		line.SetPositions(positions);

		LineScript ls = line.GetComponent<LineScript>();
		if (ls) {
			ls.ChangeValues(vals, years);
			ls.BuildMesh();
		}
	}
}
