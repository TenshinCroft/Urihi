using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PianoPuzzle : MonoBehaviour
{
    [Header("Ordem correta dos cubos (arraste aqui no Inspector)")]
    public GameObject[] ordemCorreta;

    [Header("Item que será liberado ao completar o puzzle")]
    public GameObject itemLiberado;

    [Header("Sons de feedback")]
    public AudioClip somErro;
    public AudioClip somAcerto;

    private int indiceAtual = 0;
    private Rigidbody rbItem;
    private AudioSource audioSource;
    private bool puzzleCompleto = false;
    private bool feedbackTocando = false;

    void Start()
    {
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
    }

    void Update()
    {
        if (feedbackTocando)
            return; // Bloqueia qualquer interação enquanto som de erro/acerto está tocando

        Vector2 posicaoClique = Vector2.zero;
        bool clicou = false;

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

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject objetoClicado = hit.collider.gameObject;

                int indiceDoObjeto = System.Array.IndexOf(ordemCorreta, objetoClicado);

                if (indiceDoObjeto != -1)
                {
                    // Após o puzzle ser concluído, apenas toca o som dos cubos livremente
                    if (puzzleCompleto)
                    {
                        TocarSomDoObjeto(objetoClicado);
                        return;
                    }

                    bool clicouCorretamente = objetoClicado == ordemCorreta[indiceAtual];
                    bool completou = (indiceAtual + 1) >= ordemCorreta.Length;

                    if (completou && clicouCorretamente)
                    {
                        PararSomDoCubo(objetoClicado); // Não toca a última nota

                        if (somAcerto != null)
                        {
                            StartCoroutine(TocarSomFeedback(somAcerto));
                        }

                        if (itemLiberado != null)
                        {
                            itemLiberado.SetActive(true);

                            if (rbItem != null)
                            {
                                rbItem.isKinematic = false;
                                rbItem.useGravity = true;
                            }
                        }

                        puzzleCompleto = true;
                        Debug.Log("Puzzle completo");
                    }
                    else if (clicouCorretamente)
                    {
                        TocarSomDoObjeto(objetoClicado);
                        indiceAtual++;
                    }
                    else
                    {
                        PararSomDoCubo(objetoClicado); // Evita sobreposição

                        if (somErro != null)
                        {
                            StartCoroutine(TocarSomFeedback(somErro));
                        }

                        indiceAtual = 0;
                        Debug.Log("Reiniciando sequência");
                    }
                }
            }
        }
    }

    void TocarSomDoObjeto(GameObject objeto)
    {
        if (feedbackTocando) return; // Impede som do cubo durante erro/acerto

        AudioSource audio = objeto.GetComponent<AudioSource>();
        if (audio != null && audio.clip != null)
        {
            audio.Stop();
            audio.Play();
        }
    }

    void PararSomDoCubo(GameObject objeto)
    {
        AudioSource audio = objeto.GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Stop();
        }
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



