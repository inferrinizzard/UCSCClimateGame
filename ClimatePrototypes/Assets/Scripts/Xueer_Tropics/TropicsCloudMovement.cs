using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TropicsCloudMovement : MonoBehaviour {
    //public Sprite waterVapourSprite;
    public Sprite cloudMatureSprite;
    public Sprite cloudOriginSprite;

    [SerializeField]
    private Transform cloudTransform = null;
    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private Transform cloudOriginTransform = null;
    void Update() {
        float verticalSpeed = Input.GetAxis("Vertical");
        float horizontalSpeed = Input.GetAxis("Horizontal");

        Vector3 vertical2D = new Vector3(0, 1, 0);
        Vector3 horizontal2D = new Vector3(1, 0, 0);
        cloudTransform.Translate(vertical2D * verticalSpeed * movementSpeed * Time.deltaTime);
        cloudTransform.Translate(horizontal2D * horizontalSpeed * movementSpeed * Time.deltaTime);
    }

    public void resetCloud() {
        cloudTransform.position = cloudOriginTransform.position;
        gameObject.GetComponent<SpriteRenderer>().sprite = cloudOriginSprite;
    }

    public void matureCloud() {
        gameObject.GetComponent<SpriteRenderer>().sprite = cloudMatureSprite;
    }
}
