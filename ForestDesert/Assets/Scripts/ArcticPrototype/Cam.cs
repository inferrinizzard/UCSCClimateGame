using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Cam : MonoBehaviour
{
	private Ray ray;

	public Material m;

	private Camera cam;

	[SerializeField]
	private Slider heat;

	[SerializeField]
	private LineRenderer lr;

	public GameObject markerSource;

	public GameObject lrHolder;

	public GameObject heatRay;

	public GameObject or;

	private GameObject marker;

	private bool hasRay;

	private bool rayDone;

	private Transform target;

	private void Start()
	{
		cam = GetComponent<Camera>();
	}

	private void Update()
	{
		heat.GetComponentInChildren<Text>().text = (heat.GetComponent<Slider>().value - 50f).ToString();
		Vector3 vector = default(Vector3);
		RaycastHit hitInfo;
		if (Input.GetButtonDown("Fire1"))
		{
			ray = cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
			vector = ray.origin + ray.direction * 10f;
			UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 10f, Color.yellow);
			hasRay = true;
			if (Physics.Raycast(ray, out hitInfo) && marker == null && hitInfo.transform.gameObject.name == "Ocean")
			{
				Heat component = UnityEngine.Object.Instantiate(heatRay, hitInfo.point, Quaternion.identity, or.transform).GetComponent<Heat>();
				hasRay = false;
				component.gameObject.SetActive(value: true);
				component.up = true;
				component.length = 25;
				component.loop = false;
				return;
			}
			UnityEngine.Object.Destroy(marker);
			if (hasRay)
			{
				marker = UnityEngine.Object.Instantiate(markerSource, ray.origin + ray.direction * 10f, Quaternion.identity);
			}
		}
		if (hasRay && Input.GetButtonDown("Fire1"))
		{
			if (Physics.Raycast(ray, out hitInfo))
			{
				UnityEngine.Object.Destroy(marker);
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
		hasRay = false;
		if (rayDone)
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

	public IEnumerator ShootRay(Vector3 i, Vector3 f, LineRenderer lr, float time)
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