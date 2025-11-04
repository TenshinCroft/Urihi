using UnityEngine;

public class ChaseMusicController : MonoBehaviour
{
    [Header("Configura��o do �udio")]
    public AudioSource chaseMusic; // m�sica de persegui��o
    public bool canPlayMusic = false; // s� habilita ap�s evento 2

    private bool isPlaying = false;
    private bool forcedStart = false;
    private int chasingEnemiesCount = 0;

    public static ChaseMusicController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartChase()
    {
        if (!canPlayMusic)
        {
            Debug.LogWarning("StartChase called but canPlayMusic is FALSE. Music will not play until EnableMusicAfterEvent2() is called.");
            return;
        }

        forcedStart = false;
        chasingEnemiesCount++;

        if (!isPlaying && chaseMusic != null)
        {
            chaseMusic.Play();
            isPlaying = true;
            Debug.Log($"Chase music started (chasing enemies: {chasingEnemiesCount})");
        }
        else if (isPlaying)
        {
            Debug.Log($"Chase music already playing (chasing enemies: {chasingEnemiesCount})");
        }
    }

    public void StartChaseForced()
    {
        if (!canPlayMusic) return;

        forcedStart = true;
        if (!isPlaying && chaseMusic != null)
        {
            chaseMusic.Play();
            isPlaying = true;
        }
    }

    public void StopChase()
    {
        Debug.Log($"StopChase called. forcedStart: {forcedStart}, chasingEnemiesCount before: {chasingEnemiesCount}");
        
        if (forcedStart)
        {
            Debug.Log("StopChase blocked by forcedStart");
            return;
        }

        chasingEnemiesCount--;
        if (chasingEnemiesCount < 0) chasingEnemiesCount = 0;

        Debug.Log($"chasingEnemiesCount after decrement: {chasingEnemiesCount}");

        if (chasingEnemiesCount == 0)
        {
            if (chaseMusic != null && chaseMusic.isPlaying)
            {
                chaseMusic.Stop();
                chaseMusic.time = 0f;
                isPlaying = false;
                Debug.Log("Chase music stopped (no enemies chasing)");
            }
            else if (isPlaying)
            {
                isPlaying = false;
                Debug.Log("Chase music flag reset (no enemies chasing)");
            }
            else
            {
                Debug.Log("Chase music was not playing");
            }
        }
        else
        {
            Debug.Log($"Chase music continues (still {chasingEnemiesCount} enemies chasing)");
        }
    }

    public void StopChaseForced()
    {
        forcedStart = false;
        chasingEnemiesCount = 0;
        if (chaseMusic != null && chaseMusic.isPlaying)
        {
            chaseMusic.Stop();
            chaseMusic.time = 0f;
            isPlaying = false;
        }
    }

    public void EnableMusicAfterEvent2()
    {
        canPlayMusic = true;
    }
}
