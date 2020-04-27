using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.UI;

public class CityScript : RegionController {
	[SerializeField] Text mainTitle = default; // TODO: fix
	[SerializeField] TMPro.TextMeshProUGUI leftText = default, rightText = default;
	[SerializeField] Text leftTitle = default, rightTitle = default;
	[SerializeField, Range(0.01f, 0.1f)] float speed = .1f;

	[SerializeField] SpriteRenderer leftPerson = default, rightPerson = default;
	[SerializeField] Transform personContainer = default;

	Dictionary<BillDifficulty, List<Bill>> bills = new Dictionary<BillDifficulty, List<Bill>>();
	public enum BillDifficulty { Easy, Med, Hard }
	BillDifficulty currentDifficulty = BillDifficulty.Easy;
	List<Bill> currentBillList = new List<Bill>();
	int currentBillIndex = 0;
	Bill currentBill;
	List<Coroutine> coroutines = new List<Coroutine>();

	public struct Bill {
		public string name;
		public Dictionary<string, string> left, right;

		public Bill(string _name, Dictionary<string, string> _left = null, Dictionary<string, string> _right = null) {
			name = _name;
			left = _left;
			right = _right;
		}

		public Dictionary<string, string> this [string prop] {
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
		currentBillList = bills[currentDifficulty];
		currentBill = bills[currentDifficulty][currentBillIndex];
		introBlock.GetComponentInChildren<Button>(true)?.onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
			mainTitle.transform.root.gameObject.SetActive(true);
			PrintBill(currentBill);
		}));
		// PrintBill(currentBill);

		var persons = personContainer.GetComponentsInChildren<SpriteRenderer>().Select(sr => sr.sprite).OrderBy(x => Random.value).Take(2).ToList();
		(leftPerson.sprite, rightPerson.sprite) = (persons[0], persons[1]);
	}

	public static Dictionary<BillDifficulty, List<Bill>> LoadBills() =>
		new string[] { "easy", "med", "hard" }.Map(level => {
			using(StreamReader reader = new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), $"bills_{level}.json", SearchOption.AllDirectories) [0])) {
				string json = reader.ReadToEnd();
				return (level, JsonConvert.DeserializeObject<List<Bill>>(json));
			}
		}).ToDictionary(x => (BillDifficulty) System.Enum.Parse(typeof(BillDifficulty), x.Item1, true), x => x.Item2);

	public void ChooseBill(string side) {
		currentBill[side]["tags"].Split().ForEach(
			tag => Func.Lambda(
				(string[] split) => World.GetFactor(split[0])?.Update(World.Region.City, null, float.Parse(split[1] + split[2])))
			(SplitTag(tag)));
		currentBill = GetNextBill();
		// coroutines.ForEach(co => StopCoroutine(co));
		coroutines = new List<Coroutine>();
		// PrintBill(currentBill);
	}

	public static string[] SplitTag(string tag) => Regex.Split(tag, @"([+]|[-])");

	void PrintBill(Bill currentBill) {
		var mainCo = StartCoroutine(UIController.Typewriter(mainTitle, currentBill.name, speed));
		var tCoL = StartCoroutine(UIController.Typewriter(leftTitle, currentBill.left["title"], speed));
		var tCoR = StartCoroutine(UIController.Typewriter(rightTitle, currentBill.right["title"], speed));
		var bCoL = StartCoroutine(UIController.Typewriter(leftText, currentBill.left["body"], speed));
		var bCoR = StartCoroutine(UIController.Typewriter(rightText, currentBill.right["body"], speed));
		coroutines = new List<Coroutine> { mainCo, tCoL, tCoR, bCoL, bCoR };
	}

	Bill GetNextBill() {
		if (currentBillIndex++ >= currentBillList.Count - 1) {
			currentDifficulty = (BillDifficulty) (((int) currentDifficulty + 1) % 3);
			currentBillIndex = 0;
		}
		return bills[currentDifficulty][currentBillIndex];
	}

	public void Return() => GameManager.Transition("Overworld");
}
