using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private AudioSource sfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    public void DestroyBehaviour()
    {
        sfx.Play();
        Invoke("Die", 0.2f);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
