using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CityScript : MonoBehaviour {
	public Text left, right;
	[Range(0.01f, 0.1f)] public float speed = .1f;

	
	Dictionary<string, List<Bill>> bills = new Dictionary<string, List<Bill>>();
	//enum BillDifficulty {easy, med, hard};
	public string currBillDifficulty;
	private List<Bill> currBillList = new List<Bill>();
	public int currBillIndex;
	public Bill currBill;

	private float ppm;
	private float albedoDelta;


	public struct Bill {
		public string name;
		public Dictionary<string, string> left, right;

		public Bill(string _name, Dictionary<string, string> _left = null, Dictionary<string, string> _right = null) {
			name = _name;
			left = _left;
			right = _right;
		}

		public object this[string prop] {
			get => this.GetType().GetField(prop);
			set => this.GetType().GetField(prop).SetValue(this, value);
		}

		public override string ToString() => System.String.Format("name:{0}, left:{1}, right:{2}", name,
			"{" + left.Select(kvp => $"{kvp.Key}:[{kvp.Value}]").Aggregate((acc, s) => $"{acc} {s}") + "}",
			"{" + right.Select(kvp => $"{kvp.Key}:[{kvp.Value}]").Aggregate((acc, s) => $"{acc} {s}") + "}");
	}

	void Start() {
		currBillDifficulty = "easy"; // default
		currBillList = bills[currBillDifficulty];
		currBillIndex = 0;
		currBill = bills[currBillDifficulty][currBillIndex];
		PrintBill(currBill);

		using(StreamReader reader = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), "bills_easy.json", SearchOption.AllDirectories)[0])) {
			string json = reader.ReadToEnd();
			bills["easy"] = JsonConvert.DeserializeObject<List<Bill>>(json);
		}

		using(StreamReader reader = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), "bills_med.json", SearchOption.AllDirectories)[0])) {
			string json = reader.ReadToEnd();
			bills["med"] = JsonConvert.DeserializeObject<List<Bill>>(json);
		}

		using(StreamReader reader = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), "bills_hard.json", SearchOption.AllDirectories)[0])) {
			string json = reader.ReadToEnd();
			bills["hard"] = JsonConvert.DeserializeObject<List<Bill>>(json);
		}

	}

	

	void Update() {
		currBill = GetNextBill();
		PrintBill(currBill);
	}

	void PrintBill(Bill currBill) {
		// StartCoroutine(Typewriter(left, currBill.left, speed));   //TODO: tostring method 
		// StartCoroutine(Typewriter(right, currBill.right, speed));
	}

	Bill GetNextBill()
	{
		if ( currBillIndex < currBillList.Count - 1 )
		{
			currBillIndex += 1;
		}
		else
		{
			switch(currBillDifficulty)
			{
				case "easy":
					currBillDifficulty = "med";
					currBillIndex = 0;
					break;
				case "med":
					currBillDifficulty = "hard";
					currBillIndex = 0;
					break;
				case "hard":
					currBillDifficulty = "easy";  // loop back?
					currBillIndex = 0;
					break;
					  // null if exaust TODO: message
				default:
					break;

			}
		}

		return bills[currBillDifficulty][currBillIndex];

	}
		
	void NotifyWorldChange(string billName) 
	{
		switch (billName) 
		{
			case "co2":
				// World.UpdateCO2(ppm);
				break;
			case "albedo":
				// World.UpdateAlbedo(albedoDelta);
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