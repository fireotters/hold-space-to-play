using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    SpriteRenderer renderer;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();    
    }

    void Start()
    {
        renderer.enabled = false;   
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            renderer.enabled = !renderer.enabled;
        }
    }
}
