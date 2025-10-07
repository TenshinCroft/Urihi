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

    private bool isDead = false;
    private Player playerScript;
    private PlayerLook playerLook;

    private void Awake()
    {
        // Get references
        if (playerCamera == null)
            playerCamera = Camera.main;

        playerScript = GetComponent<Player>();

        // Find PlayerLook component on the camera
        if (playerCamera != null)
            playerLook = playerCamera.GetComponent<PlayerLook>();

        // Setup audio
        if (deathAudioSource == null)
        {
            deathAudioSource = gameObject.AddComponent<AudioSource>();
            deathAudioSource.playOnAwake = false;
            deathAudioSource.spatialBlend = 0f; // 2D sound
        }

        // Setup UI
        if (deathUI != null)
            deathUI.SetActive(false);

        // Setup button events
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Initialize canvas groups
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.alpha = 0f;
            gameOverText.gameObject.SetActive(false);
        }
    }

    public void TriggerDeath(Transform enemyTransform)
    {
        if (isDead) return;

        isDead = true;
        enemy = enemyTransform;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // 1. Disable player controls
        if (playerScript != null)
            playerScript.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        // 2. Play death sound
        if (deathSound != null && deathAudioSource != null)
        {
            deathAudioSource.clip = deathSound;
            deathAudioSource.volume = deathVolume;
            deathAudioSource.Play();
        }

        // 3. Lock camera on enemy
        float elapsed = 0f;
        Vector3 initialCameraRotation = playerCamera.transform.eulerAngles;

        while (elapsed < deathSequenceDuration)
        {
            if (enemy != null && playerCamera != null)
            {
                Vector3 directionToEnemy = (enemy.position - playerCamera.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);

                playerCamera.transform.rotation = Quaternion.Slerp(
                    playerCamera.transform.rotation,
                    targetRotation,
                    cameraLookSpeed * Time.deltaTime
                );
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. Show death UI
        ShowDeathUI();
    }

    private void ShowDeathUI()
    {
        if (deathUI != null)
            deathUI.SetActive(true);

        StartCoroutine(AnimateDeathUI());
    }

    private IEnumerator AnimateDeathUI()
    {
        // 1. Fade in black screen
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);

            while (blackScreen.alpha < 1f)
            {
                blackScreen.alpha += blackScreenFadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // 2. Wait and show "Você morreu" text
        yield return new WaitForSecondsRealtime(textAppearDelay);

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);

            while (gameOverText.alpha < 1f)
            {
                gameOverText.alpha += 2f * Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // 3. Wait and show buttons
        yield return new WaitForSecondsRealtime(buttonsAppearDelay - textAppearDelay);

        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        if (quitButton != null)
            quitButton.gameObject.SetActive(true);

        // Enable cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause the game
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
