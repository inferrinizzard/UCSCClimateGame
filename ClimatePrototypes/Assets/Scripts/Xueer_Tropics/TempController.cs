using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 3 states of temp 1, 2 and 3 
public class TempController : MonoBehaviour
{
    public static int tempState { get; set;}
    public TropicsCloudMovement tropicsCloud;

    [SerializeField]
    private GameObject plus;
    [SerializeField]
    private GameObject minus;
    [SerializeField]
    private Sprite cloudOriginSprite;
    [SerializeField]
    private SpriteRenderer cloud;

    void Start() {
        tempState = 1;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                if(hit.collider.gameObject == plus )
                {
                    if (tempState < 3)
                    {
                        tempState += 1;
                        onTempChanged();
                    }
                    
                } 
                else if (hit.collider.gameObject == minus)
                {
                    if (tempState > 1)
                    {
                        tempState -= 1;
                    }
                }
                Debug.Log("Temperature State : " + tempState);
            }
        }
    }


    void onTempChanged()
    {
        //reset cloud
        tropicsCloud.resetCloud();
    }
}
