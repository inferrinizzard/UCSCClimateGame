using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CityScript : MonoBehaviour
{
	// Start is called before the first frame update
	public Text left, right;
	[Range(0.01f, 0.1f)] public float speed = .1f;
	List<string> bills = new List<string>();
	void Start()
	{
		string line, acc = "";
		System.IO.StreamReader file = new System.IO.StreamReader(@"./Assets/Scripts/bills.txt");        //read bills from file
		while ((line = file.ReadLine()) != null)
		{
			acc += line + " \n";
		}
		acc.Split('#').ToList().ForEach(x => bills.Add(x.TrimStart()));     //split by '#'
		StartCoroutine(Typewriter(left, bills[GlobalStatics.billIndex], speed));
		StartCoroutine(Typewriter(right, bills[GlobalStatics.billIndex + 1], speed));
	}

	// Update is called once per frame
	void Update() { }

	IEnumerator Typewriter(Text print, string text, float speed)        //given text to print, text ref, and print speed, does typewriter effect
	{

		for (int i = 0; i < text.Length - 1; i++)
		{
			print.text = text.Substring(0, i);
			yield return new WaitForSeconds(speed);
		}
	}

	public void nextBill()      //intereates next bill
	{
		left.text = right.text = "";
		StopAllCoroutines();
		// StartCoroutine(Typewriter(left,bills[billIndex],speed));
		// StartCoroutine(Typewriter(right,bills[billIndex+1],speed));
		GlobalStatics.billIndex += 2;
	}
}
