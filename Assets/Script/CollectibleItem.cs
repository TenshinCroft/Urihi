using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Item";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item coletado: " + itemName);
            // Aqui voc� pode adicionar l�gica de invent�rio, pontua��o, etc.
            Destroy(gameObject); // Remove o item da cena ap�s ser coletado
        }
    }
}
