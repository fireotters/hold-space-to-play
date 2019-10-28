﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Variables
    [SerializeField] private float speedMultiplier = 0, jumpForce = 3f;
    [SerializeField] private Text keyCounter;

    private Rigidbody2D rb;
    public Player_Controller_2D controller;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float moveDirection;
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
        rb = GetComponent<Rigidbody2D>();
        camera = FindObjectOfType<Camera>();

        // Find key counter text object
        var listOfTextObjects = FindObjectsOfType<Text>();
        foreach (Text textObject in listOfTextObjects)
        {
            if (textObject.transform.name == "KeyCounter (don't rename)")
            {
                keyCounter = textObject;
                break;
            }
        }
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
            moveDirection = CalculateMovementValue() * speedMultiplier;
        }
        else
        {
            // Or falsify jump boolean if the player is not grounded.
            jump = false;
        }
        
        // Flip sprite depending on move direction
        CheckFlipCharacter(moveDirection);

        // Animate player
        SetAnimatorValues(Mathf.Abs(moveDirection), jump);

        // Set camera to center on the character
        CameraFollowPlayer();
    }

    /// <summary>
    /// Physics updates occur here.
    /// Movement to the left and right are dictated by a change of velocity.
    /// Jumping is achieved by adding an upward force.
    /// </summary>
    private void FixedUpdate()
    {
        if (controller.isGrounded)
        {
            if (jump)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                jump = false;
            }
        }
        rb.velocity = new Vector2(moveDirection, rb.velocity.y);
    }

    /// <summary>
    /// Decides upon action to take if a trigger is entered by the Player.
    /// </summary>
    /// <param name="other">Colliding object with Player</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        //print(other);
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
                if (amountOfKeys > 0 && other.gameObject.GetComponent<Lock>().unlockable)
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
    /// Sets the camera position to look wherever the player is, both vertically and horizontally
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
