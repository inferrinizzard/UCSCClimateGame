using UnityEngine;

public class Heat : MonoBehaviour
{
	private int step;

	private LineRenderer lr;

	public Material heat;

	public int length = 40;

	public int mod;

	public bool up = true;

	public bool loop = true;

	private bool over;

	private int count;

	private Vector3 pos;

	private void Start()
	{
		mod = 3;
		pos = base.transform.position;
		Material material = new Material(heat);
		material.color = Color.red;
		lr = GetComponent<LineRenderer>();
		lr.material = material;
		lr.widthMultiplier = 0.1f;
		newLr();
	}

	private void Update()
	{
		if (!over)
		{
			step++;
		}
		if (step % mod != 0 || over)
		{
			return;
		}
		count++;
		lr.positionCount = count + 1;
		lr.SetPosition(count, pos + new Vector3(Mathf.Sin((float)((up ? 1 : (-1)) * count) / 2f) / 4f, (float)((up ? 1 : (-1)) * count) / 10f, 0f));
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

	private void newLr()
	{
		count = 1;
		lr.positionCount = 2;
		lr.SetPosition(0, pos + new Vector3(0f, 0f, 0f));
		lr.SetPosition(1, pos + new Vector3(Mathf.Sin(up ? 1 : (-1)) / 3f, (float)(up ? 1 : (-1)) * 0.2f, 0f));
	}
}
