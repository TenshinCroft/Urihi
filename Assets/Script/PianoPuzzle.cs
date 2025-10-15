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

    [Header("Materiais de Feedback")]
    public Material materialPreto;

    private int indiceAtual = 0;
    private Rigidbody rbItem;
    private AudioSource audioSource;
    private bool puzzleCompleto = false;
    private bool feedbackTocando = false;
    private LayerMask pianoLayerMask;

    void Start()
    {
        // Certifique-se de que a Layer está corretamente configurada como 'Interação'
        pianoLayerMask = LayerMask.GetMask("Interação");

        if (pianoLayerMask.value == 0)
        {
            Debug.LogError("ERRO: A Layer 'Interação' não foi encontrada. Verifique a ortografia.");
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
        if (puzzleCompleto || feedbackTocando)
            return;

        Vector2 posicaoClique = Vector2.zero;
        bool clicou = false;

        // Detecção de Clique (Omitido por brevidade)
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

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, pianoLayerMask))
            {
                GameObject objetoClicado = hit.collider.gameObject;

                int indiceDoObjeto = Array.IndexOf(ordemCorreta, objetoClicado);

                if (indiceDoObjeto != -1)
                {
                    if (puzzleCompleto)
                    {
                        TocarSomDoObjeto(objetoClicado);
                        return;
                    }

                    bool clicouCorretamente = (objetoClicado == ordemCorreta[indiceAtual]);
                    bool completou = (indiceAtual + 1) >= ordemCorreta.Length;

                    // NOVA LÓGICA DE INDICADOR (Movemos para antes do if/else)
                    // Se o clique for em qualquer tecla, ela acende seu próprio indicador temporariamente
                    // O indicador acendido é o que está na posição atual (indiceAtual)
                    if (indiceAtual < indicadores.Length)
                    {
                        AtualizarIndicador(indiceAtual, objetoClicado);
                    }

                    if (clicouCorretamente)
                    {
                        // Se acertar, o indicador já está aceso e a cor é mantida.

                        if (completou)
                        {
                            if (somAcerto != null) { StartCoroutine(TocarSomFeedback(somAcerto)); }
                            if (itemLiberado != null)
                            {
                                itemLiberado.SetActive(true);
                                if (rbItem != null) { rbItem.isKinematic = false; rbItem.useGravity = true; }
                            }
                            puzzleCompleto = true;
                        }
                        else
                        {
                            TocarSomDoObjeto(objetoClicado);
                            indiceAtual++;
                        }
                    }
                    else // Clicou na tecla errada
                    {
                        PararSomDoCubo(objetoClicado);

                        // O som de erro é tocado
                        if (somErro != null) { StartCoroutine(TocarSomFeedback(somErro)); }

                        // O indicador acende a cor errada, mas é resetado após o feedback
                        // Para permitir que o jogador veja o erro, vamos usar um Coroutine para resetar
                        StartCoroutine(ResetarComAtraso(0.5f)); // Espera 0.5s para o som de erro antes de resetar

                        // Reseta o índice para 0
                        indiceAtual = 0;
                    }
                }
            }
        }
    }

    // NOVO MÉTODO: Reseta os indicadores após um pequeno atraso
    IEnumerator ResetarComAtraso(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetarIndicadores();
    }

    void AtualizarIndicador(int indice, GameObject tecla)
    {
        if (indice < indicadores.Length)
        {
            Renderer teclaRenderer = tecla.GetComponent<Renderer>();
            Renderer indicadorRenderer = indicadores[indice].GetComponent<Renderer>();

            if (teclaRenderer != null && indicadorRenderer != null)
            {
                // Copia o material principal da tecla para o indicador
                indicadorRenderer.material = teclaRenderer.sharedMaterial;
            }
        }
    }

    void ResetarIndicadores()
    {
        if (materialPreto == null)
        {
            Debug.LogError("O Material Preto de Reset não foi atribuído no Inspector do PianoPuzzle.");
            return;
        }

        foreach (GameObject indicador in indicadores)
        {
            if (indicador != null)
            {
                Renderer renderer = indicador.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = materialPreto;
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

    void PararSomDoCubo(GameObject objeto)
    {
        AudioSource audio = objeto.GetComponent<AudioSource>();
        if (audio != null) { audio.Stop(); }
    }

    IEnumerator TocarSomFeedback(AudioClip clip)
    {
        feedbackTocando = true;
        audioSource.Stop();
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        feedbackTocando = false;
    }
}