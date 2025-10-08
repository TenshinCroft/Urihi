using UnityEngine;

public class canva : MonoBehaviour
{
    [Header("Victory Settings")]
    public GameObject win; // Canvas de vitória
    public int itemsToWin = 9;

    [Header("References")]
    public GameObject _player;
    public GameObject inimigo;

    private int contador = 0;
    private bool gameEnded = false;

    void Update()
    {
        // Se o jogo já terminou, não fazer mais nada
        if (gameEnded) return;

        // Atualizar contador de itens do player
        if (_player != null && _player.GetComponent<Player>() != null)
        {
            contador = _player.GetComponent<Player>()._i;
        }

        // Verificar condição de vitória
        if (contador >= itemsToWin && win != null && !win.activeSelf)
        {
            win.SetActive(true);
            gameEnded = true;
            Debug.Log("Venceu!");

            if (inimigo != null)
            {
                Destroy(inimigo);
            }
        }

        // REMOVIDO: A detecção de morte agora é feita pelo PlayerDeathSystem
        // Isso evita conflitos e duplicação de lógica
    }
}
