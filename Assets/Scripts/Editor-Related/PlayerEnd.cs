using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEnd : MonoBehaviour
{
    private SpriteRenderer sprite;
    private GameUi gameUi;
    public string levelToLoad;
    [SerializeField] private GameObject flag;
    private bool levelEnding;

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
        gameUi = FindObjectOfType<Canvas>().GetComponent<GameUi>();
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

    public void StartEndingLevel()
    {
        if (!levelEnding)
        {
            levelEnding = true;
            StartCoroutine(gameUi.FadeBlack("to", 3f));
            Invoke(nameof(TriggerEndLevel), 5f);
        }
    }

    void TriggerEndLevel()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
