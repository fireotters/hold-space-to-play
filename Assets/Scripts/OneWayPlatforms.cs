using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatforms : MonoBehaviour
{
    [SerializeField] private Collider parentCollider;
    SpriteRenderer triggerRenderer;

    void Awake()
    {
        
        triggerRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        parentCollider.enabled = false;
        triggerRenderer.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlatformTrigger")
        {
            parentCollider.enabled = true;
        }
    }
}
