using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    [Header("Configurações da Cena")]
    public bool aplicarConfiguracoesNoInicio = true;
    public bool salvarConfiguracoesAoTrocarCena = true;

    private MenuOptionsManager optionsManager;

    private void Awake()
    {
        optionsManager = MenuOptionsManager.Instance;
        
        if (aplicarConfiguracoesNoInicio)
        {
            AplicarConfiguracoesSalvas();
        }
    }

    private void AplicarConfiguracoesSalvas()
    {
        if (optionsManager == null) return;

        optionsManager.LoadSettings();
        
        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.mouseSensitivity = optionsManager.GetSensitivity();
        }

        AudioListener.volume = optionsManager.GetVolume();
        SettingsMenu.effectsEnabled = optionsManager.GetEffectsEnabled();

        PostProcessController postProcess = FindObjectOfType<PostProcessController>();
        if (postProcess != null)
        {
            postProcess.EnableMotionBlur(optionsManager.GetEffectsEnabled());
        }

        QualitySettings.SetQualityLevel(optionsManager.GetGraphicsQuality());

        Debug.Log("Configurações aplicadas na cena: " + gameObject.scene.name);
    }

    private void OnDestroy()
    {
        if (salvarConfiguracoesAoTrocarCena && optionsManager != null)
        {
            optionsManager.SaveSettings();
        }
    }
}