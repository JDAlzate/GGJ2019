using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpTime;

    Rigidbody rb;
    Animator animator;
    CameraScript camScript;
    bool isJumping;

    IEnumerator Turning;
    float desiredAngle = 9999;

    bool isCharacterActive;
    bool isDead;
    bool isSpacebarPressed;
    // Use this for initialization
    void Start()
    {
        isSpacebarPressed = false;
        isDead = false;
        isCharacterActive = true;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isJumping = false;

        Turning = TurnTo(desiredAngle);
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            isSpacebarPressed = true;

        if (Input.GetKeyUp(KeyCode.Space))
            isSpacebarPressed = false;


        if (isCharacterActive)
        {
            PlayerMovement();

            if (Input.GetKeyDown(KeyCode.C))
            {
                StopCharacter();
                isCharacterActive = false;
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

        while (currentTime < jumpTime && (isSpacebarPressed || currentTime < .2f))
        {
            currentTime += Time.deltaTime;

            float verticalVel = Mathf.Lerp(jumpSpeed, 0, currentTime / jumpTime);
            rb.velocity = new Vector2(rb.velocity.x, verticalVel);

            yield return null;
        }

        while (isJumping)
        {

            float verticalVel = Mathf.Lerp(rb.velocity.y, -10, 2 * Time.deltaTime);
            rb.velocity = new Vector2(rb.velocity.x, verticalVel);

            yield return null;
        }


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
