using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatState : MonoBehaviour {
    [SerializeField]
    private GameObject one = null;
    [SerializeField]
    private GameObject two = null;
    [SerializeField]
    private GameObject three = null;

    void Update() {
        switch (TempController.tempState) {
            case 1:
                one.SetActive(true);
                two.SetActive(false);
                three.SetActive(false);
                break;
            case 2:
                one.SetActive(false);
                two.SetActive(true);
                three.SetActive(false);
                break;
            case 3:
                one.SetActive(false);
                two.SetActive(false);
                three.SetActive(true);
                break;

            default:
                break;
        }
    }
}
