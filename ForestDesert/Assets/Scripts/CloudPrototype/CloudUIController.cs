using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls the UI objects in the cloud prototype scene
public class CloudUIController : MonoBehaviour
{
    // Displays the text of the largest recorded cloud in this section
    public Text LargestText;
    // Displays the grade you get at the end of the session
    public Text Grade;
    // Displays the body text: the effects of the grade you got
    public Text BodyText;

    // Reference to the canves group that holds the ending popups
    public GameObject EndingUIGroup;
    // Stores all the game objects active in the scene. 
    // Destroys them all when the session is over so they don't display over the reward text
    public GameObject GameGroup;

    // The size, in world units, of the largest recorded cloud in this session
    private float LargestSize = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] clouds = GameObject.FindGameObjectsWithTag("Cloud");

        // Iterate through every cloud and see if it is larger than the largest recorded cloud
        for (int i = 0; i < clouds.Length; ++i)
        {
            float currScale = clouds[i].transform.localScale.x;
            if (currScale > LargestSize)
            {
                LargestSize = currScale;
            }
        }
        LargestText.text = "Largest Cloud: " + string.Format("{0:0,0}", LargestSize*1000f) + " m^2";
    }

    // Spawns ending UI objects and calculates score
    public void EndGame()
    {
        // Destroy all current clouds
        GameObject[] clouds = GameObject.FindGameObjectsWithTag("Cloud");

        for (int i = 0; i < clouds.Length; ++i)
        {
            GameObject.Destroy(clouds[i]);
        }

        // Set the wind and cloud spawners to not active
        GameGroup.SetActive(false);
        EndingUIGroup.SetActive(true);

        float LargestSizeConverted = LargestSize * 1000f;
        BodyText.text = "Your largest storm size was: " + string.Format("{0:0,0}", LargestSizeConverted) + " m^2!";

        // Calculate score
        if (LargestSizeConverted < 1200f)
        {
            Grade.text = "C";

            BodyText.text += "\nBecause you got a " + Grade.text + " grade, all of the trees in the forest zone have grown an extra turn!";
            GrowTrees(1);
        }
        else if (LargestSizeConverted < 1450f)
        {
            Grade.text = "B";

            BodyText.text += "\nBecause you got a " + Grade.text + " grade, all of the trees in the forest zone have grown an extra turn!";
            GrowTrees(1);
        }
        else if (LargestSizeConverted < 1700f)
        {
            Grade.text = "A";

            BodyText.text += "\nBecause you got a " + Grade.text + " grade, all of the trees in the forest zone have instantly grown to adulthood!";
            GrowTrees(2);
        }
        else 
        {
            Grade.text = "S";

            BodyText.text += "\nBecause you got an " + Grade.text + " grade, all of the trees in the forest zone have instantly grown to adulthood!";
            GrowTrees(2);
        }

        // Calculate the amount to shift the desert based on largest cloud
        float ShiftAmt = 1f + 4f * Mathf.Clamp(LargestSizeConverted - 1200f, 0f, 700f) / 700f;

        BodyText.text += "\nYou also shifted the desert coverage from " + string.Format("{0:0,0.00}", GlobalStatics.DesertCoverage) + " % to ";

        // Shift the desert
        GlobalStatics.DesertCoverage = Mathf.Clamp( GlobalStatics.DesertCoverage - ShiftAmt, 15f, 85 );

        BodyText.text += string.Format("{0:0,0.00}", GlobalStatics.DesertCoverage) + " % !";
    }

    // Grow each tree by x ages
    private void GrowTrees(int x)
    {
        for(int i = 0; i < GlobalStatics.ForestTreeLocations.Count; ++i)
        {
            KeyValuePair<Vector3, int> kvp = GlobalStatics.ForestTreeLocations[i];

            GlobalStatics.ForestTreeLocations[i] = new KeyValuePair<Vector3, int>(kvp.Key, Mathf.FloorToInt(Mathf.Clamp( kvp.Value + 1, 1f, 3.1f )));
        }
    }
}
