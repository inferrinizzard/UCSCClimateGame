using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.UI;

public class CityScript : RegionController {
	[SerializeField] Text mainTitle = default;
	[SerializeField] Bill left = default, right = default;
	[SerializeField, Range(0.01f, 0.1f)] float speed = .1f;

	Dictionary<BillDifficulty, List<BillData>> bills = new Dictionary<BillDifficulty, List<BillData>>();
	public enum BillDifficulty { Easy, Med, Hard }
	BillDifficulty currentDifficulty = BillDifficulty.Easy;
	List<BillData> currentBillList => bills[currentDifficulty];
	int currentBillIndex = 0;
	BillData currentBill => currentBillList[currentBillIndex];

	public struct BillData {
		public string name;
		public BillHalf left, right;

		public struct BillHalf {
			public string title, body, tags;
			public Dictionary<string, float> effects;
			public BillHalf(string title, string body, string tags) => (this.title, this.body, this.effects, this.tags) = (title, body, null, tags);
		}

		public BillData(string name, Dictionary<string, string> left = null, Dictionary<string, string> right = null) {
			this.name = name;
			// this.left = new BillHalf(left["title"], left["body"], CityScript.ParseTag(left["tags"]));
			// this.right = new BillHalf(right["title"], right["body"], CityScript.ParseTag(right["tags"]));
			this.left = new BillHalf(left["title"], left["body"], left["tags"]);
			this.right = new BillHalf(right["title"], right["body"], right["tags"]);
		}

		// public Dictionary<string, string> this [string prop] {
		// 	get => prop == "left" ? this.left : this.right;
		// 	set { if (prop == "left") { this.left = value; } else { this.right = value; } }
		// }

		// public override string ToString() => System.String.Format("name:{0}, left:{1}, right:{2}", name,
		// 	"{" + left.Map(kvp => $"{kvp.Key}:[{kvp.Value}]").Reduce((acc, s) => $"{acc} {s}") + "}",
		// 	"{" + right.Map(kvp => $"{kvp.Key}:[{kvp.Value}]").Reduce((acc, s) => $"{acc} {s}") + "}");
	}

	protected override void Start() {
		base.Start();
		bills = LoadBills();
		currentDifficulty = (int) World.impact < 2 ? BillDifficulty.Easy : (int) World.impact < 4 ? BillDifficulty.Med : BillDifficulty.Hard;
		(left.speed, right.speed) = (speed, speed);
	}

	protected override void Init() {
		introBlock.GetComponentInChildren<Button>(true)?.onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
			mainTitle.transform.root.gameObject.SetActive(true);
			InitBill(currentBill);
		}));
	}

	protected override void GameOver() { }

	public static Dictionary<BillDifficulty, List<BillData>> LoadBills() =>
		new string[] { "easy", "med", "hard" }.Map(level =>
			(level, JsonConvert.DeserializeObject<List<BillData>>(Resources.Load<TextAsset>($"bills_{level}").text)))
		.ToDictionary(x => (BillDifficulty) System.Enum.Parse(typeof(BillDifficulty), x.Item1, true), x => { return x.Item2; });

	static Dictionary<string, float> ParseTag(string tag) => tag.Split().ToDictionary(t => Regex.Match(t, @"[A-z]*(?=\+|-)").ToString(), t => float.Parse(Regex.Match(t, @"(?:\+|-).*").ToString()));

	void InitBill(BillData currentBill) {
		currentBill.left.effects = ParseTag(currentBill.left.tags);
		currentBill.right.effects = ParseTag(currentBill.right.tags);
		left.SetBill(currentBill.left);
		right.SetBill(currentBill.right);
	}

	void GetNextBill() {
		if (currentBillIndex++ >= currentBillList.Count - 1) {
			currentDifficulty = (BillDifficulty) (((int) currentDifficulty + 1) % 3);
			currentBillIndex = 0;
		}
	}
}
