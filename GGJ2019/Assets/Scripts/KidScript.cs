using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidScript : MonoBehaviour {

    [SerializeField] float movementSpeed;
    [SerializeField] float jumpSpeed;

    Rigidbody rb;
    Animator animator;
    CameraScript camScript;
    bool isJumping;

    IEnumerator Turning;
    float desiredAngle = 9999;

    bool isCharacterActive;
    bool isDead;
	// Use this for initialization
	void Start ()
    {
        isDead = false;
        isCharacterActive = true;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isJumping = false;

        Turning = TurnTo(desiredAngle);
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isDead)
            return;

        if(isCharacterActive)
        {
            PlayerMovement();

            if (Input.GetKeyDown(KeyCode.C))
            {
                StopCharacter();
                isCharacterActive = false;
            }

            if(isJumping)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, -transform.up, out hit, .2f))
                {
                    if (hit.transform.tag == "Ground")
                    {
                        animator.SetBool("isJumping", false);
                        isJumping = false;
                    }
                }

                //if(Physics.Raycast(transform.position, transform.forward, out hit, .1f))
                //{
                //    rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, rb.velocity.y);
                //}

                //if (Physics.Raycast(transform.position, -transform.forward, out hit, .1f))
                //{

                //}
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCharacterActive = true;
                camScript.character = transform;
            }
        }

    }

    void PlayerMovement()
    {
        Vector3 prevVelocity = rb.velocity;

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, rb.velocity.y);

        if((int)rb.velocity.normalized.x != 0)
        {
            if (rb.velocity.normalized.x != prevVelocity.normalized.x)
            {
                desiredAngle = 90 * rb.velocity.normalized.x;

                StopCoroutine(Turning);
                Turning = TurnTo(desiredAngle);
                StartCoroutine(Turning);
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(JumpCoroutine());
        }
    }

    IEnumerator JumpCoroutine()
    {
        isJumping = true;
        animator.SetBool("isJumping", true);

        yield return new WaitForSeconds(.2f);

        rb.velocity = new Vector2(rb.velocity.x, 0);
        //Add force on the first frame of the jump
        rb.AddForce(Vector2.up * 8, ForceMode.Impulse);

        float jumpTime = .7f;
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


    IEnumerator TurnTo(float desiredAngle)
    {
        Quaternion currentRot = Quaternion.Euler(transform.eulerAngles);
        Quaternion desiredRot = Quaternion.Euler(new Vector3(0, desiredAngle, 0));

        while (Quaternion.Angle(currentRot, desiredRot) > 5)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, desiredAngle, 0), 4 * Time.deltaTime);
            currentRot = Quaternion.Euler(transform.eulerAngles);
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, desiredAngle, 0);

        yield return null;
    }


    void StopCharacter()
    {
        animator.SetBool("isMoving", false);
        rb.velocity = Vector3.zero;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            if (isJumping)
            {
                //isJumping = false;
            }
        }

        if(collision.transform.tag == "Spike")
        {
            StopCharacter();
            isDead = true;
            animator.Play("DeathAnim");
        }
    }
}
