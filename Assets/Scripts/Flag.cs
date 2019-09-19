using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private AudioSource sfx;
    private Animator animator;
    public bool levelEnding = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
    }

    public void FlagAnim()
    {
        if (!animator.GetBool("LevelOver"))
        {
            animator.SetBool("LevelOver", true);
            levelEnding = true;
            sfx.Play();
        }
    }
}
