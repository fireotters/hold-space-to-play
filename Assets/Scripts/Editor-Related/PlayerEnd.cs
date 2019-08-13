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
    [SerializeField] private GameObject flag = null;

    void Awake()
    {
        if (levelToLoad == "")
        {
            Debug.LogError("You must input a scene name to load in the Inspector!");
        }
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        fadeBlack = GameObject.FindObjectOfType<Canvas>().GetComponent<UI_Script>().fadeBlack;
        sprite.enabled = false;

        // Spawns a player.
        if (flag != null)
        {
            Instantiate(flag, gameObject.transform.position, Quaternion.identity);
        }
        else // This error shouldn't appear, but it's better to be safe than sorry.
        {
            Debug.LogError("You must set the Player prefab in the Inspector!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(FadeBlack("to"));
            Invoke("TriggerEndLevel", 5f);
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
        yield return new WaitForSeconds(3f);
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
