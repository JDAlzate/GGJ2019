using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrotherController : MonoBehaviour {

    Rigidbody rigidbody;
    Animator animator;
    CameraScript camScript;
    bool isJumping;

    [SerializeField] Transform camTransform;
    [SerializeField] float speed;
    [SerializeField] float carryingSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float deathSpeed;

    float move;
    bool isGrounded;
    bool isMoving;
    bool isCarrying;

    bool isCharacterActive;
    bool isDead;
    bool isSpacebarPressed;

    [SerializeField] int keyCount;

    GameObject carryItem;
    Transform raycastTransform;

    IEnumerator Turning;
    float desiredAngle = 0;

    // Use this for initialization
    void Start ()
    {
        isSpacebarPressed = false;
        isDead = false;
        isCharacterActive = true;
        isJumping = false;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        raycastTransform = transform.Find("Raycast Position").transform;
        Turning = TurnTo(desiredAngle);
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isDead)
            return;

        if (Input.GetKey(KeyCode.Space))
            isSpacebarPressed = true;
        else
            isSpacebarPressed = false;

        if (isCharacterActive)
        {
            Vector3 prevVelocity = rigidbody.velocity;

            move = Input.GetAxisRaw("Horizontal");
            rigidbody.velocity = new Vector2(move * speed * Time.deltaTime, rigidbody.velocity.y);

            isMoving = (move != 0);
            animator.SetBool("run", isMoving);
            animator.SetBool("carry", isCarrying);
            
            if(isMoving)
            {
                desiredAngle = 90 * move;

                StopCoroutine(Turning);
                Turning = TurnTo(desiredAngle);
                StartCoroutine(Turning);
            }

            // MY CODE START
            isMoving = move != 0;
            animator.SetBool("run", isMoving);

            if (!isCarrying)
            {
                isMoving = move != 0;
                rigidbody.velocity = new Vector2(move * speed, rigidbody.velocity.y);

                if (isMoving)
                {
                    transform.rotation = move > 0 ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);
                }

                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    rigidbody.AddForce(new Vector2(0, jumpForce));

                    animator.SetBool("jump", true);
                }

                if (Input.GetMouseButtonDown(0) && isGrounded)
                {

                    Debug.DrawLine(raycastTransform.position, raycastTransform.position + raycastTransform.forward * 0.7f, Color.blue, 50);

                    RaycastHit hit;
                    if (Physics.Raycast(raycastTransform.position, raycastTransform.forward, out hit, 0.7f))
                    {
                        if (hit.transform.CompareTag("Pickable"))
                        {
                            carryItem = hit.transform.gameObject;
                            carryItem.transform.parent = transform;
                            isCarrying = true;
                        }
                    }
                }
            }

            else if (isCarrying)
            {
                rigidbody.velocity = new Vector2(move * carryingSpeed, rigidbody.velocity.y);

                if (Input.GetMouseButtonUp(0) && isGrounded)
                {
                    carryItem.transform.parent = null;
                    carryItem = null;
                    isCarrying = false;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("jump", false);
            animator.SetBool("grounded", true);
        }

        if(collision.transform.CompareTag("Key"))
        {
            keyCount++;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("grounded", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            keyCount++;
            Destroy(other.gameObject);
        }
    }

    public int GetKeyCount()
    {
        return keyCount;
    }

    public void UseKey()
    {
        keyCount--;
    }

    void PlayerMovement()
    {
        Vector3 prevVelocity = rb.velocity;

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, rb.velocity.y);

        if ((int)rb.velocity.normalized.x != 0)
        {
            if (rb.velocity.normalized.x != prevVelocity.normalized.x)
            {
                //desiredAngle = 90 * rb.velocity.normalized.x;

                //StopCoroutine(Turning);
                //Turning = TurnTo(desiredAngle);
                //StartCoroutine(Turning);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isMoving", true);
            desiredAngle = 90;

            StopCoroutine(Turning);
            Turning = TurnTo(desiredAngle);
            StartCoroutine(Turning);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isMoving", true);
            desiredAngle = -90;

            StopCoroutine(Turning);
            Turning = TurnTo(desiredAngle);
            StartCoroutine(Turning);
        }
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
        if (collision.transform.tag == "Ground")
        {
            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("isJumping", false);
            }
        }

        if (collision.transform.tag == "Spike")
        {
            StopCharacter();
            isDead = true;
            animator.Play("DeathAnim");
        }
    }
}
