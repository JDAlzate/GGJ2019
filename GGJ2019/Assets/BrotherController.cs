using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrotherController : MonoBehaviour {

    Rigidbody rigidbody;

    Animator animator;

    [SerializeField]
    float speed;

    float move;

    [SerializeField]
    float jumpForce;

    bool isGrounded;

    bool isRunning;

	// Use this for initialization
	void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        move = Input.GetAxisRaw("Horizontal");

        isRunning = move != 0;

        if (isRunning)
        {
            transform.rotation = move > 0 ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);
        }

        animator.SetBool("run", isRunning);

        rigidbody.velocity = new Vector2(move * speed, rigidbody.velocity.y); 

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidbody.AddForce(new Vector2(0, jumpForce));
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
