using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferBehavior : MonoBehaviour
{
    public int bufferHealth = 2;
    private void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            TakeDamage();
            Crack();
            Destroy(collision.gameObject);
        }
    }

    private void TakeDamage()
    {
        bufferHealth -= 1;
    }
    
    private void Crack()
    {
        if (bufferHealth <= 0)
        {
            GetComponent<SpriteRenderer>().color = Color.black; 
        }
        else
        {
            if (bufferHealth < 3 )
            {
                GetComponent<SpriteRenderer>().color = Color.gray; 
            }
        }
        
        
    }
}
