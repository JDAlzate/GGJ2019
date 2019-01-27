using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameScript : MonoBehaviour
{
    public ScreenTransition transition;

    public void LoadbyIndex(int sceneIndex)
    {
        StartCoroutine(transition.LoadScene());
    }
}