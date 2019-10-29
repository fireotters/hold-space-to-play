using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UI_Script_MainMenu : MonoBehaviour
{

    public Image outlineImg, outlineSlimImg, playButtonImg, optionsButtonImg, exitButtonImg;
    private string currentOutline;

    float startHoldTime = 0f;
    float timeTapToChange = 0.2f;
    float timeHoldToActivate = 1.0f;
    float timeBetweenTapAndHold = 0.8f; // Set to (timeHoldToActivate - timeTapToChange)
    bool cancelling = false;
    bool beingHeld = false;
    bool gameStarting = false;

    // Option Screen
    bool optionsAreOpen = false;
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

    public GameObject fadeBlack;


    void Start()
    {
        musicManager = GameObject.FindObjectOfType<MusicManager>();
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
        if (Input.GetKey(KeyCode.Space) && !cancelling && !optionsAreOpen && !gameStarting)
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

    private void ActivateSelection()
    {
        switch (currentOutline)
        {
            case "PlayButton":
                StartCoroutine(FadeBlack("to"));
                gameStarting = true;
                Invoke("OpenGame", 1f);
                break;
            case "OptionsButton":
                ShowOptions();
                break;
            case "ExitButton":
                Application.Quit();
                break;
        }
    }

    private void KeepSelectionBoxFilled()
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
    private void FillBoxOfSelection()
    {
        switch (currentOutline)
        {
            case "PlayButton":
                playButtonImg.fillAmount += 1.0f / timeBetweenTapAndHold * Time.deltaTime;
                break;
            case "OptionsButton":
                optionsButtonImg.fillAmount += 1.0f / timeBetweenTapAndHold * Time.deltaTime;
                break;
            case "ExitButton":
                exitButtonImg.fillAmount += 1.0f / timeBetweenTapAndHold * Time.deltaTime;
                break;
        }
    }

    private void DecreaseAllFillBoxes()
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
    void ShowOptions()
    {
        optionsAreOpen = true;
        musicManager.sfxDemo = sfxDemoSlider;
        optionsDialog.SetActive(true);
        fullscreenToggle.interactable = !Screen.fullScreen;
        optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
        SetControlDisplay();
    }

    public void SetControlDisplay()
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
                Invoke("DoLevelLoad", 1f);
            }
        }
    }

    void DoLevelLoad()
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

    public void SetUIChoice()
    {
        if (PlayerPrefs.GetInt("UI Type") == 0)
            PlayerPrefs.SetInt("UI Type", 1);
        else
            PlayerPrefs.SetInt("UI Type", 0);
        SetControlDisplay();
    }

    // Other functions
    void OpenGame()
    {
        SceneManager.LoadScene("Level1");
    }
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
