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
    public bool isPuzzlePiece = false; // SE TRUE, USA O PuzzleItemManager
    // NOVO: Se TRUE, o item só aparece quando o método ActivateItem() for chamado.
    public bool requiresExternalActivation = false;

    [Header("Key Settings")]
    public bool isKey = false;

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

    // MANTIDA para peças de puzzle, chaves e fim de jogo.
    // Para cartas, a lógica de reabrir a UI não depende desta flag.
    private bool _coletado = false;

    void Start()
    {
        // Se a peça do puzzle está ativa, garantimos que ela tem a tag e a layer correta.
        if (isPuzzlePiece)
        {
            if (!gameObject.CompareTag("Item"))
            {
                gameObject.tag = "Item"; // IMPORTANTE: Player.cs interage com a Tag "Item"
            }
            int interactionLayer = LayerMask.NameToLayer("Interagivel");
            if (gameObject.layer != interactionLayer && interactionLayer != -1)
            {
                gameObject.layer = interactionLayer;
            }
            DeactivateItem(); // não precisa ser chamado aqui se for cuidado pelo bloco requiresExternalActivation
        }

        // CORREÇÃO: O item só começa DESATIVADO se for uma peça de puzzle E requer ativação externa.
        // Carta comum deve começar ATIVA.
        if (isPuzzlePiece && requiresExternalActivation)
        {
            gameObject.SetActive(false);
        }

        // Se não foi atribuída uma tela preta, tenta encontrar automaticamente
        if (endsGame && telaPretalFinal == null)
        {
            GameObject blackScreenObj = GameObject.Find("BlackScreen") ?? GameObject.Find("TelaPretal") ?? GameObject.Find("FadeScreen");
            if (blackScreenObj != null)
            {
                telaPretalFinal = blackScreenObj.GetComponent<Image>();
            }
        }

        // Garante que a tela preta e os textos comecem invisíveis
        if (telaPretalFinal != null)
        {
            Color c = telaPretalFinal.color; c.a = 0f; telaPretalFinal.color = c;
            telaPretalFinal.gameObject.SetActive(false);
        }
        foreach (var texto in textosFinais)
        {
            if (texto != null)
            {
                Color c = texto.color; c.a = 0f; texto.color = c;
                texto.gameObject.SetActive(false);
            }
        }
    }

    // NOVO MÉTODO PÚBLICO: Torna o item visível e coletável
    public void ActivateItem()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Debug.Log($"Item '{itemName}' ativado (apareceu) após evento.");
        }
    }

    public void DeactivateItem()
    {
        // Mantenho este método, se for chamado por fora ou pela lógica de requiresExternalActivation.
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            Debug.Log($"Item '{itemName}' desativado (desapareceu) antes do jogo.");
        }
    }


    // MÉTODO PÚBLICO para ser chamado pelo Player.cs via raycast
    public void ColetarItem(Player player)
    {
        // Define se o item é consumível (desaparece ou finaliza algo)
        bool isConsumable = isPuzzlePiece || isKey || endsGame || itemName == "Cassete";

        if (_coletado && isConsumable) return;


        Debug.Log("Item coletado via clique: " + itemName);

        if (isConsumable)
        {
            _coletado = true; // Só marca como coletado se for consumível
        }


        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // ===============================================
        // LÓGICA DE COLETA DE PEÇA DO PUZZLE (RESTAURADO!)
        // ===============================================
        if (isPuzzlePiece)
        {
            if (PuzzleManagerItem.Instance != null)
            {
                // Chama o Manager, que lida com a contagem e exibe o feedback correto
                // Usa o nome do objeto para evitar duplicação de coleta
                PuzzleManagerItem.Instance.CollectPiece(gameObject.name);
            }
            else
            {
                if (player != null)
                {
                    player.ShowFeedback($"Peça do puzzle coletada: {itemName}. Faltando PuzzleItemManager.");
                }
            }

            // Destrói o objeto APÓS a contagem
            Destroy(gameObject);
            return;
        }
        // ===============================================

        // Lógica da CHAVE
        if (isKey || itemName.ToLower().Contains("chave") || itemName.ToLower().Contains("key"))
        {
            if (player != null)
            {
                player._i += 1;
                player.ShowFeedback($"{itemName} coletada! ({player._i} chaves)");
            }
            Destroy(gameObject);
            return;
        }

        // Lógica de FIM DE JOGO
        if (endsGame)
        {
            if (player != null)
            {
                player.ShowFeedback($"{itemName} coletado! O jogo está acabando...");
            }
            StartCoroutine(GameEndSequence());
            return;
        }

        // Caso seja Cassete e não finalize o jogo (lógica de cutscene)
        if (itemName == "Cassete")
        {
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

        // LÓGICA DA CARTA COMUM
        if (player != null && cartaUI != null)
        {
            // Esta lógica NÃO marca _coletado = true e NÃO destrói o objeto, 
            // permitindo a reinteração (reabertura).
            player.ShowFeedback("Carta coletada. Pressione F para fechar.");
            player.AbrirCarta(cartaUI);
            return;
        }

        // Se o item não for nenhum dos tipos especiais acima, destrói após a coleta (comportamento padrão de item)
        if (isConsumable)
        {
            Destroy(gameObject);
        }
    }

    // O restante do código (GameEndSequence, FadeToBlack, ShowTextsSequence, FadeInText, FinishGame)
    // é mantido como na correção anterior.

    private IEnumerator GameEndSequence()
    {
        // Desativar movimento/olhar do jogador
        Player player = FindObjectOfType<Player>();
        if (player != null) { player.enabled = false; }

        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null) { playerLook.enabled = false; }

        CharacterController controller = FindObjectOfType<CharacterController>();
        if (controller != null) { controller.enabled = false; }

        yield return StartCoroutine(FadeToBlack());
        yield return StartCoroutine(ShowTextsSequence());

        FinishGame();
    }

    private IEnumerator FadeToBlack()
    {
        if (telaPretalFinal == null)
        {
            Debug.LogError("TelaPretalFinal (Image) não está atribuída ou não foi encontrada!");
            yield break;
        }

        telaPretalFinal.gameObject.SetActive(true);

        RectTransform rectTransform = telaPretalFinal.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        float elapsedTime = 0f;
        Color startColor = new Color(0f, 0f, 0f, 0f);
        Color targetColor = new Color(0f, 0f, 0f, 1f);
        telaPretalFinal.color = startColor;

        while (elapsedTime < fadeToBlackDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeToBlackDuration);
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            telaPretalFinal.color = currentColor;
            yield return null;
        }

        telaPretalFinal.color = targetColor;
    }

    private IEnumerator ShowTextsSequence()
    {
        if (textosFinais.Length == 0) { yield break; }

        for (int i = 0; i < textosFinais.Length; i++)
        {
            var texto = textosFinais[i];
            if (texto != null)
            {
                yield return StartCoroutine(FadeInText(texto));
                yield return new WaitForSecondsRealtime(delayEntreTextos);
            }
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI texto)
    {
        if (texto == null) { yield break; }

        texto.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color startColor = texto.color; startColor.a = 0f;
        Color targetColor = texto.color; targetColor.a = 1f;

        texto.color = startColor;

        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / textFadeDuration);
            Color currentColor = Color.Lerp(startColor, targetColor, t);
            texto.color = currentColor;
            yield return null;
        }

        texto.color = targetColor;
    }

    private void FinishGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        Destroy(gameObject);
    }
}