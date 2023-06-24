using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO; //required for JsonUtility

public class GameManagerLiz : MonoBehaviour
{
    public Transform startingPoint;
    public float playerLerpSpeed;
    public float witchLerpSpeed;

    [SerializeField] private TextMeshProUGUI scoreTextPrefab;
    [SerializeField] private TextMeshProUGUI highScoreText;

    public TextMeshProUGUI gameoverText;
    public Button restartButton;
    public Button startButton;
    public GameObject titleScreen;
    public GameObject gameoverScreen;

    public GameObject playerObject; // Reference to the player object in the scene

    public float scoreValue = 0f;
    public float pointIncreasedPerSecond = 1f;
    public static int highScore = 0;

    private MoveLeftLiz moveLeftScript;
    private Coroutine introCoroutine;
    private CauldronMovement cauldronMovementScript;

    [SerializeField] private PlayerControllerLiz playerControllerScript;
    [SerializeField] private SpawnManagerLiz spawnManager;
    [SerializeField] private CauldronLaunch cauldronLaunchScript;

    public bool isGameActive;
    public bool isGameOver = false;

    public GameObject howToPlayScreen;
    private bool isGamePaused = false;

    public Transform witchStartingPoint;
    public GameObject witchObject;
    public float introDuration = 2f; // Adjust the duration as needed
    public float witchIntroDelay = 20f; // Adjust the delay as needed

    public GameObject objectToDeactivate;
    public GameObject objectToActivate;

    private void Awake()
    {
        cauldronMovementScript = GameObject.Find("Cauldron2").GetComponent<CauldronMovement>();
        cauldronLaunchScript = GameObject.Find("Cauldron2").GetComponent<CauldronLaunch>();
        playerControllerScript = playerObject.GetComponent<PlayerControllerLiz>();
    }

    private void Start()
    {
        LoadHighScore();
        UpdateHighScoreText();
        startButton.onClick.AddListener(StartGame);
        cauldronMovementScript.StopMovement();

        witchObject.SetActive(false);

        // Assign the references using serialized fields
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerControllerLiz>();

        // Create titleScreen object dynamically
        titleScreen = GameObject.Find("TitleScreen");
        if (titleScreen == null)
        {
            titleScreen = new GameObject("TitleScreen");
        }
        // Disable the player object in the scene by default
        playerObject.SetActive(false);
    }

    public void StartGame()
    {
        if (!isGameActive)
        {
            isGameActive = true;
            scoreValue = 0f;
            titleScreen.SetActive(false);
            introCoroutine = StartCoroutine(PlayIntro());
            cauldronMovementScript.StartGame();

            objectToDeactivate.SetActive(false);
            objectToActivate.SetActive(false);

            // Enable the new character object when the game starts
            witchObject.SetActive(true);
            StartCoroutine(DelayedWitchIntro());

            // Enable the player object when the game starts
            playerObject.SetActive(true);

            // Enable the SpawnManagerLiz component
            spawnManager = GetComponent<SpawnManagerLiz>();
            spawnManager.StartSpawning(); // Call StartSpawning() method
            cauldronLaunchScript.StartSpawning(); // Call StartSpawning() on CauldronLaunch script

            // Call SetCanMove(true) on the MoveLeftLiz component
            moveLeftScript = FindObjectOfType<MoveLeftLiz>();
            moveLeftScript.SetCanMove(true);

            SetScoreText();
        }
    }
    private void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    private void FixedUpdate()
    {
        if (!isGameActive || isGameOver)
        {
            return; // Exit the method if the game is not active or if it's game over
        }

        scoreValue += pointIncreasedPerSecond * Time.fixedDeltaTime;
        SetScoreText(); // Call the method to update the score text
    }

    private void SetScoreText()
    {
        if (isGameActive && !isGameOver)
        {
            scoreTextPrefab.text = "Score: " + ((int)scoreValue).ToString();
            highScoreText.text = "High Score: " + highScore.ToString();

            if (scoreValue > highScore)
            {
                highScore = (int)scoreValue;
                SaveHighScore();
            }
        }
    }

    public void IncreaseScore(int amount)
    {
        scoreValue += amount;
    }

    [System.Serializable]
    class SaveData
    {
        public int highScore;
    }

