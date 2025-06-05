using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    //=============== Intera��o ===============
    [Header("Intera��o")]
    public float interactRange = 2f;
    public LayerMask interactableMask;
    private bool interactPressed;
    private bool hidePressed;
    public float hideRange = 4f;
    public LayerMask hideMask;
    public GameObject inimigo;

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
    [Header("Verifica��o de Ch�o")]
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
        if (groundCheck == null)
            Debug.LogWarning("groundCheck n�o foi atribu�do no Inspetor!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //================ UPDATE ======================
    private void Update()
    {
        if(inimigo.gameObject.GetComponent<Enemy>()._pAtq)
        {
            inputActions.Disable();
        }

        // Checa se est� no ch�o
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
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableMask);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Item"))
            {
                _itens += 1;
                Debug.Log("Item coletado: " + hit.name);
                Destroy(hit.gameObject); // Coleta/destr�i o item
                break;
            }

            if (hit.CompareTag("Porta"))
            {
                porta porta = hit.GetComponent<porta>();
                if (porta != null)
                {
                    porta.AcionarPorta();
                    Debug.Log("Porta aberta: " + hit.name);
                }
                break;
            }

        }
    }

    private void Hide()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hideRange, hideMask);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Armario"))
            {
                Debug.Log("Entrou no Armario: " + hit.name);
                //Destroy(hit.gameObject); // Coleta/destr�i o item
                break;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hideRange);
    }

}
