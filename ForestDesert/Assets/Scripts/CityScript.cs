using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text left, right;
    [Range(0.01f,0.1f)]public float speed = .1f;
    string filler = "LocationServiceStatus   [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]   sealed class MyAttribute : System.Attribute    {       // See the attribute guidelines at   //  http://go.microsoft.com/fwlink/?LinkId=85236       readonly string positionalString;             // This is a positional argument       public MyAttribute(string positionalString)       {          this.positionalString = positionalString;                       // TODO: Implement code here            throw new System.NotImplementedException();        }               public string PositionalString        {            get { return positionalString; }        }                // This is a named argument        public int NamedInt { get; set; }    }".Trim();
    void Start()
    {
        StartCoroutine(Typewriter(left,filler,speed));
        StartCoroutine(Typewriter(right,filler,speed));
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
}
