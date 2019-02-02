using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour {

    public int charactersOnMe;

    Vector3 initialPos;
    Vector3 finalPos;
    [SerializeField] bool mustHold;

    bool isCoroutineRunning;

    RespawnController respawnController;

    // Use this for initialization
    void Start ()
    {
        respawnController = gameObject.GetComponent<RespawnController>();

        if (respawnController)
            respawnController.onRespawn += ResetObject;

        initialPos = transform.position;
        finalPos = transform.Find("OpenTransform").position;

        isCoroutineRunning = false;
         charactersOnMe = 0;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (charactersOnMe > 0 && !isCoroutineRunning)
        {
            if (mustHold)
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

        while (Vector3.Distance(transform.position, finalPos) > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, finalPos, 2 * Time.deltaTime);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    IEnumerator OpenDoorHold()
    {
        isCoroutineRunning = true;

        while (Vector3.Distance(transform.position, finalPos) > .1f && charactersOnMe > 0)
        {
            transform.position = Vector3.Lerp(transform.position, finalPos, 2 * Time.deltaTime);
            yield return null;
        }

    }

    IEnumerator CloseDoor()
    {
        isCoroutineRunning = true;

        while (Vector3.Distance(transform.position, initialPos) > .1f && charactersOnMe == 0)
        {
            transform.position = Vector3.Lerp(transform.position, initialPos, 2 * Time.deltaTime);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    private void ResetObject()
    {
        if (isCoroutineRunning)
        {
            StopAllCoroutines();
        }
        isCoroutineRunning = false;
        transform.position = initialPos;
        charactersOnMe = 0;
    }

}
