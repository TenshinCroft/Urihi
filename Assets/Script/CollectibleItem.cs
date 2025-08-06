using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Item";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item coletado: " + itemName);
            // Aqui você pode adicionar lógica de inventário, pontuação, etc.
            Destroy(gameObject); // Remove o item da cena após ser coletado
        }
    }
}
