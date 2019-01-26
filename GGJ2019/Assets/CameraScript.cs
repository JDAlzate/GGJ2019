using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public enum CharacterType { Kid = 0, Dog, Brother };

    public Transform character;

    [SerializeField] float followSpeed;
    [SerializeField] float offset;

	void Start ()
    {
	}
	
	void Update ()
    {
        //Lerp to player's pos
       // Rigidbody rb = character.GetComponent<Rigidbody>();

        //if ((int)rb.velocity.normalized.x == 1)
        //{
        //    Debug.Log("Right");
        //    offset = Mathf.Abs(offset);
        //}
        //else if ((int)rb.velocity.normalized.x == -1)
        //{
        //    Debug.Log("Left");
        //    offset = -Mathf.Abs(offset);
        //}

        transform.position = Vector3.Lerp(transform.position, new Vector3(character.position.x,
                                                                          character.position.y + 2.5f, transform.position.z), 
                                                                          followSpeed * Time.deltaTime);
    }
}
