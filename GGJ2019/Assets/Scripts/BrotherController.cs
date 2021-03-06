﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrotherController : MonoBehaviour
{
    Rigidbody rigidBody;
    Animator animator;
    CameraScript camScript;
    bool isJumping;

    [SerializeField] Transform camTransform;
    [SerializeField] float speed;
    [SerializeField] float carryingSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpTime;

    float move;
    bool isGrounded;
    bool isMoving;
    bool isCarrying;

    bool isCharacterActive;
    bool isDead;
    bool isSpacebarPressed;

    bool canSwitchCharacter;

    [SerializeField] int keyCount;

    [SerializeField] GameObject kid;
    [SerializeField] GameObject dog;

    GameObject carryItem;
    Transform raycastTransform;

    IEnumerator Turning;
    float desiredAngle = 0;

    public delegate void MyDelegate();
    public event MyDelegate onDeath;

    // Use this for initialization
    void Start()
    {
        isSpacebarPressed = false;
        isDead = false;
        isCharacterActive = true;
        isJumping = false;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        raycastTransform = transform.Find("Raycast Position").transform;
        Turning = TurnTo(desiredAngle);
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        canSwitchCharacter = kid != null && dog != null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            isDead = true;
            animator.Play("Death");
            StartCoroutine(Respawn());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (isDead)
            return;

        if (canSwitchCharacter && Input.GetKeyDown(KeyCode.F))
        {
            kid.transform.position = transform.position;
            camScript.character = kid.transform;

            if (transform.rotation.y > 0)
                kid.transform.rotation = Quaternion.Euler(0, 90, 0);
            else
                kid.transform.rotation = Quaternion.Euler(0, -90, 0);

            dog.transform.position = transform.position;

            kid.SetActive(true);
            dog.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            isSpacebarPressed = true;
        if (Input.GetKeyUp(KeyCode.Space))
            isSpacebarPressed = false;

        Vector3 prevVelocity = rigidBody.velocity;

        move = Input.GetAxisRaw("Horizontal");

        isMoving = (move != 0);
        animator.SetBool("run", isMoving);
        animator.SetBool("carry", isCarrying);

        if (isMoving && !isCarrying)
        {
            desiredAngle = 90 * move;

            StopCoroutine(Turning);
            Turning = TurnTo(desiredAngle);
            StartCoroutine(Turning);
        }

        if (!isCarrying)
        {
            rigidBody.velocity = new Vector2(move * speed * Time.deltaTime, rigidBody.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                StartCoroutine(JumpCoroutine());
            }

            if (Input.GetMouseButtonDown(0) && !isJumping)
            {
                Debug.DrawLine(raycastTransform.position, raycastTransform.position + raycastTransform.forward * 0.7f, Color.blue, 50);

                RaycastHit hit;
                if (Physics.Raycast(raycastTransform.position, raycastTransform.forward, out hit, 0.7f))
                {
                    if (hit.transform.CompareTag("Pickable"))
                    {
                        if (transform.rotation.y > 0)
                            transform.rotation = Quaternion.Euler(0, 90, 0);
                        if (transform.rotation.y < 0)
                            transform.rotation = Quaternion.Euler(0, -90, 0);

                        carryItem = hit.transform.gameObject;
                        carryItem.transform.parent = transform;
                        isCarrying = true;
                    }
                }
            }
        }

        else if (isCarrying)
        {
            rigidBody.velocity = new Vector2(move * carryingSpeed * Time.deltaTime, rigidBody.velocity.y);

            if (Input.GetMouseButtonUp(0) && !isJumping)
            {
                carryItem.transform.parent = null;
                carryItem = null;
                isCarrying = false;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Pickable"))
        {
            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("jump", false);
            }
        }

        if (collision.transform.CompareTag("Key"))
        {
            keyCount++;
            Destroy(collision.gameObject);
        }

        if (collision.transform.CompareTag("Spike"))
        {
            StopCharacter();
            isDead = true;
            //animator.Play("DeathAnim");
            camScript.character = null;
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

    IEnumerator JumpCoroutine()
    {
        isJumping = true;
        animator.SetBool("jump", true);

        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        //Add force on the first frame of the jump
        rigidBody.AddForce(Vector2.up * 8, ForceMode.Impulse);

        float currentTime = 0;

        while (currentTime < jumpTime && isSpacebarPressed)
        {
            currentTime += Time.deltaTime;

            float verticalVel = Mathf.Lerp(jumpSpeed, 0, currentTime / jumpTime);
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, verticalVel);

            yield return null;
        }

        rigidBody.velocity = new Vector2(rigidBody.velocity.x, -2);

        yield return new WaitUntil(() => !isJumping);

        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

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
        rigidBody.velocity = Vector3.zero;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.2f);

        if (onDeath != null)
            onDeath.Invoke();
        animator.Play("Idle");
        isDead = false;
    }
}
