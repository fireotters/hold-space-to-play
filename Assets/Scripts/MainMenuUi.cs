using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuUi : BaseUi
{
    public Image outlineImg, outlineSlimImg, playButtonImg, optionsButtonImg, exitButtonImg;
    private bool gameStarting;

    // Option Screen
    private bool optionsAreOpen;
    public GameObject optionsDialog;
    public Slider optionMusicSlider, optionSFXSlider;
    public AudioMixer mixer;
    private MusicManager musicManager;
    private int levelNoSelected;
    public Text levelSelectText;
    public AudioSource sfxDemoSlider;
    public Button fullscreenToggle;
    public RawImage uiDemo1, uiDemo2;
    public Text uiDescTitle, uiDesc;
    internal const float TimeHoldToActivate = 1.0f;
    internal const float TimeBetweenTapAndHold = 0.8f; // Set to (TimeHoldToActivate - BaseUi.TimeTapToChange)

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
        if (musicManager)
        {
            musicManager.ChangeMusicTrack(0);
        }
        if (!PlayerPrefs.HasKey("Music") || !PlayerPrefs.HasKey("SFX"))
        {
            PlayerPrefs.SetFloat("Music", 0.5f);
            PlayerPrefs.SetFloat("SFX", 0.5f);
        }
        mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFX")) * 20);
        StartCoroutine(FadeBlack("from"));
        outlineImg.transform.position = playButtonImg.transform.position;
        currentOutline = "PlayButton";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !optionsAreOpen)
        {
            startHoldTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.Space) && !optionsAreOpen)
        {
            beingHeld = false;
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
        if (Input.GetKey(KeyCode.Space) && !cancelling && !optionsAreOpen && !gameStarting)
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
        switch (currentOutline)
        {
            case "PlayButton":
                outlineImg.gameObject.SetActive(false);
                outlineSlimImg.gameObject.SetActive(true);
                outlineSlimImg.transform.position = optionsButtonImg.transform.position;
                currentOutline = "OptionsButton";
                break;
            case "OptionsButton":
                outlineSlimImg.transform.position = exitButtonImg.transform.position;
                currentOutline = "ExitButton";
                break;
            case "ExitButton":
                outlineImg.gameObject.SetActive(true);
                outlineSlimImg.gameObject.SetActive(false);
                currentOutline = "PlayButton";
                break;
        }
    }

    protected override void ActivateSelection()
    {
        switch (currentOutline)
        {
            case "PlayButton":
                StartCoroutine(FadeBlack("to"));
                gameStarting = true;
                Invoke(nameof(OpenGame), 1f);
                break;
            case "OptionsButton":
                ShowOptions();
                break;
            case "ExitButton":
                Application.Quit();
                break;
        }
    }

    protected override void KeepSelectionBoxFilled()
    {
        switch (currentOutline)
        {
            case "PlayButton":
                playButtonImg.fillAmount = 1.0f;
                break;
            case "OptionsButton":
                optionsButtonImg.fillAmount = 1.0f;
                break;
            case "ExitButton":
                exitButtonImg.fillAmount = 1.0f;
                break;
        }
    }
    protected override void FillBoxOfSelection()
    {
        switch (currentOutline)
        {
            case "PlayButton":
                playButtonImg.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
                break;
            case "OptionsButton":
                optionsButtonImg.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
                break;
            case "ExitButton":
                exitButtonImg.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
                break;
        }
    }

    protected override void DecreaseAllFillBoxes()
    {
        playButtonImg.fillAmount -= 0.05f;
        optionsButtonImg.fillAmount -= 0.05f;
        exitButtonImg.fillAmount -= 0.05f;
        if (playButtonImg.fillAmount == 0 && optionsButtonImg.fillAmount == 0 && exitButtonImg.fillAmount == 0)
        {
            cancelling = false;
        }
    }

    // Functions related to options menu
    private void ShowOptions()
    {
        optionsAreOpen = true;
        musicManager.sfxDemo = sfxDemoSlider;
        optionsDialog.SetActive(true);
        fullscreenToggle.interactable = !Screen.fullScreen;
        optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
        SetControlDisplay();
    }

    private void SetControlDisplay()
    {
        if (PlayerPrefs.GetInt("UI Type") == 0)
        {
            uiDemo1.gameObject.SetActive(true);
            uiDemo2.gameObject.SetActive(false);
            uiDescTitle.text = "Easier Movement";
            uiDesc.text = "Three buttons, each\ndedicated to an action.";
        }
        else
        {
            uiDemo1.gameObject.SetActive(false);
            uiDemo2.gameObject.SetActive(true);
            uiDescTitle.text = "Easier Running Jumps";
            uiDesc.text = "Two buttons. One jumps,\none flips between moving\nplayer left or right.";
        }
    }
    public void CloseOptions()
    {
        optionsAreOpen = false;
        optionsDialog.SetActive(false);
        optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
        beingHeld = false;
    }

    public void GoToLevel()
    {
        if (int.TryParse(levelSelectText.text, out int levelNo))
        {
            if (levelNo > 0 && levelNo < 9)
            {
                levelNoSelected = levelNo;
                StartCoroutine(FadeBlack("to"));
                Invoke(nameof(DoLevelLoad), 1f);
            }
        }
    }

    private void DoLevelLoad()
    {
        SceneManager.LoadScene("Level" + levelNoSelected);
    }

    public void SwapFullscreen()
    {
        fullscreenToggle.interactable = false;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }

    public void SetResolution(int resolutionChoice)
    {
        fullscreenToggle.interactable = true;
        switch (resolutionChoice)
        {
            case 480:
                Screen.SetResolution(848, 480, false);
                break;
            case 720:
                Screen.SetResolution(1280, 720, false);
                break;
            case 1080:
                Screen.SetResolution(1920, 1080, false);
                break;
            case 1440:
                Screen.SetResolution(2560, 1440, false);
                break;
        }
            
    }

    public void SetUiChoice()
    {
        if (PlayerPrefs.GetInt("UI Type") == 0)
            PlayerPrefs.SetInt("UI Type", 1);
        else
            PlayerPrefs.SetInt("UI Type", 0);
        SetControlDisplay();
    }

    // Other functions
    private void OpenGame()
    {
        SceneManager.LoadScene("Level1");
    }
}
