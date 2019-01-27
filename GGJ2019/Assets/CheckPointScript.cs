using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour {

    bool isTriggered;

    public delegate void MyDelegate();
    public event MyDelegate onRespawn;
	// Use this for initialization
	void Start ()
    {
        isTriggered = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            KidScript kid = other.GetComponent<KidScript>();
            BrotherController brother = other.GetComponent<BrotherController>();
            if(kid)
            {
                kid.onDeath += OnPlayerDeath;
            }
            
            else if(brother)
            {
                brother.onDeath += OnPlayerDeath;
            }
        }
    }
    
    void OnPlayerDeath()
    {
        onRespawn.Invoke();
    }
}
