using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CityScript : MonoBehaviour {
	// Start is called before the first frame update
	public Text left, right;
	[Range(0.01f, 0.1f)] public float speed = .1f;
	// List<string> bills = new List<string>();

	Dictionary<string, Bill> bills = new Dictionary<string, Bill>();

	public struct Bill {
		public string name;
		public string left, right;

		public string[] tags;

		public Bill(string _name, string _left = "", string _right = "", string[] _tags = null) {
			name = _name;
			left = _left;
			right = _right;
			tags = _tags;
		}
	}

	void Start() {
		JsonTextReader reader = new JsonTextReader(new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), "bills.json", SearchOption.AllDirectories) [0]));

		int i = 0;
		Bill curBill = new Bill("");
		bool billFlag = true;
		while (reader.Read()) {
			if (reader.Value != null) {
				if (billFlag) {
					billFlag = false;
					curBill.name = reader.Value.ToString();
				}
			} else {
				if (reader.TokenType == JsonToken.EndObject && curBill.name != "") {
					billFlag = true;
					bills[curBill.name] = curBill;
					curBill = new Bill("");
				}
			}
		}

		// StartCoroutine(Typewriter(left, bills[World.billIndex], speed));
		// StartCoroutine(Typewriter(right, bills[World.billIndex + 1], speed));
	}

	// Update is called once per frame
	void Update() { }

	IEnumerator Typewriter(Text print, string text, float speed) //given text to print, text ref, and print speed, does typewriter effect
	{
		for (int i = 0; i < text.Length - 1; i++) {
			print.text = text.Substring(0, i);
			yield return new WaitForSeconds(speed);
		}
	}
}
