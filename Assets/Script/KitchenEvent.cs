using UnityEngine;
using System.Collections;
/// <summary>
/// Ativa luzes e um objeto específico quando o Player entra em um Trigger Collider.
/// Este script deve ser anexado ao GameObject com o Collider marcado como 'Is Trigger'.
/// </summary>
public class KitchenEvent : MonoBehaviour
{
    [Header("Objetos a serem ativados")]
    public Light[] luzesParaLigar;
    public GameObject objetoParaAtivar;

    [Header("Configuração de Áudio")]
    // O componente que reproduzirá o som. Deve estar no mesmo GameObject ou em um filho.
    public AudioSource eventoAudioSource;
    // O arquivo de som (clip) que será tocado.
    public AudioClip eventoAudioClip;

    [Header("Configuração de Tempo")]
    public float duracaoVisivel = 1.0f;

    // ---------------------
    // POST PROCESSING
    // ---------------------
    [Header("Efeito de Câmera (Post Processing)")]
    public GameObject postProcessingVolume;

    [Header("Configuração do Trigger")]
    public string playerTag = "Player";
    public bool executarApenasUmaVez = true;

    private bool eventoAtivado = false;

    // A função é chamada no primeiro frame em que o Player entra neste trigger.
    private void OnTriggerEnter(Collider other)
    {
        // 1. Verifica se o evento já foi ativado
        if (executarApenasUmaVez && eventoAtivado)
        {
            return;
        }

        // 2. Verifica se o objeto que entrou tem a tag correta (o Player)
        if (other.CompareTag(playerTag))
        {
            AtivarEvento();
        }
    }

    /// <summary>
    /// Inicia a ativação do evento, incluindo o áudio.
    /// </summary>
    private void AtivarEvento()
    {
        Debug.Log("Trigger ativado! Iniciando evento (Luzes ON, Objeto aparece/desaparece).");

        // ------------------ NOVO: REPRODUÇÃO DE ÁUDIO ------------------
        if (eventoAudioSource != null && eventoAudioClip != null)
        {
            eventoAudioSource.PlayOneShot(eventoAudioClip);
            Debug.Log($"Áudio '{eventoAudioClip.name}' tocado.");
        }
        else if (eventoAudioClip != null)
        {
            // Tenta obter o AudioSource no próprio objeto se a referência estiver faltando
            AudioSource localSource = GetComponent<AudioSource>();
            if (localSource != null)
            {
                localSource.PlayOneShot(eventoAudioClip);
                Debug.Log($"Áudio '{eventoAudioClip.name}' tocado por AudioSource local.");
            }
            else
            {
                Debug.LogError("ERRO: AudioSource não referenciado e nem encontrado no GameObject do Trigger!");
            }
        }
        // ---------------------------------------------------------------

        // Ativação das Luzes (Permanentes)
        foreach (Light luz in luzesParaLigar)
        {
            if (luz != null)
            {
                luz.enabled = true;
                Debug.Log($"Luz ativada: {luz.gameObject.name}");
            }
            else
            {
                Debug.LogError("ERRO: Uma luz no array está nula ou faltando!");
            }
        }

        // Inicia a Corotina que controla o objeto e o Post-Processing
        if (objetoParaAtivar != null || postProcessingVolume != null)
        {
            StartCoroutine(ControlarObjetoETempo());
        }
        else
        {
            Debug.LogError("ERRO: Objeto Para Ativar e Post Processing Volume estão nulos. Nenhum efeito temporário para iniciar!");
        }

        eventoAtivado = true;
    }

    /// <summary>
    /// Corrotina que ativa o objeto E o efeito de Post-Processing, espera, e desativa ambos.
    /// </summary>
    IEnumerator ControlarObjetoETempo()
    {
        // ------------------ INÍCIO DO EFEITO ------------------
        if (postProcessingVolume != null)
        {
            postProcessingVolume.SetActive(true);
            Debug.Log("Post-Processing Volume ATIVADO.");
        }

        if (objetoParaAtivar != null)
        {
            // 1. ATIVA o objeto
            objetoParaAtivar.SetActive(true);
            Debug.Log($"Objeto {objetoParaAtivar.name} ATIVADO.");
        }

        // 2. ESPERA pelo tempo definido no Inspector
        yield return new WaitForSeconds(duracaoVisivel);

        // ------------------ FIM DO EFEITO ------------------

        if (objetoParaAtivar != null)
        {
            // 3. DESATIVA o objeto
            objetoParaAtivar.SetActive(false);
            Debug.Log($"Objeto {objetoParaAtivar.name} DESATIVADO após {duracaoVisivel} segundos.");
        }

        // 4. DESATIVA o Post-Processing
        if (postProcessingVolume != null)
        {
            postProcessingVolume.SetActive(false);
            Debug.Log("Post-Processing Volume DESATIVADO.");
        }
    }
}
