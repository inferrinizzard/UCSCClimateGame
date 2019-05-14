using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticWind : MonoBehaviour
{
    public float Size = 1f;

    private bool RecentlyClicked = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(RecentlyClicked)
        {
            Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseLocation.z = 0f;

            Vector3 diff = mouseLocation - transform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            Size = Mathf.Clamp((transform.position - mouseLocation).magnitude + .4f, .2f, 3f);

            transform.localScale = new Vector3(Size, Size, Size);

            if (Input.GetMouseButtonUp(0))
            {
                RecentlyClicked = false;
            }
        }
    }

    private void OnMouseDown()
    {
        RecentlyClicked = true;
        Debug.Log("Clicked");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Cloud")
        {
            Rigidbody2D otherRB = other.GetComponent<Rigidbody2D>();

            otherRB.AddForce((Vector2)(transform.rotation * (Vector2.up * (Size / 3f))));
        }
    }
}
