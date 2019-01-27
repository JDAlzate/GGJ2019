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
        checkpoint.onRespawn += OnRespawn;
    }

    public void OnRespawn()
    {
        this.onRespawn.Invoke();
    }
}