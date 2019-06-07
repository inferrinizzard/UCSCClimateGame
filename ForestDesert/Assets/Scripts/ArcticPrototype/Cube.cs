using UnityEngine;

public class Cube : MonoBehaviour
{
	public bool selected;

	private int step;

	private void Start()
	{
	}

	private void Update()
	{
		step++;
		if (selected)
		{
			if (step % 5 == 0)
			{
				base.transform.localScale *= 0.99f;
			}
			GetComponent<MeshRenderer>().material.color = Color.blue;
			if (base.transform.lossyScale.sqrMagnitude < 0.85f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
