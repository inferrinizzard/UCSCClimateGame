using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BillEconomyPublicOp : MonoBehaviour

{

    public void IncreasePublicOpinion()
    {
        
        World.publicOpinion += 10.0f;
        Debug.Log(World.publicOpinion);
    }

    public void DecreasePublicOpinion()
    {
        World.publicOpinion -= 10.0f;
        Debug.Log(World.publicOpinion);
    }

    public void GainMoney()
    {
        World.money += 100.0f;
        Debug.Log(World.money);
    }

    public void LoseMoney()
    {
        World.money -= 100.0f;
        Debug.Log(World.money);
    }
}
