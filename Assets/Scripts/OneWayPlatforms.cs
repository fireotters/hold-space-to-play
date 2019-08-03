using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatforms : MonoBehaviour
{
    [SerializeField] SpriteRenderer topTriggerRenderer, bottomTriggerRenderer;
    Collider parentCollider;

    void Awake()
    {
        parentCollider = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (topTriggerRenderer == null || bottomTriggerRenderer == null)
        {
            Debug.LogError("No triggers have been specified!");
            Destroy(gameObject);
        }

        parentCollider.enabled = false;
        topTriggerRenderer.enabled = false;
        bottomTriggerRenderer.enabled = false;
    }

    public void SetPlatformCollider(bool value)
    {
        parentCollider.enabled = value;
    }


}
