using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Variables
    [SerializeField] private float speedMultiplier = 0, jumpSpeed = 0;
    [SerializeField] private Text keyCounter;

    public Player_Controller_2D controller;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveDirection = Vector2.zero;
    private new Camera camera;
    private float movementValue = 0f, baseSpeed = 1.5f;
    public bool jump = false;
    private bool moveLeft = false, moveRight = false;
    private int amountOfKeys = 0;
    #endregion

    #region Unity methods
    void Awake()
    {
        controller = GetComponent<Player_Controller_2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        camera = FindObjectOfType<Camera>();
        keyCounter = FindObjectOfType<Text>();
    }

    void Start()
    {
        CameraFollowPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            // We obtain what direction is being inputed, and we multiply that value by the speed multiplier.
            moveDirection = new Vector2(CalculateMovementValue(), 0f);
            moveDirection *= speedMultiplier;
            // If the jump boolean is true, player jumps.
            if (jump)
            {
                moveDirection.y = 20000f;
            }
        }

        // We move the player.
        controller.Move(moveDirection * Time.deltaTime);

        /*
        // If it collides with the ceiling, the character bonks off it.
        if (controller.collisionFlags == CollisionFlags.Above)
        {
            moveDirection.y = 0;
        }*/
        
        // Flip sprite depending on move direction
        CheckFlipCharacter(moveDirection.x);

        // Animate player and falsify jump boolean for next frame
        SetAnimatorValues(Mathf.Abs(moveDirection.x), jump);
        if (jump) { jump = false; }

        // Set camera to center on the character
        CameraFollowPlayer();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print(other);
        switch (other.tag)
        {
            case "TopTrigger":
                SetOneWayPlatformCollisionTo(other, true);
                break;
            case "BottomTrigger":
                SetOneWayPlatformCollisionTo(other, false);
                break;
            case "Key":
                amountOfKeys++;
                UpdateKeyCounter();
                Key key = other.gameObject.GetComponent<Key>();
                key.DestroyKey();
                break;
            case "Lock":
                if (amountOfKeys > 0)
                {
                    amountOfKeys--;
                    UpdateKeyCounter();
                    Lock lockObject = other.gameObject.GetComponent<Lock>();
                    lockObject.DestroyLock();
                }
                break;
            case "Flag":
                {
                    Flag flagObject = other.gameObject.GetComponent<Flag>();
                    flagObject.FlagAnim();
                    movementValue = 0;
                }
                break;
        }
    }
    #endregion

    #region Other methods
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
    /// Sets the camera position to look wherever the player is vertically speaking.
    /// </summary>
    void CameraFollowPlayer()
    {
        camera.transform.SetPositionAndRotation(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f,  -1f), Quaternion.identity);
    }

    /// <summary>
    /// Enables or disables the collider of the target one way platform.
    /// </summary>
    /// <param name="collider">One Way Platform's trigger collider.</param>
    /// <param name="value">Value we want to set.</param>
    void SetOneWayPlatformCollisionTo(Collider2D collider, bool value)
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
        else if (direction == "right")
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

    void UpdateKeyCounter()
    {
        keyCounter.text = amountOfKeys.ToString();
    }

    #endregion
}
