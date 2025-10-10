using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Referências de UI")]
    public GameObject optionsMenu;
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Toggle effectsToggle;
    public Dropdown graphicsDropdown;

    [Header("Referências externas")]
    public PlayerLook playerLook;
    public AudioMixer audioMixer;
    public PostProcessController postProcessController;
    public ScreenShake screenShake;

    public static bool isPaused = false;
    public static bool effectsEnabled = true;

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
        if (effectsToggle != null) effectsToggle.gameObject.SetActive(active);
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

        if (effectsToggle != null)
        {
            effectsToggle.onValueChanged.AddListener(SetEffects);
        }
        else
        {
            Debug.LogWarning("effectsToggle NÃO está referenciado no Inspector!");
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.onValueChanged.AddListener(SetGraphicsQuality);
            InitializeGraphicsDropdown();
        }
    }

    private void InitializeGraphicsDropdown()
    {
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

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = optionsManager.GetSensitivity();
            Debug.Log("sensitivitySlider inicializado com valor: " + optionsManager.GetSensitivity());
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = optionsManager.GetVolume();
            Debug.Log("volumeSlider inicializado com valor: " + optionsManager.GetVolume());
        }

        if (effectsToggle != null)
        {
            effectsToggle.isOn = optionsManager.GetEffectsEnabled();
            effectsEnabled = optionsManager.GetEffectsEnabled();
            Debug.Log("effectsToggle inicializado como: " + optionsManager.GetEffectsEnabled());
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = optionsManager.GetGraphicsQuality();
            Debug.Log("graphicsDropdown inicializado com valor: " + optionsManager.GetGraphicsQuality());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressionado, toggle pause");
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Debug.Log("Resuming game");
            ResumeGame();
        }
        else
        {
            Debug.Log("Pausing game");
            PauseGame();
        }
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
            Debug.Log("Sensibilidade ajustada para " + value);
        }
    }

    public void SetVolume(float value)
    {
        if (optionsManager != null)
        {
            optionsManager.SetVolume(value);
            Debug.Log("Volume ajustado para " + value);
        }
    }

    public void SetEffects(bool enabled)
    {
        if (optionsManager != null)
        {
            optionsManager.SetEffects(enabled);
            Debug.Log("Effects toggled: " + enabled);
        }
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        if (optionsManager != null)
        {
            optionsManager.SetGraphicsQuality(qualityIndex);
            Debug.Log("Graphics quality set to: " + QualitySettings.names[qualityIndex]);
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