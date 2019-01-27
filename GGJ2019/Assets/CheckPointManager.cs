using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour {

    List<CheckPointScript> checkPoints;

    private void Start()
    {
        checkPoints = new List<CheckPointScript>();

        for (int i = 0; i < transform.childCount; i++)
        {
            checkPoints.Add(transform.GetChild(i).GetComponent<CheckPointScript>());
        }
    }

    public void SetActive(CheckPointScript checkpoint)
    {
        for (int i = 0; i < checkPoints.Count; i++)
        {
            if (checkPoints[i] != checkpoint)
                checkPoints[i].SetTriggered(false);
        }
    }
}
