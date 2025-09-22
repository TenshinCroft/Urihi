using UnityEngine;

public class TriggerEvent1 : MonoBehaviour
{
    public EnemyTrigger manager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger colidiu com: " + other.name);

        if (other.CompareTag("Player"))
        {
            if (manager != null)
            {
                manager.TriggerEvent1();
            }
            else
            {
                Debug.LogWarning("EnemyTrigger não atribuído no TriggerEvent1.");
            }

            gameObject.SetActive(false); // desativa para não repetir
        }
    }
}
