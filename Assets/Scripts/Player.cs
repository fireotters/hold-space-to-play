using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0, jumpSpeed = 0, gravity = 0;

    private CharacterController controller;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private Camera camera;
    private float movementValue = 0f, baseSpeed = 1.5f;
    public bool jump = false;
    private bool moveLeft = false, moveRight = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        camera = FindObjectOfType<Camera>();
    }

    void Start()
    {
        CameraFollowPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        // What counts as "grounded"?
        // Any **3D COLLIDER** + 3D Rigidbody counts as ground. Even in 2D games. 
        // Thank you Cross you're my saviour <3
        if (controller.isGrounded)
        {
            // We obtain what direction is being inputed, and we multiply that value by the speed multiplier.
            moveDirection = new Vector2(CalculateMovementValue(), 0.0f);
            moveDirection *= speedMultiplier;

            // If the jump boolean is true, player jumps.
            if (jump)
            {
                Jump();
            }
        }

        // Gravity gets calculated. It applies at all times to the player.
        moveDirection.y -= gravity * Time.deltaTime;

        // We move the player.
        controller.Move(moveDirection * Time.deltaTime);

        // If it collides with the ceiling, the character bonks off it.
        if (controller.collisionFlags == CollisionFlags.Above)
        {
            moveDirection.y = 0;
        }
        
        CheckFlipCharacter(moveDirection.x);
        // Animate player.
        SetAnimatorValues(Mathf.Abs(moveDirection.x), jump);

        // The jump falsify statement was moved from where it was, so that SetAnimatorValues can use jump as a bool
        if (jump)
        {
            jump = false;
        }

        CameraFollowPlayer();
    }

    /// <summary>
    /// Calculates the movement value needed to be fed into the Vector2 used
    /// for moving the CharacterController.
    /// </summary>
    /// <returns>The horizontal Movement value.</returns>
    float CalculateMovementValue()
    {
        // If moveLeft == true and the movementValue is bigger than the negative base speed
        // we subtract deltaTime to movementValue
        if (moveLeft && movementValue >= -baseSpeed)
        {
            movementValue -= Time.deltaTime;
        }
        // If moveRight == true and the movement is lower than the base speed, we add deltaTime
        // to movementValue
        else if (moveRight && movementValue <= baseSpeed)
        {
            movementValue += Time.deltaTime;
        }
        // fuck my life and fuck everything this section took way too much of time to add other things to the game
        // If both moveRight and moveLeft == false and the Space key is not being pressed
        // depending on if the movementValue is positive or negative, we start subtracting or adding
        // deltaTime to the value. Once the value reaches 0.1 levels of precision, we just set the value to 0,
        // since deltaTime never ends up returning plain zero.
        else if ((!moveRight && !moveLeft) && !Input.GetKey(KeyCode.Space))
        {
            if (movementValue > 0.1f)
            {
                movementValue -= Time.deltaTime;
            }
            else if (movementValue < -0.1f)
            {
                movementValue += Time.deltaTime;
            }
            else if (movementValue <= 0.1f || movementValue >= -0.1f)
            {
                movementValue = 0;
            }
        }

        return movementValue;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TopTrigger")
        {
            SetOneWayPlatformCollisionTo(other, true);
        }
        else if (other.tag == "BottomTrigger")
        {
            SetOneWayPlatformCollisionTo(other, false);
        }
    }

    /// <summary>
    /// This function tells the animator what values to set to its variables.
    /// These variables are defined in Unity and they change the animation being played.
    /// 
    /// TODO: Re-structure this function and how the parameters are being fed in.
    /// Now there are only two animator variables but with way too many this would get out of hand.
    /// </summary>
    /// <param name="speedValue"></param>
    /// <param name="isJumping"></param>
    void SetAnimatorValues(float speedValue, bool isJumping)
    {
        animator.SetFloat("Speed", speedValue);
        animator.SetBool("IsJumping", isJumping);
    }

    /// <summary>
    /// Checks if the sprite needs to be flipped.
    /// </summary>
    /// <param name="xvalue">x value of player controller</param>    
    void CheckFlipCharacter(float xvalue)
    {
        if (xvalue > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
        }
        else if (xvalue < 0 && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// Whenever called, the player jumps.
    /// </summary>
    void Jump()
    {
        if (controller.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
    }

    /// <summary>
    /// Sets the camera position to look wherever the player is vertically speaking.
    /// </summary>
    void CameraFollowPlayer()
    {
        camera.transform.SetPositionAndRotation(new Vector3(camera.transform.position.x, gameObject.transform.position.y + 1f,  -1f), Quaternion.identity);
    }

    /// <summary>
    /// Enables or disables the collider of the target one way platform.
    /// </summary>
    /// <param name="collider">One Way Platform's trigger collider.</param>
    /// <param name="value">Value we want to set.</param>
    void SetOneWayPlatformCollisionTo(Collider collider, bool value)
    {
        OneWayPlatforms platform = collider.gameObject.GetComponentInParent<OneWayPlatforms>();

        platform.SetPlatformCollider(value);
    }

    /// <summary>
    /// Sets the direction of the player 
    /// </summary>
    /// <param name="direction">What direction the player should go to.</param>
    public void SetPlayerDirection(string direction)
    {
        if (direction == "left")
        {
            moveLeft = true;
            moveRight = false;
            movementValue = 0f;
        }
        else if (direction  == "right")
        {
            moveRight = true;
            moveLeft = false;
            movementValue = 0f;
        }
        else if (direction == "stop")
        {
            moveRight = false;
            moveLeft = false;
        }

    }
}
