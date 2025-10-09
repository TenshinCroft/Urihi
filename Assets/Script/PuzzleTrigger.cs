using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("Referências")]
    public GameObject puzzleUI;
    public PuzzleItemManager puzzleItemManager;

    [HideInInspector] public bool _bool = false;
    private bool isOpen = false;

    void Start()
    {
        // Se não foi atribuído no inspector, busca automaticamente
        if (puzzleItemManager == null)
        {
            puzzleItemManager = PuzzleItemManager.Instance;
        }
    }

    private void Update()
    {
        // Não permite abrir ou fechar puzzle se o jogo estiver pausado
        if (SettingsMenu.isPaused) return;

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

        if (_bool && isOpen && Input.GetKeyDown(KeyCode.F))
        {
            ClosePuzzle();
            _bool = false;
        }
    }

    private bool CanOpenPuzzle()
    {
        if (puzzleItemManager == null)
        {
            Debug.LogWarning("PuzzleItemManager não encontrado! Permitindo acesso ao puzzle.");
            return true; // Fallback case
        }

        return puzzleItemManager.CanUsePuzzle();
    }

    private void ShowPuzzleLockedMessage()
    {
        if (puzzleItemManager != null)
        {
            int collected = puzzleItemManager.GetCollectedPiecesCount();
            int total = puzzleItemManager.totalPiecesRequired;

            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.ShowFeedback($"Puzzle bloqueado! Colete todas as peças do quadro ({collected}/{total})");
            }

            Debug.Log($"Puzzle do quadro bloqueado! Peças coletadas: {collected}/{total}");
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
