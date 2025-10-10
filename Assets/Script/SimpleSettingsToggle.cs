using UnityEngine;
using UnityEngine.UI;

public class SimpleSettingsToggle : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject settingsMenu;
    
    private Button button;
    private bool menuStartsActive = true;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleSettingsMenu);
        }
        
        // Garante que o cursor está visível
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Esconde o menu no início se ele existe
        if (settingsMenu != null)
        {
            menuStartsActive = settingsMenu.activeSelf;
            settingsMenu.SetActive(false);
            Debug.Log("Settings menu hidden at start");
        }
    }

    public void ToggleSettingsMenu()
    {
        if (settingsMenu != null)
        {
            bool isActive = settingsMenu.activeSelf;
            settingsMenu.SetActive(!isActive);
            
            Debug.Log("Settings menu toggled: " + (!isActive ? "Opened" : "Closed"));
        }
        else
        {
            Debug.LogWarning("Settings menu not assigned!");
        }
    }

    public void CloseSettingsMenu()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
            Debug.Log("Settings menu closed");
        }
    }

    private void Update()
    {
        // Fecha o menu com ESC se estiver aberto
        if (settingsMenu != null && settingsMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettingsMenu();
        }
    }
}