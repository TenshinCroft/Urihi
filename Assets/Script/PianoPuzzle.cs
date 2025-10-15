using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class PianoPuzzle : MonoBehaviour
{
    [Header("Ordem correta dos cubos (arraste aqui no Inspector)")]
    public GameObject[] ordemCorreta;

    [Header("Indicadores de Cor na Sequência")]
    public GameObject[] indicadores;

    [Header("Item que será liberado ao completar o puzzle")]
    public GameObject itemLiberado;

    [Header("Sons de feedback")]
    public AudioClip somErro;
    public AudioClip somAcerto;

    [Header("Cor de Reset dos Indicadores")]
    // Use esta variável no Inspector para definir a cor de "vazio" (provavelmente preto)
    public Color corPreta = Color.black;

    private int indiceAtual = 0;
    private Rigidbody rbItem;
    private AudioSource audioSource;
    private bool puzzleCompleto = false;
    private bool feedbackTocando = false;
    private LayerMask pianoLayerMask;

    // Array para armazenar as N teclas clicadas pelo usuário
    private GameObject[] sequenciaUsuario;

    // Bloco de propriedades para mudar a cor de forma eficiente (sem criar novas instâncias de material)
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        // Inicializa o MaterialPropertyBlock
        propBlock = new MaterialPropertyBlock();

        // Inicializa o array de entrada do usuário com o mesmo tamanho da ordem correta
        sequenciaUsuario = new GameObject[ordemCorreta.Length];

        // Configuração da Layer: Usando o nome exato 'Interação'
        pianoLayerMask = LayerMask.GetMask("Interação");

        if (pianoLayerMask.value == 0)
        {
            Debug.LogError("ERRO: A Layer 'Interação' não foi encontrada. O Raycast do piano não funcionará.");
        }

        if (itemLiberado != null)
        {
            itemLiberado.SetActive(false);
            rbItem = itemLiberado.GetComponent<Rigidbody>();
            if (rbItem != null)
            {
                rbItem.isKinematic = true;
                rbItem.useGravity = false;
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ResetarIndicadores();
    }

    void Update()
    {
        // Bloqueia a entrada enquanto o puzzle está completo ou tocando feedback/resetando
        if (puzzleCompleto || feedbackTocando)
            return;

        Vector2 posicaoClique = Vector2.zero;
        bool clicou = false;

        // Detecção de Clique
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            posicaoClique = Mouse.current.position.ReadValue();
            clicou = true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            posicaoClique = Touchscreen.current.primaryTouch.position.ReadValue();
            clicou = true;
        }

        if (clicou)
        {
            Ray ray = Camera.main.ScreenPointToRay(posicaoClique);

            // Raycast usando a máscara de camada 'Interação'
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, pianoLayerMask))
            {
                GameObject objetoClicado = hit.collider.gameObject;

                int indiceDoObjeto = Array.IndexOf(ordemCorreta, objetoClicado);

                if (indiceDoObjeto != -1)
                {
                    // Toca o som da tecla clicada (se não estiver em loop de feedback)
                    TocarSomDoObjeto(objetoClicado);

                    // 1. Armazena a tecla clicada no slot atual
                    sequenciaUsuario[indiceAtual] = objetoClicado;

                    // 2. Acende o indicador na posição atual com a COR da tecla
                    AtualizarIndicador(indiceAtual, objetoClicado);

                    // 3. Avança para o próximo slot
                    indiceAtual++;

                    // 4. VERIFICAÇÃO: A sequência completa foi preenchida?
                    if (indiceAtual >= ordemCorreta.Length)
                    {
                        VerificarSequenciaCompleta();
                    }
                }
            }
        }
    }

    // Contém toda a lógica de checagem e punição/recompensa.
    void VerificarSequenciaCompleta()
    {
        bool sequenciaCorreta = true;

        // Itera sobre toda a sequência para checar
        for (int i = 0; i < ordemCorreta.Length; i++)
        {
            if (sequenciaUsuario[i] != ordemCorreta[i])
            {
                sequenciaCorreta = false;
                break; // Errou
            }
        }

        if (sequenciaCorreta)
        {
            // --- ACERTO COMPLETO ---
            if (somAcerto != null) { StartCoroutine(TocarSomFeedback(somAcerto, false)); }

            if (itemLiberado != null)
            {
                itemLiberado.SetActive(true);
                if (rbItem != null) { rbItem.isKinematic = false; rbItem.useGravity = true; }
            }
            puzzleCompleto = true;
            Debug.Log("Puzzle completo!");
        }
        else
        {
            // --- ERRO COMPLETO ---
            if (somErro != null) { StartCoroutine(TocarSomFeedback(somErro, true)); }
            Debug.Log("Sequência errada. Resetando.");
            // O reset (indices e cores) é feito dentro da coroutine TocarSomFeedback
        }
    }

    // Coroutine TocarSomFeedback: Modificada para lidar com o reset atrasado do modo Fill-and-Check
    IEnumerator TocarSomFeedback(AudioClip clip, bool isError)
    {
        feedbackTocando = true;

        audioSource.Stop();
        audioSource.PlayOneShot(clip);

        float delay = clip.length;

        // Se for erro, reseta o puzzle e os indicadores após o som
        if (isError)
        {
            yield return new WaitForSeconds(delay);

            ResetarIndicadores(); // Reseta visual (cores)
            LimparSequenciaUsuario(); // Reseta o array de entrada
            indiceAtual = 0; // Reseta o índice
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        feedbackTocando = false;
    }

    // Limpa o array de entrada do usuário
    void LimparSequenciaUsuario()
    {
        for (int i = 0; i < sequenciaUsuario.Length; i++)
        {
            sequenciaUsuario[i] = null;
        }
    }

    // FUNÇÃO ATUALIZADA: Usa MaterialPropertyBlock para mudar a Cor Base e a Cor de Emissão
    void AtualizarIndicador(int indice, GameObject tecla)
    {
        if (indice < indicadores.Length)
        {
            Renderer teclaRenderer = tecla.GetComponent<Renderer>();
            Renderer indicadorRenderer = indicadores[indice].GetComponent<Renderer>();

            if (teclaRenderer != null && indicadorRenderer != null)
            {
                // Obter a cor base da tecla (do material compartilhado)
                Color corDaTecla = teclaRenderer.sharedMaterial.GetColor("_BaseColor");

                // Obter o bloco de propriedades ATUAL do indicador
                indicadorRenderer.GetPropertyBlock(propBlock);

                // 1. Definir a nova COR BASE
                propBlock.SetColor("_BaseColor", corDaTecla);

                // 2. Definir a nova COR DE EMISSÃO (para fazer o cubo brilhar)
                propBlock.SetColor("_EmissionColor", corDaTecla);

                // 3. Aplicar o bloco de propriedades de volta ao indicador
                indicadorRenderer.SetPropertyBlock(propBlock);
            }
        }
    }

    // FUNÇÃO ATUALIZADA: Reseta a Cor Base e a Cor de Emissão para o preto (corPreta)
    void ResetarIndicadores()
    {
        foreach (GameObject indicador in indicadores)
        {
            if (indicador != null)
            {
                Renderer renderer = indicador.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.GetPropertyBlock(propBlock);

                    // 1. Resetar COR BASE para a cor de reset
                    propBlock.SetColor("_BaseColor", corPreta);

                    // 2. Resetar COR DE EMISSÃO para a cor de reset (desliga o brilho)
                    propBlock.SetColor("_EmissionColor", corPreta);

                    renderer.SetPropertyBlock(propBlock);
                }
            }
        }
    }

    void TocarSomDoObjeto(GameObject objeto)
    {
        if (feedbackTocando) return;
        AudioSource audio = objeto.GetComponent<AudioSource>();
        if (audio != null && audio.clip != null) { audio.Stop(); audio.Play(); }
    }

    // PararSomDoCubo não é mais usado, mas mantido caso necessite.
    void PararSomDoCubo(GameObject objeto)
    {
        AudioSource audio = objeto.GetComponent<AudioSource>();
        if (audio != null) { audio.Stop(); }
    }
}