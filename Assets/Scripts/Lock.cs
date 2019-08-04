using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    private AudioSource sfx;
    private Animator animator;
    private SpriteRenderer lockSprite;
    [SerializeField] private Collider lockCollider = null;

    void Awake()
    {
        lockSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        if (lockCollider == null)
        {
            Debug.LogError("You need to specify the collision box collider in the Inspector!");
        }
    }

    public void DestroyLock()
    {
        lockCollider.enabled = false;
        sfx.Play();
        animator.SetBool("Die", true);
        Invoke("Die", 0.51f);
        
    }

    void Die()
    {
        lockSprite.enabled = false;
        Destroy(gameObject);
    }

    
}
