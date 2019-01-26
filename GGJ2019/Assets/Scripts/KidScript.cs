using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidScript : MonoBehaviour {

    [SerializeField] float movementSpeed;

    Rigidbody rb;
    bool isJumping;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        isJumping = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        PlayerMovement();
	}

    void PlayerMovement()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Horizontal")) * movementSpeed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {

        }
    }


    IEnumerator JumpCoroutine()
    {

        yield return null;
    }

}
