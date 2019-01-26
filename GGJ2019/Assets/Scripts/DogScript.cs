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
    float desiredAngle;

    bool isCharacterActive;
    Transform playerFollowPos;
    CameraScript camScript;

    // Use this for initialization
    void Start ()
    {
        isCharacterActive = false;
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        isJumping = false;
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();

        playerFollowPos = GameObject.Find("Kid").transform.Find("FollowPos");
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(isCharacterActive)
        {
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
            transform.position = Vector3.Lerp(transform.position, playerFollowPos.position, 100 * Time.deltaTime);

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
        if (Vector3.Distance(transform.position, playerFollowPos.parent.position) > 5)
            return false;

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

        //if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        //    animator.SetBool("isMoving", true);
        //else
        //    animator.SetBool("isMoving", false);

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

        float jumpTime = .7f;
        float currentTime = 0;

        while (currentTime < jumpTime)
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
        }
    }
}
