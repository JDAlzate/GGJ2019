using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour {

    bool canPull;

    [SerializeField]
    Transform[] platform;

    Renderer handleRenderer;

    [SerializeField]
    Transform[] targetTransform;

    Vector3[] initialPosition;

    bool isTowardsTarget;

    [SerializeField] Material[] colors = new Material[2];

    RespawnController respawnController;

    // Use this for initialization
    void Start ()
    {
         respawnController = gameObject.GetComponent<RespawnController>();

        if (respawnController)
            respawnController.onRespawn += ResetObject;

        handleRenderer = transform.Find("Base").Find("Handle").GetComponent<Renderer>();

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
                handleRenderer.material = colors[0];
            else
                handleRenderer.material = colors[1];
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

    private void ResetObject()
    {
        isTowardsTarget = false;

        handleRenderer.material = colors[1];

        for (int i = 0; i < platform.Length; i++)
        {
            platform[i].position = initialPosition[i];
        }
    }
}
