using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrotherController : MonoBehaviour {

    Rigidbody rigidbody;

    Animator animator;

    [SerializeField]
    float speed;

    [SerializeField]
    float carryingSpeed;

    [SerializeField]
    Transform camTransform;

    float move;

    [SerializeField]
    float jumpForce;

    [SerializeField]
    float deathSpeed;

    bool isGrounded;

    bool isMoving;

    bool isCarrying;

    [SerializeField]
    int keyCount;

    GameObject carryItem;

    float startJumpPositionY;

    Transform raycastTransform;

    [SerializeField]
    Vector3 point0;

    [SerializeField]
    Vector3 point1;

    [SerializeField]
    float debugRadius = 0.6;

    // Use this for initialization
    void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        raycastTransform = transform.Find("Raycast Position").transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        move = Input.GetAxisRaw("Horizontal");
        isMoving = move != 0;
        animator.SetBool("run", isMoving);
        animator.SetBool("carry", isCarrying);

        if (!isCarrying)
        {
            //Collider[] colliders = Physics.OverlapCapsule(raycastTransform.position + point0, raycastTransform.position + point1, debugRadius);
            Collider[] colliders = Physics.OverlapSphere(raycastTransform.position, debugRadius);
            foreach (Collider c in colliders)
            {
                if (!c.CompareTag("Player") && !c.isTrigger)
                {
                    if (c.transform.position.x > transform.position.x)
                    {
                        if (move > 0)
                            move = 0;
                    }

                    else
                    {
                        if (move < 0)
                            move = 0;
                    }
                }

            }
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

            if(Input.GetMouseButtonDown(0) && isGrounded)
            {
                
                Debug.DrawLine(raycastTransform.position, raycastTransform.position + raycastTransform.forward * 0.7f, Color.blue, 50);

                RaycastHit hit;
                if(Physics.Raycast(raycastTransform.position, raycastTransform.forward, out hit, 0.7f))
                {
                    if(hit.transform.CompareTag("Pickable"))
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

        camTransform.position = new Vector3(transform.position.x, transform.position.y, camTransform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ground"))
        {
            if(startJumpPositionY - transform.position.y >= deathSpeed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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
            startJumpPositionY = transform.position.y;
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

    private void OnDrawGizmosSelected()
    {
       
    }

    public int GetKeyCount()
    {
        return keyCount;
    }

    public void UseKey()
    {
        keyCount--;
    }
}
