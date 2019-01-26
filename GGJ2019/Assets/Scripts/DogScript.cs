using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogScript : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpSpeed;
    float desiredAngle;

    bool isJumping;
    public bool isCharacterActive;
    public bool stay;

    Rigidbody rb;
    Animator animator;
    Transform playerFollowPos;
    CameraScript camScript;

    IEnumerator Turning;
    [SerializeField] bool isSpacebarPressed;
    [SerializeField] float jumpTime;

    bool isDead;

    // Use this for initialization
    void Start ()
    {
        isDead = false;
        stay = false;
        isCharacterActive = false;
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        isJumping = false;
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();

        playerFollowPos = GameObject.Find("Kid").transform.Find("FollowPos");
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (isDead)
            return;
        if (Input.GetKeyDown(KeyCode.S))
        {
            stay = !stay;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            rb.velocity = Vector3.zero;
        }

        if (isCharacterActive)
        {
            if(!stay)
                if (!PlayerMovement())
                    StopCharacter();

            if (Input.GetKeyDown(KeyCode.C))
            {
                isCharacterActive = false;

                transform.position = new Vector3(transform.position.x, transform.position.y, .1f);
            }

            if (isJumping)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, -transform.up, out hit, .2f))
                {
                    Debug.Log(hit.transform.tag);
                    if (hit.transform.tag == "Ground")
                    {
                        isJumping = false;
                    }
                }
            }
        }
        else
        {
            if(!stay)
            {
                Vector3 newVec = Vector3.zero;
                transform.position = Vector3.SmoothDamp(transform.position, playerFollowPos.position, ref newVec, .1f, movementSpeed);
                rb.velocity = newVec;

                //transform.position = Vector3.Lerp(transform.position, playerFollowPos.position, 3 * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                camScript.character = transform;
                isCharacterActive = true;
                camScript.character = transform;

                transform.position = new Vector3(transform.position.x, transform.position.y, -.1f);
            }
        }
    }

    bool PlayerMovement()
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

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(JumpCoroutine());
        }

        return true;
    }

    IEnumerator JumpCoroutine()
    {
        isJumping = true;

        yield return new WaitForSeconds(.2f);

        rb.velocity = new Vector2(rb.velocity.x, 0);
        //Add force on the first frame of the jump
        rb.AddForce(Vector2.up * 8, ForceMode.Impulse);

        float currentTime = 0;

        while (currentTime < jumpTime && isSpacebarPressed)
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

    void StopCharacter()
    {
        //animator.SetBool("isMoving", false);
        //rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground" && isJumping)
        {
            isJumping = false;
            camScript.character = null;
        }

        if (collision.transform.tag == "Spike" )
        {
            if(isCharacterActive)
            {
                isDead = true;
                rb.velocity = Vector3.zero;
            }
            else
            {
                transform.position = playerFollowPos.position;
            }
        }
    }
}
