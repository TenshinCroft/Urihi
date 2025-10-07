using UnityEngine;

public class EnemyDeathTrigger : MonoBehaviour
{
    private Enemy enemyScript;
    private PlayerDeathSystem playerDeathSystem;
    private bool hasTriggeredDeath = false;

    private void Start()
    {
        enemyScript = GetComponent<Enemy>();

        // Find player death system
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerDeathSystem = player.GetComponent<PlayerDeathSystem>();
            if (playerDeathSystem == null)
            {
                playerDeathSystem = player.AddComponent<PlayerDeathSystem>();
            }
        }
    }

    private void Update()
    {
        // Check if enemy is attacking and hasn't triggered death yet
        if (enemyScript != null && enemyScript._plyAtq && !hasTriggeredDeath)
        {
            TriggerPlayerDeath();
        }
    }

    private void TriggerPlayerDeath()
    {
        hasTriggeredDeath = true;

        if (playerDeathSystem != null)
        {
            playerDeathSystem.TriggerDeath(transform);
        }
    }
}
