using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidScript : MonoBehaviour {

    [SerializeField] GameObject brother;
    [SerializeField] GameObject dog;
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

    bool canSwitchCharacter;
	// Use this for initialization
	void Start ()
    {
        isSpacebarPressed = false;
        isDead = false;
        isCharacterActive = true;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isJumping = false;

        Turning = TurnTo(desiredAngle);
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        canSwitchCharacter = brother != null;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (isDead)
            return;

        if(canSwitchCharacter && Input.GetKeyDown(KeyCode.F))
        {
            brother.transform.position = transform.position;
            camScript.character = brother.transform;

            if (transform.rotation.y > 0)
                brother.transform.rotation = Quaternion.Euler(0, 90, 0);
            else
                brother.transform.rotation = Quaternion.Euler(0, -90, 0);

            brother.SetActive(true);
            dog.SetActive(false);
            gameObject.SetActive(false);
            dog.GetComponent<DogScript>().stay = false;
            dog.GetComponent<DogScript>().isCharacterActive = false;
            isCharacterActive = true;
            return;
        }
        if(Input.GetKeyDown(KeyCode.Space))
            isSpacebarPressed = true;

        if(Input.GetKeyUp(KeyCode.Space))
            isSpacebarPressed = false;


        if (isCharacterActive)
        {
            PlayerMovement();

            if (Input.GetKeyDown(KeyCode.C))
            {
                StopCharacter();
                isCharacterActive = false;
                dog.GetComponent<DogScript>().isCharacterActive = true;
                camScript.character = dog.transform;
                dog.transform.position = new Vector3(dog.transform.position.x, dog.transform.position.y, transform.position.z);
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCharacterActive = true;
                camScript.character = transform;
                dog.GetComponent<DogScript>().isCharacterActive = false;

                if(!dog.GetComponent<DogScript>().stay)
                   dog.transform.position = new Vector3(dog.transform.position.x, dog.transform.position.y, dog.GetComponent<DogScript>().playerFollowPos.position.z);
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
        else if(Input.GetKey(KeyCode.A))
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

        while(currentTime < jumpTime && isSpacebarPressed)
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
        if(collision.transform.tag == "Ground" || collision.transform.CompareTag("Pickable"))
        {
            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("isJumping", false);
            }
        }

        if(collision.transform.tag == "Spike")
        {
            StopCharacter();
            isDead = true;
            camScript.character = null;
            animator.Play("DeathAnim");
        }
    }
}
