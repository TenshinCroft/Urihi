using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuração do Item")]
    public string itemName = "Item";
    public AudioClip collectSound;
    public GameObject cartaUI;

    [Header("Puzzle Settings")]
    public bool isPuzzlePiece = false;

    [Header("Key Settings")]
    public bool isKey = false; // Nova opção para marcar se é uma chave

    [Header("Game End Settings")]
    public bool endsGame = false;
    public Image telaPretalFinal;
    public float fadeToBlackDuration = 2f;

    [Header("Textos Finais")]
    public TextMeshProUGUI[] textosFinais = new TextMeshProUGUI[3];
    public float delayEntreTextos = 2f;
    public float textFadeDuration = 1.5f;

    [Header("Scene Settings")]
    public string menuSceneName = "tela inicio";

    private bool _coletado = false;

    void Start()
    {
        // Se não foi atribuída uma tela preta, tenta encontrar automaticamente
        if (endsGame && telaPretalFinal == null)
        {
            GameObject blackScreenObj = GameObject.Find("BlackScreen") ?? GameObject.Find("TelaPretal") ?? GameObject.Find("FadeScreen");
            if (blackScreenObj != null)
            {
                telaPretalFinal = blackScreenObj.GetComponent<Image>();
            }
        }

        // Garante que a tela preta comece invisível
        if (telaPretalFinal != null)
        {
            Color c = telaPretalFinal.color;
            c.a = 0f;
            telaPretalFinal.color = c;
            telaPretalFinal.gameObject.SetActive(false);
        }

        // Garante que os textos comecem invisíveis
        foreach (var texto in textosFinais)
        {
            if (texto != null)
            {
                Color c = texto.color;
                c.a = 0f;
                texto.color = c;
                texto.gameObject.SetActive(false);
            }
        }
    }

    // MÉTODO PÚBLICO para ser chamado pelo Player.cs via raycast
    public void ColetarItem(Player player)
    {
        if (_coletado) return;

        Debug.Log("Item coletado via clique: " + itemName);
        _coletado = true;

        // Toca som na posição do item
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // Verificar se é uma peça do puzzle
        if (isPuzzlePiece)
        {
            PuzzleItemManager manager = PuzzleItemManager.Instance;
            if (manager != null)
            {
                manager.CollectPuzzlePiece(itemName);

                if (player != null)
                {
                    int collected = manager.GetCollectedPiecesCount();
                    int total = manager.totalPiecesRequired;
                    player.ShowFeedback($"Peça do quadro coletada! ({collected}/{total})");

                    if (manager.AreAllPiecesCollected())
                    {
                        player.ShowFeedback("Todas as peças coletadas! O puzzle do quadro está liberado!");
                    }
                }
            }

            Destroy(gameObject);
            return;
        }

        // Verificar se é uma chave (do piano, quadro, ou qualquer outra)
        if (isKey || itemName.ToLower().Contains("chave") || itemName.ToLower().Contains("key"))
        {
            Debug.Log($"Chave coletada: {itemName}");

            if (player != null)
            {
                player._i += 1; // INCREMENTA o contador de chaves/itens do player
                player.ShowFeedback($"{itemName} coletada! ({player._i} chaves)");
                Debug.Log($"Player agora tem {player._i} chaves/itens");
            }

            Destroy(gameObject);
            return;
        }

        // Caso seja o Cassete inicia cutscene original
        if (itemName == "Cassete")
        {
            Debug.Log("Cassete coletada! Tentando iniciar cutscene...");
            EnndingCutscene cutscene = FindObjectOfType<EnndingCutscene>();
            if (cutscene != null)
            {
                Debug.Log("CutsceneController encontrado, iniciando cutscene...");
                cutscene.IniciarCutscene();

                if (player != null)
                {
                    player.ShowFeedback("Cassete Encontrada! O FIM está próximo...");
                }
            }

            Destroy(gameObject);
            return;
        }

        // Se este item finaliza o jogo, inicia sequência de fim
        if (endsGame)
        {
            Debug.Log("Item que finaliza o jogo coletado!");
            if (player != null)
            {
                player.ShowFeedback($"{itemName} coletado! O jogo está acabando...");
            }

            StartCoroutine(GameEndSequence());
            Destroy(gameObject);
            return;
        }

        // Caso seja carta comum abre normalmente
        if (player != null && cartaUI != null)
        {
            player.ShowFeedback("Carta coletada. Pressione F para fechar.");
            player.AbrirCarta(cartaUI);
            Destroy(gameObject);
        }
    }

    private IEnumerator GameEndSequence()
    {
        Debug.Log("Iniciando sequência de fim de jogo...");

        // Desabilita o jogador
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.enabled = false;
        }

        // Desabilita o controle da câmera
        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.enabled = false;
        }

        // Desabilita o CharacterController
        CharacterController controller = FindObjectOfType<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // 1. Fade para tela preta
        yield return StartCoroutine(FadeToBlack());

        // 2. Mostrar textos em sequência
        yield return StartCoroutine(ShowTextsSequence());

        // 3. Finalizar jogo
        FinishGame();
    }

    private IEnumerator FadeToBlack()
    {
        if (telaPretalFinal == null)
        {
            Debug.LogWarning("Tela preta não configurada!");
            yield break;
        }

        telaPretalFinal.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color startColor = telaPretalFinal.color;
        Color targetColor = new Color(0f, 0f, 0f, 1f); // Preto opaco

        while (elapsedTime < fadeToBlackDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / fadeToBlackDuration;
            telaPretalFinal.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        telaPretalFinal.color = targetColor;
        Debug.Log("Fade para preto completado");
    }

    private IEnumerator ShowTextsSequence()
    {
        Debug.Log("Iniciando sequência de textos");

        // Mostrar textos em sequência
        foreach (var texto in textosFinais)
        {
            if (texto != null)
            {
                yield return StartCoroutine(FadeInText(texto));
                yield return new WaitForSecondsRealtime(delayEntreTextos);
            }
        }

        Debug.Log("Sequência de textos completada");
    }

    private IEnumerator FadeInText(TextMeshProUGUI texto)
    {
        if (texto == null) yield break;

        texto.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color startColor = texto.color;
        startColor.a = 0f;
        Color targetColor = startColor;
        targetColor.a = 1f;

        texto.color = startColor;

        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / textFadeDuration;
            texto.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        texto.color = targetColor;
        Debug.Log($"Texto '{texto.text}' apareceu na tela");
    }

    private void FinishGame()
    {
        Debug.Log("Finalizando o jogo...");

        // Reseta configurações do jogo
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Carrega a cena do menu
        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            // Se não tiver cena de menu definida, fecha o jogo
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
