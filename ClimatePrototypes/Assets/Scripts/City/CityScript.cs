using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CityScript : MonoBehaviour {
	[SerializeField] Text mainTitle;
	[SerializeField] Text leftText, rightText;
	[SerializeField] Text leftTitle, rightTitle;
	[SerializeField, Range(0.01f, 0.1f)] float speed = .1f;

	Dictionary<string, List<Bill>> bills = new Dictionary<string, List<Bill>>();
	//enum BillDifficulty {easy, med, hard};
	public string currentBillDifficulty = "easy";
	private List<Bill> currentBillList = new List<Bill>();
	public int currentBillIndex = 0;
	public Bill currentBill;

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
		bills = LoadBills();
		currentBillList = bills[currentBillDifficulty];
		currentBill = bills[currentBillDifficulty][currentBillIndex];
		PrintBill(currentBill);
	}

	public static Dictionary<string, List<Bill>> LoadBills() =>
		new string[] { "easy", "med", "hard" }.Select(level => {
			using(StreamReader reader = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), $"bills_{level}.json", SearchOption.AllDirectories)[0])) {
				string json = reader.ReadToEnd();
				return (level, JsonConvert.DeserializeObject<List<Bill>>(json));
			}
		}).ToDictionary(x => x.Item1, x => x.Item2);

	void Update() {
		// currentBill = GetNextBill();
	}

	void ChooseBill(string side) {

		currentBill = GetNextBill();
	}

	void PrintBill(Bill currentBill) {
		StartCoroutine(Typewriter(mainTitle, currentBill.name, speed));
		StartCoroutine(Typewriter(leftTitle, currentBill.left["title"], speed));
		StartCoroutine(Typewriter(rightTitle, currentBill.right["title"], speed));
		StartCoroutine(Typewriter(leftText, currentBill.left["body"], speed));
		StartCoroutine(Typewriter(rightText, currentBill.right["body"], speed));
	}

	Bill GetNextBill() {
		if (currentBillIndex < currentBillList.Count - 1) {
			currentBillIndex += 1;
		} else {
			currentBillIndex = 0;
			switch (currentBillDifficulty) {
				case "easy":
					currentBillDifficulty = "med";
					break;
				case "med":
					currentBillDifficulty = "hard";
					break;
				case "hard":
					currentBillDifficulty = "easy"; // loop back?
					break;
					// null if exaust TODO: message
				default:
					break;

			}
		}
		return bills[currentBillDifficulty][currentBillIndex];
	}

	void NotifyWorldChange(string billName) {
		switch (billName) {
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
