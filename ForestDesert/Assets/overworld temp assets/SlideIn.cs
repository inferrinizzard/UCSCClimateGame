using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideIn : MonoBehaviour
{
    bool active = false;
    // Update is called once per frame
    void Update()
    {
            
    }
    public void Active(){ active = !active; StartCoroutine(Slide(active ? -1 : 1)); }

    IEnumerator Slide(int dir){
        for(int i=0;i<GetComponent<RectTransform>().rect.width/16;i++){
            GetComponent<RectTransform>().transform.Translate(GetComponent<RectTransform>().rect.x > 0 && dir < 0 ? Vector3.zero : new Vector3(dir * .4f,0,0));
            yield return new WaitForSeconds(.01f);
        }
    }
}
