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
        // Any **3D COLLIDER** counts as ground. Even in 2D games. Thank you Cross you're my saviour <3
        if (controller.isGrounded)
        {
            // We obtain what direction is being inputed, and we multiply that value by the speed multiplier.
            moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0.0f);
            moveDirection *= speedMultiplier;

            // If the Space key is pressed, player jumps.
            if (Input.GetKeyDown(KeyCode.Space))
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
        SetAnimatorValues(Mathf.Abs(moveDirection.x), !controller.isGrounded);

        CameraFollowPlayer();
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
    /// 
    /// </summary>
    void CameraFollowPlayer()
    {
        camera.transform.SetPositionAndRotation(new Vector3(0f, gameObject.transform.position.y,  -1f), Quaternion.identity);
    }
}
