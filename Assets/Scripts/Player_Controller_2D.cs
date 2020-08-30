using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller_2D : MonoBehaviour
{
    public bool isGrounded = false;

    // Disable warning CS0649 (never assigned to). These are serialized fields that are assigned in Unity.
    #pragma warning disable CS0649
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    #pragma warning restore CS0649

    [SerializeField] private float groundedRadius = 0f;


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
                isGrounded = true;
            }
        }
    }


}
