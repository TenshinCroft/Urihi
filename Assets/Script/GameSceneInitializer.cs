using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    // --- Chaves do PlayerPrefs (As mesmas usadas no SettingsMenu) ---
    private const string SensitivityKey = "MouseSensitivity";
    private const string VolumeKey = "MasterVolume";
    private const string MotionBlurKey = "MotionBlurEnabled";
    private const string ScreenShakeKey = "ScreenShakeEnabled";

    // --- Valores Padrão (Os mesmos usados no SettingsMenu) ---
    private const float DefaultSensitivity = 5f;
    private const float DefaultVolume = 1f;
    private const bool DefaultMotionBlur = true;
    private const bool DefaultScreenShake = true;

    [Header("Configurações da Cena")]
    public bool aplicarConfiguracoesNoInicio = true;
    public bool salvarConfiguracoesAoTrocarCena = true;

    // Removendo a referência ao 'MenuOptionsManager' para ler o PlayerPrefs diretamente.
    // private MenuOptionsManager optionsManager; 

    private void Awake()
    {
        // Se você ainda quiser usar o optionsManager, precisará garantir que ele seja um Singleton
        // optionsManager = MenuOptionsManager.Instance;

        if (aplicarConfiguracoesNoInicio)
        {
            AplicarConfiguracoesSalvas();
        }
    }

    private void AplicarConfiguracoesSalvas()
    {
        // --- FUNÇÕES HELPER PARA LER O PLAYERPREFS ---
        float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);
        bool GetBool(string key, bool defaultValue) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        // ---------------------------------------------

        // --- 1. SENSIBILIDADE ---
        float sensitivity = GetFloat(SensitivityKey, DefaultSensitivity);
        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.mouseSensitivity = sensitivity;
        }

        // --- 2. VOLUME ---
        float volume = GetFloat(VolumeKey, DefaultVolume);
        AudioListener.volume = volume;

        // --- 3. MOTION BLUR (Novo Tratamento) ---
        bool motionBlurEnabled = GetBool(MotionBlurKey, DefaultMotionBlur);
        PostProcessController postProcess = FindObjectOfType<PostProcessController>();
        if (postProcess != null)
        {
            postProcess.EnableMotionBlur(motionBlurEnabled);
        }

        // --- 4. SCREEN SHAKE (Novo Tratamento) ---
        // O ScreenShake deve ler sua própria preferência dentro do seu script (como corrigimos anteriormente).
        // Aqui, garantimos apenas que o PlayerPrefs já esteja carregado.
        bool screenShakeEnabled = GetBool(ScreenShakeKey, DefaultScreenShake);
        // Não precisamos fazer nada com a variável aqui, apenas a leitura já atualiza a cache do PlayerPrefs.

        // --- 5. QUALIDADE GRÁFICA (Mantido) ---
        // Se a qualidade gráfica for uma configuração persistente, você precisará de uma chave de PlayerPrefs para ela também.
        // QualitySettings.SetQualityLevel(optionsManager.GetGraphicsQuality());

        Debug.Log($"Configurações aplicadas na cena. Sensibilidade: {sensitivity}, Motion Blur: {motionBlurEnabled}");
    }

    private void OnDestroy()
    {
        // O PlayerPrefs salva automaticamente em certos eventos (como Application.quit),
        // mas é melhor garantir o salvamento se o MenuOptionsManager ainda for usado
        // e ele contiver outras lógicas de salvamento importantes.
        if (salvarConfiguracoesAoTrocarCena)
        {
            // Se você não está usando o optionsManager, comente esta linha.
            // optionsManager?.SaveSettings(); 

            // Caso contrário, use PlayerPrefs.Save() diretamente para garantir
            PlayerPrefs.Save();
        }
    }
}