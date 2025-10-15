using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Este é o valor padrão de fallback
    public Transform playerBody;

    private PlayerControls inputActions;
    private Vector2 mouseInput;
    private float xRotation = 0f;

    // Campo para armazenar o valor original (para o reset do evento)
    private float _originalSensitivity;

    private void Awake()
    {
        inputActions = new PlayerControls();

        inputActions.Player.Look.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => mouseInput = Vector2.zero;

        // Salva o valor padrão do Inspector na Awake, antes do Start sobrescrever.
        // Se a sensibilidade fosse definida por um script externo, este valor seria sobrescrito.
        _originalSensitivity = mouseSensitivity;
    }

    // NOVO: Carregar a sensibilidade do sistema de configurações
    private void Start()
    {
        // ASSUMIR QUE O SettingsMenu.GetMouseSensitivity() É O MÉTODO CORRETO
        // Substitua 'SettingsMenu.GetMouseSensitivity()' pelo seu método/variável estática
        // que armazena a sensibilidade configurada pelo jogador.

        // Exemplo: Se você armazena em PlayerPrefs:
        // float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
        // mouseSensitivity = savedSensitivity;

        // OU se você tem um método no SettingsMenu:
        // if (SettingsMenu.Instance != null)
        // {
        //     mouseSensitivity = SettingsMenu.Instance.GetSavedSensitivity();
        // }

        // Mantenho apenas a declaração original para não mudar a funcionalidade,
        // mas estou destacando que este é o PONTO onde você precisa garantir
        // que a sensibilidade correta seja carregada.

        // Se você está usando o valor do Inspector, não mexa aqui.
        // Se o valor está sendo sobrescrito, o problema não está neste script, mas no
        // script que o sobrescreve (provavelmente um Settings/Options Manager).
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        // não mexe câmera se o jogo estiver pausado
        if (SettingsMenu.isPaused) return;

        // Usa Time.unscaledDeltaTime para evitar que o slow motion afete a sensibilidade
        float timeFactor = Time.unscaledDeltaTime;

        // não trava a rotação se GM1 estiver ativo
        float mouseX = mouseInput.x * mouseSensitivity * timeFactor;
        float mouseY = mouseInput.y * mouseSensitivity * timeFactor;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    // MÉTODOS DE CONTROLE DE SENSIBILIDADE (MANTIDOS)
    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    public void ResetSensitivity()
    {
        mouseSensitivity = _originalSensitivity;
    }
}