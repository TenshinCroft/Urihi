using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Configurações")]
    public int _totalPieces; // total de peças no puzzle
    private int _placedPieces = 0; // quantas já foram encaixadas

    [Header("Referências")]
    public GameObject _rewardObject; // opcional: objeto que será liberado (porta, item, etc)

    // chamado pelo PuzzlePiece quando uma peça é colocada
    public void PiecePlaced()
    {
        _placedPieces++;

        if (_placedPieces >= _totalPieces)
        {
            PuzzleSolved();
        }
    }

    void PuzzleSolved()
    {
        Debug.Log("Puzzle resolvido!");

        // exemplo: ativar recompensa
        if (_rewardObject != null)
        {
            _rewardObject.SetActive(true);
        }

        // fecha o puzzle
        GetComponentInParent<PuzzleTrigger>().FecharPuzzle();
    }
}
