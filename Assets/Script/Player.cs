using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    

    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+=+= INTERAÇÃO =+=+=+=+=+=+=+=
    [Header("Interação")]
    //---------------- floats -----------------
    public float _alcanceDeInteração = 8f;
    //--------------- layermasks --------------
    public LayerMask _mascaraDeInteração;
    //-------------- game objects -------------
    public GameObject _inimigo;
    //----------------- bools -----------------
    private bool _intPressed;
    private bool _runPressed;
    private bool _lntPressed;
    private bool _giz;
    public bool _carta = true;
    //--------------- components --------------
    public Camera _pCam;
    ///////////////////////////////////////////


    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+=+= MOVIMENTO =+=+=+=+=+=+=+=
    [Header("Movimento")]
    //---------------- floats -----------------
    public float _velocidade = 12;
    public float _multiplicadorDeVelocidade = 1.5f;
    public float _alturaDoPulo = 2f;
    public float _velocidadeNoAr = 0.5f;
    //---------------- floats -----------------
    private float _g = 9.81f;
    private float _speed;
    //---------------- vectors ----------------
    private Vector2 _inpMove;
    private Vector3 _vel;
    private Vector3 _m;
    //----------------- bools -----------------
    public bool _isOnG;
    public bool _jPressed;
    public bool _lntrOn; // variável de estado da lanterna
    //---------------- floats -----------------
    private float _cntrMult;
    ///////////////////////////////////////////


    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+= GROUND CHECK +=+=+=+=+=+=+=
    [Header("Verificação de Chão")]
    //---------------- floats -----------------
    public float _distanciaDoChão = 0.4f;
    //--------------- layermasks --------------
    public LayerMask _chão;
    //--------------- components --------------
    public Transform _gCheck;
    ///////////////////////////////////////////


    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+= LANERNA + SONS =+=+=+=+=+=+
    [Header("Lanterna")]
    public AudioSource _lanternAudioSource;
    public AudioClip _somLigarLanterna;
    public AudioClip _somDesligarLanterna;
    [Range(0f, 1f)] public float _lanternVolume = 1f;
    ///////////////////////////////////////////


    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+=+ CONTROLER +=+=+=+=+=+=+=+=
    [Header("Sistema de Input")]
    [HideInInspector]
    public int _i = 0;
    private PlayerControls _inpActions;
    private CharacterController _cntr;
    ///////////////////////////////////////////


    //=+=+=+=+= ANTES DO JOGO COMEÇAR =+=+=+=+=
    public void Awake()
    {
        // Pega o character controller e inicializa os inputs
        _cntr = GetComponent<CharacterController>();
        _inpActions = new PlayerControls();

        // verifica se os inputs foram ativados
        _inpActions.Player.Move.performed += ctx => _inpMove = ctx.ReadValue<Vector2>();
        _inpActions.Player.Move.canceled += ctx => _inpMove = Vector2.zero;
        _inpActions.Player.Jump.performed += ctx => _jPressed = true;
        _inpActions.Player.Interact.performed += ctx => _intPressed = true;
        _inpActions.Player.Correr.performed += ctx => _runPressed = true;
        _inpActions.Player.Correr.canceled += ctx => _runPressed = false;
        _inpActions.Player.Lanterna.performed += ctx => _lntPressed = true;

        // ==== ÁUDIO DA LANTERNA ==== //
        if (_lanternAudioSource == null)
            _lanternAudioSource = GetComponent<AudioSource>();

        if (_lanternAudioSource != null)
        {
            _lanternAudioSource.playOnAwake = false;
            _lanternAudioSource.loop = false;
            _lanternAudioSource.spatialBlend = 0f; // 2D para sempre ouvir
        }
        else
        {
            Debug.LogWarning("[Player] AudioSource ausente no Player.");
        }
    }

    public void OnEnable() => _inpActions.Enable();
    public void OnDisable() => _inpActions.Disable();


    //=+=+=+=+= QUANDO O JOGO COMEÇA +=+=+=+=+=
    public void Start()
    {
        if (_pCam == null)
            _pCam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    //=+=+=+=+ LOOP DO JOGO =+=+=+=+=
    public void Update()
    {
        Run();

        if (_inimigo != null)
        {
            if (_inimigo.gameObject.GetComponent<Enemy>()._plyAtq)
            {
                _inpActions.Disable();
            }
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


    public void InteractWithObject()
    {
        Ray ray = new Ray(_pCam.transform.position, _pCam.transform.forward);
        RaycastHit hit;

        // Importante: seta para também acertar colliders com "Is Trigger"
        if (Physics.Raycast(ray, out hit, _alcanceDeInteração, _mascaraDeInteração, QueryTriggerInteraction.Collide))
        {
           
            // ====== ITEM ======
            if (hit.collider.CompareTag("Item"))
            {
                _i += 1;
                Debug.Log("Item coletado: " + hit.collider.name);
                Destroy(hit.collider.gameObject);
                return;
            }

            // ====== CARTA ======
            if (hit.collider.CompareTag("Carta"))
            {
                Debug.Log("Carta Aberta: " + hit.collider.name);
                _carta = !_carta;
                hit.collider.GetComponent<CollectibleItem>()._carta = !_carta;
                return;
            }

            // ====== PORTA ======
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

            Debug.Log($"[Interação] Ray acertou '{hit.collider.name}', mas não é Item/Porta e não tem Carta.");
        }
        else
        {
            Debug.Log("[Interação] Nada Interagivel");
        }
    }

    public void Run()
    {
        if (_runPressed)
            _speed = _velocidade * _multiplicadorDeVelocidade;
        else
            _speed = _velocidade;
    }

    // ==== LANERNA + ÁUDIO ====
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


    private void OnDrawGizmos()
    {
        if (_giz)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_pCam.transform.position, _pCam.transform.forward * _alcanceDeInteração);
        }
    }


}
