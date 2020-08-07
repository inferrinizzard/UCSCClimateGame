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
		public Dictionary<string, string> left, right;

		public BillData(string _name, Dictionary<string, string> _left = null, Dictionary<string, string> _right = null) {
			name = _name;
			left = _left;
			right = _right;
		}

		public Dictionary<string, string> this [string prop] {
			get => prop == "left" ? this.left : this.right;
			set { if (prop == "left") { this.left = value; } else { this.right = value; } }
		}

		public override string ToString() => System.String.Format("name:{0}, left:{1}, right:{2}", name,
			"{" + left.Map(kvp => $"{kvp.Key}:[{kvp.Value}]").Reduce((acc, s) => $"{acc} {s}") + "}",
			"{" + right.Map(kvp => $"{kvp.Key}:[{kvp.Value}]").Reduce((acc, s) => $"{acc} {s}") + "}");
	}

	protected override void Start() {
		base.Start();
		bills = LoadBills();
		currentDifficulty = (int) World.impact < 2 ? BillDifficulty.Easy : (int) World.impact < 4 ? BillDifficulty.Med : BillDifficulty.Hard;
	}

	protected override void Init() {
		introBlock.GetComponentInChildren<Button>(true)?.onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
			mainTitle.transform.root.gameObject.SetActive(true);
			PrintBill(currentBill);
		}));
	}

	protected override void GameOver() { }

	public static Dictionary<BillDifficulty, List<BillData>> LoadBills() =>
		new string[] { "easy", "med", "hard" }.Map(level =>
			(level, JsonConvert.DeserializeObject<List<BillData>>(Resources.Load<TextAsset>($"bills_{level}").text)))
		.ToDictionary(x => (BillDifficulty) System.Enum.Parse(typeof(BillDifficulty), x.Item1, true), x => x.Item2);

	public void ChooseBill(string side) {
		currentBill[side]["tags"].Split().ForEach(
			tag => Func.Lambda(
				(string[] split) => World.GetFactor(split[0])?.Update(World.Region.City, null, float.Parse(split[1] + split[2])))
			(SplitTag(tag)));
	}

	public static string[] SplitTag(string tag) => Regex.Split(tag, @"([+]|[-])");

	void PrintBill(BillData currentBill) {
		left.Print(currentBill.left["title"], currentBill.left["body"]);
		right.Print(currentBill.right["title"], currentBill.right["body"]);
	}

	void GetNextBill() {
		if (currentBillIndex++ >= currentBillList.Count - 1) {
			currentDifficulty = (BillDifficulty) (((int) currentDifficulty + 1) % 3);
			currentBillIndex = 0;
		}
	}
}
