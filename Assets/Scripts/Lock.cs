using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    private AudioSource sfx;
    private Animator animator;
    private SpriteRenderer lockSprite;
    [SerializeField] private Collider2D lockSolidCollider = null;
    public bool unlockable = true;

    void Awake()
    {
        lockSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        if (lockSolidCollider == null)
        {
            Debug.LogError("You need to specify the SOLID (not trigger) collision box in the Inspector!");
        }
    }

    public void DestroyLock()
    {
        lockSolidCollider.enabled = false;
        unlockable = false;
        sfx.Play();
        animator.SetBool("Die", true);
        Invoke(nameof(Die), 0.5f);
        
    }

    void Die()
    {
        lockSprite.enabled = false;
        Invoke(nameof(ActuallyDie), 2f);
    }

    void ActuallyDie()
    {
        Destroy(gameObject);
    }

    
}
