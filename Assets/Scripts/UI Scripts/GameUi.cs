using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUi : BaseUi
{
    private Player player;
    public MusicManager musicManager;
    public int choiceOfMusic, levelNo;
    private DiscordManager discordManager;

    // UI Placement Variables
    public GameObject gamePausePanel;
    private GameObject baseUiObject, altBaseUiObject;
    public Image outlineImg, leftArrowImg, upArrowImg, rightArrowImg;
    public Image altUiMoveArrowImg, altUiUpArrowImg, altUiReversedArrowImg; // Alt UI uses different image files for transform.position purposes
    private string altUiDirection = "left";

    // UI Gameplay Variables
    private const float TimeHoldToActivate = 0.35f;
    private const float TimeBetweenTapAndHold = 0.15f; // Set to (TimeHoldToActivate - BaseUi.TimeTapToChange)

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
        if (!musicManager)
        {
            Instantiate(musicManagerIfNotFoundInScene);
            musicManager = FindObjectOfType<MusicManager>();
        }
        musicManager.ChangeMusicTrack(choiceOfMusic);

        // Change discord status
        discordManager = FindObjectOfType<DiscordManager>();
        if (discordManager == null)
        {
            Debug.Log("DiscordManager not updated: Debugging individual level.");
        }
        else if (discordManager.UpdateDiscordRp(DiscordActivities.StartGameActivity(levelNo)))
        {
            Debug.Log("Rich presence has been updated.");
        }

        // Find the player gameobject and fade in the level
        player = FindObjectOfType<Player>();
        StartCoroutine(FadeBlack("from"));
    }

    void Update()
    {
        if (player == null) { player = FindObjectOfType<Player>(); } // Reassign player if it goes missing
        CheckKeyInputs();
        HandleSelectionBoxes();
    }

    private void CheckKeyInputs()
    {

        // Key input checks
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePausePanel.activeInHierarchy)
            {
                PauseGame(0);
            }
            else
            {
                PauseGame(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwapFullscreen();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !gamePausePanel.activeInHierarchy)
        {
            startHoldTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.Space) && !gamePausePanel.activeInHierarchy)
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
    }
    private void HandleSelectionBoxes()
    {

        // If the current action is not being cancelled, then fill the selection box.
        if (Input.GetKey(KeyCode.Space) && !cancelling && !gamePausePanel.activeInHierarchy)
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
                player.initiateJump = true;
                break;
            case "RightArrow":
                player.SetPlayerDirection("right");
                break;
            // Alternate UI
            case "MoveArrowAlt":
                player.SetPlayerDirection(altUiDirection); // "left" or "right" set by ChangeSelection()
                break;
            case "UpArrowAlt":
                player.initiateJump = true;
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
            leftArrowImg.fillAmount -= 2.5f * Time.deltaTime;
            upArrowImg.fillAmount -= 2.5f * Time.deltaTime;
            rightArrowImg.fillAmount -= 2.5f * Time.deltaTime;
            if (leftArrowImg.fillAmount == 0 && upArrowImg.fillAmount == 0 && rightArrowImg.fillAmount == 0)
            {
                cancelling = false;
            }
        }
        else if (altBaseUiObject.activeInHierarchy)
        {
            altUiMoveArrowImg.fillAmount -= 2.5f * Time.deltaTime;
            altUiUpArrowImg.fillAmount -= 2.5f * Time.deltaTime;
            if (altUiMoveArrowImg.fillAmount == 0f && altUiUpArrowImg.fillAmount == 0f)
            {
                cancelling = false;
            }
        }
    }

    public void PauseGame(int intent)
    {
        if (intent == 0)
        { // Pause game
            if (musicManager != null) {
                musicManager.PauseMusic();
                musicManager.FindAllSfxAndPlayPause(0);
            }

            gamePausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        else if (intent == 1)
        { // Resume game
            if (PlayerPrefs.GetFloat("Music") > 0f && musicManager != null) {
                musicManager.ResumeMusic();
                musicManager.FindAllSfxAndPlayPause(1);
            }

            gamePausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void SwapFullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, false);
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
    }

    public void ExitLevel()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}
