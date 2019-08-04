using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerEnd : MonoBehaviour
{
    private SpriteRenderer sprite;
    public GameObject fadeBlack;
    [SerializeField] private string levelToLoad;

    void Awake()
    {
        if (levelToLoad == null)
        {
            Debug.LogError("You must input a scene name to load in the Inspector!");
        }
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        sprite.enabled = false;    
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(FadeBlack("to"));
            Invoke("TriggerEndLevel", 1f);
        }
    }

    void TriggerEndLevel()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    // Other methods
    public IEnumerator FadeBlack(string ToOrFrom)
    {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 1.2f;
        float fadingAlpha;
        fadeBlack.SetActive(true);
        if (ToOrFrom == "from")
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f)
            {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            fadeBlack.SetActive(false);
        }
        else if (ToOrFrom == "to")
        {
            fadingAlpha = 0f;
            while (fadingAlpha < 1f)
            {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }
}
