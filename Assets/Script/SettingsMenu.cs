using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Toggle effectsToggle;
    public GameObject panel;

    [Header("References")]
    public Camera mainCamera;

    private ScreenShake screenShake;
    private PostProcessController postController;

    // flags globais
    public static bool isPaused = false;
    public static bool effectsAllowed = true;

    private void Awake()
    {
        if (mainCamera != null)
        {
            screenShake = mainCamera.GetComponent<ScreenShake>();
            postController = mainCamera.GetComponentInChildren<PostProcessController>();
        }
    }

    private void Start()
    {
        // Carrega valores salvos ou defaults
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        bool savedEffects = PlayerPrefs.GetInt("Effects", 1) == 1; // default ON

        // Aplica nos sliders/toggle
        sensitivitySlider.value = savedSensitivity;
        volumeSlider.value = savedVolume;
        effectsToggle.isOn = savedEffects;

        ApplySensitivity(savedSensitivity);
        ApplyVolume(savedVolume);
        ApplyEffects(savedEffects);

        // Começa com menu fechado
        SetUIActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // listeners
        sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
        effectsToggle.onValueChanged.AddListener(ApplyEffects);
    }

    private void Update()
    {
        // Toggle menu com ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        bool active = panel.activeSelf;
        SetUIActive(!active);
        PauseGame(!active);
    }

    public void ApplySensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        // Aqui você aplicaria ao seu input de camera/player
    }

    public void ApplyVolume(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
        // Aqui você aplicaria ao seu AudioListener ou mixer
    }

    public void ApplyEffects(bool enabled)
    {
        effectsAllowed = enabled;
        PlayerPrefs.SetInt("Effects", enabled ? 1 : 0);

        if (screenShake != null)
            screenShake.enabled = enabled && !isPaused;

        if (postController != null)
            postController.EnableMotionBlur(enabled && !isPaused);
    }

    private void SetUIActive(bool active)
    {
        if (panel != null)
            panel.SetActive(active);
        if (sensitivitySlider != null)
            sensitivitySlider.gameObject.SetActive(active);
        if (volumeSlider != null)
            volumeSlider.gameObject.SetActive(active);
        if (effectsToggle != null)
            effectsToggle.gameObject.SetActive(active);

        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void PauseGame(bool pause)
    {
        isPaused = pause;
        ApplyEffects(effectsAllowed); // atualiza efeitos conforme pause
        Time.timeScale = pause ? 0f : 1f; // pausa física do jogo
    }
}
