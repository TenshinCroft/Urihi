using UnityEngine;
using UnityEngine.Audio;

public class MenuOptionsManager : MonoBehaviour
{
    private static MenuOptionsManager instance;
    public static MenuOptionsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MenuOptionsManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("MenuOptionsManager");
                    instance = go.AddComponent<MenuOptionsManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Configurações Padrão")]
    public float defaultSensitivity = 2f;
    public float defaultVolume = 1f;
    public bool defaultEffectsEnabled = true;
    public int defaultGraphicsQuality = 2;

    [Header("Teclas para Configurações")]
    private const string SENSITIVITY_KEY = "MouseSensitivity";
    private const string VOLUME_KEY = "Volume";
    private const string EFFECTS_KEY = "EffectsEnabled";
    private const string GRAPHICS_QUALITY_KEY = "GraphicsQuality";

    private float currentSensitivity;
    private float currentVolume;
    private bool currentEffectsEnabled;
    private int currentGraphicsQuality;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadSettings();
    }

    public void LoadSettings()
    {
        currentSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, defaultSensitivity);
        currentVolume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
        currentEffectsEnabled = PlayerPrefs.GetInt(EFFECTS_KEY, defaultEffectsEnabled ? 1 : 0) == 1;
        currentGraphicsQuality = PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY, defaultGraphicsQuality);

        ApplySettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, currentSensitivity);
        PlayerPrefs.SetFloat(VOLUME_KEY, currentVolume);
        PlayerPrefs.SetInt(EFFECTS_KEY, currentEffectsEnabled ? 1 : 0);
        PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, currentGraphicsQuality);
        PlayerPrefs.Save();
    }

    private void ApplySettings()
    {
        SetVolume(currentVolume);
        SetGraphicsQuality(currentGraphicsQuality);
        SettingsMenu.effectsEnabled = currentEffectsEnabled;
    }

    public void SetSensitivity(float sensitivity)
    {
        currentSensitivity = Mathf.Clamp(sensitivity, 0.1f, 10f);
        
        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.mouseSensitivity = currentSensitivity;
        }
        
        SaveSettings();
    }

    public void SetVolume(float volume)
    {
        currentVolume = Mathf.Clamp01(volume);
        AudioListener.volume = currentVolume;
        SaveSettings();
    }

    public void SetEffects(bool enabled)
    {
        currentEffectsEnabled = enabled;
        SettingsMenu.effectsEnabled = enabled;

        PostProcessController postProcess = FindObjectOfType<PostProcessController>();
        if (postProcess != null)
        {
            postProcess.EnableMotionBlur(enabled);
        }

        if (!enabled)
        {
            ScreenShake screenShake = FindObjectOfType<ScreenShake>();
            if (screenShake != null)
            {
                screenShake.StopAllCoroutines();
            }
        }

        SaveSettings();
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        currentGraphicsQuality = Mathf.Clamp(qualityIndex, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(currentGraphicsQuality);
        SaveSettings();
    }

    public float GetSensitivity()
    {
        return currentSensitivity;
    }

    public float GetVolume()
    {
        return currentVolume;
    }

    public bool GetEffectsEnabled()
    {
        return currentEffectsEnabled;
    }

    public int GetGraphicsQuality()
    {
        return currentGraphicsQuality;
    }

    public void ResetToDefaults()
    {
        SetSensitivity(defaultSensitivity);
        SetVolume(defaultVolume);
        SetEffects(defaultEffectsEnabled);
        SetGraphicsQuality(defaultGraphicsQuality);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSettings();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveSettings();
        }
    }

    private void OnDestroy()
    {
        SaveSettings();
    }
}