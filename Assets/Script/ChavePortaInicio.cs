using UnityEngine;
using UnityEngine.InputSystem;

public class ChavePortaInicio : MonoBehaviour
{
    public static bool hasKey = false;
    private bool isPlayerNear = false;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();

        playerControls.Player.Interact.performed += OnInteract;
    }

    private void OnDestroy()
    {
        playerControls.Player.Interact.performed -= OnInteract;
        playerControls.Player.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerNear && !hasKey)
        {
            hasKey = true;
            Debug.Log("Chave coletada");
            Destroy(gameObject);
        }
    }
}
