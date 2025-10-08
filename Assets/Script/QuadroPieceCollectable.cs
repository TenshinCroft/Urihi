using UnityEngine;

public class QuadroPieceCollectable : MonoBehaviour
{
    [HideInInspector] public QuadroPieceManager manager;

    [Header("Configurações de Interação")]
    [SerializeField] private float distanciaMaxima = 8f;
    [SerializeField] private LayerMask layerInteracao = -1;

    private Camera playerCamera;
    private bool podeInteragir = false;

    void Start()
    {
        // Encontra a câmera do player
        if (playerCamera == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerCamera = player.GetComponentInChildren<Camera>();
            }

            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
        }

        // Remove o trigger do collider se existir
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false; // Não usa mais trigger
        }

        // Configura a tag da peça
        if (!gameObject.CompareTag("Item"))
        {
            gameObject.tag = "Item"; // Usa a mesma tag dos outros itens
        }
    }

    void Update()
    {
        if (playerCamera == null || manager == null) return;

        // Verifica se o player está olhando para esta peça
        VerificarInteracao();

        // Verifica clique do mouse
        if (podeInteragir && Input.GetMouseButtonDown(0)) // Botão esquerdo do mouse
        {
            ColetarPeca();
        }
    }

    void VerificarInteracao()
    {
        // Raycast da câmera
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaMaxima, layerInteracao))
        {
            // Verifica se está olhando para esta peça
            if (hit.collider.gameObject == gameObject)
            {
                podeInteragir = true;
                MostrarDicaInteracao();
            }
            else
            {
                podeInteragir = false;
                EsconderDicaInteracao();
            }
        }
        else
        {
            podeInteragir = false;
            EsconderDicaInteracao();
        }
    }

    void MostrarDicaInteracao()
    {
        // Tenta encontrar o sistema de dicas do player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null && playerScript.interactionHintText != null)
            {
                playerScript.interactionHintText.text = "Clique para coletar peça";
                playerScript.interactionHintText.gameObject.SetActive(true);
            }
        }
    }

    void EsconderDicaInteracao()
    {
        // Tenta encontrar o sistema de dicas do player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null && playerScript.interactionHintText != null)
            {
                playerScript.interactionHintText.gameObject.SetActive(false);
            }
        }
    }

    void ColetarPeca()
    {
        if (manager != null)
        {
            // Feedback para o player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.ShowFeedback($"Peça do quadro coletada!");
                }
            }

            // Chama o manager para processar a coleta
            manager.ColetarPeca(gameObject);
        }
    }

    void OnDestroy()
    {
        // Esconde a dica quando a peça é destruída
        EsconderDicaInteracao();
    }
}
