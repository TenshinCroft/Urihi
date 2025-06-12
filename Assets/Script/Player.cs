using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+=+= INTERAÇÃO =+=+=+=+=+=+=+=
    [Header("Interação")]
    //||||||||||||||| PUBLICAS ||||||||||||||||
    //---------------- floats -----------------
    public float _alcanceDeInteração = 8f;
    //--------------- layermasks --------------
    public LayerMask _mascaraDeInteração;
    //-------------- game objects -------------
    public GameObject _inimigo;
    //||||||||||||||| PRIVADAS ||||||||||||||||
    //----------------- bools -----------------
    private bool _intPressed;
    private bool _cScPressed;
    private bool _giz;
    //--------------- components --------------
    private Camera _pCam;
    ///////////////////////////////////////////


    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+=+= MOVIMENTO =+=+=+=+=+=+=+=
    [Header("Movimento")]
    //||||||||||||||| PUBLICAS ||||||||||||||||
    //---------------- floats -----------------
    public float _velocidade = 12f;
    public float _alturaDoPulo = 2f;
    public float _velocidadeNoAr = 0.5f;
    //||||||||||||||| PRIVADAS ||||||||||||||||
    //---------------- floats -----------------
    private float _g = 9.81f;
    //---------------- vectors ----------------
    private Vector2 _inpMove;
    private Vector3 _vel;
    private Vector3 _m;
    //----------------- bools -----------------
    private bool _isOnG;
    private bool _jPressed;
    //---------------- floats -----------------
    private float _cntrMult;
    ///////////////////////////////////////////


    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+= GROUND CHECK +=+=+=+=+=+=+=
    [Header("Verificação de Chão")]
    //||||||||||||||| PUBLICAS ||||||||||||||||
    //---------------- floats -----------------
    public float _distanciaDoChão = 0.4f;
    //--------------- layermasks --------------
    public LayerMask _chão;
    //||||||||||||||| PRIVADAS ||||||||||||||||
    //--------------- components --------------
    private Transform _gCheck;
    ///////////////////////////////////////////


    //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //=+=+=+=+=+=+=+ CONTROLER +=+=+=+=+=+=+=+=
    [Header("Sistema de Input")]
    //||||||||||||||| PUBLICAS ||||||||||||||||
    //----------------- ints ------------------
    //++++++++++++++ invisiveis +++++++++++++++
    [HideInInspector]
    public int _i = 0;
    //||||||||||||||| PRIVADAS ||||||||||||||||
    //--------------- components --------------
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
        _inpActions.Player.Hide.performed += ctx => _cScPressed = true;

    }

    // verifica se o jogador foi ativado
    public void OnEnable()
    {
        // ativa os input actions
        _inpActions.Enable();
    }

    // verifica se o jogador foi desativado
    public void OnDisable()
    {
        // desativa os input actions
        _inpActions.Disable();
    }


    //=+=+=+=+= QUANDO O JOGO COMEÇA +=+=+=+=+=
    public void Start()
    {
        // verifica se o player tem uma camera
        if (_pCam == null)
        {
            // atribui uma camera ao jogador
            _pCam = Camera.main;
        }

        // prende o cursor na tela e dexa ele invisivel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    //=+=+=+=+ QUANDO O JOGO COMEÇA =+=+=+=+=
    public void Update()
    {
        // verifica se tem um inimigo atribuido
        if(_inimigo != null)
        {
            // verifica se o inimigo atacou
            if (_inimigo.gameObject.GetComponent<Enemy>()._plyAtq)
            {
                // desativa os input actions
                _inpActions.Disable();
            }
        }

        // checa se está no chão
        _isOnG = Physics.CheckSphere(_gCheck.position, _distanciaDoChão, _chão);

        // checa se está no chão e caindo
        if (_isOnG && _vel.y < 0f)
        {
            // atribui uma velocidade negativa a queda para impedir bugs
            _vel.y = -2f;
        }

        // movimentação do jogador
        _m = transform.right * _inpMove.x + transform.forward * _inpMove.y;
        _cntrMult = _isOnG ? 1f : _velocidadeNoAr;
        _cntr.Move(_m * _velocidade * _cntrMult * Time.deltaTime);

        // verifica se foi precionado o pulo e esta no chão
        if (_jPressed && _isOnG)
        {
            // desativa o botão do pulo e atribui uma velocidade pra cima
            _vel.y = Mathf.Sqrt(_alturaDoPulo * 2f * _g);
            _jPressed = false;
        }

        // verifica se o botão de interação foi precionado
        if (_intPressed)
        {
            // chama a função de interação e desativa o botão
            InteractWithObject();
            _intPressed = false;
        }

        // verifica se o botão de mudar de cena foi precionado
        if (_cScPressed)
        {
            //chama a função de mudar de cena e desativa o botão
            ChangeSC();
            _cScPressed = false;
        }

        // aplica a gravidade do jogador
        _vel.y += -_g * Time.deltaTime;
        _cntr.Move(_vel * Time.deltaTime);
    }
    public void InteractWithObject()
    {
        Ray ray = new Ray(_pCam.transform.position, _pCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _alcanceDeInteração, _mascaraDeInteração))
        {
            if (hit.collider.CompareTag("Item"))
            {
                _i += 1;
                Debug.Log("Item coletado: " + hit.collider.name);
                Destroy(hit.collider.gameObject); // Coleta/destrói o item
                
            }
            else if (hit.collider.CompareTag("Porta"))
            {
                porta porta = hit.collider.GetComponent<porta>();
                if (porta != null)
                {
                    if (_i >= hit.collider.GetComponent<porta>()._port)
                    {
                        porta.AcionarPorta();
                        Debug.Log("Porta aberta: " + hit.collider.name);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Nada interagível na frente");
        }
    }

    public void ChangeSC()
    {
        if (MySceneManager._inst != null)
        {
            if (SceneManager.GetActiveScene().name == "Testes")
            {
                MySceneManager._inst.LoadScene("Ambiente");
            }
            else
            {
                //MySceneManager._inst.LoadScene("Testes");
            }
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
