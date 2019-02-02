using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour {

    [SerializeField] string nextScene;
    [SerializeField] Animator anim;

	// Update is called once per frame
    public IEnumerator LoadScene()
    {
        anim.SetTrigger("Trigger");
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(nextScene);
    }


    private void OnTriggerEnter(Collider collider)
    {
        if(collider.transform.CompareTag("Player"))
        {
            StartCoroutine(LoadScene());
        }
    }
}
