using UnityEngine;
using System.Collections.Generic;

public class PuzzleItemManager : MonoBehaviour
{
    [Header("Configuração do Puzzle")]
    public int totalPiecesRequired = 8;
    public string[] puzzlePieceNames = new string[8]
    {
        "Peça 1", "Peça 2", "Peça 3", "Peça 4",
        "Peça 5", "Peça 6", "Peça 7", "Peça 8"
    };

    [Header("Referências")]
    public PuzzleTrigger puzzleQuadro;

    private HashSet<string> collectedPieces = new HashSet<string>();
    private static PuzzleItemManager instance;

    public static PuzzleItemManager Instance
    {
        get
        {
            if (instance == null)
                instance = Object.FindFirstObjectByType<PuzzleItemManager>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (puzzleQuadro == null)
            puzzleQuadro = Object.FindFirstObjectByType<PuzzleTrigger>();
    }

    public void CollectPuzzlePiece(string pieceName)
    {
        if (System.Array.Exists(puzzlePieceNames, piece => piece == pieceName))
        {
            if (!collectedPieces.Contains(pieceName))
            {
                collectedPieces.Add(pieceName);
                Debug.Log($"Peça coletada: {pieceName} ({collectedPieces.Count}/{totalPiecesRequired})");

                Player player = Object.FindFirstObjectByType    <Player>();
                if (player != null)
                {
                    player.ShowFeedback($"Peça do puzzle coletada! ({collectedPieces.Count}/{totalPiecesRequired})");
                }

                if (AreAllPiecesCollected())
                {
                    Debug.Log("Todas as peças foram coletadas! Puzzle liberado!");
                    if (player != null)
                    {
                        player.ShowFeedback("Todas as peças coletadas! O puzzle do quadro está liberado!");
                    }
                }
            }
        }
    }
    public void CollectPuzzlePieceFromCollectible(string pieceName)
    {
        // Método específico para ser chamado pelo CollectibleItem
        CollectPuzzlePiece(pieceName);

        // Feedback adicional quando todas as peças são coletadas
        if (AreAllPiecesCollected())
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.ShowFeedback("Puzzle do quadro desbloqueado! Vá até o quadro para resolver o puzzle.");
            }
        }
    }
    public bool AreAllPiecesCollected()
    {
        return collectedPieces.Count >= totalPiecesRequired;
    }

    public int GetCollectedPiecesCount()
    {
        return collectedPieces.Count;
    }

    public bool CanUsePuzzle()
    {
        return AreAllPiecesCollected();
    }
}
