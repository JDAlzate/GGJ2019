using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour {

    bool canPull;

    [SerializeField]
    Transform platform;

    [SerializeField]
    Transform targetTransform;

    Vector3 initialPosition;

    bool isTowardsTarget;

	// Use this for initialization
	void Start ()
    {
        initialPosition = platform.position;

        canPull = false;
        isTowardsTarget = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(1) && canPull)
        {
            isTowardsTarget = !isTowardsTarget;
        }

        if (!isTowardsTarget)
            GoTowards(initialPosition);
        else
            GoTowards(targetTransform.position);
    }

    private void GoTowards(Vector3 targetPosition)
    {
        if (Vector3.Distance(platform.transform.position, targetPosition) > 0.1f)
        {
            platform.transform.position = Vector3.Lerp(platform.transform.position, targetPosition, 0.05f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPull = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPull = false;
        }
    }
}
