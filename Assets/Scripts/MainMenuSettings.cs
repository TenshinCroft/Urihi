using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuSettings : MonoBehaviour
{
    [Header("Referências de UI")]
    public GameObject optionsMenu;
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Toggle effectsToggle;
    public Button closeButton;

    [Header("Referências externas")]
    public AudioMixer audioMixer;

    public static bool effectsEnabled = true;
    private SettingsButtonHandler closeHandler;

    private void Awake()
    {
        Debug.Log("MainMenuSettings Awake");
        
        // Na tela inicial, não gerenciamos pausa
        SetupUI();
        SetupSliders();
        SetupToggles();
        SetupCloseButton();
        
        // Cursor sempre visível no menu principal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Método para o SettingsButtonHandler definir a referência
    public void SetCloseHandler(SettingsButtonHandler handler)
    {
        closeHandler = handler;
    }

    private void SetupUI()
    {
        // Não ativa automaticamente - será controlado pelo SimpleSettingsToggle
        if (optionsMenu != null)
        {
            Debug.Log("Main menu settings configurado (não ativado automaticamente)");
        }
    }

    private void SetupSliders()
    {
        if (sensitivitySlider != null)
        {
            // Para o menu principal, pode usar um valor padrão
            sensitivitySlider.value = 5f;
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
            Debug.Log("Sensitivity slider configurado para menu principal");
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            volumeSlider.value = AudioListener.volume;
            Debug.Log("Volume slider configurado para menu principal com valor: " + AudioListener.volume);
        }
    }

    private void SetupToggles()
    {
        if (effectsToggle != null)
        {
            effectsToggle.onValueChanged.AddListener(SetEffects);
            effectsToggle.isOn = effectsEnabled;
            Debug.Log("Effects toggle configurado para menu principal");
        }
    }

    private void SetupCloseButton()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
            Debug.Log("Close button configurado para menu principal");
        }
    }

    public void SetSensitivity(float value)
    {
        // Salva a sensibilidade para quando o jogo começar
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        Debug.Log("Sensibilidade salva para: " + value);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        Debug.Log("Volume ajustado para " + value + " no menu principal");
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        }
        
        // Salva o volume
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetEffects(bool enabled)
    {
        effectsEnabled = enabled;
        PlayerPrefs.SetInt("EffectsEnabled", enabled ? 1 : 0);
        Debug.Log("Effects toggled no menu principal: " + enabled);
    }

    public void CloseSettings()
    {
        Debug.Log("CloseSettings chamado");
        
        // Se temos um handler, usa ele para fechar adequadamente
        if (closeHandler != null)
        {
            closeHandler.CloseMenu();
        }
        else
        {
            // Fallback: destrói o GameObject diretamente
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // ESC será controlado pelo SimpleSettingsToggle
        // Removido daqui para evitar conflitos
    }
}