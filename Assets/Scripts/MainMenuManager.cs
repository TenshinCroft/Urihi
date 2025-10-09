using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject settingsMenuPrefab;
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    
    private GameObject settingsMenuInstance;
    private bool settingsMenuOpen = false;

    private void Start()
    {
        SetupButtons();
        SetupCursor();
    }

    private void SetupButtons()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ToggleSettingsMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void SetupCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Permite fechar o menu de configurações com ESC
        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenuOpen)
        {
            CloseSettingsMenu();
        }
    }

    public void PlayGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("CasaInterna");
    }

    public void ToggleSettingsMenu()
    {
        if (settingsMenuOpen)
        {
            CloseSettingsMenu();
        }
        else
        {
            OpenSettingsMenu();
        }
    }

    private void OpenSettingsMenu()
    {
        if (settingsMenuPrefab != null && settingsMenuInstance == null)
        {
            settingsMenuInstance = Instantiate(settingsMenuPrefab);
            settingsMenuOpen = true;
            
            // Garante que o menu apareça na frente
            Canvas settingsCanvas = settingsMenuInstance.GetComponent<Canvas>();
            if (settingsCanvas != null)
            {
                settingsCanvas.sortingOrder = 100;
            }
            
            Debug.Log("Settings menu opened");
        }
    }

    private void CloseSettingsMenu()
    {
        if (settingsMenuInstance != null)
        {
            Destroy(settingsMenuInstance);
            settingsMenuInstance = null;
            settingsMenuOpen = false;
            Debug.Log("Settings menu closed");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}