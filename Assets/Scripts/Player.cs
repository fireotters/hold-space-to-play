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
    private float movementValue = 0f;

    public bool jump = false, moveLeft = false, moveRight = false;

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
                jump = false;
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
        SetAnimatorValues(Mathf.Abs(moveDirection.x), !controller.isGrounded);

        CameraFollowPlayer();
    }

    float CalculateMovementValue()
    {
        if ((moveLeft && movementValue > -5.0f) || ((!moveRight && !moveLeft) && movementValue != 0))
        {
            movementValue -= Time.deltaTime;
        }
        else if (moveRight && movementValue < 5.0f)
        {
            movementValue += Time.deltaTime;
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

    // Checks if the sprite needs to be flipped.
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
        camera.transform.SetPositionAndRotation(new Vector3(camera.transform.position.x, gameObject.transform.position.y,  -1f), Quaternion.identity);
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
}
