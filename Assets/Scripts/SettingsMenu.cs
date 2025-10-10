using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("Refer�ncias de UI")]
    public GameObject optionsMenu;
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public Toggle effectsToggle;

    [Header("Refer�ncias externas")]
    public PlayerLook playerLook;
    public AudioMixer audioMixer;
    public PostProcessController postProcessController;
    public ScreenShake screenShake;

    public static bool isPaused = false;
    public static bool effectsEnabled = true;

    private void Awake()
    {
        Debug.Log("SettingsMenu Awake");

        if (optionsMenu != null)
        {
            optionsMenu.gameObject.SetActive(false);
            sensitivitySlider.gameObject.SetActive(false);
            volumeSlider.gameObject.SetActive(false);
            effectsToggle.gameObject.SetActive(false);
            Debug.Log("optionsMenu inicializado como inativo");
        }
        else
        {
            Debug.LogWarning("optionsMenu N�O est� referenciado no Inspector!");
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
            if (playerLook != null)
            {
                sensitivitySlider.value = playerLook.mouseSensitivity;
                Debug.Log("sensitivitySlider inicializado com valor: " + playerLook.mouseSensitivity);
            }
        }
        else
        {
            Debug.LogWarning("sensitivitySlider N�O est� referenciado no Inspector!");
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            // Inicializa o slider com o volume atual do AudioListener
            volumeSlider.value = AudioListener.volume;
            Debug.Log("volumeSlider referenciado e inicializado com valor: " + AudioListener.volume);
        }
        else
        {
            Debug.LogWarning("volumeSlider N�O est� referenciado no Inspector!");
        }

        if (effectsToggle != null)
        {
            effectsToggle.onValueChanged.AddListener(SetEffects);
            effectsToggle.isOn = true;
            effectsEnabled = true;
            Debug.Log("effectsToggle inicializado como ON");
        }
        else
        {
            Debug.LogWarning("effectsToggle N�O est� referenciado no Inspector!");
        }

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Detecta a tecla ESC para abrir/fechar o menu
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
            sensitivitySlider.gameObject.SetActive(true);
            volumeSlider.gameObject.SetActive(true);
            effectsToggle.gameObject.SetActive(true);
            Debug.Log("Menu ativado com filhos vis�veis");
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
            // Mant�m o Canvas ativo, mas desativa os filhos
            optionsMenu.gameObject.SetActive(false);
            sensitivitySlider.gameObject.SetActive(false);
            volumeSlider.gameObject.SetActive(false);
            effectsToggle.gameObject.SetActive(false);
            Debug.Log("Filhos do menu desativados");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Fun��o para ativar/desativar todos os filhos de um GameObject
    private void SetChildrenActive(GameObject parent, bool active)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    public void SetSensitivity(float value)
    {
        if (playerLook != null)
        {
            playerLook.mouseSensitivity = value;
            Debug.Log("Sensibilidade ajustada para " + value);
        }
    }

    public void SetVolume(float value)
    {
        // Controla o volume global do Unity (0.0 a 1.0)
        AudioListener.volume = value;
        Debug.Log("Volume ajustado para " + value);
        
        // Se o audioMixer estiver configurado, também controla pelo mixer
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        }
    }

    public void SetEffects(bool enabled)
    {
        effectsEnabled = enabled;
        Debug.Log("Effects toggled: " + enabled);

        if (postProcessController != null)
            postProcessController.EnableMotionBlur(enabled);

        if (!enabled && screenShake != null)
            screenShake.StopAllCoroutines();
    }

    public void QuitGame()
    {
        Debug.Log("Quit game called");
        Application.Quit();
    }
}
