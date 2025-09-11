using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuração do Item")]
    public string itemName = "Item";
    public AudioClip collectSound;
    public GameObject cartaUI;  // UI que será mostrada quando coletar

    private bool _coletado = false;

    void OnTriggerEnter(Collider other)
    {
        if (_coletado) return; // impede ativar mais de uma vez
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item coletado: " + itemName);

            // toca som na posição da carta
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // pede para o Player abrir a UI
            Player player = other.GetComponent<Player>();
            if (player != null && cartaUI != null)
            {
                player.AbrirCarta(cartaUI);
            }

            _coletado = true;
        }
    }
}
