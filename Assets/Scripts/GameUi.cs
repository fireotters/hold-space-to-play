using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUi : BaseUi
{
    public int choiceOfMusic;
    public Image outlineImg, leftArrowImg, upArrowImg, rightArrowImg;
    public Image altUiMoveArrowImg, altUiUpArrowImg, altUiReversedArrowImg; // Alt UI uses different image files for transform.position purposes
    private string altUiDirection = "left";
    private Player player;
    private MusicManager musicManager;
    private GameObject baseUiObject;
    private GameObject altBaseUiObject;
    private const float TimeHoldToActivate = 0.4f;
    private const float TimeBetweenTapAndHold = 0.2f; // Set to (TimeHoldToActivate - BaseUi.TimeTapToChange)

    void Start()
    {
        // Begin with assigning the UI gameobjects
        baseUiObject = gameObject.transform.GetChild(0).gameObject;
        altBaseUiObject = gameObject.transform.GetChild(1).gameObject;

        // Set initial UI transform values, etc
        if (PlayerPrefs.GetInt("UI Type") == 0)
        {
            baseUiObject.SetActive(true);
            outlineImg.transform.position = upArrowImg.transform.position;
            currentOutline = "UpArrow";
        }
        else
        {
            altBaseUiObject.SetActive(true);
            outlineImg.transform.position = altUiMoveArrowImg.transform.position;
            currentOutline = "MoveArrowAlt";
        }

        // Change music track
        musicManager = FindObjectOfType<MusicManager>();
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
            Invoke(nameof(ExitLevel), 1f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startHoldTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            beingHeld = false;
            player.SetPlayerDirection("stop");
            if (startHoldTime + TimeTapToChange >= Time.time)
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
            else if (startHoldTime + TimeTapToChange < Time.time)
            {
                FillBoxOfSelection();
            }
            if (startHoldTime + TimeHoldToActivate <= Time.time && !beingHeld)
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

    protected override void ChangeSelection()
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

    protected override void ActivateSelection()
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
    protected override void KeepSelectionBoxFilled()
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

    protected override void FillBoxOfSelection()
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
        boxToFill.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
    }

    protected override void DecreaseAllFillBoxes()
    {
        if (baseUiObject.activeInHierarchy)
        {
            leftArrowImg.fillAmount -= 0.05f;
            upArrowImg.fillAmount -= 0.05f;
            rightArrowImg.fillAmount -= 0.05f;
            if (leftArrowImg.fillAmount == 0 && upArrowImg.fillAmount == 0 && rightArrowImg.fillAmount == 0)
            {
                cancelling = false;
            }
        }
        else if (altBaseUiObject.activeInHierarchy)
        {
            altUiMoveArrowImg.fillAmount -= 0.05f;
            altUiUpArrowImg.fillAmount -= 0.05f;
            if (altUiMoveArrowImg.fillAmount == 0f && altUiUpArrowImg.fillAmount == 0f)
            {
                cancelling = false;
            }
        }
    }

    private void ExitLevel()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
