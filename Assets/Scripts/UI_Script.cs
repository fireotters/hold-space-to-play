using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Script : MonoBehaviour
{
    public int choiceOfMusic;
    public Image outlineImg, leftArrowImg, upArrowImg, rightArrowImg, pauseImg;
    private string currentOutline;

    float startHoldTime = 0f;
    float timeTapToChange = 0.2f;
    float timeHoldToActivate = 0.5f;
    float timeBetweenTapAndHold = 0.3f; // Set to (timeHoldToActivate - timeTapToChange)
    bool cancelling = false;
    bool beingHeld = false;
    public GameObject fadeBlack;

    private Player player;
    private MusicManager musicManager;
    private GameObject baseUI;

    void Start()
    {
        baseUI = gameObject.transform.GetChild(0).gameObject;
        baseUI.SetActive(true);
        musicManager = GameObject.FindObjectOfType<MusicManager>();
        if (musicManager)
        {
            musicManager.ChangeMusicTrack(choiceOfMusic);
        }
        outlineImg.transform.position = upArrowImg.transform.position;
        currentOutline = "UpArrow";
        player = FindObjectOfType<Player>();
        StartCoroutine(FadeBlack("from"));
    }

    void Update()
    {
        if (player == null) //Because Unity is the worst
        {
            player = FindObjectOfType<Player>();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(FadeBlack("to"));
            Invoke("ExitLevel", 1f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startHoldTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            beingHeld = false;
            player.SetPlayerDirection("stop");
            if (startHoldTime + timeTapToChange >= Time.time)
            {
                ChangeSelection();
            }
            else
            {
                cancelling = true;
            }
        }
        // If the current action is not being cancelled, then fill the selection box.
        if (Input.GetKey(KeyCode.Space) && !cancelling)
        {
            if (beingHeld)
            {
                KeepSelectionBoxFilled();
            }
            else if (startHoldTime + timeTapToChange < Time.time)
            {
                FillBoxOfSelection();
            }
            if (startHoldTime + timeHoldToActivate <= Time.time && !beingHeld)
            {
                ActivateSelection();
                beingHeld = true;
            }
        }
        else
        {
            DecreaseAllFillBoxes();
        }
    }

    private void ChangeSelection()
    {
        switch (currentOutline)
        {
            case "LeftArrow":
                outlineImg.transform.position = upArrowImg.transform.position;
                currentOutline = "UpArrow";
                break;
            case "UpArrow":
                outlineImg.transform.position = rightArrowImg.transform.position;
                currentOutline = "RightArrow";
                break;
            case "RightArrow":
                outlineImg.transform.position = leftArrowImg.transform.position;
                currentOutline = "LeftArrow";
                break;
        }
    }

    private void ActivateSelection()
    {
        switch (currentOutline)
        {
            case "LeftArrow":
                player.SetPlayerDirection("left");
                break;
            case "UpArrow":
                player.jump = true;
                break;
            case "RightArrow":
                player.SetPlayerDirection("right");
                break;
        }
    }
    private void KeepSelectionBoxFilled()
    {
        switch (currentOutline)
        {
            case "LeftArrow":
                leftArrowImg.fillAmount = 1.0f;
                break;
            case "UpArrow":
                upArrowImg.fillAmount = 1.0f;
                break;
            case "RightArrow":
                rightArrowImg.fillAmount = 1.0f;
                break;
        }
    }

    private void FillBoxOfSelection()
    {
        switch (currentOutline)
        {
            case "LeftArrow":
                leftArrowImg.fillAmount += 1.0f/timeBetweenTapAndHold * Time.deltaTime;
                break;
            case "UpArrow":
                upArrowImg.fillAmount += 1.0f/timeBetweenTapAndHold * Time.deltaTime;
                break;
            case "RightArrow":
                rightArrowImg.fillAmount += 1.0f/timeBetweenTapAndHold * Time.deltaTime;
                break;
        }
    }

    private void DecreaseAllFillBoxes()
    {
        leftArrowImg.fillAmount -= 0.05f;
        upArrowImg.fillAmount -= 0.05f;
        rightArrowImg.fillAmount -= 0.05f;
        if (leftArrowImg.fillAmount == 0 && upArrowImg.fillAmount == 0 && rightArrowImg.fillAmount == 0)
        {
            cancelling = false;
        }
    }

    // Other functions
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

    void ExitLevel()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
