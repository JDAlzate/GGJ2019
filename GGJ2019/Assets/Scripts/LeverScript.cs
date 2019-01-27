using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour {

    bool canPull;

    [SerializeField]
    Transform[] platform;

    [SerializeField]
    Transform[] targetTransform;

    Vector3[] initialPosition;

    bool isTowardsTarget;

    [SerializeField] Material[] colors = new Material[2];

	// Use this for initialization
	void Start ()
    {
        initialPosition = new Vector3[platform.Length];

        for (int i = 0; i < platform.Length; i++)
        {
            initialPosition[i] = platform[i].position;
        }

        canPull = false;
        isTowardsTarget = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPull)
        {
            isTowardsTarget = !isTowardsTarget;

            if(isTowardsTarget)
                transform.Find("Base").Find("Handle").GetComponent<Renderer>().material = colors[0];
            else
                transform.Find("Base").Find("Handle").GetComponent<Renderer>().material = colors[1];
        }

        if (!isTowardsTarget)
        {
            for (int i = 0; i < platform.Length; i++)
            {
                GoTowards(platform[i], initialPosition[i]);

            }
        }
        else
        {
            for (int i = 0; i < platform.Length; i++)
            {
                GoTowards(platform[i], targetTransform[i].position);
            }
        }
    }

    private void GoTowards(Transform platform, Vector3 targetPosition)
    {
        if (Vector3.Distance(platform.position, targetPosition) > 0.1f)
        {
            platform.position = Vector3.Lerp(platform.position, targetPosition, 0.05f);
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
