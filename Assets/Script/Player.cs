using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    // CARTA
    [Header("Carta")]
    private GameObject cartaAtualUI;  // carta ativa no momento
    public bool cartaAberta = false;

    [Tooltip("Imagem que aparece no canto da tela junto com qualquer carta")]
    public GameObject imagemExtraUI; // <- arraste no Inspector (Canvas)


    // INTERAÇÃO
    [Header("Interação")]
    public float _alcanceDeInteração = 8f;
    public LayerMask _mascaraDeInteração;
    public GameObject _inimigo;
    private bool _intPressed;
    private bool _runPressed;
    private bool _lntPressed;
    private bool _giz;
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

        if (_lanternAudioSource == null)
            _lanternAudioSource = GetComponent<AudioSource>();

        if (_lanternAudioSource != null)
        {
            _lanternAudioSource.playOnAwake = false;
            _lanternAudioSource.loop = false;
            _lanternAudioSource.spatialBlend = 0f;
        }
    }

    public void OnEnable() => _inpActions.Enable();
    public void OnDisable() => _inpActions.Disable();

    // ===== START =====
    public void Start()
    {
        if (_pCam == null)
            _pCam = Camera.main;

        if (cartaAtualUI != null)
            cartaAtualUI.SetActive(false);

        if (imagemExtraUI != null)
            imagemExtraUI.SetActive(false); // começa desativada

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ===== UPDATE =====
    public void Update()
    {
        
        {
            if (cartaAberta)
            {
                // fecha carta quando apertar F
                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    FecharCarta();
                }

                // impede que o player se mova enquanto a carta está aberta
                return;
            }

            Run();

            if (_inimigo != null)
            {
                if (_inimigo.gameObject.GetComponent<Enemy>()._plyAtq)
                    _inpActions.Disable();
            }

            _isOnG = Physics.CheckSphere(_gCheck.position, _distanciaDoChão, _chão);

            if (_isOnG && _vel.y < 0f)
                _vel.y = -2f;

            _m = transform.right * _inpMove.x + transform.forward * _inpMove.y;
            _cntrMult = _isOnG ? 1f : _velocidadeNoAr;
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
            _cntr.Move(_vel * Time.deltaTime);
        }
    }

        // ===== INTERAÇÃO =====
        public void InteractWithObject()
    {
        Ray ray = new Ray(_pCam.transform.position, _pCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _alcanceDeInteração, _mascaraDeInteração, QueryTriggerInteraction.Collide))
        {
            // ITEM
            if (hit.collider.CompareTag("Item"))
            {
                _i += 1;
                Debug.Log("Item coletado: " + hit.collider.name);
                Destroy(hit.collider.gameObject);
                return;
            }

            // CARTA
            if (hit.collider.CompareTag("Carta"))
            {
                Debug.Log("Interagiu com a Carta: " + hit.collider.name);

                CollectibleItem carta = hit.collider.GetComponent<CollectibleItem>();
                if (carta != null && carta.cartaUI != null)
                {
                    AbrirCarta(carta.cartaUI);
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
                    }
                    else
                    {
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
