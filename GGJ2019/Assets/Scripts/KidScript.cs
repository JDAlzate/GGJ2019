using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidScript : MonoBehaviour {

    [SerializeField] float movementSpeed;
    [SerializeField] float jumpSpeed;

    Rigidbody rb;
    Animator animator;
    bool isJumping;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isJumping = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        PlayerMovement();
	}

    void PlayerMovement()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));


        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
        }
        
        if(Input.GetKeyDown(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, -90, transform.eulerAngles.z);
        }

        if(Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(JumpCoroutine());
        }
    }


    IEnumerator JumpCoroutine()
    {
        isJumping = true;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        //Add force on the first frame of the jump
        rb.AddForce(Vector2.up * 8, ForceMode.Impulse);

        float jumpTime = 2;
        float currentTime = 0;

        while(currentTime < jumpTime)
        {
            currentTime += Time.deltaTime;

            float verticalVel = Mathf.Lerp(jumpSpeed, 0, currentTime / jumpTime);
            rb.velocity = new Vector2(rb.velocity.x, verticalVel);

            yield return null;
        }

        rb.velocity = new Vector2(rb.velocity.x, -2);

        yield return new WaitUntil(() => !isJumping);

        rb.velocity = new Vector2(rb.velocity.x, 0);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            if (isJumping)
                isJumping = false;
        }
    }
}