    public void SaveHighScore()
    {
        SaveData data = new SaveData();

        data.highScore = highScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.highScore;
        }
    }
    public void ResetHighScore()
    {
        highScore = 0;
        SaveHighScore(); // Save the updated high score
    }

    IEnumerator PlayIntro()
    {
        yield return new WaitForEndOfFrame();

        if (playerControllerScript != null)
        {
            Vector3 playerStartPos = playerControllerScript.transform.position;
            Vector3 playerEndPos = startingPoint.position;
            float playerJourneyLength = Vector3.Distance(playerStartPos, playerEndPos);
            float playerStartTime = Time.time;

            float playerDistanceCovered = (Time.time - playerStartTime) * playerLerpSpeed;
            float playerFractionOfJourney = playerDistanceCovered / playerJourneyLength;

            Animator playerAnimator = playerControllerScript.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("Speed_Multiplier", 0.5f);
            }

            while (playerFractionOfJourney < 1)
            {
                playerDistanceCovered = (Time.time - playerStartTime) * playerLerpSpeed;
                playerFractionOfJourney = playerDistanceCovered / playerJourneyLength;
                playerControllerScript.transform.position = Vector3.Lerp(playerStartPos, playerEndPos, playerFractionOfJourney);
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("PlayerControllerLiz not found. Make sure the player object is present in the scene.");
        }

        // Witch movement
        if (witchObject != null)
        {
            Vector3 witchStartPos = witchObject.transform.position;
            Vector3 witchEndPos = witchStartingPoint.position;
            float witchJourneyLength = Vector3.Distance(witchStartPos, witchEndPos);
            float witchStartTime = Time.time;

            float witchDistanceCovered = (Time.time - witchStartTime) * witchLerpSpeed;
            float witchFractionOfJourney = witchDistanceCovered / witchJourneyLength;

            witchObject.SetActive(true);

            while (witchFractionOfJourney < 1)
            {
                witchDistanceCovered = (Time.time - witchStartTime) * witchLerpSpeed;
                witchFractionOfJourney = witchDistanceCovered / witchJourneyLength;
                witchObject.transform.position = Vector3.Lerp(witchStartPos, witchEndPos, witchFractionOfJourney);
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Witch object not assigned. Make sure to assign the witch object in the GameManagerLiz script.");
        }

        StartCoroutine(AnimateWitchIntro());
    }

    private IEnumerator DelayedWitchIntro()
    {
        yield return new WaitForSeconds(witchIntroDelay);

        // Start the witch intro animation
        StartCoroutine(AnimateWitchIntro());
    }
    private IEnumerator AnimateWitchIntro()
    {
        if (witchObject != null)
        {
            Vector3 startPos = witchObject.transform.position;
            Vector3 endPos = witchStartingPoint.position;
            float journeyLength = Vector3.Distance(startPos, endPos);
            float startTime = Time.time;
            float duration = introDuration; // Use the introDuration variable for the duration of the animation

            Animator witchAnimator = witchObject.GetComponent<Animator>();
            if (witchAnimator != null)
            {
                witchAnimator.SetFloat("Speed Multiplier", 0.5f);
            }

            while (Time.time - startTime < duration)
            {
                float timePassed = Time.time - startTime;
                float normalizedTime = Mathf.Clamp01(timePassed / duration);
                float easedTime = EaseInOutCubic(normalizedTime); // Apply easing function
                witchObject.transform.position = Vector3.Lerp(startPos, endPos, easedTime);
                yield return null;
            }

            // Set the witch's final position explicitly to ensure accuracy
            witchObject.transform.position = endPos;
        }
        else
        {
            Debug.LogWarning("Witch object not assigned. Make sure to assign the witch object in the GameManagerLiz script.");
        }
    }

    // Cubic easing function
    private float EaseInOutCubic(float t)
    {
        t /= 0.5f;
        if (t < 1f)
            return 0.5f * t * t * t;
        t -= 2f;
        return 0.5f * (t * t * t + 2f);
    }

    public void GameOver()
    {
        isGameActive = false;
        isGameOver = true; // Set isGameOver to true
        restartButton.gameObject.SetActive(true);
        gameoverScreen.SetActive(true);

        objectToDeactivate.SetActive(false);
        objectToActivate.SetActive(true);

        if (playerControllerScript != null)
        {
            // Stop player movement
            playerControllerScript.SetCanMove(false);
        }
        else
        {
            Debug.LogWarning("PlayerControllerLiz not found. Make sure the player object is present in the scene.");
        }

        // Stop spawning
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }
        else
        {
            Debug.LogWarning("SpawnManagerLiz not found. Make sure the GameManagerLiz has the SpawnManagerLiz component attached.");
        }

        if (cauldronLaunchScript != null)
        {
            cauldronLaunchScript.StopSpawning();
            // Stop the cauldron movement
            CauldronMovement cauldronMovementScript = cauldronLaunchScript.GetComponent<CauldronMovement>();
            if (cauldronMovementScript != null)
            {
                cauldronMovementScript.StopMovement();
            }
            else
            {
                Debug.LogWarning("CauldronMovement script not found. Make sure the GameManagerLiz has the CauldronMovement component attached to the CauldronLaunch object.");
            }
        }
        else
        {
            Debug.LogWarning("CauldronLaunch script not found. Make sure the GameManagerLiz has the CauldronLaunch component attached.");
        }
#if UNITY_EDITOR
        // Uncheck the "Is Game Active" box in the Inspector
        isGameActive = false;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    public void ToggleHowToPlayScreen()
    {
        bool isPanelActive = !howToPlayScreen.activeSelf;

        if (isPanelActive)
        {
            PauseGame();
        }
        else
        {
            if (isGamePaused)
            {
                UnpauseGame();
            }
        }

        howToPlayScreen.SetActive(isPanelActive);
    }

    public void PauseGame()
    {
        if (isGameActive && !isGamePaused)
        {
            Time.timeScale = 0f;
            isGamePaused = true;
        }
    }

    public void UnpauseGame()
    {
        if (isGameActive && isGamePaused)
        {
            Time.timeScale = 1f;
            isGamePaused = false;
        }
    }

    public void ExitButtonClicked()
    {
        if (isGamePaused)
        {
            UnpauseGame();
        }

        if (howToPlayScreen.activeSelf)
        {
            howToPlayScreen.SetActive(false);
        }

        // Additional code to handle exit button functionality
    }

    public void RestartGame()
    {
        // Reset variables
        isGameActive = false;
        isGameOver = false;
        scoreValue = 0f;

        Physics.gravity = new Vector3(0, -9.8f, 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}