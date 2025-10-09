using UnityEngine;

public class QuadroPieceCollectable : MonoBehaviour
{
    [HideInInspector] public QuadroPieceManager manager;
    [HideInInspector] public QuadroPieceManager_Fixed managerFixed;

    [Header("Configura��es de Intera��o")]
    [SerializeField] private float distanciaMaxima = 8f;
    [SerializeField] private LayerMask layerInteracao = -1;

    private Camera playerCamera;
    private bool podeInteragir = false;

    void Start()
    {
        // Encontra a c�mera do player
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

        // Configura o collider corretamente
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false; // N�o usa trigger, funciona com raycast
        }

        // Garante que a pe�a tenha a tag e layer corretas
        if (!gameObject.CompareTag("Item"))
        {
            gameObject.tag = "Item";
        }
        
        int interactionLayer = LayerMask.NameToLayer("Intera��o");
        if (gameObject.layer != interactionLayer)
        {
            gameObject.layer = interactionLayer;
        }

        // Se não tem manager, tenta encontrar automaticamente
        if (manager == null && managerFixed == null)
        {
            manager = FindObjectOfType<QuadroPieceManager>();
            managerFixed = FindObjectOfType<QuadroPieceManager_Fixed>();
        }
    }

    void Update()
    {
        if (playerCamera == null || (manager == null && managerFixed == null)) return;

        // Verifica se o player est� olhando para esta pe�a
        VerificarInteracao();

        // Verifica clique do mouse
        if (podeInteragir && Input.GetMouseButtonDown(0)) // Bot�o esquerdo do mouse
        {
            ColetarPeca();
        }
    }

    void VerificarInteracao()
    {
        // Raycast da c�mera
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaMaxima, layerInteracao))
        {
            // Verifica se est� olhando para esta pe�a
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
                playerScript.interactionHintText.text = "Clique para coletar pe�a";
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
        // Chama o manager que estiver disponível
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

            // Chama o manager original para processar a coleta
            manager.ColetarPeca(gameObject);
        }
        else if (managerFixed != null)
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

            // Chama o manager corrigido para processar a coleta
            managerFixed.ColetarPeca(gameObject);
        }
    }

    void OnDestroy()
    {
        // Esconde a dica quando a pe�a � destru�da
        EsconderDicaInteracao();
    }
}
