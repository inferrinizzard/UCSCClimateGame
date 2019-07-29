using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IceCloud : MonoBehaviour
{
	public Vector3 origin, reflect;		//start of ray, reflection destination

	LineRenderer lr;		//placeholder linerenderer ref

	public Material mat, red;		//default material, red glow mat

	public bool active;		//if hit by ray

	public GameObject lrHolder;		//ref for reflection ray

	GameObject[] models;		//children GO

	bool finish;		//enum finish

	Vector3 pos;		//this.transform

	public GameObject heatObj;		//heat ray prefab

	public Slider s;		//ui slider

	void Start()
	{
		pos = transform.position;
		//initialise array of children
		models = new GameObject[transform.GetChild(0).childCount];
		for (int i = 0; i < transform.GetChild(0).childCount; i++)
			models[i] = transform.GetChild(0).GetChild(i).gameObject;
	}

	void Update()
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
				StartCoroutine(EmissionLerp(obj, 3f, default(Color), Color.red));
			active = false;
		}
		if (finish)
		{
			finish = false;
			Rays();
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
				StartCoroutine(EmissionLerp(meshRenderer.gameObject, 1f, Color.red, default(Color)));
		}
	}

	IEnumerator EmissionLerp(GameObject obj, float fadeTime, Color a, Color b)		//lerps between natural and glow material
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

	void Rays()		//shoots heat rays from clouds
	{
		//spawn each and set params (use Linq next time)
		Heat heatRay = UnityEngine.Object.Instantiate(heatObj, transform.position, Quaternion.identity).GetComponent<Heat>();
		heatRay.transform.SetParent(transform);
		heatRay.gameObject.SetActive(true);
		heatRay.up = true;
		heatRay.length = 25;
		heatRay.loop = false;
		Heat heatRay2 = UnityEngine.Object.Instantiate(heatObj, transform.position, Quaternion.identity).GetComponent<Heat>();
		heatRay2.transform.SetParent(transform);
		heatRay2.up = false;
		heatRay2.gameObject.SetActive(true);
		heatRay2.length = 25;
		heatRay2.loop = false;
		StartCoroutine(WaitAndDisable(heatRay.gameObject, 3f));
		StartCoroutine(WaitAndDisable(heatRay2.gameObject, 3f));
		s.GetComponent<Slider>().value += 5f;
	}

	public IEnumerator ShootRay(Vector3 i, Vector3 f, LineRenderer lr, float time)	//lerps heat rays over time
	{
		float start = Time.time;
		bool inProgress = true;
		while (inProgress)
		{
			yield return null;
			float num = Time.time - start;
			lr.SetPosition(1, Vector3.Lerp(i, f, num));
			if (num > time)
				inProgress = false;
		}
	}

	IEnumerator WaitAndDisable(GameObject g, float t)		//pause and destroy after time
	{
		yield return new WaitForSeconds(t);
		g.SetActive(false);
		UnityEngine.Object.Destroy(g);
	}

}
