using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CityScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text left, right;
    [Range(0.01f,0.1f)]public float speed = .1f;
    List<string> bills = new List<string>();
    public int billIndex = -2;
    void Start()
    {
        string line, acc = "";
        System.IO.StreamReader file = new System.IO.StreamReader(@"./Assets/Scripts/bills.txt");
		while ((line = file.ReadLine()) != null){
            acc += line + " \n";
        }
        acc.Split('#').ToList().ForEach(x=>bills.Add(x.TrimStart()));
        nextBill();
        // StartCoroutine(Typewriter(left,bills[billIndex],speed));
        // StartCoroutine(Typewriter(right,bills[billIndex+1],speed));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Typewriter(Text print, string text, float speed){
     
        for (int i = 0; i < text.Length-1; i++)
        {
            print.text = text.Substring(0,i);
            yield return new WaitForSeconds(speed);
        }
    }

    public void nextBill(){
        Debug.Log(billIndex);
        left.text = right.text = "";
        StopAllCoroutines();
        StartCoroutine(Typewriter(left,bills[billIndex],speed));
        StartCoroutine(Typewriter(right,bills[billIndex+1],speed));
        billIndex+=2;
    }
}
