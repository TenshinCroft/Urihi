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
        Debug.Log($"endsGame está configurado como: {endsGame}");
        _coletado = true;

        // Toca som na posição do item
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // Lógica de coleta de itens (Peças de Puzzle, Chaves, Cartas) ...
        // ... (Deixada intacta, assumindo que funcionam) ...

        if (isPuzzlePiece)
        {
            // Notifica o sistema de peças do quadro
            QuadroPieceManager quadroManager = FindObjectOfType<QuadroPieceManager>();
            if (quadroManager != null)
            {
                quadroManager.ColetarPeca(gameObject);
            }

            // Também notifica o PuzzleItemManager
            PuzzleItemManager puzzleManager = PuzzleItemManager.Instance;
            if (puzzleManager != null)
            {
                puzzleManager.CollectPuzzlePieceFromCollectible(itemName);
            }

            if (player != null)
            {
                player.ShowFeedback($"{itemName} coletada!");
            }

            Destroy(gameObject);
            return;
        }

        if (isKey || itemName.ToLower().Contains("chave") || itemName.ToLower().Contains("key"))
        {
            // ... (Lógica da chave) ...
            if (player != null)
            {
                player._i += 1;
                player.ShowFeedback($"{itemName} coletada! ({player._i} chaves)");
            }
            Destroy(gameObject);
            return;
        }

        // Se este item finaliza o jogo, inicia sequência de fim
        if (endsGame)
        {
            Debug.Log("Item que finaliza o jogo coletado! Iniciando sequência de fim.");
            if (player != null)
            {
                player.ShowFeedback($"{itemName} coletado! O jogo está acabando...");
            }

            // CORREÇÃO CRÍTICA:
            // Removemos o 'Destroy(gameObject)' daqui. Se o objeto for destruído,
            // ele mata o 'GameEndSequence' antes que o fade e os textos terminem.
            StartCoroutine(GameEndSequence());
            return; // A destruição agora ocorrerá no final da cena.
        }

        // Caso seja Cassete e não finalize o jogo (lógica de cutscene)
        if (itemName == "Cassete")
        {
            Debug.Log("Cassete coletada! Tentando iniciar cutscene...");
            EnndingCutscene cutscene = FindObjectOfType<EnndingCutscene>();
            if (cutscene != null)
            {
                cutscene.IniciarCutscene();
                if (player != null)
                {
                    player.ShowFeedback("Cassete Encontrada! O FIM está próximo...");
                }
            }
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

        // Congela o tempo do jogo para que apenas os Coroutines baseados em Time.unscaledDeltaTime continuem.
        // Isso garante que a UI animada não dependa da taxa de quadros do jogo (que pode travar).
        Time.timeScale = 0f;

        // Desabilita o jogador e a câmera
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.enabled = false;
        }

        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.enabled = false;
        }

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

        // Destruição do objeto aqui (embora a mudança de cena o torne irrelevante, é para limpeza)
        Destroy(gameObject);
    }

    private IEnumerator FadeToBlack()
    {
        Debug.Log("Iniciando FadeToBlack()");
        if (telaPretalFinal == null)
        {
            Debug.LogWarning("Tela preta não configurada!");
            yield break;
        }

        // Garante que a tela preta está ativa antes de tentar mudar a cor
        telaPretalFinal.gameObject.SetActive(true);

        // Garante que a imagem tenha cor preta, começando transparente
        Color initialColor = Color.black;
        initialColor.a = 0f;
        telaPretalFinal.color = initialColor;

        // Configura RectTransform para ocupar toda a tela (melhor prática de UI)
        RectTransform rectTransform = telaPretalFinal.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;      // (0,0)
            rectTransform.anchorMax = Vector2.one;       // (1,1) 
            rectTransform.offsetMin = Vector2.zero;      // Left e Bottom = 0
            rectTransform.offsetMax = Vector2.zero;      // Right e Top = 0
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        float elapsedTime = 0f;
        Color startColor = telaPretalFinal.color;
        Color targetColor = new Color(0f, 0f, 0f, 1f); // Preto opaco

        Debug.Log($"Iniciando fade de {startColor} para {targetColor} em {fadeToBlackDuration} segundos (Usando Time.unscaledDeltaTime)");

        while (elapsedTime < fadeToBlackDuration)
        {
            // Usamos Time.unscaledDeltaTime para que o fade funcione mesmo com Time.timeScale = 0f
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeToBlackDuration); // Usar Clamp01 garante que t não passe de 1
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            telaPretalFinal.color = currentColor;

            yield return null;
        }

        telaPretalFinal.color = targetColor;
        Debug.Log($"Fade para preto completado! Cor final: {telaPretalFinal.color}");
    }

    private IEnumerator ShowTextsSequence()
    {
        Debug.Log("Iniciando sequência de textos");
        if (textosFinais.Length == 0)
        {
            Debug.LogWarning("Nenhum texto final configurado!");
            yield break;
        }

        // Mostrar textos em sequência
        for (int i = 0; i < textosFinais.Length; i++)
        {
            var texto = textosFinais[i];
            if (texto != null)
            {
                Debug.Log($"Mostrando texto {i + 1}: '{texto.text}' no objeto '{texto.name}'");
                yield return StartCoroutine(FadeInText(texto));

                // Usamos WaitForSecondsRealtime para ignorar Time.timeScale = 0f
                yield return new WaitForSecondsRealtime(delayEntreTextos);
            }
            else
            {
                Debug.LogWarning($"Texto {i + 1} é null!");
            }
        }

        Debug.Log("Sequência de textos completada");
    }

    private IEnumerator FadeInText(TextMeshProUGUI texto)
    {
        if (texto == null)
        {
            Debug.LogWarning("Texto é null no FadeInText!");
            yield break;
        }

        Debug.Log($"Iniciando fade do texto: '{texto.text}' no objeto '{texto.name}'");

        texto.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color startColor = texto.color;
        startColor.a = 0f;
        Color targetColor = startColor;
        targetColor.a = 1f;

        texto.color = startColor;

        Debug.Log($"Fazendo fade de {startColor} para {targetColor} em {textFadeDuration} segundos (Usando Time.unscaledDeltaTime)");

        while (elapsedTime < textFadeDuration)
        {
            // Usamos Time.unscaledDeltaTime
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / textFadeDuration);
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            texto.color = currentColor;

            yield return null;
        }

        texto.color = targetColor;
        Debug.Log($"Texto '{texto.text}' apareceu na tela.");
    }

    private void FinishGame()
    {
        Debug.Log("Finalizando o jogo...");

        // Restaura o Time.timeScale antes de carregar a nova cena
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