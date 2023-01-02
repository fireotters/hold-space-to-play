using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Lean.Localization;
using TMPro;
using UnityEngine.Video;
using System.Collections;

public class MainMenuUi : BaseUi
{
    #region Variables
    public Image outlineImg, outlineSlimImg, playButtonImg, optionsButtonImg, optionsButtonImgWeb, exitButtonImg;
    private bool gameStarting;

    // Option Screen
    private bool optionsAreOpen;
    public GameObject optionsDialog;
    public Slider optionMusicSlider, optionSFXSlider;
    public Button optionEngButton, optionEspButton;
    private int levelNoSelected;

    // Audio
    public AudioMixer mixer;
    private MusicManager musicManager;
    public AudioSource sfxDemoSlider;

    // UI
    public Button btnFullscreenToggle;
    public GameObject optionsButton, optionsButtonWeb, exitButton;
    [SerializeField] private TextMeshProUGUI txtBtnFullscreenToggle, txtUiDescTitle, txtUiDesc, txtLevelSelect;

    // UI Videos
    [SerializeField] private VideoPlayer vidUiDemo_3btn, vidUiDemo_2btn;
    [SerializeField] private Texture firstFrameUiDemo_3btn, firstFrameUiDemo_2btn;
    private RawImage rimgUiDemo_3btn, rimgUiDemo_2btn;
    private Color vidVisible = new(1.0f, 1.0f, 1.0f, 1.0f), vidInvisible = new(1.0f, 1.0f, 1.0f, 0);

    // Menu Interaction
    private const float TimeHoldToActivate = 1.0f;
    private const float TimeBetweenTapAndHold = 0.8f; // Set to (TimeHoldToActivate - BaseUi.TimeTapToChange)

    // Plugins
    private LeanLocalization leanLoc;
    private DiscordManager discordManager;
    #endregion

