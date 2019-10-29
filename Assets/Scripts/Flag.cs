using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private AudioSource sfx;
    private Animator animator;
    private PlayerEnd playerEnd;
    public bool levelEnding = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        playerEnd = FindObjectOfType<PlayerEnd>();
    }

    public void FlagAnim()
    {
        if (!animator.GetBool("LevelOver"))
        {
            animator.SetBool("LevelOver", true);
            levelEnding = true;
            sfx.Play();
            playerEnd.StartEndingLevel();
        }
    }
}
