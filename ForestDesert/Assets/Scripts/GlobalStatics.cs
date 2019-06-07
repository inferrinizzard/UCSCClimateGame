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

    // This list stores the position (Vector3) and age (int) of each tree in the forest area
    public static List<KeyValuePair<Vector3, int>> ForestTreeLocations = new List<KeyValuePair<Vector3, int>>();
}
