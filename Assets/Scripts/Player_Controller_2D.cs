﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller_2D : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isGrounded = false;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundedRadius = 0f;
    [SerializeField] private LayerMask whatIsGround;

    void Start()
    {
    }

    void FixedUpdate()
    {
        isGrounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                // TODO find a way to disallow double jumps inside oneway platforms
                isGrounded = true;
            }
        }
    }


}
