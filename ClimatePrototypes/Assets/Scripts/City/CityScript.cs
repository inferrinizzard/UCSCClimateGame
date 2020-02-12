using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CityScript : MonoBehaviour {
	[SerializeField] Text mainTitle = default;
	[SerializeField] Text leftText = default, rightText = default;
	[SerializeField] Text leftTitle = default, rightTitle = default;
	[SerializeField, Range(0.01f, 0.1f)] float speed = .1f;

	Dictionary<string, List<Bill>> bills = new Dictionary<string, List<Bill>>();
	//enum BillDifficulty {easy, med, hard};
	public string currentBillDifficulty = "easy";
	private List<Bill> currentBillList = new List<Bill>();
	public int currentBillIndex = 0;
	public Bill currentBill;

	List<Coroutine> coroutines = new List<Coroutine>();

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

		public Dictionary<string, string> this[string prop] {
			get => prop == "left" ? this.left : this.right;
			set {
				if (prop == "left") { this.left = value; } else { this.right = value; }
			}
			// get => this.GetType().GetField(prop);
			// set => this.GetType().GetField(prop).SetValue(this, value);
		}

		public override string ToString() => System.String.Format("name:{0}, left:{1}, right:{2}", name,
			"{" + left.Map(kvp => $"{kvp.Key}:[{kvp.Value}]").Reduce((acc, s) => $"{acc} {s}") + "}",
			"{" + right.Map(kvp => $"{kvp.Key}:[{kvp.Value}]").Reduce((acc, s) => $"{acc} {s}") + "}");
	}

	void Start() {
		bills = LoadBills();
		currentBillList = bills[currentBillDifficulty];
		currentBill = bills[currentBillDifficulty][currentBillIndex];
		PrintBill(currentBill);
	}

	public static Dictionary<string, List<Bill>> LoadBills() =>
		new string[] { "easy", "med", "hard" }.Map(level => {
			using(StreamReader reader = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), $"bills_{level}.json", SearchOption.AllDirectories)[0])) {
				string json = reader.ReadToEnd();
				return (level, JsonConvert.DeserializeObject<List<Bill>>(json));
			}
		}).ToDictionary(x => x.Item1, x => x.Item2);

	void Update() {
		// currentBill = GetNextBill();
	}

	public void ChooseBill(string side) {
		currentBill[side]["tags"].Split().ForEach(
			tag => Func.Lambda(
				(string[] split) => World.UpdateFactor(split[0], float.Parse(split[1] + split[2])))
			(SplitTag(tag)));
		currentBill = GetNextBill();
		coroutines.ForEach(co => StopCoroutine(co));
		coroutines = new List<Coroutine>();
		PrintBill(currentBill);
	}

	public static string[] SplitTag(string tag) => Regex.Split(tag, @"([+]|-)");

	void PrintBill(Bill currentBill) {
		var mainCo = StartCoroutine(Typewriter(mainTitle, currentBill.name, speed));
		var tCoL = StartCoroutine(Typewriter(leftTitle, currentBill.left["title"], speed));
		var tCoR = StartCoroutine(Typewriter(rightTitle, currentBill.right["title"], speed));
		var bCoL = StartCoroutine(Typewriter(leftText, currentBill.left["body"], speed));
		var bCoR = StartCoroutine(Typewriter(rightText, currentBill.right["body"], speed));
		coroutines = new List<Coroutine> { mainCo, tCoL, tCoR, bCoL, bCoR };
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

	IEnumerator Typewriter(Text print, string text, float speed) //given text to print, text ref, and print speed, does typewriter effect
	{
		for (int i = 0; i < text.Length - 1; i++) {
			print.text = text.Substring(0, i);
			yield return new WaitForSeconds(speed);
		}
	}
}
