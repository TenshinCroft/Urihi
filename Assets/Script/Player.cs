using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    //=============== Interação ===============
    [Header("Interação")]
    public float interactRange = 8f;
    public LayerMask interactableMask;
    private bool interactPressed;
    private bool hidePressed;
    public bool _giz;
    public float hideRange = 4f;
    public LayerMask hideMask;
    public GameObject inimigo;
    public Camera playerCam;

    //================ MOVIMENTO ================
    [Header("Movimento")]
    public float moveSpeed = 12f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float airControlMultiplier = 0.5f;

    public Vector2 inputMove;
    public Vector3 velocity;

    public bool isGrounded;
    public bool jumpPressed;


    //================ GROUND CHECK ================
    [Header("Verificação de Chão")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    //================ SISTEMA DE INPUT ================
    [Header("Sistema de Input")]
    public PlayerControls inputActions;

    //================ CONTROLLER ================
    [Header("Character Controller")]
    public CharacterController controller;

    public int _itens = 0;

    //================ CICLO DE VIDA ================
    public void Awake()
    {
        // Pega o CharacterController e inicializa os inputs
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerControls();

        inputActions.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => inputMove = Vector2.zero;
        inputActions.Player.Jump.performed += ctx => jumpPressed = true;
        inputActions.Player.Interact.performed += ctx => interactPressed = true;
        inputActions.Player.Hide.performed += ctx => hidePressed = true;

    }

    public void OnEnable()
    {
        inputActions.Enable();
    }

    public void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        if (playerCam == null)
            playerCam = Camera.main;

        if (groundCheck == null)
            Debug.LogWarning("groundCheck não foi atribuído no Inspetor!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //================ UPDATE ======================
    private void Update()
    {
        if(inimigo.gameObject.GetComponent<Enemy>()._plyAtq)
        {
            inputActions.Disable();
        }

        // Checa se está no chão
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;


        // Movimento
        Vector3 move = transform.right * inputMove.x + transform.forward * inputMove.y;
        float controlMultiplier = isGrounded ? 1f : airControlMultiplier;
        controller.Move(move * moveSpeed * controlMultiplier * Time.deltaTime);

        // Pulo
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        // Interagir (coletar item)
        if (interactPressed)
        {
            InteractWithObject();
            interactPressed = false;
        }

        // Interagir (entrar no armario)
        if (hidePressed)
        {
            Hide();
            hidePressed = false;
        }

        // Gravidade
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public void InteractWithObject()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableMask))
        {
            if (hit.collider.CompareTag("Item"))
            {
                _itens += 1;
                Debug.Log("Item coletado: " + hit.collider.name);
                Destroy(hit.collider.gameObject); // Coleta/destrói o item
            }
            else if (hit.collider.CompareTag("Porta"))
            {
                porta porta = hit.collider.GetComponent<porta>();
                if (porta != null)
                {
                    porta.AcionarPorta();
                    Debug.Log("Porta aberta: " + hit.collider.name);
                }
            }
        }
        else
        {
            Debug.Log("Nada interagível na frente");
        }
    }

    public void Hide()
    {
        if (MySceneManager._inst != null)
        {
            if (SceneManager.GetActiveScene().name == "Testes")
            {
                MySceneManager._inst.LoadScene("Ambiente");
            }
            else
            {
                MySceneManager._inst.LoadScene("Testes");
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (_giz)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCam.transform.position, playerCam.transform.forward * interactRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, hideRange);
        }
    }

}
