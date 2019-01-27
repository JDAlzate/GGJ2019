using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour {

    CheckPointManager manager;
    bool isTriggered;

    public delegate void MyDelegate();
    public event MyDelegate onRespawn;

    [SerializeField] KidScript kid;
    [SerializeField] BrotherController brother;
    [SerializeField] DogScript dog;

    // Use this for initialization
    void Start ()
    {
        manager = transform.parent.GetComponent<CheckPointManager>();
        isTriggered = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            if(kid)
            {
                kid.onDeath += OnPlayerDeath;
            }
            
            if(brother)
            {
                brother.onDeath += OnPlayerDeath;
            }

            if(dog)
            {
                dog.onDeath += OnPlayerDeath;
            }

            manager.SetActive(this);
        }
    }
    
    void OnPlayerDeath()
    {
        if (isTriggered)
        {
            if(kid)
            kid.transform.position = transform.position;
            if(brother)
            brother.transform.position = transform.position;
            if(dog)
            dog.transform.position = transform.position;

            if (onRespawn != null)
                onRespawn.Invoke();
        }
    }

    public void SetTriggered(bool triggered)
    {
        isTriggered = triggered;
    }
}
