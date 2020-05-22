using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolunteerManager : MonoBehaviour
{
    
    public Sprite unemployedVolunteerIcon;
    public Sprite employedVolunteerIcon;

    public Transform purchaseIcon;
    public int numberofMaxVolunteers = 6;
    public GameObject[] volunteers;
    private Dictionary<GameObject , bool > volunteerStatus = new Dictionary<GameObject, bool>();
    
    public float initialBudget = 60;
    public float money = 60;
    public float costPer = 10;
    // Start is called before the first frame update
    void Start()
    {
        // initialize all volunteers to be not employed state
        for (int i =0; i <numberofMaxVolunteers; i++)
        {
            volunteerStatus.Add(volunteers[i],false);
            volunteers[i].GetComponent<VolunteerState>().amIEmployed = false;
            UnemployVolunteer(volunteers[i].GetComponent<SpriteRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Purchase one volunteer
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform == purchaseIcon)
            {
                // if enough money
                if (money >= costPer)
                {
                    money -= costPer;
                    foreach (var vol in volunteerStatus.Keys)
                    {
                        if (volunteerStatus[vol] == false)
                        {
                            volunteerStatus[vol] = true;
                            vol.GetComponent<VolunteerState>().amIEmployed = true;
                            EmployVolunteer(vol.GetComponent<SpriteRenderer>());
                            return;
                        }
                    }
                }
            }
        }

    }
    

    void EmployVolunteer(SpriteRenderer volunteer)
    {
        volunteer.sprite = employedVolunteerIcon;
    }

    void UnemployVolunteer(SpriteRenderer volunteer)
    {
        volunteer.sprite = unemployedVolunteerIcon;
    }
    
    
}
