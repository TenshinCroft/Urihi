using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootstepsAmbiente : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource footstepsSource;
    [SerializeField] private float walkPitch = 1f;
    [SerializeField] private float runPitch = 1.5f;

    [Header("Step Sounds")]
    [SerializeField] private AudioClip defaultStepSound;
    [SerializeField] private List<SurfaceFootstep> surfaceFootsteps;

    [Header("Detection")]
    [SerializeField] private Transform groundCheckOrigin;
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private LayerMask groundLayer;

    private Dictionary<string, AudioClip> surfaceAudioMap;
    private PlayerControls inputActions;
    private Vector2 moveInput;
    private bool isRunning;

    private string currentSurfaceTag = "";
    private AudioClip currentStepClip;

    private void Awake()
    {
        inputActions = new PlayerControls();
        surfaceAudioMap = new Dictionary<string, AudioClip>();

        foreach (var surface in surfaceFootsteps)
        {
            Debug.Log($"Mapeando tag '{surface.surfaceTag}' para clip '{surface.footstepClip?.name}'");
            if (!surfaceAudioMap.ContainsKey(surface.surfaceTag) && surface.footstepClip != null)
            {
                surfaceAudioMap.Add(surface.surfaceTag, surface.footstepClip);
            }
        }

        currentStepClip = defaultStepSound;
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
        footstepsSource.pitch = runPitch;
        Debug.Log("Correndo - pitch aumentado");
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false;
        footstepsSource.pitch = walkPitch;
        Debug.Log("Parou de correr - pitch normal");
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        DetectSurface();

        footstepsSource.pitch = isRunning ? runPitch : walkPitch;

        if (isMoving)
        {
            if (!footstepsSource.isPlaying)
            {
                Debug.Log($"Tocando som de passos: {currentStepClip?.name ?? "null"}");
                PlayStepSound();
            }
        }
        else
        {
            if (footstepsSource.isPlaying)
            {
                footstepsSource.Stop();
                Debug.Log("Parou de andar - parando som");
            }
        }
    }

    private void DetectSurface()
    {
        if (Physics.Raycast(groundCheckOrigin.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            Debug.Log($"Raycast acertou: {hit.collider.name} com tag: {hit.collider.tag}");

            string surfaceTag = hit.collider.tag;

            if (surfaceTag != currentSurfaceTag)
            {
                Debug.Log($"Mudando superfície de {currentSurfaceTag} para {surfaceTag}");
                currentSurfaceTag = surfaceTag;

                if (surfaceAudioMap.TryGetValue(surfaceTag, out AudioClip clip))
                {
                    currentStepClip = clip;
                    Debug.Log($"Clip atualizado para {clip.name}");
                }
                else
                {
                    currentStepClip = defaultStepSound;
                    Debug.Log("Usando clip default");
                }

                if (footstepsSource.isPlaying)
                {
                    PlayStepSound();
                }
            }
        }
        else
        {
            if (currentSurfaceTag != "")
            {
                Debug.Log("Raycast não detectou chão, resetando clip para default");
                currentSurfaceTag = "";
                currentStepClip = defaultStepSound;

                if (footstepsSource.isPlaying)
                {
                    PlayStepSound();
                }
            }
        }
    }

    private void PlayStepSound()
    {
        footstepsSource.Stop();
        footstepsSource.clip = currentStepClip;
        footstepsSource.Play();
    }
}

[System.Serializable]
public class SurfaceFootstep
{
    public string surfaceTag;
    public AudioClip footstepClip;
}
