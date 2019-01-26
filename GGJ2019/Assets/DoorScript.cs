using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

    bool isOpen;

	// Use this for initialization
	void Start ()
    {
        isOpen = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isOpen)
        {
            if (transform.rotation.y > 0)
                transform.Rotate(new Vector3(0, -5.0f, 0));
            else
            {
                Collider[] colliders = this.GetComponentsInChildren<Collider>();

                foreach (Collider c in colliders)
                {
                    c.enabled = false;
                }

                this.GetComponent<Collider>().enabled = false;
            }
        }
	}

    private void OnTriggerEnter(Collider collision)
    {
        if(!isOpen && collision.CompareTag("Player"))
        {
            KidScript kidScript = collision.gameObject.GetComponent<KidScript>();

            if(kidScript.GetKeyCount() > 0)
            {
                kidScript.UseKey();
                isOpen = true;
            }
        }
    }
}
