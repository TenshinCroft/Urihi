using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private PlayerControls inputActions;
    private Vector2 mouseInput;
    private float xRotation = 0f;

    private void Awake()
    {
        inputActions = new PlayerControls();

        inputActions.Player.Look.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => mouseInput = Vector2.zero;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        // Câmera (pra cima/baixo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Player (pra esquerda/direita)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
