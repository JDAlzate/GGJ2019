using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    [SerializeField] Transform door;

    Transform openTransform;
    [SerializeField] bool mustHold;

    [SerializeField] int charactersOnMe;
    Vector3 InitialPos;

    bool isCoroutineRunning;
	// Use this for initialization
	void Start ()
    {
        door = transform.parent;
        openTransform = door.Find("OpenTransform");
        openTransform.parent = null;
        transform.parent = null;
        charactersOnMe = 0;
        InitialPos = door.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.M))
            door.transform.position = openTransform.position;

        if(Input.GetKeyDown(KeyCode.N))
            door.transform.position = InitialPos;

        if (charactersOnMe > 0 && !isCoroutineRunning)
        {
            if(mustHold)
            {
                StartCoroutine(OpenDoorHold());
            }
            else
            {
                StartCoroutine(OpenDoor());
            }
        }
        else if (charactersOnMe == 0 && mustHold)
        {
            StartCoroutine(CloseDoor());
        }
	}

    IEnumerator OpenDoor()
    {
        isCoroutineRunning = true;

        while (Vector3.Distance(door.position, openTransform.position) > .1f)
        {
            door.position = Vector3.Lerp(door.position, openTransform.position, 2 * Time.deltaTime);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    IEnumerator OpenDoorHold()
    {
        isCoroutineRunning = true;

        while (Vector3.Distance(door.position, openTransform.position) > .1f && charactersOnMe > 0)
        {
            door.position = Vector3.Lerp(door.position, openTransform.position, 2 * Time.deltaTime);
            yield return null;
        }

    }

    IEnumerator CloseDoor()
    {
        isCoroutineRunning = true;

        while (Vector3.Distance(door.position, InitialPos) > .1f && charactersOnMe == 0)
        {
            door.position = Vector3.Lerp(door.position, InitialPos, 2 * Time.deltaTime);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            charactersOnMe++;
            transform.position -= new Vector3(0, .1f, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            charactersOnMe--;
            transform.position += new Vector3(0, .1f, 0);
        }
    }
}
