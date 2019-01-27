using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{

    [SerializeField] CheckPointScript checkpoint;

    public delegate void MyDelegate();
    public event MyDelegate onRespawn;

    Vector2 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
        checkpoint.onRespawn += OnRespawn;
    }

    public void OnRespawn()
    {
        if (onRespawn != null)
            onRespawn.Invoke();

        else
            transform.position = initialPosition;
    }
}