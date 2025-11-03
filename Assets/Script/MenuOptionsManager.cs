using UnityEngine;
using UnityEngine.Audio;

public class MenuOptionsManager : MonoBehaviour
{
    private static MenuOptionsManager instance;
    public static MenuOptionsManager Instance
    {
        get
        {
            // Padrão Singleton DontDestroyOnLoad
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
    public bool defaultMotionBlurEnabled = true;     // RENOMEADO
    public bool defaultScreenShakeEnabled = true;    // NOVO
    public int defaultGraphicsQuality = 2;

    [Header("Teclas para Configurações")]
    private const string SENSITIVITY_KEY = "MouseSensitivity";
    private const string VOLUME_KEY = "MasterVolume";
    private const string MOTION_BLUR_KEY = "MotionBlurEnabled";   // NOVO
    private const string SCREEN_SHAKE_KEY = "ScreenShakeEnabled"; // NOVO
    private const string GRAPHICS_QUALITY_KEY = "GraphicsQuality";

    // Variáveis de estado
    private float currentSensitivity;
    private float currentVolume;
    private bool currentMotionBlurEnabled;     // RENOMEADO
    private bool currentScreenShakeEnabled;    // NOVO
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
        // Carrega configurações existentes
        currentSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, defaultSensitivity);
        currentVolume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);

        // NOVO: Carrega Motion Blur
        currentMotionBlurEnabled = PlayerPrefs.GetInt(MOTION_BLUR_KEY, defaultMotionBlurEnabled ? 1 : 0) == 1;

        // NOVO: Carrega Screen Shake
        currentScreenShakeEnabled = PlayerPrefs.GetInt(SCREEN_SHAKE_KEY, defaultScreenShakeEnabled ? 1 : 0) == 1;

        currentGraphicsQuality = PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY, defaultGraphicsQuality);

        ApplySettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, currentSensitivity);
        PlayerPrefs.SetFloat(VOLUME_KEY, currentVolume);

        // NOVO: Salva Motion Blur
        PlayerPrefs.SetInt(MOTION_BLUR_KEY, currentMotionBlurEnabled ? 1 : 0);

        // NOVO: Salva Screen Shake
        PlayerPrefs.SetInt(SCREEN_SHAKE_KEY, currentScreenShakeEnabled ? 1 : 0);

        PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, currentGraphicsQuality);
        PlayerPrefs.Save();
    }

    private void ApplySettings()
    {
        SetVolume(currentVolume);
        SetGraphicsQuality(currentGraphicsQuality);

        // NOVO: Aplica os efeitos separadamente
        SetMotionBlur(currentMotionBlurEnabled);
        SetScreenShakeInternal(currentScreenShakeEnabled);

        // Removida: SettingsMenu.effectsEnabled = currentEffectsEnabled;
    }

    // --- MÉTODOS DE AJUSTE (SETTERS) ---

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
        // Se você usa AudioMixer, adicione a lógica aqui também:
        // audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(currentVolume, 0.0001f)) * 20);
        SaveSettings();
    }

    // RENOMEADO/ATUALIZADO: Trata apenas o Motion Blur
    public void SetMotionBlur(bool enabled)
    {
        currentMotionBlurEnabled = enabled;

        PostProcessController postProcess = FindObjectOfType<PostProcessController>();
        if (postProcess != null)
        {
            postProcess.EnableMotionBlur(enabled);
        }
        SaveSettings();
    }

    // NOVO MÉTODO: Trata apenas o Screen Shake.
    // Usamos um nome interno para evitar chamadas recursivas de SaveSettings
    private void SetScreenShakeInternal(bool enabled)
    {
        currentScreenShakeEnabled = enabled;

        // Esta lógica deve ser suficiente, mas a verificação principal acontece no script ScreenShake.
        if (!enabled)
        {
            ScreenShake screenShake = FindObjectOfType<ScreenShake>();
            if (screenShake != null)
            {
                screenShake.ResetCameraPosition(); // É mais seguro chamar ResetCameraPosition ou StopAllCoroutines
            }
        }
    }

    // NOVO MÉTODO: Setter público para o Menu (chama o método interno e salva)
    public void SetScreenShake(bool enabled)
    {
        SetScreenShakeInternal(enabled);
        SaveSettings();
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        currentGraphicsQuality = Mathf.Clamp(qualityIndex, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(currentGraphicsQuality);
        SaveSettings();
    }

    // --- MÉTODOS DE LEITURA (GETTERS) ---

    public float GetSensitivity() => currentSensitivity;
    public float GetVolume() => currentVolume;
    public int GetGraphicsQuality() => currentGraphicsQuality;

    // RENOMEADO: Get para Motion Blur
    public bool GetMotionBlurEnabled() => currentMotionBlurEnabled;

    // NOVO: Get para Screen Shake
    public bool GetScreenShakeEnabled() => currentScreenShakeEnabled;

    // REMOVIDO: GetEffectsEnabled() e a variável currentEffectsEnabled

    // --- MÉTODOS DE UTILIDADE ---

    public void ResetToDefaults()
    {
        SetSensitivity(defaultSensitivity);
        SetVolume(defaultVolume);
        SetMotionBlur(defaultMotionBlurEnabled);        // NOVO
        SetScreenShake(defaultScreenShakeEnabled);      // NOVO
        SetGraphicsQuality(defaultGraphicsQuality);
    }

    // ... (OnApplicationPause, OnApplicationFocus, OnDestroy permanecem os mesmos)

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveSettings();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveSettings();
    }

    private void OnDestroy()
    {
        SaveSettings();
    }
}