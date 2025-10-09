using UnityEngine;
using UnityEngine.UI;

public class CloseSettingsButton : MonoBehaviour
{
    [Header("References")]
    public GameObject settingsMenu; // O menu que deve ser fechado
    
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(CloseMenu);
        }

        // Se não foi atribuído manualmente, tenta encontrar automaticamente
        if (settingsMenu == null)
        {
            // Procura pelo Canvas pai que contenha MainMenuSettings
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null && parentCanvas.GetComponent<MainMenuSettings>() != null)
            {
                settingsMenu = parentCanvas.gameObject;
            }
        }
    }

    public void CloseMenu()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
            Debug.Log("Settings menu closed via close button");
        }
        else
        {
            Debug.LogWarning("Settings menu reference not found!");
        }
    }
}