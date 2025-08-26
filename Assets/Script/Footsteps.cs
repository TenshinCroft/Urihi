using UnityEngine;
using UnityEngine.InputSystem;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioSource footstepsSound;

    [Header("Footstep Settings")]
    [SerializeField] private float walkPitch = 1f;
    [SerializeField] private float runPitch = 1.5f;

    private PlayerControls inputActions;
    private Vector2 moveInput;
    private bool isRunning;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Correr.started += OnRunStarted;
        inputActions.Player.Correr.canceled += OnRunCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Correr.started -= OnRunStarted;
        inputActions.Player.Correr.canceled -= OnRunCanceled;
        inputActions.Player.Disable();
    }

    private void OnRunStarted(InputAction.CallbackContext context)
    {
        isRunning = true;
        if (footstepsSound.isPlaying)
        {
            footstepsSound.pitch = runPitch;
        }
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false;
        if (footstepsSound.isPlaying)
        {
            footstepsSound.pitch = walkPitch;
        }
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            footstepsSound.pitch = isRunning ? runPitch : walkPitch;

            if (!footstepsSound.isPlaying)
            {
                footstepsSound.Play();
            }
        }
        else
        {
            if (footstepsSound.isPlaying)
            {
                footstepsSound.Stop();
            }
        }
    }
}
