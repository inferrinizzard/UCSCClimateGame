using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalStatics
{
    public static float Temperature = 88f;
    public static float CashMoney = 100f;
    public static float DesertCoverage = 20f;
    public static int Turn = 1;
    public static int ActionsRemaining = 2;

    public static Dictionary<Vector3, int> ForestTreeLocations = new Dictionary<Vector3, int>();
}
