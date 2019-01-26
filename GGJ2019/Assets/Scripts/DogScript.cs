using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogScript : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpSpeed;

    Rigidbody rb;
    Animator animator;
    bool isJumping;

    IEnumerator Turning;
    float desiredAngle = 9999;

    bool isCharacterActive;
    Transform playerFollowPos;

    // Use this for initialization
    void Start ()
    {
        isCharacterActive = false;
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        isJumping = false;

        playerFollowPos = GameObject.Find("Kid").transform.Find("FollowPos");
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(isCharacterActive)
        {
            PlayerMovement();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, playerFollowPos.position, movementSpeed * Time.deltaTime);
        }
	}

    void PlayerMovement()
    {
        Vector3 prevVelocity = rb.velocity;

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, rb.velocity.y);

        if (rb.velocity.normalized.x != 0)
        {
            if (rb.velocity.normalized.x != prevVelocity.normalized.x)
            {
                desiredAngle = 90 * rb.velocity.normalized.x;

                //StopCoroutine(Turning);
                //Turning = TurnTo(desiredAngle);
                //StartCoroutine(Turning);
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
          //  StartCoroutine(JumpCoroutine());
        }
    }

}
