using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuadroPieceManager : MonoBehaviour
{
    [Header("Configurações dos Colliders")]
    [SerializeField] private Vector3 tamanhoCollider = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private bool usarTamanhoAutomatico = true;
    [SerializeField] private float fatorReducao = 0.8f;

    [Header("Configurações do Sistema")]
    [SerializeField] private int totalPecas = 8;
    [SerializeField] private int pecasColetadas = 0;

    [Header("Eventos e Triggers")]
    [SerializeField] private EnemyTriggerEvent segundoEvento;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI legendaTexto;
    [SerializeField] private GameObject legendaCanvas;

    [Header("Recompensa - Chave")]
    [SerializeField] private GameObject chaveRecompensa;
    [SerializeField] private Transform localDropChave;
    [SerializeField] private float forcaDropChave = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip somColetarPeca;
    [SerializeField] private AudioClip somPuzzleCompleto;
    [SerializeField] private AudioClip somDropChave;

    [Header("Efeitos Visuais")]
    [SerializeField] private GameObject efeitoColeta;
    [SerializeField] private GameObject efeitoDropChave;

    private bool sistemaAtivado = false;
    private bool puzzleCompleto = false;
    private List<GameObject> pecasQuadro = new List<GameObject>();

    void Start()
    {
        EncontrarPecasNaCena();
        InicializarChave();
        AtualizarUI();

        // Desativa todas as peças no início
        if (!sistemaAtivado)
        {
            DesativarTodasPecas();
        }

        // Monitora os eventos do bicho
        if (segundoEvento != null)
        {
            StartCoroutine(MonitorarSegundoEvento());
        }
    }


    void EncontrarPecasNaCena()
    {
        // Encontra todas as peças que já estão na cena
        pecasQuadro.Clear();

        for (int i = 1; i <= totalPecas; i++)
        {
            GameObject peca = GameObject.Find("Peça " + i);
            if (peca != null)
            {
                pecasQuadro.Add(peca);
                Debug.Log($"Encontrada peça: {peca.name}");
            }
            else
            {
                Debug.LogWarning($"Peça {i} não encontrada na cena!");
            }
        }

        Debug.Log($"Total de peças encontradas: {pecasQuadro.Count}");
    }

    void InicializarChave()
    {
        // Se não foi definida no inspector, busca automaticamente
        if (chaveRecompensa == null)
        {
            chaveRecompensa = GameObject.Find("Chave do Quarto");
        }

        // Garante que a chave comece desativada
        if (chaveRecompensa != null)
        {
            chaveRecompensa.SetActive(false);
        }

        // Se não foi definido local de drop, usa a posição do puzzle
        if (localDropChave == null)
        {
            GameObject puzzleQuadro = GameObject.Find("Quadro");
            if (puzzleQuadro != null)
            {
                localDropChave = puzzleQuadro.transform;
            }
            else
            {
                localDropChave = transform; // Fallback para este GameObject
            }
        }
    }

    System.Collections.IEnumerator MonitorarSegundoEvento()
    {
        // Espera até o segundo evento ser ativado
        while (segundoEvento != null && segundoEvento.GetComponent<Collider>().enabled)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Aguarda um tempo após o segundo evento
        yield return new WaitForSeconds(2f);

        AtivarSistemaPecas();
    }

    public void AtivarSistemaPecas()
    {
        if (sistemaAtivado) return;

        sistemaAtivado = true;

        // Ativa todas as peças e configura elas para serem coletáveis
        foreach (GameObject peca in pecasQuadro)
        {
            if (peca != null)
            {
                peca.SetActive(true);
                ConfigurarPeca(peca);
            }
        }

        // Mostra mensagem para o player
        if (legendaTexto != null)
        {
            legendaTexto.text = "Peças do quadro apareceram pela casa! Colete todas elas.";
            StartCoroutine(EsconderMensagemTemporaria(3f));
        }

        AtualizarUI();

        Debug.Log("Sistema de peças do quadro ativado!");
    }

    void ConfigurarPeca(GameObject peca)
    {
        // Configura layer de interação
        peca.layer = LayerMask.NameToLayer("Interação");

        // Adiciona o componente de peça coletável
        QuadroPieceCollectable collectible = peca.GetComponent<QuadroPieceCollectable>();
        if (collectible == null)
        {
            collectible = peca.AddComponent<QuadroPieceCollectable>();
        }
        collectible.manager = this;

        // Adiciona efeito visual
        AdicionarEfeitoVisual(peca);

        Debug.Log($"Peça {peca.name} configurada para coleta!");
    }

    void AdicionarEfeitoVisual(GameObject peca)
    {
        // Adiciona uma luz para destacar a peça
        Light luz = peca.GetComponentInChildren<Light>();
        if (luz == null)
        {
            GameObject luzObj = new GameObject("LuzPeca");
            luzObj.transform.SetParent(peca.transform);
            luzObj.transform.localPosition = Vector3.up * 0.5f;

            luz = luzObj.AddComponent<Light>();
            luz.type = LightType.Point;
            luz.color = Color.yellow;
            luz.intensity = 2f;
            luz.range = 3f;
            luz.enabled = true;

            StartCoroutine(AnimarLuz(luz));
        }
    }

    System.Collections.IEnumerator AnimarLuz(Light luz)
    {
        float tempo = 0f;
        float intensidadeBase = luz.intensity;

        while (luz != null && luz.enabled && luz.gameObject != null)
        {
            tempo += Time.deltaTime * 2f;
            luz.intensity = intensidadeBase + Mathf.Sin(tempo) * 0.5f;
            yield return null;
        }
    }

    public void ColetarPeca(GameObject peca)
    {
        if (!sistemaAtivado || puzzleCompleto) return;

        pecasColetadas++;

        // Efeito sonoro
        if (somColetarPeca != null)
        {
            AudioSource.PlayClipAtPoint(somColetarPeca, peca.transform.position);
        }

        // Efeito visual
        if (efeitoColeta != null)
        {
            Instantiate(efeitoColeta, peca.transform.position, Quaternion.identity);
        }

        // Desativa a peça (ao invés de destruir para manter referências)
        peca.SetActive(false);

        AtualizarUI();

        // Verifica se coletou todas
        if (pecasColetadas >= totalPecas)
        {
            CompletarPuzzle();
        }

        Debug.Log($"Peça {peca.name} coletada! {pecasColetadas}/{totalPecas}");
    }

    void AtualizarUI()
    {
        if (legendaTexto != null && sistemaAtivado)
        {
            legendaTexto.text = $"Peças coletadas: {pecasColetadas}/{totalPecas}";

            if (legendaCanvas != null)
            {
                legendaCanvas.SetActive(true);
            }
        }
    }

    void CompletarPuzzle()
    {
        puzzleCompleto = true;

        // Som de conclusão
        if (somPuzzleCompleto != null)
        {
            AudioSource.PlayClipAtPoint(somPuzzleCompleto, transform.position);
        }

        // Atualiza UI
        if (legendaTexto != null)
        {
            legendaTexto.text = "Todas as peças coletadas! Uma chave foi liberada.";
            StartCoroutine(EsconderMensagemTemporaria(4f));
        }

        // Dropa a chave
        StartCoroutine(DroparChave());

        Debug.Log("Puzzle do quadro completo! Chave liberada.");
    }

    System.Collections.IEnumerator DroparChave()
    {
        yield return new WaitForSeconds(1f);

        if (chaveRecompensa != null && localDropChave != null)
        {
            Vector3 posicaoInicialChave = localDropChave.position + Vector3.up * 2f;
            chaveRecompensa.transform.position = posicaoInicialChave;

            chaveRecompensa.SetActive(true);

            Rigidbody rbChave = chaveRecompensa.GetComponent<Rigidbody>();
            if (rbChave != null)
            {
                rbChave.isKinematic = false;
                rbChave.useGravity = true;

                Vector3 forcaInicial = Vector3.up * forcaDropChave + Random.insideUnitSphere * 2f;
                rbChave.AddForce(forcaInicial, ForceMode.Impulse);
            }

            if (efeitoDropChave != null)
            {
                Instantiate(efeitoDropChave, posicaoInicialChave, Quaternion.identity);
            }

            if (somDropChave != null)
            {
                AudioSource.PlayClipAtPoint(somDropChave, posicaoInicialChave);
            }

            Light luzChave = chaveRecompensa.GetComponentInChildren<Light>();
            if (luzChave != null)
            {
                luzChave.enabled = true;
                luzChave.color = Color.green;
                luzChave.intensity = 3f;
                StartCoroutine(AnimarLuzChave(luzChave));
            }

            Debug.Log("Chave dropada com sucesso!");
        }
    }

    System.Collections.IEnumerator AnimarLuzChave(Light luz)
    {
        float tempo = 0f;
        float intensidadeBase = luz.intensity;

        while (luz != null && luz.enabled && luz.gameObject != null)
        {
            tempo += Time.deltaTime * 3f;
            luz.intensity = intensidadeBase + Mathf.Sin(tempo) * 1f;
            yield return null;
        }
    }


    void DesativarTodasPecas()
    {
        foreach (GameObject peca in pecasQuadro)
        {
            if (peca != null)
            {
                peca.SetActive(false);
            }
        }
    }

    System.Collections.IEnumerator EsconderMensagemTemporaria(float tempo)
    {
        yield return new WaitForSeconds(tempo);

        if (legendaTexto != null && !puzzleCompleto)
        {
            AtualizarUI();
        }
        else if (puzzleCompleto && legendaCanvas != null)
        {
            legendaCanvas.SetActive(false);
        }
    }

    // Métodos para testes
    [ContextMenu("Ativar Sistema de Peças")]
    public void AtivarManualmente()
    {
        AtivarSistemaPecas();
    }

    [ContextMenu("Resetar Sistema")]
    public void ResetarSistema()
    {
        pecasColetadas = 0;
        sistemaAtivado = false;
        puzzleCompleto = false;

        // Reativa todas as peças
        foreach (GameObject peca in pecasQuadro)
        {
            if (peca != null)
            {
                peca.SetActive(true);

                // Remove componentes adicionados
                QuadroPieceCollectable collectible = peca.GetComponent<QuadroPieceCollectable>();
                if (collectible != null)
                    DestroyImmediate(collectible);

                // Remove luz se foi adicionada
                Transform luzObj = peca.transform.Find("LuzPeca");
                if (luzObj != null)
                    DestroyImmediate(luzObj.gameObject);
            }
        }

        if (chaveRecompensa != null)
        {
            chaveRecompensa.SetActive(false);
        }

        DesativarTodasPecas();
        AtualizarUI();
    }
}
