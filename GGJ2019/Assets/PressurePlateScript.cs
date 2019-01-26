using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    GateScript gate;

    void Awake()
    {
        gate = transform.parent.GetComponent<GateScript>();
    }

    void Start()
    {
        transform.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gate.charactersOnMe++;
            transform.position -= new Vector3(0, .1f, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            gate.charactersOnMe--;
            transform.position += new Vector3(0, .1f, 0);
        }
    }
}
