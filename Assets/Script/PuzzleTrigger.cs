using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("Referências")]
    public GameObject puzzleUI;

    [HideInInspector] public bool _bool = false;
    private bool isOpen = false;

    private void Update()
    {
        // Não permite abrir ou fechar puzzle se o jogo estiver pausado
        if (SettingsMenu.isPaused) return;

        // 1. Lógica para TENTAR ABRIR o puzzle (acionado pelo Player)
        if (_bool && !isOpen)
        {
            // Verifica se todas as peças foram coletadas antes de abrir o puzzle
            if (CanOpenPuzzle())
            {
                OpenPuzzle();
            }
            else
            {
                ShowPuzzleLockedMessage();
                _bool = false; // Reset para evitar spam
            }
        }

        // 2. Lógica para FECHAR o puzzle (pressionando F)
        // Usamos GetKeyDown para detectar o clique
        if (isOpen && Input.GetKeyDown(KeyCode.F))
        {
            ClosePuzzle();
            _bool = false; // Reset
        }
        // Note: Removido o cheque `_bool && isOpen` para fechar, pois o puzzle pode ser fechado sem o player estar olhando.
    }

    private bool CanOpenPuzzle()
    {
        // Usa o singleton do PuzzleManagerItem
        var manager = PuzzleManagerItem.Instance;

        if (manager == null)
        {
            Debug.LogWarning("PuzzleManagerItem não encontrado! Permitindo acesso ao puzzle (Fallback).");
            return true; // Fallback case: Se não há manager, assume que está pronto.
        }

        // Usa o método AreAllPiecesCollected que definimos no manager
        return manager.AreAllPiecesCollected();
    }

    private void ShowPuzzleLockedMessage()
    {
        var manager = PuzzleManagerItem.Instance;

        if (manager != null)
        {
            // Usa as propriedades públicas CollectedCount e totalPiecesRequired
            int collected = manager.CollectedCount;
            int total = manager.totalPiecesRequired;

            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                // Mostra a contagem de peças que faltam
                player.ShowFeedback($"Puzzle bloqueado! Colete todas as peças do quadro ({collected}/{total})");
            }

            Debug.Log($"Puzzle do quadro bloqueado! Peças coletadas: {collected}/{total}");
        }
        else
        {
            // Feedback caso o manager não esteja na cena
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.ShowFeedback("Erro no puzzle: O Gerenciador de Peças não foi encontrado na cena.");
            }
        }
    }

    public void OpenPuzzle()
    {
        if (puzzleUI != null) puzzleUI.SetActive(true);
        isOpen = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Puzzle aberto!");
    }

    public void ClosePuzzle()
    {
        if (puzzleUI != null) puzzleUI.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Puzzle fechado!");
    }

    public void FecharPuzzle()
    {
        if (isOpen) ClosePuzzle();
        _bool = false;
    }
}