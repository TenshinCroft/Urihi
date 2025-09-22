using UnityEngine;

public class TriggerEvent2 : MonoBehaviour
{

    public EnemyTrigger manager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger colidiu com: " + other.name);

        if (other.CompareTag("Player"))
        {
            if (manager != null)
            {
                manager.TriggerEvent2();
            }
            else
            {
                Debug.LogWarning("EnemyTrigger não atribuído no TriggerEvent2.");
            }

            gameObject.SetActive(false); // desativa para não repetir
        }
    }
}
