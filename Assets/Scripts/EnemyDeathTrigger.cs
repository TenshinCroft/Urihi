using UnityEngine;

public class EnemyDeathTrigger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public bool showEnemyStateInUI = true;

    [Header("References")]
    public Enemy enemyScript;
    public PlayerDeathSystem playerDeathSystem;

    private bool hasTriggeredDeath = false;
    private bool wasAttacking = false;

    private void Awake()
    {
        if (enableDebugLogs) Debug.Log("=== EnemyDeathTrigger Awake ===");
    }

    private void Start()
    {
        if (enableDebugLogs) Debug.Log("=== EnemyDeathTrigger Start ===");

        // Get Enemy script from this GameObject
        if (enemyScript == null)
        {
            enemyScript = GetComponent<Enemy>();
        }

        if (enemyScript == null)
        {
            Debug.LogError("❌ Enemy script not found on " + gameObject.name);
            return;
        }
        else
        {
            if (enableDebugLogs) Debug.Log("✅ Enemy script found successfully");
        }

        // Check if enemy has player reference
        if (enemyScript._p == null)
        {
            Debug.LogWarning("⚠️ Enemy does not have Player reference! Trying to find automatically...");

            // Try to find player automatically
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                enemyScript._p = playerObj;
                Debug.Log("✅ Player reference auto-assigned to Enemy: " + playerObj.name);
            }
            else
            {
                Debug.LogError("❌ No GameObject with 'Player' tag found!");
            }
        }
        else
        {
            if (enableDebugLogs) Debug.Log("✅ Enemy has Player reference: " + enemyScript._p.name);
        }

        // Find player death system
        if (playerDeathSystem == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerDeathSystem = player.GetComponent<PlayerDeathSystem>();
                if (playerDeathSystem == null)
                {
                    Debug.LogError("❌ PlayerDeathSystem not found on Player!");
                }
                else
                {
                    if (enableDebugLogs) Debug.Log("✅ EnemyDeathTrigger found PlayerDeathSystem successfully");
                }
            }
            else
            {
                Debug.LogError("❌ Player GameObject with tag 'Player' not found!");
            }
        }

        // Final check
        if (enemyScript != null && playerDeathSystem != null)
        {
            Debug.Log("🎯 EnemyDeathTrigger fully initialized and ready!");
        }
        else
        {
            Debug.LogError("💥 EnemyDeathTrigger initialization failed!");
        }
    }

    private void Update()
    {
        if (enemyScript == null || playerDeathSystem == null) return;

        // Continuous debug of enemy state (every 2 seconds)
        if (enableDebugLogs && Time.time % 2f < 0.1f)
        {
            float distanceToPlayer = enemyScript._p != null ?
                Vector3.Distance(transform.position, enemyScript._p.transform.position) : -1f;

            Debug.Log($"🔍 Enemy Status - Attack: {enemyScript._plyAtq} | Distance: {distanceToPlayer:F1} | PlayerVisible: {enemyScript._playerVisible}");
        }

        // Debug the enemy attack state change
        if (enemyScript._plyAtq != wasAttacking)
        {
            wasAttacking = enemyScript._plyAtq;
            Debug.Log($"🚨 Enemy attack state changed: {wasAttacking}");

            if (wasAttacking)
            {
                Debug.Log("⚔️ ENEMY IS NOW ATTACKING!");
            }
        }

        // Check if enemy is attacking and hasn't triggered death yet
        if (enemyScript._plyAtq && !hasTriggeredDeath)
        {
            Debug.Log("💀 Enemy is attacking! Triggering player death...");
            TriggerPlayerDeath();
        }
    }

    private void TriggerPlayerDeath()
    {
        hasTriggeredDeath = true;
        Debug.Log("💀 === TRIGGERING PLAYER DEATH ===");

        if (playerDeathSystem != null)
        {
            playerDeathSystem.TriggerDeath(transform);
            Debug.Log("✅ Death trigger sent to PlayerDeathSystem");
        }
        else
        {
            Debug.LogError("❌ PlayerDeathSystem is null when trying to trigger death!");
        }
    }

    // Método para resetar o trigger (útil para testes)
    [ContextMenu("Reset Death Trigger")]
    public void ResetDeathTrigger()
    {
        hasTriggeredDeath = false;
        wasAttacking = false;
        Debug.Log("🔄 Death trigger reset");
    }

    // Método para forçar teste de morte
    [ContextMenu("Force Death Test")]
    public void ForceDeathTest()
    {
        Debug.Log("🧪 === FORCING DEATH TEST ===");
        if (playerDeathSystem != null)
        {
            playerDeathSystem.TriggerDeath(transform);
            Debug.Log("✅ Forced death test triggered");
        }
        else
        {
            Debug.LogError("❌ PlayerDeathSystem not found for death test!");
        }
    }

    // Show status in GUI for easier debugging
    private void OnGUI()
    {
        if (!showEnemyStateInUI || enemyScript == null) return;

        GUILayout.BeginArea(new Rect(10, 100, 300, 200));
        GUILayout.Label("=== ENEMY DEBUG ===");
        GUILayout.Label($"Attack State: {enemyScript._plyAtq}");
        GUILayout.Label($"Player Visible: {enemyScript._playerVisible}");
        GUILayout.Label($"Death Triggered: {hasTriggeredDeath}");

        if (enemyScript._p != null)
        {
            float distance = Vector3.Distance(transform.position, enemyScript._p.transform.position);
            GUILayout.Label($"Distance to Player: {distance:F2}");
        }

        if (GUILayout.Button("Force Death Test"))
        {
            ForceDeathTest();
        }

        if (GUILayout.Button("Reset Death Trigger"))
        {
            ResetDeathTrigger();
        }

        GUILayout.EndArea();
    }
}
