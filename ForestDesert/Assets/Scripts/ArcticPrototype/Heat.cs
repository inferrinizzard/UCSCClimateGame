using UnityEngine;

public class Heat : MonoBehaviour
{
	int step;		//time iterator

	LineRenderer lr;		//linerenderer ref

	public Material heat;		//red glow material

	public int length = 40;		//heat ray length

	public int mod;		//sine step size

	public bool up = true;		//heat ray direction

	public bool loop = true;		//repeats?

	bool over;		//if reached length

	int count;

	Vector3 pos;		//starting pos

	void Start()
	{
		mod = 3;
		pos = transform.position;
		Material material = new Material(heat);
		material.color = Color.red;
		lr = GetComponent<LineRenderer>();
		lr.material = material;
		lr.widthMultiplier = 0.1f;
		newLr();
	}

	void Update()		//update time and draw heat ray along sine curve
	{
		if (!over)
			step++;
		if (step % mod != 0 || over)
			return;
		count++;
		lr.positionCount = count + 1;
		lr.SetPosition(count, pos + new Vector3(Mathf.Sin((float)((up ? 1 : -1) * count) / 2f) / 4f, (float)((up ? 1 : -1) * count) / 10f, 0f));
		if (step % ((mod * length == 0) ? 1 : length) == 0)
		{
			if (loop)
			{
				newLr();
				return;
			}
			step = 0;
			over = true;
		}
	}

	void newLr()		//create new heat ray
	{
		count = 1;
		lr.positionCount = 2;
		lr.SetPosition(0, pos + new Vector3(0f, 0f, 0f));
		lr.SetPosition(1, pos + new Vector3(Mathf.Sin(up ? 1 : (-1)) / 3f, (float)(up ? 1 : (-1)) * 0.2f, 0f));
	}
}
