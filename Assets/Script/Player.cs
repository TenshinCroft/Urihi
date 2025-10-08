
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic; // NOVO: Para usar a Lista

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private bool isFeedbackActive = false;
    // ANIMAÇÃO
    private Animator _animator;

    [Header("Dica de Interação")]
    public TMP_Text interactionHintText;

    // CARTA
    [Header("Carta")]
    private GameObject cartaAtualUI;  // carta ativa no momento
    public bool cartaAberta = false;

    [Tooltip("Imagem que aparece no canto da tela junto com qualquer carta")]
    public GameObject imagemExtraUI; // <- arraste no Inspector (Canvas)

    // FEEDBACK UI
    [Header("Feedback UI")]
    public TMP_Text feedbackText; // O objeto de texto na UI para mensagens de feedback
    public float feedbackDuration = 3f;
    private Coroutine hideFeedbackCoroutine;


    // INTERAÇÃO
    [Header("Interação")]
    public float _alcanceDeInteração = 8f;
    public LayerMask _mascaraDeInteração;
    public GameObject _inimigo;
    private bool _intPressed;
    private bool _runPressed;
    private bool _lntPressed;
    private bool _gmod1Pressed;
    private bool _giz;
    public bool _GameMode1Enabled = false;
    public bool _gmod1;
    public Camera _pCam;

    // MOVIMENTO
    [Header("Movimento")]
    public float _velocidade = 12;
    public float _multiplicadorDeVelocidade = 1.5f;
    public float _alturaDoPulo = 2f;
    public float _velocidadeNoAr = 0.5f;
    private float _g = 9.81f;
    private float _speed;
    private Vector2 _inpMove;
    private Vector3 _vel;
    private Vector3 _m;
    public bool _isOnG;
    public bool _jPressed;
    public bool _lntrOn;
    private float _cntrMult;

    // CHÃO
    [Header("Verificação de Chão")]
    public float _distanciaDoChão = 0.4f;
    public LayerMask _chão;
    public Transform _gCheck;

    // LANTERNA
    [Header("Lanterna")]
    public AudioSource _lanternAudioSource;
    public AudioClip _somLigarLanterna;
    public AudioClip _somDesligarLanterna;
    [Range(0f, 1f)] public float _lanternVolume = 1f;

    // CONTROLER
    [Header("Sistema de Input")]
    [HideInInspector] public int _i = 0;
    private PlayerControls _inpActions;
    private CharacterController _cntr;

    // ===== AWAKE =====
    public void Awake()
    {
        _cntr = GetComponent<CharacterController>();
        _inpActions = new PlayerControls();

        _inpActions.Player.Move.performed += ctx => _inpMove = ctx.ReadValue<Vector2>();
        _inpActions.Player.Move.canceled += ctx => _inpMove = Vector2.zero;
        _inpActions.Player.Jump.performed += ctx => _jPressed = true;
        _inpActions.Player.Interact.performed += ctx => _intPressed = true;
        _inpActions.Player.Correr.performed += ctx => _runPressed = true;
        _inpActions.Player.Correr.canceled += ctx => _runPressed = false;
        _inpActions.Player.Lanterna.performed += ctx => _lntPressed = true;
        _inpActions.Player.GAMEMODE1.performed += ctx => _gmod1Pressed = true;

        if (_lanternAudioSource == null)
            _lanternAudioSource = GetComponent<AudioSource>();

        if (_lanternAudioSource != null)
        {
            _lanternAudioSource.playOnAwake = false;
            _lanternAudioSource.loop = false;
            _lanternAudioSource.spatialBlend = 0f;
        }
        // Inicializa Animator
        _animator = GetComponent<Animator>();
    }

    public void OnEnable() => _inpActions.Enable();
    public void OnDisable() => _inpActions.Disable();

    // ===== START =====
    public void Start()
    {
        if (interactionHintText != null)
            interactionHintText.gameObject.SetActive(false);

        if (_pCam == null)
            _pCam = Camera.main;

        if (cartaAtualUI != null)
            cartaAtualUI.SetActive(false);

        if (imagemExtraUI != null)
            imagemExtraUI.SetActive(false); // começa desativada

        // NOVO: Desativa o texto de feedback ao iniciar
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ===== UPDATE =====
    public void Update()
    {
        // Toggle do GameMode1
        if (_gmod1Pressed && _GameMode1Enabled)
        {
            _gmod1 = !_gmod1;
            _lntPressed = false;
        }
        _gmod1Pressed = false;

        if (!_GameMode1Enabled && _gmod1)
            _gmod1 = false;

        gMode(); // GM1 agora não desativa rotação da câmera

        // Carta aberta
        if (cartaAberta)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
                FecharCarta();
            return;
        }

        if (_inimigo != null && _inimigo.GetComponent<Enemy>()._plyAtq)
            _inpActions.Disable();
        else
            _inpActions.Enable();

        if (SettingsMenu.isPaused) return; // pausa movimentação, inputs e shake

        Run();

        _isOnG = Physics.CheckSphere(_gCheck.position, _distanciaDoChão, _chão);
        if (_isOnG && _vel.y < 0f) _vel.y = -2f;

        _m = transform.right * _inpMove.x + transform.forward * _inpMove.y;
        _cntrMult = _isOnG ? 1f : _velocidadeNoAr;
        if (_cntr.enabled)
            _cntr.Move(_m * _speed * _cntrMult * Time.deltaTime);

        if (_jPressed && _isOnG)
        {
            _vel.y = Mathf.Sqrt(_alturaDoPulo * 2f * _g);
            _jPressed = false;
        }

        if (_intPressed)
        {
            InteractWithObject();
            _intPressed = false;
        }

        if (_lntPressed)
        {
            LanternPres();
            _lntPressed = false;
        }

        _vel.y += -_g * Time.deltaTime;
        if (_cntr.enabled)
            _cntr.Move(_vel * Time.deltaTime);

        CheckAnimations();
        CheckForInteractable();
    }


    private void CheckForInteractable()
    {
       
        // Se carta está aberta, GM1 ativo ou feedback ativo, esconde dica
        if (cartaAberta || _gmod1 || isFeedbackActive)
        {
            if (interactionHintText != null)
                interactionHintText.gameObject.SetActive(false);
            return;
        }

        Ray ray = new Ray(_pCam.transform.position, _pCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _alcanceDeInteração, _mascaraDeInteração, QueryTriggerInteraction.Collide))
        {
            // Tags interagíveis
            if (hit.collider.CompareTag("Item") ||
                hit.collider.CompareTag("Carta") ||
                hit.collider.CompareTag("Porta") ||
                hit.collider.CompareTag("Quadro"))
            {
                if (interactionHintText != null)
                {
                    interactionHintText.text = "Pressione LMB";
                    interactionHintText.gameObject.SetActive(true);
                }
                return;
            }
        }

        // Se não estiver olhando para nada interagível
        if (interactionHintText != null)
            interactionHintText.gameObject.SetActive(false);
    }

    // ===== FEEDBACK UI (NOVO MÉTODO) =====
    public void ShowFeedback(string message)
    {
        if (feedbackText == null) return;

        if (hideFeedbackCoroutine != null)
            StopCoroutine(hideFeedbackCoroutine);

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        isFeedbackActive = true; // <-- ATIVA

        hideFeedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay(feedbackDuration));
    }

    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        isFeedbackActive = false; // <-- DESATIVA quando some
    }


    // ===== ANIMAÇÕES =====
    private void CheckAnimations()
    {
        // Movimento (andar/correr)
        bool isMoving = _inpMove.magnitude > 0.1f;
        _animator.SetBool("isWalking", isMoving && !_runPressed);
        _animator.SetBool("isRunning", isMoving && _runPressed);
    }

    // ===== INTERAÇÃO (ATUALIZADO) =====
    public void InteractWithObject()
    {
        Ray ray = new Ray(_pCam.transform.position, _pCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _alcanceDeInteração, _mascaraDeInteração, QueryTriggerInteraction.Collide))
        {
            
            // ITEM
            if (hit.collider.CompareTag("Item"))
            {
                // Verifica se tem CollectibleItem para lógica especial
                CollectibleItem collectible = hit.collider.GetComponent<CollectibleItem>();
                if (collectible != null)
                {
                    collectible.ColetarItem(this); // Chama a lógica especial
                }
                else
                {
                    // Item comum sem CollectibleItem
                    _i += 1;
                    ShowFeedback(hit.collider.name + " Coletada");
                    Debug.Log("Item coletado: " + hit.collider.name);
                    Destroy(hit.collider.gameObject);
                }
                return;
            }


            // CARTA (Agora usando Raycast - Se você usa CollectibleItem como Trigger, veja o segundo script)
            if (hit.collider.CompareTag("Carta"))
            {
                Debug.Log("Interagiu com a Carta: " + hit.collider.name);

                CollectibleItem carta = hit.collider.GetComponent<CollectibleItem>();
                if (carta != null)
                {
                    // Chamamos a função do CollectibleItem para centralizar a lógica de coleta
                    // e evitar duplicação com o trigger. Usamos o feedback aqui, mas o item ainda não se destrói
                    // aqui se for um trigger.
                    if (carta.cartaUI != null)
                    {
                        ShowFeedback("Pressione F para fechar a carta.");
                        AbrirCarta(carta.cartaUI);
                    }
                }
                return;
            }

            // PORTA
            if (hit.collider.CompareTag("Porta"))
            {
                porta porta = hit.collider.GetComponent<porta>();
                if (porta != null)
                {
                    if (_i >= porta._itensParaAbrir)
                    {
                        porta.AcionarPorta();
                        
                        Debug.Log("Porta aberta: " + hit.collider.name);

                        if (_animator != null)
                            _animator.SetTrigger("openDoor");
                    }
                    else
                    {
                        // Feedback de porta trancada/faltando itens
                        ShowFeedback("Trancado...");
                        Debug.Log($"Precisa de {porta._itensParaAbrir} itens, você tem {_i}.");
                    }
                }
                return;
            }

            // QUADRO
            if (hit.collider.CompareTag("Quadro"))
            {
                PuzzleTrigger pzt = hit.collider.GetComponent<PuzzleTrigger>();
                if (pzt != null)
                {
                    pzt._bool = true;
                    ShowFeedback("Você ativou o Quadro!"); // Feedback de quadro
                }
                return;
            }
        }
    }

    // ===== ABRIR CARTA =====
    public void AbrirCarta(GameObject cartaUI)
    {
        cartaAtualUI = cartaUI;
        cartaAtualUI.SetActive(true);

        if (imagemExtraUI != null)
            imagemExtraUI.SetActive(true); // ativa a imagem no canto

        cartaAberta = true;
        Time.timeScale = 0f; // congela o jogo

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Carta aberta!");
    }

    // ===== FECHAR CARTA =====
    private void FecharCarta()
    {
        if (cartaAtualUI != null)
            cartaAtualUI.SetActive(false);

        if (imagemExtraUI != null)
            imagemExtraUI.SetActive(false); // desativa junto

        cartaAberta = false;
        Time.timeScale = 1f; // volta o jogo

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Carta fechada!");
    }

    // ===== MOVIMENTO =====
    public void Run()
    {
        _speed = _runPressed ? _velocidade * _multiplicadorDeVelocidade : _velocidade;
    }

    // ===== LANTERNA =====
    public void LanternPres()
    {
        _lntrOn = !_lntrOn;

        if (_lntrOn)
        {
            if (_somLigarLanterna != null)
                AudioSource.PlayClipAtPoint(_somLigarLanterna, transform.position, _lanternVolume);
        }
        else
        {
            if (_somDesligarLanterna != null)
                AudioSource.PlayClipAtPoint(_somDesligarLanterna, transform.position, _lanternVolume);
        }
    }

    // ===== GAME MODE 1 (Noclip/Fly) =====
    public void gMode()
    {
        if (_gmod1)
        {
            _cntr.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            _vel = Vector3.zero;

            Vector3 dir = Vector3.zero;
            dir += transform.forward * _inpMove.y;
            dir += transform.right * _inpMove.x;

            if (Keyboard.current.spaceKey.isPressed) dir += Vector3.up;
            if (Keyboard.current.leftShiftKey.isPressed) dir += Vector3.down;

            float flySpeed = _velocidade * 2f;
            transform.position += dir.normalized * flySpeed * Time.deltaTime;
        }
        else
        {
            if (!_cntr.enabled) _cntr.enabled = true;
            GetComponent<CapsuleCollider>().enabled = true;
        }
    }

    // ===== GIZMOS =====
    private void OnDrawGizmos()
    {
        if (_giz)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_pCam.transform.position, _pCam.transform.forward * _alcanceDeInteração);
        }
    }
}