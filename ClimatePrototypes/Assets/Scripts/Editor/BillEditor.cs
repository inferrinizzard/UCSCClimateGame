using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class BillEditor : EditorWindow {
	static Dictionary<string, List<CityScript.Bill>> bills = new Dictionary<string, List<CityScript.Bill>>();
	static int index = 0;

	static CityScript.Bill newBill = new CityScript.Bill("", new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } }, new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } });
	static Dictionary<string, Dictionary<string, float>> newTags = new Dictionary<string, Dictionary<string, float>> { { "left", new Dictionary<string, float> { { "co2", 0f }, { "land", 0f }, { "money", 0f }, { "opinion", 0f } } },
		{ "right", new Dictionary<string, float> { { "co2", 0f }, { "land", 0f }, { "money", 0f }, { "opinion", 0f } } }
	};

	static BillEditor self = null;
	Decks d;

	enum Decks { easy, med, hard }

	[MenuItem("Window/Bill Editor")]
	static void Awake() {
		bills = CityScript.LoadBills();
		self = GetWindow<BillEditor>();
		self.AssignBill(BillEditor.bills["easy"][index]);
		EditorStyles.textArea.wordWrap = true;
		EditorStyles.textArea.clipping = TextClipping.Overflow;
	}
	static void ShowWindow() {
		self.titleContent = new GUIContent("Bill Editor");
		self.Show();
	}

	void OnGUI() {
		if (self == null)
			self = GetWindow<BillEditor>();

		if (bills.Count == 0) {
			bills = CityScript.LoadBills();
			self.AssignBill(BillEditor.bills["easy"][index]);
		}

		GUILayout.Label("Edit a Bill", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck();
		d = (Decks)EditorGUILayout.EnumPopup("Bill Deck", d);
		string deckName = d.ToString();
		if (EditorGUI.EndChangeCheck())
			AssignBill(BillEditor.bills[deckName][index]);

		if (BillEditor.bills[deckName].Count > 0) {
			CityScript.Bill bill = BillEditor.bills[deckName][index];

			GUILayout.BeginHorizontal();
			GUILayout.Label("Choose which Bill:");
			EditorGUI.BeginChangeCheck();
			index = EditorGUILayout.Popup(index, bills[deckName].Select((b, i) => $"{b.name} ({i})").ToArray());
			if (EditorGUI.EndChangeCheck())
				AssignBill(BillEditor.bills[deckName][index]);
			GUILayout.EndHorizontal();

			newBill.name = EditorGUILayout.TextField("Name", newBill.name);
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Label("Left", EditorStyles.boldLabel);
			GUILayout.Label("Title");
			newBill.left["title"] = EditorGUILayout.TextField(newBill.left["title"], EditorStyles.textArea);
			GUILayout.Label("Body");
			newBill.left["body"] = EditorGUILayout.TextArea(newBill.left["body"], EditorStyles.textArea, GUILayout.MinHeight(position.height / 2 - 100), GUILayout.MaxWidth(position.width * .49f));
			GUILayout.Label("Tags");
			newTags["left"]["co2"] = EditorGUILayout.FloatField("Emissions:", newTags["left"]["co2"]);
			newTags["left"]["land"] = EditorGUILayout.FloatField("Land Use:", newTags["left"]["land"]);
			newTags["left"]["money"] = EditorGUILayout.FloatField("Money:", newTags["left"]["money"]);
			newTags["left"]["opinion"] = EditorGUILayout.FloatField("Opinion:", newTags["left"]["opinion"]);
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			GUILayout.Label("Right", EditorStyles.boldLabel);
			GUILayout.Label("Title");
			newBill.right["title"] = EditorGUILayout.TextField(newBill.right["title"], EditorStyles.textArea);
			GUILayout.Label("Body");
			newBill.right["body"] = EditorGUILayout.TextArea(newBill.right["body"], EditorStyles.textArea, GUILayout.MinHeight(position.height / 2 - 100), GUILayout.MaxWidth(position.width * .49f));
			GUILayout.Label("Tags");
			newTags["right"]["co2"] = EditorGUILayout.FloatField("Emissions:", newTags["right"]["co2"]);
			newTags["right"]["land"] = EditorGUILayout.FloatField("Land Use:", newTags["right"]["land"]);
			newTags["right"]["money"] = EditorGUILayout.FloatField("Money:", newTags["right"]["money"]);
			newTags["right"]["opinion"] = EditorGUILayout.FloatField("Opinion:", newTags["right"]["opinion"]);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save")) {
				bill.name = newBill.name;
				newBill.left["tags"] = System.String.Join(" ", newTags["left"].Where(kvp => kvp.Value != 0).Select(kvp => kvp.Key + (kvp.Value > 0 ? "+" : "") + kvp.Value.ToString()));
				bill.left = newBill.left;
				newBill.right["tags"] = System.String.Join(" ", newTags["right"].Where(kvp => kvp.Value != 0).Select(kvp => kvp.Key + (kvp.Value > 0 ? "+" : "") + kvp.Value.ToString()));
				bill.right = newBill.right;
				Debug.Log("Saved: " + bill);
			}
			if (GUILayout.Button("Reset"))
				self.AssignBill(BillEditor.bills[deckName][index]);
			GUILayout.EndHorizontal();
		} else
			GUILayout.Label("No Bills in this deck Found");
	}

	void AssignBill(CityScript.Bill b) {
		newBill.name = b.name;
		newBill.left = b.left;
		newBill.right = b.right;
		newTags.Keys.ToList().ForEach(section => {
			newTags[section] = newTags[section].ToDictionary(kvp => kvp.Key, kvp => 0f);
			b.left["tags"].Split().ToList().ForEach(tag => {
				var match = Regex.Split(tag, @"([+]|-)");
				newTags[section][match[0]] = float.Parse(match[2]);
			});
		});
	}

}
