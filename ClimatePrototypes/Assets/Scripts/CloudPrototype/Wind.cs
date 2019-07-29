using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector3 StartPos;
    public float Size = 1f;
    public bool WindActive = false;

    private float TimeAlive;
    // Start is called before the first frame update
    void Start()
    {
        TimeAlive = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        TimeAlive += Time.deltaTime;
        Size = Mathf.Clamp(Size, .2f, 3f);
        transform.localScale = new Vector3(Size, Size);

        if(TimeAlive > 3f)
            Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Cloud")
        {
            Rigidbody2D otherRB = other.GetComponent<Rigidbody2D>();

            otherRB.AddForce((Vector2)(transform.rotation * (Vector2.up*(Size/3f))));
        }
    }
}
