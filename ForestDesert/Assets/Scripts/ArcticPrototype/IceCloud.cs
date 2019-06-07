using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IceCloud : MonoBehaviour
{
	public Vector3 origin;

	public Vector3 reflect;

	private LineRenderer lr;

	public Material mat;

	public Material red;

	public bool active;

	public GameObject lrHolder;

	private GameObject[] models;

	private bool finish;

	private Vector3 pos;

	public GameObject heatRay;

	public Slider s;

	private void Start()
	{
		pos = base.transform.position;
		models = new GameObject[base.transform.GetChild(0).childCount];
		for (int i = 0; i < base.transform.GetChild(0).childCount; i++)
		{
			models[i] = base.transform.GetChild(0).GetChild(i).gameObject;
		}
	}

	private void Update()
	{
		if (active)
		{
			lr = lrHolder.GetComponent<LineRenderer>();
			Vector3 f = Vector3.Reflect(origin - pos, Vector3.up) + pos;
			lr.positionCount = 2;
			lr.SetPosition(0, pos);
			StartCoroutine(ShootRay(pos, f, lr, 0.75f));
			s.GetComponent<Slider>().value -= 3f;
			lr.widthMultiplier = 0.05f;
			Material material = new Material(mat);
			Color color2 = material.color = new Color(1f, 1f, 0.25f, 0.5f);
			lr.material = material;
			lr.startColor = color2;
			lr.endColor = color2;
			GameObject[] array = models;
			foreach (GameObject obj in array)
			{
				StartCoroutine(EmissionLerp(obj, 3f, default(Color), Color.red));
			}
			active = false;
		}
		if (finish)
		{
			finish = false;
			Rays();
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				StartCoroutine(EmissionLerp(meshRenderer.gameObject, 1f, Color.red, default(Color)));
			}
		}
	}

	private IEnumerator EmissionLerp(GameObject obj, float fadeTime, Color a, Color b)
	{
		float start = Time.time;
		bool fading = true;
		while (fading)
		{
			yield return new WaitForEndOfFrame();
			float num = Time.time - start;
			Color color = obj.GetComponent<MeshRenderer>().material.color;
			Color value = Color.Lerp(a, b, num / fadeTime);
			obj.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", value);
			if (num > fadeTime)
			{
				fading = false;
				finish = a.Equals(default(Color));
			}
		}
	}

	private void Rays()
	{
		Heat component = UnityEngine.Object.Instantiate(heatRay, base.transform.position, Quaternion.identity).GetComponent<Heat>();
		component.transform.SetParent(base.transform);
		component.gameObject.SetActive(value: true);
		component.up = true;
		component.length = 25;
		component.loop = false;
		Heat component2 = UnityEngine.Object.Instantiate(heatRay, base.transform.position, Quaternion.identity).GetComponent<Heat>();
		component2.transform.SetParent(base.transform);
		component2.up = false;
		component2.gameObject.SetActive(value: true);
		component2.length = 25;
		component2.loop = false;
		StartCoroutine(WaitAndDisable(component.gameObject, 3f));
		StartCoroutine(WaitAndDisable(component2.gameObject, 3f));
		s.GetComponent<Slider>().value += 5f;
	}

	public IEnumerator ShootRay(Vector3 i, Vector3 f, LineRenderer lr, float time)
	{
		float start = Time.time;
		bool inProgress = true;
		while (inProgress)
		{
			yield return null;
			float num = Time.time - start;
			lr.SetPosition(1, Vector3.Lerp(i, f, num));
			if (num > time)
			{
				inProgress = false;
			}
		}
	}

	private IEnumerator WaitAndDisable(GameObject g, float t)
	{
		yield return new WaitForSeconds(t);
		g.SetActive(value: false);
		UnityEngine.Object.Destroy(g);
	}
}
