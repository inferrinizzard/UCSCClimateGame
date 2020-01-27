﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class BillEditor : EditorWindow {
	static Dictionary<string, List<CityScript.Bill>> bills = new Dictionary<string, List<CityScript.Bill>>();
	static int index = 0;

	static CityScript.Bill newBill = new CityScript.Bill("", new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } }, new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } });
	static Dictionary<string, Dictionary<string, float>> newTags = new Dictionary<string, Dictionary<string, float>> { { "left", new Dictionary<string, float> { { "co2", 0f }, { "land", 0f }, { "money", 0f }, { "opinion", 0f } } },
		{ "right", new Dictionary<string, float> { { "co2", 0f }, { "land", 0f }, { "money", 0f }, { "opinion", 0f } } }
	};
	readonly static Dictionary<string, string> tagsVerbose = new Dictionary<string, string> { { "co2", "Emissions:" }, { "land", "Land Use:" }, { "money", "Money:" }, { "opinion", "Opinion:" } };

	static BillEditor self = null;

	static bool deletePrompt = false;
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
			new List<string> { "left", "right" }.ForEach(section => {
				GUILayout.BeginVertical();
				GUILayout.Label(char.ToUpper(section[0]) + section.Substring(1), EditorStyles.boldLabel);
				GUILayout.Label("Title");
				newBill[section]["title"] = EditorGUILayout.TextField(newBill[section]["title"], EditorStyles.textArea);
				GUILayout.Label("Body");
				newBill[section]["body"] = EditorGUILayout.TextArea(newBill[section]["body"], EditorStyles.textArea, GUILayout.MinHeight(position.height / 2 - 100), GUILayout.MaxWidth(position.width * .49f));
				GUILayout.Label("Tags");
				newTags[section].Keys.ToList().ForEach(k => newTags[section][k] = EditorGUILayout.FloatField(tagsVerbose[k], newTags[section][k]));
				GUILayout.EndVertical();
			});
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save")) {
				bill.name = newBill.name;
				newBill.left["tags"] = System.String.Join(" ", newTags["left"].Where(kvp => kvp.Value != 0).Select(kvp => kvp.Key + (kvp.Value > 0 ? "+" : "") + kvp.Value.ToString()));
				bill.left = newBill.left;
				newBill.right["tags"] = System.String.Join(" ", newTags["right"].Where(kvp => kvp.Value != 0).Select(kvp => kvp.Key + (kvp.Value > 0 ? "+" : "") + kvp.Value.ToString()));
				bill.right = newBill.right;
				Debug.Log("Saved: " + bill);
				bills[deckName] = bills[deckName].Select((b, i) => i == index?bill : b).ToList();
				WriteToFile(deckName);
			}
			if (GUILayout.Button("Reset"))
				self.AssignBill(BillEditor.bills[deckName][index]);
			if (GUILayout.Button("Add New Bill")) {
				CityScript.Bill empty = new CityScript.Bill("new bill", new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } }, new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } });
				bills[deckName].Add(empty);
				// index = bill[deckName].Count - 1;
			}
			if (GUILayout.Button("Delete Bill"))
				if (!deletePrompt)
					deletePrompt = true;
				else {
					BillEditor.bills[deckName].RemoveAt(index);
					WriteToFile(deckName);
					index = 0;
					deletePrompt = false;
				}
			GUILayout.EndHorizontal();
			if (deletePrompt)
				GUILayout.Label("Click the «Delete Bill» again to delete.");
		} else
			GUILayout.Label("No Bills in this deck Found");
	}

	void AssignBill(CityScript.Bill b) {
		newBill.name = b.name;
		newBill.left = b.left;
		newBill.right = b.right;
		newTags.Keys.ToList().ForEach(section => {
			newTags[section] = newTags[section].ToDictionary(kvp => kvp.Key, kvp => 0f);
			if (b.name != "new bill")
				b[section]["tags"].Split().ToList().ForEach(tag => {
					var match = CityScript.SplitTag(tag);
					newTags[section][match[0]] = float.Parse(match[2]);
				});
		});
	}

	void WriteToFile(string deckName) {
		string json = JsonConvert.SerializeObject(bills[deckName], Formatting.Indented);
		using(StreamWriter writer = new StreamWriter(Directory.GetFiles(Directory.GetCurrentDirectory(), $"bills_{deckName}.json", SearchOption.AllDirectories)[0])) {
			writer.Write(json);
		}
	}

}