    #region Start & Update
    void Start()
    {
        // Load Localization, and set English as default if nothing is set
        leanLoc = FindObjectOfType<LeanLocalization>();
        if (string.IsNullOrEmpty(leanLoc.CurrentLanguage))
            SetNewLanguage("English");

        // Load UI Demo videos
        vidUiDemo_3btn.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Demo-3button.webm");
        vidUiDemo_2btn.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Demo-2button.webm");
        rimgUiDemo_3btn = vidUiDemo_3btn.GetComponent<RawImage>();
        rimgUiDemo_2btn = vidUiDemo_2btn.GetComponent<RawImage>();

        // Remove exit button on WebGL version
#if UNITY_WEBGL
        exitButton.SetActive(false);
        optionsButton.SetActive(false);
        optionsButtonWeb.SetActive(true);
#else
        exitButton.SetActive(true);
        optionsButton.SetActive(true);
        optionsButtonWeb.SetActive(false);
#endif

        // Discord Rich Presence
        discordManager = FindObjectOfType<DiscordManager>();
        if (discordManager.UpdateDiscordRp(DiscordActivities.MainMenuActivity))
            Debug.Log("Rich presence updated.");

        // Handle music/sfx
        musicManager = FindObjectOfType<MusicManager>();
        if (!musicManager)
        {
            Instantiate(musicManagerIfNotFoundInScene);
            musicManager = FindObjectOfType<MusicManager>();
        }
        if (musicManager)
        {
            musicManager.sfxDemo = optionSFXSlider.GetComponent<AudioSource>();
            musicManager.ChangeMusicTrack(0);
        }
        if (!PlayerPrefs.HasKey("Music") || !PlayerPrefs.HasKey("SFX"))
        {
            PlayerPrefs.SetFloat("Music", 0.5f);
            PlayerPrefs.SetFloat("SFX", 0.5f);
        }
        mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFX")) * 20);

        // Screen transition & select 'Play' button
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
    #endregion

    #region BaseUI overrides
    protected override void ChangeSelection()
    {
        switch (currentOutline)
        {
            case "PlayButton":
                outlineImg.gameObject.SetActive(false);
                outlineSlimImg.gameObject.SetActive(true);
#if UNITY_WEBGL
                outlineSlimImg.transform.position = optionsButtonImgWeb.transform.position;
#else
                outlineSlimImg.transform.position = optionsButtonImg.transform.position;
#endif
                currentOutline = "OptionsButton";
                break;
            case "OptionsButton":
#if UNITY_WEBGL
                outlineImg.gameObject.SetActive(true);
                outlineSlimImg.gameObject.SetActive(false);
                currentOutline = "PlayButton";                
#else
                outlineSlimImg.transform.position = exitButtonImg.transform.position;
                currentOutline = "ExitButton";
#endif
                break;
#if UNITY_STANDALONE
            case "ExitButton":
                outlineImg.gameObject.SetActive(true);
                outlineSlimImg.gameObject.SetActive(false);
                currentOutline = "PlayButton";
                break;
#endif
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
#if UNITY_STANDALONE
            case "ExitButton":
                Application.Quit();
                break;
#endif
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
#if UNITY_WEBGL
                optionsButtonImgWeb.fillAmount = 1.0f;
#else
                optionsButtonImg.fillAmount = 1.0f;
#endif
                break;
#if UNITY_STANDALONE
            case "ExitButton":
                exitButtonImg.fillAmount = 1.0f;
                break;
#endif
        }
    }
    protected override void FillBoxOfSelection()
    {
        switch (currentOutline)
        {
            case "PlayButton":
                playButtonImg.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
                break;
#if UNITY_WEBGL
            case "OptionsButton":
                optionsButtonImgWeb.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
                break;
#else
            case "OptionsButton":
                optionsButtonImg.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
                break;
            case "ExitButton":
                exitButtonImg.fillAmount += 1.0f / TimeBetweenTapAndHold * Time.deltaTime;
                break;
#endif

        }
    }

    protected override void DecreaseAllFillBoxes()
    {
        playButtonImg.fillAmount -= 2.5f * Time.deltaTime;
#if UNITY_WEBGL
        optionsButtonImgWeb.fillAmount -= 2.5f * Time.deltaTime;
#else
        optionsButtonImg.fillAmount -= 2.5f * Time.deltaTime;
        exitButtonImg.fillAmount -= 2.5f * Time.deltaTime;
#endif
        if (playButtonImg.fillAmount == 0
#if UNITY_WEBGL
            && optionsButtonImgWeb.fillAmount == 0
#else
            && optionsButtonImg.fillAmount == 0 
#endif
            && exitButtonImg.fillAmount == 0)
        {
            cancelling = false;
        }
    }
#endregion

    #region Other UI methods
    private void ShowOptions()
    {
        optionsAreOpen = true;
        musicManager.sfxDemo = sfxDemoSlider;
        optionsDialog.SetActive(true);
        SetBtnFullscreenText();
        optionMusicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        optionSFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
        SetActiveLangButton();
        SetControlDisplay();
        // The video player's render texture retains the last played frame, and video restarts itself when gameobject is made inactive.
        // Force the video's first frame to load.
        Graphics.Blit(firstFrameUiDemo_2btn, vidUiDemo_2btn.targetTexture);
        Graphics.Blit(firstFrameUiDemo_3btn, vidUiDemo_3btn.targetTexture);
    }

    private void SetControlDisplay()
    {
        string descTitle = LeanLocalization.GetTranslationText("Options/Controls/DescTitle" + PlayerPrefs.GetInt("UI Type"));
        string descText = LeanLocalization.GetTranslationText("Options/Controls/Desc" + PlayerPrefs.GetInt("UI Type"));

        if (PlayerPrefs.GetInt("UI Type") == 0)
        {
            rimgUiDemo_3btn.color = vidVisible;
            rimgUiDemo_2btn.color = vidInvisible;
        }
        else
        {
            rimgUiDemo_3btn.color = vidInvisible;
            rimgUiDemo_2btn.color = vidVisible;
        }

        txtUiDescTitle.text = descTitle;
        txtUiDesc.text = descText;
    }

    public void SetNewLanguage(string newLang)
    {
        leanLoc.SetCurrentLanguage(newLang);
        SetControlDisplay();
        SetBtnFullscreenText();
    }

    private void SetActiveLangButton()
    {
        switch (leanLoc.CurrentLanguage)
        {
            case "English":
                optionEngButton.interactable = false;
                optionEspButton.interactable = true;
                break;
            case "Spanish":
                optionEngButton.interactable = true;
                optionEspButton.interactable = false;
                break;
        }
    }

    public void CloseOptions()
    {
        optionsAreOpen = false;
        optionsDialog.SetActive(false);
        beingHeld = false;
    }

    public void LevelSelectChange(int modifier)
    {
        int.TryParse(txtLevelSelect.text, out int levelNo);
        levelNo += modifier;
        if (levelNo == 0)
            levelNo = 8;
        else if (levelNo == 9)
            levelNo = 1;
        txtLevelSelect.text = levelNo.ToString();
    }
    public void LevelSelectPlay()
    {
        if (int.TryParse(txtLevelSelect.text, out int levelNo))
        {
            levelNoSelected = levelNo;
            StartCoroutine(FadeBlack("to"));
            Invoke(nameof(DoLevelLoad), 1f);
        }
    }

    private void DoLevelLoad()
    {
        SceneManager.LoadScene("Level" + levelNoSelected);
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
        Invoke(nameof(SetBtnFullscreenText), 0.1f);
    }

    public void SetBtnFullscreenText()
    {
        if (Screen.fullScreen)
        {
            txtBtnFullscreenToggle.text = LeanLocalization.GetTranslationText("Options/Visuals/BtnFS_On");
        }
        else
        {
            txtBtnFullscreenToggle.text = LeanLocalization.GetTranslationText("Options/Visuals/BtnFS_Off");
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
#endregion

    #region Other methods
    private void OpenGame()
    {
        SceneManager.LoadScene("Level1");
    }
    #endregion
}
