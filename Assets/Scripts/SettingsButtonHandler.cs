using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonHandler : MonoBehaviour
{
    [Header("References")]
    public GameObject settingsMenuPrefab;
    
    private GameObject settingsMenuInstance;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSettingsButtonClick);
        }
        
        // Garante que o cursor está visível na tela inicial
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnSettingsButtonClick()
    {
        if (settingsMenuInstance == null)
        {
            OpenSettingsMenu();
        }
        else
        {
            CloseSettingsMenu();
        }
    }

    private void OpenSettingsMenu()
    {
        if (settingsMenuPrefab != null)
        {
            settingsMenuInstance = Instantiate(settingsMenuPrefab);
            
            // Garante que o menu apareça na frente
            Canvas settingsCanvas = settingsMenuInstance.GetComponent<Canvas>();
            if (settingsCanvas != null)
            {
                settingsCanvas.sortingOrder = 100;
                settingsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            
            // Conecta o sistema de fechar do MainMenuSettings com este handler
            MainMenuSettings mainMenuSettings = settingsMenuInstance.GetComponent<MainMenuSettings>();
            if (mainMenuSettings != null)
            {
                mainMenuSettings.SetCloseHandler(this);
                Time.timeScale = 1f; // Não pausa na tela inicial
            }
            
            // Fallback para SettingsMenu original se estiver usando o prefab antigo
            SettingsMenu settingsScript = settingsMenuInstance.GetComponent<SettingsMenu>();
            if (settingsScript != null)
            {
                Time.timeScale = 1f;
            }
            
            Debug.Log("Settings menu opened from main menu");
        }
        else
        {
            Debug.LogWarning("Settings menu prefab not assigned!");
        }
    }

    private void CloseSettingsMenu()
    {
        if (settingsMenuInstance != null)
        {
            Destroy(settingsMenuInstance);
            settingsMenuInstance = null;
            Debug.Log("Settings menu closed");
        }
    }

    // Método público para que o MainMenuSettings possa fechar o menu
    public void CloseMenu()
    {
        CloseSettingsMenu();
    }

    private void Update()
    {
        // Remove o controle de ESC daqui - deixa o MainMenuSettings controlar
        // ESC será controlado pelo MainMenuSettings
    }
}