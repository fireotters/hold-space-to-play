using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private AudioSource sfx;
    private SpriteRenderer key;
    private Collider2D triggerCollider;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        key = GetComponent<SpriteRenderer>();
        triggerCollider = GetComponent<Collider2D>();
    }

    public void DestroyKey()
    {
        key.enabled = false;
        triggerCollider.enabled = false;
        sfx.Play();
        Invoke(nameof(Die), 1f);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
