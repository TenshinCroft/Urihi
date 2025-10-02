using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Toggle effectsToggle;

    [Header("Audio")]
    public AudioMixer audioMixer; // precisa de um AudioMixer com exposed param "MasterVolume"

    [Header("References")]
    public GameObject mainCamera; // arrasta a Main Camera aqui

    private ScreenShake screenShake;
    private PostProcessController postController;

    private void Start()
    {
        if (mainCamera != null)
        {
            screenShake = mainCamera.GetComponent<ScreenShake>();
            postController = mainCamera.GetComponent<PostProcessController>();
        }

        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1f);
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        effectsToggle.isOn = PlayerPrefs.GetInt("Effects", 1) == 1;

        ApplySensitivity(sensitivitySlider.value);
        ApplyVolume(volumeSlider.value);
        ApplyEffects(effectsToggle.isOn);

        sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
        effectsToggle.onValueChanged.AddListener(ApplyEffects);
    }

    private void ApplySensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        // Se houver PlayerController: PlayerController.sensitivity = value;
    }

    private void ApplyVolume(float value)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        }
        PlayerPrefs.SetFloat("Volume", value);
    }

    private void ApplyEffects(bool enabled)
    {
        PlayerPrefs.SetInt("Effects", enabled ? 1 : 0);

        if (screenShake != null)
            screenShake.enabled = enabled;

        if (postController != null)
            postController.EnableMotionBlur(enabled);
    }
}
