using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Referências de UI")]
    public GameObject optionsMenu;
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    // Removido: public Toggle effectsToggle; 
    public Toggle motionBlurToggle;     // NOVO: Toggle para Motion Blur
    public Toggle screenShakeToggle;    // NOVO: Toggle para Screen Shake
    public Dropdown graphicsDropdown;

    [Header("Referências externas")]
    public PlayerLook playerLook;
    public AudioMixer audioMixer;
    public PostProcessController postProcessController;
    public ScreenShake screenShake;

    public static bool isPaused = false;
    // Removida: public static bool effectsEnabled = true;

    private MenuOptionsManager optionsManager;

    private void Awake()
    {
        Debug.Log("SettingsMenuManager Awake");
        optionsManager = MenuOptionsManager.Instance;

        InitializeMenuState();
        InitializeUIElements();
        LoadSettingsFromManager();

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializeMenuState()
    {
        if (optionsMenu != null)
        {
            optionsMenu.gameObject.SetActive(false);
            SetUIElementsActive(false);
            Debug.Log("optionsMenu inicializado como inativo");
        }
        else
        {
            Debug.LogWarning("optionsMenu NÃO está referenciado no Inspector!");
        }
    }

    private void SetUIElementsActive(bool active)
    {
        if (sensitivitySlider != null) sensitivitySlider.gameObject.SetActive(active);
        if (volumeSlider != null) volumeSlider.gameObject.SetActive(active);

        // ATUALIZADO: Ativar/Desativar os novos Toggles
        if (motionBlurToggle != null) motionBlurToggle.gameObject.SetActive(active);
        if (screenShakeToggle != null) screenShakeToggle.gameObject.SetActive(active);

        if (graphicsDropdown != null) graphicsDropdown.gameObject.SetActive(active);
    }

    private void InitializeUIElements()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }
        else
        {
            Debug.LogWarning("sensitivitySlider NÃO está referenciado no Inspector!");
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        else
        {
            Debug.LogWarning("volumeSlider NÃO está referenciado no Inspector!");
        }

        // NOVO: Listener para Motion Blur
        if (motionBlurToggle != null)
        {
            motionBlurToggle.onValueChanged.AddListener(SetMotionBlur);
        }
        else
        {
            Debug.LogWarning("motionBlurToggle NÃO está referenciado no Inspector!");
        }

        // NOVO: Listener para Screen Shake
        if (screenShakeToggle != null)
        {
            screenShakeToggle.onValueChanged.AddListener(SetScreenShake);
        }
        else
        {
            Debug.LogWarning("screenShakeToggle NÃO está referenciado no Inspector!");
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.onValueChanged.AddListener(SetGraphicsQuality);
            InitializeGraphicsDropdown();
        }
    }

    private void InitializeGraphicsDropdown()
    {
        // ... (o código do Dropdown permanece o mesmo)
        if (graphicsDropdown != null)
        {
            graphicsDropdown.ClearOptions();
            string[] qualityNames = QualitySettings.names;

            for (int i = 0; i < qualityNames.Length; i++)
            {
                graphicsDropdown.options.Add(new Dropdown.OptionData(qualityNames[i]));
            }

            graphicsDropdown.RefreshShownValue();
        }
    }

    private void LoadSettingsFromManager()
    {
        if (optionsManager == null) return;

        // Sensibilidade e Volume
        if (sensitivitySlider != null) sensitivitySlider.value = optionsManager.GetSensitivity();
        if (volumeSlider != null) volumeSlider.value = optionsManager.GetVolume();

        // NOVO: Motion Blur
        if (motionBlurToggle != null)
        {
            motionBlurToggle.isOn = optionsManager.GetMotionBlurEnabled();
            Debug.Log("motionBlurToggle inicializado como: " + optionsManager.GetMotionBlurEnabled());
        }

        // NOVO: Screen Shake
        if (screenShakeToggle != null)
        {
            screenShakeToggle.isOn = optionsManager.GetScreenShakeEnabled();
            Debug.Log("screenShakeToggle inicializado como: " + optionsManager.GetScreenShakeEnabled());
        }

        // Gráficos
        if (graphicsDropdown != null) graphicsDropdown.value = optionsManager.GetGraphicsQuality();
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
            SetUIElementsActive(true);
            Debug.Log("Menu ativado com filhos visíveis");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // Salva as configurações ao fechar o menu de pausa (boa prática)
        if (optionsManager != null) optionsManager.SaveSettings();

        if (optionsMenu != null)
        {
            optionsMenu.gameObject.SetActive(false);
            SetUIElementsActive(false);
            Debug.Log("Filhos do menu desativados");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetSensitivity(float value)
    {
        if (optionsManager != null)
        {
            optionsManager.SetSensitivity(value);
        }
    }

    public void SetVolume(float value)
    {
        if (optionsManager != null)
        {
            optionsManager.SetVolume(value);
        }
    }

    // REMOVIDO: public void SetEffects(bool enabled) { ... }

    // NOVO: Chama o setter específico para Motion Blur
    public void SetMotionBlur(bool enabled)
    {
        if (optionsManager != null)
        {
            optionsManager.SetMotionBlur(enabled);
            Debug.Log("Motion Blur toggled: " + enabled);
        }
    }

    // NOVO: Chama o setter específico para Screen Shake
    public void SetScreenShake(bool enabled)
    {
        if (optionsManager != null)
        {
            optionsManager.SetScreenShake(enabled);
            Debug.Log("Screen Shake toggled: " + enabled);
        }
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        if (optionsManager != null)
        {
            optionsManager.SetGraphicsQuality(qualityIndex);
        }
    }

    public void ResetToDefaults()
    {
        if (optionsManager != null)
        {
            optionsManager.ResetToDefaults();
            LoadSettingsFromManager();
            Debug.Log("Configurações restauradas para os padrões");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit game called");
        if (optionsManager != null)
        {
            optionsManager.SaveSettings();
        }
        Application.Quit();
    }
}