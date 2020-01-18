using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CityScript : MonoBehaviour
{
	// Start is called before the first frame update
	public Text left, right;
	[Range(0.01f, 0.1f)] public float speed = .1f;

	public string currBillTag;
	public int currBillIndex;

	public static List<Bill> bills = new List<Bill>();

	public struct Bill
	{
		public string name;
		public Dictionary<string, string> left, right;

		public Bill(string _name, Dictionary<string, string> _left = null, Dictionary<string, string> _right = null)
		{
			name = _name;
			left = _left;
			right = _right;
		}

		public object this[string prop]
		{
			get => this.GetType().GetField(prop);
			set => this.GetType().GetField(prop).SetValue(this, value);
		}

		public override string ToString() => System.String.Format("name:{0}, left:{1}, right:{2}", name,
			"{" + left.Select(kvp => $"{kvp.Key}:[{kvp.Value}]").Aggregate((acc, s) => $"{acc} {s}") + "}",
			"{" + right.Select(kvp => $"{kvp.Key}:[{kvp.Value}]").Aggregate((acc, s) => $"{acc} {s}") + "}");
	}

	void Start()
	{
		currBillIndex = 0; // default

		using(StreamReader reader = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), "bills.json", SearchOption.AllDirectories)[0]))
		{
			string json = reader.ReadToEnd();
			bills = JsonConvert.DeserializeObject<List<Bill>>(json);
			Debug.Log(bills[0]);
			// StartCoroutine(Typewriter(left, bills[0].left, speed));
			// StartCoroutine(Typewriter(right, bills[0].right, speed));
		}
	}

	// Update is called once per frame
	void Update()
	{
		DetermineCurrentBill();
	}

	private void DetermineCurrentBill()
	{
		currBillTag = "co2"; // default
		// bill switching logic
	}

	IEnumerator Typewriter(Text print, string text, float speed) //given text to print, text ref, and print speed, does typewriter effect
	{
		for (int i = 0; i < text.Length - 1; i++)
		{
			print.text = text.Substring(0, i);
			yield return new WaitForSeconds(speed);
		}
	}
}
