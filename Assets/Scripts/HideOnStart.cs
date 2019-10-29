using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    SpriteRenderer platformRenderer;

    void Awake()
    {
        platformRenderer = GetComponent<SpriteRenderer>();    
    }

    void Start()
    {
        platformRenderer.enabled = false;   
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.K)) {
            renderer.enabled = !renderer.enabled;
        }*/
    }
}
