using Unity.VisualScripting;
using UnityEngine;

public class tp : MonoBehaviour
{
    [Header("Cenas disponíveis pra teleportar")]
    public string[] cenasDisponiveis = new string[] { "Testes", "Ambiente", "CasaInterna" };

    [Header("Cena de destino")]
    public int indiceCenaDestino = 0; // Index pra escolher cena da lista no editor
    private void OnTriggerEnter(Collider other)
    {
        // Confirma se foi o jogador que encostou (tag "Player")
        if (other.CompareTag("Player"))
        {
            // Pega o nome da cena com base no índice
            if (indiceCenaDestino >= 0 && indiceCenaDestino < cenasDisponiveis.Length)
            {
                string cenaEscolhida = cenasDisponiveis[indiceCenaDestino];
                Debug.Log("Teleportando pra cena: " + cenaEscolhida);
                MySceneManager._inst.LoadScene(cenaEscolhida);
            }
            else
            {
                Debug.LogWarning("Índice de cena inválido!");
            }
        }
    }
}
