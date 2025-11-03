using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfiguracoesMenu : MonoBehaviour
{
    [Header("Referências de UI do Menu Principal")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    // Removido: public Toggle effectsToggle; 
    public Toggle motionBlurToggle;     // NOVO: Toggle para Motion Blur
    public Toggle screenShakeToggle;    // NOVO: Toggle para Screen Shake
    public Dropdown graphicsDropdown;
    public Button resetButton;
    public Button applyButton;

    private MenuOptionsManager optionsManager;

    private void Start()
    {
        optionsManager = MenuOptionsManager.Instance;
        InitializeUIElements();
        LoadCurrentSettings();
    }

    private void InitializeUIElements()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // NOVO: Listener para Motion Blur
        if (motionBlurToggle != null)
        {
            motionBlurToggle.onValueChanged.AddListener(OnMotionBlurChanged);
        }

        // NOVO: Listener para Screen Shake
        if (screenShakeToggle != null)
        {
            screenShakeToggle.onValueChanged.AddListener(OnScreenShakeChanged);
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);
            InitializeGraphicsDropdown();
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetToDefaults);
        }

        if (applyButton != null)
        {
            applyButton.onClick.AddListener(ApplyAndSaveSettings);
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

    private void LoadCurrentSettings()
    {
        if (optionsManager == null) return;

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = optionsManager.GetSensitivity();
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = optionsManager.GetVolume();
        }

        // NOVO: Carrega Motion Blur
        if (motionBlurToggle != null)
        {
            motionBlurToggle.isOn = optionsManager.GetMotionBlurEnabled();
        }

        // NOVO: Carrega Screen Shake
        if (screenShakeToggle != null)
        {
            screenShakeToggle.isOn = optionsManager.GetScreenShakeEnabled();
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = optionsManager.GetGraphicsQuality();
        }
    }

    private void OnSensitivityChanged(float value)
    {
        if (optionsManager != null)
        {
            optionsManager.SetSensitivity(value);
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (optionsManager != null)
        {
            optionsManager.SetVolume(value);
        }
    }

    // REMOVIDO: private void OnEffectsChanged(bool enabled) { ... }

    // NOVO: Chama o setter específico para Motion Blur
    private void OnMotionBlurChanged(bool enabled)
    {
        if (optionsManager != null)
        {
            optionsManager.SetMotionBlur(enabled);
        }
    }

    // NOVO: Chama o setter específico para Screen Shake
    private void OnScreenShakeChanged(bool enabled)
    {
        if (optionsManager != null)
        {
            optionsManager.SetScreenShake(enabled);
        }
    }

    private void OnGraphicsQualityChanged(int qualityIndex)
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
            // Recarrega a UI para exibir os novos valores padrão
            LoadCurrentSettings();
        }
    }

    public void ApplyAndSaveSettings()
    {
        if (optionsManager != null)
        {
            optionsManager.SaveSettings();
        }
    }

    public void StartGame(string sceneName)
    {
        ApplyAndSaveSettings();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        ApplyAndSaveSettings();
        Application.Quit();
    }
}