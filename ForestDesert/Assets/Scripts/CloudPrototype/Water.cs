using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
	public float StartTemperature = 75f;
	public Color HotRed;
	public Color IceBlue;

	[Range(0, .1f)]
	public float SampleX = .0355f;

	[Range(0, 100)]
	public float Displacement = 0;

	[Range(0, 100)]
	public float TempRange = 25f;

	private SpriteRenderer sr;
	private Color StartColor;
	private float CurrTime;
	public float Temperature;
	private float baseTemperature;
	public float Deviation = 0f;
	private bool bDeviating = false;
	private float DeviationTime = 0f;
	// Start is called before the first frame update
	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		StartColor = sr.color;
		HotRed = new Color(169f / 255, 45f / 255, 135f / 255, 1f);
		//IceBlue = new Color(120, 160, 195);

		CurrTime = 0f;
		Displacement = Random.Range(0f, 100f);

		StartTemperature = GlobalStatics.temperature;
	}

	// Update is called once per frame
	void Update()
	{
		CurrTime += Time.deltaTime;

		if (!bDeviating)
			Deviation = Mathf.Lerp(Deviation, 0f, .45f * Time.deltaTime);
		else
		{
			DeviationTime += Time.deltaTime;
			Deviation = Mathf.Lerp(Deviation, 0f, .2f * Time.deltaTime);
			if (DeviationTime > 3f)
			{
				bDeviating = false;
			}
		}

		baseTemperature = StartTemperature + (Mathf.PerlinNoise(Displacement, CurrTime * SampleX) * TempRange - TempRange / 2f);
		Temperature = baseTemperature + Deviation;

		UpdateColor();
	}

	void UpdateColor()
	{
		float diff = Temperature - 75f;
		diff = Mathf.Clamp(diff, -15f, 15f);

		if (diff > 0f)
		{
			//Debug.Log("Before: " + sr.color);
			sr.color = Color.Lerp(StartColor, HotRed, diff / 15f);
			//Debug.Log("After: " + sr.color);
		}
		else
		{
			sr.color = Color.Lerp(StartColor, IceBlue, -1f * diff / 15f);
		}
	}

	public void Deviate(float d)
	{
		bDeviating = true;
		Deviation += d;

		DeviationTime = 0f;
	}
}
