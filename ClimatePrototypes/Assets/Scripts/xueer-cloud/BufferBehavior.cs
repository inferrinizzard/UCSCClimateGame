using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferBehavior : MonoBehaviour
{
    public enum IceHealth
    {
        Full,
        Damaged,
        Melted
    }

    public IceHealth health = IceHealth.Full;
    private void OnTriggerEnter2D (Collider2D collision)
    {
        Debug.Log("hit");
        if (collision.gameObject.tag == "SolarRadiation")
        {
            TakeDamage();
            UpdateGraphics();
            
        }
    }

    private void TakeDamage()
    {
        
        switch (health)
        {
            case IceHealth.Full:
                health = IceHealth.Damaged;
                break;
            default:
                health = IceHealth.Melted;
                break;
        }
    }

    private void UpdateGraphics()
    {
        switch (health)
        {
            case IceHealth.Damaged:
                GetComponent<SpriteRenderer>().color = Color.black;
                break;
            case IceHealth.Melted:
                Destroy(gameObject);
                break;
        }
    }
}
