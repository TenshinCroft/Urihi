using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfiguracoesMenu : MonoBehaviour
{
    [Header("Referências de UI do Menu Principal")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Toggle effectsToggle;
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

        if (effectsToggle != null)
        {
            effectsToggle.onValueChanged.AddListener(OnEffectsChanged);
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

        if (effectsToggle != null)
        {
            effectsToggle.isOn = optionsManager.GetEffectsEnabled();
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

    private void OnEffectsChanged(bool enabled)
    {
        if (optionsManager != null)
        {
            optionsManager.SetEffects(enabled);
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