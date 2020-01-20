using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class BillEditor : EditorWindow {
    static Dictionary<string, List<CityScript.Bill>> bills = new Dictionary<string, List<CityScript.Bill>>();
    static int index = 0;

    CityScript.Bill newBill = new CityScript.Bill("", new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } }, new Dictionary<string, string> { { "title", "" }, { "body", "" }, { "tags", "" } });
    Dictionary<string, Dictionary<string, float>> newTags = new Dictionary<string, Dictionary<string, float>> { { "left", new Dictionary<string, float> { { "co2", 0f }, { "land", 0f }, { "money", 0f }, { "opinion", 0f } } },
        { "right", new Dictionary<string, float> { { "co2", 0f }, { "land", 0f }, { "money", 0f }, { "opinion", 0f } } }
    };
    Decks d = Decks.Easy;

    enum Decks {
        Easy,
        Medium,
        Hard
    }

    static Dictionary<string, List<CityScript.Bill>> bills2 = new Dictionary<string, List<CityScript.Bill>> {
        {
        "easy",
        new List<CityScript.Bill> {
        new CityScript.Bill("ae",
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } },
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } }),
        new CityScript.Bill("be",
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } },
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } }),
        }
        },
        {
        "medium",
        new List<CityScript.Bill> {
        new CityScript.Bill("am",
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } },
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } }),
        new CityScript.Bill("bm",
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } },
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } }),
        }
        },
        {
        "hard",
        new List<CityScript.Bill> {
        new CityScript.Bill("ah",
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } },
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } }),
        new CityScript.Bill("bh",
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } },
        new Dictionary<string, string> { { "title", "qwertyu" }, { "body", "qwertyu" }, { "tags", "" } }),
        }
        }
    };

    [MenuItem("Window/Bill Editor")]
    [ExecuteInEditMode] void OnEnable() {
        if (!Application.isPlaying) {
            // bills = CityScript.Parse();
            AssignBill(BillEditor.bills2[d.ToString().ToLower()][index]);
        }
    }
    static void ShowWindow() {
        BillEditor window = GetWindow<BillEditor>();
        window.titleContent = new GUIContent("Bill Editor");
        window.Show();
    }

    void OnGUI() {
        GUILayout.Label("Edit a Bill", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        d = (Decks)EditorGUILayout.EnumPopup("Bill Deck", d);
        string deckName = d.ToString().ToLower();
        if (EditorGUI.EndChangeCheck())
            AssignBill(BillEditor.bills2[deckName][index]);
        if (BillEditor.bills2[deckName].Count > 0) {
            CityScript.Bill bill = BillEditor.bills2[deckName][index];

            GUILayout.BeginHorizontal();
            GUILayout.Label("Choose which Bill:");
            EditorGUI.BeginChangeCheck();
            index = EditorGUILayout.Popup(index, bills2[deckName].Select((b, i) => $"{b.name} ({i})").ToArray());
            if (EditorGUI.EndChangeCheck())
                AssignBill(BillEditor.bills2[deckName][index]);
            GUILayout.EndHorizontal();

            newBill.name = EditorGUILayout.TextField("Name", newBill.name);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Left", EditorStyles.boldLabel);
            GUILayout.Label("Title");
            newBill.left["title"] = EditorGUILayout.TextField(newBill.left["title"]);
            GUILayout.Label("Body");
            newBill.left["body"] = EditorGUILayout.TextArea(newBill.left["body"]);
            GUILayout.Label("Tags");
            newTags["left"]["co2"] = EditorGUILayout.FloatField("Emissions:", newTags["left"]["co2"]);
            newTags["left"]["land"] = EditorGUILayout.FloatField("Land Use:", newTags["left"]["land"]);
            newTags["left"]["money"] = EditorGUILayout.FloatField("Money:", newTags["left"]["money"]);
            newTags["left"]["opinion"] = EditorGUILayout.FloatField("Opinion:", newTags["left"]["opinion"]);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Right", EditorStyles.boldLabel);
            GUILayout.Label("Title");
            newBill.right["title"] = EditorGUILayout.TextField(newBill.right["title"]);
            GUILayout.Label("Body");
            newBill.right["body"] = EditorGUILayout.TextArea(newBill.right["body"]);
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
                // Debug.Log(bills2);
                // Directory.GetFiles(Directory.GetCurrentDirectory(), "bills.json", SearchOption.AllDirectories)[0]
                using(StreamWriter writer = new StreamWriter(File.Open("test.json", FileMode.Truncate))) {
                    writer.Write(JsonConvert.SerializeObject(bills2, Formatting.Indented));
                }
                Debug.Log("Saved!");
            }
            if (GUILayout.Button("Reset")) {
                newBill.name = bill.name;
                newBill.left["title"] = bill.left["title"];
                newBill.left["body"] = bill.left["body"];
                newBill.right["title"] = bill.right["title"];
                newBill.right["body"] = bill.right["body"];
                newTags["left"]["co2"] = newTags["left"]["land"] = newTags["left"]["money"] = newTags["left"]["opinion"] = newTags["right"]["co2"] = newTags["right"]["land"] = newTags["right"]["money"] = newTags["right"]["opinion"] = 0f;
            }
            GUILayout.EndHorizontal();
        } else
            GUILayout.Label("No Bills in this deck Found");
    }

    void AssignBill(CityScript.Bill b) {
        newBill.name = b.name;
        newBill.left = b.left;
        newBill.right = b.right;
        newTags["left"] = b.left["tags"].Split().Select(tag => Regex.Split(tag, "(+|-)")).ToDictionary(split => split[0], split => float.Parse(split[1]));
        newTags["right"] = b.left["tags"].Split().Select(tag => Regex.Split(tag, "(+|-)")).ToDictionary(split => split[0], split => float.Parse(split[1]));
    }

}
