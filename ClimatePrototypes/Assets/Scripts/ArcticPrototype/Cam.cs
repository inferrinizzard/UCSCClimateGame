using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Cam : MonoBehaviour
{
	Ray ray;    //stores ray from camera to world space
	public Material m;      //default meterial
	Camera cam;     //Main Camera

	[SerializeField] Slider heat = default;        //reference to UI heat gauge

	[SerializeField] LineRenderer lr, tracer = null;    //linerenderer prefab, tracing line reference

	public GameObject markerSource;   //marker prefan

	public GameObject lrHolder, click;    //temp holder for reflection, 	ui click indicator

	public GameObject heatObj;    //wavy ray prefab

	public GameObject or;   //ocean ray

	GameObject marker;    //reference to marker sphere

	bool hasRay;    //marker active

	bool rayDone;   //ray enum finished

	Transform target;       //reference to ray end object
	Vector3 vector;   //line from marker to target


	void Start()
	{
		cam = Camera.main;
	}

	void Update()
	{
		if (marker != null)   //if no marker active, spawn new and activate tracer
		{
			tracer.SetPosition(1, cam.ScreenToWorldPoint(Input.mousePosition));
			// click.transform.position = cam.WorldToScreenPoint(Input.mousePosition);
			// if (Physics.Raycast(ray))	
			//     click.transform.position = Input.mousePosition;
		}
		// heat.GetComponentInChildren<Text>().text = (heat.GetComponent<Slider>().value - 50f).ToString();
		RaycastHit hitInfo;   //raycast out
		if (Input.GetButtonDown("Fire1"))
		{
			ray = cam.ScreenPointToRay(Input.mousePosition);
			vector = ray.origin + ray.direction * 10;
			// vector = cam.ScreenToWorldPoint(10);
			// Debug.DrawRay(ray.origin, ray.direction * 10f, Color.yellow);
			hasRay = true;
			if (Physics.Raycast(ray, out hitInfo) && marker == null && hitInfo.transform.gameObject.name == "Ocean")
			{
				//shoot heat ray up from ocean
				Heat heatRay = UnityEngine.Object.Instantiate(heatObj, hitInfo.point, Quaternion.identity, or.transform).GetComponent<Heat>();
				hasRay = false;
				heatRay.gameObject.SetActive(value: true);
				heatRay.up = true;
				heatRay.length = 25;
				heatRay.loop = false;
				return;
			}
			UnityEngine.Object.Destroy(marker);
			if (hasRay)
			{
				//draw tracer line
				marker = UnityEngine.Object.Instantiate(markerSource, ray.origin + ray.direction * 10f, Quaternion.identity);
				tracer.gameObject.SetActive(true);
				tracer.SetPosition(0, marker.transform.position);
			}
		}

		if (hasRay)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				if (Physics.Raycast(ray, out hitInfo))
				{
					UnityEngine.Object.Destroy(marker);
					tracer.gameObject.SetActive(false);
					target = hitInfo.transform.root;
					if (lr != null)
					{
						Vector3 position = lr.GetPosition(0);
						if (target.gameObject.name != "Ocean")
						{
							lr.SetPosition(0, new Vector3(position.x, position.y, target.position.z));
						}
						StartCoroutine(ShootRay(lr.GetPosition(0), (target.gameObject.name == "Ocean") ? hitInfo.point : target.position, lr, 0.75f));
						lr.transform.SetParent((target.gameObject.name != "Ocean") ? target : or.transform);
						lr = null;
					}
				}
				else
				{
					if (lr == null)
					{
						lr = new GameObject().AddComponent<LineRenderer>();
					}
					lr.positionCount = 2;
					lr.SetPosition(0, vector);
					lr.SetPosition(1, vector + 0.001f * Vector3.down);
					lr.widthMultiplier = 0.05f;
					Material material = new Material(m);
					Color color2 = material.color = new Color(1f, 1f, 0f, 1f);
					lr.material = material;
					lr.startColor = color2;
					lr.endColor = color2;
				}
			}
		}
		hasRay = false;
		if (rayDone)    //draw different rays depending on target
		{
			if (target.gameObject.tag == "Ice")
			{
				target.gameObject.GetComponent<Cube>().selected = true;
				heat.GetComponent<Slider>().value += 10f;
			}
			if (target.gameObject.tag == "Cloud")
			{
				IceCloud component2 = target.gameObject.GetComponent<IceCloud>();
				component2.origin = ((lr != null) ? vector : component2.origin);
				component2.active = true;
				component2.mat = m;
				component2.lrHolder = ((lr != null) ? UnityEngine.Object.Instantiate(lrHolder) : component2.lrHolder);
				heat.GetComponent<Slider>().value -= 5f;
			}
			if (target.gameObject.name == "Ocean")
			{
				heat.GetComponent<Slider>().value += 8f;
			}
			rayDone = false;
		}
	}

	public IEnumerator ShootRay(Vector3 i, Vector3 f, LineRenderer lr, float time)    //lerps between two points and draws a line in progress
	{
		float start = Time.time;
		bool inProgress = true;
		while (inProgress)
		{
			yield return null;
			float num = Time.time - start;
			lr.SetPosition(1, Vector3.Lerp(i, f, num / time));
			if (num > time)
			{
				inProgress = false;
				rayDone = true;
			}
		}
	}
}