using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    GameObject stayIndicator;

    public Transform playerFollowPos;
    public CameraScript camScript;

    IEnumerator Turning;
    [SerializeField] bool isSpacebarPressed;
    [SerializeField] float jumpTime;

    bool isDead;

    public delegate void MyDelegate();
    public event MyDelegate onDeath;
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

        stayIndicator = transform.Find("Wait").gameObject;
        stayIndicator.SetActive(false);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Respawn();
        }

        if (isDead)
            return;

        if (Input.GetKeyDown(KeyCode.S))
        {
            stay = !stay;
            stayIndicator.SetActive(stay);

            if (stay)
                transform.position = new Vector3(transform.position.x, transform.position.y, playerFollowPos.parent.position.z);
            else if(!isCharacterActive)
                transform.position = new Vector3(transform.position.x, transform.position.y, playerFollowPos.position.z);


            rb.velocity = Vector3.zero;
        }

        if (isCharacterActive && !stay)
            PlayerMovement();

        else if (!stay)
        {
            Vector3 newVec = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, playerFollowPos.position, ref newVec, .1f, movementSpeed);
            rb.velocity = newVec;
        }
    }

    bool PlayerMovement()
    {
        Vector3 prevVelocity = rb.velocity;
        
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(JumpCoroutine());
        }

        return true;
    }

    IEnumerator JumpCoroutine()
    {
        isJumping = true;

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
        }

        if (collision.transform.tag == "Spike" )
        {
            if(isCharacterActive)
            {
                isDead = true;
                camScript.character = null;
                rb.velocity = Vector3.zero;
            }
            else
            {
                transform.position = playerFollowPos.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.0f);

        if (onDeath != null)
            onDeath.Invoke();
    }
}
