using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatforms : MonoBehaviour
{
    [SerializeField] SpriteRenderer topTriggerRenderer, bottomTriggerRenderer;
    Collider2D parentCollider;

    void Awake()
    {
        parentCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // If the triggers are not present, error out.
        if (topTriggerRenderer == null || bottomTriggerRenderer == null)
        {
            Debug.LogError("No triggers have been specified!");
            Destroy(gameObject);
        }

        // Disables the platform's collider and the trigger sprites.
        parentCollider.enabled = false;
        topTriggerRenderer.enabled = false;
        bottomTriggerRenderer.enabled = false;
    }

    /// <summary>
    /// Enables or disables the platform's collider.
    /// </summary>
    /// <param name="value">Whether true of false, depending if we want it active or not.</param>
    public void SetPlatformCollider(bool value)
    {
        parentCollider.enabled = value;
    }


}
