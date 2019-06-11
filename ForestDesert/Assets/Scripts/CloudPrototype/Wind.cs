using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    // The position we were spawned at
    public Vector3 StartPos;
    // Current scale
    public float Size = 1f;
    // Whether or not we are active and influencing clouds
    public bool WindActive = false;

    // total time we've been alive
    private float TimeAlive;
    // Start is called before the first frame update
    void Start()
    {
        TimeAlive = 0f;
    }

    // Increment time alive, if we've been alive for too long destroy ourself
    void Update()
    {
        TimeAlive += Time.deltaTime;
        Size = Mathf.Clamp(Size, .2f, 3f);
        transform.localScale = new Vector3(Size, Size);

        if(TimeAlive > 3f)
            Destroy(this.gameObject);
    }

    // If we overlap with a cloud, move it based on our direction
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Cloud")
        {
            Rigidbody2D otherRB = other.GetComponent<Rigidbody2D>();

            otherRB.AddForce((Vector2)(transform.rotation * (Vector2.up*(Size/3f))));
        }
    }
}
