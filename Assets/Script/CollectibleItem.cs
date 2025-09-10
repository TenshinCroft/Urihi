using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Item";
  
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item coletado: " + itemName);
           
            Destroy(gameObject); 
        }
    }
}
