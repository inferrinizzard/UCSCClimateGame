using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reservoir : MonoBehaviour
{
    /// <summary>
    /// Replenish water
    /// </summary>
    public void AddWater()
    {
        Debug.Log("add water");
        WaterIHave.AddWater();
    }
    
    /// <summary>
    /// Put out a fire
    /// </summary>
    private void OnMouseDown()
    {
        /*// click on fire icon
        // if on fire, puts out fire; Pre: with remaining water 
        if (isOnFire && WaterIHave.EnoughWater())
        {
            isOnFire = false;  // stops the fire growth
            fireRenderer.color = oriFireColor;
            WaterIHave.UseWater();
        }*/

        PlayerInteractions.addDestinationToPath(gameObject.transform);
    }
}
