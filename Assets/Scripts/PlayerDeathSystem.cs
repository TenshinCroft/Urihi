using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDeathSystem : MonoBehaviour
{
    [Header("Camera Death Settings")]
    public Camera playerCamera;
    public Transform enemy;
    public float cameraLookSpeed = 2f;
    public float deathSequenceDuration = 3f;

    [Header("Audio")]
    public AudioSource deathAudioSource;
    public AudioClip deathSound;
    [Range(0f, 1f)] public float deathVolume = 1f;

    [Header("UI References")]
    public GameObject deathUI;
    public Button restartButton;
    public Button quitButton;
    public CanvasGroup blackScreen;
    public CanvasGroup gameOverText;

    [Header("Animation Settings")]
    public float blackScreenFadeSpeed = 1f;
    public float textAppearDelay = 1.5f;
    public float buttonsAppearDelay = 2.5f;

    [Header("Scene Names")]
    public string menuSceneName = "tela inicio";

    private bool isDead = false;
    private Player playerScript;
    private PlayerLook playerLook;
    private CharacterController characterController;

    private void Awake()
    {
        // Get references
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
                playerCamera = Camera.main;
        }

        playerScript = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();

        // Find PlayerLook component on the camera
        if (playerCamera != null)
            playerLook = playerCamera.GetComponent<PlayerLook>();

        // Auto-find UI elements
        AutoFindUIReferences();

        // Setup audio
        SetupDeathAudio();

        // Setup button events
        SetupButtonEvents();

        Debug.Log("PlayerDeathSystem initialized successfully");
    }

    private void Start()
    {
        // Initialize UI state
        InitializeUIState();
    }

    private void SetupDeathAudio()
    {
        // Try to find DeathSound GameObject
        GameObject deathSoundObj = GameObject.Find("DeathSound");
        if (deathSoundObj != null)
        {
            deathAudioSource = deathSoundObj.GetComponent<AudioSource>();
            if (deathAudioSource != null)
            {
                Debug.Log("DeathSound AudioSource found and connected!");
            }
        }

        // Create AudioSource if not found
        if (deathAudioSource == null)
        {
            deathAudioSource = gameObject.AddComponent<AudioSource>();
            deathAudioSource.playOnAwake = false;
            deathAudioSource.spatialBlend = 0f;
        }
    }

    private void SetupButtonEvents()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitToMenu);
    }

    private void InitializeUIState()
    {
        if (deathUI != null)
            deathUI.SetActive(false);

        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            if (blackScreen.gameObject.activeSelf)
                blackScreen.gameObject.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.alpha = 0f;
            if (gameOverText.gameObject.activeSelf)
                gameOverText.gameObject.SetActive(false);
        }

        // Initially hide buttons
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
        if (quitButton != null)
            quitButton.gameObject.SetActive(false);
    }

    private void AutoFindUIReferences()
    {
        // Try to find DeathUI
        if (deathUI == null)
        {
            GameObject foundDeathUI = GameObject.Find("DeathUi");
            if (foundDeathUI != null)
            {
                deathUI = foundDeathUI;
                Debug.Log("Auto-found DeathUi GameObject");
            }
        }

        // Try to find BalckScreen CanvasGroup
        if (blackScreen == null)
        {
            GameObject blackScreenObj = GameObject.Find("BalckScreen");
            if (blackScreenObj != null)
            {
                blackScreen = blackScreenObj.GetComponent<CanvasGroup>();
                if (blackScreen == null)
                {
                    blackScreen = blackScreenObj.AddComponent<CanvasGroup>();
                }
                Debug.Log("Auto-found BalckScreen CanvasGroup");
            }
        }

        // Try to find Text CanvasGroup
        if (gameOverText == null)
        {
            GameObject textObj = GameObject.Find("Text");
            if (textObj != null && textObj.transform.parent != null && textObj.transform.parent.name == "DeathUi")
            {
                gameOverText = textObj.GetComponent<CanvasGroup>();
                if (gameOverText == null)
                {
                    gameOverText = textObj.AddComponent<CanvasGroup>();
                }
                Debug.Log("Auto-found Text CanvasGroup");
            }
        }

        // Try to find buttons
        if (restartButton == null)
        {
            GameObject restartObj = GameObject.Find("Reiniciar");
            if (restartObj != null)
            {
                restartButton = restartObj.GetComponent<Button>();
                Debug.Log("Auto-found Reiniciar Button");
            }
        }

        if (quitButton == null)
        {
            GameObject quitObj = GameObject.Find("Sair");
            if (quitObj != null)
            {
                quitButton = quitObj.GetComponent<Button>();
                Debug.Log("Auto-found Sair Button");
            }
        }
    }

    public void TriggerDeath(Transform enemyTransform)
    {
        if (isDead)
        {
            Debug.Log("Death already triggered, ignoring");
            return;
        }

        Debug.Log("=== DEATH TRIGGERED ===");
        isDead = true;
        enemy = enemyTransform;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        Debug.Log("=== DEATH SEQUENCE STARTED ===");

        // 1. Disable player movement and look
        if (characterController != null)
        {
            characterController.enabled = false;
            Debug.Log("CharacterController disabled");
        }

        if (playerScript != null)
        {
            playerScript.enabled = false;
            Debug.Log("Player script disabled");
        }

        if (playerLook != null)
        {
            playerLook.enabled = false;
            Debug.Log("PlayerLook disabled");
        }

        // 2. Play death sound
        if (deathSound != null && deathAudioSource != null)
        {
            deathAudioSource.clip = deathSound;
            deathAudioSource.volume = deathVolume;
            deathAudioSource.Play();
            Debug.Log("Death sound played");
        }

        // 3. Lock camera on enemy
        Debug.Log("Starting camera lock on enemy");
        float elapsed = 0f;

        while (elapsed < deathSequenceDuration && enemy != null && playerCamera != null)
        {
            Vector3 directionToEnemy = (enemy.position - playerCamera.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);

            playerCamera.transform.rotation = Quaternion.Slerp(
                playerCamera.transform.rotation,
                targetRotation,
                cameraLookSpeed * Time.deltaTime
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Camera lock completed, showing death UI");

        // 4. Show death UI
        ShowDeathUI();
    }

    private void ShowDeathUI()
    {
        Debug.Log("=== SHOWING DEATH UI ===");

        if (deathUI != null)
        {
            deathUI.SetActive(true);
            Debug.Log("DeathUI activated");
            StartCoroutine(AnimateDeathUI());
        }
        else
        {
            Debug.LogError("DeathUI is null! Cannot show death screen.");
        }
    }

    private IEnumerator AnimateDeathUI()
    {
        Debug.Log("=== ANIMATING DEATH UI ===");

        // 1. Fade in black screen
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 0f;
            Debug.Log("Black screen activated, starting fade");

            while (blackScreen.alpha < 1f)
            {
                blackScreen.alpha += blackScreenFadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }
            blackScreen.alpha = 1f;
            Debug.Log("Black screen fade completed");
        }

        // 2. Wait and show text
        Debug.Log($"Waiting {textAppearDelay} seconds before showing text");
        yield return new WaitForSecondsRealtime(textAppearDelay);

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.alpha = 0f;
            Debug.Log("Game over text activated, starting fade");

            while (gameOverText.alpha < 1f)
            {
                gameOverText.alpha += 2f * Time.unscaledDeltaTime;
                yield return null;
            }
            gameOverText.alpha = 1f;
            Debug.Log("Game over text fade completed");
        }

        // 3. Wait and show buttons
        float buttonDelay = buttonsAppearDelay - textAppearDelay;
        Debug.Log($"Waiting {buttonDelay} seconds before showing buttons");
        yield return new WaitForSecondsRealtime(buttonDelay);

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            Debug.Log("Restart button activated");
        }

        if (quitButton != null)
        {
            quitButton.gameObject.SetActive(true);
            Debug.Log("Quit button activated");
        }

        // Enable cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause game
        Time.timeScale = 0f;
        Debug.Log("Game paused, death sequence complete");
    }

    public void RestartGame()
    {
        Debug.Log("=== REINICIANDO JOGO COM MYSCENEMANAGER ===");

        // Reset all states before restart
        Time.timeScale = 1f;
        SettingsMenu.isPaused = false;

        // Reset cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Use the existing MySceneManager to reload the scene
        if (MySceneManager._inst != null)
        {
            Debug.Log("Usando MySceneManager.ReloadScene()");
            MySceneManager._inst.ReloadScene();
        }
        else
        {
            Debug.LogWarning("MySceneManager não encontrado, usando SceneManager diretamente");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void QuitToMenu()
    {
        Debug.Log("=== VOLTANDO PARA O MENU ===");

        // Reset all game states
        Time.timeScale = 1f;
        SettingsMenu.isPaused = false;

        // Reset cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Use the existing MySceneManager to go to menu
        if (MySceneManager._inst != null)
        {
            Debug.Log("Usando MySceneManager.LoadScene() para ir ao menu");
            MySceneManager._inst.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogWarning("MySceneManager não encontrado, usando SceneManager diretamente");
            SceneManager.LoadScene(menuSceneName);
        }
    }

    // For debugging
    [ContextMenu("Force Restart Game")]
    public void ForceRestartGame()
    {
        RestartGame();
    }
}
