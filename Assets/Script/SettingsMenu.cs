using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    // --- Chaves para PlayerPrefs ---
    private const string SensitivityKey = "MouseSensitivity";
    private const string VolumeKey = "MasterVolume";
    private const string MotionBlurKey = "MotionBlurEnabled";
    private const string ScreenShakeKey = "ScreenShakeEnabled";

    // --- Valores Padrão (Fallback) ---
    private const float DefaultSensitivity = 5f;
    private const float DefaultVolume = 1f;
    private const bool DefaultMotionBlur = true;
    private const bool DefaultScreenShake = true;

    [Header("Referências de UI")]
    public GameObject optionsMenu;
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Toggle motionBlurToggle;     // O antigo effectsToggle foi substituído por este
    public Toggle screenShakeToggle;    // NOVO campo para o Toggle de Screen Shake

    [Header("Referências externas")]
    public PlayerLook playerLook;
    public AudioMixer audioMixer;
    public PostProcessController postProcessController;
    public ScreenShake screenShake;

    public static bool isPaused = false;

    private void Awake()
    {
        Debug.Log("SettingsMenu Awake - Tentando carregar configurações...");

        // 1. CARREGAR E APLICAR CONFIGURAÇÕES PERSISTENTES
        LoadSettings();

        // 2. INICIALIZAÇÃO DA UI E LISTENERS
        InitializeUI();

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializeUI()
    {
        // Oculta o menu de opções e seus filhos na inicialização
        if (optionsMenu != null)
        {
            optionsMenu.gameObject.SetActive(false);

            // Desativar Sliders e Toggles individualmente
            if (sensitivitySlider != null) sensitivitySlider.gameObject.SetActive(false);
            if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
            if (motionBlurToggle != null) motionBlurToggle.gameObject.SetActive(false);
            if (screenShakeToggle != null) screenShakeToggle.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("optionsMenu NÃO está referenciado no Inspector!");
        }

        // Adiciona Listeners
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
            Debug.Log("sensitivitySlider configurado com Listener.");
        }
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            Debug.Log("volumeSlider configurado com Listener.");
        }
        if (motionBlurToggle != null)
        {
            motionBlurToggle.onValueChanged.AddListener(SetMotionBlur);
            Debug.Log("motionBlurToggle configurado com Listener.");
        }
        if (screenShakeToggle != null)
        {
            screenShakeToggle.onValueChanged.AddListener(SetScreenShake);
            Debug.Log("screenShakeToggle configurado com Listener.");
        }
    }

    private void LoadSettings()
    {
        // --- 1. Sensibilidade ---
        float loadedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity);
        if (playerLook != null) playerLook.mouseSensitivity = loadedSensitivity;
        if (sensitivitySlider != null) sensitivitySlider.value = loadedSensitivity;
        Debug.Log($"Sensibilidade carregada: {loadedSensitivity}");

        // --- 2. Volume ---
        float loadedVolume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
        SetVolume(loadedVolume);
        if (volumeSlider != null) volumeSlider.value = loadedVolume;
        Debug.Log($"Volume carregado: {loadedVolume}");

        // --- 3. Motion Blur ---
        bool loadedMotionBlur = PlayerPrefs.GetInt(MotionBlurKey, DefaultMotionBlur ? 1 : 0) == 1;
        SetMotionBlur(loadedMotionBlur);
        if (motionBlurToggle != null) motionBlurToggle.isOn = loadedMotionBlur;
        Debug.Log($"Motion Blur carregado: {loadedMotionBlur}");

        // --- 4. Screen Shake ---
        bool loadedScreenShake = PlayerPrefs.GetInt(ScreenShakeKey, DefaultScreenShake ? 1 : 0) == 1;
        SetScreenShake(loadedScreenShake);
        if (screenShakeToggle != null) screenShakeToggle.isOn = loadedScreenShake;
        Debug.Log($"Screen Shake carregado: {loadedScreenShake}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (optionsMenu != null)
        {
            optionsMenu.gameObject.SetActive(true);

            // Ativar Sliders e Toggles individualmente
            if (sensitivitySlider != null) sensitivitySlider.gameObject.SetActive(true);
            if (volumeSlider != null) volumeSlider.gameObject.SetActive(true);
            if (motionBlurToggle != null) motionBlurToggle.gameObject.SetActive(true);
            if (screenShakeToggle != null) screenShakeToggle.gameObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (optionsMenu != null)
        {
            optionsMenu.gameObject.SetActive(false);

            // Desativar Sliders e Toggles individualmente
            if (sensitivitySlider != null) sensitivitySlider.gameObject.SetActive(false);
            if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
            if (motionBlurToggle != null) motionBlurToggle.gameObject.SetActive(false);
            if (screenShakeToggle != null) screenShakeToggle.gameObject.SetActive(false);
        }

        SaveSettings(); // Salva ao sair do menu

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Configurações salvas no disco (PlayerPrefs.Save())");
    }

    public void SetSensitivity(float value)
    {
        if (playerLook != null) playerLook.mouseSensitivity = value;
        PlayerPrefs.SetFloat(SensitivityKey, value);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        }
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    public void SetMotionBlur(bool enabled)
    {
        if (postProcessController != null)
            postProcessController.EnableMotionBlur(enabled);

        Debug.Log("Motion Blur toggled: " + enabled);

        PlayerPrefs.SetInt(MotionBlurKey, enabled ? 1 : 0);
    }

    public void SetScreenShake(bool enabled)
    {
        if (!enabled && screenShake != null)
            screenShake.StopAllCoroutines();

        Debug.Log("Screen Shake toggled: " + enabled);

        PlayerPrefs.SetInt(ScreenShakeKey, enabled ? 1 : 0);
    }

    public void QuitGame()
    {
        SaveSettings();
        Debug.Log("Quit game called");
        Application.Quit();
    }
}