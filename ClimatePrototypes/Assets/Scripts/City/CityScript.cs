using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Web;

public class CityScript : MonoBehaviour {
	// Start is called before the first frame update
	public Text left, right;
	[Range(0.01f, 0.1f)] public float speed = .1f;

	public string currBillName;
	public int currBillIndex;

	private float ppm;
	private float albedoDelta;

	Dictionary<string, Bill> bills = new Dictionary<string, Bill>();

	public class Bill {
		public string name;
		public string left, right;

		public string tags;

		public Bill(string _name, string _left = "", string _right = "", string _tags = "") {
			name = _name;
			left = _left;
			right = _right;
			// tags = _tags ?? new string[0];
			tags = _tags;
		}

		public object this [string prop] {
			get => this.GetType().GetField(prop);
			set => this.GetType().GetField(prop).SetValue(this, value);
		}

		public override string ToString() => $"name:{name}, left:{left}, right:{right}, tags:[{System.String.Join(" ",tags)}]";
	}

	void Start() 
	{
		currBillIndex = 0; // default

		using (StreamReader r = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), "bills.json", SearchOption.AllDirectories) [0]))
		{
			string json = r.ReadToEnd();
			List<Bill> bills = JsonConvert.DeserializeObject<List<Bill>>(json);
			StartCoroutine(Typewriter(left, bills[0].left, speed));
			StartCoroutine(Typewriter(right, bills[0].right, speed));
		}

		//JsonTextReader reader = new JsonTextReader(new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), "bills.json", SearchOption.AllDirectories) [0]));
		//Bill curBill = new Bill("");
		// string prop = "name";
		// while (reader.Read()) {
		// 	if (reader.Value != null)
		// 		if (prop == "")
		// 			prop = reader.Value.ToString();
		// 		else {
		// 			curBill[prop] = reader.Value.ToString();

		// 			prop = "";
		// 		}
		// 	else if (reader.TokenType == JsonToken.EndObject && prop != "name") {
		// 		prop = "name";
		// 		bills[curBill.name] = curBill;
		// 		curBill = new Bill("");
		// 	}
		// }
	}
	
	// Update is called once per frame
	void Update() 
	{ 
		determineCurrBill();
		notifyWorldChange(currBillName);
	}

	private void determineCurrBill()
	{
		currBillName = "co2"; // default
		// bill switching logic, flow unknown
	}

	private void notifyWorldChange(string billName)
	{
		switch(billName)
		{
			case "co2":
				World.UpdateCO2(ppm);
				break;
			case "albedo":
				World.UpdateAlbedo(albedoDelta);
				break;
		}

	}

	IEnumerator Typewriter(Text print, string text, float speed) //given text to print, text ref, and print speed, does typewriter effect
	{
		for (int i = 0; i < text.Length - 1; i++) {
			print.text = text.Substring(0, i);
			yield return new WaitForSeconds(speed);
		}
	}
}
