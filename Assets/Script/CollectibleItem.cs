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

    // MÉTODO PÚBLICO para ser chamado pelo Player.cs via raycast
    public void ColetarItem(Player player)
    {
        if (_coletado) return;

        Debug.Log("Item coletado via clique: " + itemName);
        _coletado = true;

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // ===============================================
        // LÓGICA DE COLETA DE PEÇA DO PUZZLE (CORREÇÃO)
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

        // Caso seja carta comum abre normalmente
        if (player != null && cartaUI != null)
        {
            player.ShowFeedback("Carta coletada. Pressione F para fechar.");
            player.AbrirCarta(cartaUI);
            Destroy(gameObject);
        }
    }

    // O restante do código (GameEndSequence, FadeToBlack, etc.)
    // é mantido como no seu script original.

    private IEnumerator GameEndSequence()
    {
        Time.timeScale = 0f;

        Player player = FindObjectOfType<Player>();
        if (player != null) { player.enabled = false; }

        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null) { playerLook.enabled = false; }

        CharacterController controller = FindObjectOfType<CharacterController>();
        if (controller != null) { controller.enabled = false; }

        yield return StartCoroutine(FadeToBlack());
        yield return StartCoroutine(ShowTextsSequence());

        FinishGame();
        Destroy(gameObject);
    }

    private IEnumerator FadeToBlack()
    {
        if (telaPretalFinal == null) { yield break; }
        telaPretalFinal.gameObject.SetActive(true);
        Color initialColor = Color.black; initialColor.a = 0f; telaPretalFinal.color = initialColor;

        RectTransform rectTransform = telaPretalFinal.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero; rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero; rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero; rectTransform.sizeDelta = Vector2.zero;
        }

        float elapsedTime = 0f;
        Color startColor = telaPretalFinal.color;
        Color targetColor = new Color(0f, 0f, 0f, 1f);

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
        Color targetColor = startColor; targetColor.a = 1f;

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
    }
}