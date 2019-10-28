using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Script : MonoBehaviour
{
    public int choiceOfMusic;
    public Image outlineImg, leftArrowImg, upArrowImg, rightArrowImg;
    public Image altUiMoveArrowImg, altUiUpArrowImg, altUiReversedArrowImg; // Alt UI uses different image files for transform.position purposes
    private string currentOutline;
    private string altUiDirection = "left";

    float startHoldTime = 0f;
    float timeTapToChange = 0.2f;
    float timeHoldToActivate = 0.4f;
    float timeBetweenTapAndHold = 0.2f; // Set to (timeHoldToActivate - timeTapToChange)
    bool cancelling = false;
    bool beingHeld = false;
    public GameObject fadeBlack;

    private Player player;
    private MusicManager musicManager;
    private GameObject baseUI;
    private GameObject baseUIAlt;

    void Start()
    {
        // Begin with assigning the UI gameobjects
        baseUI = gameObject.transform.GetChild(0).gameObject;
        baseUIAlt = gameObject.transform.GetChild(1).gameObject;

        // Set initial UI transform values, etc
        if (PlayerPrefs.GetInt("UI Type") == 0)
        {
            baseUI.SetActive(true);
            outlineImg.transform.position = upArrowImg.transform.position;
            currentOutline = "UpArrow";
        }
        else
        {
            baseUIAlt.SetActive(true);
            outlineImg.transform.position = altUiMoveArrowImg.transform.position;
            currentOutline = "MoveArrowAlt";
        }

        // Change music track
        musicManager = GameObject.FindObjectOfType<MusicManager>();
        if (musicManager)
        {
            musicManager.ChangeMusicTrack(choiceOfMusic);
        }

        // Find the player gameobject and fade in the level
        player = FindObjectOfType<Player>();
        StartCoroutine(FadeBlack("from"));
    }

    void Update()
    {
        // Check for a null player gameobject, reassign it if it goes missing
        if (player == null) 
        {
            player = FindObjectOfType<Player>();
        }

        // Key input checks
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
        Vector3 positionToMoveSelector = new Vector3();
        switch (currentOutline)
        {
            case "LeftArrow":
                positionToMoveSelector = upArrowImg.transform.position;
                currentOutline = "UpArrow";
                break;
            case "UpArrow":
                positionToMoveSelector = rightArrowImg.transform.position;
                currentOutline = "RightArrow";
                break;
            case "RightArrow":
                positionToMoveSelector = leftArrowImg.transform.position;
                currentOutline = "LeftArrow";
                break;
            // Alternate UI
            case "MoveArrowAlt":
                positionToMoveSelector = altUiUpArrowImg.transform.position;
                currentOutline = "UpArrowAlt";
                // When the selector passes over alt UI movement arrow, change the direction
                ChangeAltUiDirection();
                break;
            case "UpArrowAlt":
                positionToMoveSelector = altUiMoveArrowImg.transform.position;
                currentOutline = "MoveArrowAlt";
                break;
        }
        outlineImg.transform.position = positionToMoveSelector;
    }

    private void ChangeAltUiDirection()
    {
        if (altUiDirection == "left")
        {
            altUiDirection = "right";
            altUiReversedArrowImg.gameObject.SetActive(false);
        }
        else
        {
            altUiDirection = "left";
            altUiReversedArrowImg.gameObject.SetActive(true);
        }
        altUiMoveArrowImg.transform.Rotate(0, 0, 180);
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
            // Alternate UI
            case "MoveArrowAlt":
                player.SetPlayerDirection(altUiDirection); // "left" or "right" set by ChangeSelection()
                break;
            case "UpArrowAlt":
                player.jump = true;
                break;
        }
    }
    private void KeepSelectionBoxFilled()
    {
        Image boxToKeepFilled = null;
        switch (currentOutline)
        {
            case "LeftArrow":
                boxToKeepFilled = leftArrowImg;
                break;
            case "UpArrow":
                boxToKeepFilled = upArrowImg;
                break;
            case "RightArrow":
                boxToKeepFilled = rightArrowImg;
                break;
            // Alternate UI
            case "MoveArrowAlt":
                boxToKeepFilled = altUiMoveArrowImg;
                break;
            case "UpArrowAlt":
                boxToKeepFilled = altUiUpArrowImg;
                break;
        }
        boxToKeepFilled.fillAmount = 1.0f;
    }

    private void FillBoxOfSelection()
    {
        Image boxToFill = null;
        switch (currentOutline)
        {
            case "LeftArrow":
                boxToFill = leftArrowImg;
                break;
            case "UpArrow":
                boxToFill = upArrowImg;
                break;
            case "RightArrow":
                boxToFill = rightArrowImg;
                break;
            // Alternate UI
            case "MoveArrowAlt":
                boxToFill = altUiMoveArrowImg;
                break;
            case "UpArrowAlt":
                boxToFill = altUiUpArrowImg;
                break;
        }
        boxToFill.fillAmount += 1.0f / timeBetweenTapAndHold * Time.deltaTime;
    }

    private void DecreaseAllFillBoxes()
    {
        if (baseUI.activeInHierarchy)
        {
            leftArrowImg.fillAmount -= 0.05f;
            upArrowImg.fillAmount -= 0.05f;
            rightArrowImg.fillAmount -= 0.05f;
            if (leftArrowImg.fillAmount == 0 && upArrowImg.fillAmount == 0 && rightArrowImg.fillAmount == 0)
            {
                cancelling = false;
            }
        }
        else if (baseUIAlt.activeInHierarchy)
        {
            altUiMoveArrowImg.fillAmount -= 0.05f;
            altUiUpArrowImg.fillAmount -= 0.05f;
            if (altUiMoveArrowImg.fillAmount == 0 && altUiUpArrowImg.fillAmount == 0)
            {
                cancelling = false;
            }
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
