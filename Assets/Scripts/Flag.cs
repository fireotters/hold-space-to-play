using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private AudioSource sfx;
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
    }

    public void FlagAnim()
    {
        animator.SetBool("LevelOver", true);
        sfx.Play();
    }
}
