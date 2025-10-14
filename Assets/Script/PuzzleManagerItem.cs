using UnityEngine;
using System.Collections.Generic;

public class PuzzleManagerItem : MonoBehaviour
{
    // Singleton para acesso fácil de qualquer lugar
    public static PuzzleManagerItem Instance { get; private set; }

    [Header("Configurações do Puzzle")]
    [Tooltip("Total de peças necessárias para completar o puzzle.")]
    public int totalPiecesRequired = 8;

    // Lista para rastrear as peças coletadas
    private HashSet<string> collectedPieces = new HashSet<string>();

    // Propriedade de leitura
    public int CollectedCount => collectedPieces.Count;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // MÉTODO CRÍTICO: Chamado pelo CollectibleItem quando uma peça é coletada
    public void CollectPiece(string pieceName)
    {
        // 1. Adiciona a peça e verifica se é uma nova
        if (collectedPieces.Add(pieceName))
        {
            Debug.Log($"Peça coletada: {pieceName}. Total: {CollectedCount}/{totalPiecesRequired}");

            // 2. Gera e exibe o feedback de contagem
            string feedback = GetCollectionFeedback();
            DisplayFeedbackToPlayer(feedback);

            // 3. Verifica se o puzzle foi completado
            if (CollectedCount >= totalPiecesRequired)
            {
                Debug.Log("Puzzle COMPLETO! Chave ou acesso liberado.");
                // Aqui você pode adicionar a lógica para notificar o Quadro
            }
        }
    }

    // Gera a mensagem de feedback com a contagem (Contagem/Total ou Faltando)
    private string GetCollectionFeedback()
    {
        int restantes = totalPiecesRequired - CollectedCount;

        if (CollectedCount < totalPiecesRequired)
        {
            return $"**Peça coletada!** Faltam {restantes} peças.";
        }
        else
        {
            return "**Todas as peças coletadas!** O puzzle do quadro está liberado!";
        }
    }

    // Encontra o script do Player para exibir a mensagem (Chama Player.ShowFeedback)
    private void DisplayFeedbackToPlayer(string message)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.ShowFeedback(message);
        }
    }

    public bool AreAllPiecesCollected()
    {
        return CollectedCount >= totalPiecesRequired;
    }
}